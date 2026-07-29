// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2020 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Contains methods to get number of chart data points and data labels and their indexes.
    /// </summary>
    internal class ChartSeriesHelper
    {
        internal ChartSeriesHelper(ChartSeries series)
        {
            mSeries = series;
        }

        /// <summary>
        /// Gets label indexes that are displayed by default by MS Word with the current data.
        /// </summary>
        /// <remarks>
        /// In some chart types of Word 2016, depending on chart data and series options, some label indexes are unused,
        /// a label with such index is not displayed/created by MS Word/VBA. Although, if a label with unused index is
        /// explicitly defined, MS Word e.g. may display it as duplicate of another label. It seems MS Word preserves
        /// explicitly defined labels with unused indexes on re-saving.
        /// This method returns used label indexes. Indexes of deleted/hidden labels are included too.
        /// The returned indexes are sorted in ascending order.
        /// </remarks>
        internal IntList GetLabelIndexesDeterminedByData()
        {
            if (mSeries == null)
                return null;

            if (mSeries.Chart.ChartType != DmlChartType.ChartExChart)
                return null;

            // It seems only charts of the following two Word 2016 types can have unused label indexes.
            switch (mSeries.LayoutType)
            {
                case SeriesLayout.BoxWhisker:
                {
                    // It seems the behaviour of MS Word is more complex than the implemented below and depends on chart
                    // size: labels that are too near to other ones are not displayed. Skip it for now.

                    // See, for example, https://www150.statcan.gc.ca/n1/edu/power-pouvoir/ch12/5214889-eng.htm for
                    // naming of elements of a Box and Whisker plot.

                    Debug.Assert(mSeries.X.DimensionType == DimensionType.Category);
                    Debug.Assert(mSeries.Y.DimensionType == DimensionType.Value);

                    IntList indexes = new IntList();

                    List<BoxAndWhiskerPlot> items = mSeries.DataAnalyzer.GetBoxAndWhiskerPlots(mSeries.X.Values,
                        mSeries.Y.Values, mSeries.LayoutPr.QuartileMethod);
                    int categoryFirstLabelIndex = 0;

                    foreach (BoxAndWhiskerPlot item in items)
                    {
                        FillBoxAndWhiskerPlotLabelIndexes(item, categoryFirstLabelIndex, indexes);
                        categoryFirstLabelIndex += item.Values.Length + 4;
                    }

                    return indexes;
                }
                case SeriesLayout.Treemap:
                {
                    Debug.Assert(mSeries.X.DimensionType == DimensionType.Category);

                    return GetTreemapLabelIndexes();
                }
                default:
                    return null;
            }
        }

        private void FillBoxAndWhiskerPlotLabelIndexes(BoxAndWhiskerPlot item, int categoryFirstLabelIndex, IntList indexes)
        {
            DmlChartSeriesLayoutPr layoutPr = mSeries.LayoutPr;

            // Indexes of lower outlier labels.
            if (layoutPr.IsOutliersVisible)
                FillLowerOutlierLabelIndexes(item, categoryFirstLabelIndex, indexes);

            // Index of lower whisker/extreme label.
            if (MathUtil.IsLessOrEqual(item.LowerExtreme, item.LowerQuartile))
                indexes.Add(categoryFirstLabelIndex + item.LowerOutlierCount);

            // Indexes of non-outlier labels.
            if (layoutPr.IsNonOutliersVisible)
                FillNonOutlierLabelIndexes(item, categoryFirstLabelIndex, indexes);

            int valueCount = item.Values.Length;

            // Index of upper whisker/extreme label.
            if ((valueCount > 1) && MathUtil.IsGreaterOrEqual(item.UpperExtreme, item.UpperQuartile))
                indexes.Add(categoryFirstLabelIndex + valueCount - item.UpperOutlierCount - 1);

            // Indexes of upper outlier labels.
            if (layoutPr.IsOutliersVisible)
                FillUpperOutlierLabelIndexes(item, categoryFirstLabelIndex, indexes);

            int additionalLabelsFirstIndex = categoryFirstLabelIndex + valueCount;

            // Indexes of lower quantile, median, upper quantile labels.
            indexes.Add(additionalLabelsFirstIndex);
            indexes.Add(additionalLabelsFirstIndex + 1);
            indexes.Add(additionalLabelsFirstIndex + 2);

            // Index of mean label.
            if (layoutPr.IsMeanMarkerVisible)
                indexes.Add(additionalLabelsFirstIndex + 3);
        }

        private static void FillLowerOutlierLabelIndexes(BoxAndWhiskerPlot plot, int categoryFirstLabelIndex,
            IntList indexes)
        {
            for (int i = 0; i < plot.LowerOutlierCount; i++)
            {
                // Skip duplicates.
                if ((i == 0) || !MathUtil.AreEqual(plot.Values[i], plot.Values[i - 1]))
                    indexes.Add(categoryFirstLabelIndex + i);
            }
        }

        private static void FillNonOutlierLabelIndexes(BoxAndWhiskerPlot plot, int categoryFirstLabelIndex,
            IntList indexes)
        {
            // The extremes are not included into the cycle.
            for (int i = plot.LowerOutlierCount + 1; i < plot.Values.Length - plot.UpperOutlierCount - 1; i++)
            {
                // Skip duplicates.
                if (!MathUtil.AreEqual(plot.Values[i], plot.Values[i - 1]))
                    indexes.Add(categoryFirstLabelIndex + i);
            }
        }

        private static void FillUpperOutlierLabelIndexes(BoxAndWhiskerPlot item, int categoryFirstLabelIndex,
            IntList indexes)
        {
            int valueCount = item.Values.Length;

            for (int i = valueCount - item.UpperOutlierCount; i < valueCount; i++)
            {
                // Skip duplicates.
                if (!MathUtil.AreEqual(item.Values[i], item.Values[i - 1]))
                    indexes.Add(categoryFirstLabelIndex + i);
            }
        }

        /// <summary>
        /// Gets the number of data labels 1) that can be displayed for the parent series and 2) that are explicitly
        /// defined. Returns <b>0</b> if the property <see cref="ChartSeries.HasDataLabels"/> is <c>false</c>.
        /// </summary>
        internal ItemCountInfo GetDataLabelCount()
        {
            // VBA does not count deleted/hidden labels of Word 2016 charts, but counts for old charts. This is determined
            // by differences in the way that such labels are represented in format. This different behaviour for different
            // chart types looks as not correct, so let's always count them for now.

            if ((mSeries == null) || (mSeries.Chart == null) || !mSeries.HasDataLabels)
                return new ItemCountInfo(0);

            if (mSeries.Chart.ChartType == DmlChartType.ChartExChart)
                return GetChartExDataLabelCount();

            int count = mSeries.ValueCount;

            // OfPieChart contains additional label that represents sum of two smallest values.
            if ((mSeries.Chart.ChartType == DmlChartType.OfPieChart) && (count > 2))
                count++;

            return new ItemCountInfo(count);
        }

        /// <summary>
        /// Gets the number of data points that can be displayed for the parent series.
        /// </summary>
        internal int GetDataPointCount()
        {
            if ((mSeries == null) || (mSeries.Chart == null))
                return 0;

            if (mSeries.Chart.ChartType == DmlChartType.ChartExChart)
                return GetChartExDataPointCount();

            int count = mSeries.ValueCount;

            // OfPieChart contains additional point that represents sum of two smallest values.
            return ((mSeries.Chart.ChartType == DmlChartType.OfPieChart) && (count > 2)) ? count + 1 : count;
        }

        /// <summary>
        /// Gets indexes of data labels of the parent Word 2016 treemap chart series.
        /// </summary>
        [JavaAttributes.JavaGenericArguments("ArrayList<DmlChartDataAnalyzer.Category>")]
        private IntList GetTreemapLabelIndexes()
        {
            IntList indexes = new IntList();
            const int stemLevel = 1;
            List<DmlChartDataAnalyzer.Category> categories = mSeries.DataAnalyzer.GetCategories(mSeries.X.Values);

            // Stem labels are not shown in a Treemap charts: exclude their indexes.
            for (int index = 0; index < categories.Count; index++)
            {
                if (categories[index].LevelIndex != stemLevel)
                    indexes.Add(index);
            }

            return indexes;
        }

        /// <summary>
        /// Returns data label count of a <see cref="DmlChartType.ChartExChart"/> chart.
        /// </summary>
        /// <remarks>
        /// The chart types before Word 2016 have count of data labels equal to number of data items (except
        /// <see cref="DmlChartType.OfPieChart"/> chart in which there is one additional label). In some charts of Word
        /// 2016 label count is different than data item count and depends on data or on series options.
        /// </remarks>
        [JavaAttributes.JavaGenericArguments("ArrayList<DmlChartDataAnalyzer.Category>")]
        private ItemCountInfo GetChartExDataLabelCount()
        {
            int count;
            int lastUsedLabelIndex;

            switch (mSeries.LayoutType)
            {
                case SeriesLayout.BoxWhisker:
                case SeriesLayout.Treemap:
                {
                    IntList labelIndexes = GetLabelIndexesDeterminedByData();
                    Debug.Assert(labelIndexes != null);

                    lastUsedLabelIndex = labelIndexes[labelIndexes.Count - 1];
                    count = labelIndexes.Count;

                    // Calc labels that have unused indexes but have non-default formatting.
                    foreach (ChartDataLabel label in mSeries.DataLabels.MaterializedDataLabels)
                    {
                        if ((labelIndexes.BinarySearch(label.Index) < 0) && label.HasNonDefaultFormatting)
                            count++;
                    }

                    break;
                }
                default:
                {
                    // For other chart types, number of labels equals to number of data points.
                    count = GetChartExDataPointCount();
                    lastUsedLabelIndex = count - 1;
                    break;
                }
            }

            return new ItemCountInfo(count, lastUsedLabelIndex);
        }

        /// <summary>
        /// Returns data point count of a <see cref="DmlChartType.ChartExChart"/> chart.
        /// </summary>
        /// <remarks>
        /// The chart types before Word 2016 have count of data points equal to number of data items (except
        /// <see cref="DmlChartType.OfPieChart"/> chart in which there is one additional point). In some charts of Word
        /// 2016 point count is different than data item count and depends on data.
        /// </remarks>
        [JavaAttributes.JavaGenericArguments("ArrayList<DmlChartDataAnalyzer.Category>")]
        private int GetChartExDataPointCount()
        {
            switch (mSeries.LayoutType)
            {
                case SeriesLayout.BoxWhisker:
                {
                    int categoryCount = mSeries.DataAnalyzer.GetUniqueCategoryCount(mSeries.X.Values, true);
                    return mSeries.X.Values.LastNonEmptyValueIndex + 1 + categoryCount * BoxWhiskerSpecialPointCount;
                }
                case SeriesLayout.Treemap:
                {
                    List<DmlChartDataAnalyzer.Category> categories =
                        mSeries.DataAnalyzer.GetCategories(mSeries.X.Values);
                    return categories.Count;
                }
                case SeriesLayout.ClusteredColumn:
                {
                    if (mSeries.LayoutPr.IsAggregation)
                    {
                        // Pareto chart
                        Debug.Assert(mSeries.X.DimensionType == DimensionType.Category);
                        return mSeries.DataAnalyzer.GetUniqueCategoryCount(mSeries.X.Values, false);
                    }
                    else
                    {
                        // Histogram chart
                        return GetHistogramDataPointCount();
                    }
                }
                case SeriesLayout.ParetoLine:
                    // It seems it is not possible to display data labels for a pareto line in MS Word.
                    return 0;
                case SeriesLayout.Sunburst:
                {
                    Debug.Assert(mSeries.X.DimensionType == DimensionType.Category);
                    List<DmlChartDataAnalyzer.Category> categories =
                        mSeries.DataAnalyzer.GetCategories(mSeries.X.Values);
                    return categories.Count;
                }
                case SeriesLayout.Funnel:
                case SeriesLayout.RegionMap:
                case SeriesLayout.Waterfall:
                    return mSeries.ValueCount;
                default:
                {
                    Debug.Assert(false, "Unknown series layout type.");
                    return mSeries.ValueCount;
                }
            }
        }

        /// <summary>
        /// Calculates number of data points of the parent Word 2016 histogram chart series.
        /// </summary>
        private int GetHistogramDataPointCount()
        {
            DmlChartBinningPr binning = mSeries.LayoutPr.Binning;

            if (binning.BinCount > 0)
            {
                return binning.BinCount;
            }
            else if (!double.IsNaN(binning.BinSize))
            {
                Debug.Assert(mSeries.X.DimensionType == DimensionType.Value);

                DmlChartDataAnalyzer.ValueRange range = mSeries.DataAnalyzer.GetValueRange(mSeries.X.Values);
                if (range == null)
                    return 0;

                double size = System.Math.Abs(binning.BinSize);
                int additionalCount = 0;

                double min = range.Start;
                if (!binning.Underflow.IsNullOrAuto)
                {
                    min = binning.Underflow.Value;
                    additionalCount++;
                }

                double max = range.End;
                if (!binning.Overflow.IsNullOrAuto)
                {
                    max = binning.Overflow.Value;
                    additionalCount++;
                }

                if (max < min)
                    max = min;

                // Calc with rounding to greater value.
                return (int)System.Math.Ceiling((max - min) / size) + additionalCount;
            }
            else
            {
                // If bins are not defined explicitly, MS Word uses some algorithm of calculation high
                // bound and bin size, we just return max value got experimentally for now.
                const int maxAutomaticBinCount = 11;
                return maxAutomaticBinCount;
            }
        }

        private readonly ChartSeries mSeries;

        internal class ItemCountInfo
        {
            internal ItemCountInfo(int count)
                : this(count, count - 1)
            {
            }

            internal ItemCountInfo(int count, int lastUsedIndex)
            {
                Count = count;
                LastUsedIndex = lastUsedIndex;
            }

            internal int Count { get; private set; }

            internal int LastUsedIndex { get; private set; }
        }

        /// <summary>
        /// Gets a number of special data points/labels of a category in a Box and Whisker chart. The data points/labels
        /// are: lower quantile, median, upper quantile, mean.
        /// </summary>
        internal const int BoxWhiskerSpecialPointCount = 4;
    }
}
