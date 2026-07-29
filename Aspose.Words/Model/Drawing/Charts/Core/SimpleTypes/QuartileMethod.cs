// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2017 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies quartile calculation methods.
    /// Corresponds to the 2.24.4.16 ST_QuartileMethod simple type [MS-ODRAWXML].
    /// </summary>
    internal enum QuartileMethod
    {
        /// <summary>
        /// The quartile calculation includes the median when splitting the dataset into quartiles.
        /// </summary>
        InclusiveMedian,

        /// <summary>
        /// The quartile calculation excludes the median when splitting the dataset into quartiles.
        /// </summary>
        ExclusiveMedian
    }
}
