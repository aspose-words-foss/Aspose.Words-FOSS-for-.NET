// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents doughnut chart.
    /// 5.7.2.50 doughnutChart (Doughnut Charts).
    /// </summary>
    internal class DmlDoughnutChart : DmlPieChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.DoughnutChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Doughnut; }
        }

        internal int HoleSize
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.HoleSize); }
            set { ChartPr.SetProperty(DmlChartAttrs.HoleSize, value); }
        }
    }
}
