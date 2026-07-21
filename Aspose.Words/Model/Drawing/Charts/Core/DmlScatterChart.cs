// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents scatter chart.
    /// 5.7.2.162 scatterChart (Scatter Charts).
    /// </summary>
    internal class DmlScatterChart : DmlCartesianPlaneChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.ScatterChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Scatter; }
        }

        internal ScatterStyle ScatterStyle
        {
            get { return (ScatterStyle)ChartPr.GetProperty(DmlChartAttrs.ScatterStyle); }
            set { ChartPr.SetProperty(DmlChartAttrs.ScatterStyle, value); }
        }

        /// <summary>
        /// Returns true if line must be rendered for the chart.
        /// </summary>
        internal bool ShowLine
        {
            get
            {
                switch (ScatterStyle)
                {
                    case ScatterStyle.Marker:
                    case ScatterStyle.None:
                        return false;
                    case ScatterStyle.Line:
                    case ScatterStyle.Smooth:
                    case ScatterStyle.LineMarker:
                    case ScatterStyle.SmoothMarker:
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// Returns true if markers must be rendered for the chart.
        /// </summary>
        internal bool ShowMarker
        {
            get
            {
                switch (ScatterStyle)
                {
                    case ScatterStyle.Marker:
                    case ScatterStyle.LineMarker:
                    case ScatterStyle.SmoothMarker:
                        return true;
                    case ScatterStyle.Line:
                    case ScatterStyle.Smooth:
                    case ScatterStyle.None:
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns true if points must be connected with smooth lines for the chart.
        /// </summary>
        internal bool Smooth
        {
            get
            {
                switch (ScatterStyle)
                {
                    case ScatterStyle.SmoothMarker:
                    case ScatterStyle.Smooth:
                        return true;
                    case ScatterStyle.Marker:
                    case ScatterStyle.LineMarker:
                    case ScatterStyle.Line:
                    case ScatterStyle.None:
                    default:
                        return false;
                }
            }
        }
    }
}
