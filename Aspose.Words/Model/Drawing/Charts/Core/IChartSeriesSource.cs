// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2024 by Alexander Zhiltsov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Provides a list of chart series to be used in <see cref="ChartSeriesCollection"/> as a source.
    /// </summary>
    internal interface IChartSeriesSource
    {
        /// <summary>
        /// Removes a <see cref="ChartSeries"/> at the specified index.
        /// </summary>
        void RemoveAt(int index);

        /// <summary>
        /// Removes all <see cref="ChartSeries"/> from this series source.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a list of series.
        /// </summary>
        IList<ChartSeries> SeriesList { get; }

        /// <summary>
        /// Gets the DML chart to which adding series are placed.
        /// </summary>
        DmlChart DestinationChart { get; }
    }
}
