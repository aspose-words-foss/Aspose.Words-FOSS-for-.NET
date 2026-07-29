// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2017 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Contains list of attributes of the 2.24.3.7 CT_Binning type [MS-ODRAWXML].
    /// </summary>
    internal enum DmlChartBinningAttr
    {
        /// <summary>
        /// Specifies the binning by bin size.
        /// Property data type is <b>double</b>.
        /// </summary>
        BinSize,

        /// <summary>
        /// Specifies the binning by bin count.
        /// Property data type is <b>int</b>.
        /// </summary>
        BinCount,

        /// <summary>
        /// Specifies the interval closed side.
        /// Property data type is <see cref="IntervalClosedSide"/>.
        /// </summary>
        IntervalClosed,

        /// <summary>
        /// Specifies the custom value for underflow bin, or an automatic value.
        /// Property data type is <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        Underflow,

        /// <summary>
        /// Specifies the custom value for overflow bin, or an automatic value.
        /// Property data type is <see cref="DoubleOrAutomatic"/>.
        /// </summary>
        Overflow
    }
}
