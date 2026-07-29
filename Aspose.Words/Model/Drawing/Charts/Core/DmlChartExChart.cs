// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2017 by Alexander Zhiltsov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// This class represents a chart of the http://schemas.microsoft.com/office/drawing/2014/chartex schema 
    /// [MS-ODRAWXML]. Charts of the schema are introduced in MS Office 2016.
    /// </summary>
    internal class DmlChartExChart : DmlCartesianPlaneChart
    {
        /// <summary>
        /// Clones this DML chart.
        /// </summary>
        internal override DmlChart Clone()
        {
            DmlChartExChart lhs = (DmlChartExChart)base.Clone();

            return lhs;
        }

        /// <summary>
        /// Returns chart type. Detailed type is defined in chart series.
        /// </summary>
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.ChartExChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get
            {
                // Cannot determine series type because it is defined using ChartSeries.LayoutType.
                return ChartSeriesType.BoxAndWhisker;
            }
        }

        /// <summary>
        /// Gets the X axis.
        /// </summary>
        public override ChartAxis AxX
        {
            get 
            {
                ChartAxis axis = (SeriesType == ChartSeriesType.Funnel)
                    ? PlotArea.GetAxis(AxIdX)
                    : PlotArea.GetCrossAxis(AxIdY, AxIdX);

                if (axis == null)
                {
                    ChartAxis stubXAxis = new ChartAxis(ChartAxisType.Category, this, Document, true);
                    stubXAxis.AxId = AxIdX;
                    stubXAxis.PlotArea = PlotArea;
                    PlotArea.AddAxis(stubXAxis);
                    mStubXId = stubXAxis.AxId;
                    axis = stubXAxis;
                }

                if (!mIsDefaultXSet)
                    SetXDefaults(axis);

                return axis;
            }
        }

        private void SetXDefaults(ChartAxis axis)
        {
            if (SeriesType == ChartSeriesType.Funnel)
            {
                axis.AxPos = SimpleTypes.AxisPosition.Left;
                axis.CrossAx = axis.AxId == 1 ? 0 : 1;
                axis.ReverseOrder = true;
            }
            else
            {
                axis.AxPos = SimpleTypes.AxisPosition.Bottom;
            }

            mIsDefaultXSet = true;
        }

        /// <summary>
        /// Gets the Y axis.
        /// </summary>
        public override ChartAxis AxY
        {
            get
            {
                bool isFunnel = (SeriesType == ChartSeriesType.Funnel);
                ChartAxis axis = isFunnel ? PlotArea.GetAxis(AxIdX == 0 ? 1 : 0) : PlotArea.GetAxis(AxIdY);

                if (axis == null)
                {
                    ChartAxis stubYAxis = new ChartAxis(ChartAxisType.Value, this, Document, true);
                    mStubYId = AxIdX == 0 ? 1 : 0;
                    stubYAxis.AxId = mStubYId;
                    stubYAxis.PlotArea = PlotArea;
                    stubYAxis.Hidden = true;
                    stubYAxis.AxPos = isFunnel ? SimpleTypes.AxisPosition.Bottom : SimpleTypes.AxisPosition.Left;
                    PlotArea.AddAxis(stubYAxis);
                    axis = stubYAxis;
                }

                return axis;
            }
        }
     
        ///// <summary>
        ///// Indicates whether the axis is stub and should not be written.
        ///// </summary>
        internal bool IsStubAxis(int axId)
        {
            return (mStubXId == axId) || (mStubYId == axId);
        }

        private int mStubXId = int.MinValue;
        private int mStubYId = int.MinValue;
        private bool mIsDefaultXSet = false;
    }
}
