// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2012 by Alexey Noskov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents collection of a <see cref="ChartDataPoint"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartDataPointCollection : IEnumerable<ChartDataPoint>
    {
        internal ChartDataPointCollection(ChartSeries series)
        {
            mSeries = series;
        }

        /// <summary>
        /// Returns <see cref="ChartDataPoint"/> for the specified index.
        /// </summary>
        public ChartDataPoint this[int index]
        {
            get
            {
                return MaterializeItem(index);
            }
        }

        internal ChartDataPointCollection Clone()
        {
            ChartDataPointCollection lhs = new ChartDataPointCollection(mSeries);

            foreach (ChartDataPoint point in MaterializedDataPoints)
            {
                if (!point.HasNonDefaultFormatting)
                    continue;

                ChartDataPoint cloned = point.Clone();
                lhs.AddDataPoint(cloned);
            }

            return lhs;
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartDataPoint> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears format of all <see cref="ChartDataPoint"/> in this collection.
        /// </summary>
        public void ClearFormat()
        {
            // Do not clear the collection: let's keep currently stored data point objects.
            foreach (ChartDataPoint point in MaterializedDataPoints)
                point.ClearFormat();
        }

        /// <summary>
        /// Gets a flag indicating whether the data point at the specified index has default format.
        /// </summary>
        public bool HasDefaultFormat(int dataPointIndex)
        {
            ChartDataPoint point = mDataPoints[dataPointIndex];
            return (point == null) || !point.HasNonDefaultFormatting;
        }

        /// <summary>
        /// Copies format from the source data point to the destination data point.
        /// </summary>
        public void CopyFormat(int sourceIndex, int destinationIndex)
        {
            // The upper bounds of the indexes are currently not checked because we cannot calculate data point count
            // exactly for some chart types of MS Word 2016 (the same behavior as in the 'this' property).
            ArgumentUtil.CheckNonNegative(sourceIndex, "sourceIndex");
            ArgumentUtil.CheckNonNegative(destinationIndex, "destinationIndex");

            if (sourceIndex == destinationIndex)
                return;

            this[destinationIndex].CopyFormatFrom(this[sourceIndex]);
        }

        internal void AddDataPoint(ChartDataPoint dpt)
        {
            mDataPoints[dpt.Index] = dpt;
            dpt.SetParent(mSeries.DefaultDataPoint);
        }

        /// <summary>
        /// Inserts a data point with default formatting at the specified index. The indexes of the points following
        /// the inserted one are incremented by 1.
        /// </summary>
        internal void Insert(int index)
        {
            if (mDataPoints.Count == 0)
                return;

            IntToObjDictionary<ChartDataPoint> dataPoints = new IntToObjDictionary<ChartDataPoint>(mDataPoints.Count);

            foreach (ChartDataPoint dataPoint in mDataPoints.Values)
            {
                if (dataPoint.Index >= index)
                    dataPoint.Index += 1;

                dataPoints.Add(dataPoint.Index, dataPoint);
            }

            mDataPoints = dataPoints;
        }

        /// <summary>
        /// Removes the data point at the specified index. The indexes of the points following the removed one are
        /// decremented by 1.
        /// </summary>
        internal void Remove(int index)
        {
            if (mDataPoints.Count == 0)
                return;

            IntToObjDictionary<ChartDataPoint> dataPoints = new IntToObjDictionary<ChartDataPoint>(mDataPoints.Count);

            foreach (ChartDataPoint dataPoint in mDataPoints.Values)
            {
                if (dataPoint.Index == index)
                    continue;

                if (dataPoint.Index > index)
                    dataPoint.Index -= 1;

                dataPoints.Add(dataPoint.Index, dataPoint);
            }

            mDataPoints = dataPoints;
        }

        /// <summary>
        /// Sets the parent series of this collection.
        /// </summary>
        internal void SetSeries(ChartSeries series)
        {
            mSeries = series;

            foreach (ChartDataPoint point in MaterializedDataPoints)
                point.SetChart(series.Chart);
        }

        /// <summary>
        /// Gets a flag indicating whether "invertIfNegative" attribute is supported for a data point collection of
        /// the specified chart type.
        /// </summary>
        internal static bool SupportsInvertIfNegative(DmlChartType chartType)
        {
            switch (chartType)
            {
                case DmlChartType.BubbleChart:
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// If the collection does not contain a data point at the specified index yet, this method creates it and puts
        /// into the collection.
        /// </summary>
        private ChartDataPoint MaterializeItem(int index)
        {
            // The index is not compared with Count now since we cannot calculate Count exactly for some chart types
            // of MS Word 2016.
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            ChartDataPoint point = mDataPoints[index];
            if (point == null)
            {
                point = new ChartDataPoint(mSeries.Chart);
                point.Index = index;

                AddDataPoint(point);
            }

            return point;
        }

        /// <summary>
        /// Gets indexes of data points, which have non-default formatting, starting with the specified index.
        /// </summary>
        internal List<int> GetDataPointIndexesFrom(int labelIndex)
        {
            List<int> list = new List<int>();

            foreach (ChartDataPoint dataPoint in mDataPoints.Values)
            {
                int index = dataPoint.Index;
                if (index < labelIndex)
                    continue;

                if (dataPoint.HasNonDefaultFormatting)
                    list.Add(index);
            }

            list.Sort();

            return list;
        }

        /// <summary>
        /// Returns the number of <see cref="ChartDataPoint"/> in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                ChartSeriesHelper helper = new ChartSeriesHelper(mSeries);
                int count = helper.GetDataPointCount();

                return
                    count +
                    // Count points with indexes larger than the last point index possible to display.
                    // Calculate only points with non-default formatting.
                    GetDataPointIndexesFrom(count).Count;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this collection contains at least one explicitly-defined point.
        /// </summary>
        internal bool HasItems
        {
            get { return mDataPoints.Count > 0; }
        }

        /// <summary>
        /// Returns true if the collection contains data points with non-default formatting.
        /// </summary>
        internal bool HasCustomDataPoints
        {
            get
            {
                foreach (ChartDataPoint point in MaterializedDataPoints)
                {
                    if (point.HasNonDefaultFormatting)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the series to which these bars belongs.
        /// </summary>
        /// <remarks>Property exposed for test purposes.</remarks>
        internal ChartSeries Series
        {
            get { return mSeries; }
        }

        /// <summary>
        /// Gets an enumerable over all materialized data points.
        /// </summary>
        internal IEnumerable<ChartDataPoint> MaterializedDataPoints
        {
            get { return mDataPoints.Values; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartSeries mSeries;
        private IntToObjDictionary<ChartDataPoint> mDataPoints = new IntToObjDictionary<ChartDataPoint>();

        /// <summary>
        /// Enumerator over all data points of the series.
        /// </summary>
        /// <remarks>
        /// The following are included in the enumeration:
        /// 1) points that are displayed by default by MS Word for the current series data and series options even if
        /// they have default formatting defined in <see cref="ChartSeries.DefaultDataPoint"/>;
        /// 2) labels that are not displayed with the current data, but which have non-default formatting.
        /// </remarks>
        private sealed class Enumerator : IEnumerator<ChartDataPoint>
        {
            internal Enumerator(ChartDataPointCollection collection)
            {
                ChartSeriesHelper helper = new ChartSeriesHelper(collection.mSeries);
                mCollection = collection;
                mPointCount = helper.GetDataPointCount();
            }

            public bool MoveNext()
            {
                if (mCollection.mSeries == null)
                    return false;

                if (mCurrentIndex < mPointCount - 1)
                {
                    mCurrentIndex++;
                    return true;
                }

                if (mExtraDataPointIndexes == null) // Lazy initialization.
                    mExtraDataPointIndexes = mCollection.GetDataPointIndexesFrom(mPointCount);

                // mExtraDataPointIndexes is almost always empty: just enumerate through the list.
                foreach (int index in mExtraDataPointIndexes)
                {
                    if (mCurrentIndex < index)
                    {
                        mCurrentIndex = index;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                mCurrentIndex = -1;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public ChartDataPoint Current
            {
                get { return mCollection[mCurrentIndex]; }
            }

            private readonly ChartDataPointCollection mCollection;
            private readonly int mPointCount;
            private List<int> mExtraDataPointIndexes;
            private int mCurrentIndex = -1;
        }
    }
}
