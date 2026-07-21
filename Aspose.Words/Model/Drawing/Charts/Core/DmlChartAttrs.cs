// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// List of chart attributes.
    /// </summary>
    internal enum DmlChartAttrs
    {
        /// <summary>
        /// Id of X axis.
        /// Integer.
        /// </summary>
        AxIdX,

        /// <summary>
        /// Id of Y axis.
        /// Integer.
        /// </summary>
        AxIdY,
        
        /// <summary>
        /// Id of Z axis (for 3D charts).
        /// Integer.
        /// </summary>
        AxIdZ,

        /// <summary>
        /// Specifies the settings for the data labels for the entire chart.
        /// <see cref="ChartDataLabelCollection"/>.
        /// </summary>
        DLbls,

        /// <summary>
        /// Specifies drop lines formatting.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        DropLines,

        /// <summary>
        /// Specifies that drop lines are visible.
        /// Has effect only for Line, Area and Stock charts.
        /// Boolean.
        /// </summary>
        IsDropLinesVisible,
        
        /// <summary>
        /// Specifies the high-low lines formatting for the series.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        HiLowLines,

        /// <summary>
        /// Specifies that high-low lines are visible.
        /// Has effect only for Line charts.
        /// Boolean.
        /// </summary>
        IsHiLowLinesVisible,

        /// <summary>
        /// Specifies series lines formatting for the chart.
        /// <see cref="DmlChartSpPr"/>.
        /// </summary>
        SerLines,

        /// <summary>
        /// Flag indicates whether series lines must be shown.
        /// </summary>
        ShowSerLine,

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// Integer [0;500].
        /// </summary>
        GapDepth,

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// Integer [0;500].
        /// </summary>
        GapWidth,

        /// <summary>
        /// Specifies the type of grouping for a column, line, bar, or area chart.
        /// <see cref="SimpleTypes.Grouping"/>.
        /// </summary>
        Grouping,

        /// <summary>
        /// Specifies list of series of the chart.
        /// IList{T} with items of <see cref="ChartSeries"/> type.
        /// </summary>
        Series,

        /// <summary>
        /// Specifies that each data marker in the series shall have a different color.
        /// Boolean.
        /// </summary>
        VaryColors,

        /// <summary>
        /// Specifies whether the series form a bar (horizontal) chart or a column (vertical) chart.
        /// <see cref="BarDirection"/>.
        /// </summary>
        BarDir,

        /// <summary>
        /// Specifies the shape of a 3-D bar chart.
        /// </summary>
        Shape,

        /// <summary>
        /// Specifies how much bars and columns shall overlap on 2-D charts.
        /// Integer [-100;100].
        /// </summary>
        Overlap,

        /// <summary>
        /// Specifies the scale factor for the bubble chart.
        /// Integer [0; 300].
        /// </summary>
        BubbleScale,

        /// <summary>
        /// Specifies negative sized bubbles shall be shown on a bubble chart.
        /// Boolean.
        /// </summary>
        ShowNegBubbles,

        /// <summary>
        /// Specifies how the bubble size values are represented on the chart.
        /// <see cref="SimpleTypes.SizeRepresents"/>.
        /// </summary>
        SizeRepresents,

        /// <summary>
        /// Specifies the angle of the first pie or doughnut chart slice, in degrees (clockwise from up).
        /// Integer [0;300].
        /// </summary>
        FirstSliceAng,

        /// <summary>
        /// Specifies the size of the hole in a doughnut chart.
        /// Integer [10;90].
        /// </summary>
        HoleSize,

        /// <summary>
        /// Specifies that the marker shall be shown.
        /// Boolean.
        /// </summary>
        ShowMarker,

        /// <summary>
        /// Specifies the line connecting the points on the chart shall be smoothed using Catmull-Rom splines.
        /// Boolean.
        /// </summary>
        Smooth,

        /// <summary>
        /// Specifies the up and down bars.
        /// <see cref="DmlChartUpDownBars"/>.
        /// </summary>
        UpDownBars,

        /// <summary>
        /// Contains the custom split information for a pie-of-pie or bar-of-pie chart with a custom split type.
        /// Array of Integers.
        /// </summary>
        CustSplit,

        /// <summary>
        /// Specifies whether this chart is pie of pie or bar of pie.
        /// <see cref="SimpleTypes.OfPieType"/>.
        /// </summary>
        OfPieType,

        /// <summary>
        /// specifies the size of the second pie or bar of a pie of pie chart or a bar of pie chart, 
        /// as a percentage of the size of the first pie.
        /// Integer [5;200].
        /// </summary>
        SecondPieSize,

        /// <summary>
        /// Specifies a value that shall be used to determine which data points 
        /// are in the second pie or bar on a pie of pie or bar of pie chart.
        /// Double.
        /// </summary>
        SplitPos,

        /// <summary>
        /// Specifies how to determine which data points are in the second pie or bar on a pie of pie or bar of pie chart.
        /// <see cref="SimpleTypes.SplitType"/>.
        /// </summary>
        SplitType,

        /// <summary>
        /// Specifies what type of radar chart shall be drawn.
        /// <see cref="SimpleTypes.RadarStyle"/>.
        /// </summary>
        RadarStyle,

        /// <summary>
        /// Specifies the type of lines for the scatter chart.
        /// <see cref="SimpleTypes.ScatterStyle"/>.
        /// </summary>
        ScatterStyle,

        /// <summary>
        /// Contains a collection of formatting bands for a surface chart indexed from low to high.
        /// <see cref="DmlChartBandFormats"/>.
        /// </summary>
        BandFmts,

        /// <summary>
        /// Specifies the surface chart is drawn as a wireframe.
        /// Boolean.
        /// </summary>
        Wireframe,

        /// <summary>
        /// extLst (Chart Extensibility) This element contains tags used for future extensibility of the file format
        /// <see cref="StringToObjDictionary{T}"/>
        /// </summary>
        Extensions

    }
}
