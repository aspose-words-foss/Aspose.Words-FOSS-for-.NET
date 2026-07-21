// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.143 pivotFmt (Pivot Format) element.
    /// This element contains a set of formatting to be applied to the chart that is based on a pivotTable.
    /// </summary>
    internal class DmlChartPivotFormat : DmlExtensionListSource
    {
        internal DmlChartPivotFormat(IThemeProvider themeProvider)
        {
            mThemeProvider = themeProvider;
        }

        internal DmlChartPivotFormat Clone()
        {
            DmlChartPivotFormat lhs = (DmlChartPivotFormat)MemberwiseClone();
            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mDLbl != null)
                lhs.mDLbl = mDLbl.Clone();

            if (mMarker != null)
                lhs.mMarker = mMarker.Clone();

            if (mTxPr != null)
                lhs.mTxPr = mTxPr.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal ChartDataLabel DLbl
        {
            get { return mDLbl; }
            set { mDLbl = value; }
        }

        internal int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        internal ChartMarker Marker
        {
            get
            {
                if (mMarker == null)
                    mMarker = new ChartMarker(mThemeProvider);

                return mMarker;
            }
        }

        internal DmlChartSpPr SpPr
        {
            get
            {
                if (mSpPr == null)
                    mSpPr = new DmlChartSpPr();

                return mSpPr;
            }
        }

        internal DmlChartTxPr TxPr
        {
            get
            {
                if (mTxPr == null)
                    mTxPr = new DmlChartTxPr();

                return mTxPr;
            }
        }

        private ChartDataLabel mDLbl;
        private int mIndex;
        private ChartMarker mMarker;
        private DmlChartSpPr mSpPr;
        private DmlChartTxPr mTxPr;
        private readonly IThemeProvider mThemeProvider;
    }
}
