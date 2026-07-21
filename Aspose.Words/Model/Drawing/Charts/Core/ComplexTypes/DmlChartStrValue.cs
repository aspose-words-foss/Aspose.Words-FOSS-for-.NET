// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents pt (String Point) (5.7.2.152) element.
    /// This element specifies string data for a specific data point.
    /// </summary>
    internal class DmlChartStrValue : DmlChartValue
    {
        internal DmlChartStrValue(int index, string value)
            : base(index, DmlChartValueType.String)
        {
            mValue = value;
        }

        internal override string StringValue
        {
            get { return mValue; }
        }

        /// <summary>
        /// Returns one based index as float value.
        /// </summary>
        internal override double Value
        {
            get { return (Index + 1); }
        }

        private readonly string mValue;
    }
}
