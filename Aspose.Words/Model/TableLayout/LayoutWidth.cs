// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2006 by Dmitry Vorobyev

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Min and max widths are often used in the layout algorithm in aggregate so it's reasonable
    /// to encapsulate them together.
    /// </summary>
    internal class LayoutWidth
    {
        internal void Add(double value)
        {
            mMin += value;
            mMax += value;
        }

        internal void Update(LayoutWidth newWidth)
        {
            mMin = System.Math.Max(mMin, newWidth.Min);
            mMax = System.Math.Max(mMax, newWidth.Max);
        }

        internal LayoutWidth Clone()
        {
            LayoutWidth lhs = new LayoutWidth();

            lhs.Min = mMin;
            lhs.Max = mMax;

            return lhs;
        }

        internal double Min
        {
            get { return mMin; }
            set { mMin = value; }
        }

        internal double Max
        {
            get { return mMax; }
            set { mMax = value; }
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0}/{1}", mMin, mMax);
        }
#endif

        private double mMin;
        private double mMax;
    }
}