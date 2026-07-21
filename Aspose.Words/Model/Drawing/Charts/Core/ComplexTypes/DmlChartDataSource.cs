// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using System.Text;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents CT_NumDataSource or CT_StrDataSource complex types.
    /// It contains either embedded data or referenced data.
    /// </summary>
    internal class DmlChartDataSource
    {
        internal DmlChartDataSource Clone()
        {
            DmlChartDataSource lhs = (DmlChartDataSource)MemberwiseClone();

            if (mValueRef != null)
                lhs.mValueRef = mValueRef.Clone();

            if (IsFallbackValueRefActual())
                lhs.FallbackValueRef = mFallbackValueRef.Clone();
            else
                lhs.mFallbackValueRef = null;

            return lhs;
        }

        /// <summary>
        /// Gets or generates a fallback value collection that is needed for a multi-level string category in
        /// a pre-Word 2016 chart. Values of a such category are written as an AlternateContent XML element.
        /// </summary>
        /// <returns></returns>
        internal DmlChartValueCollection GetOrGenerateFallbackValues()
        {
            // Fallback values are needed only for multi-level string values.
            if ((Values == null) || (Values.ValueType != DmlChartValueType.MultiLvlString))
                return null;

            // Check for existing fallback values.
            if (IsFallbackValueRefActual())
                return mFallbackValueRef.Values;

            DmlChartValueCollection fallbackValues = new DmlChartValueCollection(DmlChartValueType.String);

            foreach (DmlChartValue val in Values)
            {
                DmlChartMultiLvlStrValue value = (DmlChartMultiLvlStrValue)val;
                StringBuilder sb = new StringBuilder();

                foreach (object level in value.Levels)
                {
                    string levelValue = (string)level;
                    if (sb.Length > 0)
                        sb.Append("\r\n");

                    sb.Append(levelValue);
                }

                fallbackValues.Add(new DmlChartStrValue(value.Index, sb.ToString()));
            }

            FallbackValueRef = new DmlChartValueRef(fallbackValues);
            return fallbackValues;
        }

        /// <summary>
        /// Gets a flag indicating whether <see cref="FallbackValueRef"/> is not null and contains actual fallback data
        /// for the data stored in the <see cref="Values"/> property.
        /// </summary>
        private bool IsFallbackValueRefActual()
        {
            return
                (mFallbackValueRef != null) &&
                (Values != null) &&
                (mFallbackValueChangeCount == Values.ValueChangeCount);
        }

        /// <summary>
        /// Gets or sets series data.
        /// </summary>
        internal DmlChartValueCollection Values
        {
            get { return (mValueRef != null) ? mValueRef.Values : null; }
            set { ValueRef = (value != null) ? new DmlChartValueRef(value) : null; }
        }

        /// <summary>
        /// Gets or sets reference to values.
        /// </summary>
        internal DmlChartValueRef ValueRef
        {
            get { return mValueRef; }
            set
            {
                mValueRef = value;
                mFallbackValueRef = null;
            }
        }

        /// <summary>
        /// Gets or sets a reference to fallback values.
        /// </summary>
        internal DmlChartValueRef FallbackValueRef
        {
            get { return mFallbackValueRef; }
            set
            {
                // A Choice part of an alternate content is always before a fallback part: expect that Values already
                // exists.
                Debug.Assert(Values != null);
                mFallbackValueChangeCount = Values.ValueChangeCount;
                mFallbackValueRef = value;
            }
        }

        private DmlChartValueRef mValueRef;
        private DmlChartValueRef mFallbackValueRef;
        private int mFallbackValueChangeCount;
    }
}
