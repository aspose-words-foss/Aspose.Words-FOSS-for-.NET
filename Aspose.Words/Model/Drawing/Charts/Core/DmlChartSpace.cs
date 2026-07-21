// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents 5.7.2.29 chartSpace (Chart Space) element.
    /// This element specifies overall settings for a single chart, and is the root node for the chart part.
    /// Root element of DrawingML Chart part.
    /// </summary>
    internal class DmlChartSpace : DmlNode, IDmlCommonShapePrSource
    {
        internal DmlChartSpace()
            : this(false)
        {
        }

        internal DmlChartSpace(bool isChartEx)
        {
            mIsChartEx = isChartEx;
        }

        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            // Probably we should set mEmbeddedData data to null here. It seems we do the same for embedded ole objects in shapes.

            DmlChartSpace lhs = (DmlChartSpace) base.Clone(isCloneChildren, cloningListener);

            if (mChartFormat != null)
            {
                lhs.ChartFormat = mChartFormat.Clone();
                lhs.ChartFormat.SetChartSpace(lhs);
            }

            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mPivotSource != null)
                lhs.mPivotSource = mPivotSource.Clone();

            if (mProtection != null)
                lhs.mProtection = mProtection.Clone();

            if (mTxPr != null)
                lhs.mTxPr = mTxPr.Clone();

            if (mThemeOverride != null)
                lhs.mThemeOverride = mThemeOverride.Clone();

            if (mColorMapOverride != null)
                lhs.mColorMapOverride = mColorMapOverride.Clone();

            if (mColorStyle != null)
                lhs.mColorStyle = mColorStyle.Clone();

            if (mDmlChartStyle != null)
                lhs.mDmlChartStyle = mDmlChartStyle.Clone();

            if (mData != null)
                lhs.mData = mData.Clone();

            if (mFormatOverrides != null)
            {
                lhs.mFormatOverrides = new List<DmlChartFormatOverride>();
                foreach (DmlChartFormatOverride item in mFormatOverrides)
                    lhs.mFormatOverrides.Add(item.Clone());
            }

            if (mExtLst != null)
            {
                lhs.mExtLst = new StringToObjDictionary<DmlExtension>();
                foreach (DmlExtension ext in mExtLst.Values)
                {
                    DmlExtension cloned = ext.Clone();
                    lhs.mExtLst[cloned.Uri] = cloned;
                }
            }

            return lhs;
        }

        internal void RemoveExternalDataLinkage()
        {
            // Just remove embedded document or link to a file, but keep reference formulas like '<c:f>Sheet1!$B$1</c:f>'
            // to preserve structure of XLSX file if possible.
            EmbeddedData = null;
            mLinkedData = null;
        }

        /// <summary>
        /// Sets the specified chart style to the chart space.
        /// </summary>
        internal void SetChartStyle(ChartStyle chartStyle)
        {
            const ChartSeriesType unknownSeriesType = (ChartSeriesType)(-1);
            ChartSeriesType seriesType = unknownSeriesType;
            if (FirstChart != null)
            {
                seriesType = (FirstChart.Series.Count > 0)
                    ? FirstChart.Series[0].SeriesType
                    : FirstChart.SeriesType;
            }

            SetChartStyle(chartStyle, DmlChartUtil.SeriesTypeToChartType(seriesType), true);
        }

        /// <summary>
        /// Sets the specified chart style to the chart space.
        /// </summary>
        internal void SetChartStyle(ChartStyle chartStyle, ChartType chartType, bool apply)
        {
            int styleId = ChartStyleResolver.GetStyleId(chartType, chartStyle);
            mDmlChartStyle = PresetChartReader.ReadChartStyle(styleId, (Document)Dml.Document);

            if (apply)
            {
                ChartStyleApplier styleApplier = new ChartStyleApplier(this);
                styleApplier.ApplyStyle();
            }
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified series.
        /// </summary>
        internal void ApplyChartStyle(ChartSeries series)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(series);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified data labels.
        /// </summary>
        internal void ApplyChartStyle(ChartDataLabelCollection dataLabels)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(dataLabels);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified axis.
        /// </summary>
        internal void ApplyChartStyle(ChartAxis axis)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(axis);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified legend.
        /// </summary>
        internal void ApplyChartStyle(ChartLegend legend)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(legend);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified data table.
        /// </summary>
        internal void ApplyChartStyle(ChartDataTable dataTable)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(dataTable);
        }

        /// <summary>
        /// Applies the specified chart style item to the text properties.
        /// </summary>
        internal void ApplyChartStyle(DmlChartTxPr txPr, DmlChartStyleItem styleItem)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(txPr, styleItem);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified marker.
        /// </summary>
        internal void ApplyChartStyle(ChartMarker marker, int seriesIndex)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyle(marker, seriesIndex);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the title of the specified axis.
        /// </summary>
        internal void ApplyChartStyleToAxisTitle(ChartAxis axis)
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyleToAxisTitle(axis);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the chart title.
        /// </summary>
        internal void ApplyChartStyleToChartTitle()
        {
            if (mDmlChartStyle == null)
                return;

            ChartStyleApplier styleApplier = new ChartStyleApplier(this);
            styleApplier.ApplyStyleToChartTitle();
        }

        internal override DmlNodeType DmlNodeType
        {
            get { return mIsChartEx ? DmlNodeType.ChartEx : DmlNodeType.Chart; }
        }

        #region IDmlCommonShapePrSource implementation

        public DmlFill Fill
        {
            get { return SpPr.Fill; }
            set { SpPr.Fill = value; }
        }

        public DmlOutline Outline
        {
            get { return SpPr.Outline; }
            set { SpPr.Outline = value; }
        }

        DmlShapeStyle IDmlCommonShapePrSource.Style
        {
            get
            {
                // Currently only index of the ShapeStyle is stored in the model, to preserve it during
                // open/save, please see StyleIndex property.
                // That is why null is always returned.
                return null;
            }
            set
            {
                // Do nothing.
            }
        }

        #endregion

        internal DmlChartFormat ChartFormat
        {
            get { return mChartFormat; }
            set
            {
                value.SetChartSpace(this);
                mChartFormat = value;
            }
        }

        /// <summary>
        /// Gets a list of DML charts of this chart space.
        /// </summary>
        internal IList<DmlChart> Charts
        {
            get { return mChartFormat.PlotArea.Charts; }
        }

        /// <summary>
        /// Gets the first DML chart of this chart space.
        /// </summary>
        internal DmlChart FirstChart
        {
            get { return mChartFormat.PlotArea.FirstChart; }
        }

        internal bool Date1904
        {
            get { return mDate1904; }
            set { mDate1904 = value; }
        }

        internal IEmbeddedObject EmbeddedData { get; set; }

        internal string LinkedData
        {
            get { return mLinkedData; }
            set { mLinkedData = value; }
        }

        internal bool HasExternalData
        {
            get { return ((EmbeddedData != null) || (LinkedData != null)); }
        }

        internal string Lang
        {
            get { return mLang; }
            set { mLang = value; }
        }

        internal DmlChartPivotSource PivotSource
        {
            get { return mPivotSource; }
            set { mPivotSource = value; }
        }

        internal DmlChartProtection Protection
        {
            get { return mProtection; }
            set { mProtection = value; }
        }

        internal bool RoundedCorners
        {
            get { return mRoundedCorners; }
            set { mRoundedCorners = value; }
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

        internal int StyleIndex
        {
            get { return mStyleIndex; }
            set
            {
                if (value >= 1 && value <= 48)
                    mStyleIndex = value;
            }
        }

        // Negative value means property is not set.
        internal int Word2010Style
        {
            get { return mWord2010Style; }
            set { mWord2010Style = value; }
        }

        internal DmlChartTxPr TxPr
        {
            get
            {
                if (mTxPr == null)
                    mTxPr = new DmlChartTxPr();

                return mTxPr;
            }
            set { mTxPr = value; }
        }

        /// <summary>
        /// This is external data of the chart.
        /// Temporary solution for writing charts.
        /// </summary>
        internal byte[] ChartExternalData
        {
            get { return mChartExternalData; }
            set { mChartExternalData = value; }
        }

        /// <summary>
        /// Theme override for chart.
        /// </summary>
        internal Theme ThemeOverride
        {
            get { return mThemeOverride; }
            set { mThemeOverride = value; }
        }

        /// <summary>
        /// Flag indicates whether chart must be auto updated from external data.
        /// Default is true.
        /// </summary>
        internal bool AutoUpdate
        {
            get { return mAutoUpdate; }
            set { mAutoUpdate = value; }
        }

        internal DmlChartColorMapOverride ColorMapOverride
        {
            get { return mColorMapOverride; }
            set { mColorMapOverride = value; }
        }

        /// <summary>
        /// Gets or sets <see cref="DmlChartColorStyle"/> object that specifies color style for the chart.
        /// </summary>
        internal DmlChartColorStyle ColorStyle
        {
            get { return mColorStyle; }
            set { mColorStyle = value; }
        }

        /// <summary>
        /// Gets or sets <see cref="ComplexTypes.DmlChartStyle"/> object that specifies style for the chart.
        /// </summary>
        internal DmlChartStyle DmlChartStyle
        {
            get { return mDmlChartStyle; }
            set { mDmlChartStyle = value; }
        }

        /// <summary>
        /// Flag indicates whether Word2010+ styles are used for this chart space.
        /// </summary>
        internal bool UseWord2010Style
        {
            get { return Word2010Style > 0 || IsChartEx ;}
        }

        /// <summary>
        /// Indicates that current document is created in MS Word 2007 or in lower version.
        /// </summary>
        internal bool IsWord2007OrLower
        {
            get
            {
                Document doc = (Document) Dml.Document;
                return (doc != null) && doc.BuiltInDocumentProperties.IsWord2007OrLower;
            }
        }



        public override string AlternativeText
        {
            get
            {
                return Dml.IsTopLevel ? base.AlternativeText : string.Empty;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.AlternativeText = value;
            }
        }

        public override string Title
        {
            get
            {
                return Dml.IsTopLevel ? base.Title : string.Empty;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.Title = value;
            }
        }

        public override string Name
        {
            get
            {
                return Dml.IsTopLevel ? base.Name : string.Empty;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.Name = value;
            }
        }

        internal override bool Hidden
        {
            get { return Dml.IsTopLevel && base.Hidden; }
            set
            {
                // Probably chart always placed into the "graphic frame" i.e. needs a container in which to be displayed.
                if (Dml.IsTopLevel)
                    base.Hidden = value;
            }
        }

        internal override bool AspectRatioLocked
        {
            get
            {
                return Dml.IsTopLevel && base.AspectRatioLocked;
            }
            set
            {
                if (Dml.IsTopLevel)
                    base.AspectRatioLocked = value;
            }
        }

        /// <summary>
        /// Indicates that this chart is an extension of the ISO/IEC 29000 format that is defined by
        /// the http://schemas.microsoft.com/office/drawing/2015/9/8/chartex schema of [MS-ODRAWXML].
        /// The extension is introduced in MS Word 2016.
        /// </summary>
        internal bool IsChartEx
        {
            get { return mIsChartEx; }
        }

        /// <summary>
        /// Stores data of the 'data' element of the 2.24.3.10 CT_ChartData complex type [MS-ODRAWXML].
        /// </summary>
        internal DmlChartDataCollection Data
        {
            get
            {
                if (mData == null)
                    mData = new DmlChartDataCollection();
                return mData;
            }
        }

        /// <summary>
        /// Stores data of the 'fmtOvrs' element of the 2.24.3.27 CT_FormatOverrides complex type [MS-ODRAWXML].
        /// Each item is of the <see cref="DmlChartFormatOverride"/> data type.
        /// </summary>
        internal IList<DmlChartFormatOverride> FormatOverrides
        {
            get
            {
                if (mFormatOverrides == null)
                    mFormatOverrides = new List<DmlChartFormatOverride>();
                return mFormatOverrides;
            }
        }

        private StringToObjDictionary<DmlExtension> mExtLst;
        private byte[] mChartExternalData;
        private bool mAutoUpdate = true;
        private DmlChartColorMapOverride mColorMapOverride;
        private DmlChartFormat mChartFormat;
        private bool mDate1904;
        private string mLang;
        private DmlChartPivotSource mPivotSource;
        private DmlChartProtection mProtection;
        private bool mRoundedCorners;
        private DmlChartSpPr mSpPr;
        private int mStyleIndex = 2;
        // Negative value means property is not set.
        private int mWord2010Style = int.MinValue;
        private DmlChartTxPr mTxPr;

        private readonly bool mIsChartEx;
        private string mLinkedData;
        private Theme mThemeOverride;
        private DmlChartColorStyle mColorStyle;
        private DmlChartStyle mDmlChartStyle;
        private DmlChartDataCollection mData;
        private List<DmlChartFormatOverride> mFormatOverrides;
    }
}
