// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2017 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// Represents values of the 2.24.4.12 ST_NumericDimensionType and 2.24.4.20 ST_StringDimensionType 
    /// types [MS-ODAWXML].
    /// </summary>
    internal enum DimensionType
    {
        /// <summary>
        /// The dimension is a value.
        /// </summary>
        Value,
        /// <summary>
        /// The dimension is an x-coordinate.
        /// </summary>
        X,
        /// <summary>
        /// The dimension is an y-coordinate.
        /// </summary>
        Y,
        /// <summary>
        /// The dimension is a size.
        /// </summary>
        Size,
        /// <summary>
        /// The dimension is a category.
        /// </summary>
        Category,
        /// <summary>
        /// The dimension is a value that is used to generate a color in a map chart.
        /// </summary>
        ColorValue
    }
}
