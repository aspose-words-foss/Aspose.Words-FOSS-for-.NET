// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents line 3d chart.
    /// 5.7.2.97 line3DChart (3D Line Charts)
    /// </summary>
    internal class DmlLine3DChart : DmlLineChart, IDmlChart3D
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.Line3DChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Line3D; }
        }

        /// <summary>
        /// Returns id of the Z-axis. If the z-axis is not specified, -1 is returned.
        /// </summary>
        /// <remarks>
        /// 5.7.2.9 The possible values for id are defined by the XML Schema unsignedInt datatype. 
        /// The value space of xsd:unsignedInt is the range of integers between 0 and 4294967295
        /// </remarks>
        public int AxIdZ
        {
            get
            {
                object axIdZ = ChartPr.GetProperty(DmlChartAttrs.AxIdZ);
                return axIdZ != null ? (int)axIdZ : -1;
            }
        }

        public ChartAxis AxZ
        {
            get { return PlotArea.GetAxis(AxIdZ); }
        }

        internal int GapDepth
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.GapDepth); }
            set { ChartPr.SetProperty(DmlChartAttrs.GapDepth, value); }
        }
    }
}
