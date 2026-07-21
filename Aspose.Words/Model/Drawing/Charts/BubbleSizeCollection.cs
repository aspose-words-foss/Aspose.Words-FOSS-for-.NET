// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/09/2022 by Alexander Zhiltsov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a collection of bubble sizes for a chart series.
    /// </summary>
    /// <remarks>
    /// <p>The collection allows only changing bubble sizes. To add or insert new values to a chart series, or remove
    /// values, the appropriate methods of the <see cref="ChartSeries"/> class can be used.</p>
    /// <p>Empty bubble size values are represented as <see cref="double.NaN"/>.</p>
    /// </remarks>
    /// <seealso cref="ChartSeries.Add(ChartXValue)"/>
    /// <seealso cref="ChartSeries.Add(ChartXValue, ChartYValue)"/>
    /// <seealso cref="ChartSeries.Add(ChartXValue, ChartYValue, double)"/>
    /// <seealso cref="ChartSeries.Insert(int, ChartXValue)"/>
    /// <seealso cref="ChartSeries.Insert(int, ChartXValue, ChartYValue)"/>
    /// <seealso cref="ChartSeries.Insert(int, ChartXValue, ChartYValue, double)"/>
    /// <seealso cref="ChartSeries.Remove"/>
    public class BubbleSizeCollection : IEnumerable<double>
    {
        /// <summary>
        /// Ctor to create an instance of this class.
        /// </summary>
        internal BubbleSizeCollection(ISeriesDataStore seriesDataStore)
        {
            mSeriesDataStore = seriesDataStore;
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<double> GetEnumerator()
        {
            // Some bubble sizes may not yet be materialized, so use the special Enumerator class to materialize them
            // on access.
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
        /// Inserts the value into this collection at the specified index.
        /// </summary>
        internal void Insert(int index, double value)
        {
            while (mValues.Count < index)
                mValues.Add(NonLoadedBubbleSize);

            mValues.Insert(index, value);
        }

        /// <summary>
        /// Removes the bubble size at the specified index from the collection.
        /// </summary>
        internal void Remove(int index)
        {
            if (mValues.Count > index)
                mValues.RemoveAt(index);
        }

        /// <summary>
        /// Removes all values from the collection.
        /// </summary>
        internal void Clear()
        {
            mValues.Clear();
        }

        /// <summary>
        /// If a bubble size value that corresponds to the item of the bubble size <see cref="DmlChartValueCollection"/>
        /// at the specified index has not been put to the collection yet, this method retrieves and puts it.
        /// </summary>
        /// <dev>
        /// Although bubble size is <see cref="double"/> and stored by value, but let's use the same implementation
        /// as in <see cref="ChartXValueCollection"/> and <see cref="ChartYValueCollection"/>, in which elements are
        /// loaded as they are accessed.
        /// </dev>
        private double MaterializeItem(int index)
        {
            Debug.Assert(index < Count);

            while (mValues.Count <= index)
                mValues.Add(NonLoadedBubbleSize);

            if (IsNonLoadedBubbleSize(mValues[index]))
            {
                mValues[index] = mSeriesDataStore.GetBubbleSize(index);
            }
            else
            {
                // Only this class changes bubble size data in the series: let's insure that the value has not changed
                // in the corresponding DmlChartValueCollection since the last time.
                Debug.Assert(MathUtil.AreEqual(mValues[index], mSeriesDataStore.GetBubbleSize(index)));
            }

            return mValues[index];
        }

        /// <summary>
        /// Returns <b>true</b> if the specified value is used as an indication that the bubble size value is not
        /// loaded from the corresponding <see cref="DmlChartValueCollection"/>.
        /// </summary>
        private static bool IsNonLoadedBubbleSize(double value)
        {
            return double.IsNaN(value);
        }

        /// <summary>
        /// Gets the number of items in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return mSeriesDataStore.IsBubbleSizeSupported ? mSeriesDataStore.ValueCount : 0;
            }
        }

        /// <summary>
        /// Gets or sets the bubble size value at the specified index.
        /// </summary>
        public double this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index");

                return MaterializeItem(index);
            }
            set
            {
                if (!mSeriesDataStore.IsBubbleSizeSupported)
                    throw new InvalidOperationException("This chart does not support bubble size.");

                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index");

                while (mValues.Count <= index)
                    mValues.Add(NonLoadedBubbleSize);

                mValues[index] = value;

                mSeriesDataStore.ChangeBubbleSize(value, index);
            }
        }

        /// <summary>
        /// Gets or sets the format code applied to the bubble sizes.
        /// </summary>
        /// <remarks>
        /// Number formatting is used to change the way values appears in the chart.
        /// The examples of number formats:
        /// <para>Number - "#,##0.00"</para>
        /// <para>Currency - "\"$\"#,##0.00"</para>
        /// <para>Time - "[$-x-systime]h:mm:ss AM/PM"</para>
        /// <para>Date - "d/mm/yyyy"</para>
        /// <para>Percentage - "0.00%"</para>
        /// <para>Fraction - "# ?/?"</para>
        /// <para>Scientific - "0.00E+00"</para>
        /// <para>Accounting - "_-\"$\"* #,##0.00_-;-\"$\"* #,##0.00_-;_-\"$\"* \"-\"??_-;_-@_-"</para>
        /// <para>Custom with color - "[Red]-#,##0.0"</para>
        /// </remarks>
        public string FormatCode
        {
            get { return mSeriesDataStore.BubbleSizeFormatCode ?? string.Empty; }
            set { mSeriesDataStore.BubbleSizeFormatCode = value; }
        }

        private readonly ISeriesDataStore mSeriesDataStore;
        private readonly List<double> mValues = new List<double>();

        /// <summary>
        /// Enumerator over bubble size values. Bubble sizes are materialized when accessed.
        /// </summary>
        private sealed class Enumerator : IEnumerator<double>
        {
            internal Enumerator(BubbleSizeCollection collection)
            {
                mCollection = collection;
            }

            public bool MoveNext()
            {
                mCurrentIndex++;
                return mCurrentIndex < mCollection.Count;
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

            public double Current
            {
                get { return mCollection[mCurrentIndex]; }
            }

            private readonly BubbleSizeCollection mCollection;
            private int mCurrentIndex = -1;
        }

        /// <summary>
        /// Value that is used as an indication that a bubble size value is not loaded from the corresponding
        /// <see cref="DmlChartValueCollection"/>.
        /// </summary>
        private const double NonLoadedBubbleSize = double.NaN;
    }
}
