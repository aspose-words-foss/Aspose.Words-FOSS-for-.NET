// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2024 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents properties of a chart series group, that is, the properties of chart series of the same type
    /// associated with the same axes.
    /// </summary>
    /// <remarks>
    /// <para>Combo charts contains multiple chart series groups, with a separate group for each series type.</para>
    /// <para>Also, you can create a chart series group to assign secondary axes to one or more chart series.</para>
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">
    /// Working with Charts</a> documentation article.</para>
    /// </remarks>
    public class ChartSeriesGroup
    {
        /// <summary>
        /// Ctor to create instances of this class.
        /// </summary>
        internal ChartSeriesGroup(DmlChart dmlChart)
        {
            mDmlChart = dmlChart;
        }

        /// <summary>
        /// Gets the necessary type of X axis for series of the specified type.
        /// </summary>
        private static ChartAxisType GetAxisXType(ChartSeriesType seriesType)
        {
            switch (seriesType)
            {
                case ChartSeriesType.Area:
                case ChartSeriesType.AreaStacked:
                case ChartSeriesType.AreaPercentStacked:
                case ChartSeriesType.Area3D:
                case ChartSeriesType.Area3DStacked:
                case ChartSeriesType.Area3DPercentStacked:
                case ChartSeriesType.Bar:
                case ChartSeriesType.BarStacked:
                case ChartSeriesType.BarPercentStacked:
                case ChartSeriesType.Bar3D:
                case ChartSeriesType.Bar3DStacked:
                case ChartSeriesType.Bar3DPercentStacked:
                case ChartSeriesType.Column:
                case ChartSeriesType.ColumnStacked:
                case ChartSeriesType.ColumnPercentStacked:
                case ChartSeriesType.Column3D:
                case ChartSeriesType.Column3DStacked:
                case ChartSeriesType.Column3DPercentStacked:
                case ChartSeriesType.Column3DClustered:
                case ChartSeriesType.Line:
                case ChartSeriesType.LineStacked:
                case ChartSeriesType.LinePercentStacked:
                case ChartSeriesType.Line3D:
                case ChartSeriesType.Radar:
                case ChartSeriesType.Stock:
                case ChartSeriesType.Surface:
                case ChartSeriesType.Surface3D:
                case ChartSeriesType.Histogram:
                case ChartSeriesType.Pareto:
                case ChartSeriesType.ParetoLine:
                case ChartSeriesType.BoxAndWhisker:
                case ChartSeriesType.Waterfall:
                case ChartSeriesType.Funnel:
                    return ChartAxisType.Category;
                case ChartSeriesType.Bubble:
                case ChartSeriesType.Bubble3D:
                case ChartSeriesType.Scatter:
                    return ChartAxisType.Value;
                case ChartSeriesType.Doughnut:
                case ChartSeriesType.Pie:
                case ChartSeriesType.Pie3D:
                case ChartSeriesType.PieOfBar:
                case ChartSeriesType.PieOfPie:
                case ChartSeriesType.Treemap:
                case ChartSeriesType.Sunburst:
                case ChartSeriesType.RegionMap:
                    // Series of this types have no axes.
                    return ChartAxisType.Category;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the number of series groups that have axes.
        /// </summary>
        private int GetNumberOfSeriesGroupsWithAxes()
        {
            int count = 0;

            foreach (DmlChart chart in PlotArea.Charts)
            {
                if (chart.HasAxis)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Gets the type of chart series included in this group.
        /// </summary>
        public ChartSeriesType SeriesType
        {
            get { return mDmlChart.SeriesType; }
        }

        /// <summary>
        /// Gets or sets the axis group to which this series group belongs.
        /// </summary>
        public AxisGroup AxisGroup
        {
            get
            {
                return (!mDmlChart.HasAxis || (AxisX == PlotArea.PrimaryXAxis) || (AxisY == PlotArea.PrimaryYAxis))
                    ? AxisGroup.Primary
                    : AxisGroup.Secondary;
            }
            set
            {
                if (AxisGroup == value)
                    return;

                if (!mDmlChart.HasAxis)
                    throw new InvalidOperationException(UnsupportedAxesError);

                switch (value)
                {
                    case AxisGroup.Primary:
                    {
                        Debug.Assert(PlotArea.PrimaryXAxis != null);
                        Debug.Assert(PlotArea.PrimaryYAxis != null);
                        mDmlChart.AxisX = PlotArea.PrimaryXAxis;
                        mDmlChart.AxisY = PlotArea.PrimaryYAxis;
                        break;
                    }
                    case AxisGroup.Secondary:
                    {
                        if (PlotArea.Charts.Count <= 1)
                            throw new InvalidOperationException(SecondaryAxesInNonComboChartError);

                        if (GetNumberOfSeriesGroupsWithAxes() <= 1)
                            throw new InvalidOperationException(SecondaryAxesForSingleSeriesGroupError);

                        mDmlChart.AxisX = PlotArea.GetOrCreateSecondaryXAxis();
                        mDmlChart.AxisX.SetType(GetAxisXType(SeriesType));
                        mDmlChart.AxisY = PlotArea.GetOrCreateSecondaryYAxis();

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        /// <summary>
        /// Provides access to properties of the X axis of this series group.
        /// </summary>
        public ChartAxis AxisX
        {
            get { return mDmlChart.AxisX; }
        }

        /// <summary>
        /// Provides access to properties of the Y axis of this series group.
        /// </summary>
        public ChartAxis AxisY
        {
            get { return mDmlChart.AxisY; }
        }

        /// <summary>
        /// Gets a collection of series that belong to this series group.
        /// </summary>
        public ChartSeriesCollection Series
        {
            get
            {
                if (mSeriesCollection == null)
                    mSeriesCollection = new ChartSeriesCollection(mDmlChart);

                return mSeriesCollection;
            }
        }

        /// <summary>
        /// Gets or sets the percentage of how much the series bars or columns overlap.
        /// </summary>
        /// <remarks>
        /// <para>Applies to series groups of all bar and column types.</para>
        /// <para>The range of acceptable values is from -100 to 100 inclusive. A value of 0 indicates that there is no
        /// space between bars/columns. If the value is -100, the distance between bars/columns is equal to their width.
        /// A value of 100 means that the bars/columns overlap completely.</para>
        /// </remarks>
        public int Overlap
        {
            get
            {
                DmlBarChart barChart = mDmlChart as DmlBarChart;
                return (barChart != null) ? barChart.Overlap : 0;
            }
            set
            {
                DmlBarChart barChart = mDmlChart as DmlBarChart;
                if (barChart == null)
                    throw new InvalidOperationException(SettingUnsupportedPropertyError);

                ArgumentUtil.CheckRangeInclusive(value, -100, 100, "value");

                barChart.Overlap = value;
            }
        }

        /// <summary>
        /// Gets or sets the percentage of gap width between chart elements.
        /// </summary>
        /// <remarks>
        /// <para>Applies only to series groups of the bar, column, pie-of-bar, pie-of-pie, histogram, box&amp;whisker,
        /// waterfall and funnel types.</para>
        /// <para>The range of acceptable values is from 0 to 500 inclusive. For bar/column-based series groups, the
        /// property represents the space between bar clusters as a percentage of their width. For pie-of-pie and
        /// bar-of-pie charts, this is the space between the primary and secondary sections of the chart.</para>
        /// </remarks>
        public int GapWidth
        {
            get
            {
                if (!IsGapWidthSupported)
                    return 0;

                DmlBarChart barChart = mDmlChart as DmlBarChart;
                if (barChart != null)
                    return barChart.GapWidth;

                DmlOfPieChart ofPieChart = mDmlChart as DmlOfPieChart;
                if (ofPieChart != null)
                    return ofPieChart.GapWidth;

                Debug.Assert(mDmlChart.ChartType == DmlChartType.ChartExChart);
                AxisScaling axisScaling = mDmlChart.AxisX.Scaling;
                return !axisScaling.GapWidth.IsNullOrAuto
                    ? MathUtil.DoubleToInt(axisScaling.GapWidth.Value * 100) // Convert to percents.
                    : ChartExAutoGapWidth;
            }
            set
            {
                if (!IsGapWidthSupported)
                    throw new InvalidOperationException(SettingUnsupportedPropertyError);

                ArgumentUtil.CheckRangeInclusive(value, 0, 500, "value");

                DmlBarChart barChart = mDmlChart as DmlBarChart;
                if (barChart != null)
                {
                    barChart.GapWidth = value;
                    return;
                }

                DmlOfPieChart ofPieChart = mDmlChart as DmlOfPieChart;
                if (ofPieChart != null)
                {
                    ofPieChart.GapWidth = value;
                    return;
                }

                Debug.Assert(mDmlChart.ChartType == DmlChartType.ChartExChart);
                mDmlChart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(value / 100d); // Convert from percents.
            }
        }

        /// <summary>
        /// Gets or sets the size of the bubbles as a percentage of their default size.
        /// </summary>
        /// <remarks>
        /// <para>Applies only to series groups of the <see cref="ChartSeriesType.Bubble"/> and
        /// <see cref="ChartSeriesType.Bubble3D"/> types.</para>
        /// <para>The range of acceptable values is from 0 to 300 inclusive. The default value is 100.</para>
        /// </remarks>
        public int BubbleScale
        {
            get
            {
                DmlBubbleChart bubbleChart = mDmlChart as DmlBubbleChart;
                return (bubbleChart != null) ? bubbleChart.BubbleScale : 0;
            }
            set
            {
                DmlBubbleChart bubbleChart = mDmlChart as DmlBubbleChart;
                if (bubbleChart == null)
                    throw new InvalidOperationException(SettingUnsupportedPropertyError);

                ArgumentUtil.CheckRangeInclusive(value, 0, 300, "value");

                bubbleChart.BubbleScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle, in degrees, of the first slice of the parent pie chart.
        /// </summary>
        /// <remarks>
        /// <para>Applies to series groups of the <see cref="ChartSeriesType.Pie"/>, <see cref="ChartSeriesType.Pie3D"/>
        /// and <see cref="ChartSeriesType.Doughnut"/> types.</para>
        /// <para>The range of acceptable values is from 0 to 360 inclusive. The default value is 0.</para>
        /// </remarks>
        public int FirstSliceAngle
        {
            get
            {
                DmlPieChart pieChart = mDmlChart as DmlPieChart;
                if (pieChart == null)
                    return 0;

                switch (pieChart.ChartType)
                {
                    case DmlChartType.PieChart:
                    case DmlChartType.DoughnutChart:
                        return pieChart.FirstSliceAng;
                    case DmlChartType.Pie3DChart:
                        // MS Word and Word VBA uses this property as a first slice angle in a Pie 3D chart.
                        return pieChart.ChartSpace.ChartFormat.View3D.RotY;
                    default:
                        return 0;
                }
            }
            set
            {
                DmlPieChart pieChart = mDmlChart as DmlPieChart;
                if (pieChart == null)
                    throw new InvalidOperationException(SettingUnsupportedPropertyError);

                ArgumentUtil.CheckRangeInclusive(value, 0, 360, "value");

                switch (pieChart.ChartType)
                {
                    case DmlChartType.PieChart:
                    case DmlChartType.DoughnutChart:
                        pieChart.FirstSliceAng = value;
                        break;
                    case DmlChartType.Pie3DChart:
                        // MS Word and Word VBA uses this property as a first slice angle in a Pie 3D chart.
                        pieChart.ChartSpace.ChartFormat.View3D.RotY = value;
                        break;
                    default:
                        throw new InvalidOperationException(SettingUnsupportedPropertyError);
                }
            }
        }

        /// <summary>
        /// Gets or sets the hole size of the parent doughnut chart as a percentage.
        /// </summary>
        /// <remarks>
        /// <para>Applies only to series groups of the <see cref="ChartSeriesType.Doughnut"/> type.</para>
        /// <para>The range of acceptable values is from 0 to 90 inclusive. The default value is 75.</para>
        /// </remarks>
        public int DoughnutHoleSize
        {
            get
            {
                DmlDoughnutChart doughnutChart = mDmlChart as DmlDoughnutChart;
                return (doughnutChart != null) ? doughnutChart.HoleSize : 0;
            }
            set
            {
                DmlDoughnutChart doughnutChart = mDmlChart as DmlDoughnutChart;
                if (doughnutChart == null)
                    throw new InvalidOperationException(SettingUnsupportedPropertyError);

                ArgumentUtil.CheckRangeInclusive(value, 0, 90, "value");

                doughnutChart.HoleSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the pie chart secondary section as a percentage.
        /// </summary>
        /// <remarks>
        /// <para>Applies to series groups of the <see cref="ChartSeriesType.PieOfPie"/> and
        /// <see cref="ChartSeriesType.PieOfBar"/> types.</para>
        /// <para>The range of acceptable values is from 5 to 200 inclusive. The default value is 75.</para>
        /// </remarks>
        public int SecondSectionSize
        {
            get
            {
                DmlOfPieChart pieChart = mDmlChart as DmlOfPieChart;
                return (pieChart != null) ? pieChart.SecondPieSize : 0;
            }
            set
            {
                DmlOfPieChart pieChart = mDmlChart as DmlOfPieChart;
                if (pieChart == null)
                    throw new InvalidOperationException(SettingUnsupportedPropertyError);

                ArgumentUtil.CheckRangeInclusive(value, 5, 200, "value");

                pieChart.SecondPieSize = value;
            }
        }

        /// <summary>
        /// Gets a DML chart corresponding to this series group.
        /// </summary>
        internal DmlChart DmlChart
        {
            get { return mDmlChart; }
        }

        /// <summary>
        /// Gets a plot area of this series group.
        /// </summary>
        private DmlChartPlotArea PlotArea
        {
            get { return mDmlChart.PlotArea; }
        }

        /// <summary>
        /// Gets a flag indicating whether the <see cref="GapWidth"/> property is supported by this series group.
        /// </summary>
        private bool IsGapWidthSupported
        {
            get
            {
                switch (SeriesType)
                {
                    case ChartSeriesType.Bar:
                    case ChartSeriesType.BarStacked:
                    case ChartSeriesType.BarPercentStacked:
                    case ChartSeriesType.Bar3D:
                    case ChartSeriesType.Bar3DStacked:
                    case ChartSeriesType.Bar3DPercentStacked:
                    case ChartSeriesType.Column:
                    case ChartSeriesType.ColumnStacked:
                    case ChartSeriesType.ColumnPercentStacked:
                    case ChartSeriesType.Column3D:
                    case ChartSeriesType.Column3DClustered:
                    case ChartSeriesType.Column3DStacked:
                    case ChartSeriesType.Column3DPercentStacked:
                    case ChartSeriesType.PieOfPie:
                    case ChartSeriesType.PieOfBar:
                    case ChartSeriesType.Histogram:
                    case ChartSeriesType.BoxAndWhisker:
                    case ChartSeriesType.Waterfall:
                    case ChartSeriesType.Funnel:
                        return true;
                    default:
                        return false;
                }
            }
        }

        private readonly DmlChart mDmlChart;
        private ChartSeriesCollection mSeriesCollection;

        private const int ChartExAutoGapWidth = 33;

        private const string SettingUnsupportedPropertyError =
            "The property cannot be set for a series group of this type.";
        private const string SecondaryAxesInNonComboChartError =
            "Only a combo chart, that is, a chart with multiple series groups can have secondary axes.";
        private const string SecondaryAxesForSingleSeriesGroupError =
            "You cannot create secondary axes for a combo chart with a single series group that can have axes.";
        private const string UnsupportedAxesError =
            "Axes are not supported by this type of series group.";
    }
}
