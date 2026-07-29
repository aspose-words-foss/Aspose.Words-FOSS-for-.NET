// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the possible ways to layout the plot area.
    /// Corresponds ST_LayoutTarget simple type (5.7.3.21).
    /// </summary>
    internal enum LayoutTarget
    {
        /// <summary>
        /// Specifies that the plot area size shall determine the size of the plot area, 
        /// not including the tick marks and axis labels.
        /// </summary>
        Inner,
        /// <summary>
        /// Specifies that the plot area size shall determine the size of the plot area, 
        /// the tick marks, and the axis labels.
        /// </summary>
        Outer
    }
}
