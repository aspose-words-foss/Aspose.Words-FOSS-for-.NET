// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2022 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    internal static class DmlChartUtil
    {
        /// <summary>
        /// Converts double to date, taking into account DateTime.MinValue and DateTime.MaxValue.
        /// </summary>
        /// <param name="value">The specified double</param>
        /// <returns>The converted datetime value</returns>
        internal static DateTime GetDateFromDouble(double value)
        {
            // The minimum date for MS Word is "00.01.1900", internally "0", which is used for time-only values. But this
            // date is not valid, so use "01.01.1900" as a minimum.
            // There is a difference between MS Word and .NET while converting a double to a date. "1" is "01/01/1900" in
            // MS Word and "31/12/1899" in .NET. Also, MS Word considers that "29/02/1900" (internally "60") exists, but
            // .NET doesn't.
            // If the value is more than 60, then there is no difference in conversion.
            double correctedValue = value;

            if (value < WordOneDayDifferenceNumericDateValue)
                correctedValue += (value < WordTwoDaysDifferenceNumericDateValue) ? 2 : 1;

            DateTime baseDate;

            if (MathUtil.IsLessOrEqual(correctedValue, DateTime.MinValue.ToOADate()))
                baseDate = DateTime.MinValue;
            else if (MathUtil.IsGreaterOrEqual(correctedValue, DateTime.MaxValue.ToOADate()))
                baseDate = DateTime.MaxValue;
            else
                baseDate = GetMsWordDate(correctedValue);

            return baseDate;
        }

        /// <summary>
        /// Converts date to double.
        /// </summary>
        internal static double GetDoubleFromDate(DateTime value)
        {
            // The minimum date for MS Word is "00.01.1900", internally "0", which is used for time-only values. But this
            // date is not valid, so use "01.01.1900" as a minimum.
            // There is a difference between MS Word and .NET while converting a double to a date. "1" is "01/01/1900" in
            // MS Word and "31/12/1899" in .NET. Also, MS Word considers that "29/02/1900" (internally "60") exists, but
            // .NET doesn't.
            // If the value is more than 60, then there is no difference in conversion.
            double numericValue = value.ToOADate();
            if (numericValue < WordOneDayDifferenceNumericDateValue)
                numericValue -= (numericValue < WordTwoDaysDifferenceNumericDateValue + 1) ? 2 : 1;

            return numericValue;
        }

        /// <summary>
        /// Checks whether the specified double value is within the allowed datetime range.
        /// </summary>
        internal static bool CanBeDate(double value)
        {
            DateTime date = GetDateFromDouble(value);

            return (date > DateTime.MinValue) && (date < DateTime.MaxValue);
        }

        /// <summary>
        /// Indicates whether the current document is created in MS Word 2007 or in an earlier version.
        /// </summary>
        internal static bool IsMsWord2007OrLower(DocumentBase doc)
        {
            Document document = (doc as Document);
            if (document == null)
                return true;

            MsWordVersionCore version = (MsWordVersionCore)(document.BuiltInDocumentProperties.Version >> 16);

            return (version <= MsWordVersionCore.Word2007);
        }

        /// <summary>
        /// Moves the specified X axis to a position opposite the specified position.
        /// </summary>
        internal static void SetXAxisOppositePosition(ChartAxis axis, AxisPosition referencedPosition)
        {
            switch (referencedPosition)
            {
                case AxisPosition.Bottom:
                {
                    if (axis.ActualAxisPosition == AxisPosition.Top)
                        return;

                    axis.AxPos = AxisPosition.Top;
                    axis.Crosses = AxisCrosses.Maximum;
                    // Axis title overlaps tick labels without this.
                    axis.CrossBetween = CrossBetween.MidpointOfCategory;
                    break;
                }
                case AxisPosition.Top:
                {
                    if (axis.ActualAxisPosition == AxisPosition.Bottom)
                        return;

                    axis.AxPos = AxisPosition.Bottom;
                    axis.Crosses = AxisCrosses.Automatic;
                    break;
                }
            }
        }

        /// <summary>
        /// Moves the specified Y axis to a position opposite the specified position.
        /// </summary>
        internal static void SetYAxisOppositePosition(ChartAxis axis, AxisPosition referencedPosition)
        {
            switch (referencedPosition)
            {
                case AxisPosition.Left:
                {
                    if (axis.ActualAxisPosition == AxisPosition.Right)
                        return;

                    axis.AxPos = AxisPosition.Right;
                    axis.Crosses = AxisCrosses.Maximum;
                    break;
                }
                case AxisPosition.Right:
                {
                    if (axis.ActualAxisPosition == AxisPosition.Left)
                        return;

                    axis.AxPos = AxisPosition.Left;
                    axis.Crosses = AxisCrosses.Automatic;
                    break;
                }
            }
        }

        /// <summary>
        /// Gets a corresponding <see cref="ChartType"/> for the specified <see cref="ChartSeriesType"/>.
        /// Throws an exception if the conversion cannot be performed.
        /// </summary>
        internal static ChartType SeriesTypeToChartType(ChartSeriesType seriesType)
        {
            switch (seriesType)
            {
                case ChartSeriesType.Area:
                    return ChartType.Area;
                case ChartSeriesType.AreaStacked:
                    return ChartType.AreaStacked;
                case ChartSeriesType.AreaPercentStacked:
                    return ChartType.AreaPercentStacked;
                case ChartSeriesType.Area3D:
                    return ChartType.Area3D;
                case ChartSeriesType.Area3DStacked:
                    return ChartType.Area3DStacked;
                case ChartSeriesType.Area3DPercentStacked:
                    return ChartType.Area3DPercentStacked;
                case ChartSeriesType.Bar:
                    return ChartType.Bar;
                case ChartSeriesType.BarStacked:
                    return ChartType.BarStacked;
                case ChartSeriesType.BarPercentStacked:
                    return ChartType.BarPercentStacked;
                case ChartSeriesType.Bar3D:
                    return ChartType.Bar3D;
                case ChartSeriesType.Bar3DStacked:
                    return ChartType.Bar3DStacked;
                case ChartSeriesType.Bar3DPercentStacked:
                    return ChartType.Bar3DPercentStacked;
                case ChartSeriesType.Bubble:
                    return ChartType.Bubble;
                case ChartSeriesType.Bubble3D:
                    return ChartType.Bubble3D;
                case ChartSeriesType.Column:
                    return ChartType.Column;
                case ChartSeriesType.ColumnStacked:
                    return ChartType.ColumnStacked;
                case ChartSeriesType.ColumnPercentStacked:
                    return ChartType.ColumnPercentStacked;
                case ChartSeriesType.Column3D:
                    return ChartType.Column3D;
                case ChartSeriesType.Column3DStacked:
                    return ChartType.Column3DStacked;
                case ChartSeriesType.Column3DPercentStacked:
                    return ChartType.Column3DPercentStacked;
                case ChartSeriesType.Column3DClustered:
                    return ChartType.Column3DClustered;
                case ChartSeriesType.Doughnut:
                    return ChartType.Doughnut;
                case ChartSeriesType.Line:
                    return ChartType.Line;
                case ChartSeriesType.LineStacked:
                    return ChartType.LineStacked;
                case ChartSeriesType.LinePercentStacked:
                    return ChartType.LinePercentStacked;
                case ChartSeriesType.Line3D:
                    return ChartType.Line3D;
                case ChartSeriesType.Pie:
                    return ChartType.Pie;
                case ChartSeriesType.Pie3D:
                    return ChartType.Pie3D;
                case ChartSeriesType.PieOfBar:
                    return ChartType.PieOfBar;
                case ChartSeriesType.PieOfPie:
                    return ChartType.PieOfPie;
                case ChartSeriesType.Radar:
                    return ChartType.Radar;
                case ChartSeriesType.Scatter:
                    return ChartType.Scatter;
                case ChartSeriesType.Stock:
                    return ChartType.Stock;
                case ChartSeriesType.Surface:
                    return ChartType.Surface;
                case ChartSeriesType.Surface3D:
                    return ChartType.Surface3D;
                case ChartSeriesType.Treemap:
                    return ChartType.Treemap;
                case ChartSeriesType.Sunburst:
                    return ChartType.Sunburst;
                case ChartSeriesType.Histogram:
                    return ChartType.Histogram;
                case ChartSeriesType.Pareto:
                case ChartSeriesType.ParetoLine:
                    return ChartType.Pareto;
                case ChartSeriesType.BoxAndWhisker:
                    return ChartType.BoxAndWhisker;
                case ChartSeriesType.Waterfall:
                    return ChartType.Waterfall;
                case ChartSeriesType.Funnel:
                    return ChartType.Funnel;
                // There are no corresponding chart types for these series types.
                case ChartSeriesType.RegionMap:
                default:
                    return UnknownChartType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the position is supported by data labels in a series of the specified type.
        /// </summary>
        internal static bool IsDataLabelPositionSupported(ChartSeriesType seriesType, ChartDataLabelPosition value)
        {
            switch (seriesType)
            {
                case ChartSeriesType.Area:
                case ChartSeriesType.AreaStacked:
                case ChartSeriesType.AreaPercentStacked:
                case ChartSeriesType.Area3D:
                case ChartSeriesType.Area3DStacked:
                case ChartSeriesType.Area3DPercentStacked:
                case ChartSeriesType.Bar3D:
                case ChartSeriesType.Bar3DStacked:
                case ChartSeriesType.Bar3DPercentStacked:
                case ChartSeriesType.Column3D:
                case ChartSeriesType.Column3DStacked:
                case ChartSeriesType.Column3DPercentStacked:
                case ChartSeriesType.Column3DClustered:
                case ChartSeriesType.Doughnut:
                case ChartSeriesType.Line3D:
                case ChartSeriesType.Radar:
                case ChartSeriesType.Surface:
                case ChartSeriesType.Surface3D:
                case ChartSeriesType.Treemap:
                case ChartSeriesType.Sunburst:
                case ChartSeriesType.ParetoLine:
                case ChartSeriesType.Funnel:
                case ChartSeriesType.RegionMap:
                    return false;
                case ChartSeriesType.Bar:
                case ChartSeriesType.Column:
                case ChartSeriesType.Histogram:
                case ChartSeriesType.Pareto:
                case ChartSeriesType.Waterfall:
                    return
                        (value == ChartDataLabelPosition.Center) || (value == ChartDataLabelPosition.InsideBase) ||
                        (value == ChartDataLabelPosition.InsideEnd) || (value == ChartDataLabelPosition.OutsideEnd);
                case ChartSeriesType.BarStacked:
                case ChartSeriesType.BarPercentStacked:
                case ChartSeriesType.ColumnStacked:
                case ChartSeriesType.ColumnPercentStacked:
                    return
                        (value == ChartDataLabelPosition.Center) || (value == ChartDataLabelPosition.InsideBase) ||
                        (value == ChartDataLabelPosition.InsideEnd);
                case ChartSeriesType.Bubble:
                case ChartSeriesType.Bubble3D:
                case ChartSeriesType.Line:
                case ChartSeriesType.LineStacked:
                case ChartSeriesType.LinePercentStacked:
                case ChartSeriesType.Scatter:
                case ChartSeriesType.Stock:
                    return
                        (value == ChartDataLabelPosition.Center) || (value == ChartDataLabelPosition.Left) ||
                        (value == ChartDataLabelPosition.Right) || (value == ChartDataLabelPosition.Above) ||
                        (value == ChartDataLabelPosition.Below);
                case ChartSeriesType.Pie:
                case ChartSeriesType.Pie3D:
                case ChartSeriesType.PieOfBar:
                case ChartSeriesType.PieOfPie:
                    return
                        (value == ChartDataLabelPosition.Center) || (value == ChartDataLabelPosition.InsideEnd) ||
                        (value == ChartDataLabelPosition.OutsideEnd) || (value == ChartDataLabelPosition.BestFit);
                case ChartSeriesType.BoxAndWhisker:
                    return
                        (value == ChartDataLabelPosition.Left) || (value == ChartDataLabelPosition.Right) ||
                        (value == ChartDataLabelPosition.Above) || (value == ChartDataLabelPosition.Below);
                default:
                    throw new InvalidOperationException("Unknown series type.");
            }
        }

        /// <summary>
        /// Gets the default position of data labels in a series of the specified type.
        /// </summary>
        internal static ChartDataLabelPosition GetDataLabelDefaultPosition(ChartSeriesType seriesType)
        {
            switch (seriesType)
            {
                case ChartSeriesType.Area:
                case ChartSeriesType.AreaStacked:
                case ChartSeriesType.AreaPercentStacked:
                case ChartSeriesType.Area3D:
                case ChartSeriesType.Area3DStacked:
                case ChartSeriesType.Area3DPercentStacked:
                case ChartSeriesType.Bar3D:
                case ChartSeriesType.Bar3DStacked:
                case ChartSeriesType.Bar3DPercentStacked:
                case ChartSeriesType.Column3D:
                case ChartSeriesType.Column3DStacked:
                case ChartSeriesType.Column3DPercentStacked:
                case ChartSeriesType.Column3DClustered:
                case ChartSeriesType.Doughnut:
                case ChartSeriesType.Line3D:
                case ChartSeriesType.Radar:
                case ChartSeriesType.Surface:
                case ChartSeriesType.Surface3D:
                case ChartSeriesType.Treemap:
                case ChartSeriesType.Sunburst:
                case ChartSeriesType.ParetoLine:
                case ChartSeriesType.Funnel:
                case ChartSeriesType.RegionMap:
                    return DmlChartDataLabelPr.DefaultPosition;
                case ChartSeriesType.Bar:
                case ChartSeriesType.Column:
                case ChartSeriesType.Histogram:
                case ChartSeriesType.Pareto:
                case ChartSeriesType.Waterfall:
                    return ChartDataLabelPosition.OutsideEnd;
                case ChartSeriesType.BarStacked:
                case ChartSeriesType.BarPercentStacked:
                case ChartSeriesType.ColumnStacked:
                case ChartSeriesType.ColumnPercentStacked:
                    return ChartDataLabelPosition.Center;
                case ChartSeriesType.Bubble:
                case ChartSeriesType.Bubble3D:
                case ChartSeriesType.Line:
                case ChartSeriesType.LineStacked:
                case ChartSeriesType.LinePercentStacked:
                case ChartSeriesType.Scatter:
                case ChartSeriesType.Stock:
                    return ChartDataLabelPosition.Right;
                case ChartSeriesType.Pie:
                case ChartSeriesType.Pie3D:
                case ChartSeriesType.PieOfBar:
                case ChartSeriesType.PieOfPie:
                    return ChartDataLabelPosition.BestFit;
                case ChartSeriesType.BoxAndWhisker:
                    return ChartDataLabelPosition.Right;
                default:
                    throw new InvalidOperationException("Unknown series type.");
            }
        }

        /// <summary>
        /// Gets a date based on the double representation of the one second in MS Word.
        /// </summary>
        private static DateTime GetMsWordDate(double value)
        {

            double intPart = System.Math.Round(value);
            double delta = value - intPart;
            DateTime baseDate = DateTime.FromOADate(intPart);

            if (MathUtil.IsZero(delta))
                return baseDate;

            // WORDSNET-19731 The numbers after the point must be converted to hours, minutes and seconds. But .Net
            // and MS Word convert the value differently, the minimum unit of time in MS Word is a second, and .Net is a
            // millisecond.
            double seconds = System.Math.Round(delta / OneMsWordSecond);

            return baseDate.AddSeconds(seconds);
        }

        /// <summary>
        /// Gets the minimum date value allowed in MS Excel (01.01.1900).
        /// </summary>
        internal static readonly DateTime MinDateValue = new DateTime(1900, 1, 1);

        /// <summary>
        /// Double representation of one second in MS Word charts.
        /// </summary>
        /// <remarks>
        /// This is an experimental value.
        /// </remarks>
        private const double OneMsWordSecond = 1.15740740727512E-05d;

        private const int WordOneDayDifferenceNumericDateValue = 61;
        private const int WordTwoDaysDifferenceNumericDateValue = 1;

        internal const ChartType UnknownChartType = (ChartType)(-1);

        internal const string ChartExUnsupportedProperty = "The property cannot be set in a Word 2016 chart.";
    }
}
