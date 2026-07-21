// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents 2d bar chart.
    /// 5.7.2.16 barChart (Bar Charts).
    /// </summary>
    internal class DmlBarChart : DmlCartesianPlaneChart 
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.BarChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get
            {
                switch (ChartPr.BarDirection)
                {
                    case BarDirection.Bar:
                    {
                        switch (ChartPr.Grouping)
                        {
                            case Grouping.Stacked:
                                return ChartSeriesType.BarStacked;
                            case Grouping.PercentStacked:
                                return ChartSeriesType.BarPercentStacked;
                            default:
                                return ChartSeriesType.Bar;
                        }
                    }
                    case BarDirection.Column:
                    {
                        switch (ChartPr.Grouping)
                        {
                            case Grouping.Stacked:
                                return ChartSeriesType.ColumnStacked;
                            case Grouping.PercentStacked:
                                return ChartSeriesType.ColumnPercentStacked;
                            default:
                                return ChartSeriesType.Column;
                        }
                    }
                    default:
                        throw new InvalidOperationException("Unknown bar or column chart.");
                }
            }
        }

        internal BarDirection BarDir
        {
            get { return (BarDirection)ChartPr.GetProperty(DmlChartAttrs.BarDir); }
            set { ChartPr.SetProperty(DmlChartAttrs.BarDir, value); }
        }

        internal int GapWidth
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.GapWidth); }
            set { ChartPr.SetProperty(DmlChartAttrs.GapWidth, value); }
        }

        internal Grouping Grouping
        {
            get { return ChartPr.Grouping; }
            set { ChartPr.Grouping = value; }
        }

        internal int Overlap
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.Overlap); }
            set { ChartPr.SetProperty(DmlChartAttrs.Overlap, value); }
        }

        internal DmlChartSpPr SerLines
        {
            get { return (DmlChartSpPr)ChartPr.GetProperty(DmlChartAttrs.SerLines); }
        }
    }
}
