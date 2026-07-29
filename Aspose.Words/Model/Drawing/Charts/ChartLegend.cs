// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents chart legend properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// 5.7.2.94 legend (Legend) element.
    /// </dev>
    public class ChartLegend : IDmlExtensionListSource, IChartFormatSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegend"/> class.
        /// </summary>
        /// <dev>
        /// A customer should not be able to create an instance of this class.
        /// </dev>
        internal ChartLegend()
        {
        }

        /// <summary>
        /// Initializes a new instance of the this class with defining a parent <see cref="DmlChartFormat"/> object.
        /// </summary>
        internal ChartLegend(DmlChartFormat chartFormat)
        {
            mChartFormat = chartFormat;
            mLegendEntries = new ChartLegendEntryCollection(chartFormat);
        }

        internal void AddEntry(ChartLegendEntry entry)
        {
            mLegendEntries.AddEntry(entry);
        }

        internal ChartLegendEntry GetEntry(int index)
        {
            return mLegendEntries.GetEntry(index);
        }

        internal ChartLegend Clone(DmlChartFormat parentDmlChartFormat)
        {
            ChartLegend lhs = (ChartLegend)MemberwiseClone();
            lhs.mChartFormat = parentDmlChartFormat;

            if (mLayout != null)
                lhs.mLayout = mLayout.Clone();

            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mTxPr != null)
                lhs.mTxPr = mTxPr.Clone();

            lhs.mLegendEntries = mLegendEntries.Clone(parentDmlChartFormat);

            if (mExtensions != null)
                lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            lhs.mFont = null;

            return lhs;
        }

        /// <summary>
        /// Sets the legend position without applying the chart style.
        /// </summary>
        internal void SetPositionInternal(LegendPosition value)
        {
            mIsVisible = (value != LegendPosition.None);

            mPositionAlignment = (value == LegendPosition.TopRight)
                ? PositionAlignment.Maximum
                : PositionAlignment.Center;

            switch (value)
            {
                case LegendPosition.Bottom:
                    mSidePosition = SidePosition.Bottom;
                    break;
                case LegendPosition.Left:
                    mSidePosition = SidePosition.Left;
                    break;
                case LegendPosition.Right:
                    mSidePosition = SidePosition.Right;
                    break;
                case LegendPosition.Top:
                case LegendPosition.TopRight:
                    mSidePosition = SidePosition.Top;
                    break;
                case LegendPosition.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns a collection of legend entries for all series and trendlines of the parent chart.
        /// </summary>
        public ChartLegendEntryCollection LegendEntries
        {
            get { return mLegendEntries; }
        }

        /// <summary>
        /// Specifies the position of the legend on a chart.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="LegendPosition.Right"/> for pre-Word 2016 charts and
        /// <see cref="LegendPosition.Top"/> for Word 2016 charts.
        /// </remarks>
        public LegendPosition Position
        {
            get
            {
                if (!mIsVisible)
                    return LegendPosition.None;

                switch (mSidePosition)
                {
                    case SidePosition.Left:
                        return LegendPosition.Left;
                    case SidePosition.Top:
                        return (mPositionAlignment == PositionAlignment.Maximum)
                            ? LegendPosition.TopRight
                            : LegendPosition.Top;
                    case SidePosition.Right:
                        return LegendPosition.Right;
                    case SidePosition.Bottom:
                        return LegendPosition.Bottom;
                    default:
                        return DefaultPosition;
                }
            }
            set
            {
                if (Position == value)
                    return;

                bool oldIsVisible = mIsVisible;

                SetPositionInternal(value);

                if (!oldIsVisible && mIsVisible)
                    mChartFormat.DmlChartSpace.ApplyChartStyle(this);
            }
        }

        /// <summary>
        /// Provides access to the default font formatting of legend entries. To override the font formatting for
        /// a specific legend entry, use the<see cref="ChartLegendEntry.Font"/> property.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                {
                    IRunAttrSource runAttrSource =
                        new ChartCollectionDmlRunPropertiesSource(TxPr, mChartFormat.DmlChartSpace);
                    mFont = Font.MakeFont(runAttrSource, mChartFormat.Document);
                }

                return mFont;
            }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the legend.
        /// </summary>
        public ChartFormat Format
        {
            get
            {
                if (mFormat == null)
                    mFormat = new ChartFormat(this);

                return mFormat;
            }
        }

        /// <summary>
        /// Gets or sets the legend side position.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="Core.SimpleTypes.SidePosition.Right"/> for pre-Word 2016 charts and
        /// <see cref="Core.SimpleTypes.SidePosition.Top"/> for Word 2016 charts.
        /// </remarks>
        internal SidePosition SidePosition
        {
            get { return mSidePosition; }
            set { mSidePosition = value; }
        }

        /// <summary>
        /// Gets or sets the alignment along the side position of the legend.
        /// </summary>
        /// <remarks>
        /// The default position alignment is <see cref="Core.SimpleTypes.PositionAlignment.Center"/>.
        /// The property has limited use for charts prior to Word 2016: only the Center and Maximum values can be used,
        /// and the Maximum value can only be used with the Top side position.
        /// </remarks>
        internal PositionAlignment PositionAlignment
        {
            get { return mPositionAlignment; }
            set { mPositionAlignment = value; }
        }

        /// <summary>
        /// Determines whether other chart elements shall be allowed to overlap legend.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool Overlay
        {
            get { return mOverlay; }
            set { mOverlay = value; }
        }

        internal DmlChartManualLayout Layout
        {
            get { return mLayout; }
            set { mLayout = value; }
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

        /// <summary>
        /// Gets or sets a flag indicating whether the legend is visible on the chart.
        /// </summary>
        internal bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; }
        }

        /// <summary>
        /// Represents extLst: a CT_OfficeArtExtensionList ([ISO/IEC29500-1:2012] section A.4.1) element that specifies
        /// the extension list in which all future extensions of element type ext is defined.
        ///  </summary>
        /// <remarks>
        /// Explicit implementation hides this from public.
        /// </remarks>
        StringToObjDictionary<DmlExtension> IDmlExtensionListSource.Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // Do nothing for a legend.
        }

        bool IChartFormatSource.IsFillSupported
        {
            get { return true; }
        }

        DmlFill IChartFormatSource.Fill
        {
            get { return SpPr.Fill; }
            set { SpPr.Fill = value; }
        }

        DmlOutline IChartFormatSource.Outline
        {
            get { return SpPr.Outline; }
            set { SpPr.Outline = value; }
        }

        ChartShapeType IChartFormatSource.ShapeType
        {
            get
            {
                // Not supported by a legend.
                return ChartShapeType.Default;
            }
            set
            {
                // Not supported by a legend.
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return mChartFormat.Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return !SpPr.IsEmpty; }
        }

        #endregion

        private StringToObjDictionary<DmlExtension> mExtensions;
        private DmlChartManualLayout mLayout;
        private bool mOverlay;
        private DmlChartSpPr mSpPr;
        private DmlChartTxPr mTxPr;
        private ChartLegendEntryCollection mLegendEntries;
        private bool mIsVisible;
        private SidePosition mSidePosition = SidePosition.Right;
        private PositionAlignment mPositionAlignment = PositionAlignment.Center;
        private Font mFont;
        private ChartFormat mFormat;
        private DmlChartFormat mChartFormat;

        internal const LegendPosition DefaultPosition = LegendPosition.Right;
        internal const SidePosition ChartExDefaultSidePosition = SidePosition.Top;
    }
}
