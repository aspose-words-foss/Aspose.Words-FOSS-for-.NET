// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Class represents stock chart.
    /// 5.7.2.199 stockChart (Stock Charts)
    /// </summary>
    internal class DmlStockChart :  DmlCartesianPlaneChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.StockChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return ChartSeriesType.Stock; }
        }

        internal DmlChartSpPr DropLines
        {
            get { return (DmlChartSpPr)ChartPr.GetProperty(DmlChartAttrs.DropLines); }
        }

        internal bool IsDropLinesVisible
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.IsDropLinesVisible); }
        }

        internal DmlChartSpPr HiLowLines
        {
            get { return (DmlChartSpPr)ChartPr.GetProperty(DmlChartAttrs.HiLowLines); }
        }

        internal DmlChartUpDownBars UpDownBars
        {
            get { return (DmlChartUpDownBars)ChartPr.GetProperty(DmlChartAttrs.UpDownBars); }
        }
    }
}
