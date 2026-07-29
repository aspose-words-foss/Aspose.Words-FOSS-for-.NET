// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents area chart element. 
    /// 5.7.2.5 areaChart (Area Charts).
    /// </summary>
    internal class DmlAreaChart : DmlCartesianPlaneChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.AreaChart; }
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
                        return ChartSeriesType.AreaStacked;
                    case Grouping.PercentStacked:
                        return ChartSeriesType.AreaPercentStacked;
                    default:
                        return ChartSeriesType.Area;
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
    }
}
