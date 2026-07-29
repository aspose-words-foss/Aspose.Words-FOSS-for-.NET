// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.105 manualLayout (Manual Layout) element.
    /// This element specifies the exact position of a chart element.
    /// </summary>
    internal class DmlChartManualLayout : DmlExtensionListSource
    {
        internal DmlChartManualLayout Clone()
        {
            DmlChartManualLayout lhs = (DmlChartManualLayout)MemberwiseClone();
            lhs.Extensions = CloneExtensions();
            return lhs;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlChartManualLayout value = (DmlChartManualLayout)obj;

            return
                MathUtil.AreEqual(Height, value.Height) &&
                (HeightMode == value.HeightMode) &&
                (LayoutTarget == value.LayoutTarget) &&
                MathUtil.AreEqual(Width, value.Width) &&
                (WidthMode == value.WidthMode) &&
                MathUtil.AreEqual(Left, value.Left) &&
                (LeftMode == value.LeftMode) &&
                MathUtil.AreEqual(Top, value.Top) &&
                (TopMode == value.TopMode);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            hash ^= GetHashOfDouble(Height);
            hash ^= HeightMode.GetHashCode();
            hash ^= LayoutTarget.GetHashCode();
            hash ^= GetHashOfDouble(Width);
            hash ^= WidthMode.GetHashCode();
            hash ^= GetHashOfDouble(Left);
            hash ^= LeftMode.GetHashCode();
            hash ^= GetHashOfDouble(Top);
            hash ^= TopMode.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Gets a hash value with support of approximate equality.
        /// </summary>
        private static int GetHashOfDouble(double value)
        {
            const int precision = 10;
            return System.Math.Round(value, precision).GetHashCode();
        }

        internal double Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        internal LayoutMode HeightMode
        {
            get { return mHeightMode; }
            set { mHeightMode = value; }
        }

        internal LayoutTarget LayoutTarget
        {
            get { return mLayoutTarget; }
            set { mLayoutTarget = value; }
        }

        internal double Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        internal LayoutMode WidthMode
        {
            get { return mWidthMode; }
            set { mWidthMode = value; }
        }

        internal double Left
        {
            get { return mLeft; }
            set { mLeft = value; }
        }

        internal LayoutMode LeftMode
        {
            get { return mLeftMode; }
            set { mLeftMode = value; }
        }

        internal double Top
        {
            get { return mTop; }
            set { mTop = value; }
        }

        internal LayoutMode TopMode
        {
            get { return mTopMode; }
            set { mTopMode = value; }
        }

        /// <summary>
        /// Returns true if manual layout is empty, i.e. position and size of target rectangle is zero.
        /// MS Word seems to ignore such manual layouts, so we should do the same.
        /// </summary>
        internal bool IsEmpty
        {
            get { return (!IsXSet && !IsYSet && !IsWidthSet && !IsHeightSet); }
        }

        /// <summary>
        /// Indicates whether the x-value of the manual layout is set.
        /// </summary>
        internal bool IsXSet
        {
            get { return !double.IsNaN(mLeft); }
        }

        /// <summary>
        /// Indicates whether the y-value of the manual layout is set.
        /// </summary>
        internal bool IsYSet
        {
            get { return !double.IsNaN(mTop); }
        }

        /// <summary>
        /// Indicates whether the width of the manual layout is set.
        /// </summary>
        internal bool IsWidthSet
        {
            get { return !double.IsNaN(mWidth); }
        }

        /// <summary>
        /// Indicates whether the height of the manual layout is set.
        /// </summary>
        internal bool IsHeightSet
        {
            get { return !double.IsNaN(mHeight); }
        }

        private double mHeight = double.NaN;
        private LayoutMode mHeightMode = LayoutMode.Factor;
        private LayoutTarget mLayoutTarget = LayoutTarget.Outer;
        private double mWidth = double.NaN;
        private LayoutMode mWidthMode = LayoutMode.Factor;
        private double mLeft = double.NaN;
        private LayoutMode mLeftMode = LayoutMode.Factor;
        private double mTop = double.NaN;
        private LayoutMode mTopMode = LayoutMode.Factor;
    }
}
