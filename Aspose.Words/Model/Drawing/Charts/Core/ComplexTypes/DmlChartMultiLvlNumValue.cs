// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/02/2017 by Alexander Zhiltsov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents a multi-level numeric point (the complex type 2.24.3.59 CT_NumericValue of [MS-ODRAWXML]).
    /// This element specifies numeric data for a specific data point.
    /// </summary>
    internal class DmlChartMultiLvlNumValue : DmlChartMultiLvlValue
    {
        /// <summary>
        /// Default ctor. Allows specifying level properties that contain format string by levels.
        /// </summary>
        internal DmlChartMultiLvlNumValue(int index, IList<DmlChartDataLevelProperties> levelProperties)
            : base(index, DmlChartValueType.MultiLvlNumeric)
        {
            mLevelProperties = levelProperties;
        }

        /// <summary>
        /// Returns value of the specified level.
        /// </summary>
        internal override DmlChartValue GetLevelValue(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= LevelsCount || Levels[levelIndex] == null)
                return new DmlChartDummyValue(Index, Value);

            return new DmlChartNumValue(Index, (double)Levels[levelIndex], GetFormatCode(levelIndex));
        }

        /// <summary>
        /// Sets level properties.
        /// </summary>
        internal void SetLevelProperties(List<DmlChartDataLevelProperties> levelProperties)
        {
            mLevelProperties = levelProperties;
        }

        /// <summary>
        /// Returns format code for the specified level index.
        /// </summary>
        internal string GetFormatCode(int levelIndex)
        {
            if (mLevelProperties != null && mLevelProperties.Count > levelIndex)
                return mLevelProperties[levelIndex].FormatCode;
            else
                return null;
        }

        /// <summary>
        /// Returns value of the first level formatted with using the format string.
        /// </summary>
        internal override string StringValue
        {
            get { return DmlChartNumValue.ValueToString(Value, 1, GetFormatCode(0)); }
        }

        /// <summary>
        /// Just returns value of the first level.
        /// </summary>
        internal override double Value
        {
            get { return Levels[0] != null ? (double)Levels[0] : 0; }
        }

        /// <summary>
        /// Returns true if format string of the first level is date format.
        /// </summary>
        internal override bool IsDate
        {
            get { return DmlChartFormatCodeValidator.IsDateFormat(GetFormatCode(0)); }
        }

        private IList<DmlChartDataLevelProperties> mLevelProperties;
    }
}
