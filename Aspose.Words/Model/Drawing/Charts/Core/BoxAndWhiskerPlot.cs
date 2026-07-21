// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/12/2019 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents data for an item of Box and Whisker chart.
    /// </summary>
    internal class BoxAndWhiskerPlot
    {
        internal BoxAndWhiskerPlot(string category, double lowerExtreme, double lowerQuartile, double median,
            double upperQuartile, double upperExtreme, double mean, int lowerOutlierCount, int upperOutlierCount,
            double[] values)
        {
            Category = category;
            LowerExtreme = lowerExtreme;
            LowerQuartile = lowerQuartile;
            Median = median;
            UpperQuartile = upperQuartile;
            UpperExtreme = upperExtreme;
            Mean = mean;
            LowerOutlierCount = lowerOutlierCount;
            UpperOutlierCount = upperOutlierCount;
            Values = values;
        }

        /// <summary>
        /// Bound of the lower whisker.
        /// </summary>
        internal double LowerExtreme { get; private set; }

        /// <summary>
        /// Position of the lower quartile.
        /// </summary>
        internal double LowerQuartile { get; private set; }

        /// <summary>
        /// Median of input numerical data.
        /// </summary>
        internal double Median { get; private set; }

        /// <summary>
        /// Position of the upper quartile.
        /// </summary>
        internal double UpperQuartile { get; private set; }

        /// <summary>
        /// Bound of the upper whisker.
        /// </summary>
        internal double UpperExtreme { get; private set; }

        /// <summary>
        /// Arithmetical mean of input numerical data.
        /// </summary>
        internal double Mean { get; private set; }

        /// <summary>
        /// Number of data values that have value less than <see cref="LowerExtreme"/>. This values are plotted as an
        /// outliers in the chart.
        /// </summary>
        internal int LowerOutlierCount { get; private set; }

        /// <summary>
        /// Number of data values that have value larger than <see cref="UpperExtreme"/>. This values are plotted as an
        /// outliers in the chart.
        /// </summary>
        internal int UpperOutlierCount { get; private set; }

        /// <summary>
        /// Category name of this plot.
        /// </summary>
        internal string Category { get; private set; }

        /// <summary>
        /// Data values sorted in ascending order.
        /// </summary>
        internal double[] Values { get; private set; }
    }
}
