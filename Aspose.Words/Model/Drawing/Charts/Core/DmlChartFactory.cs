// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using System;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Factory for creating DML charts.
    /// </summary>
    internal static class DmlChartFactory
    {
        internal static DmlChart CreateChart(DmlChartType chartType)
        {
            switch (chartType)
            {
                case DmlChartType.AreaChart:
                    return new DmlAreaChart();
                case DmlChartType.Area3DChart:
                    return new DmlArea3DChart();
                case DmlChartType.LineChart:
                    return new DmlLineChart();
                case DmlChartType.Line3DChart:
                    return new DmlLine3DChart();
                case DmlChartType.StockChart:
                    return new DmlStockChart();
                case DmlChartType.RadarChart:
                    return new DmlRadarChart();
                case DmlChartType.ScatterChart:
                    return new DmlScatterChart();
                case DmlChartType.PieChart:
                    return new DmlPieChart();
                case DmlChartType.Pie3DChart:
                    return new DmlPie3DChart();
                case DmlChartType.DoughnutChart:
                    return new DmlDoughnutChart();
                case DmlChartType.BarChart:
                    return new DmlBarChart();
                case DmlChartType.Bar3DChart:
                    return new DmlBar3DChart();
                case DmlChartType.OfPieChart:
                    return new DmlOfPieChart();
                case DmlChartType.SurfaceChart:
                    return new DmlSurfaceChart();
                case DmlChartType.Surface3DChart:
                    return new DmlSurface3DChart();
                case DmlChartType.BubbleChart:
                    return new DmlBubbleChart();
                default:
                    throw new ArgumentException("Unexpected chart type.");
            }
        }
    }
}
