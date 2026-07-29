// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents area 3d chart.
    /// 5.7.2.4 area3DChart (3D Area Charts)
    /// </summary>
    internal class DmlArea3DChart : DmlAreaChart, IDmlChart3D
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.Area3DChart; }
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
                    case SimpleTypes.Grouping.Stacked:
                        return ChartSeriesType.Area3DStacked;
                    case SimpleTypes.Grouping.PercentStacked:
                        return ChartSeriesType.Area3DPercentStacked;
                    default:
                        return ChartSeriesType.Area3D;
                }
            }
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
                return axIdZ != null? (int)axIdZ :-1;
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
