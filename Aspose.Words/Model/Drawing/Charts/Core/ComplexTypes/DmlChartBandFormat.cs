// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/24/2014 by Alexey Noskov


namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// 5.7.2.13 bandFmt (Band Format) This element specifies the formatting band of a surface chart.
    /// </summary>
    internal class DmlChartBandFormat
    {
        internal DmlChartBandFormat(int index, DmlChartSpPr spPr)
        {
            mIndex = index;
            mSpPr = spPr;
        }

        internal DmlChartBandFormat Clone()
        {
            DmlChartBandFormat lhs = (DmlChartBandFormat)MemberwiseClone();
            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            return lhs;
        }

        internal int Index
        {
            get { return mIndex; }
        }

        internal DmlChartSpPr SpPr
        {
            get { return mSpPr; }
        }

        private readonly int mIndex;
        private DmlChartSpPr mSpPr;
    }
}
