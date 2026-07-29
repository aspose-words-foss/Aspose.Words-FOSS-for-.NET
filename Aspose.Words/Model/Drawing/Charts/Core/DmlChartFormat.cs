// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents 5.7.2.27 chart (Chart) element.
    /// This element specifies the chart.
    /// </summary>
    internal class DmlChartFormat : DmlExtensionListSource, IDmlChartTitleHolder
    {
        /// <summary>
        /// If font size is not specified for a chart title, MS Word uses 1.2 * [font size of chart space]. This method
        /// is used to get such calculated font size.
        /// </summary>
        public int GetRelativeFontSize(int chartFontSize)
        {
            return (int)System.Math.Round(chartFontSize * 1.2);
        }

        public void Warn(WarningType warningType, string message)
        {
            if (mChartSpace == null)
                return;

            IWarningCallback warningCallback = mChartSpace.Dml.Document.WarningCallback;
            if (warningCallback != null)
                warningCallback.Warning(new WarningInfo(warningType, WarningSource.DrawingML, message));
        }

        internal void SetChartSpace(DmlChartSpace chartSpace)
        {
            mChartSpace = chartSpace;
        }

        internal DmlChartFormat Clone()
        {
            DmlChartFormat lhs = (DmlChartFormat)MemberwiseClone();
            if (mTitle != null)
            {
                lhs.mTitle = mTitle.Clone();
                lhs.mTitle.SetTitleHolder(lhs);
            }

            if (mBackWall != null)
                lhs.mBackWall = mBackWall.Clone();

            if (mFloor != null)
                lhs.mFloor = mFloor.Clone();

            if (mSideWall != null)
                lhs.mSideWall = mSideWall.Clone();

            if (mLegend != null)
                lhs.mLegend = mLegend.Clone(lhs);

            if (mPivotFmts != null)
                lhs.mPivotFmts = mPivotFmts.Clone();

            if (mPlotArea != null)
            {
                DmlChartPlotArea cloned = mPlotArea.Clone();
                cloned.SetChartFormat(lhs);
                lhs.mPlotArea = cloned;
            }

            if (mView3D != null)
                lhs.mView3D = mView3D.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal DmlChartSpace DmlChartSpace
        {
            get { return mChartSpace; }
        }

        internal bool AutoTitleDeleted
        {
            get { return mAutoTitleDeleted; }
            set { mAutoTitleDeleted = value; }
        }

        internal bool PlotVisOnly
        {
            get { return mPlotVisOnly; }
            set { mPlotVisOnly = value; }
        }

        internal bool ShowDLblsOverMax
        {
            get { return mShowDLblsOverMax; }
            set { mShowDLblsOverMax = value; }
        }

        internal DmlChartSurface BackWall
        {
            get { return mBackWall; }
            set { mBackWall = value; }
        }

        internal DmlChartSurface Floor
        {
            get { return mFloor; }
            set { mFloor = value; }
        }

        internal DmlChartSurface SideWall
        {
            get { return mSideWall; }
            set { mSideWall = value; }
        }

        internal DisplayBlanksAs DispBlanksAs
        {
            get { return mDispBlanksAs; }
            set { mDispBlanksAs = value; }
        }

        internal ChartLegend Legend
        {
            get { return mLegend; }
            set { mLegend = value; }
        }

        internal DmlChartPivotFormats PivotFmts
        {
            get { return mPivotFmts; }
            set { mPivotFmts = value; }
        }

        internal DmlChartPlotArea PlotArea
        {
            get { return mPlotArea; }
            set { mPlotArea = value; }
        }

        internal DmlChartView3D View3D
        {
            get { return mView3D; }
            set { mView3D = value; }
        }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        public DmlChartTitle DCTitle
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Returns the position of the title.
        /// </summary>
        public TitlePosition TitlePosition
        {
            get { return TitlePosition.Top; }
        }

        /// <summary>
        /// Indicates whether the chart title is visible.
        /// </summary>
        bool IDmlChartTitleHolder.IsVisible
        {
            get { return !AutoTitleDeleted && (DCTitle != null); }
        }

        /// <summary>
        /// Gets the default title text.
        /// </summary>
        string IDmlChartTitleHolder.DefaultTitleText
        {
            get { return DefaultChartTitles.GetTitle(); }
        }

        /// <summary>
        /// Gets the title default font size in points.
        /// </summary>
        double IDmlChartTitleHolder.DefaultFontSize
        {
            get { return mChartSpace.IsChartEx ? 14 : 18; }
        }

        /// <summary>
        /// Gets the title font size in points that MS Word sets for created charts.
        /// </summary>
        double IDmlChartTitleHolder.DefaultDisplayedFontSize
        {
            get { return 14; }
        }

        DmlChartStyleItem IDmlChartTitleHolder.StyleItem
        {
            get { return DmlChartStyleItem.Title; }
        }

        /// <summary>
        /// Gets the parent chart space.
        /// </summary>
        DmlChartSpace IDmlChartTitleHolder.ChartSpace
        {
            get { return mChartSpace; }
        }

        /// <summary>
        /// Returns the document containing the parent chart.
        /// </summary>
        internal DocumentBase Document
        {
            get { return mChartSpace.Dml.Document; }
        }

        /// <summary>
        /// Returns true if title should not be displayed even if it exists.
        /// </summary>
        public bool TitleDeleted
        {
            get { return AutoTitleDeleted; }
            set
            {
                if (AutoTitleDeleted == value)
                    return;

                AutoTitleDeleted = value;
                if (!value)
                {
                    if (mTitle == null)
                        mTitle = new DmlChartTitle(this);

                    mChartSpace.ApplyChartStyleToChartTitle();
                }
            }
        }

        private bool mAutoTitleDeleted;
        private DmlChartSurface mBackWall;
        private DisplayBlanksAs mDispBlanksAs = DisplayBlanksAs.Zero;
        private DmlChartSurface mFloor;
        private ChartLegend mLegend;
        private DmlChartPivotFormats mPivotFmts;
        private DmlChartPlotArea mPlotArea;
        private bool mPlotVisOnly;
        private bool mShowDLblsOverMax;
        private DmlChartSurface mSideWall;
        private DmlChartTitle mTitle;
        private DmlChartView3D mView3D;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlChartSpace mChartSpace;
    }
}
