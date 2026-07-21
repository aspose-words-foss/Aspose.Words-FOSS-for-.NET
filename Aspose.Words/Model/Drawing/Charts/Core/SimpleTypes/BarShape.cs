// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the possible shapes for a 3-D data marker.
    /// Corresponds ST_Shape simple type (5.7.3.42).
    /// </summary>
    internal enum BarShape
    {
        /// <summary>
        /// Specifies the chart shall be drawn with a box shape.
        /// </summary>
        Box,
        /// <summary>
        /// Specifies the chart shall be drawn as a cone, with the base of the cone 
        /// on the floor and the point of the cone at the top of the data marker.
        /// </summary>
        Cone,
        /// <summary>
        /// Specifies the chart shall be drawn with truncated cones such that the point 
        /// of the cone would be the maximum data value.
        /// </summary>
        ConeToMax,
        /// <summary>
        /// Specifies the chart shall be drawn as a cylinder.
        /// </summary>
        Cylinder,
        /// <summary>
        /// Specifies the chart shall be drawn as a rectangular pyramid, 
        /// with the base of the pyramid on the floor and the point of the pyramid at the top of the data marker.
        /// </summary>
        Pyramid,
        /// <summary>
        /// Specifies the chart shall be drawn with truncated cones such that the point 
        /// of the cone would be the maximum data value.
        /// </summary>
        PyramidToMax,
        /// <summary>
        /// Synthetic value that indicates value is not set.
        /// </summary>
        None
    }
}
