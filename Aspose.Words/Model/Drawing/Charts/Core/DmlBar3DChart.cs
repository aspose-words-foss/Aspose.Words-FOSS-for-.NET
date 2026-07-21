// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents 3d bar chart.
    /// 5.7.2.15 bar3DChart (3D Bar Charts).
    /// </summary>
    internal class DmlBar3DChart : DmlBarChart, IDmlChart3D
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.Bar3DChart; }
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
                                return ChartSeriesType.Bar3DStacked;
                            case Grouping.PercentStacked:
                                return ChartSeriesType.Bar3DPercentStacked;
                            default:
                                return ChartSeriesType.Bar3D;
                        }
                    }
                    case BarDirection.Column:
                    {
                        switch (ChartPr.Grouping)
                        {
                            case Grouping.Stacked:
                                return ChartSeriesType.Column3DStacked;
                            case Grouping.PercentStacked:
                                return ChartSeriesType.Column3DPercentStacked;
                            case Grouping.Clustered:
                                return ChartSeriesType.Column3DClustered;
                            default:
                                return ChartSeriesType.Column3D;
                        }
                    }
                    default:
                        throw new InvalidOperationException();
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

        internal Shape Shape
        {
            get { return (Shape)ChartPr.GetProperty(DmlChartAttrs.Shape); }
            set { ChartPr.SetProperty(DmlChartAttrs.Shape, value); }
        }
    }
}
