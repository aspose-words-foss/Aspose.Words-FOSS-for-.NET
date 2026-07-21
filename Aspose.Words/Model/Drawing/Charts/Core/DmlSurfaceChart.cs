// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents surface chart.
    /// 5.7.2.205 surfaceChart (Surface Charts)
    /// </summary>
    internal class DmlSurfaceChart : DmlCartesianPlaneChart, IDmlChart3D
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.SurfaceChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Surface; }
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

        internal DmlChartBandFormats BandFmts
        {
            get { return (DmlChartBandFormats)ChartPr.GetProperty(DmlChartAttrs.BandFmts); }
        }

        internal bool Wireframe
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.Wireframe); }
            set { ChartPr.SetProperty(DmlChartAttrs.Wireframe, value); }
        }
    }
}
