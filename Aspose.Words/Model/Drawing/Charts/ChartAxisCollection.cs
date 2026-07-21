// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2023 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a collection of chart axes.
    /// </summary>
    public class ChartAxisCollection : IEnumerable<ChartAxis>
    {
        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        internal ChartAxisCollection(DmlChartSpace chartSpace)
        {
            mChartSpace = chartSpace;
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartAxis> GetEnumerator()
        {
            return Axes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the axis at the specified index.
        /// </summary>
        public ChartAxis this[int index]
        {
            get { return Axes[index]; }
        }

        /// <summary>
        /// Gets the number of axes in this collection.
        /// </summary>
        public int Count
        {
            get { return Axes.Count; }
        }

        /// <summary>
        /// Gets the underlying list of the axes.
        /// </summary>
        private IList<ChartAxis> Axes
        {
            get { return mChartSpace.ChartFormat.PlotArea.Axes; }
        }

        private readonly DmlChartSpace mChartSpace;
    }
}
