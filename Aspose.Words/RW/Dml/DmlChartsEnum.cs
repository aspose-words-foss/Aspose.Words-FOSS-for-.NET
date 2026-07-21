// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.RW.Dml
{
    /// <summary>
    /// Class contains method for converting string values read from XML to the corresponding enum of simple types values.
    /// </summary>
    internal static class DmlChartsEnum
    {
        internal static AxisPosition DmlToAxisPosition(string value)
        {
            return (AxisPosition)gAxisPositionMap.GetValue(value, (int)AxisPosition.Bottom);
        }

        internal static string AxisPositionToDml(AxisPosition value)
        {
            return gAxisPositionMap.GetValue((int)value, "");
        }

        internal static BarDirection DmlToBarDirection(string value)
        {
            return (BarDirection)gBarDirectionMap.GetValue(value, (int)BarDirection.Column);
        }

        internal static string BarDirectionToDml(BarDirection value)
        {
            return gBarDirectionMap.GetValue((int)value, "");
        }

        internal static Grouping DmlToGrouping(string value)
        {
            return (Grouping)gBarGroupingMap.GetValue(value, (int)Grouping.Clustered);
        }

        internal static string GroupingToDml(Grouping value)
        {
            return gBarGroupingMap.GetValue((int)value, "");
        }

        internal static AxisBuiltInUnit DmlToBuiltInUnit(string value)
        {
            return (AxisBuiltInUnit)gBuiltInUnitMap.GetValue(value, (int)AxisBuiltInUnit.Thousands);
        }

        internal static string BuiltInUnitToDml(AxisBuiltInUnit value)
        {
            return gBuiltInUnitMap.GetValue((int)value, "");
        }

        internal static CrossBetween DmlToCrossBetween(string value)
        {
            return (CrossBetween)gCrossBetweenMap.GetValue(value, (int)CrossBetween.Between);
        }

        internal static string CrossBetweenToDml(CrossBetween value)
        {
            return gCrossBetweenMap.GetValue((int)value, "");
        }

        internal static AxisCrosses DmlToCrosses(string value)
        {
            return (AxisCrosses)gCrossesMap.GetValue(value, (int)AxisCrosses.Automatic);
        }

        internal static string CrossesToDml(AxisCrosses value)
        {
            return gCrossesMap.GetValue((int)value, "");
        }

        internal static ChartDataLabelPosition DmlToDataLabelPosition(string value)
        {
            return (ChartDataLabelPosition)gDataLabelPositionMap.GetValue(value, (int)DmlChartDataLabelPr.DefaultPosition);
        }

        internal static string DataLabelPositionToDml(ChartDataLabelPosition value)
        {
            return gDataLabelPositionMap.GetValue((int)value, "");
        }

        internal static DisplayBlanksAs DmlToDisplayBlanksAs(string value)
        {
            return (DisplayBlanksAs)gDisplayBlanksAsMap.GetValue(value, (int)DisplayBlanksAs.Zero);
        }

        internal static string DisplayBlanksAsToDml(DisplayBlanksAs value)
        {
            return gDisplayBlanksAsMap.GetValue((int) value, "");
        }

        internal static ErrorBarDirection DmlToErrorBarDirection(string value)
        {
            return (ErrorBarDirection)gErrorBarDirectionMap.GetValue(value, (int)ErrorBarDirection.X);
        }

        internal static string ErrorBarDirectionToDml(ErrorBarDirection value)
        {
            return gErrorBarDirectionMap.GetValue((int)value, "");
        }

        internal static ErrorBarType DmlToErrorBarType(string value)
        {
            return (ErrorBarType)gErrorBarTypeMap.GetValue(value, (int)ErrorBarType.Both);
        }

        internal static string ErrorBarTypeToDml(ErrorBarType value)
        {
            return gErrorBarTypeMap.GetValue((int)value, "");
        }

        internal static ErrorValueType DmlToErrorValueType(string value)
        {
            return (ErrorValueType)gErrorValueTypeMap.GetValue(value, (int)ErrorValueType.FixedValue);
        }

        internal static string ErrorValueTypeToDml(ErrorValueType value)
        {
            return gErrorValueTypeMap.GetValue((int)value, "");
        }

        internal static LabelAlignment DmlToLabelAlignment(string value)
        {
            return (LabelAlignment)gLabelAlignmentMap.GetValue(value, (int)LabelAlignment.Center);
        }

        internal static string LabelAlignmentToDml(LabelAlignment value)
        {
            return gLabelAlignmentMap.GetValue((int)value, "");
        }

        internal static LayoutMode DmlToLayoutMode(string value)
        {
            return (LayoutMode)gLayoutModeMap.GetValue(value, (int)LayoutMode.Factor);
        }

        internal static string LayoutModeToDml(LayoutMode value)
        {
            return gLayoutModeMap.GetValue((int)value, "");
        }

        internal static LayoutTarget DmlToLayoutTarget(string value)
        {
            return (LayoutTarget)gLayoutTargetMap.GetValue(value, (int)LayoutTarget.Outer);
        }

        internal static string LayoutTargetToDml(LayoutTarget value)
        {
            return gLayoutTargetMap.GetValue((int)value, "");
        }

        internal static LegendPosition DmlToLegendPosition(string value)
        {
            return (LegendPosition)gLegendPositionMap.GetValue(value, (int)ChartLegend.DefaultPosition);
        }

        internal static string LegendPositionToDml(LegendPosition value)
        {
            return gLegendPositionMap.GetValue((int)value, "");
        }

        internal static MarkerSymbol DmlToMarkerStyle(string value)
        {
            return (MarkerSymbol)gMarkerStyleMap.GetValue(value, (int)MarkerSymbol.Default);
        }

        internal static string MarkerStyleToDml(MarkerSymbol value)
        {
            return gMarkerStyleMap.GetValue((int)value, "");
        }

        internal static OfPieType DmlToOfPieType(string value)
        {
            return (OfPieType)gOfPieTypeMap.GetValue(value, (int)OfPieType.Pie);
        }

        internal static string OfPieTypeToDml(OfPieType value)
        {
            return gOfPieTypeMap.GetValue((int)value, "");
        }

        internal static AxisOrientation DmlToAxisOrientation(string value)
        {
            return (AxisOrientation)gOrientationMap.GetValue(value, (int)AxisOrientation.MinMax);
        }

        internal static string AxisOrientationToDml(AxisOrientation value)
        {
            return gOrientationMap.GetValue((int)value, "");
        }

        internal static PageSetupOrientation DmlToPageSetupOrientation(string value)
        {
            return (PageSetupOrientation)gPageSetupOrientationMap.GetValue(value, (int)PageSetupOrientation.Default);
        }

        internal static string PageSetupOrientationToDml(PageSetupOrientation value)
        {
            return gPageSetupOrientationMap.GetValue((int)value, "");
        }

        internal static PictureFormat DmlToPictureFormat(string value)
        {
            return (PictureFormat)gPictureFormatMap.GetValue(value, (int)PictureFormat.Stack);
        }

        internal static string PictureFormatToDml(PictureFormat value)
        {
            return gPictureFormatMap.GetValue((int)value, "");
        }

        internal static RadarStyle DmlToRadarStyle(string value)
        {
            return (RadarStyle)gRadarStyleMap.GetValue(value, (int)RadarStyle.Standard);
        }

        internal static string RadarStyleToDml(RadarStyle value)
        {
            return gRadarStyleMap.GetValue((int)value, "");
        }

        internal static ScatterStyle DmlToScatterStyle(string value)
        {
            return (ScatterStyle)gScatterStyle.GetValue(value, (int)ScatterStyle.Marker);
        }

        internal static string ScatterStyleToDml(ScatterStyle value)
        {
            return gScatterStyle.GetValue((int)value, "");
        }

        internal static BarShape DmlToShape(string value)
        {
            return (BarShape)gShapeMap.GetValue(value, (int)BarShape.Box);
        }

        internal static string ShapeToDml(BarShape value)
        {
            return gShapeMap.GetValue((int)value, "");
        }

        internal static SizeRepresents DmlToSizeRepresents(string value)
        {
            return (SizeRepresents)gSizeRepresentsMap.GetValue(value, (int)SizeRepresents.Area);
        }

        internal static string SizeRepresentsToDml(SizeRepresents value)
        {
            return gSizeRepresentsMap.GetValue((int)value, "");
        }

        internal static SplitType DmlToSplitType(string value)
        {
            return (SplitType)gSplitTypeMap.GetValue(value, (int)SplitType.Auto);
        }

        internal static string SplitTypeToDml(SplitType value)
        {
            return gSplitTypeMap.GetValue((int)value, "");
        }

        internal static AxisTickLabelPosition DmlToTickLabelPosition(string value)
        {
            return (AxisTickLabelPosition)gTickLabelPositionMap.GetValue(value, (int)AxisTickLabelPosition.NextToAxis);
        }

        internal static string TickLabelPositionToDml(AxisTickLabelPosition value)
        {
            return gTickLabelPositionMap.GetValue((int)value, "");
        }

        internal static AxisTickMark DmlToTickMark(string value)
        {
            return (AxisTickMark)gTickMarkMap.GetValue(value, (int)AxisTickMark.None);
        }

        internal static string TickMarkToDml(AxisTickMark value)
        {
            return gTickMarkMap.GetValue((int)value, "");
        }

        internal static AxisTimeUnit DmlToTimeUnit(string value)
        {
            return (AxisTimeUnit)gTimeUnitMap.GetValue(value, (int)AxisTimeUnit.Days);
        }

        internal static string TimeUnitToDml(AxisTimeUnit value)
        {
            return gTimeUnitMap.GetValue((int)value, "");
        }

        internal static TrendlineType DmlToTrendlineType(string value)
        {
            return (TrendlineType)gTrendlineTypeMap.GetValue(value, (int)TrendlineType.Linear);
        }

        internal static string TrendlineTypeToDml(TrendlineType value)
        {
            return gTrendlineTypeMap.GetValue((int)value, "");
        }

        internal static DimensionType DmlToDimensionType(string value)
        {
            return (DimensionType)gDimensionTypeMap.GetValue(value, (int)DimensionType.Value);
        }

        internal static string DimensionTypeToDml(DimensionType value)
        {
            return gDimensionTypeMap.GetValue((int)value, "");
        }

        internal static FormulaDirection DmlToFormulaDirection(string value)
        {
            return (FormulaDirection)gFormulaDirectionMap.GetValue(value, (int)FormulaDirection.Column);
        }

        internal static string FormulaDirectionToDml(FormulaDirection value)
        {
            return gFormulaDirectionMap.GetValue((int)value, "");
        }

        internal static SidePosition DmlToSidePosition(string value, SidePosition defaultValue)
        {
            return (SidePosition)gSidePositionMap.GetValue(value, (int)defaultValue);
        }

        internal static string SidePositionToDml(SidePosition value)
        {
            return gSidePositionMap.GetValue((int)value, "");
        }

        internal static PositionAlignment DmlToPositionAlignment(string value)
        {
            return (PositionAlignment)gPositionAlignmentMap.GetValue(value, (int)PositionAlignment.Center);
        }

        internal static string PositionAlignmentToDml(PositionAlignment value)
        {
            return gPositionAlignmentMap.GetValue((int)value, "");
        }

        internal static SeriesLayout DmlToSeriesLayout(string value)
        {
            return (SeriesLayout)gSeriesLayoutMap.GetValue(value, (int)SeriesLayout.BoxWhisker);
        }

        internal static string SeriesLayoutToDml(SeriesLayout value)
        {
            return gSeriesLayoutMap.GetValue((int)value, "");
        }

        internal static RegionLabelLayout DmlToRegionLabelLayout(string value)
        {
            return (RegionLabelLayout)gRegionLabelLayoutMap.GetValue(value, (int)RegionLabelLayout.None);
        }

        internal static string RegionLabelLayoutToDml(RegionLabelLayout value)
        {
            return gRegionLabelLayoutMap.GetValue((int)value, "");
        }

        internal static ParentLabelLayout DmlToParentLabelLayout(string value)
        {
            return (ParentLabelLayout)gParentLabelLayoutMap.GetValue(value, (int)ParentLabelLayout.None);
        }

        internal static string ParentLabelLayoutToDml(ParentLabelLayout value)
        {
            return gParentLabelLayoutMap.GetValue((int)value, "");
        }

        internal static IntervalClosedSide DmlToIntervalClosedSide(string value)
        {
            return (IntervalClosedSide)gIntervalClosedSideMap.GetValue(value, (int)IntervalClosedSide.Left);
        }

        internal static string IntervalClosedSideToDml(IntervalClosedSide value)
        {
            return gIntervalClosedSideMap.GetValue((int)value, "");
        }

        internal static QuartileMethod DmlToQuartileMethod(string value)
        {
            return (QuartileMethod)gQuartileMethodMap.GetValue(value, (int)QuartileMethod.InclusiveMedian);
        }

        internal static string QuartileMethodToDml(QuartileMethod value)
        {
            return gQuartileMethodMap.GetValue((int)value, "");
        }

        static DmlChartsEnum()
        {
            gAxisPositionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "b", "l", "r", "t" },
                new int[] { (int)AxisPosition.Bottom, (int)AxisPosition.Left, (int)AxisPosition.Right, (int)AxisPosition.Top });

            gBarDirectionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "bar", "col" },
                new int[] { (int)BarDirection.Bar, (int)BarDirection.Column });

            gBarGroupingMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "clustered", "percentStacked", "stacked", "standard" },
                new int[] { (int)Grouping.Clustered, (int)Grouping.PercentStacked, (int)Grouping.Stacked, (int)Grouping.Standard });

            gBuiltInUnitMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "billions", "hundredMillions", "hundreds", "hundredThousands",
                    "millions", "tenMillions", "tenThousands", "thousands", "trillions", "percentage" },
                new int[] { (int)AxisBuiltInUnit.Billions, (int)AxisBuiltInUnit.HundredMillions,
                    (int)AxisBuiltInUnit.Hundreds, (int)AxisBuiltInUnit.HundredThousands,
                    (int)AxisBuiltInUnit.Millions, (int)AxisBuiltInUnit.TenMillions, (int)AxisBuiltInUnit.TenThousands,
                    (int)AxisBuiltInUnit.Thousands, (int)AxisBuiltInUnit.Trillions, (int)AxisBuiltInUnit.Percentage });

            gCrossBetweenMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "between", "midCat" },
                new int[] { (int)CrossBetween.Between, (int)CrossBetween.MidpointOfCategory });

            gCrossesMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "autoZero", "max", "min" },
                new int[] { (int)AxisCrosses.Automatic, (int)AxisCrosses.Maximum, (int)AxisCrosses.Minimum });

            gDataLabelPositionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "b", "bestFit", "ctr", "inBase", "inEnd", "l", "outEnd", "r", "t" },
                new int[] { (int)ChartDataLabelPosition.Below, (int)ChartDataLabelPosition.BestFit,
                    (int)ChartDataLabelPosition.Center, (int)ChartDataLabelPosition.InsideBase,
                    (int)ChartDataLabelPosition.InsideEnd, (int)ChartDataLabelPosition.Left,
                    (int)ChartDataLabelPosition.OutsideEnd, (int)ChartDataLabelPosition.Right,
                    (int)ChartDataLabelPosition.Above});

            gDisplayBlanksAsMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "gap", "span", "zero" },
                new int[] { (int)DisplayBlanksAs.Gap, (int)DisplayBlanksAs.Span, (int)DisplayBlanksAs.Zero });

            gErrorBarDirectionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "x", "y" },
                new int[] { (int)ErrorBarDirection.X, (int)ErrorBarDirection.Y });

            gErrorBarTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "both", "minus", "plus" },
                new int[] { (int)ErrorBarType.Both, (int)ErrorBarType.Minus, (int)ErrorBarType.Plus });

            gErrorValueTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "cust", "fixedVal", "percentage", "stdDev", "stdErr" },
                new int[] { (int)ErrorValueType.CustomErrorBars, (int)ErrorValueType.FixedValue, (int)ErrorValueType.Percentage,
                    (int)ErrorValueType.StandardDeviation, (int)ErrorValueType.StandardError });

            gLabelAlignmentMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "ctr", "l", "r" },
                new int[] { (int)LabelAlignment.Center, (int)LabelAlignment.Left, (int)LabelAlignment.Right });

            gLayoutModeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "edge", "factor" },
                new int[] { (int)LayoutMode.Edge, (int)LayoutMode.Factor });

            gLayoutTargetMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "inner", "outer" },
                new int[] { (int)LayoutTarget.Inner, (int)LayoutTarget.Outer });

            gLegendPositionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "b", "l", "r", "t", "tr" },
                new int[] { (int)LegendPosition.Bottom, (int)LegendPosition.Left, (int)LegendPosition.Right, (int)LegendPosition.Top, (int)LegendPosition.TopRight });

            gMarkerStyleMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "circle", "dash", "diamond", "dot", "none", "picture",
                    "plus", "square", "star", "triangle", "x" },
                new int[] { (int)MarkerSymbol.Circle, (int)MarkerSymbol.Dash, (int)MarkerSymbol.Diamond, (int)MarkerSymbol.Dot, (int)MarkerSymbol.None, (int)MarkerSymbol.Picture,
                    (int)MarkerSymbol.Plus, (int)MarkerSymbol.Square, (int)MarkerSymbol.Star, (int)MarkerSymbol.Triangle, (int)MarkerSymbol.X});

            gOfPieTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "bar", "pie" },
                new int[] { (int)OfPieType.Bar, (int)OfPieType.Pie });

            gOrientationMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "maxMin", "minMax" },
                new int[] { (int)AxisOrientation.MaxMin, (int)AxisOrientation.MinMax });

            gPageSetupOrientationMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "default", "landscape", "portrait" },
                new int[] { (int)PageSetupOrientation.Default, (int)PageSetupOrientation.Landscape, (int)PageSetupOrientation.Portrait });

            gPictureFormatMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "stack", "stackScale", "stretch" },
                new int[] { (int)PictureFormat.Stack, (int)PictureFormat.StackScale, (int)PictureFormat.Stretch });

            gRadarStyleMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "filled", "marker", "standard" },
                new int[] { (int)RadarStyle.Filled, (int)RadarStyle.Marker, (int)RadarStyle.Standard });

            gScatterStyle = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "line", "lineMarker", "marker", "none", "smooth", "smoothMarker" },
                new int[] { (int)ScatterStyle.Line, (int)ScatterStyle.LineMarker, (int)ScatterStyle.Marker, (int)ScatterStyle.None, (int)ScatterStyle.Smooth, (int)ScatterStyle.SmoothMarker });

            gShapeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "box", "cone", "coneToMax", "cylinder", "pyramid", "pyramidToMax" },
                new int[] { (int)BarShape.Box, (int)BarShape.Cone, (int)BarShape.ConeToMax, (int)BarShape.Cylinder, (int)BarShape.Pyramid, (int)BarShape.PyramidToMax });

            gSizeRepresentsMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "area", "w" },
                new int[] { (int)SizeRepresents.Area, (int)SizeRepresents.Width });

            gSplitTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "auto", "cust", "percent", "pos", "val" },
                new int[] { (int)SplitType.Auto, (int)SplitType.Custom, (int)SplitType.Percentage, (int)SplitType.Position, (int)SplitType.Value });

            gTickLabelPositionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "high", "low", "nextTo", "none" },
                new int[] { (int)AxisTickLabelPosition.High, (int)AxisTickLabelPosition.Low,
                    (int)AxisTickLabelPosition.NextToAxis, (int)AxisTickLabelPosition.None });

            gTickMarkMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "cross", "in", "none", "out" },
                new int[] { (int)AxisTickMark.Cross, (int)AxisTickMark.Inside, (int)AxisTickMark.None,
                    (int)AxisTickMark.Outside });

            gTimeUnitMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "days", "months", "years" },
                new int[] { (int)AxisTimeUnit.Days, (int)AxisTimeUnit.Months, (int)AxisTimeUnit.Years });

            gTrendlineTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "exp", "linear", "log", "movingAvg", "poly", "power" },
                new int[] { (int)TrendlineType.Exponential, (int)TrendlineType.Linear, (int)TrendlineType.Logarithmic, (int)TrendlineType.MovingAverage,
                    (int)TrendlineType.Polynomial, (int)TrendlineType.Power});

            gDimensionTypeMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "val", "x", "y", "size", "cat", "colorVal" },
                new int[] { (int)DimensionType.Value, (int)DimensionType.X, (int)DimensionType.Y,
                    (int)DimensionType.Size, (int)DimensionType.Category, (int)DimensionType.ColorValue });

            gFormulaDirectionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "col", "row" },
                new int[] { (int)FormulaDirection.Column, (int)FormulaDirection.Row });

            gSidePositionMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "l", "t", "r", "b" },
                new int[] { (int)SidePosition.Left, (int)SidePosition.Top, (int)SidePosition.Right,
                    (int)SidePosition.Bottom });

            gPositionAlignmentMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "min", "ctr", "max" },
                new int[] { (int)PositionAlignment.Minimum, (int)PositionAlignment.Center,
                    (int)PositionAlignment.Maximum });

            gSeriesLayoutMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "boxWhisker", "clusteredColumn", "funnel", "paretoLine", "regionMap", "sunburst",
                    "treemap", "waterfall" },
                new int[] { (int)SeriesLayout.BoxWhisker, (int)SeriesLayout.ClusteredColumn, (int)SeriesLayout.Funnel,
                    (int)SeriesLayout.ParetoLine, (int)SeriesLayout.RegionMap, (int)SeriesLayout.Sunburst,
                    (int)SeriesLayout.Treemap, (int)SeriesLayout.Waterfall });

            gRegionLabelLayoutMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "none", "bestFitOnly", "showAll" },
                new int[] { (int)RegionLabelLayout.None, (int)RegionLabelLayout.BestFitOnly,
                    (int)RegionLabelLayout.ShowAll });

            gParentLabelLayoutMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "none", "banner", "overlapping" },
                new int[] { (int)ParentLabelLayout.None, (int)ParentLabelLayout.Banner,
                    (int)ParentLabelLayout.Overlapping });

            gIntervalClosedSideMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "l", "r" },
                new int[] { (int)IntervalClosedSide.Left, (int)IntervalClosedSide.Right });

            gQuartileMethodMap = DmlEnumUtil.InitHashtableWithValues(
                new string[] { "inclusive", "exclusive" },
                new int[] { (int)QuartileMethod.InclusiveMedian, (int)QuartileMethod.ExclusiveMedian });
        }

        /// <summary>
        /// Hash tables where key is string and value is the corresponding enum value.
        /// </summary>
        private static readonly StringToIntBidirectionalMap gAxisPositionMap;
        private static readonly StringToIntBidirectionalMap gBarDirectionMap;
        private static readonly StringToIntBidirectionalMap gBarGroupingMap;
        private static readonly StringToIntBidirectionalMap gBuiltInUnitMap;
        private static readonly StringToIntBidirectionalMap gCrossBetweenMap;
        private static readonly StringToIntBidirectionalMap gCrossesMap;
        private static readonly StringToIntBidirectionalMap gDataLabelPositionMap;
        private static readonly StringToIntBidirectionalMap gDisplayBlanksAsMap;
        private static readonly StringToIntBidirectionalMap gErrorBarDirectionMap;
        private static readonly StringToIntBidirectionalMap gErrorBarTypeMap;
        private static readonly StringToIntBidirectionalMap gErrorValueTypeMap;
        private static readonly StringToIntBidirectionalMap gLabelAlignmentMap;
        private static readonly StringToIntBidirectionalMap gLayoutModeMap;
        private static readonly StringToIntBidirectionalMap gLayoutTargetMap;
        private static readonly StringToIntBidirectionalMap gLegendPositionMap;
        private static readonly StringToIntBidirectionalMap gMarkerStyleMap;
        private static readonly StringToIntBidirectionalMap gOfPieTypeMap;
        private static readonly StringToIntBidirectionalMap gOrientationMap;
        private static readonly StringToIntBidirectionalMap gPageSetupOrientationMap;
        private static readonly StringToIntBidirectionalMap gPictureFormatMap;
        private static readonly StringToIntBidirectionalMap gRadarStyleMap;
        private static readonly StringToIntBidirectionalMap gScatterStyle;
        private static readonly StringToIntBidirectionalMap gShapeMap;
        private static readonly StringToIntBidirectionalMap gSizeRepresentsMap;
        private static readonly StringToIntBidirectionalMap gSplitTypeMap;
        private static readonly StringToIntBidirectionalMap gTickLabelPositionMap;
        private static readonly StringToIntBidirectionalMap gTickMarkMap;
        private static readonly StringToIntBidirectionalMap gTimeUnitMap;
        private static readonly StringToIntBidirectionalMap gTrendlineTypeMap;
        private static readonly StringToIntBidirectionalMap gDimensionTypeMap;
        private static readonly StringToIntBidirectionalMap gFormulaDirectionMap;
        private static readonly StringToIntBidirectionalMap gSidePositionMap;
        private static readonly StringToIntBidirectionalMap gPositionAlignmentMap;
        private static readonly StringToIntBidirectionalMap gSeriesLayoutMap;
        private static readonly StringToIntBidirectionalMap gRegionLabelLayoutMap;
        private static readonly StringToIntBidirectionalMap gParentLabelLayoutMap;
        private static readonly StringToIntBidirectionalMap gIntervalClosedSideMap;
        private static readonly StringToIntBidirectionalMap gQuartileMethodMap;
    }
}
