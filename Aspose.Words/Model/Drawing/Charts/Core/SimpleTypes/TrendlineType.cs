// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies all types of trendline which are available for series in a chart.
    /// Corresponds ST_TrendlineType simple type (5.7.3.51).
    /// </summary>
    internal enum TrendlineType
    {
        /// <summary>
        /// Specifies the trendline shall be an exponential curve in the form y=ab^x.
        /// </summary>
        Exponential,
        /// <summary>
        /// Specifies the trendline shall be a line in the form y=mx+b.
        /// </summary>
        Linear,
        /// <summary>
        /// Specifies the trendline shall be a logarithmic curve in the form y=a*log(x)+b, where log is the natural logarithm.
        /// </summary>
        Logarithmic,
        /// <summary>
        /// Specifies the trendline shall be a moving average of period Period.
        /// </summary>
        MovingAverage,
        /// <summary>
        /// Specifies the trendline shall be a polynomial curve of order Order in the form y=ax^6+bx^5+cx^4+dx^3+ex^2+fe+g.
        /// </summary>
        Polynomial,
        /// <summary>
        /// Specifies the trendline shall be a power curve in the form y=ax^b.
        /// </summary>
        Power
    }
}
