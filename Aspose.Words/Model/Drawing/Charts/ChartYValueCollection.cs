// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2022 by Alexander Zhiltsov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a collection of Y values for a chart series.
    /// </summary>
    /// <remarks>
    /// <p>All items of the collection other than <b>null</b> must have the same <see cref="ChartYValue.ValueType"/>.</p>
    /// <p>The collection allows only changing Y values. To add or insert new values to a chart series, or remove values,
    /// the appropriate methods of the <see cref="ChartSeries"/> class can be used.</p>
    /// </remarks>
    /// <seealso cref="ChartSeries.Add(ChartXValue)"/>
    /// <seealso cref="ChartSeries.Add(ChartXValue, ChartYValue)"/>
    /// <seealso cref="ChartSeries.Add(ChartXValue, ChartYValue, double)"/>
    /// <seealso cref="ChartSeries.Insert(int, ChartXValue)"/>
    /// <seealso cref="ChartSeries.Insert(int, ChartXValue, ChartYValue)"/>
    /// <seealso cref="ChartSeries.Insert(int, ChartXValue, ChartYValue, double)"/>
    /// <seealso cref="ChartSeries.Remove"/>
    public class ChartYValueCollection : IEnumerable<ChartYValue>
    {
        /// <summary>
        /// Ctor to create an instance of this class.
        /// </summary>
        internal ChartYValueCollection(ISeriesDataStore seriesDataStore)
        {
            mSeriesDataStore = seriesDataStore;
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartYValue> GetEnumerator()
        {
            // Some Y values may not yet be materialized, so use the special Enumerator class to materialize them
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
        internal void Insert(int index, ChartYValue value)
        {
            EnsureCorrectDataType(value);

            while (mValues.Count < index)
                mValues.Add(null);

            mValues.Insert(index, value);
        }

        /// <summary>
        /// Sets the value in this collection at the specified index.
        /// </summary>
        internal void SetValue(int index, ChartYValue value)
        {
            EnsureCorrectDataType(value);

            while (mValues.Count <= index)
                mValues.Add(null);

            mValues[index] = value;
        }

        /// <summary>
        /// Removes the value at the specified index from the collection.
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
        /// If an Y value that corresponds to the internal <see cref="DmlChartValue"/> at the specified index has not
        /// been created yet, this method creates it and puts into the collection.
        /// </summary>
        private ChartYValue MaterializeItem(int index)
        {
            Debug.Assert(index < Count);

            while (mValues.Count <= index)
                mValues.Add(null);

            if (mValues[index] == null)
            {
                mValues[index] = mSeriesDataStore.GetYValue(index, ValueType);
            }
            else
            {
                // Only this class changes Y data in the series: let's insure that the value has not changed in the
                // corresponding DmlChartValueCollection collection since the last time.
                Debug.Assert(object.Equals(mValues[index], mSeriesDataStore.GetYValue(index, ValueType)));
            }

            return mValues[index];
        }

        /// <summary>
        /// Gets the first non-null value stored in this collection.
        /// </summary>
        private ChartYValue GetFirstNotNullValue()
        {
            foreach (ChartYValue value in mValues)
            {
                if (value != null)
                    return value;
            }

            return null;
        }

        /// <summary>
        /// Checks that the data type of the specified value is correct for this collection.
        /// </summary>
        /// <remarks>
        /// An exception is thrown if the collection contains values with a different <see cref="ChartYValueType"/>
        /// than the specified one.
        /// If there are no non-null values in the collection, the method can change the data type of the series
        /// if necessary.
        /// </remarks>
        private void EnsureCorrectDataType(ChartYValue value)
        {
            // The value must be of the correct data type, or be null.
            if (value == null)
                return;

            ChartYValue existingValue = GetFirstNotNullValue();
            if (existingValue == null)
            {
                mSeriesDataStore.YValueType = value.ValueType;
                return;
            }

            if (existingValue.ValueType != value.ValueType)
            {
                throw new InvalidOperationException(
                    "The value must be of the same type as the existing items in the collection.");
            }
        }

        /// <summary>
        /// Gets the number of items in this collection.
        /// </summary>
        public int Count
        {
            get { return mSeriesDataStore.IsYValueSupported ? mSeriesDataStore.ValueCount : 0; }
        }

        /// <summary>
        /// Gets or sets the Y value at the specified index.
        /// </summary>
        /// <remarks>
        /// Empty values are represented as <b>null</b>.
        /// </remarks>
        public ChartYValue this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index");

                return MaterializeItem(index);
            }
            set
            {
                if (!mSeriesDataStore.IsYValueSupported)
                    throw new InvalidOperationException("This chart series does not support Y values.");

                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index");

                SetValue(index, value);

                mSeriesDataStore.ChangeYValue(value, index);
            }
        }

        /// <summary>
        /// Gets or sets the format code applied to the Y values.
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
            get { return mSeriesDataStore.YFormatCode ?? string.Empty; }
            set { mSeriesDataStore.YFormatCode = value; }
        }

        /// <summary>
        /// Gets the type of values stored in the collection.
        /// </summary>
        private ChartYValueType ValueType
        {
            get
            {
                ChartYValue existingValue = GetFirstNotNullValue();
                return (existingValue != null) ? existingValue.ValueType : mSeriesDataStore.YValueType;
            }
        }

        private readonly ISeriesDataStore mSeriesDataStore;
        private readonly List<ChartYValue> mValues = new List<ChartYValue>();

        /// <summary>
        /// Enumerator over Y values. Y values are materialized when accessed.
        /// </summary>
        private sealed class Enumerator : IEnumerator<ChartYValue>
        {
            internal Enumerator(ChartYValueCollection collection)
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

            public ChartYValue Current
            {
                get { return mCollection[mCurrentIndex]; }
            }

            private readonly ChartYValueCollection mCollection;
            private int mCurrentIndex = -1;
        }
    }
}
