// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Specifies the possible crossing points for an axis.
    /// </summary>
    public enum AxisCrosses
    {
        // Corresponds ST_Crosses simple type (5.7.3.8).
        
        /// <summary>
        /// The category axis crosses at the zero point of the value axis (if possible), or at the minimum value
        /// if the minimum is greater than zero, or at the maximum if the maximum is less than zero.
        /// </summary>
        Automatic,

        /// <summary>
        /// A perpendicular axis crosses at the maximum value of the axis.
        /// </summary>
        Maximum,
        
        /// <summary>
        /// A perpendicular axis crosses at the minimum value of the axis.
        /// </summary>
        Minimum,

        /// <summary>
        /// A perpendicular axis crosses at the specified value of the axis.
        /// </summary>
        Custom
    }
}
