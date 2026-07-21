// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2022 by Alexander Zhiltsov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a collection of chart legend entries.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartLegendEntryCollection : IEnumerable<ChartLegendEntry>
    {
        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        internal ChartLegendEntryCollection(DmlChartFormat chartFormat)
        {
            mChartFormat = chartFormat;
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartLegendEntry> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a deep clone of this collection.
        /// </summary>
        internal ChartLegendEntryCollection Clone(DmlChartFormat chartFormat)
        {
            ChartLegendEntryCollection lhs = new ChartLegendEntryCollection(chartFormat);

            if (mLegendEntries.Count == 0)
                return lhs;

            foreach (ChartLegendEntry value in mLegendEntries.Values)
                lhs.mLegendEntries.Add(value.Index, value.Clone());

            return lhs;
        }

        /// <summary>
        /// Adds the chart legend entry to this collection. If the collection already contains an entry with that
        /// index, the entry will be replaced.
        /// </summary>
        internal void AddEntry(ChartLegendEntry entry)
        {
            Debug.Assert(entry != null);
            mLegendEntries[entry.Index] = entry;
            entry.SetParentDmlChartFormat(mChartFormat);
        }

        /// <summary>
        /// Gets <see cref="ChartLegendEntry"/> for the specified index. Returns <b>null</b> if the entry does not
        /// exist yet.
        /// </summary>
        internal ChartLegendEntry GetEntry(int index)
        {
            return mLegendEntries[index];
        }

        /// <summary>
        /// Clears all legend entries of this collection.
        /// </summary>
        internal void Clear()
        {
            mLegendEntries.Clear();
        }

        /// <summary>
        /// Inserts legend entries related to the specified added series and its trendlines.
        /// </summary>
        internal void InsertSeriesLegendEntries(ChartSeries series)
        {
            LegendEntryIndexes indexes = GetLegendEntryIndexes(series);

            int seriesInsertIndex = indexes.SeriesIndex;
            // indexes.FirstTrendlineIndex includes the inserted series: decrease.
            int trendlinesInsertIndex = (indexes.FirstTrendlineIndex >= 0) ? indexes.FirstTrendlineIndex - 1 : -1;
            int seriesTrendLineCount = series.Trendlines.Count;

            List<ChartLegendEntry> sortedEntries = new List<ChartLegendEntry>(mLegendEntries.Values);
            sortedEntries.Sort(new ChartLegendEntryComparerByIndex(true));

            foreach (ChartLegendEntry entry in sortedEntries)
            {
                int index = entry.Index;
                if (index < seriesInsertIndex)
                    break;

                int newIndex = index + 1;
                if ((trendlinesInsertIndex >= 0) && (index >= trendlinesInsertIndex))
                    newIndex += seriesTrendLineCount;

                mLegendEntries.Remove(index);
                entry.Index = newIndex;
                mLegendEntries.Add(newIndex, entry);
            }
        }

        /// <summary>
        /// Removes legend entries related to the specified series and its trendlines.
        /// </summary>
        internal void RemoveSeriesLegendEntries(ChartSeries series)
        {
            LegendEntryIndexes indexes = GetLegendEntryIndexes(series);
            int seriesTrendLineCount = series.Trendlines.Count;

            List<ChartLegendEntry> sortedEntries = new List<ChartLegendEntry>(mLegendEntries.Values);
            sortedEntries.Sort(new ChartLegendEntryComparerByIndex(false));

            foreach (ChartLegendEntry entry in sortedEntries)
            {
                int index = entry.Index;
                if (index < indexes.SeriesIndex)
                    continue;

                mLegendEntries.Remove(index);

                if ((index == indexes.SeriesIndex) ||
                    ((indexes.FirstTrendlineIndex >= 0) &&
                     (index >= indexes.FirstTrendlineIndex) &&
                     (index < indexes.FirstTrendlineIndex + seriesTrendLineCount)))
                {
                    continue;
                }

                int newIndex = index - 1;
                if ((indexes.FirstTrendlineIndex >= 0) && (index > indexes.FirstTrendlineIndex))
                    newIndex -= seriesTrendLineCount;

                entry.Index = newIndex;
                mLegendEntries.Add(newIndex, entry);
            }
        }

        /// <summary>
        /// Gets a legend entry related to the specified chart series.
        /// </summary>
        internal ChartLegendEntry GetSeriesLegendEntry(ChartSeries series)
        {
            LegendEntryIndexes indexes = GetLegendEntryIndexes(series);
            return this[indexes.SeriesIndex];
        }

        private LegendEntryIndexes GetLegendEntryIndexes(ChartSeries series)
        {
            // Series legend entries are located before trendline legend entries.

            LegendEntryIndexes indexes = new LegendEntryIndexes();
            int seriesCount = 0;
            int trendlineCount = 0;

            foreach (DmlChart chart in mChartFormat.PlotArea.Charts)
            {
                foreach (ChartSeries currentSeries in chart.Series)
                {
                    if (currentSeries == series)
                    {
                        indexes.SeriesIndex = seriesCount;
                        indexes.FirstTrendlineIndex = trendlineCount;
                    }

                    seriesCount++;
                    trendlineCount += currentSeries.Trendlines.Count;
                }
            }

            indexes.FirstTrendlineIndex = (series.Trendlines.Count > 0)
                ? indexes.FirstTrendlineIndex + seriesCount
                : -1;

            return indexes;
        }

        /// <summary>
        /// Returns the number of <see cref="ChartLegendEntry"/> in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (mChartFormat == null)
                {
                    Debug.Assert(false);
                    return mLegendEntries.Count;
                }

                int count = 0;
                foreach (DmlChart chart in mChartFormat.PlotArea.Charts)
                {
                    foreach (ChartSeries series in chart.Series)
                        count += series.Trendlines.Count + 1; // Trendlines and the series itself.
                }

                return count;
            }
        }

        /// <summary>
        /// Returns <see cref="ChartLegendEntry"/> for the specified index.
        /// </summary>
        public ChartLegendEntry this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index");

                if (!mLegendEntries.ContainsKey(index))
                {
                    ChartLegendEntry entry = new ChartLegendEntry();
                    entry.Index = index;

                    AddEntry(entry);
                }

                return mLegendEntries[index];
            }
        }

        /// <summary>
        /// Gets an enumerable over all materialized legend entries.
        /// </summary>
        internal IEnumerable<ChartLegendEntry> MaterializedLegendEntries
        {
            get { return mLegendEntries.Values; }
        }

        private readonly IntToObjDictionary<ChartLegendEntry> mLegendEntries = new IntToObjDictionary<ChartLegendEntry>();

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DmlChartFormat mChartFormat;

        /// <summary>
        /// Stores information about legend entry indexes related to a chart series and its trendlines.
        /// </summary>
        private class LegendEntryIndexes
        {
            internal LegendEntryIndexes()
            {
                SeriesIndex = -1;
                FirstTrendlineIndex = -1;
            }

            internal int SeriesIndex { get; set; }
            internal int FirstTrendlineIndex { get; set; }
        }

        /// <summary>
        /// Implements a comparer of <see cref="ChartLegendEntry"/> by index.
        /// </summary>
        private class ChartLegendEntryComparerByIndex : IComparer<ChartLegendEntry>
        {
            internal ChartLegendEntryComparerByIndex(bool isDescending)
            {
                mIsDescending = isDescending;
            }

            /// <summary>
            /// Performs a comparison of two <see cref="ChartLegendEntry"/> and returns a value indicating whether
            /// the first entry is less than, equal to, or greater than the second one.
            /// </summary>
            public int Compare(ChartLegendEntry x, ChartLegendEntry y)
            {
                int sign = mIsDescending ? -1 : 1;

                if (x == null)
                    return (y == null) ? 0 : -sign;

                if (y == null)
                    return sign;

                return sign * x.Index.CompareTo(y.Index);
            }

            private bool mIsDescending;
        }

        /// <summary>
        /// Represents an enumerator over all legend entries, including those not yet materialized, in order from
        /// the entry at index 0 to the entry at index <see cref="Count"/> - 1.
        /// </summary>
        private sealed class Enumerator : IEnumerator<ChartLegendEntry>
        {
            internal Enumerator(ChartLegendEntryCollection collection)
            {
                mCollection = collection;
                mCount = collection.Count;
            }

            public bool MoveNext()
            {
                mCurrentIndex++;
                return (mCurrentIndex < mCount);
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

            public ChartLegendEntry Current
            {
                get { return mCollection[mCurrentIndex]; }
            }

            private readonly ChartLegendEntryCollection mCollection;
            private readonly int mCount;
            private int mCurrentIndex = -1;
        }
    }
}
