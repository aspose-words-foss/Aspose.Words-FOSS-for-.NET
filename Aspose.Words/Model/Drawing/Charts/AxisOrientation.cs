// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// This enum specifies the possible ways to place a picture on a data point, series, wall, or floor.
    /// </summary>
    internal enum AxisOrientation
    {
        // Corresponds ST_Orientation simple type (5.7.3.30).

        /// <summary>
        /// Specifies that the values on the axis shall be reversed so they go from maximum to minimum.
        /// </summary>
        MaxMin,

        /// <summary>
        /// Specifies that the axis values shall be in the usual order, minimum to maximum.
        /// </summary>
        MinMax
    }
}
