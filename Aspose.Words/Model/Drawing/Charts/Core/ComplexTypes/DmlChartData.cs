// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2017 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents the 2.24.3.15 CT_Data complex type [MS-ODRAWXML].
    /// It specifies number or string dimensions for chart data.
    /// </summary>
    internal class DmlChartData : DmlExtensionListSource
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartData Clone()
        {
            DmlChartData lhs = (DmlChartData)MemberwiseClone();

            lhs.mDataSources = new List<DmlChartDataSource>();
            foreach (DmlChartDataSource dataSource in mDataSources)
                lhs.DataSources.Add(dataSource.Clone());

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Gets a chart dimension data source of the specified type.
        /// </summary>
        internal DmlChartDataSource GetDataSource(DimensionType type)
        {
            foreach (DmlChartDataSource dataSource in mDataSources)
            {
                if ((dataSource.ValueRef != null) && (dataSource.ValueRef.DimensionType == type))
                    return dataSource;
            }

            return null;
        }

        /// <summary>
        /// Removes a chart dimension data source of the specified type from the chart data.
        /// </summary>
        internal void RemoveDataSource(DimensionType type)
        {
            foreach (DmlChartDataSource dataSource in mDataSources)
            {
                if ((dataSource.ValueRef != null) && (dataSource.ValueRef.DimensionType == type))
                {
                    mDataSources.Remove(dataSource);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets a list containing chart dimension data sources that store data of the CT_NumericDimension or
        /// CT_StringDimension complex type (the numDim or strDim element).
        /// </summary>
        internal IList<DmlChartDataSource> DataSources
        {
            get { return mDataSources; }
        }

        /// <summary>
        /// Gets or sets identifier of this data.
        /// </summary>
        internal int Id
        {
            get { return mId; }
            set { mId = value; }
        }

        private List<DmlChartDataSource> mDataSources = new List<DmlChartDataSource>();
        private int mId;
    }
}
