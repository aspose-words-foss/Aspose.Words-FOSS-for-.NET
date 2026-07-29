// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2022 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents collection of <see cref="ChartSeries"/> related to a <see cref="DmlChart"/> instance.
    /// </summary>
    internal class DmlChartSeriesCollection : IEnumerable<ChartSeries>
    {
        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        internal DmlChartSeriesCollection(DmlChart dmlChart)
        {
            mDmlChart = dmlChart;
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartSeries> GetEnumerator()
        {
            return SeriesList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified series to this collection.
        /// </summary>
        internal void Add(ChartSeries series)
        {
            AddForLoad(series);

            if (LegendEntries != null)
                LegendEntries.InsertSeriesLegendEntries(series);
        }

        /// <summary>
        /// Adds the specified series to this collection. This method is intended to use when a document is being loaded;
        /// no special updates are performed.
        /// </summary>
        internal void AddForLoad(ChartSeries series)
        {
            SeriesList.Add(series);
            mDmlChart.UpdateSeriesType();
        }

        /// <summary>
        /// Removes a <see cref="ChartSeries"/> at the specified index.
        /// </summary>
        internal void RemoveAt(int index)
        {
            ChartSeries series = SeriesList[index];

            if (LegendEntries != null)
                LegendEntries.RemoveSeriesLegendEntries(series);

            mDmlChart.RemoveSeriesData(series.DataId);

            SeriesList.RemoveAt(index);
        }

        /// <summary>
        /// Removes the specified <see cref="ChartSeries"/> from the collection.
        /// </summary>
        internal void Remove(ChartSeries series)
        {
            int index = SeriesList.IndexOf(series);
            Debug.Assert(index >= 0);
            RemoveAt(index);
        }

        /// <summary>
        /// Removes all <see cref="ChartSeries"/> from this collection.
        /// </summary>
        internal void Clear()
        {
            if (LegendEntries != null)
                LegendEntries.Clear();

            SeriesList.Clear();
        }

        /// <summary>
        /// Determines the index of the specified series in the collection.
        /// </summary>
        internal int IndexOf(ChartSeries series)
        {
            return SeriesList.IndexOf(series);
        }

        /// <summary>
        /// Returns a <see cref="ChartSeries"/> at the specified index.
        /// </summary>
        internal ChartSeries this[int index]
        {
            get { return SeriesList[index]; }
        }

        /// <summary>
        /// Returns the number of <see cref="ChartSeries"/> in this collection.
        /// </summary>
        internal int Count
        {
            get { return SeriesList.Count; }
        }

        internal IList<ChartSeries> SeriesList
        {
            get { return (IList<ChartSeries>)mDmlChart.ChartPr.GetProperty(DmlChartAttrs.Series); }
        }

        private ChartLegendEntryCollection LegendEntries
        {
            get
            {
                DmlChartFormat chartFormat = mDmlChart.PlotArea.ParentChartFormat;
                if (chartFormat.Legend == null)
                    return null;

                return chartFormat.Legend.LegendEntries;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DmlChart mDmlChart;
    }
}
