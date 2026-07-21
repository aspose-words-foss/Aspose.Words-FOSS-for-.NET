// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2024 by Alexander Zhiltsov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents a collection of <see cref="ChartSeriesGroup"/> objects.
    /// </summary>
    /// <remarks>
    /// To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a>
    /// documentation article.
    /// </remarks>
    /// <dev>
    /// Only Pareto charts can have a <see cref="ChartSeriesGroup"/> instance that contains series of different types:
    /// <see cref="ChartSeriesType.Pareto"/> and <see cref="ChartSeriesType.ParetoLine"/>. When a Pareto series is added
    /// to a <see cref="ChartSeriesCollection"/>, a Pareto Line series is automatically added as well.
    /// Pareto line series have their own secondary Y axis.
    /// </dev>
    public class ChartSeriesGroupCollection : IEnumerable<ChartSeriesGroup>
    {
        /// <summary>
        /// Ctor to create instances of this class.
        /// </summary>
        internal ChartSeriesGroupCollection(DmlChartSpace chartSpace)
        {
            mChartSpace = chartSpace;
            FillSeriesGroups();
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartSeriesGroup> GetEnumerator()
        {
            return mSeriesGroups.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes a series group at the specified index. All child series will be removed from the chart.
        /// </summary>
        public void RemoveAt(int index)
        {
            ArgumentUtil.CheckRangeInclusive(index, 0, Count - 1, "index");

            // Let's do not remove all series groups so that adding series using the Chart.Series property will work
            // without errors. (When using the property, new series are added to the first DmlChart/SeriesGroup.)
            if (Count == 1)
                throw new InvalidOperationException(RequiredAtLeastOneSeriesGroupError);

            Debug.Assert(this[index].DmlChart == mChartSpace.Charts[index]);
            mChartSpace.Charts.RemoveAt(index);

            mSeriesGroups.RemoveAt(index);
        }

        /// <summary>
        /// Adds a new series group of the specified series type to this collection.
        /// </summary>
        /// <remarks>
        /// Combo charts can contain series groups only of the following types: area, bar, column, line, pie, scatter,
        /// radar and stock types (except the corresponding 3D series types).
        /// </remarks>
        public ChartSeriesGroup Add(ChartSeriesType seriesType)
        {
            // It is expected that a chart contains at least one series group.
            Debug.Assert(Count > 0);

            if (mChartSpace.IsChartEx ||
                // Throw only if the current chart is not already a combo chart.
                ((Count == 1) && !IsAllowedInComboCharts(this[0].SeriesType)))
            {
                throw new InvalidOperationException(CannotAddSeriesGroupsError);
            }

            if (!IsAllowedInComboCharts(seriesType))
                throw new InvalidOperationException(CannotAddSeriesGroupsOfThisTypeError);

            DmlChart dmlChart = CreateDmlChart(seriesType);
            mChartSpace.Charts.Add(dmlChart);

            return Add(dmlChart);
        }

        /// <summary>
        /// Creates and adds a new series group to this collection.
        /// </summary>
        private ChartSeriesGroup Add(DmlChart dmlChart)
        {
            ChartSeriesGroup seriesGroup = new ChartSeriesGroup(dmlChart);
            mSeriesGroups.Add(seriesGroup);
            return seriesGroup;
        }

        /// <summary>
        /// Creates an instance of <see cref="DmlChart"/> for the specified series type.
        /// </summary>
        private DmlChart CreateDmlChart(ChartSeriesType seriesType)
        {
            ChartType chartType = DmlChartUtil.SeriesTypeToChartType(seriesType);
            Document doc = (Document)mChartSpace.ChartFormat.Document;
            IChartInserter chartInserter = ReaderFactory.CreateChartInserter();

            DmlChartSpace tmpChartSpace = chartInserter.CreateChartSpace(chartType, doc);
            DmlChart newDmlChart = tmpChartSpace.FirstChart;
            newDmlChart.Series.Clear();

            if (newDmlChart.HasAxis)
                DefineAxesForNewDmlChart(newDmlChart);

            newDmlChart.SetPlotArea(mChartSpace.ChartFormat.PlotArea);

            return newDmlChart;
        }

        /// <summary>
        /// Sets the exiting axes to use in the specified newly created DML chart, or gets axes from the DML chart and
        /// adds them to the axes collection of the chart.
        /// </summary>
        private void DefineAxesForNewDmlChart(DmlChart dmlChart)
        {
            DmlChartPlotArea plotArea = mChartSpace.ChartFormat.PlotArea;

            IDmlChart2D chart2D = dmlChart as IDmlChart2D;
            if ((chart2D != null) && (chart2D.AxX != null))
            {
                Debug.Assert((chart2D.AxX != null) == (chart2D.AxY != null));

                ChartAxis primaryXAxis = plotArea.PrimaryXAxis;
                ChartAxis primaryYAxis = plotArea.PrimaryYAxis;
                Debug.Assert((primaryXAxis != null) == (primaryYAxis != null));

                AxisGroup axisGroup = GetNewDmlChartAxisGroup(dmlChart);
                ChartAxis axisX = (axisGroup == AxisGroup.Primary) ? primaryXAxis : plotArea.SecondaryXAxis;
                ChartAxis axisY = (axisGroup == AxisGroup.Primary) ? primaryYAxis : plotArea.SecondaryYAxis;
                Debug.Assert((axisX != null) == (axisY != null));

                if (axisX == null)
                {
                    plotArea.Axes.Add(chart2D.AxX);
                    plotArea.Axes.Add(chart2D.AxY);

                    if (primaryXAxis != null)
                    {
                        DmlChartUtil.SetXAxisOppositePosition(chart2D.AxX, primaryXAxis.ActualAxisPosition);
                        DmlChartUtil.SetYAxisOppositePosition(chart2D.AxY, primaryYAxis.ActualAxisPosition);
                    }
                }
                else
                {
                    dmlChart.AxisX = axisX;
                    chart2D.AxX = axisX;
                    dmlChart.AxisY = axisY;
                    chart2D.AxY = axisY;
                }
            }

            // Combo charts do not support 3D chart series groups.
            Debug.Assert(!(dmlChart is IDmlChart3D));
        }

        /// <summary>
        /// Gets an axis group to set to the specified newly created DML chart (series group).
        /// </summary>
        private AxisGroup GetNewDmlChartAxisGroup(DmlChart dmlChart)
        {
            if (Count == 0)
                return AxisGroup.Primary;

            // Assign numeric X chart series to a separate axis group than category X chart series like MS Word does.
            // Otherwise category values would refer to 1, 2, 3... numbers.
            if (IsNumericX(this[0].SeriesType) == IsNumericX(dmlChart.SeriesType))
                return this[0].AxisGroup;
            else
                return (this[0].AxisGroup == AxisGroup.Primary) ? AxisGroup.Secondary : AxisGroup.Primary;
        }

        /// <summary>
        /// Gets a flag indicating whether the specified series type has numeric X values.
        /// </summary>
        private static bool IsNumericX(ChartSeriesType seriesType)
        {
            switch (seriesType)
            {
                case ChartSeriesType.Scatter:
                case ChartSeriesType.Bubble:
                case ChartSeriesType.Bubble3D:
                case ChartSeriesType.Histogram:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates series groups for each DML chart of the parent chart and adds them to this collection.
        /// </summary>
        private void FillSeriesGroups()
        {
            foreach (DmlChart dmlChart in mChartSpace.Charts)
                Add(dmlChart);
        }

        /// <summary>
        /// Gets a flag indicating whether series groups of the specified type are allowed in combo charts.
        /// </summary>
        private static bool IsAllowedInComboCharts(ChartSeriesType seriesType)
        {
            switch (seriesType)
            {
                case ChartSeriesType.Area:
                case ChartSeriesType.AreaStacked:
                case ChartSeriesType.AreaPercentStacked:
                case ChartSeriesType.Bar:
                case ChartSeriesType.BarStacked:
                case ChartSeriesType.BarPercentStacked:
                case ChartSeriesType.Column:
                case ChartSeriesType.ColumnStacked:
                case ChartSeriesType.ColumnPercentStacked:
                case ChartSeriesType.Doughnut:
                case ChartSeriesType.Line:
                case ChartSeriesType.LineStacked:
                case ChartSeriesType.LinePercentStacked:
                case ChartSeriesType.Pie:
                case ChartSeriesType.PieOfBar:
                case ChartSeriesType.PieOfPie:
                case ChartSeriesType.Radar:
                case ChartSeriesType.Scatter:
                case ChartSeriesType.Stock:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a <see cref="ChartSeriesGroup"/> at the specified index.
        /// </summary>
        public ChartSeriesGroup this[int index]
        {
            get { return mSeriesGroups[index]; }
        }

        /// <summary>
        /// Returns the number of series groups in this collection.
        /// </summary>
        public int Count
        {
            get { return mSeriesGroups.Count; }
        }

        private readonly DmlChartSpace mChartSpace;
        private readonly List<ChartSeriesGroup> mSeriesGroups = new List<ChartSeriesGroup>();

        private const string RequiredAtLeastOneSeriesGroupError = "There must be at least one series group in a chart.";
        private const string CannotAddSeriesGroupsError = "You cannot add series groups to charts of this type.";
        private const string CannotAddSeriesGroupsOfThisTypeError = "You cannot add series groups of this type.";
    }
}
