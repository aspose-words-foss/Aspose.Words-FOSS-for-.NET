// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2017 by Alexander Zhiltsov

using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents 2.24.3.10 CT_ChartData complex type [MS-ODRAWXML] (except the externalData 
    /// element, which contents are stored directly in the <see cref="DmlChartSpace"/> object).
    /// It specifies the data source for the chart object.
    /// </summary>
    internal class DmlChartDataCollection : DmlExtensionListSource, IEnumerable<DmlChartData>
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartDataCollection Clone()
        {
            DmlChartDataCollection lhs = (DmlChartDataCollection)MemberwiseClone();

            lhs.mData = new Dictionary<int, DmlChartData>();
            foreach (DmlChartData item in mData.Values)
                lhs.Add(item.Clone());

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Returns an enumerator that iterates through data items.
        /// </summary>
        public IEnumerator<DmlChartData> GetEnumerator()
        {
            return mData.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to this collection of chart data.
        /// </summary>
        internal void Add(DmlChartData data)
        {
            mData.Add(data.Id, data);
        }

        /// <summary>
        /// Creates a <see cref="DmlChartData"/> instance and adds it to this chart data collection.
        /// </summary>
        internal DmlChartData Add()
        {
            DmlChartData data = new DmlChartData();
            data.Id = GetNextDataId();

            Add(data);
            return data;
        }

        /// <summary>
        /// Gets the next unused data ID.
        /// </summary>
        private int GetNextDataId()
        {
            int nextId = mLastDataId;
            bool found;

            do
            {
                found = false;
                nextId++;

                foreach (int id in mData.Keys)
                {
                    if (id == nextId)
                    {
                        found = true;
                        break;
                    }
                }
            }
            while (found);

            mLastDataId = nextId;
            return nextId;
        }

        /// <summary>
        /// Removes the data item with the specified ID from this collection.
        /// </summary>
        internal void Remove(int id)
        {
            mData.Remove(id);
        }

        /// <summary>
        /// Removes all the data from this chart data collection.
        /// </summary>
        internal void Clear()
        {
            mData.Clear();
        }

        /// <summary>
        /// Gets number of items of this chart data collection.
        /// </summary>
        internal int Count
        {
            get { return mData.Count; }
        }

        /// <summary>
        /// Gets an item of this chart data collection.
        /// </summary>
        internal DmlChartData this[int dataId]
        {
            get { return mData.GetValueOrNull(dataId); }
        }

        private Dictionary<int, DmlChartData> mData = new Dictionary<int, DmlChartData>();
        private int mLastDataId = -1;
    }
}
