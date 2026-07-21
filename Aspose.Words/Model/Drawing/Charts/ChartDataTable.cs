// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Allows to specify properties of a chart data table.
    /// </summary>
    public class ChartDataTable : IChartItemTextProperties, IChartFormatSource
    {
        /// <summary>
        /// Non-public ctor to prevent creating instances of this class in customer code.
        /// </summary>
        internal ChartDataTable(DmlChartPlotArea plotArea)
        {
            mPlotArea = plotArea;
        }

        /// <summary>
        /// Creates a data table and sets the default font and border formatting.
        /// </summary>
        internal static ChartDataTable CreateDataTableWithDefaultFormat(DmlChartPlotArea plotArea)
        {
            ChartDataTable dataTable = new ChartDataTable(plotArea);

            // Define border line defaults.

            DmlOutline outline = dataTable.SpPr.Outline;
            outline.Weight = 0.75;

            DmlSchemeColor borderColor = new DmlSchemeColor(ThemeColor.Text1);

            DmlLuminanceModulation modulation = new DmlLuminanceModulation();
            modulation.Value = 0.15;
            borderColor.ColorModifiers.Add(modulation);

            DmlLuminanceOffset offset = new DmlLuminanceOffset();
            offset.Value = 0.85;
            borderColor.ColorModifiers.Add(offset);

            outline.Fill = new DmlSolidFill(borderColor);

            // Define font defaults.

            DmlParagraph paragraph = dataTable.TxPr.AddParagraph();
            DmlParagraphProperties properties = paragraph.Properties;
            properties.HasDefaultRunProperties = true;
            properties.DefaultRunProperties.FontSize = new DmlTextPoints(900);

            DmlSchemeColor textColor = new DmlSchemeColor(ThemeColor.Text1);

            modulation = new DmlLuminanceModulation();
            modulation.Value = 0.65;
            textColor.ColorModifiers.Add(modulation);

            offset = new DmlLuminanceOffset();
            offset.Value = 0.35;
            textColor.ColorModifiers.Add(offset);

            properties.DefaultRunProperties.Fill = new DmlSolidFill(textColor);

            return dataTable;
        }

        /// <summary>
        /// Clones this instance of the data table.
        /// </summary>
        internal ChartDataTable Clone()
        {
            ChartDataTable lhs = (ChartDataTable)MemberwiseClone();
            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mTxPr != null)
                lhs.mTxPr = mTxPr.Clone();

            lhs.mFontRunPropertiesSource = null;
            lhs.mFont = null;
            lhs.mFormat = null;

            return lhs;
        }

        /// <summary>
        /// Sets the parent plot area of this data table.
        /// </summary>
        internal void SetPlotArea(DmlChartPlotArea plotArea)
        {
            mPlotArea = plotArea;
        }

        /// <summary>
        /// Gets a flag indicating whether the parent chart supports a data table.
        /// </summary>
        internal bool IsDataTableSupported()
        {
            foreach (DmlChart chart in mPlotArea.Charts)
            {
                if (chart.IsScatter ||
                    chart.IsPieChart ||
                    chart.IsSurfaceChart ||
                    chart.IsSurface3DChart ||
                    chart.IsRadarChart ||
                    chart.IsChartEx)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets a flag indicating whether the data table will be displayed on the chart without checking for data table
        /// support and applying the chart style.
        /// </summary>
        internal void SetShowInternal(bool value)
        {
            mShow = value;
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the data table will be shown for the chart.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// The following chart types do not support data tables: Scatter, Pie, Doughnut, Surface, Radar, Treemap,
        /// Sunburst, Histogram, Pareto, Box and Whisker, Waterfall, Funnel, Combo charts that include series of
        /// these types. Showing a data table for the chart types throws a <see cref="InvalidOperationException"/>
        /// exception.
        /// </remarks>
        public bool Show
        {
            get { return mShow; }
            set
            {
                if (mShow == value)
                    return;

                if (value && !IsDataTableSupported())
                    throw new InvalidOperationException("This chart type does not support a data table.");

                mShow = value;
                if (mShow)
                    mPlotArea.ParentChartFormat.DmlChartSpace.ApplyChartStyle(this);
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether legend keys are displayed in the data table.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool HasLegendKeys
        {
            get { return mHasLegendKeys; }
            set { mHasLegendKeys = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether a horizontal border of the data table is displayed.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool HasHorizontalBorder
        {
            get { return mHasHorizontalBorder; }
            set { mHasHorizontalBorder = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether a vertical border of the data table is displayed.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool HasVerticalBorder
        {
            get { return mHasVerticalBorder; }
            set { mHasVerticalBorder = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether an outline border, that is, a border around series and category names,
        /// is displayed.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool HasOutlineBorder
        {
            get { return mHasOutlineBorder; }
            set { mHasOutlineBorder = value; }
        }

        /// <summary>
        /// Provides access to font formatting of the data table.
        /// </summary>
        public Font Font
        {
            get
            {
                if (mFont == null)
                    mFont = new Font(FontRunPropertiesSource, mPlotArea.ParentChartFormat.Document);

                return mFont;
            }
        }

        /// <summary>
        /// Provides access to fill of text background and border formatting of the data table.
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
        /// Gets the shape properties of the data table.
        /// </summary>
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
        /// Gets the text properties of the data table.
        /// </summary>
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
        /// Gets a <see cref="ChartItemDmlRunPropertiesSource"/> instance that is the source of the data table
        /// font properties.
        /// </summary>
        private ChartItemDmlRunPropertiesSource FontRunPropertiesSource
        {
            get
            {
                if (mFontRunPropertiesSource == null)
                {
                    mFontRunPropertiesSource =
                        new ChartItemDmlRunPropertiesSource(this, mPlotArea.ParentChartFormat.DmlChartSpace);
                }

                return mFontRunPropertiesSource;
            }
        }

        #region IChartItemTextProperties members

        string IChartItemTextProperties.GenerateItemText()
        {
            return null; // Not used for data table.
        }

        object IChartItemTextProperties.FetchSpecialDefaultRunPropertyValue(int key)
        {
            return null; // Not used for data table.
        }

        object IChartItemTextProperties.GetRelativePropertyValue(int key, object value)
        {
            return value;
        }

        DmlChartTx IChartItemTextProperties.ItemTx
        {
            get { return null; }
        }

        DmlChartTxPr IChartItemTextProperties.ItemTxPr
        {
            get { return TxPr; }
            set { mTxPr = value; }
        }

        DmlChartSpPr IChartItemTextProperties.ItemSpPr
        {
            get { return SpPr; }
        }

        DmlChartTxPr IChartItemTextProperties.CollectionTxPr
        {
            get { return null; }
        }

        #endregion

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // Do nothing for a data table.
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
                // Not supported by axis.
                return ChartShapeType.Default;
            }
            set
            {
                // Not supported by axis.
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return mPlotArea.ParentChartFormat.Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return !SpPr.IsEmpty; }
        }

        #endregion

        private bool mShow;
        private bool mHasLegendKeys = true;
        private bool mHasHorizontalBorder = true;
        private bool mHasVerticalBorder = true;
        private bool mHasOutlineBorder = true;
        private DmlChartSpPr mSpPr;
        private DmlChartTxPr mTxPr;
        private Font mFont;
        private ChartItemDmlRunPropertiesSource mFontRunPropertiesSource;
        private ChartFormat mFormat;
        private DmlChartPlotArea mPlotArea;
    }
}
