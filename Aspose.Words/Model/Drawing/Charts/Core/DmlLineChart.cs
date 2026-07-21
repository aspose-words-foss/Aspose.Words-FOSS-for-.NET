// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents line chart.
    /// 5.7.2.98 lineChart (Line Charts).
    /// </summary>
    internal class DmlLineChart : DmlCartesianPlaneChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.LineChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get
            {
                switch (ChartPr.Grouping)
                {
                    case Grouping.Stacked:
                        return ChartSeriesType.LineStacked;
                    case Grouping.PercentStacked:
                        return ChartSeriesType.LinePercentStacked;
                    default:
                        return ChartSeriesType.Line;
                }
            }
        }

        internal DmlChartSpPr DropLines
        {
            get { return (DmlChartSpPr)ChartPr.GetProperty(DmlChartAttrs.DropLines); }
        }

        internal bool IsDropLinesVisible
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.IsDropLinesVisible); }
        }

        internal Grouping Grouping
        {
            get { return ChartPr.Grouping; }
            set { ChartPr.Grouping = value; }
        }

        internal DmlChartSpPr HiLowLines
        {
            get { return (DmlChartSpPr)ChartPr.GetProperty(DmlChartAttrs.HiLowLines); }
        }

        internal bool IsHiLowLinesVisible
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.IsHiLowLinesVisible); }
        }

        internal bool ShowMarker
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.ShowMarker); }
            set { ChartPr.SetProperty(DmlChartAttrs.ShowMarker, value); }
        }

        internal bool Smooth
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.Smooth); }
            set { ChartPr.SetProperty(DmlChartAttrs.Smooth, value); }
        }

        internal DmlChartUpDownBars UpDownBars
        {
            get { return (DmlChartUpDownBars)ChartPr.GetProperty(DmlChartAttrs.UpDownBars); }
        }
    }
}
