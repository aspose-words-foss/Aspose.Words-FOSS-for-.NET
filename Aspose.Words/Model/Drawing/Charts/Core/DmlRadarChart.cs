// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents radar chart.
    /// 5.7.2.154 radarChart (Radar Charts)
    /// </summary>
    internal class DmlRadarChart : DmlCartesianPlaneChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.RadarChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Radar; }
        }

        internal RadarStyle RadarStyle
        {
            get { return (RadarStyle)ChartPr.GetProperty(DmlChartAttrs.RadarStyle); }
            set { ChartPr.SetProperty(DmlChartAttrs.RadarStyle, value); }
        }
    }
}
