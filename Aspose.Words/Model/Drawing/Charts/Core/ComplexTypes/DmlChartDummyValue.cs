// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    internal class DmlChartDummyValue : DmlChartValue
    {
        internal DmlChartDummyValue(int index, double value)
            : this(index, value, true)
        {
        }

        internal DmlChartDummyValue(int index, double value, bool isVisible)
            : base(index, DmlChartValueType.None)
        {
            mValue = value;
            mIsVisible = isVisible;
        }

        internal override string StringValue
        {
            get { return string.Empty; }
        }

        internal override double Value
        {
            get { return mValue; }
        }

        internal override bool IsVisible
        {
            get { return mIsVisible; }
        }

        private readonly double mValue;
        private readonly bool mIsVisible;
    }
}
