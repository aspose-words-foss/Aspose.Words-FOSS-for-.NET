// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2017 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the layout type for region labels of a geospatial series.
    /// Corresponds to the 2.24.4.17 ST_RegionLabelLayout simple type [MS-ODRAWXML].
    /// </summary>
    internal enum RegionLabelLayout
    {
        /// <summary>
        /// Specifies that no region labels appear in a geospatial series.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that region labels only appear if they can fit in their respective containing geometries
        /// in a geospatial series.
        /// </summary>
        BestFitOnly,

        /// <summary>
        /// Specifies that all region labels appear.
        /// </summary>
        ShowAll
    }
}
