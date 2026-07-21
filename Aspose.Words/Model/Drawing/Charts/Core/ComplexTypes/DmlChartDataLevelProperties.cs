// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/02/2017 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents properties of a dimension data level.
    /// (Stores attributes of the CT_NumericLevel or CT_StringLevel complex types [MS-ODRAWXML].)
    /// </summary>
    internal class DmlChartDataLevelProperties
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartDataLevelProperties Clone()
        {
            return (DmlChartDataLevelProperties)MemberwiseClone();
        }

        /// <summary>
        /// Gets or sets the number of data values.
        /// </summary>
        internal int ValueCount { get; set; }

        /// <summary>
        /// Gets or sets any custom information of data values formatting.
        /// </summary>
        internal string FormatCode { get; set; }

        /// <summary>
        /// Gets or sets name of this level of dimension data.
        /// </summary>
        internal string Name { get; set; }
    }
}
