// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/02/2017 by Alexander Zhiltsov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Base class for multi-level numeric and string values.
    /// </summary>
    internal abstract class DmlChartMultiLvlValue : DmlChartValue
    {
        protected DmlChartMultiLvlValue(int index, DmlChartValueType valueType)
            : base(index, valueType)
        {
        }

        /// <summary>
        /// Creates a copy of this chart data value.
        /// </summary>
        internal override DmlChartValue Clone()
        {
            DmlChartMultiLvlValue lhs = (DmlChartMultiLvlValue)base.Clone();

            lhs.mLevels = new List<object>();
            foreach (object value in mLevels)
                lhs.mLevels.Add(value);

            return lhs;
        }

        /// <summary>
        /// Adds a value of the specified level index. Level index is ignored if negative.
        /// </summary>
        internal void AddLevelValue(object value, int levelIndex)
        {
            if (levelIndex >= 0)
            {
                while (mLevels.Count <= levelIndex)
                    mLevels.Add(null);

                mLevels[levelIndex] = value;
            }
            else
            {
                mLevels.Add(value);
            }

            string stringValue = value as string;

            // WORDSNET-23206 To render a multilevel value, MS Word uses the text of all levels, sorted in reverse order.
            if (!string.IsNullOrEmpty(stringValue))
                mMultiLevelString = string.Concat(stringValue, ControlChar.Space, mMultiLevelString);

            MarkCollectionAsChanged();
        }

        /// <summary>
        /// Returns a list of values by levels.
        /// </summary>
        internal IList<object> Levels
        {
            get { return mLevels; }
        }

        /// <summary>
        /// Number of levels at this value index (<see cref="DmlChartValue.Index"/>).
        /// </summary>
        internal override int LevelsCount
        {
            get { return Levels.Count; }
        }

        /// <summary>
        /// The string representation of the multilevel value. Used to render series labels and legend items.
        /// </summary>
        internal override string StringValue
        {
            get { return mMultiLevelString; }
        }
        
        private string mMultiLevelString;
        private List<object> mLevels = new List<object>();
    }
}
