// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections;
using Aspose.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents the axis options of the chart.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartAxis : IDmlChartTitleHolder, IDmlExtensionListSource, INumberFormatProvider, IChartFormatSource
    {
        internal ChartAxis(ChartAxisType axisType, DmlChart chart, DocumentBase document, bool isChartEx)
        {
            mAxisType = axisType;
            mChart = chart;
            ChartAxisPr = new DmlChartAxisPr(document, isChartEx);
        }

        #region Public API.

        /// <summary>
        /// Returns type of the axis.
        /// </summary>
        public ChartAxisType Type
        {
            get { return mAxisType; }
        }

        /// <summary>
        /// Gets or sets type of the category axis.
        /// </summary>
        /// <remarks>
        /// Only text categories (<see cref="AxisCategoryType.Category"/>) are allowed in MS Office 2016 new charts.
        /// </remarks>
        /// <dev>
        /// The property is internally represented as two bool attributes since the <see cref="AxisCategoryType"/> type
        /// provides no enough information that is needed to store the property in document file.
        /// </dev>
        public AxisCategoryType CategoryType
        {
            get
            {
                if ((bool)ChartAxisPr.GetProperty(DmlChartAxisAttrs.IsAutoCategoryType))
                    return AxisCategoryType.Automatic;
                else if ((bool)ChartAxisPr.GetProperty(DmlChartAxisAttrs.IsDateCategoryAxis))
                    return AxisCategoryType.Time;
                else
                    return AxisCategoryType.Category;
            }
            set
            {
                switch (value)
                {
                    case AxisCategoryType.Automatic:
                        ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsAutoCategoryType, true);
                        break;
                    case AxisCategoryType.Time:
                    {
                        ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsAutoCategoryType, false);
                        ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsDateCategoryAxis, true);
                        break;
                    }
                    case AxisCategoryType.Category:
                    {
                        ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsAutoCategoryType, false);
                        ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsDateCategoryAxis, false);
                        break;
                    }
                    default:
                        throw new ArgumentException(String.Format("Wrong value is specified: {0}.", value));
                }
            }
        }

        /// <summary>
        /// Specifies how this axis crosses the perpendicular axis.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <see cref="AxisCrosses.Automatic"/>.</para>
        /// <para>The property is not supported by MS Office 2016 new charts.</para>
        /// </remarks>
        public AxisCrosses Crosses
        {
            get { return (AxisCrosses)ChartAxisPr.GetProperty(DmlChartAxisAttrs.Crosses); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.Crosses, value); }
        }

        /// <summary>
        /// Specifies where on the perpendicular axis the axis crosses.
        /// </summary>
        /// <remarks>
        /// <para>The property has effect only if <see cref="Crosses"/> are set to <see cref="AxisCrosses.Custom"/>.
        /// It is not supported by MS Office 2016 new charts.</para>
        /// <para>The units are determined by the type of axis. When the axis is a value axis, the value of the property
        /// is a decimal number on the value axis. When the axis is a time category axis, the value is defined as
        /// an integer number of days relative to the base date (30/12/1899). For a text category axis, the value is
        /// an integer category number, starting with 1 as the first category.</para>
        /// </remarks>
        public double CrossesAt
        {
            get { return (double)ChartAxisPr.GetProperty(DmlChartAxisAttrs.CrossesAt); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.CrossesAt, value); }
        }

        /// <summary>
        /// Returns or sets a flag indicating whether values of axis should be displayed in reverse order, i.e.
        /// from max to min.
        /// </summary>
        /// <remarks>
        /// The property is not supported by MS Office 2016 new charts. Default value is <c>false</c>.
        /// </remarks>
        public bool ReverseOrder
        {
            get { return (Scaling.Orientation == AxisOrientation.MaxMin); }
            set { Scaling.Orientation = (value ? AxisOrientation.MaxMin : AxisOrientation.MinMax); }
        }

        /// <summary>
        /// Returns or sets the major tick marks.
        /// </summary>
        public AxisTickMark MajorTickMark
        {
            get { return (AxisTickMark)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MajorTickMark); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MajorTickMark, value); }
        }

        /// <summary>
        /// Returns or sets the minor tick marks for the axis.
        /// </summary>
        public AxisTickMark MinorTickMark
        {
            get { return (AxisTickMark)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MinorTickMark); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MinorTickMark, value); }
        }

        /// <summary>
        /// Returns or sets the distance between major tick marks.
        /// </summary>
        /// <remarks>
        /// <para>Valid range of a value is greater than zero. The property has effect for time category and
        /// value axes.</para>
        /// <para>Setting this property sets the <see cref="MajorUnitIsAuto"/> property to <c>false</c>.</para>
        /// </remarks>
        public double MajorUnit
        {
            get { return MajorUnitPr.Value; }
            set
            {
                ArgumentUtil.CheckPositive(value, "value");
                MajorUnitPr.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether default distance between major tick marks shall be used.
        /// </summary>
        /// <remarks>
        /// The property has effect for time category and value axes.
        /// </remarks>
        public bool MajorUnitIsAuto
        {
            get { return MajorUnitPr.IsNullOrAuto; }
            set { MajorUnitPr.IsAuto = value; }
        }

        /// <summary>
        /// Returns or sets the scale value for major tick marks on the time category axis.
        /// </summary>
        /// <remarks>
        /// The property has effect only for time category axes.
        /// </remarks>
        public AxisTimeUnit MajorUnitScale
        {
            get { return (AxisTimeUnit)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MajorUnitScale); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MajorUnitScale, value); }
        }

        /// <summary>
        /// Returns or sets the distance between minor tick marks.
        /// </summary>
        /// <remarks>
        /// <para>Valid range of a value is greater than zero. The property has effect for time category and
        /// value axes.</para>
        /// <para>Setting this property sets the <see cref="MinorUnitIsAuto"/> property to <c>false</c>.</para>
        /// </remarks>
        public double MinorUnit
        {
            get { return MinorUnitPr.Value; }
            set
            {
                ArgumentUtil.CheckPositive(value, "value");
                MinorUnitPr.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether default distance between minor tick marks shall be used.
        /// </summary>
        /// <remarks>
        /// The property has effect for time category and value axes.
        /// </remarks>
        public bool MinorUnitIsAuto
        {
            get { return MinorUnitPr.IsNullOrAuto; }
            set { MinorUnitPr.IsAuto = value; }
        }

        /// <summary>
        /// Returns or sets the scale value for minor tick marks on the time category axis.
        /// </summary>
        /// <remarks>
        /// The property has effect only for time category axes.
        /// </remarks>
        public AxisTimeUnit MinorUnitScale
        {
            get { return (AxisTimeUnit)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MinorUnitScale); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MinorUnitScale, value); }
        }

        /// <summary>
        /// Returns or sets the smallest time unit that is represented on the time category axis.
        /// </summary>
        /// <remarks>
        /// The property has effect only for time category axes.
        /// </remarks>
        public AxisTimeUnit BaseTimeUnit
        {
            get { return (AxisTimeUnit)ChartAxisPr.GetProperty(DmlChartAxisAttrs.BaseTimeUnit); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.BaseTimeUnit, value); }
        }

        /// <summary>
        /// Returns a <see cref="ChartNumberFormat"/> object that allows defining number formats for the axis.
        /// </summary>
        public ChartNumberFormat NumberFormat
        {
            get
            {
                if (mNumberFormat == null)
                    mNumberFormat = new ChartNumberFormat(this, mChart);

                return mNumberFormat;
            }
        }

        /// <summary>
        /// Specifies the scaling value of the display units for the value axis.
        /// </summary>
        /// <remarks>
        /// The property has effect only for value axes.
        /// </remarks>
        public AxisDisplayUnit DisplayUnit
        {
            get
            {
                AxisDisplayUnit dispUnitObj = (AxisDisplayUnit)ChartAxisPr.GetProperty(DmlChartAxisAttrs.DisplayUnit);
                if (dispUnitObj == null)
                {
                    dispUnitObj = new AxisDisplayUnit();
                    dispUnitObj.SetAxis(this);
                    ChartAxisPr.SetProperty(DmlChartAxisAttrs.DisplayUnit, dispUnitObj);
                }
                return dispUnitObj;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the value axis crosses the category axis between categories.
        /// </summary>
        /// <remarks>
        /// The property has effect only for value axes. It is not supported by MS Office 2016 new charts.
        /// </remarks>
        public bool AxisBetweenCategories
        {
            get { return CrossBetween == CrossBetween.Between; }
            set { CrossBetween = value ? CrossBetween.Between : CrossBetween.MidpointOfCategory; }
        }

        /// <summary>
        /// Provides access to the scaling options of the axis.
        /// </summary>
        public AxisScaling Scaling
        {
            get { return (AxisScaling)ChartAxisPr.GetProperty(DmlChartAxisAttrs.Scaling); }
        }

        /// <summary>
        /// Gets or sets the interval, at which tick marks are drawn.
        /// </summary>
        /// <remarks>
        /// <para>The property has effect for text category and series axes. It is not supported by MS Office 2016
        /// new charts.</para>
        /// <para>Valid range of a value is greater than or equal to 1.</para>
        /// </remarks>
        /// <dev>
        /// The ISO standard states that this element specifies how many tick marks shall be skipped before the next
        /// one shall be drawn.
        /// In MS Office, this element specifies the interval at which tick marks are drawn.
        /// </dev>
        public int TickMarkSpacing
        {
            get { return (int)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TickMarkSpacing); }
            set
            {
                ArgumentUtil.CheckPositive(value, "value");
                ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickMarkSpacing, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether this axis is hidden or not.
        /// </summary>
        /// <remarks>
        /// Default value is <c>false</c>.
        /// </remarks>
        /// <dev>
        /// Corresponds to the 21.2.2.40 delete element [ISO/IEC 29500] and to the hidden attribute of the 2.24.3.3
        /// CT_Axis complex type [MS-ODRAWXML].
        /// </dev>
        public bool Hidden
        {
            get { return (bool)ChartAxisPr.GetProperty(DmlChartAxisAttrs.Hidden); }
            set
            {
                bool isChanged = (Hidden != value);
                SetHiddenInternal(value);
                if (isChanged && !value)
                    mPlotArea.ParentChartFormat.DmlChartSpace.ApplyChartStyle(this);
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the axis has major gridlines.
        /// </summary>
        public bool HasMajorGridlines
        {
            get { return (MajorGridlines != null); }
            set
            {
                if (HasMajorGridlines == value)
                    return;

                MajorGridlines = value ? GetDefaultGridLines(GridlineType.Major) : null;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the axis has minor gridlines.
        /// </summary>
        public bool HasMinorGridlines
        {
            get { return (MinorGridlines != null); }
            set
            {
                if (HasMinorGridlines == value)
                    return;

                MinorGridlines = value ? GetDefaultGridLines(GridlineType.Minor) : null;
            }
        }

        /// <summary>
        /// Provides access to the axis title properties.
        /// </summary>
        public ChartAxisTitle Title
        {
            get
            {
                if (mTitle == null)
                    mTitle = new ChartAxisTitle(this);

                return mTitle;
            }
        }

        /// <summary>
        /// Provides access to the properties of the axis tick mark labels.
        /// </summary>
        public AxisTickLabels TickLabels
        {
            get
            {
                if (mTickLabels == null)
                    mTickLabels = new AxisTickLabels(this);

                return mTickLabels;
            }
        }

        /// <summary>
        /// Provides access to line formatting of the axis and fill of the tick labels.
        /// </summary>
        /// <remarks>
        /// Fill of chart tick marks can be changed only for pre Word 2016 charts. Word 2016 charts do not support this.
        /// </remarks>
        public ChartFormat Format
        {
            get
            {
                if (mFormat == null)
                    mFormat = new ChartFormat(this);

                return mFormat;
            }
        }

        #endregion

        #region Raw Axis Properties (values corresponds the appropriate values in the source XML).

        /// <summary>
        /// Returns a flag indicating whether this axis is a datetime category axis.
        /// </summary>
        internal bool IsDateCategoryAxis
        {
            get { return IsCategory && (bool)ChartAxisPr.GetProperty(DmlChartAxisAttrs.IsDateCategoryAxis); }
        }

        /// <summary>
        /// Returns properties of this axis.
        /// </summary>
        internal DmlChartAxisPr ChartAxisPr { get; set; }

        /// <summary>
        /// Specifies the text alignment for the tick labels on the axis.
        /// </summary>
        /// <remarks>
        /// The property has effect only for text category axes.
        /// </remarks>
        internal LabelAlignment LblAlgn
        {
            get { return (LabelAlignment)ChartAxisPr.GetProperty(DmlChartAxisAttrs.LblAlgn); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.LblAlgn, value); }
        }

        /// <summary>
        /// Specifies the identifier for the axis.
        /// </summary>
        internal int AxId
        {
            get { return (int)ChartAxisPr.GetProperty(DmlChartAxisAttrs.AxId); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.AxId, value); }
        }

        /// <summary>
        /// Specifies the position of the axis on the chart.
        /// </summary>
        internal AxisPosition AxPos
        {
            get { return (AxisPosition)ChartAxisPr.GetProperty(DmlChartAxisAttrs.AxPos); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.AxPos, value); }
        }

        /// <summary>
        /// Specifies the ID of axis that this axis crosses.
        /// </summary>
        internal int CrossAx
        {
            get { return (int)ChartAxisPr.GetProperty(DmlChartAxisAttrs.CrossAx); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.CrossAx, value); }
        }

        /// <summary>
        /// Specifies whether the value axis crosses the category axis between categories.
        /// </summary>
        /// <remarks>
        /// The property has effect only for value axes.
        /// </remarks>
        internal CrossBetween CrossBetween
        {
            get { return (CrossBetween)ChartAxisPr.GetProperty(DmlChartAxisAttrs.CrossBetween); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.CrossBetween, value); }
        }

        /// <summary>
        /// Specifies major gridlines.
        /// </summary>
        internal DmlChartGridlines MajorGridlines
        {
            get
            {
                object majorGridlinesObj = ChartAxisPr.GetProperty(DmlChartAxisAttrs.MajorGridlines);
                if (majorGridlinesObj != null)
                    return (DmlChartGridlines)majorGridlinesObj;

                return null;
            }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MajorGridlines, value); }
        }

        /// <summary>
        /// Specifies the minor gridlines.
        /// </summary>
        internal DmlChartGridlines MinorGridlines
        {
            get
            {
                object minorGridlinesObj = ChartAxisPr.GetProperty(DmlChartAxisAttrs.MinorGridlines);
                if (minorGridlinesObj != null)
                    return (DmlChartGridlines)minorGridlinesObj;

                return null;
            }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MinorGridlines, value); }
        }

        /// <summary>
        /// Specifies the labels shall be shown as flat text.
        /// </summary>
        /// <remarks>
        /// The property has effect only for text category axes.
        /// </remarks>
        internal bool NoMultiLvlLbl
        {
            get { return (bool)ChartAxisPr.GetProperty(DmlChartAxisAttrs.NoMultiLvlLbl); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.NoMultiLvlLbl, value); }
        }

        /// <summary>
        /// Specifies the formatting for the parent chart element.
        /// </summary>
        internal DmlChartSpPr SpPr
        {
            get { return (DmlChartSpPr)ChartAxisPr.GetProperty(DmlChartAxisAttrs.SpPr); }
        }

        /// <summary>
        /// Allows defining the distance between major ticks.
        /// </summary>
        internal DoubleOrAutomatic MajorUnitPr
        {
            get { return (DoubleOrAutomatic)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MajorUnit); }
        }

        /// <summary>
        /// Allows defining the distance between minor ticks.
        /// </summary>
        internal DoubleOrAutomatic MinorUnitPr
        {
            get { return (DoubleOrAutomatic)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MinorUnit); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether automatic interval of drawing tick marks shall be used.
        /// </summary>
        /// <remarks>
        /// The property has effect for text category and series axes.
        /// </remarks>
        /// <dev>
        /// VBA has no TickMarkSpacingIsAuto public property.
        /// </dev>
        internal bool TickMarkSpacingIsAuto
        {
            get { return ChartAxisPr.GetDirectProperty(DmlChartAxisAttrs.TickMarkSpacing) == null; }
        }

        /// <summary>
        /// Specifies number formatting for the parent element.
        /// </summary>
        internal DmlChartNumFormat NumFmt
        {
            get
            {
                object numFmtObj = ChartAxisPr.GetProperty(DmlChartAxisAttrs.NumFmt);
                if (numFmtObj != null)
                    return (DmlChartNumFormat)numFmtObj;

                return null;
            }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.NumFmt, value); }
        }

        /// <summary>
        /// Represents a format code that is used to render axis values.
        /// </summary>
        internal string FormatCode
        {
            get
            {
                if (mFormatCode == null)
                    mFormatCode =(NumFmt != null) ? NumFmt.FormatCode : string.Empty;

                return mFormatCode;
            }

            set { mFormatCode = value; }
        }

        /// <summary>
        /// Specifies text formatting.
        /// </summary>
        internal DmlChartTxPr TxPr
        {
            get { return (DmlChartTxPr)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TxPr); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether tick labels are shown.
        /// Default value is <c>true</c>.
        /// </summary>
        /// <dev>
        /// There is no tick labels position property in charts of the schema
        /// http://schemas.microsoft.com/office/drawing/2014/chartex, but only a flag indicating if they are visible.
        /// </dev>
        internal bool AreTickLabelsVisible
        {
            get { return TickLabels.Position != AxisTickLabelPosition.None; }
            set { TickLabels.Position = value ? AxisTickLabelPosition.Default : AxisTickLabelPosition.None; }
        }

        /// <summary>
        /// Gets or sets tick labels extensions.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal StringToObjDictionary<DmlExtension> TickLabelExtensions
        {
            get { return (StringToObjDictionary<DmlExtension>)ChartAxisPr.GetProperty(DmlChartAxisAttrs.TickLblExtensions); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLblExtensions, value); }
        }

        /// <summary>
        /// Gets or sets major tick mark extensions.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal StringToObjDictionary<DmlExtension> MajorTickMarkExtensions
        {
            get { return (StringToObjDictionary<DmlExtension>)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MajorTickMarkExtensions); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MajorTickMarkExtensions, value); }
        }

        /// <summary>
        /// Gets or sets minor tick mark extensions.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal StringToObjDictionary<DmlExtension> MinorTickMarkExtensions
        {
            get { return (StringToObjDictionary<DmlExtension>)ChartAxisPr.GetProperty(DmlChartAxisAttrs.MinorTickMarkExtensions); }
            set { ChartAxisPr.SetProperty(DmlChartAxisAttrs.MinorTickMarkExtensions, value); }
        }

        #endregion

        #region IDmlChartTitleHolder members

        int IDmlChartTitleHolder.GetRelativeFontSize(int chartFontSize)
        {
            return chartFontSize; // An axis title uses font size defined on the chart without changes.
        }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        DmlChartTitle IDmlChartTitleHolder.DCTitle
        {
            get { return TitleInternal; }
            set { TitleInternal = value; }
        }

        /// <summary>
        /// Returns the position of the title.
        /// </summary>
        TitlePosition IDmlChartTitleHolder.TitlePosition
        {
            get
            {
                switch (ActualAxisPosition)
                {
                    case AxisPosition.Top:
                        return TitlePosition.Top;
                    case AxisPosition.Bottom:
                        return TitlePosition.Bottom;
                    case AxisPosition.Left:
                        return TitlePosition.Left;
                    case AxisPosition.Right:
                        return TitlePosition.Right;
                    default:
                        return TitlePosition.Top;
                }
            }
        }

        /// <summary>
        /// Returns the document containing the parent chart.
        /// </summary>
        public DocumentBase Document
        {
            get { return Chart.ChartSpace.Dml.Document; }
        }

        /// <summary>
        /// Returns <c>true</c> if tile should not be displayed even if it exists.
        /// </summary>
        bool IDmlChartTitleHolder.TitleDeleted
        {
            get { return !mHasTitle || (TitleInternal == null); }
            set
            {
                if (mHasTitle == !value)
                    return;

                mHasTitle = !value;
                if (mHasTitle)
                    mPlotArea.ParentChartFormat.DmlChartSpace.ApplyChartStyleToAxisTitle(this);
            }
        }

        /// <summary>
        /// Gets the default title text.
        /// </summary>
        string IDmlChartTitleHolder.DefaultTitleText
        {
            get { return DefaultAxisTitles.GetTitle(); }
        }

        /// <summary>
        /// Gets the default font size in points.
        /// </summary>
        double IDmlChartTitleHolder.DefaultFontSize
        {
            get { return GetDefaultFontSizeInternal(); }
        }

        /// <summary>
        /// Gets the title font size in points that MS Word sets for created charts.
        /// </summary>
        double IDmlChartTitleHolder.DefaultDisplayedFontSize
        {
            get { return GetDefaultFontSizeInternal(); }
        }

        DmlChartStyleItem IDmlChartTitleHolder.StyleItem
        {
            get { return DmlChartStyleItem.AxisTitle; }
        }

        /// <summary>
        /// Gets the parent chart space.
        /// </summary>
        DmlChartSpace IDmlChartTitleHolder.ChartSpace
        {
            get { return Chart.ChartSpace; }
        }

        /// <summary>
        /// Indicates whether the axis title is visible.
        /// </summary>
        bool IDmlChartTitleHolder.IsVisible
        {
            get { return !((IDmlChartTitleHolder)this).TitleDeleted; }
        }

        #endregion

        #region INumberFormatProvider members

        DmlChartNumFormat INumberFormatProvider.NumFmt_INumberFormatProvider
        {
            get { return NumFmt; }
            set { NumFmt = value; }
        }

        bool INumberFormatProvider.IsInherited
        {
            get { return false; }
        }

        #endregion

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            if (ChartAxisPr.IsPropertySpecified(DmlChartAxisAttrs.SpPr))
                return;

            DmlChartSpPr spPr = new DmlChartSpPr();
            ChartAxisPr.SetProperty(DmlChartAxisAttrs.SpPr, spPr);
        }

        bool IChartFormatSource.IsFillSupported
        {
            get { return !Chart.ChartSpace.IsChartEx; }
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
            get { return Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return ChartAxisPr.IsPropertySpecified(DmlChartAxisAttrs.SpPr) && !SpPr.IsEmpty; }
        }

        #endregion

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        internal DmlChartTitle TitleInternal
        {
            get { return mTitleInternal; }
            set { mTitleInternal = value; }
        }

        internal ChartAxis Clone()
        {
            ChartAxis lhs = (ChartAxis)MemberwiseClone();

            // FOSS

            // FOSS

            if (ChartAxisPr != null)
            {
                lhs.ChartAxisPr = ChartAxisPr.Clone();

                AxisDisplayUnit displayUnit =
                    (AxisDisplayUnit)lhs.ChartAxisPr.GetDirectProperty(DmlChartAxisAttrs.DisplayUnit);

                if (displayUnit != null)
                    displayUnit.SetAxis(lhs);
            }

            if (mTitleInternal != null)
            {
                lhs.mTitleInternal = mTitleInternal.Clone();
                lhs.mTitleInternal.SetTitleHolder(lhs);
            }

            // FOSS

            lhs.mCrossAxis = null;
            lhs.mNumberFormat = null;
            lhs.mTickLabels = null;
            lhs.mFormat = null;

            if (mExtensions != null)
                lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            return lhs;
        }

        /// <summary>
        /// Sets axis type.
        /// </summary>
        /// <dev>
        /// It is separate from the <see cref="Type"/> property, since the property is intended to be public.
        /// </dev>
        internal void SetType(ChartAxisType value)
        {
            mAxisType = value;
        }

        /// <summary>
        /// Sets display unit.
        /// </summary>
        internal void SetDisplayUnit(AxisDisplayUnit value)
        {
            if (value != null)
                value.SetAxis(this);
            ChartAxisPr.SetProperty(DmlChartAxisAttrs.DisplayUnit, value);
        }

        /// <summary>
        /// Sets the parent chart of this axis.
        /// </summary>
        internal void SetChart(DmlChart chart)
        {
            mChart = chart;

            foreach (ChartSeries series in chart.Series)
                Series.Add(series);
        }

        /// <summary>
        /// Sets a flag indicating whether the axis is hidden without applying the chart style.
        /// </summary>
        internal void SetHiddenInternal(bool value)
        {
            ChartAxisPr.SetProperty(DmlChartAxisAttrs.Hidden, value);
        }

        /// <summary>
        /// Gets default chart gridlines.
        /// </summary>
        private DmlChartGridlines GetDefaultGridLines(GridlineType type)
        {
            DmlOutline outline = new DmlOutline();
            outline.WidthInEmus = 9525;
            outline.EndCap = EndCap.Flat;
            outline.CompoundLineType = ShapeLineStyle.Single;
            outline.StrokeAlignment = false;

            List<IDmlColorModifier> modifiers = new List<IDmlColorModifier>();
            DmlLuminanceModulation luminanceModulation = new DmlLuminanceModulation();
            luminanceModulation.Value = (type == GridlineType.Major) ? 0.15d : 0.05d;
            modifiers.Add(luminanceModulation);
            DmlLuminanceOffset luminanceOffset = new DmlLuminanceOffset();
            luminanceOffset.Value = (type == GridlineType.Major) ? 0.85d : 0.95d;
            modifiers.Add(luminanceOffset);

            DmlSchemeColor color = new DmlSchemeColor(ThemeColor.Text1);
            color.ColorModifiers = modifiers;

            DmlSolidFill fill = new DmlSolidFill();
            fill.Color = color;
            outline.Fill = fill;

            DmlChartGridlines gridlines = new DmlChartGridlines();
            gridlines.SpPr.Outline = outline;

            return gridlines;
        }

        /// <summary>
        /// Gets the default font size of axis title and tick labels.
        /// </summary>
        internal int GetDefaultFontSizeInternal()
        {
            return Chart.ChartSpace.IsChartEx ? 9 : 10;
        }

        /// <summary>
        /// Gets or sets the plot area this axis belongs to.
        /// </summary>
        internal DmlChartPlotArea PlotArea
        {
            get { return mPlotArea; }
            set { mPlotArea = value; }
        }

        /// <summary>
        /// Gets or sets target rectangle where axis will be rendered.
        /// </summary>
        internal RectangleF TargetRectangle
        {
            get { return mPlotAreaTargetRect; }
            set { mPlotAreaTargetRect = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if the axis is vertical.
        /// </summary>
        internal bool IsVertical
        {
            get
            {
                return ((AxPos == AxisPosition.Left) || (AxPos == AxisPosition.Right) ||
                   (mChart != null && mChart.IsChartEx && !IsCategory && mChart.SeriesType != ChartSeriesType.Funnel));
            }
        }

        /// <summary>
        /// Indicates whether this axis is a category axis.
        /// </summary>
        internal bool IsCategory
        {
            get { return (Type == ChartAxisType.Category) ; }
        }

        /// <summary>
        /// Returns axis that crosses the current axis.
        /// </summary>
        internal ChartAxis CrossAxis
        {
            get
            {
                if (mCrossAxis == null)
                    mCrossAxis = mPlotArea.GetAxis(CrossAx);
                return mCrossAxis;
            }
            set { mCrossAxis = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if labels for this axis should be rendered before axis line,
        /// i.e. if it is vertical axis - labels on the left side of the axis line,
        /// if it is horizontal axis, labels are below the axis line.
        /// </summary>
        internal bool AreLabelsBefore
        {
            get
            {
                bool isAxisAtLowerLeftCorner = (ActualAxisPosition == AxisPosition.Left) ||
                    (ActualAxisPosition == AxisPosition.Bottom);

                bool areLabelsVeryAfter = CrossAxis.ReverseOrder
                    ? (TickLabels.Position == AxisTickLabelPosition.Low)
                    : (TickLabels.Position == AxisTickLabelPosition.High);

                bool areLabelsVeryBefore = CrossAxis.ReverseOrder
                    ? (TickLabels.Position == AxisTickLabelPosition.High)
                    : (TickLabels.Position == AxisTickLabelPosition.Low);

                return ((isAxisAtLowerLeftCorner && !areLabelsVeryAfter) || areLabelsVeryBefore);
            }
        }

        /// <summary>
        /// Returns actual axis position taking in account where axis crosses the perpendicular axis.
        /// </summary>
        internal AxisPosition ActualAxisPosition
        {
            get
            {
                // WORDSNET-28193 If ChartEx axis is not "Category" the position should be "left".
                if (mChart != null && mChart.IsChartEx && !IsCategory)
                    return AxisPosition.Left;

                // If axis crosses perpendicular axis at maximum value and the perpendicular axis' orientation is MinMax,
                // axis position is right or top whatever value of AxPos is set.
                // The same if axis crosses perpendicular axis at minimum value and the perpendicular axis' orientation is MaxMin.
                if ((Crosses == AxisCrosses.Maximum) && (CrossAxis.Scaling.Orientation == AxisOrientation.MinMax) ||
                    (Crosses == AxisCrosses.Minimum) && (CrossAxis.Scaling.Orientation == AxisOrientation.MaxMin))
                {
                    if (AxPos == AxisPosition.Left)
                        return AxisPosition.Right;
                    if (AxPos == AxisPosition.Bottom)
                        return AxisPosition.Top;
                }

                // And vise versa if axis crosses perpendicular axis at min value and the perpendicular axis' orientation is MinMax,
                // axis position is left or bottom whatever value of AxPos is set.
                // The same if axis crosses perpendicular axis at max value and the perpendicular axis' orientation is MaxMin.
                if ((Crosses == AxisCrosses.Minimum) && (CrossAxis.Scaling.Orientation == AxisOrientation.MinMax) ||
                    (Crosses == AxisCrosses.Maximum) && (CrossAxis.Scaling.Orientation == AxisOrientation.MaxMin))
                {
                    if (AxPos == AxisPosition.Right)
                        return AxisPosition.Left;
                    if (AxPos == AxisPosition.Top)
                        return AxisPosition.Bottom;
                }

                // WORDSNET-12797 In case of AutoZero position and MinMax orientation of cross axis,
                // right position of axis must be switched to left.
                if ((Crosses == AxisCrosses.Automatic) && (AxPos == AxisPosition.Right) &&
                    (CrossAxis.Scaling.Orientation == AxisOrientation.MinMax))
                {
                    return AxisPosition.Left;
                }

                return AxPos;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if plot area can overlap axis labels.
        /// </summary>
        internal bool CanPlotAreaOverlapLabels
        {
            get
            {
                object tickLblPos = ChartAxisPr.GetDirectProperty(DmlChartAxisAttrs.TickLblPos);
                return (tickLblPos != null) && ((AxisTickLabelPosition)tickLblPos == AxisTickLabelPosition.NextToAxis);
            }
        }

        /// <summary>
        /// Gets or sets flag that indicates whether consider values of this axis as percent staked.
        /// </summary>
        internal bool IsPercentStacked
        {
            get { return mIsPercentStacked; }
            set { mIsPercentStacked = value; }
        }

        /// <summary>
        /// Gets or sets flag that indicates whether this Axis is one of 3D coordinate plane Axis.
        /// Flag is used upon calculating auto max, min and step values.
        /// MS Word uses special rules for 3D charts.
        /// </summary>
        internal bool Is3D
        {
            get { return mIs3D; }
            set { mIs3D = value; }
        }

        internal bool Is2DSurface
        {
            get { return mIs2DSurface; }
            set { mIs2DSurface = value; }
        }

        /// <summary>
        /// Returns the labels angle in degrees.
        /// </summary>
        internal double LabelsRotation
        {
            get { return TxPr.BodyPr.Rotation.ValueInDegrees; }
        }

        internal double Unit
        {
            get { return DisplayUnit.GetActualUnit(); }
        }

        /// <summary>
        /// Returns <c>true</c> if axis is logarithmic scaled.
        /// </summary>
        internal bool IsLogScaled
        {
            get { return Scaling.Type == AxisScaleType.Logarithmic; }
        }

        internal bool IsStaked
        {
            get { return mIsStacked; }
            set { mIsStacked = value; }
        }

        /// <summary>
        /// Returns the underlying chart object for internal use.
        /// </summary>
        internal DmlChart Chart
        {
            get { return mChart; }
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

        /// <summary>
        /// Returns the series that use this axis.
        /// </summary>
        internal IList<ChartSeries> Series
        {
            get { return mSeries; }
        }

        /// <summary>
        /// Gets or sets the axis direction (X, Y, or Z). The property can also return the
        /// <see cref="AxisDirection.Unspecified"/> value.
        /// </summary>
        internal AxisDirection Direction { get; set; }

        private StringToObjDictionary<DmlExtension> mExtensions;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlChart mChart;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartAxis mCrossAxis;
        private ChartAxisType mAxisType;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlChartPlotArea mPlotArea;
        private List<ChartSeries> mSeries = new List<ChartSeries>();
        private RectangleF mPlotAreaTargetRect;
        private bool mIsPercentStacked;
        private bool mIsStacked;
        private bool mIs3D;
        private bool mIs2DSurface;

        /// <summary>
        /// Represents the format code that is used to render axis values.
        /// </summary>
        private string mFormatCode;

        private DmlChartTitle mTitleInternal;
        private ChartAxisTitle mTitle;
        private bool mHasTitle = true;
        private ChartNumberFormat mNumberFormat;
        private AxisTickLabels mTickLabels;
        private ChartFormat mFormat;

        /// <summary>
        /// Specifies a type of chart gridlines: minor or major.
        /// </summary>
        private enum GridlineType
        {
            Minor,
            Major
        }
    }
}
