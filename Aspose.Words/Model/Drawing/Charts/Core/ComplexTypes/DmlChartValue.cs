// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2012 by Alexey Noskov

using System;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents a data value of a chart.
    /// </summary>
    /// <dev>
    /// If state of a <see cref="DmlChartValue"/> is changed, the parent collection should be notified using
    /// <see cref="MarkCollectionAsChanged"/>.
    /// </dev>
    internal abstract class DmlChartValue : IComparable<DmlChartValue>
    {
        protected DmlChartValue(int index, DmlChartValueType valueType)
        {
            mIndex = index;
            mValueType = valueType;
        }

        public int CompareTo(DmlChartValue other)
        {
            // alexnosk: After changing IComparable to IComparable<DmlChartValue> it is required to handle null values.
            // Emulate old behavior and place null values at the very beginning of the sorted array.
            int test = Value.CompareTo((other == null) ? double.MinValue : other.Value);
            if (test != 0)
                return test;

            // If values are the same place them in index order.
            return other.Index.CompareTo(mIndex);
        }

        /// <summary>
        /// Checks whether the specified <see cref="DmlChartValue"/> is null or is NaN.
        /// </summary>
        /// <param name="value">The specified  <see cref="DmlChartValue"/></param>
        /// <returns>"True" if specified <see cref="DmlChartValue"/> is null or is NaN, otherwise "False"</returns>
        internal static bool IsNullOrNaN(DmlChartValue value)
        {
            return (value == null) || value.IsNaN;
        }

        /// <summary>
        /// Checks whether the specified value is <b>null</b> or of the <see cref="DmlChartValueType.None"/> type.
        /// </summary>
        internal static bool IsNullOrNone(DmlChartValue value)
        {
            return (value == null) || (value.ValueType == DmlChartValueType.None);
        }

        /// <summary>
        /// Creates a copy of this chart data value.
        /// </summary>
        internal virtual DmlChartValue Clone()
        {
            DmlChartValue lhs = (DmlChartValue)MemberwiseClone();
            lhs.mCollection = null;
            return lhs;
        }

        /// <summary>
        /// Returns value for the specified level index, if index is negative or grater than level count in this value
        /// returns dummy value. Note: never returns null.
        /// </summary>
        internal virtual DmlChartValue GetLevelValue(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= LevelsCount)
                return new DmlChartDummyValue(Index, Value);

            return this;
        }

        /// <summary>
        /// Marks the parent collection as changed.
        /// </summary>
        protected void MarkCollectionAsChanged()
        {
            if (mCollection != null)
                mCollection.MarkAsChanged();
        }

        internal int Index
        {
            get { return mIndex; }
            set
            {
                if (mIndex == value)
                    return;

                mIndex = value;
                MarkCollectionAsChanged();
            }
        }

        /// <summary>
        /// Indicates whether value is NaN.
        /// </summary>
        internal bool IsNaN
        {
            get { return double.IsNaN(Value); }
        }

        internal DmlChartValueType ValueType
        {
            get { return mValueType; }
        }

        internal virtual int LevelsCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Flag indicates whether value is visible on Axis. 
        /// By default returns true.
        /// </summary>
        internal virtual bool IsVisible
        {
            get
            {
                return true; 
            }
        }

        internal abstract string StringValue { get; }

        internal float FloatValue
        {
            get { return (float)Value; }
        }

        internal abstract double Value { get; }

        internal virtual bool IsDate
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the parent collection.
        /// </summary>
        internal DmlChartValueCollection Collection
        {
            get { return mCollection; }
            set { mCollection = value; }
        }

        private int mIndex;
        private readonly DmlChartValueType mValueType;
        private DmlChartValueCollection mCollection;

        /// <summary>
        /// String representation of NaN values.
        /// </summary>
        internal const string NanStringValue = "#N/A";
    }
}
