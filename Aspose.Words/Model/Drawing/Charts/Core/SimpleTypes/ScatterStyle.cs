// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the possible styles of scatter chart.
    /// Corresponds ST_ScatterStyle simple type (5.7.3.40).
    /// </summary>
    internal enum ScatterStyle
    {
        /// <summary>
        /// Specifies the points on the scatter chart shall be connected with straight lines but markers shall not be drawn.
        /// </summary>
        Line,
        /// <summary>
        /// Specifies the points on the scatter chart shall be connected with straight lines and markers shall be drawn.
        /// </summary>
        LineMarker,
        /// <summary>
        /// Specifies the points on the scatter chart shall not be connected with lines and markers shall be drawn.
        /// </summary>
        Marker,
        /// <summary>
        /// Specifies the points on the scatter chart shall not be connected with straight lines and markers shall not be drawn.
        /// </summary>
        None,
        /// <summary>
        /// Specifies the points on the scatter chart shall be connected with smoothed lines and markers shall not be drawn.
        /// </summary>
        Smooth,
        /// <summary>
        /// Specifies the points on the scatter chart shall be connected with smoothed lines and markers shall be drawn.
        /// </summary>
        SmoothMarker
    }
}
