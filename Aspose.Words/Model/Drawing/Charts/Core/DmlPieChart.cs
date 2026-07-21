// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents pie chart.
    /// 5.7.2.142 pieChart (Pie Charts).
    /// </summary>
    internal class DmlPieChart : DmlChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.PieChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Pie; }
        }

        internal override bool RenderLegendForDataPoints
        {
            get { return true; }
        }

        internal int FirstSliceAng
        {
            get { return UseZeroFirstSliceAng ? 0 : (int)ChartPr.GetProperty(DmlChartAttrs.FirstSliceAng); }
            set { ChartPr.SetProperty(DmlChartAttrs.FirstSliceAng, value); }
        }

        private bool UseZeroFirstSliceAng
        {
            get { return (ChartType == DmlChartType.Pie3DChart) || (ChartType == DmlChartType.OfPieChart); }
        }
    }
}
