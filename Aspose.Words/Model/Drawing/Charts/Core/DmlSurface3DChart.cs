// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents surface 3d chart.
    /// 5.7.2.204 surface3DChart (3D Surface Charts)
    /// </summary>
    internal class DmlSurface3DChart : DmlSurfaceChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.Surface3DChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Surface3D; }
        }
    }
}
