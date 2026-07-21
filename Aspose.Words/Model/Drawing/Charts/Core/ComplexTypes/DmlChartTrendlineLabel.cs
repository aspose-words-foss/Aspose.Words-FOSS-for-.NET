// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.213 trendlineLbl (Trendline Label) element.
    /// This element specifies the label for the trendline.
    /// </summary>
    internal class DmlChartTrendlineLabel : DmlExtensionListSource
    {
        internal DmlChartTrendlineLabel Clone()
        {
            DmlChartTrendlineLabel lhs = (DmlChartTrendlineLabel)MemberwiseClone();
            if (mLayout != null)
                lhs.mLayout = mLayout.Clone();

            if (mNumFmt != null)
                lhs.mNumFmt = mNumFmt.Clone();

            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mTx != null)
                lhs.mTx = mTx.Clone();

            if (mTxPr != null)
                lhs.mTxPr = mTxPr.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal DmlChartManualLayout Layout
        {
            get { return mLayout; }
            set { mLayout = value; }
        }

        internal DmlChartNumFormat NumFmt
        {
            get { return mNumFmt; }
            set { mNumFmt = value; }
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

        /// <summary>
        /// Indicates whether the data label position should be changed. 
        /// </summary>
        internal bool IsPositionChanged
        {
            get { return HasLayout && (Layout.IsXSet || Layout.IsYSet); }
        }

        /// <summary>
        /// Indicates whether size of label should be changed. 
        /// </summary>
        internal bool IsSizeChanged
        {
            get { return HasLayout && (Layout.IsWidthSet || Layout.IsHeightSet); }
        }

        /// <summary>
        /// Indicates whether the data label has layout. 
        /// </summary>
        internal bool HasLayout
        {
            get { return (Layout != null); }
        }

        internal DmlChartTx Tx
        {
            get { return mTx; }
            set { mTx = value; }
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

        private DmlChartManualLayout mLayout;
        private DmlChartNumFormat mNumFmt;
        private DmlChartSpPr mSpPr;
        private DmlChartTx mTx;
        private DmlChartTxPr mTxPr;
    }
}
