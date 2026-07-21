// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.219 upDownBars (Up/Down Bars) element.
    /// This element specifies the up and down bars.
    /// </summary>
    internal class DmlChartUpDownBars : DmlExtensionListSource
    {
        internal DmlChartUpDownBars Clone()
        {
            DmlChartUpDownBars lhs = (DmlChartUpDownBars)MemberwiseClone();

            if (mUpBars != null)
                lhs.mUpBars = mUpBars.Clone();

            if (mDownBars != null)
                lhs.mDownBars = mDownBars.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Valid range of values is [0;500].
        /// </summary>
        internal int GapWidth
        {
            get { return mGapWidth; }
            set
            {
                if (value >= 0 && value <= 500)
                    mGapWidth = value;
            }
        }

        internal DmlChartSpPr UpBars
        {
            get
            {
                if (mUpBars == null)
                    mUpBars = new DmlChartSpPr();

                return mUpBars;
            }
        }

        internal DmlChartSpPr DownBars
        {
            get
            {
                if (mDownBars == null)
                    mDownBars = new DmlChartSpPr();

                return mDownBars;
            }
        }

        /// <summary>
        /// There is no such field in the spec. Added for convenience.
        /// If this flag is true up-down bars should not be rendered in line charts.
        /// Has no effect for stock charts.
        /// </summary>
        internal bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; }
        }

        internal bool HasUpBars
        {
            get { return mHasUpBars; }
            set { mHasUpBars = value; }
        }

        internal bool HasDownBars
        {
            get { return mHasDownBars; }
            set { mHasDownBars = value; }
        }

        internal bool IsEmpty
        {
            get { return !HasUpBars && !HasDownBars; }
        }

        /// <summary>
        /// Default gap width is 150% of bar width.
        /// </summary>
        private int mGapWidth = 150;
        private DmlChartSpPr mUpBars;
        private bool mHasUpBars;
        private DmlChartSpPr mDownBars;
        private bool mHasDownBars;
        private bool mIsVisible = false;
    }
}
