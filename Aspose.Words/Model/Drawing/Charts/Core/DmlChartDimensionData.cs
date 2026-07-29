// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2021 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents a data of a chart dimension (X, Y or Bubble Size).
    /// </summary>
    internal class DmlChartDimensionData
    {
        internal DmlChartDimensionData(DmlChartDataSource dataSource)
        {
            mDataSource = dataSource;
        }

        /// <summary>
        /// Gets a value of the rendering data at the specified index.
        /// </summary>
        internal DmlChartValue GetValue(int index)
        {
            return (Data != null)
                ? Data.GetValue(index) // Gets actual data value.
                : DmlChartRenderingData.GetValue(null, index); // Returns a dummy value.
        }

        internal DmlChartDimensionData Clone()
        {
            DmlChartDimensionData lhs = (DmlChartDimensionData)MemberwiseClone();

            lhs.mDataSource = mDataSource.Clone();

            if (mData != null)
                lhs.mData = mData.Clone(lhs.Values);

            return lhs;
        }

        /// <summary>
        /// Copies data from the specified data source.
        /// </summary>
        internal void CopyDataFrom(DmlChartDimensionData source)
        {
            if (source.IsEmpty)
                return;

            mData = source.Data.Clone();
        }

        /// <summary>
        /// Gets the <see cref="DmlChartDimensionData"/> with "dummy" values.
        /// </summary>
        internal static DmlChartDimensionData GetDummyDimensionData(int valueCount)
        {
            DmlChartDataSource dummySource = new DmlChartDataSource();
            dummySource.Values = GetDummyCollection(valueCount, DmlChartValueType.Numeric);
            return new DmlChartDimensionData(dummySource);
        }

        /// <summary>
        /// Gets <see cref="DmlChartValueCollection"/> with "dummy" values.
        /// </summary>
        /// <param name="valueCount">The number of data values.</param>
        /// <param name="valuesType"><see cref="DmlChartValueType"/> that should be used.</param>
        /// <returns><see cref="DmlChartValueCollection"/> with "dummy" values.</returns>
        private static DmlChartValueCollection GetDummyCollection(int valueCount, DmlChartValueType valuesType)
        {
            DmlChartValueCollection dummyCollection = new DmlChartValueCollection(valuesType);
            dummyCollection.ValueCount = valueCount;
            dummyCollection.FillWithDummyValues();
            return dummyCollection;
        }

        /// <summary>
        /// Gets data for rendering.
        /// </summary>
        /// <remarks>
        /// This property returns series data modified for rendering.
        /// </remarks>
        internal DmlChartRenderingData Data
        {
            get
            {
                if (mData == null)
                {
                    if (Values == null)
                        return null;

                    mData = new DmlChartRenderingData(Values);
                }

                return mData;
            }
        }

        /// <summary>
        /// Gets or sets series data.
        /// </summary>
        internal DmlChartValueCollection Values
        {
            get { return (mDataSource != null) ? mDataSource.Values : null; }
            set
            {
                mDataSource.Values = value;
                mData = null;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if values in the collection are dates.
        /// </summary>
        internal bool IsDate
        {
            get { return (Data != null) && Data.IsDate; }
        }

        /// <summary>
        /// Returns values type in the data source. If the data source has no data <see cref="DmlChartValueType.None"/>
        /// is returned.
        /// </summary>
        internal DmlChartValueType ValueType
        {
            get { return (Data != null) ? Data.ValueType : DmlChartValueType.None; }
        }

        internal bool IsEmpty
        {
            get { return (Values == null); }
        }

        /// <summary>
        /// Gets type of this dimension.
        /// </summary>
        internal DimensionType DimensionType
        {
            get { return mDataSource.ValueRef.DimensionType; }
            set
            {
                Debug.Assert(mDataSource.ValueRef != null);
                mDataSource.ValueRef.DimensionType = value;
            }
        }

        /// <summary>
        /// Gets a data source of this dimension. 
        /// </summary>
        internal DmlChartDataSource DataSource
        {
            get { return mDataSource; }
        }

        private DmlChartDataSource mDataSource;
        private DmlChartRenderingData mData;
    }
}
