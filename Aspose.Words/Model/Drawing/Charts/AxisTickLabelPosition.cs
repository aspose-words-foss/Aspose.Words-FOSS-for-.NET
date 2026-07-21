// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Specifies the possible positions for tick labels.
    /// </summary>
    public enum AxisTickLabelPosition
    {
        // Corresponds ST_TickLblPos simple type (5.7.3.48).

        /// <summary>
        /// Specifies the axis labels shall be at the high end of the perpendicular axis.
        /// </summary>
        High,

        /// <summary>
        /// Specifies the axis labels shall be at the low end of the perpendicular axis.
        /// </summary>
        Low,

        /// <summary>
        /// Specifies the axis labels shall be next to the axis.
        /// </summary>
        NextToAxis,

        /// <summary>
        /// Specifies the axis labels are not drawn.
        /// </summary>
        None,

        /// <summary>
        /// Specifies default value of tick labels position.
        /// </summary>
        Default = NextToAxis
    }
}
