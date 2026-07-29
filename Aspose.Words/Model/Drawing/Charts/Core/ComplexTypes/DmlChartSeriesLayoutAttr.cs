// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2017 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Contains list of attributes of 2.24.3.72 CT_SeriesLayoutProperties [MS-ODRAWXML].
    /// </summary>
    internal enum DmlChartSeriesLayoutAttr
    {
        /// <summary>
        /// Specifies the layout type for the parent labels.
        /// Property data type is <see cref="SimpleTypes.ParentLabelLayout"/>.
        /// </summary>
        ParentLabelLayout,

        /// <summary>
        /// Specifies the layout type for region labels of a geospatial series.
        /// Property data type is <see cref="SimpleTypes.RegionLabelLayout"/>.
        /// </summary>
        RegionLabelLayout,

        /// <summary>
        /// Specifies the visibility of connector lines between data points.
        /// Property data type is <b>bool</b>.
        /// </summary>
        IsConnectorLinesVisible,

        /// <summary>
        /// Specifies the visibility of the line connecting all mean points.
        /// Property data type is <b>bool</b>.
        /// </summary>
        IsMeanLineVisible,

        /// <summary>
        /// Specifies the visibility of markers denoting the mean.
        /// Property data type is <b>bool</b>.
        /// </summary>
        IsMeanMarkerVisible,

        /// <summary>
        /// Specifies the visibility of non-outlier data points.
        /// Property data type is <b>bool</b>.
        /// </summary>
        IsNonOutliersVisible,

        /// <summary>
        /// Specifies the visibility of outlier data points.
        /// Property data type is <b>bool</b>.
        /// </summary>
        IsOutliersVisible,

        /// <summary>
        /// Specifies whether data aggregation is performed.
        /// Property data type is <b>bool</b>.
        /// </summary>
        IsAggregation,

        /// <summary>
        /// Specifies the data binning properties for the series.
        /// Property data type is <see cref="DmlChartBinningPr"/>.
        /// </summary>
        Binning,

        /// <summary>
        /// Specifies layout properties for a geospatial series.
        /// </summary>
        Geography,

        /// <summary>
        /// Specifies the quartile calculation method.
        /// Property data type is <see cref="SimpleTypes.QuartileMethod"/>.
        /// </summary>
        QuartileMethod,

        /// <summary>
        /// Specifies a list of subtotal data points.
        /// Property data type is <see cref="List{T}"/> of <b>int</b>.
        /// </summary>
        Subtotals,

        /// <summary>
        /// Specifies an extensibility container.
        /// Property data type is <see cref="StringToObjDictionary{TValue}"/>.
        /// </summary>
        Extensions
    }
}
