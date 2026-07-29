// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents pie 3d chart.
    /// 5.7.2.141 pie3DChart (3D Pie Charts).
    /// </summary>
    internal class DmlPie3DChart : DmlPieChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.Pie3DChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Pie3D; }
        }
    }
}
