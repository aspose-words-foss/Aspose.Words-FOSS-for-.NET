// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents a multi-level string point (the element 5.7.2.152 pt of ISO/IEC 29500 and 
    /// the complex type 2.24.3.76 CT_StringValue of [MS-ODRAWXML]).
    /// This element specifies string data for a specific data point.
    /// </summary>
    internal class DmlChartMultiLvlStrValue : DmlChartMultiLvlValue
    {
        internal DmlChartMultiLvlStrValue(int index)
            : base(index, DmlChartValueType.MultiLvlString)
        {
        }

        /// <summary>
        /// Returns value of the specified level.
        /// </summary>
        internal override DmlChartValue GetLevelValue(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= LevelsCount || Levels[levelIndex] == null)
                return new DmlChartDummyValue(Index, Value);

            return new DmlChartStrValue(Index, (string)Levels[levelIndex]);
        }

        /// <summary>
        /// Gets text of the specified level, indexed from the end of the level list.
        /// </summary>
        internal string GetLevelTextInReverseOrder(int levelIndexFromEnd)
        {
            return (levelIndexFromEnd < LevelsCount)
                ? (string)Levels[LevelsCount - levelIndexFromEnd - 1]
                : null;
        }

        /// <summary>
        /// Returns one based index as double value.
        /// </summary>
        internal override double Value
        {
            get { return (Index + 1); }
        }
    }
}
