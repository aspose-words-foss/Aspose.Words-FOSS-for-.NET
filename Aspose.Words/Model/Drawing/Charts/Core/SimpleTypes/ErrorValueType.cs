// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the possible ways to determine the length of the error bars
    /// Corresponds ST_ErrValType simple type (5.7.3.14).
    /// </summary>
    internal enum ErrorValueType
    {
        /// <summary>
        /// Specifies that the length of the error bars shall be determined by the Plus and Minus elements.
        /// </summary>
        CustomErrorBars,
        /// <summary>
        /// Specifies that the length of the error bars shall be the fixed value determined by Error Bar Value.
        /// </summary>
        FixedValue,
        /// <summary>
        /// Specifies that the length of the error bars shall be Error Bar Value percent of the data.
        /// </summary>
        Percentage,
        /// <summary>
        /// Specifies that the length of the error bars shall be Error Bar Value standard deviations of the data.
        /// </summary>
        StandardDeviation,
        /// <summary>
        /// Specifies that the length of the error bars shall be Error Bar Value standard errors of the data.
        /// </summary>
        StandardError
    }
}
