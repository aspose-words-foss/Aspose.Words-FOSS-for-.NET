// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents chart element.
    /// See 5.7 section in OOXML spec.
    /// </summary>
    internal abstract class DmlChart : IChartSeriesSource
    {
        internal void Warn(WarningType warningType, string message)
        {
            if (mPlotArea != null)
                mPlotArea.Warn(warningType, message);
        }

        /// <summary>
        /// Use this method to add axis id to the chart, there might be 2 or 3 ids.
        /// It seems they are ordered like x, y, z. So the first is for X axis.
        /// </summary>
        /// <remarks>
        /// A Pareto chart has two Y axes: for Y value series and for Pareto lines.
        /// </remarks>
        /// <returns>Direction of the referenced axis.</returns>
        internal AxisDirection AddAxId(int axId)
        {
            AxisDirection direction = AxisDirection.Unspecified;

            switch (mAxIdIndex)
            {
                case 0:
                {
                    direction = (SeriesType == ChartSeriesType.Funnel) ? AxisDirection.Y :AxisDirection.X;
                    ChartPr.SetProperty(DmlChartAttrs.AxIdX, axId);

                    break;
                }
                case 1:
                {
                    ChartPr.SetProperty(DmlChartAttrs.AxIdY, axId);
                    direction = AxisDirection.Y;
                    break;
                }
                case 2:
                {
                    // Word 2016 chart has no Z axes. This is a secondary Y axis in that chart.
                    if (IsChartEx)
                    {
                        if (SeriesType == ChartSeriesType.Pareto)
                            direction = AxisDirection.Y;
                    }
                    else
                    {
                        ChartPr.SetProperty(DmlChartAttrs.AxIdZ, axId);
                        direction = AxisDirection.Z;
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentException("there cannot be more than 3 axis.");
                }
            }

            mAxIdIndex++;

            return direction;
        }

        internal void SetPlotArea(DmlChartPlotArea plotArea)
        {
            mPlotArea = plotArea;
        }

        /// <summary>
        /// Sets the <see cref="DmlChartAxisAttrs.IsDateCategoryAxis"/> attribute of the X axis.
        /// </summary>
        internal void SetXAxisDateCategoryAttribute(bool value)
        {
            if ((AxisX != null) && AxisX.IsCategory)
                AxisX.ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsDateCategoryAxis, value);
        }

        /// <summary>
        /// Updates the <see cref="SeriesType"/> property in accordance with the type of an existing chart series.
        /// </summary>
        internal void UpdateSeriesType()
        {
            if ((mSeriesType != UnknownSeriesType) || (mSeries == null) || (mSeries.Count == 0))
                return;

            mSeriesType = mSeries[0].SeriesType;

            // A pareto line is a special series in a pareto chart.
            if (mSeriesType == ChartSeriesType.ParetoLine)
                mSeriesType = ChartSeriesType.Pareto;
        }

        /// <summary>
        /// Sets stacked Y-values.
        /// </summary>
        /// <param name="series">The specified <see cref="ChartSeries"/></param>
        /// <param name="yValuesStackNegative">Array of stacked negative values</param>
        /// <param name="yValuesStackPositive">Array of stacked positive values</param>
        /// <param name="maxYValues">Array of Y values sum</param>
        private void SetYStackedValues(ChartSeries series, double[] yValuesStackNegative, double[] yValuesStackPositive,
            double[] maxYValues)
        {
            DmlChartDimensionData yValues = series.Y;

            for (int i = 0; i < MaxPointsCount; i++)
            {
                DmlChartValue value = yValues.Data.GetOriginalValue(i);
                bool isNumValue = (value != null) && (value.ValueType == DmlChartValueType.Numeric);

                // WORDSNET-9080 Check whether value is num value. Dummy value might occur here.
                string formatCode = isNumValue ? ((DmlChartNumValue)value).FormatCode : null;

                double currentNumValue = isNumValue
                    ? double.IsNaN(value.Value) ? 0.0d : value.Value
                    : 0.0d;

                // Use different stacks only for bar chart.
                double[] yValuesStack = (currentNumValue < 0) && IsBarChart
                    ? yValuesStackNegative
                    : yValuesStackPositive;

                yValuesStack[i] += currentNumValue;
                double currentValue = IsPercentStacked ? (yValuesStack[i] / maxYValues[i]) : yValuesStack[i];

                // Method add will reset or add value at the specified index.
                yValues.Data.AddValue(new DmlChartNumValue(i, currentValue, formatCode));
            }
        }

        /// <summary>
        /// Clones this DML chart.
        /// </summary>
        internal virtual DmlChart Clone()
        {
            DmlChart lhs = (DmlChart)MemberwiseClone();

            lhs.mPlotArea = null;
            lhs.mNonEmptySeries = null;

            if (mChartPr != null)
                lhs.ChartPr = mChartPr.Clone();

            lhs.mSeries = new DmlChartSeriesCollection(lhs);
            foreach (ChartSeries series in lhs.Series)
                series.SetChart(lhs);

            ChartDataLabelCollection labels =
                (ChartDataLabelCollection)lhs.ChartPr.GetDirectProperty(DmlChartAttrs.DLbls);
            if (labels != null)
                labels.SetChart(lhs, null);

            lhs.mAxisX = null;
            lhs.mAxisY = null;
            lhs.mAxisZ = null;

            return lhs;
        }

        /// <summary>
        /// Returns the series index, which is used when displaying the Z-axis, based on axis orientation.
        /// </summary>
        /// <param name="index">The sequential number</param>
        /// <returns>
        /// Sequential number if the standard orientation is used, otherwise - sequential number from the end of
        /// the collection
        /// </returns>
        internal int GetSeriesIndex(int index)
        {
            return IsZAxisWithReverseOrder ? (NonEmptySeries.Count - index - 1) : index;
        }

        /// <summary>
        /// Removes series data with the specified ID from a Word 2016 chart. Data of these charts is stored separately
        /// from series.
        /// </summary>
        internal void RemoveSeriesData(int dataId)
        {
            if (IsChartEx && (dataId >= 0))
                ChartSpace.Data.Remove(dataId);
        }

        /// <summary>
        /// Removes the specified series, including series data, from the parent plot area.
        /// </summary>
        private void RemoveSeries(ChartSeries series)
        {
            RemoveSeriesData(series.DataId);
            PlotArea.RemoveSeries(series);
        }

        /// <summary>
        /// Must be overridden and return type of the concrete chart.
        /// </summary>
        internal abstract DmlChartType ChartType { get; }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        /// <remarks>
        /// The returned value is always equal to <see cref="SeriesType"/> for pre Word 2016 charts. For Word 2016
        /// charts, series layout type stored in <see cref="ChartSeries"/> is needed to determine series type.
        /// </remarks>
        internal abstract ChartSeriesType DefaultSeriesType { get; }

        /// <summary>
        /// Legend is rendered for each data point if there is only one chart, chart contains only one series
        /// and at least one data point has custom formatting or at each data marker in the series shall have a different color.
        /// </summary>
        internal virtual bool RenderLegendForDataPoints
        {
            get { return ((Series.Count == 1) && (FirstSeries != null) &&
                          (FirstSeries.DataPoints.HasCustomDataPoints || VaryColors)
                          && (SeriesType != ChartSeriesType.Waterfall)); }
        }

        /// <summary>
        /// Returns plot area of the current chart.
        /// </summary>
        internal DmlChartPlotArea PlotArea
        {
            get { return mPlotArea; }
        }

        /// <summary>
        /// Gets <see cref="DocumentBase"/> that this node belongs to.
        /// </summary>
        internal DocumentBase Document
        {
            get
            {
                if (mDocument == null)
                    mDocument = ((mPlotArea == null) || (ChartSpace == null)) ? null : ChartSpace.Dml.Document;

                return mDocument;
            }

            set { mDocument = value; }
        }

        /// <summary>
        /// Returns chart space that specifies overall settings of the current chart.
        /// </summary>
        internal DmlChartSpace ChartSpace
        {
            get { return mPlotArea.ParentChartFormat.DmlChartSpace; }
        }

        /// <summary>
        /// Returns the first series in the chart.
        /// </summary>
        internal ChartSeries FirstSeries
        {
            get
            {
                if (mFirstSeries == null)
                {
                    mFirstSeries =  (NonEmptySeries.Count == 0) ? null : NonEmptySeries[0];

                    // WORDSNET-23643 Use the first series containing x and y values.
                    for (int i = 0; i < NonEmptySeries.Count; i++)
                    {
                        if (!NonEmptySeries[i].X.IsEmpty)
                        {
                            mFirstSeries = NonEmptySeries[i];
                            break;
                        }
                    }
                }

                return mFirstSeries;
            }
        }

        /// <summary>
        /// Returns a collection of <see cref="ChartSeries"/> of this chart.
        /// </summary>
        internal DmlChartSeriesCollection Series
        {
            get
            {
                if (mSeries == null)
                    mSeries = new DmlChartSeriesCollection(this);

                return mSeries;
            }
        }

        /// <summary>
        /// Returns the type of series of this DML chart.
        /// </summary>
        internal ChartSeriesType SeriesType
        {
            get
            {
                if (mSeriesType == UnknownSeriesType)
                    return DefaultSeriesType;

                return mSeriesType;
            }
        }

        /// <summary>
        /// Returns only series that has points.
        /// Use this collection for proper rendering of bar charts, since number of series affects width of bars.
        /// </summary>
        internal IList<ChartSeries> NonEmptySeries
        {
            get
            {
                if (mNonEmptySeries == null)
                {
                    mNonEmptySeries = new List<ChartSeries>();
                    foreach (ChartSeries series in Series)
                    {
                        if (series.HasPoints)
                            mNonEmptySeries.Add(series);
                    }
                }

                return mNonEmptySeries;
            }
        }

        internal void ResetNonEmtpySeries()
        {
            mNonEmptySeries = null;
            mFirstSeries = null;
        }

        /// <summary>
        /// Flag specifies that each data marker in the series shall have a different color.
        /// Has no effect for surface and stock charts.
        /// </summary>
        internal bool VaryColors
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.VaryColors); }
            set { ChartPr.SetProperty(DmlChartAttrs.VaryColors, value); }
        }

        /// <summary>
        /// Specifies the settings for the data labels for an entire series or the entire chart.
        /// Has no effect for surface charts.
        /// </summary>
        internal ChartDataLabelCollection DataLabels
        {
            get
            {
               object dataLabels = ChartPr.GetProperty(DmlChartAttrs.DLbls);
               return  (dataLabels != null) ? (ChartDataLabelCollection)(dataLabels) : new ChartDataLabelCollection(this, null);
            }
        }

        /// <summary>
        /// Returns properties of this chart.
        /// </summary>
        internal DmlChartPr ChartPr
        {
            get
            {
                if (mChartPr == null)
                    mChartPr = new DmlChartPr(Document);

                return mChartPr;
            }

            set { mChartPr = value; }
        }

        /// <summary>
        /// Returns true if the chart has 3D chart.
        /// </summary>
        internal bool Is3D
        {
            get
            {
                switch (ChartType)
                {
                    case DmlChartType.Pie3DChart:
                    case DmlChartType.Area3DChart:
                    case DmlChartType.Line3DChart:
                    case DmlChartType.Bar3DChart:
                    case DmlChartType.Surface3DChart:
                    case DmlChartType.SurfaceChart:
                        return true;
                    case DmlChartType.AreaChart:
                    case DmlChartType.LineChart:
                    case DmlChartType.StockChart:
                    case DmlChartType.RadarChart:
                    case DmlChartType.ScatterChart:
                    case DmlChartType.BarChart:
                    case DmlChartType.BubbleChart:
                    case DmlChartType.PieChart:
                    case DmlChartType.DoughnutChart:
                    case DmlChartType.OfPieChart:
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Indicates whether the chart has Z-axis and axis values should be rendered in reverse order (from max to min).
        /// </summary>
        private bool IsZAxisWithReverseOrder
        {
            get  { return (Is3D && (AxisZ != null) && AxisZ.ReverseOrder); }
        }

        /// <summary>
        /// Returns true if the chart has axis.
        /// Returns false, if chart type is Pie or Doughnut, because these charts cannot have axis.
        /// </summary>
        internal bool HasAxis
        {
            get
            {
                switch (ChartType)
                {
                    case DmlChartType.AreaChart:
                    case DmlChartType.Area3DChart:
                    case DmlChartType.LineChart:
                    case DmlChartType.Line3DChart:
                    case DmlChartType.StockChart:
                    case DmlChartType.RadarChart:
                    case DmlChartType.ScatterChart:
                    case DmlChartType.BarChart:
                    case DmlChartType.Bar3DChart:
                    case DmlChartType.SurfaceChart:
                    case DmlChartType.Surface3DChart:
                    case DmlChartType.BubbleChart:
                        return true;
                    case DmlChartType.ChartExChart:
                    {
                        if (SeriesType == ChartSeriesType.Sunburst ||
                            SeriesType == ChartSeriesType.Treemap ||
                            SeriesType == ChartSeriesType.RegionMap)
                            return false;

                        return true;
                    }
                    case DmlChartType.PieChart:
                    case DmlChartType.Pie3DChart:
                    case DmlChartType.DoughnutChart:
                    case DmlChartType.OfPieChart:
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Provides access to properties of the X axis of the chart.
        /// </summary>
        internal ChartAxis AxisX
        {
            get
            {
                if ((mAxisX == null) && HasAxis)
                {
                    IDmlChart2D chart2D = this as IDmlChart2D;
                    if (chart2D != null)
                        mAxisX = chart2D.AxX;
                }

                return mAxisX;
            }
            set
            {
                // Cannot remove axis from a DML chart.
                Debug.Assert(value != null);

                ChartPr.SetProperty(DmlChartAttrs.AxIdX, value.AxId);
                mAxisX = value;
            }
        }

        /// <summary>
        /// Provides access to properties of the Y axis of the chart.
        /// </summary>
        internal ChartAxis AxisY
        {
            get
            {
                if ((mAxisY == null) && HasAxis)
                {
                    IDmlChart2D chart2D = this as IDmlChart2D;
                    if (chart2D != null)
                        mAxisY = chart2D.AxY;
                }

                return mAxisY;
            }
            set
            {
                // Cannot remove axis from a DML chart.
                Debug.Assert(value != null);

                ChartPr.SetProperty(DmlChartAttrs.AxIdY, value.AxId);
                mAxisY = value;
            }
        }

        /// <summary>
        /// Provides access to properties of the Z axis of the chart.
        /// </summary>
        internal ChartAxis AxisZ
        {
            get
            {
                if ((mAxisZ == null) && HasAxis)
                {
                    IDmlChart3D chart3D = this as IDmlChart3D;
                    if (chart3D != null)
                        mAxisZ = chart3D.AxZ;
                }

                return mAxisZ;
            }
        }

        /// <summary>
        /// Returns maximum number of data points in all chart's series.
        /// </summary>
        internal int MaxPointsCount
        {
            get
            {
                if (mMaxPointsCount<0)
                {
                    foreach (ChartSeries series in Series)
                        mMaxPointsCount = System.Math.Max(mMaxPointsCount, series.ValueCount);
                }
                return mMaxPointsCount;
            }
        }

        /// <summary>
        /// Indicates whether the chart is "RadarChart".
        /// </summary>
        internal bool IsRadarChart
        {
            get { return (ChartType == DmlChartType.RadarChart); }
        }

        /// <summary>
        /// Indicates whether the chart is "PieChart".
        /// </summary>
        internal bool IsPieChart
        {
            get
            {
                switch (ChartType)
                {
                    case DmlChartType.PieChart:
                    case DmlChartType.Pie3DChart:
                    case DmlChartType.OfPieChart:
                    case DmlChartType.DoughnutChart:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Indicates whether the chart is <see cref=" DmlChartType.ScatterChart"/>
        /// or <see cref=" DmlChartType.BubbleChart"/>.
        /// </summary>
        internal bool IsScatter
        {
            get
            {
                switch (ChartType)
                {
                    case DmlChartType.ScatterChart:
                    case DmlChartType.BubbleChart:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Indicates whether the chart is "SurfaceChart".
        /// </summary>
        internal bool IsSurfaceChart
        {
            get { return ChartType == DmlChartType.SurfaceChart;}
        }

        /// <summary>
        /// Indicates whether the chart is "Surface3DChart".
        /// </summary>
        internal bool IsSurface3DChart
        {
            get { return ChartType == DmlChartType.Surface3DChart; }
        }

        /// <summary>
        /// Indicates whether the chart is "BubbleChart".
        /// </summary>
        internal bool IsBubbleChart
        {
            get { return ChartType == DmlChartType.BubbleChart; }
        }

        /// <summary>
        /// Indicates whether the chart is "LineChart".
        /// </summary>
        internal bool IsLineChart
        {
            get { return ChartType == DmlChartType.LineChart; }
        }

        /// <summary>
        /// Indicates whether chart can be rendered.
        /// </summary>
        /// <remark>
        /// The histogram is currently supported.
        /// </remark>
        internal bool IsSupportedChartEx
        {
            get
            {
                return (SeriesType == ChartSeriesType.Histogram) ||
                    (SeriesType == ChartSeriesType.Waterfall) ||
                    (SeriesType == ChartSeriesType.Funnel);
            }
        }

        /// <summary>
        /// Indicates whether the chart type is "BarChart" or "Bar3DChart".
        /// </summary>
        internal bool IsBarChart
        {
            get
            {
                switch (ChartType)
                {
                    case DmlChartType.BarChart:
                    case DmlChartType.Bar3DChart:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the chart is a Word 2016 chart.
        /// </summary>
        internal bool IsChartEx
        {
            get { return (ChartType == DmlChartType.ChartExChart); }
        }

        /// <summary>
        /// Indicated whether the chart is percent stacked.
        /// </summary>
        internal bool IsPercentStacked
        {
            get { return (ChartPr.Grouping == Grouping.PercentStacked); }
        }

        /// <summary>
        /// Indicated whether the chart is stacked.
        /// </summary>
        internal bool IsStacked
        {
            get
            {
                Grouping grouping = ChartPr.Grouping;
                return (grouping == Grouping.PercentStacked) || (grouping == Grouping.Stacked);
            }
        }

        /// <summary>
        /// Indicates whether the chart type is supported.
        /// </summary>
        internal bool IsSupported
        {
            get
            {
                switch (ChartType)
                {
                    case DmlChartType.LineChart:
                    case DmlChartType.Line3DChart:
                    case DmlChartType.BubbleChart:
                    case DmlChartType.PieChart:
                    case DmlChartType.DoughnutChart:
                    case DmlChartType.Pie3DChart:
                    case DmlChartType.OfPieChart:
                    case DmlChartType.AreaChart:
                    case DmlChartType.Area3DChart:
                    case DmlChartType.BarChart:
                    case DmlChartType.Bar3DChart:
                    case DmlChartType.ScatterChart:
                    case DmlChartType.StockChart:
                    case DmlChartType.RadarChart:
                    case DmlChartType.SurfaceChart:
                    case DmlChartType.Surface3DChart:
                        return true;
                    case DmlChartType.ChartExChart:
                        if (IsSupportedChartEx)
                            return true;
                        else
                            return false;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Indicates that the chart contains no series or is not supported.
        /// </summary>
        internal bool IsEmpty
        {
            get { return !IsSupported || ((NonEmptySeries.Count == 0) &&
                    (SeriesType != ChartSeriesType.Waterfall) &&
                    (SeriesType != ChartSeriesType.Funnel)); }
        }

        #region IChartSeriesSource

        /// <summary>
        /// Removes the series at the specified index from the chart.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IChartSeriesSource.RemoveAt(int index)
        {
            ChartSeries series = Series[index];
            Series.RemoveAt(index);
            RemoveSeries(series);
        }

        /// <summary>
        /// Removes all the series from the chart.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IChartSeriesSource.Clear()
        {
            foreach (ChartSeries series in Series)
                RemoveSeries(series);

            Series.Clear();

            if ((AxisX != null) && AxisX.IsCategory && !PlotArea.HasSeriesOfXAxis(AxisX))
            {
                // Need to clear the X axis date flag otherwise the axis type may not match inserted data.
                SetXAxisDateCategoryAttribute(false);
            }
        }

        /// <summary>
        /// Returns a list of chart series of the DML chart.
        /// Do not add/remove chart series to/from this list directly.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        IList<ChartSeries> IChartSeriesSource.SeriesList
        {
            get { return Series.SeriesList; }
        }

        /// <summary>
        /// Gets the DML chart to which adding series are placed.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        DmlChart IChartSeriesSource.DestinationChart
        {
            get { return this; }
        }

        #endregion IChartSeriesSource

        /// <summary>
        /// Negative value means the value must be recalculated.
        /// </summary>
        private List<ChartSeries> mNonEmptySeries;
        private DmlChartSeriesCollection mSeries;
        private ChartSeries mFirstSeries;
        private DmlChartPr mChartPr;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocumentBase mDocument;
        private int mAxIdIndex;
        private DmlChartPlotArea mPlotArea;

        /// <summary>
        /// Negative values means that values should be recalculated.
        /// </summary>
        private int mMaxPointsCount = -1;
        private ChartAxis mAxisX;
        private ChartAxis mAxisY;
        private ChartAxis mAxisZ;
        private ChartSeriesType mSeriesType = UnknownSeriesType;

        private const ChartSeriesType UnknownSeriesType = (ChartSeriesType)(-1);
    }
}
