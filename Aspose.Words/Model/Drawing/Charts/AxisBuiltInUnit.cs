// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Specifies the display units for an axis.
    /// </summary>
    /// <dev>
    /// Corresponds to the ST_BuiltInUnit simple type (5.7.3.6).
    /// </dev>
    public enum AxisBuiltInUnit
    {
        /// <summary>
        /// Specifies the values on the chart shall displayed as is.
        /// </summary>
        None,
        /// <summary>
        /// Specifies the values on the chart shall be divided by a user-defined divisor. This value is not supported
        /// by the new chart types of MS Office 2016.
        /// </summary>
        Custom,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 1,000,000,000.
        /// </summary>
        Billions,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 100,000,000.
        /// </summary>
        HundredMillions,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 100.
        /// </summary>
        Hundreds,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 100,000.
        /// </summary>
        HundredThousands,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 1,000,000.
        /// </summary>
        Millions,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 10,000,000.
        /// </summary>
        TenMillions,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 10,000.
        /// </summary>
        TenThousands,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 1,000.
        /// </summary>
        Thousands,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 1,000,000,000,0000.
        /// </summary>
        Trillions,
        /// <summary>
        /// Specifies the values on the chart shall be divided by 0.01. This value is supported only by the new chart
        /// types of MS Office 2016.
        /// </summary>
        Percentage
    }
}
