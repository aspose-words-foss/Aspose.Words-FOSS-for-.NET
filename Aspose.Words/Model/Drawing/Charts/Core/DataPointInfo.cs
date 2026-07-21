// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/11/2022 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents information about a data point/label related to a particular chart data value.
    /// </summary>
    internal class DataPointInfo
    {
        internal DataPointInfo(int index, int valueIndex, int valueLevel, bool isPointReferencedByOtherValues)
        {
            Index = index;
            ValueIndex = valueIndex;
            ValueLevel = valueLevel;
            IsPointReferencedByOtherValues = isPointReferencedByOtherValues;
        }

        /// <summary>
        /// Gets the index of the data point/label.
        /// </summary>
        internal int Index { get; }

        /// <summary>
        /// Gets the index of the chart data value that corresponds to the data point/label.
        /// </summary>
        internal int ValueIndex { get; set; }

        /// <summary>
        /// Gets the level of the chart data value that corresponds to the data point/label.
        /// </summary>
        internal int ValueLevel { get; }

        /// <summary>
        /// Gets a flag indicating whether there are other chart data values that reference to the data point/label and
        /// that the data point and label should be preserved when the value is removed.
        /// </summary>
        internal bool IsPointReferencedByOtherValues { get; }
    }
}
