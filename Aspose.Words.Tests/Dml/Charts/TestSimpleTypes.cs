// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.RW.Dml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Test <see cref="DmlChartsEnum"/>.
    /// </summary>
    [TestFixture]
    public class TestSimpleTypes
    {
        [Test]
        public void TestToAxisPosition()
        {
            Assert.That(DmlChartsEnum.DmlToAxisPosition("b"), Is.EqualTo(AxisPosition.Bottom));
            Assert.That(DmlChartsEnum.DmlToAxisPosition("l"), Is.EqualTo(AxisPosition.Left));
            Assert.That(DmlChartsEnum.DmlToAxisPosition("r"), Is.EqualTo(AxisPosition.Right));
            Assert.That(DmlChartsEnum.DmlToAxisPosition("t"), Is.EqualTo(AxisPosition.Top));
        }

        [Test]
        public void TestToBarDirection()
        {
            Assert.That(DmlChartsEnum.DmlToBarDirection("bar"), Is.EqualTo(BarDirection.Bar));
            Assert.That(DmlChartsEnum.DmlToBarDirection("col"), Is.EqualTo(BarDirection.Column));
        }

        [Test]
        public void TestToBarGrouping()
        {
            Assert.That(DmlChartsEnum.DmlToGrouping("clustered"), Is.EqualTo(Grouping.Clustered));
            Assert.That(DmlChartsEnum.DmlToGrouping("percentStacked"), Is.EqualTo(Grouping.PercentStacked));
            Assert.That(DmlChartsEnum.DmlToGrouping("stacked"), Is.EqualTo(Grouping.Stacked));
            Assert.That(DmlChartsEnum.DmlToGrouping("standard"), Is.EqualTo(Grouping.Standard));
        }

        [Test]
        public void TestToBuiltInUnit()
        {
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("billions"), Is.EqualTo(AxisBuiltInUnit.Billions));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("hundredMillions"), Is.EqualTo(AxisBuiltInUnit.HundredMillions));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("hundreds"), Is.EqualTo(AxisBuiltInUnit.Hundreds));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("hundredThousands"), Is.EqualTo(AxisBuiltInUnit.HundredThousands));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("millions"), Is.EqualTo(AxisBuiltInUnit.Millions));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("tenMillions"), Is.EqualTo(AxisBuiltInUnit.TenMillions));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("tenThousands"), Is.EqualTo(AxisBuiltInUnit.TenThousands));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("thousands"), Is.EqualTo(AxisBuiltInUnit.Thousands));
            Assert.That(DmlChartsEnum.DmlToBuiltInUnit("trillions"), Is.EqualTo(AxisBuiltInUnit.Trillions));
        }

        [Test]
        public void TestToCrossBetween()
        {
            Assert.That(DmlChartsEnum.DmlToCrossBetween("between"), Is.EqualTo(CrossBetween.Between));
            Assert.That(DmlChartsEnum.DmlToCrossBetween("midCat"), Is.EqualTo(CrossBetween.MidpointOfCategory));
        }

        [Test]
        public void TestToCrosses()
        {
            Assert.That(DmlChartsEnum.DmlToCrosses("autoZero"), Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(DmlChartsEnum.DmlToCrosses("max"), Is.EqualTo(AxisCrosses.Maximum));
            Assert.That(DmlChartsEnum.DmlToCrosses("min"), Is.EqualTo(AxisCrosses.Minimum));
        }

        [Test]
        public void TestToDataLabelPosition()
        {
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("b"), Is.EqualTo(ChartDataLabelPosition.Below));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("bestFit"), Is.EqualTo(ChartDataLabelPosition.BestFit));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("ctr"), Is.EqualTo(ChartDataLabelPosition.Center));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("inBase"), Is.EqualTo(ChartDataLabelPosition.InsideBase));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("inEnd"), Is.EqualTo(ChartDataLabelPosition.InsideEnd));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("l"), Is.EqualTo(ChartDataLabelPosition.Left));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("outEnd"), Is.EqualTo(ChartDataLabelPosition.OutsideEnd));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("r"), Is.EqualTo(ChartDataLabelPosition.Right));
            Assert.That(DmlChartsEnum.DmlToDataLabelPosition("t"), Is.EqualTo(ChartDataLabelPosition.Above));
        }

        [Test]
        public void TestToDisplayBlanksAs()
        {
            Assert.That(DmlChartsEnum.DmlToDisplayBlanksAs("gap"), Is.EqualTo(DisplayBlanksAs.Gap));
            Assert.That(DmlChartsEnum.DmlToDisplayBlanksAs("span"), Is.EqualTo(DisplayBlanksAs.Span));
            Assert.That(DmlChartsEnum.DmlToDisplayBlanksAs("zero"), Is.EqualTo(DisplayBlanksAs.Zero));
        }

        [Test]
        public void TestToErrorBarDirection()
        {
            Assert.That(DmlChartsEnum.DmlToErrorBarDirection("x"), Is.EqualTo(ErrorBarDirection.X));
            Assert.That(DmlChartsEnum.DmlToErrorBarDirection("y"), Is.EqualTo(ErrorBarDirection.Y));
        }

        [Test]
        public void TestToErrorBarType()
        {
            Assert.That(DmlChartsEnum.DmlToErrorBarType("both"), Is.EqualTo(ErrorBarType.Both));
            Assert.That(DmlChartsEnum.DmlToErrorBarType("minus"), Is.EqualTo(ErrorBarType.Minus));
            Assert.That(DmlChartsEnum.DmlToErrorBarType("plus"), Is.EqualTo(ErrorBarType.Plus));
        }

        [Test]
        public void TestToErrorValueType()
        {
            Assert.That(DmlChartsEnum.DmlToErrorValueType("cust"), Is.EqualTo(ErrorValueType.CustomErrorBars));
            Assert.That(DmlChartsEnum.DmlToErrorValueType("fixedVal"), Is.EqualTo(ErrorValueType.FixedValue));
            Assert.That(DmlChartsEnum.DmlToErrorValueType("percentage"), Is.EqualTo(ErrorValueType.Percentage));
            Assert.That(DmlChartsEnum.DmlToErrorValueType("stdDev"), Is.EqualTo(ErrorValueType.StandardDeviation));
            Assert.That(DmlChartsEnum.DmlToErrorValueType("stdErr"), Is.EqualTo(ErrorValueType.StandardError));
        }

        [Test]
        public void TestToLabelAlignment()
        {
            Assert.That(DmlChartsEnum.DmlToLabelAlignment("ctr"), Is.EqualTo(LabelAlignment.Center));
            Assert.That(DmlChartsEnum.DmlToLabelAlignment("l"), Is.EqualTo(LabelAlignment.Left));
            Assert.That(DmlChartsEnum.DmlToLabelAlignment("r"), Is.EqualTo(LabelAlignment.Right));
        }

        [Test]
        public void TestToLayoutMode()
        {
            Assert.That(DmlChartsEnum.DmlToLayoutMode("edge"), Is.EqualTo(LayoutMode.Edge));
            Assert.That(DmlChartsEnum.DmlToLayoutMode("factor"), Is.EqualTo(LayoutMode.Factor));
        }

        [Test]
        public void TestToLayoutTarget()
        {
            Assert.That(DmlChartsEnum.DmlToLayoutTarget("inner"), Is.EqualTo(LayoutTarget.Inner));
            Assert.That(DmlChartsEnum.DmlToLayoutTarget("outer"), Is.EqualTo(LayoutTarget.Outer));
        }

        [Test]
        public void TestToLegendPosition()
        {
            Assert.That(DmlChartsEnum.DmlToLegendPosition("b"), Is.EqualTo(LegendPosition.Bottom));
            Assert.That(DmlChartsEnum.DmlToLegendPosition("l"), Is.EqualTo(LegendPosition.Left));
            Assert.That(DmlChartsEnum.DmlToLegendPosition("r"), Is.EqualTo(LegendPosition.Right));
            Assert.That(DmlChartsEnum.DmlToLegendPosition("t"), Is.EqualTo(LegendPosition.Top));
            Assert.That(DmlChartsEnum.DmlToLegendPosition("tr"), Is.EqualTo(LegendPosition.TopRight));
        }

        [Test]
        public void TestToMarkerStyle()
        {
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("circle"), Is.EqualTo(MarkerSymbol.Circle));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("dash"), Is.EqualTo(MarkerSymbol.Dash));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("diamond"), Is.EqualTo(MarkerSymbol.Diamond));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("dot"), Is.EqualTo(MarkerSymbol.Dot));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("none"), Is.EqualTo(MarkerSymbol.None));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("picture"), Is.EqualTo(MarkerSymbol.Picture));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("plus"), Is.EqualTo(MarkerSymbol.Plus));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("square"), Is.EqualTo(MarkerSymbol.Square));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("star"), Is.EqualTo(MarkerSymbol.Star));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("triangle"), Is.EqualTo(MarkerSymbol.Triangle));
            Assert.That(DmlChartsEnum.DmlToMarkerStyle("x"), Is.EqualTo(MarkerSymbol.X));
        }

        [Test]
        public void TestToOfPieType()
        {
            Assert.That(DmlChartsEnum.DmlToOfPieType("bar"), Is.EqualTo(OfPieType.Bar));
            Assert.That(DmlChartsEnum.DmlToOfPieType("pie"), Is.EqualTo(OfPieType.Pie));
        }

        [Test]
        public void TestToOrientation()
        {
            Assert.That(DmlChartsEnum.DmlToAxisOrientation("maxMin"), Is.EqualTo(AxisOrientation.MaxMin));
            Assert.That(DmlChartsEnum.DmlToAxisOrientation("minMax"), Is.EqualTo(AxisOrientation.MinMax));
        }

        [Test]
        public void TestToPageSetupOrientation()
        {
            Assert.That(DmlChartsEnum.DmlToPageSetupOrientation("default"), Is.EqualTo(PageSetupOrientation.Default));
            Assert.That(DmlChartsEnum.DmlToPageSetupOrientation("landscape"), Is.EqualTo(PageSetupOrientation.Landscape));
            Assert.That(DmlChartsEnum.DmlToPageSetupOrientation("portrait"), Is.EqualTo(PageSetupOrientation.Portrait));
        }

        [Test]
        public void TestToPictureFormat()
        {
            Assert.That(DmlChartsEnum.DmlToPictureFormat("stack"), Is.EqualTo(PictureFormat.Stack));
            Assert.That(DmlChartsEnum.DmlToPictureFormat("stackScale"), Is.EqualTo(PictureFormat.StackScale));
            Assert.That(DmlChartsEnum.DmlToPictureFormat("stretch"), Is.EqualTo(PictureFormat.Stretch));
        }

        [Test]
        public void TestToRadarStyle()
        {
            Assert.That(DmlChartsEnum.DmlToRadarStyle("filled"), Is.EqualTo(RadarStyle.Filled));
            Assert.That(DmlChartsEnum.DmlToRadarStyle("marker"), Is.EqualTo(RadarStyle.Marker));
            Assert.That(DmlChartsEnum.DmlToRadarStyle("standard"), Is.EqualTo(RadarStyle.Standard));
        }

        [Test]
        public void TestToScatterStyle()
        {
            Assert.That(DmlChartsEnum.DmlToScatterStyle("line"), Is.EqualTo(ScatterStyle.Line));
            Assert.That(DmlChartsEnum.DmlToScatterStyle("lineMarker"), Is.EqualTo(ScatterStyle.LineMarker));
            Assert.That(DmlChartsEnum.DmlToScatterStyle("marker"), Is.EqualTo(ScatterStyle.Marker));
            Assert.That(DmlChartsEnum.DmlToScatterStyle("none"), Is.EqualTo(ScatterStyle.None));
            Assert.That(DmlChartsEnum.DmlToScatterStyle("smooth"), Is.EqualTo(ScatterStyle.Smooth));
            Assert.That(DmlChartsEnum.DmlToScatterStyle("smoothMarker"), Is.EqualTo(ScatterStyle.SmoothMarker));
        }

        [Test]
        public void TestToShape()
        {
            Assert.That(DmlChartsEnum.DmlToShape("box"), Is.EqualTo(BarShape.Box));
            Assert.That(DmlChartsEnum.DmlToShape("cone"), Is.EqualTo(BarShape.Cone));
            Assert.That(DmlChartsEnum.DmlToShape("coneToMax"), Is.EqualTo(BarShape.ConeToMax));
            Assert.That(DmlChartsEnum.DmlToShape("cylinder"), Is.EqualTo(BarShape.Cylinder));
            Assert.That(DmlChartsEnum.DmlToShape("pyramid"), Is.EqualTo(BarShape.Pyramid));
            Assert.That(DmlChartsEnum.DmlToShape("pyramidToMax"), Is.EqualTo(BarShape.PyramidToMax));
        }

        [Test]
        public void TestToSizeRepresents()
        {
            Assert.That(DmlChartsEnum.DmlToSizeRepresents("area"), Is.EqualTo(SizeRepresents.Area));
            Assert.That(DmlChartsEnum.DmlToSizeRepresents("w"), Is.EqualTo(SizeRepresents.Width));
        }

        [Test]
        public void TestToSplitType()
        {
            Assert.That(DmlChartsEnum.DmlToSplitType("auto"), Is.EqualTo(SplitType.Auto));
            Assert.That(DmlChartsEnum.DmlToSplitType("cust"), Is.EqualTo(SplitType.Custom));
            Assert.That(DmlChartsEnum.DmlToSplitType("percent"), Is.EqualTo(SplitType.Percentage));
            Assert.That(DmlChartsEnum.DmlToSplitType("pos"), Is.EqualTo(SplitType.Position));
            Assert.That(DmlChartsEnum.DmlToSplitType("val"), Is.EqualTo(SplitType.Value));
        }

        [Test]
        public void TestToTickLabelPosition()
        {
            Assert.That(DmlChartsEnum.DmlToTickLabelPosition("high"), Is.EqualTo(AxisTickLabelPosition.High));
            Assert.That(DmlChartsEnum.DmlToTickLabelPosition("low"), Is.EqualTo(AxisTickLabelPosition.Low));
            Assert.That(DmlChartsEnum.DmlToTickLabelPosition("nextTo"), Is.EqualTo(AxisTickLabelPosition.NextToAxis));
            Assert.That(DmlChartsEnum.DmlToTickLabelPosition("none"), Is.EqualTo(AxisTickLabelPosition.None));
        }

        [Test]
        public void TestToTickMark()
        {
            Assert.That(DmlChartsEnum.DmlToTickMark("cross"), Is.EqualTo(AxisTickMark.Cross));
            Assert.That(DmlChartsEnum.DmlToTickMark("in"), Is.EqualTo(AxisTickMark.Inside));
            Assert.That(DmlChartsEnum.DmlToTickMark("none"), Is.EqualTo(AxisTickMark.None));
            Assert.That(DmlChartsEnum.DmlToTickMark("out"), Is.EqualTo(AxisTickMark.Outside));
        }

        [Test]
        public void TestToTimeUnit()
        {
            Assert.That(DmlChartsEnum.DmlToTimeUnit("days"), Is.EqualTo(AxisTimeUnit.Days));
            Assert.That(DmlChartsEnum.DmlToTimeUnit("months"), Is.EqualTo(AxisTimeUnit.Months));
            Assert.That(DmlChartsEnum.DmlToTimeUnit("years"), Is.EqualTo(AxisTimeUnit.Years));
        }

        [Test]
        public void TestToTrendlineType()
        {
            Assert.That(DmlChartsEnum.DmlToTrendlineType("exp"), Is.EqualTo(TrendlineType.Exponential));
            Assert.That(DmlChartsEnum.DmlToTrendlineType("linear"), Is.EqualTo(TrendlineType.Linear));
            Assert.That(DmlChartsEnum.DmlToTrendlineType("log"), Is.EqualTo(TrendlineType.Logarithmic));
            Assert.That(DmlChartsEnum.DmlToTrendlineType("movingAvg"), Is.EqualTo(TrendlineType.MovingAverage));
            Assert.That(DmlChartsEnum.DmlToTrendlineType("poly"), Is.EqualTo(TrendlineType.Polynomial));
            Assert.That(DmlChartsEnum.DmlToTrendlineType("power"), Is.EqualTo(TrendlineType.Power));
        }
    }
}