// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the possible ways to split a pie of pie or bar of pie chart.
    /// Corresponds ST_SplitType simple type (5.7.3.45).
    /// </summary>
    internal enum SplitType
    {
        /// <summary>
        /// Specifies the data points shall be split using the default mechanism for this chart type.
        /// </summary>
        Auto,
        /// <summary>
        /// Specifies the data points shall be split between the pie 
        /// and the second chart according to the Custom Split values.
        /// </summary>
        Custom,
        /// <summary>
        /// Specifies the data points shall be split between the pie and the second chart by putting the points 
        /// with percentage less than Split Position percent in the second chart.
        /// </summary>
        Percentage,
        /// <summary>
        /// Specifies the data points shall be split between the pie and the second chart by putting 
        /// the last Split Position of the data points in the second chart
        /// </summary>
        Position,
        /// <summary>
        /// Specifies the data points shall be split between the pie and the second chart by putting 
        /// the data points with value less than Split Position in the second chart
        /// </summary>
        Value
    }
}
