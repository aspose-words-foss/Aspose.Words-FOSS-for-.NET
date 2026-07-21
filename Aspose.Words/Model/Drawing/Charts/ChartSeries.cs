// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents chart series properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartSeries : IChartDataPoint, IDmlExtensionListSource
    {
        /// <summary>
        /// 5.7.2.168 ser (Bubble Chart Series)
        /// 5.7.2.169 ser (Line Chart Series)
        /// 5.7.2.170 ser (Pie Chart Series)
        /// 5.7.2.171 ser (Surface Chart Series)
        /// 5.7.2.172 ser (Scatter Chart Series)
        /// 5.7.2.173 ser (Radar Chart Series)
        /// 5.7.2.174 ser (Area Chart Series)
        /// 5.7.2.175 ser (Bar Chart Series)
        /// 2.24.3.70 CT_Series [MS-ODRAWXML]
        /// </summary>
        internal ChartSeries(DmlChart chart)
        {
            mDPtCollection = new ChartDataPointCollection(this);
            mChart = chart;
        }

        /// <summary>
        /// Adds the specified X value to the chart series. If the series supports Y values and bubble sizes, they will
        /// be empty for the X value.
        /// </summary>
        public void Add(ChartXValue xValue)
        {
            EnsureAddingValuesSupported();
            DataProvider.AddValue(xValue, null, DefaultBubbleSize);
        }

        /// <summary>
        /// Adds the specified X and Y values to the chart series.
        /// </summary>
        public void Add(ChartXValue xValue, ChartYValue yValue)
        {
            EnsureAddingValuesSupported();

            if (!DataProvider.IsYValueSupported)
                mChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetYValue);

            DataProvider.AddValue(xValue, yValue, DefaultBubbleSize);
        }

        /// <summary>
        /// Adds the specified X value, Y value and bubble size to the chart series.
        /// </summary>
        public void Add(ChartXValue xValue, ChartYValue yValue, double bubbleSize)
        {
            EnsureAddingValuesSupported();

            if (!DataProvider.IsYValueSupported)
                mChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetYValue);

            if (!DataProvider.IsBubbleSizeSupported)
                mChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetBubbleSize);

            DataProvider.AddValue(xValue, yValue, bubbleSize);
        }

        /// <summary>
        /// Inserts the specified X value into the chart series at the specified index. If the series supports Y values
        /// and bubble sizes, they will be empty for the X value.
        /// </summary>
        /// <remarks>
        /// The corresponding data point with default formatting will be inserted into the data point collection. And,
        /// if data labels are displayed, the corresponding data label with default formatting will be inserted too.
        /// </remarks>
        public void Insert(int index, ChartXValue xValue)
        {
            EnsureAddingValuesSupported();
            InsertCore(index, xValue, null, DefaultBubbleSize);
        }

        /// <summary>
        /// Inserts the specified X and Y values into the chart series at the specified index.
        /// </summary>
        /// <remarks>
        /// The corresponding data point with default formatting will be inserted into the data point collection. And,
        /// if data labels are displayed, the corresponding data label with default formatting will be inserted too.
        /// </remarks>
        public void Insert(int index, ChartXValue xValue, ChartYValue yValue)
        {
            EnsureAddingValuesSupported();

            if (!DataProvider.IsYValueSupported)
                mChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetYValue);

            InsertCore(index, xValue, yValue, DefaultBubbleSize);
        }

        /// <summary>
        /// Inserts the specified X value, Y value and bubble size into the chart series at the specified index.
        /// </summary>
        /// <remarks>
        /// The corresponding data point with default formatting will be inserted into the data point collection. And,
        /// if data labels are displayed, the corresponding data label with default formatting will be inserted too.
        /// </remarks>
        public void Insert(int index, ChartXValue xValue, ChartYValue yValue, double bubbleSize)
        {
            EnsureAddingValuesSupported();

            if (!DataProvider.IsYValueSupported)
                mChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetYValue);

            if (!DataProvider.IsBubbleSizeSupported)
                mChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetBubbleSize);

            InsertCore(index, xValue, yValue, bubbleSize);
        }

        /// <summary>
        /// Removes the X value, Y value, and bubble size, if supported, from the chart series at the specified index.
        /// The corresponding data point and data label are also removed.
        /// </summary>
        public void Remove(int index)
        {
            if ((index < 0) || (index >= DataProvider.ValueCount))
                throw new ArgumentOutOfRangeException("index");

            DataProvider.RemoveValue(index);
        }

        /// <summary>
        /// Removes all data values from the chart series. Format of all individual data points and data labels is cleared.
        /// </summary>
        public void Clear()
        {
            ClearValues();

            DataPoints.ClearFormat();
            DataLabels.ClearFormat();
        }

        /// <summary>
        /// Removes all data values from the chart series with preserving the format of the data points and data labels.
        /// </summary>
        public void ClearValues()
        {
            DataProvider.RemoveAllValues();
        }

        /// <summary>
        /// Copies default data point format from the data point with the specified index.
        /// </summary>
        public void CopyFormatFrom(int dataPointIndex)
        {
            // The upper bound of the index is currently not checked because we cannot calculate DataPoints.Count
            // exactly for some chart types of MS Word 2016 (the same behavior as in the 'DataPoints.this' property).
            ArgumentUtil.CheckNonNegative(dataPointIndex, "dataPointIndex");

            // To copy format from a data point of this series, we need to merge its changed properties to default
            // data point properties.
            DefaultDataPoint.MergeFormatFrom(DataPoints[dataPointIndex]);
        }

        /// <summary>
        /// Sets the parent chart of this series.
        /// </summary>
        internal void SetChart(DmlChart chart)
        {
            mChart = chart;

            if (mDefaultDPt != null)
                mDefaultDPt.SetChart(chart);

            foreach (ChartDataPoint dataPoint in mDPtCollection.MaterializedDataPoints)
                dataPoint.SetChart(chart);

            if (mDLbls != null)
                mDLbls.SetChart(chart, this);
        }

        internal ChartSeries Clone()
        {
            ChartSeries lhs = (ChartSeries)MemberwiseClone();

            if (mDefaultDPt != null)
                lhs.mDefaultDPt = mDefaultDPt.Clone();

            if (IsChartExChart)
            {
                // Data is cloned when cloning chart space.
                lhs.mX = null;
                lhs.mY = null;
                lhs.mSize = null;
            }
            else
            {
                if (mX != null)
                    lhs.mX = mX.Clone();

                if (mY != null)
                    lhs.mY = mY.Clone();

                if (mSize != null)
                    lhs.mSize = mSize.Clone();
            }

            if (mXErrBars != null)
            {
                lhs.mXErrBars = mXErrBars.Clone();
                lhs.mXErrBars.SetSeries(lhs);
            }

            if (mYErrBars != null)
            {
                lhs.mYErrBars = mYErrBars.Clone();
                lhs.mYErrBars.SetSeries(lhs);
            }

            if (mTx != null)
                lhs.mTx = mTx.Clone();

            if (mDLbls != null)
                lhs.mDLbls = mDLbls.Clone();
            if (lhs.HasDataLabels != HasDataLabels)
                lhs.SetHasDataLabels(HasDataLabels);

            lhs.mTrendlines = new List<DmlChartTrendline>();
            foreach (DmlChartTrendline trendline in mTrendlines)
            {
                DmlChartTrendline cloned = trendline.Clone();
                cloned.SetSeries(lhs);
                lhs.mTrendlines.Add(cloned);
            }

            lhs.mDPtCollection = mDPtCollection.Clone();
            lhs.mDPtCollection.SetSeries(lhs);

            if (mLayoutPr != null)
                lhs.mLayoutPr = mLayoutPr.Clone();

            if (mValueColors != null)
                lhs.mValueColors = mValueColors.Clone();

            if (mExtensions != null)
                lhs.mExtensions = DmlExtensionListSource.CloneExtensions(mExtensions);

            lhs.mInheritedXVal = null;
            lhs.mXValues = null;
            lhs.mYValues = null;
            lhs.mBubbleSizes = null;
            lhs.mDataProvider = null;

            return lhs;
        }

        /// <summary>
        /// Creates a data storage for this series.
        /// </summary>
        /// <remarks>
        /// This method has effect only for series of Word 2016 charts, in which chart data is stored separately as
        /// an item of the chart data collection <see cref="DmlChartSpace.Data"/>.
        /// </remarks>
        internal void CreateChartExDataStorage()
        {
            if (!IsChartExChart)
                return;

            DmlChartData data = Chart.ChartSpace.Data.Add();
            mDataId = data.Id;

            DmlChartDataSource xDataSource = new DmlChartDataSource();
            data.DataSources.Add(xDataSource);
            mX = new DmlChartDimensionData(xDataSource);

            // Histogram charts do not have Y values.
            if (Chart.SeriesType != ChartSeriesType.Histogram)
            {
                DmlChartDataSource yDataSource = new DmlChartDataSource();
                data.DataSources.Add(yDataSource);
                mY = new DmlChartDimensionData(yDataSource);
            }
        }

        /// <summary>
        /// Configures the layout type and layout properties according to the specified series type.
        /// </summary>
        /// <remarks>
        /// This method has effect for series of Word 2016 charts only.
        /// </remarks>
        internal void ConfigureChartExLayout(ChartSeriesType seriesType)
        {
            if (!IsChartExChart)
                return;

            switch (seriesType)
            {
                case ChartSeriesType.Treemap:
                {
                    LayoutType = SeriesLayout.Treemap;
                    LayoutPr.SetProperty(DmlChartSeriesLayoutAttr.ParentLabelLayout, ParentLabelLayout.Overlapping);
                    DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, ChartDataLabelPosition.InsideEnd);
                    break;
                }
                case ChartSeriesType.Sunburst:
                {
                    LayoutType = SeriesLayout.Sunburst;
                    DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, ChartDataLabelPosition.Center);
                    break;
                }
                case ChartSeriesType.Histogram:
                {
                    LayoutType = SeriesLayout.ClusteredColumn;
                    LayoutPr.Binning = new DmlChartBinningPr();
                    LayoutPr.Binning.SetProperty(DmlChartBinningAttr.IntervalClosed, IntervalClosedSide.Right);
                    break;
                }
                case ChartSeriesType.Pareto:
                {
                    LayoutType = SeriesLayout.ClusteredColumn;
                    LayoutPr.IsAggregation = true;
                    break;
                }
                case ChartSeriesType.BoxAndWhisker:
                {
                    LayoutType = SeriesLayout.BoxWhisker;
                    LayoutPr.IsMeanLineVisible = false;
                    LayoutPr.IsMeanMarkerVisible = true;
                    LayoutPr.IsNonOutliersVisible = false;
                    LayoutPr.IsOutliersVisible = true;
                    LayoutPr.QuartileMethod = QuartileMethod.ExclusiveMedian;
                    break;
                }
                case ChartSeriesType.Waterfall:
                {
                    LayoutType = SeriesLayout.Waterfall;
                    DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, ChartDataLabelPosition.OutsideEnd);
                    break;
                }
                case ChartSeriesType.Funnel:
                {
                    LayoutType = SeriesLayout.Funnel;
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the X values of the series with defining the required dimension type.
        /// </summary>
        internal void SetXValues(DmlChartValueCollection values)
        {
            X.Values = values;
            X.DimensionType = XDimensionType;
        }

        /// <summary>
        /// Sets the Y values of the series with defining the required dimension type.
        /// </summary>
        internal void SetYValues(DmlChartValueCollection values)
        {
            Y.Values = values;
            Y.DimensionType = YDimensionType;
        }

        /// <summary>
        /// Inserts the specified X value, Y value and bubble size into the chart series at the specified index.
        /// </summary>
        private void InsertCore(int index, ChartXValue xValue, ChartYValue yValue, double bubbleSize)
        {
            if ((index < 0) || (index > DataProvider.ValueCount))
                throw new ArgumentOutOfRangeException("index");

            DataProvider.InsertValue(index, xValue, yValue, bubbleSize);
        }

        /// <summary>
        /// Throws an exception if values cannot be added to the chart series.
        /// </summary>
        private void EnsureAddingValuesSupported()
        {
            if (!DataProvider.IsXValueSupported && !DataProvider.IsYValueSupported && !DataProvider.IsBubbleSizeSupported)
                throw new InvalidOperationException("This series type cannot have data.");
        }

        /// <summary>
        /// Sets a flag indicating whether data labels are displayed for the series without applying the chart style.
        /// </summary>
        internal void SetHasDataLabels(bool value)
        {
            mHasDataLabels = value;
        }

        /// <summary>
        /// Specifies a default data point formatting for this series.
        /// Use this property explicitly when you need to get formatting of whole series,
        /// if formatting of concrete data point is needed use <see cref="DataPoints"/> collection.
        ///
        /// Internal, for public access <see cref="IChartDataPoint"/> is used.
        /// </summary>
        internal ChartDataPoint DefaultDataPoint
        {
            get
            {
                if (mDefaultDPt == null)
                    mDefaultDPt = new ChartDataPoint(mChart);

                return mDefaultDPt;
            }
        }

        #region IChartDataPoint implementation
        /// <summary>
        /// Specifies the amount the data point shall be moved from the center of the pie.
        /// Can be negative, negative means that property is not set and no explosion should be applied.
        /// Applies only to Pie charts.
        /// </summary>
        public int Explosion
        {
            get { return DefaultDataPoint.Explosion; }
            set { DefaultDataPoint.Explosion = value; }
        }

        /// <summary>
        /// Specifies whether the parent element shall inverts its colors if the value is negative.
        /// </summary>
        public bool InvertIfNegative
        {
            get { return DefaultDataPoint.InvertIfNegative; }
            set { DefaultDataPoint.InvertIfNegative = value; }
        }

        /// <summary>
        /// Specifies a data marker. Marker is automatically created when requested.
        /// </summary>
        public ChartMarker Marker
        {
            get { return DefaultDataPoint.Marker; }
        }

        /// <summary>
        /// Specifies whether the bubbles in Bubble chart should have a 3-D effect applied to them.
        /// </summary>
        public bool Bubble3D
        {
            get { return DefaultDataPoint.Bubble3D; }
            set { DefaultDataPoint.Bubble3D = value; }
        }

        #endregion

        /// <summary>
        /// Returns a collection of formatting objects for all data points in this series.
        /// </summary>
        public ChartDataPointCollection DataPoints
        {
            get { return mDPtCollection; }
        }

        /// <summary>
        /// Gets or sets the name of the series, if name is not set explicitly it is generated using index.
        /// By default returns Series plus one based index.
        /// </summary>
        public string Name
        {
            // Part of WORDSNET-15096 Note, if series name is not set explicitly, there should not be whitespace.
            // Name should be 'SeriesN'.
            get { return (Tx == null) ? string.Format("Series{0}", (Index + 1)) : Tx.GetText(); }
            set
            {
                if (!StringUtil.HasChars(value))
                {
                    // Reset series name to default. Set Tx to null, in this case getter returns default series name 'SeriesN'.
                    Tx = null;
                    mChart.Warn(WarningType.DataLoss, "Series name cannot be empty, name as Series plus one based index will be generated.");
                    return;
                }

                if (Tx == null)
                    Tx = new DmlChartTx();

                Tx.PlainText = value;
            }
        }

        /// <summary>
        /// Allows to specify whether the line connecting the points on the chart shall be smoothed using Catmull-Rom splines.
        /// </summary>
        public bool Smooth
        {
            get { return mSmooth; }
            set
            {
                mSmooth = value;
                mSmoothExplicitlySet = true;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether data labels are displayed for the series.
        /// </summary>
        public bool HasDataLabels
        {
            get { return mHasDataLabels; }
            set
            {
                if (mHasDataLabels == value)
                    return;

                mHasDataLabels = value;
                if (mHasDataLabels)
                    Chart.ChartSpace.ApplyChartStyle(DataLabels);
            }
        }

        /// <summary>
        /// Specifies the settings for the data labels for the entire series.
        /// </summary>
        public ChartDataLabelCollection DataLabels
        {
            get
            {
                if (mDLbls == null)
                    mDLbls = new ChartDataLabelCollection(mChart, this);

                return mDLbls;
            }
            internal set { mDLbls = value; }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the series.
        /// </summary>
        public ChartFormat Format
        {
            get { return DefaultDataPoint.Format; }
        }

        /// <summary>
        /// Gets a legend entry for this chart series.
        /// </summary>
        public ChartLegendEntry LegendEntry
        {
            get
            {
                DmlChartFormat dmlChartFormat = mChart.PlotArea.ParentChartFormat;
                if (dmlChartFormat.Legend == null)
                    dmlChartFormat.Legend = new ChartLegend(dmlChartFormat);

                return dmlChartFormat.Legend.LegendEntries.GetSeriesLegendEntry(this);
            }
        }

        /// <summary>
        /// Gets the type of this chart series.
        /// </summary>
        public ChartSeriesType SeriesType
        {
            get
            {
                switch (mChart.ChartType)
                {
                    case DmlChartType.AreaChart:
                    case DmlChartType.Area3DChart:
                    case DmlChartType.BarChart:
                    case DmlChartType.Bar3DChart:
                    case DmlChartType.BubbleChart:
                    case DmlChartType.DoughnutChart:
                    case DmlChartType.LineChart:
                    case DmlChartType.Line3DChart:
                    case DmlChartType.OfPieChart:
                    case DmlChartType.PieChart:
                    case DmlChartType.Pie3DChart:
                    case DmlChartType.RadarChart:
                    case DmlChartType.ScatterChart:
                    case DmlChartType.StockChart:
                    case DmlChartType.SurfaceChart:
                    case DmlChartType.Surface3DChart:
                        return mChart.DefaultSeriesType;
                    case DmlChartType.ChartExChart:
                    {
                        switch (mLayoutType)
                        {
                            case SeriesLayout.BoxWhisker:
                                return ChartSeriesType.BoxAndWhisker;
                            case SeriesLayout.ClusteredColumn:
                                return LayoutPr.IsAggregation ? ChartSeriesType.Pareto : ChartSeriesType.Histogram;
                            case SeriesLayout.Funnel:
                                return ChartSeriesType.Funnel;
                            case SeriesLayout.ParetoLine:
                                return ChartSeriesType.ParetoLine;
                            case SeriesLayout.RegionMap:
                                return ChartSeriesType.RegionMap;
                            case SeriesLayout.Sunburst:
                                return ChartSeriesType.Sunburst;
                            case SeriesLayout.Treemap:
                                return ChartSeriesType.Treemap;
                            case SeriesLayout.Waterfall:
                                return ChartSeriesType.Waterfall;
                            default:
                                throw new InvalidOperationException("Unknown chart of Word 2016 or later.");
                        }
                    }
                    default:
                        throw new InvalidOperationException("Unknown chart type.");
                }
            }
        }

        /// <summary>
        /// Gets a collection of X values for this chart series.
        /// </summary>
        public ChartXValueCollection XValues
        {
            get
            {
                if (mXValues == null)
                {
                    if (DataProvider.IsXValueSupported && (X.Values == null))
                        X.Values = new DmlChartValueCollection(DmlChartValueType.Numeric);

                    mXValues = new ChartXValueCollection(DataProvider);
                }

                return mXValues;
            }
        }

        /// <summary>
        /// Gets a collection of Y values for this chart series.
        /// </summary>
        public ChartYValueCollection YValues
        {
            get
            {
                if (mYValues == null)
                {
                    if (DataProvider.IsYValueSupported && (Y.Values == null))
                        Y.Values = new DmlChartValueCollection(DmlChartValueType.Numeric);

                    mYValues = new ChartYValueCollection(DataProvider);
                }

                return mYValues;
            }
        }

        /// <summary>
        /// Gets a collection of bubble sizes for this chart series.
        /// </summary>
        public BubbleSizeCollection BubbleSizes
        {
            get
            {
                if (mBubbleSizes == null)
                {
                    if (DataProvider.IsBubbleSizeSupported && (Size.Values == null))
                        Size.Values = new DmlChartValueCollection(DmlChartValueType.Numeric);

                    mBubbleSizes = new BubbleSizeCollection(DataProvider);
                }

                return mBubbleSizes;
            }
        }

        /// <summary>
        /// Depending on the error bars directions, sets either X or Y error bars.
        /// </summary>
        internal void AddErrorBars(DmlChartErrorBars errorBars)
        {
            errorBars.SetSeries(this);

            switch (errorBars.ErrDir)
            {
                case ErrorBarDirection.X:
                    mXErrBars = errorBars;
                    break;
                case ErrorBarDirection.Y:
                default:
                    mYErrBars = errorBars;
                    break;
            }
        }

        /// <summary>
        /// Adds trend line to the series.
        /// </summary>
        internal void AddTrendline(DmlChartTrendline trendline)
        {
            trendline.SetSeries(this);
            mTrendlines.Add(trendline);
        }

        /// <summary>
        /// Returns an ordered array of X values.
        /// </summary>
        private DmlChartValue[] GetOrderedXValuesArrayInternal()
        {
            if (mOrderedXValuesArray != null)
                return mOrderedXValuesArray;

            List<DmlChartValue> xvaluesArray = new List<DmlChartValue>();
            // If X values are not present, generate array of dummy values.
            if (XValuesInherited.Data == null)
            {
                for (int i = 0; i <= LastNonEmptyValueIndex; i++)
                    xvaluesArray.Add(new DmlChartDummyValue(i, i + 1));
            }
            else
            {
                for (int i = 0; i < ValueCount; i++)
                {
                    DmlChartValue xVal = XValuesInherited.GetValue(i);

                    if (DmlChartValue.IsNullOrNaN(xVal))
                        continue;

                    xvaluesArray.Add(xVal);
                }
            }

            xvaluesArray.Sort(new DmlChartValueComparer());
            mOrderedXValuesArray = xvaluesArray.ToArray();

            return mOrderedXValuesArray;
        }

        /// <summary>
        /// Gets the index of the <see cref="DmlChartValue"/>. If value is null returns -1
        /// </summary>
        /// <remarks>
        /// Used to link a data label or marker with a series value. In normal cases, the marker or label
        /// is associated with the series element with the same index. But if the values are dates, MS Word reorders the values
        /// in ascending order.
        /// </remarks>
        /// <param name="xValue">Specified <see cref="DmlChartValue"/></param>
        /// <returns>The index of the specified <see cref="DmlChartValue"/> in the ordered array.</returns>
        internal int GetIndexInOrderedXValuesArray(DmlChartValue xValue)
        {
            if (xValue == null)
                return -1;

            // If XValues are dates, take the index in the reordered array.
            return (X.IsDate) ? Array.BinarySearch(GetOrderedXValuesArrayInternal(), xValue) : xValue.Index;
        }

        /// <summary>
        /// Gets the index in the ordered array of <see cref="DmlChartValue"/> with specified index.
        /// </summary>
        /// <param name="index">The specified index</param>
        /// <returns>The index in the ordered array of <see cref="DmlChartValue"/> with specified index</returns>
        internal int GetIndexInOrderedXValuesArray(int index)
        {
            DmlChartValue x = X.GetValue(index);

            // If XValues are dates, take the index in the reordered array.
            return GetIndexInOrderedXValuesArray(x);
        }

        /// <summary>
        /// Gets X-value by index, considering that if the data type is "Date" the values should be ordered.
        /// </summary>
        internal DmlChartValue GetXValueByIndex(int index)
        {
            return (X.IsDate && !IsScatterChartSeries)
                ? (index < OrderedXValuesArray.Length) ? OrderedXValuesArray[index] : null
                : XValuesInherited.GetValue(index);
        }

        /// <summary>
        /// Call this method when finish reading series.
        /// </summary>
        internal void EndReading()
        {
            // WORDSNET-8709 There are series with empty datasource, which caused incorrect axis values calculation.
            // To prevent incorrect values calculating, fill rendering data with dummy values if it is empty.
            if (!IsChartExChart && !X.IsEmpty)
                X.Data.AddDummyData(LastNonEmptyValueIndex + 1);
        }

        /// <summary>
        /// Specifies the index of the series in the chart.
        /// </summary>
        internal int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        /// <summary>
        /// Specifies the order of the series in the collection. It is 0 based.
        /// </summary>
        internal int Order
        {
            get { return mOrder; }
            set { mOrder = value; }
        }

        /// <summary>
        /// Returns true if <see cref="Smooth"/> flag is explicitly set in the chart.
        /// </summary>
        internal bool SmoothExplicitlySet
        {
            get { return mSmoothExplicitlySet; }
        }

        /// <summary>
        /// Specifies the shape of a series or a 3-D bar chart.
        /// </summary>
        internal BarShape Shape
        {
            get { return mShape; }
            set { mShape = value; }
        }

        /// <summary>
        /// Returns data for X axis.
        /// If data for x axis is not specified for this series returns data from the first chart's series.
        /// Use this property only for rendering.
        /// </summary>
        internal DmlChartDimensionData XValuesInherited
        {
            get
            {
                if (mInheritedXVal == null)
                    mInheritedXVal = SearchForProperXValuesSource();
                return mInheritedXVal;
            }
        }

        /// <summary>
        /// Returns ordered array of X values.
        /// </summary>
        internal DmlChartValue[] OrderedXValuesArray
        {
            get
            {
                return GetOrderedXValuesArrayInternal();
            }
        }

        /// <summary>
        /// Method returns X values source that should be used for rendering the chart.
        /// MS Word not always uses X values specified in series.
        /// </summary>
        /// <remarks>
        /// Code is experimental, since this behavior is not described in spec.
        /// </remarks>
        private DmlChartDimensionData SearchForProperXValuesSource()
        {
            // WORDSNET-18662 Series of ScatterChart can have non-equal numbers of X and Y values.
            // WORDSNET-24211 For Scatter chart use the values from source. Do not replace them.
            if (IsScatterChartSeries)
                return X;

            // If x values of current series is empty try to use x values of the first series.
            DmlChartDimensionData src = X.IsEmpty ? mChart.FirstSeries.X : X;

            // If number of points in X values does not match number of points in y values,
            // try to find other x values source with the same number of points.
            if (!src.IsEmpty && (src.Data.ValueCount == Y.Data.ValueCount))
                return src;

            DmlChartDimensionData firstNotEmptyChartDataSource = GetChartDimensionData();

            return (firstNotEmptyChartDataSource != null) ? firstNotEmptyChartDataSource : src;
        }

        /// <summary>
        /// Gets the first not empty <see cref="DmlChartDimensionData"/>
        /// </summary>
        /// <returns>The first not empty <see cref="DmlChartDimensionData"/> </returns>
        private DmlChartDimensionData GetChartDimensionData()
        {
            // There is nothing to search if current chart does not have axis.
            IDmlChart2D srcChart2D = mChart as IDmlChart2D;

            // There is nothing to search if current chart does not have axis.
            if (srcChart2D == null)
                return null;

            foreach (DmlChart chart in mChart.PlotArea.NonEmptyCharts)
            {
                IDmlChart2D chart2D = chart as IDmlChart2D;
                // Skip charts without axis.
                if (chart2D == null)
                    continue;

                // skip charts that are rendered on other axis that current chart.
                if (chart2D.AxX.AxId != srcChart2D.AxX.AxId)
                    continue;

                DmlChartDimensionData nextChartValues = chart.FirstSeries.X;

                // Use X values with the same number of not points as Y values has.
                // WORDSNET-18098 LastNonEmptyValueIndex should be checked too.
                if (!nextChartValues.IsEmpty &&
                    ((nextChartValues.Values.LastNonEmptyValueIndex == Y.Values.LastNonEmptyValueIndex) ||
                     (nextChartValues.Values.ValueCount == Y.Values.ValueCount)))
                {
                    return nextChartValues;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the number of series values.
        /// </summary>
        /// <remarks>
        /// WORDSNET-24750 Use (-1) as an indicator that the data has no value. In <see cref="ValueCount"/> method,
        /// this value is converted to zero.
        /// </remarks>
        private static int GetValueCount(DmlChartDimensionData values, bool useNonEmptyValueCount)
        {
            if (values.IsEmpty)
                return -1;

            int valueCount = useNonEmptyValueCount ? values.Data.NonEmptyValueCount : values.Data.ValueCount;

            return (valueCount > 0) ? valueCount : -1;
        }

        /// <summary>
        /// Get the number of series values.
        /// </summary>
        private static int GetValueCount(DmlChartDimensionData values)
        {
            return GetValueCount(values, false);
        }

        /// <summary>
        /// Refers to data of a Word 2016 chart series.
        /// </summary>
        internal DmlChartData ChartExChartData
        {
            get
            {
                if (!IsChartExChart || (mDataId < 0) || (mChart.ChartSpace == null))
                    return null;

                return mChart.ChartSpace.Data[mDataId];
            }
        }

        /// <summary>
        /// Specifies the data used for the X axis.
        /// 'cat' element in 'Line Chart', 'Pie Chart', 'Surface Chart', 'Radar Chart', 'Area Chart' and 'Bar Chart' Series.
        /// 'xVal' element in 'Bubble Chart Series' and 'Scatter Chart Series'.
        /// 'val' dimension of Histogram Word 2016 chart series.
        /// 'cat' dimension of other Word 2016 chart series.
        /// </summary>
        internal DmlChartDimensionData X
        {
            get
            {
                if (mX == null)
                {
                    DmlChartDataSource dataSource = (ChartExChartData != null)
                        ? ChartExChartData.GetDataSource(XDimensionType)
                        : new DmlChartDataSource();

                    mX = new DmlChartDimensionData(dataSource);
                }

                return mX;
            }
        }

        /// <summary>
        /// Specifies the data used for the Y axis.
        /// 'val' element in 'Line Chart', 'Pie Chart', 'Surface Chart', 'Radar Chart', 'Area Chart' and 'Bar Chart' Series.
        /// 'yVal' element in 'Bubble Chart Series' and 'Scatter Chart Series'.
        /// 'val' dimension of Funnel, Pareto, Waterfall and Box-and-Whisker Word 2016 chart series.
        /// 'size' dimension of Treemap and Sunburst Word 2016 chart series.
        /// Histogram chart has no Y values.
        /// Values are recalculated after reading all series of the chart.
        /// </summary>
        internal DmlChartDimensionData Y
        {
            get
            {
                if (mY == null)
                {
                    // A Histogram chart has only X values.
                    DmlChartDataSource dataSource = ((ChartExChartData != null) && !IsHistogramChart)
                        ? ChartExChartData.GetDataSource(YDimensionType)
                        : new DmlChartDataSource();

                    mY = new DmlChartDimensionData(dataSource);
                }

                return mY;
            }
        }

        /// <summary>
        /// Specifies the data for the sizes of the bubbles on the bubble chart.
        /// </summary>
        internal DmlChartDimensionData Size
        {
            get
            {
                if (mSize == null)
                    mSize = new DmlChartDimensionData(new DmlChartDataSource());

                return mSize;
            }
        }

        internal int ValueCount
        {
            get
            {
                // WORDSNET-21034 The radar chart uses only non-empty values for category axis data.
                int xCount = GetValueCount(X, mChart.IsRadarChart);
                int yCount = GetValueCount(Y);

                return System.Math.Max(GetValueBasedOnChartType(yCount, xCount), 0);
            }
        }

        /// <summary>
        /// Returns index of the last non-empty chart data value.
        /// </summary>
        internal int LastNonEmptyValueIndex
        {
            get
            {
                int yIndex = Y.IsEmpty ? -1 : Y.Data.LastNonEmptyValueIndex;
                int xIndex = X.IsEmpty ? -1 : X.Data.LastNonEmptyValueIndex;
                return GetValueBasedOnChartType(yIndex, xIndex);
            }
        }

        /// <summary>
        /// Returns X error bars, if null no error bars is applied to X.
        /// </summary>
        internal DmlChartErrorBars XErrorBars
        {
            get { return mXErrBars; }
        }

        /// <summary>
        /// Returns Y error bars, if null no error bars is applied to Y.
        /// </summary>
        internal DmlChartErrorBars YErrorBars
        {
            get { return mYErrBars; }
        }

        /// <summary>
        /// Returns collection of trendlines.
        /// Do not add trendlines to this collection directly, instead use <see cref="AddTrendline"/> method.
        /// </summary>
        internal IList<DmlChartTrendline> Trendlines
        {
            get { return mTrendlines; }
        }

        /// <summary>
        /// Specifies text for a series name, without rich text formatting.
        /// </summary>
        internal DmlChartTx Tx
        {
            get { return mTx; }
            set { mTx = value; }
        }

        /// <summary>
        /// Represents extLst: a CT_OfficeArtExtensionList ([ISO/IEC29500-1:2012] section A.4.1) element that specifies
        /// the extension list in which all future extensions of element type ext is defined.
        /// </summary>
        /// <remarks>
        /// Explicit implementation - do not need this in public.
        /// </remarks>
        StringToObjDictionary<DmlExtension> IDmlExtensionListSource.Extensions
        {
            get { return mExtensions; }
            set { mExtensions = value; }
        }

        /// <summary>
        /// Returns the parent chart of this series.
        /// </summary>
        internal DmlChart Chart
        {
            get { return mChart; }
        }

        /// <summary>
        /// Returns <c>true</c> if the series has y points including empty ones.
        /// Values for x can be auto generated, in case of empty y value series must be ignored.
        /// </summary>
        internal bool HasPoints
        {
            get { return (mY != null) && !mY.IsEmpty && (mY.Values.ValueCount > 0); }
        }

        /// <summary>
        /// Gets a flag indicating whether data labels range data is defined for the series.
        /// </summary>
        /// <remarks>
        /// The mentioned data can be displayed using the <see cref="ChartDataLabelCollection.ShowDataLabelsRange"/>
        /// property.
        /// </remarks>
        internal bool HasDataLabelsRangeData
        {
            get
            {
                if (mDataLabelsRangeData != null)
                    return mDataLabelsRangeData.Data != null;

                if (mExtensions == null)
                    return false;

                DmlExtension extension = mExtensions[DmlExtensionUri.Filtering];
                if (extension == null)
                    return false;

                return extension.DataLabelsRangeData.Values != null;
            }
        }

        /// <summary>
        /// Specifies the data for the data labels.
        /// Note: it is not required to write this data back to document, these values are written with extensions list.
        /// </summary>
        internal DmlChartDimensionData DataLabelsRangeData
        {
            get
            {
                if (mDataLabelsRangeData == null)
                {
                    if (mExtensions == null)
                        mExtensions = new StringToObjDictionary<DmlExtension>();

                    DmlExtension extension = DmlExtension.GetOrCreateExtension(mExtensions, DmlExtensionUri.Filtering);

                    mDataLabelsRangeData =  new DmlChartDimensionData(extension.DataLabelsRangeData);
                }

                return mDataLabelsRangeData;
            }
        }

        /// <summary>
        /// Gets or sets the colors used to represent data values as a continuous gradient of colors.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal DmlChartValueColors ValueColors
        {
            get { return mValueColors; }
            set { mValueColors = value; }
        }

        /// <summary>
        /// Gets or sets the layout type of this series.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal SeriesLayout LayoutType
        {
            get { return mLayoutType; }
            set { mLayoutType = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether this series is hidden from layout.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal bool Hidden
        {
            get { return mHidden; }
            set { mHidden = value; }
        }

        /// <summary>
        /// Gets or sets a series that owns this series.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal ChartSeries Owner
        {
            get { return mOwner; }
            set { mOwner = value; }
        }

        /// <summary>
        /// Gets or sets a unique identifier for the series.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal string UniqueId
        {
            get { return mUniqueId; }
            set { mUniqueId = value; }
        }

        /// <summary>
        /// Gets or sets the index of the format to use for default styling of this series.
        /// If value is negative, the property is not defined.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal int FormatIndex
        {
            get { return mFormatIndex; }
            set { mFormatIndex = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the data source for this series.
        /// If value is negative, the property is not defined.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal int DataId
        {
            get { return mDataId; }
            set { mDataId = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of an axis for this series.
        /// If value is negative, the property is not defined.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal int AxisId
        {
            get { return mAxisId; }
            set { mAxisId = value; }
        }

        /// <summary>
        /// Gets or sets the properties of the series layout.
        /// </summary>
        /// <dev>
        /// Supported by charts of the schema http://schemas.microsoft.com/office/drawing/2014/chartex.
        /// </dev>
        internal DmlChartSeriesLayoutPr LayoutPr
        {
            get
            {
                if (mLayoutPr == null)
                    mLayoutPr = new DmlChartSeriesLayoutPr();
                return mLayoutPr;
            }
        }

        /// <summary>
        /// Indicates whether the parent chart is <see cref=" DmlChartType.ScatterChart"/>
        /// or <see cref=" DmlChartType.BubbleChart"/>.
        /// </summary>
        internal bool IsScatterChartSeries
        {
            get
            {
                switch (Chart.ChartType)
                {
                    case DmlChartType.ScatterChart:
                        return true;
                    default:
                        return IsBubbleChartSeries;
                }
            }
        }

        /// <summary>
        /// Indicates whether the parent chart is <see cref=" DmlChartType.BubbleChart"/>.
        /// </summary>
        internal bool IsBubbleChartSeries
        {
            get { return Chart.IsBubbleChart; }
        }

        /// <summary>
        /// Indicates whether the type of the parent chart is "Pie".
        /// </summary>
        internal bool IsPieChartSeries
        {
            get { return Chart.IsPieChart; }
        }

        /// <summary>
        /// Gets a flag indicating whether the series is a Box and Whisker chart series.
        /// </summary>
        internal bool IsBoxAndWhiskerChartSeries
        {
            get { return IsChartExChart && (mLayoutType == SeriesLayout.BoxWhisker); }
        }

        internal DmlChartDataAnalyzer DataAnalyzer
        {
            get { return mDataAnalyzer; }
        }

        /// <summary>
        /// Gets a flag indicating whether the parent chart is a Word 2016 chart.
        /// </summary>
        private bool IsChartExChart
        {
            get { return mChart.IsChartEx; }
        }

        /// <summary>
        /// Gets a flag indicating whether the parent chart is a Word 2016 Histogram chart.
        /// </summary>
        private bool IsHistogramChart
        {
            get
            {
                return
                    IsChartExChart &&
                    (mLayoutType == SeriesLayout.ClusteredColumn) &&
                    !LayoutPr.IsAggregation;
            }
        }

        /// <summary>
        /// Gets a data provider instance which operates with the series data.
        /// </summary>
        private ChartSeriesDataProvider DataProvider
        {
            get
            {
                if (mDataProvider == null)
                    mDataProvider = new ChartSeriesDataProvider(this);

                return mDataProvider;
            }
        }

        /// <summary>
        /// Gets the dimension type of X values.
        /// </summary>
        /// <remarks>
        /// Dimension types are used for Word 2016 charts only.
        /// </remarks>
        private DimensionType XDimensionType
        {
            get
            {
                // Histogram chart has only 'val' dimension, which relates to X values.
                return IsHistogramChart ? DimensionType.Value : DimensionType.Category;
            }
        }

        /// <summary>
        /// Gets the dimension type of Y values.
        /// </summary>
        /// <remarks>
        /// Dimension types are used for Word 2016 charts only.
        /// </remarks>
        private DimensionType YDimensionType
        {
            get
            {
                switch (LayoutType)
                {
                    case SeriesLayout.Treemap:
                    case SeriesLayout.Sunburst:
                        return DimensionType.Size;
                    case SeriesLayout.RegionMap:
                        return DimensionType.ColorValue;
                    default:
                        return DimensionType.Value;
                }
            }
        }

        /// <summary>
        /// Returns the number of chart data values or the index of the last non-empty data value taking into account
        /// the chart type.
        /// </summary>
        private int GetValueBasedOnChartType(int y, int x)
        {
            // WORDSNET-19737 If the chart type is "Scatter", the minimum between the number of x values and
            // the number of y values should be used.
            if (IsScatterChartSeries && (y > 0) && (x > 0))
                return System.Math.Min(y, x);
            else if (y >= 0)
                return y;
            else
                return x;
        }

        private DmlChartValue[] mOrderedXValuesArray;
        private int mIndex;
        private int mOrder;
        private bool mSmooth;
        private bool mSmoothExplicitlySet;
        private BarShape mShape = BarShape.None;
        private DmlChartDimensionData mX;
        private DmlChartDimensionData mInheritedXVal;
        private DmlChartDimensionData mY;
        private DmlChartDimensionData mSize;
        private DmlChartDimensionData mDataLabelsRangeData;
        private ChartXValueCollection mXValues;
        private ChartYValueCollection mYValues;
        private BubbleSizeCollection mBubbleSizes;
        private ChartDataLabelCollection mDLbls;
        private List<DmlChartTrendline> mTrendlines = new List<DmlChartTrendline>();
        private DmlChartTx mTx;
        private ChartDataPoint mDefaultDPt;
        private ChartDataPointCollection mDPtCollection;
        private DmlChartErrorBars mXErrBars;
        private DmlChartErrorBars mYErrBars;
        private DmlChart mChart;
        private StringToObjDictionary<DmlExtension> mExtensions;
        private DmlChartValueColors mValueColors;
        private SeriesLayout mLayoutType;
        private bool mHidden;
        private ChartSeries mOwner;
        private string mUniqueId;
        private int mFormatIndex = -1;
        private int mDataId = -1;
        private int mAxisId = -1;
        private DmlChartSeriesLayoutPr mLayoutPr;
        private readonly DmlChartDataAnalyzer mDataAnalyzer = new DmlChartDataAnalyzer();
        private ChartSeriesDataProvider mDataProvider;
        private bool mHasDataLabels;

        /// <summary>
        /// The default value for a bubble size when using the add value method without specifying a bubble size.
        /// </summary>
        /// <remarks>
        /// The same default value as when using <see cref="ChartSeriesCollection.Add(string,double[],double[])"/> for
        /// a bubble chart.
        /// </remarks>
        private const double DefaultBubbleSize = double.NaN;
    }
}
