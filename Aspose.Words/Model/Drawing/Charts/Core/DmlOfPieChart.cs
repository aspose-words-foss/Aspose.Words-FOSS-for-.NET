// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents OfPie Chart.
    /// 5.7.2.127 ofPieChart (Pie of Pie or Bar of Pie Charts)
    /// </summary>
    internal class DmlOfPieChart : DmlPieChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.OfPieChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get
            {
                switch (ChartPr.OfPieType)
                {
                    case OfPieType.Bar:
                        return ChartSeriesType.PieOfBar;
                    case OfPieType.Pie:
                    default:
                        return ChartSeriesType.PieOfPie;
                }
            }
        }

        /// <summary>
        /// Specifies whether this chart is pie of pie or bar of pie.
        /// </summary>
        internal OfPieType OfPieType
        {
            get { return ChartPr.OfPieType; }
            set { ChartPr.OfPieType = value; }
        }

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        internal int GapWidth
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.GapWidth); }
            set { ChartPr.SetProperty(DmlChartAttrs.GapWidth, value); }
        }

        /// <summary>
        /// Specifies how to determine which data points are in the second pie or bar on a pie of pie or bar of pie chart.
        /// </summary>
        internal SplitType SplitType
        {
            get { return (SplitType)ChartPr.GetProperty(DmlChartAttrs.SplitType); }
            set { ChartPr.SetProperty(DmlChartAttrs.SplitType, value); }
        }

        /// <summary>
        /// Specifies a value that shall be used to determine which data points 
        /// are in the second pie or bar on a pie of pie or bar of pie chart.
        /// </summary>
        internal double SplitPos
        {
            get { return (double)ChartPr.GetProperty(DmlChartAttrs.SplitPos); }
            set { ChartPr.SetProperty(DmlChartAttrs.SplitPos, value); }
        }

        /// <summary>
        /// Contains the custom split information for a pie-of-pie or bar-of-pie chart with a custom split type.
        /// </summary>
        internal int[] CustSplit
        {
            get { return (int[])ChartPr.GetProperty(DmlChartAttrs.CustSplit); }
            set { ChartPr.SetProperty(DmlChartAttrs.CustSplit, value); }
        }

        /// <summary>
        /// specifies the size of the second pie or bar of a pie of pie chart or a bar of pie chart, 
        /// as a percentage of the size of the first pie.
        /// </summary>
        internal int SecondPieSize
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.SecondPieSize); }
            set { ChartPr.SetProperty(DmlChartAttrs.SecondPieSize, value); }
        }

        internal DmlChartSpPr SerLines
        {
            get { return (DmlChartSpPr)ChartPr.GetProperty(DmlChartAttrs.SerLines); }
        }
    }
}
