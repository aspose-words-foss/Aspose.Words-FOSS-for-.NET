// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2015 by Andrey Noskov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Contains properties of a single data point on the chart.
    /// </summary>
    public interface IChartDataPoint
    {
        /// <summary>
        /// Specifies the amount the data point shall be moved from the center of the pie.
        /// Can be negative, negative means that property is not set and no explosion should be applied.
        /// Applies only to Pie charts.
        /// </summary>
        int Explosion { get; set; }

        /// <summary>
        /// Specifies whether the parent element shall inverts its colors if the value is negative.
        /// </summary>
        bool InvertIfNegative { get; set; }

        /// <summary>
        /// Specifies a data marker. Marker is automatically created when requested.
        /// </summary>
        ChartMarker Marker { get; }

        /// <summary>
        /// Specifies whether the bubbles in Bubble chart should have a 3-D effect applied to them.
        /// </summary>
        bool Bubble3D { get; set; }
    }
}
