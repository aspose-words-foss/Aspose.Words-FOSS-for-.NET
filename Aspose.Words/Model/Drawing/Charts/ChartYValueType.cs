// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2022 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Allows to specify type of an Y value of a chart series.
    /// </summary>
    public enum ChartYValueType
    {
        /// <summary>
        /// Specifies that an Y value is a double-precision floating-point number.
        /// </summary>
        Double,

        /// <summary>
        /// Specifies that an Y value is a date and time of day.
        /// </summary>
        /// <dev>
        /// Values of this type are locale-specific in Word 2016 charts: correct datetime format must be specified to
        /// correctly display value/axis labels. See TestChartDataApi tests.
        /// And even if allowed, this value type may be not useful/correct for some chart types.
        /// </dev>
        DateTime,

        /// <summary>
        /// Specifies that an X value is a time of day.
        /// </summary>
        /// <dev>
        /// Values of this type are locale-specific in Word 2016 charts: correct time format must be specified to
        /// correctly display value/axis labels. See TestChartDataApi tests.
        /// And even if allowed, this value type may be not useful/correct for some chart types.
        /// </dev>
        Time
    }
}
