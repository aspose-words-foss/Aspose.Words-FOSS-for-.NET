// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2012 by Alexey Noskov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents 5.7.2.146 plotArea (Plot Area) element.
    /// This element specifies the plot area of the chart.
    /// </summary>
    internal class DmlChartPlotArea : DmlExtensionListSource, IChartSeriesSource
    {
        internal DmlChartPlotArea(DmlChartFormat chartFormat)
        {
            mParentChartFormat = chartFormat;
        }

        internal void Warn(WarningType warningType, string message)
        {
            mParentChartFormat.Warn(warningType, message);
        }

        /// <summary>
        /// Adds axis to the collection.
        /// Axis can be gotten by its axid.
        /// </summary>
        internal void AddAxis(ChartAxis axis)
        {
            axis.PlotArea = this;
            mAxes.Add(axis);
            mAxesMap[axis.AxId] = axis;
        }

        /// <summary>
        /// Remove the axis with the specified ID from this plot area.
        /// </summary>
        internal void RemoveAxis(int axisId)
        {
            ChartAxis axis = mAxesMap[axisId];
            if (axis == null)
                return;

            mAxesMap.Remove(axisId);
            mAxes.Remove(axis);
        }

        /// <summary>
        /// Remove all axes from this plot area.
        /// </summary>
        internal void ClearAxes()
        {
            mAxes.Clear();
            mAxesMap.Clear();
        }

        /// <summary>
        /// Adds chart to the collection. There might be more than one chart on one plot area.
        /// </summary>
        internal void AddChart(DmlChart chart)
        {
            chart.SetPlotArea(this);
            mCharts.Add(chart);
        }

        /// <summary>
        /// Returns axis that corresponds the specified id.
        /// </summary>
        internal ChartAxis GetAxis(int axId)
        {
            return mAxesMap[axId];
        }

        /// <summary>
        /// Returns axis that crosses the specified one.
        /// </summary>
        internal ChartAxis GetCrossAxis(int axId, int supposedCrossAxIdX)
        {
            // Get supposed cross axis and check whether it actually crosses the specified axis.
            ChartAxis crossAx = GetAxis(supposedCrossAxIdX);
            if ((crossAx == null) || (crossAx.CrossAx == axId))
                return crossAx;

            foreach (ChartAxis axis in mAxes)
            {
                if (axis.CrossAx == axId)
                    return axis;
            }

            return crossAx;
        }

        /// <summary>
        /// Gets or creates a secondary X axis.
        /// </summary>
        internal ChartAxis GetOrCreateSecondaryXAxis()
        {
            // MS Word treats the first X axis and the first Y axis as primary. And it expects that they are linked
            // using the CrossAxis property.
            // The next two axes are secondary and must also be linked.
            // If only one secondary axis is needed: X or Y, the other must exist and be hidden.
            // A combo chart cannot have 3D DML charts, so there is no secondary Z axis.

            ChartAxis secondaryXAxis = SecondaryXAxis;
            if (secondaryXAxis != null)
                return secondaryXAxis;

            Debug.Assert(PrimaryXAxis != null);

            secondaryXAxis = PrimaryXAxis.Clone();
            secondaryXAxis.AxId = GenerateAxisId();
            DmlChartUtil.SetXAxisOppositePosition(secondaryXAxis, secondaryXAxis.ActualAxisPosition);

            ChartAxis secondaryYAxis = SecondaryYAxis;
            if (secondaryYAxis != null)
                LinkAxes(secondaryXAxis, secondaryYAxis);

            mAxes.Add(secondaryXAxis);
            return secondaryXAxis;
        }

        /// <summary>
        /// Gets or creates a secondary Y axis.
        /// </summary>
        internal ChartAxis GetOrCreateSecondaryYAxis()
        {
            ChartAxis secondaryYAxis = SecondaryYAxis;
            if (secondaryYAxis != null)
                return secondaryYAxis;

            Debug.Assert(PrimaryYAxis != null);

            secondaryYAxis = PrimaryYAxis.Clone();
            secondaryYAxis.AxId = GenerateAxisId();
            DmlChartUtil.SetYAxisOppositePosition(secondaryYAxis, secondaryYAxis.ActualAxisPosition);

            ChartAxis secondaryXAxis = SecondaryXAxis;
            if (secondaryXAxis != null)
                LinkAxes(secondaryXAxis, secondaryYAxis);

            mAxes.Add(secondaryYAxis);
            return secondaryYAxis;
        }

        internal DmlChartPlotArea Clone()
        {
            DmlChartPlotArea lhs = (DmlChartPlotArea)MemberwiseClone();

            if (mDataTable != null)
                lhs.mDataTable = mDataTable.Clone();

            if (mLayout != null)
                lhs.mLayout = mLayout.Clone();

            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mCharts != null)
            {
                List<DmlChart> chartList = new List<DmlChart>();
                foreach (DmlChart chart in mCharts)
                {
                   DmlChart cloned = chart.Clone();
                   cloned.SetPlotArea(lhs);
                   chartList.Add(cloned);
                   lhs.mCharts = chartList;
                }
            }

            if (mSurface != null)
                lhs.mSurface = mSurface.Clone();

            lhs.mRegionExtensions = CloneExtensions(mRegionExtensions);

            lhs.mAxes = new List<ChartAxis>(mAxes.Count);
            lhs.mAxesMap = new IntToObjDictionary<ChartAxis>();
            foreach (ChartAxis axis in mAxes)
            {
                ChartAxis clonedAxis = axis.Clone();
                lhs.AddAxis(clonedAxis);
                int chartIndex = mCharts.IndexOf(axis.Chart);
                clonedAxis.SetChart((chartIndex < 0) ? null : lhs.mCharts[chartIndex]);
            }

            lhs.Extensions = CloneExtensions();

            lhs.mSeriesList = null;

            return lhs;
        }

        /// <summary>
        /// Adds the series to the plot area.
        /// </summary>
        internal void AddSeries(ChartSeries series)
        {
            // On opening in MS Word, series are displayed on a chart by the order as they are located in XML markup
            // (i.e. by the order of DmlChart objects in DmlChartPlotArea.Charts and then by the order of series in
            // DmlChart.Series). In the list that is opened by clicking 'Change Chart Series Type', series are ordered
            // by the Order property value.
            series.Index = GetMaxSeriesIndex() + 1;
            series.Order = GetMaxSeriesOrder() + 1;

            if (mSeriesList != null)
                mSeriesList.Add(series);

            series.Chart.Series.Add(series);
        }

        /// <summary>
        /// Removes the specified series from the series list.
        /// </summary>
        internal void RemoveSeries(ChartSeries series)
        {
            if (mSeriesList != null)
                mSeriesList.Remove(series);
        }

        /// <summary>
        /// Fills in the <see cref="ChartAxis.Direction"/> property of all axes of the plot area.
        /// </summary>
        internal void InitAxisDirection()
        {
            foreach (DmlChart chart in mCharts)
            {
                if (chart.ChartPr.IsPropertySpecified(DmlChartAttrs.AxIdX))
                {
                    int xAxisId = (int)chart.ChartPr.GetProperty(DmlChartAttrs.AxIdX);
                    ChartAxis xAxis = GetAxis(xAxisId);
                    if ((xAxis != null) && (xAxis.Direction == AxisDirection.Unspecified))
                        xAxis.Direction = AxisDirection.X;
                }

                if (chart.ChartPr.IsPropertySpecified(DmlChartAttrs.AxIdY))
                {
                    int yAxisId = (int)chart.ChartPr.GetProperty(DmlChartAttrs.AxIdY);
                    ChartAxis yAxis = GetAxis(yAxisId);
                    if ((yAxis != null) && (yAxis.Direction == AxisDirection.Unspecified))
                        yAxis.Direction = AxisDirection.Y;
                }

                if (chart.ChartPr.IsPropertySpecified(DmlChartAttrs.AxIdZ))
                {
                    int zAxisId = (int)chart.ChartPr.GetProperty(DmlChartAttrs.AxIdZ);
                    ChartAxis zAxis = GetAxis(zAxisId);
                    if ((zAxis != null) && (zAxis.Direction == AxisDirection.Unspecified))
                        zAxis.Direction = AxisDirection.Z;
                }
            }

            // Let's use axis position as a fallback.
            foreach (ChartAxis axis in Axes)
            {
                if (axis.Direction != AxisDirection.Unspecified)
                    continue;

                axis.Direction = ((axis.AxPos == AxisPosition.Bottom) || (axis.AxPos == AxisPosition.Top))
                    ? AxisDirection.X
                    : AxisDirection.Y;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the plot area contains series related to the specified X axis.
        /// </summary>
        internal bool HasSeriesOfXAxis(ChartAxis axisX)
        {
            foreach (DmlChart chart in Charts)
            {
                if (chart.HasAxis && (chart.AxisX == axisX) && (chart.Series.Count > 0))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the height correction factor for 3D charts.
        /// </summary>
        /// <remarks>
        /// This experimental method is trying to repeat the MS Office behavior.
        /// </remarks>
        /// <param name="chart">the specified chart</param>
        /// <param name="heightFactor">the base height factor</param>
        /// <returns>the height correction factor</returns>
        private float GetHeightCorrectionCoeff(DmlChart chart, float heightFactor)
        {
            float factor = 1;
            int countCategories = 0;

            if (!chart.IsEmpty)
                foreach (ChartSeries series in chart.Series)
                    if (countCategories < series.ValueCount)
                        countCategories = series.ValueCount;

            //If the number of categories is 1, use a special coefficient.
            if (countCategories == 1)
                return 1 / (2 * heightFactor);

            IDmlChart3D chart3D = (IDmlChart3D)chart;

            if ((chart3D.AxZ != null) && !chart.IsSurface3DChart)
            {
                // The 3DAreaChart behavior is slightly different from 3DLineChart and 3DBarChart.
                // This can be seen clearly on a chart with zero values of RotX and RotY.
                factor = (chart.ChartType == DmlChartType.Area3DChart)
                    ? GetHeightFactorArea3DChart(chart, heightFactor)
                    : GetHeightFactor3D(chart, heightFactor, factor);
            }

            return factor;
        }

        /// <summary>
        /// Calculates the height factor for 3D charts.
        /// </summary>
        private float GetHeightFactor3D(DmlChart chart, float heightFactor, float factor)
        {
            if (mParentChartFormat.View3D.RotX != 0)
            {
                if (chart.Series.Count == 1)
                    factor = (heightFactor < 0.85) ? 0.67f : 0.49f;
                else
                    factor = (heightFactor < 0.84) ? 0.86f : 0.55f;
            }

            return factor;
        }

        /// <summary>
        /// Calculates the height factor for Area3D charts.
        /// </summary>
        private static float GetHeightFactorArea3DChart(DmlChart chart, float heightFactor)
        {
            float factor;

            if (chart.Series.Count == 1)
                factor = (heightFactor < 0.85) ? 0.67f : 0.45f;
            else
                factor = (heightFactor < 0.83) ? 0.85f : 0.55f;

            return factor;
        }

        /// <summary>
        /// Generates an unique axis ID.
        /// </summary>
        private int GenerateAxisId()
        {
            int id = mLastAxisId;
            bool found;

            do
            {
                found = false;
                id++;

                foreach (ChartAxis axis in mAxes)
                {
                    if (axis.AxId == id)
                    {
                        found = true;
                        break;
                    }
                }
            }
            while (found);

            mLastAxisId = id;
            return id;
        }

        /// <summary>
        /// Links the specified X and Y axes.
        /// </summary>
        private static void LinkAxes(ChartAxis xAxis, ChartAxis yAxis)
        {
            xAxis.CrossAx = yAxis.AxId;
            xAxis.CrossAxis = yAxis;
            yAxis.CrossAx = xAxis.AxId;
            yAxis.CrossAxis = xAxis;
        }

        /// <summary>
        /// Finds the first or second axis of the specified direction depending on the <paramref name="returnSecond"/>
        /// parameter.
        /// </summary>
        private ChartAxis FindAxis(AxisDirection direction, bool returnSecond)
        {
            bool isFirst = true;

            foreach (ChartAxis axis in Axes)
            {
                if (axis.Direction == direction)
                {
                    if (!returnSecond || !isFirst)
                        return axis;

                    isFirst = false;
                }

                Debug.Assert(axis.Direction != AxisDirection.Unspecified);
            }

            return null;
        }

        /// <summary>
        /// Returns true if legend should be rendered for each data point.
        /// </summary>
        internal bool RenderLegendForDataPoints
        {
            get
            {
                // Legend is rendered for each data point if there is only one chart and "RenderLegendForDataPoints"
                // property of the first series is "true".
                if (Charts.Count == 1)
                    return FirstChart.RenderLegendForDataPoints;

                return false;
            }
        }

        /// <summary>
        /// Returns list of charts of the current plot area.
        /// Do not add items to this list, use <see cref="AddChart"/> method instead.
        /// </summary>
        internal IList<DmlChart> Charts
        {
            get { return mCharts; }
        }

        /// <summary>
        /// Returns the list of not empty charts of the current plot area.
        /// </summary>
        internal IList<DmlChart> NonEmptyCharts
        {
            get
            {
                if (mNonEmptyCharts == null)
                {
                    mNonEmptyCharts = new List<DmlChart>();
                    foreach (DmlChart chart in Charts)
                    {
                        if (!chart.IsEmpty)
                            mNonEmptyCharts.Add(chart);
                    }
                }

                return mNonEmptyCharts;
            }
        }

        internal DmlChart FirstChart
        {
            get
            {
                if (Charts.Count > 0)
                    return Charts[0];

                return null;
            }
        }

        internal DmlChartFormat ParentChartFormat
        {
            get { return mParentChartFormat; }
        }

        /// <summary>
        /// Sets the parent chart format of this plot area.
        /// </summary>
        internal void SetChartFormat(DmlChartFormat chartFormat)
        {
            mParentChartFormat = chartFormat;
            if (mDataTable != null)
                mDataTable.SetPlotArea(chartFormat.PlotArea);
        }

        /// <summary>
        /// Gets a list of chart axes of this plot area.
        /// </summary>
        internal IList<ChartAxis> Axes
        {
            get { return mAxes; }
        }

        internal ChartDataTable DataTable
        {
            get { return mDataTable; }
            set { mDataTable = value; }
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

        /// <summary>
        /// Returns true if plot area represent surface 3D chart.
        /// </summary>
        internal bool IsSurface3D
        {
            get { return (FirstChart != null) && FirstChart.IsSurface3DChart; }
        }

        /// <summary>
        /// Returns true if plot area contains a pie chart.
        /// </summary>
        internal bool HasPieChart
        {
            get
            {
                foreach (DmlChart chart in Charts)
                    if (!chart.IsEmpty && chart.IsPieChart && (chart.ChartType != DmlChartType.OfPieChart))
                            return true;

                return false;
            }
        }

        /// <summary>
        /// Returns true if the plot area has axis.
        /// </summary>
        internal bool HasAxis
        {
            get { return (mAxes.Count > 0); }
        }

        /// <summary>
        /// Provides access to the primary X axis of the plot area.
        /// </summary>
        internal ChartAxis PrimaryXAxis
        {
            get
            {
                // MS Word treats the first X axis as primary. And it expects that the first X axis and the first Y axis
                // are linked using the CrossAxis property, otherwise there may be issues with the axes in MS Word.
                // So, just get the first X axis.
                return FindAxis(AxisDirection.X, false);
            }
        }

        /// <summary>
        /// Provides access to the primary Y axis of the plot area.
        /// </summary>
        internal ChartAxis PrimaryYAxis
        {
            get
            {
                // MS Word treats the first Y axis as primary.
                return FindAxis(AxisDirection.Y, false);
            }
        }

        /// <summary>
        /// Provides access to the Z axis of the plot area.
        /// </summary>
        /// <dev>
        /// It seems in contrast with X and Y axes, a chart can contain only one Z axis.
        /// </dev>
        internal ChartAxis ZAxis
        {
            get
            {
                return FindAxis(AxisDirection.Z, false);
            }
        }

        /// <summary>
        /// Provides access to the secondary X axis of the plot area.
        /// </summary>
        internal ChartAxis SecondaryXAxis
        {
            get
            {
                // The first X and first Y axes are primary axes, they are lined using the CrossAxis property.
                // The other two axes are X and Y secondary axes. They must be linked to each other too.
                // If it is needed only X or Y secondary axis, the other must exist and be hidden.
                // Combo chart cannot contain 3D DML charts, so it looks like there cannot be a secondary Z axis.
                return FindAxis(AxisDirection.X, true);
            }
        }

        /// <summary>
        /// Provides access to the secondary Y axis of the plot area.
        /// </summary>
        internal ChartAxis SecondaryYAxis
        {
            get
            {
                // See the comment in SecondaryXAxis.
                return FindAxis(AxisDirection.Y, true);
            }
        }

        /// <summary>
        /// Gets or sets properties of the canvas on which the series is plotted.
        /// </summary>
        /// <dev>
        /// Stores data of the plotSurface element of the 2.24.3.66 CT_PlotSurface complex type [MS-ODRAWXML].
        /// </dev>
        internal DmlChartSurface Surface
        {
            get { return mSurface; }
            set { mSurface = value; }
        }

        /// <summary>
        /// Gets or sets extensions of plot area region.
        /// </summary>
        /// <dev>
        /// Stores data of the extLst element of the 2.24.3.65 CT_PlotAreaRegion complex type [MS-ODRAWXML].
        /// </dev>
        internal StringToObjDictionary<DmlExtension> RegionExtensions
        {
            get { return mRegionExtensions; }
            set { mRegionExtensions = value; }
        }

        /// <summary>
        /// Gets the maximum value of <see cref="ChartSeries.Index"/> property of all series in this plot area.
        /// </summary>
        private int GetMaxSeriesIndex()
        {
            int maxIndex = -1;

            foreach (DmlChart chart in Charts)
            {
                foreach (ChartSeries series in chart.Series)
                {
                    if (maxIndex < series.Index)
                        maxIndex = series.Index;
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Gets the maximum value of <see cref="ChartSeries.Order"/> property of all series in this plot area.
        /// </summary>
        private int GetMaxSeriesOrder()
        {
            int maxOrder = -1;

            foreach (DmlChart chart in Charts)
            {
                foreach (ChartSeries series in chart.Series)
                {
                    if (maxOrder < series.Order)
                        maxOrder = series.Order;
                }
            }

            return maxOrder;
        }

        /// <summary>
        /// Specifies that the plot area size shall determine the size of the plot area,
        /// not including the tick marks and axis labels.
        /// </summary>
        internal bool IsInner
        {
            get { return ((Layout != null) && (Layout.LayoutTarget == LayoutTarget.Inner)); }
        }

        #region IChartSeriesSource

        /// <summary>
        /// Removes the series at the specified index from the chart.
        /// </summary>
        void IChartSeriesSource.RemoveAt(int index)
        {
            ChartSeries series = ((IChartSeriesSource)this).SeriesList[index];
            mSeriesList.RemoveAt(index);
            series.Chart.Series.Remove(series);
        }

        /// <summary>
        /// Removes all the series from the chart.
        /// </summary>
        void IChartSeriesSource.Clear()
        {
            if (mSeriesList != null)
                mSeriesList.Clear();

            DmlChartSpace chartSpace = mParentChartFormat.DmlChartSpace;
            if (chartSpace.IsChartEx)
                chartSpace.Data.Clear();

            foreach (DmlChart chart in Charts)
            {
                chart.Series.Clear();

                // Need to clear the X axis date flag otherwise the axis type may not match inserted data.
                chart.SetXAxisDateCategoryAttribute(false);
            }
        }

        /// <summary>
        /// Returns a list of all chart series of the current plot area.
        /// Do not add/remove chart series to/from this list directly.
        /// </summary>
        IList<ChartSeries> IChartSeriesSource.SeriesList
        {
            get
            {
                if (mSeriesList == null)
                {
                    mSeriesList = new List<ChartSeries>();
                    foreach (DmlChart chart in mCharts)
                        mSeriesList.AddRange(chart.Series);

                    mSeriesList.Sort(new ChartSeriesComparerByIndex());
                }

                return mSeriesList;
            }
        }

        /// <summary>
        /// Gets the DML chart to which adding series are placed.
        /// </summary>
        DmlChart IChartSeriesSource.DestinationChart
        {
            get { return FirstChart; }
        }

        #endregion IChartSeriesSource

        private List<DmlChart> mCharts = new List<DmlChart>();
        private List<DmlChart> mNonEmptyCharts;
        private IntToObjDictionary<ChartAxis> mAxesMap = new IntToObjDictionary<ChartAxis>();
        private List<ChartAxis> mAxes = new List<ChartAxis>();
        private ChartDataTable mDataTable;
        private DmlChartManualLayout mLayout;
        private DmlChartSpPr mSpPr;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlChartFormat mParentChartFormat;
        private DmlChartSurface mSurface;
        private StringToObjDictionary<DmlExtension> mRegionExtensions;
        private List<ChartSeries> mSeriesList;
        private int mLastAxisId;

        /// <summary>
        /// Compares chart series by <see cref="ChartSeries.Index"/>.
        /// </summary>
        private class ChartSeriesComparerByIndex : IComparer<ChartSeries>
        {
            int IComparer<ChartSeries>.Compare(ChartSeries series1, ChartSeries series2)
            {
                return series1.Index - series2.Index;
            }
        }
    }
}
