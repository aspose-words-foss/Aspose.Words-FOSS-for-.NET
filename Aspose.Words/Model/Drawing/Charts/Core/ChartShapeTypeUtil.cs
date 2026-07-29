// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/04/2023 by Alexander Zhiltsov

using Aspose.Collections;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Contains methods for a bidirectional conversion from a preset geometry string to <see cref="ChartShapeType"/>.
    /// </summary>
    internal static class ChartShapeTypeUtil
    {
        /// <summary>
        /// Gets a <see cref="ChartShapeType"/> value for the specified preset geometry string.
        /// </summary>
        internal static ChartShapeType PresetGeometryToChartShapeType(string presetName)
        {
            return (ChartShapeType)gPresetToChartShapeTypeMap.GetValue(presetName, (int)ChartShapeType.Default);
        }

        /// <summary>
        /// Gets a preset geometry string for the specified <see cref="ChartShapeType"/> value.
        /// </summary>
        internal static string ChartShapeTypeToDmlPresetGeom(ChartShapeType value)
        {
            return gPresetToChartShapeTypeMap.GetValue((int)value, "");
        }

        /// <summary>
        /// Static ctor to generate a map between preset geometry strings and <see cref="ChartShapeType"/> values.
        /// </summary>
        static ChartShapeTypeUtil()
        {
            gPresetToChartShapeTypeMap.AddEntry("accentBorderCallout1", (int)ChartShapeType.AccentBorderCallout1);
            gPresetToChartShapeTypeMap.AddEntry("accentBorderCallout2", (int)ChartShapeType.AccentBorderCallout2);
            gPresetToChartShapeTypeMap.AddEntry("accentBorderCallout3", (int)ChartShapeType.AccentBorderCallout3);
            gPresetToChartShapeTypeMap.AddEntry("accentCallout1", (int)ChartShapeType.AccentCallout1);
            gPresetToChartShapeTypeMap.AddEntry("accentCallout2", (int)ChartShapeType.AccentCallout2);
            gPresetToChartShapeTypeMap.AddEntry("accentCallout3", (int)ChartShapeType.AccentCallout3);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonBackPrevious", (int)ChartShapeType.ActionButtonBackPrevious);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonBeginning", (int)ChartShapeType.ActionButtonBeginning);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonBlank", (int)ChartShapeType.ActionButtonBlank);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonDocument", (int)ChartShapeType.ActionButtonDocument);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonEnd", (int)ChartShapeType.ActionButtonEnd);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonForwardNext", (int)ChartShapeType.ActionButtonForwardNext);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonHelp", (int)ChartShapeType.ActionButtonHelp);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonHome", (int)ChartShapeType.ActionButtonHome);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonInformation", (int)ChartShapeType.ActionButtonInformation);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonMovie", (int)ChartShapeType.ActionButtonMovie);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonReturn", (int)ChartShapeType.ActionButtonReturn);
            gPresetToChartShapeTypeMap.AddEntry("actionButtonSound", (int)ChartShapeType.ActionButtonSound);
            gPresetToChartShapeTypeMap.AddEntry("bentConnector2", (int)ChartShapeType.BentConnector2);
            gPresetToChartShapeTypeMap.AddEntry("bentConnector3", (int)ChartShapeType.BentConnector3);
            gPresetToChartShapeTypeMap.AddEntry("bentConnector4", (int)ChartShapeType.BentConnector4);
            gPresetToChartShapeTypeMap.AddEntry("bentConnector5", (int)ChartShapeType.BentConnector5);
            gPresetToChartShapeTypeMap.AddEntry("bevel", (int)ChartShapeType.Bevel);
            gPresetToChartShapeTypeMap.AddEntry("borderCallout1", (int)ChartShapeType.BorderCallout1);
            gPresetToChartShapeTypeMap.AddEntry("borderCallout2", (int)ChartShapeType.BorderCallout2);
            gPresetToChartShapeTypeMap.AddEntry("borderCallout3", (int)ChartShapeType.BorderCallout3);
            gPresetToChartShapeTypeMap.AddEntry("bracePair", (int)ChartShapeType.BracePair);
            gPresetToChartShapeTypeMap.AddEntry("bracketPair", (int)ChartShapeType.BracketPair);
            gPresetToChartShapeTypeMap.AddEntry("callout1", (int)ChartShapeType.Callout1);
            gPresetToChartShapeTypeMap.AddEntry("callout2", (int)ChartShapeType.Callout2);
            gPresetToChartShapeTypeMap.AddEntry("callout3", (int)ChartShapeType.Callout3);
            gPresetToChartShapeTypeMap.AddEntry("can", (int)ChartShapeType.Can);
            gPresetToChartShapeTypeMap.AddEntry("chevron", (int)ChartShapeType.Chevron);
            gPresetToChartShapeTypeMap.AddEntry("cloudCallout", (int)ChartShapeType.CloudCallout);
            gPresetToChartShapeTypeMap.AddEntry("cube", (int)ChartShapeType.Cube);
            gPresetToChartShapeTypeMap.AddEntry("curvedConnector2", (int)ChartShapeType.CurvedConnector2);
            gPresetToChartShapeTypeMap.AddEntry("curvedConnector3", (int)ChartShapeType.CurvedConnector3);
            gPresetToChartShapeTypeMap.AddEntry("curvedConnector4", (int)ChartShapeType.CurvedConnector4);
            gPresetToChartShapeTypeMap.AddEntry("curvedConnector5", (int)ChartShapeType.CurvedConnector5);
            gPresetToChartShapeTypeMap.AddEntry("curvedDownArrow", (int)ChartShapeType.CurvedDownArrow);
            gPresetToChartShapeTypeMap.AddEntry("curvedLeftArrow", (int)ChartShapeType.CurvedLeftArrow);
            gPresetToChartShapeTypeMap.AddEntry("curvedRightArrow", (int)ChartShapeType.CurvedRightArrow);
            gPresetToChartShapeTypeMap.AddEntry("curvedUpArrow", (int)ChartShapeType.CurvedUpArrow);
            gPresetToChartShapeTypeMap.AddEntry("diamond", (int)ChartShapeType.Diamond);
            gPresetToChartShapeTypeMap.AddEntry("donut", (int)ChartShapeType.Donut);
            gPresetToChartShapeTypeMap.AddEntry("doubleWave", (int)ChartShapeType.DoubleWave);
            gPresetToChartShapeTypeMap.AddEntry("downArrow", (int)ChartShapeType.DownArrow);
            gPresetToChartShapeTypeMap.AddEntry("downArrowCallout", (int)ChartShapeType.DownArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("ellipse", (int)ChartShapeType.Ellipse);
            gPresetToChartShapeTypeMap.AddEntry("ellipseRibbon", (int)ChartShapeType.EllipseRibbon);
            gPresetToChartShapeTypeMap.AddEntry("ellipseRibbon2", (int)ChartShapeType.EllipseRibbon2);
            gPresetToChartShapeTypeMap.AddEntry("flowChartAlternateProcess", (int)ChartShapeType.FlowChartAlternateProcess);
            gPresetToChartShapeTypeMap.AddEntry("flowChartCollate", (int)ChartShapeType.FlowChartCollate);
            gPresetToChartShapeTypeMap.AddEntry("flowChartConnector", (int)ChartShapeType.FlowChartConnector);
            gPresetToChartShapeTypeMap.AddEntry("flowChartDecision", (int)ChartShapeType.FlowChartDecision);
            gPresetToChartShapeTypeMap.AddEntry("flowChartDelay", (int)ChartShapeType.FlowChartDelay);
            gPresetToChartShapeTypeMap.AddEntry("flowChartDisplay", (int)ChartShapeType.FlowChartDisplay);
            gPresetToChartShapeTypeMap.AddEntry("flowChartDocument", (int)ChartShapeType.FlowChartDocument);
            gPresetToChartShapeTypeMap.AddEntry("flowChartExtract", (int)ChartShapeType.FlowChartExtract);
            gPresetToChartShapeTypeMap.AddEntry("flowChartInputOutput", (int)ChartShapeType.FlowChartInputOutput);
            gPresetToChartShapeTypeMap.AddEntry("flowChartInternalStorage", (int)ChartShapeType.FlowChartInternalStorage);
            gPresetToChartShapeTypeMap.AddEntry("flowChartMagneticDisk", (int)ChartShapeType.FlowChartMagneticDisk);
            gPresetToChartShapeTypeMap.AddEntry("flowChartMagneticDrum", (int)ChartShapeType.FlowChartMagneticDrum);
            gPresetToChartShapeTypeMap.AddEntry("flowChartMagneticTape", (int)ChartShapeType.FlowChartMagneticTape);
            gPresetToChartShapeTypeMap.AddEntry("flowChartManualInput", (int)ChartShapeType.FlowChartManualInput);
            gPresetToChartShapeTypeMap.AddEntry("flowChartManualOperation", (int)ChartShapeType.FlowChartManualOperation);
            gPresetToChartShapeTypeMap.AddEntry("flowChartMerge", (int)ChartShapeType.FlowChartMerge);
            gPresetToChartShapeTypeMap.AddEntry("flowChartMultidocument", (int)ChartShapeType.FlowChartMultidocument);
            gPresetToChartShapeTypeMap.AddEntry("flowChartOfflineStorage", (int)ChartShapeType.FlowChartOfflineStorage);
            gPresetToChartShapeTypeMap.AddEntry("flowChartOffpageConnector", (int)ChartShapeType.FlowChartOffpageConnector);
            gPresetToChartShapeTypeMap.AddEntry("flowChartOnlineStorage", (int)ChartShapeType.FlowChartOnlineStorage);
            gPresetToChartShapeTypeMap.AddEntry("flowChartOr", (int)ChartShapeType.FlowChartOr);
            gPresetToChartShapeTypeMap.AddEntry("flowChartPredefinedProcess",
                (int)ChartShapeType.FlowChartPredefinedProcess);
            gPresetToChartShapeTypeMap.AddEntry("flowChartPreparation", (int)ChartShapeType.FlowChartPreparation);
            gPresetToChartShapeTypeMap.AddEntry("flowChartProcess", (int)ChartShapeType.FlowChartProcess);
            gPresetToChartShapeTypeMap.AddEntry("flowChartPunchedCard", (int)ChartShapeType.FlowChartPunchedCard);
            gPresetToChartShapeTypeMap.AddEntry("flowChartPunchedTape", (int)ChartShapeType.FlowChartPunchedTape);
            gPresetToChartShapeTypeMap.AddEntry("flowChartSort", (int)ChartShapeType.FlowChartSort);
            gPresetToChartShapeTypeMap.AddEntry("flowChartSummingJunction", (int)ChartShapeType.FlowChartSummingJunction);
            gPresetToChartShapeTypeMap.AddEntry("flowChartTerminator", (int)ChartShapeType.FlowChartTerminator);
            gPresetToChartShapeTypeMap.AddEntry("foldedCorner", (int)ChartShapeType.FoldedCorner);
            gPresetToChartShapeTypeMap.AddEntry("hexagon", (int)ChartShapeType.Hexagon);
            gPresetToChartShapeTypeMap.AddEntry("homePlate", (int)ChartShapeType.HomePlate);
            gPresetToChartShapeTypeMap.AddEntry("horizontalScroll", (int)ChartShapeType.HorizontalScroll);
            gPresetToChartShapeTypeMap.AddEntry("irregularSeal1", (int)ChartShapeType.IrregularSeal1);
            gPresetToChartShapeTypeMap.AddEntry("irregularSeal2", (int)ChartShapeType.IrregularSeal2);
            gPresetToChartShapeTypeMap.AddEntry("leftArrow", (int)ChartShapeType.LeftArrow);
            gPresetToChartShapeTypeMap.AddEntry("leftArrowCallout", (int)ChartShapeType.LeftArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("leftBrace", (int)ChartShapeType.LeftBrace);
            gPresetToChartShapeTypeMap.AddEntry("leftBracket", (int)ChartShapeType.LeftBracket);
            gPresetToChartShapeTypeMap.AddEntry("leftRightArrow", (int)ChartShapeType.LeftRightArrow);
            gPresetToChartShapeTypeMap.AddEntry("leftRightArrowCallout", (int)ChartShapeType.LeftRightArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("lightningBolt", (int)ChartShapeType.LightningBolt);
            gPresetToChartShapeTypeMap.AddEntry("line", (int)ChartShapeType.Line);
            gPresetToChartShapeTypeMap.AddEntry("moon", (int)ChartShapeType.Moon);
            gPresetToChartShapeTypeMap.AddEntry("noSmoking", (int)ChartShapeType.NoSmoking);
            gPresetToChartShapeTypeMap.AddEntry("notchedRightArrow", (int)ChartShapeType.NotchedRightArrow);
            gPresetToChartShapeTypeMap.AddEntry("octagon", (int)ChartShapeType.Octagon);
            gPresetToChartShapeTypeMap.AddEntry("parallelogram", (int)ChartShapeType.Parallelogram);
            gPresetToChartShapeTypeMap.AddEntry("pentagon", (int)ChartShapeType.Pentagon);
            gPresetToChartShapeTypeMap.AddEntry("plaque", (int)ChartShapeType.Plaque);
            gPresetToChartShapeTypeMap.AddEntry("plus", (int)ChartShapeType.Plus);
            gPresetToChartShapeTypeMap.AddEntry("rect", (int)ChartShapeType.Rectangle);
            gPresetToChartShapeTypeMap.AddEntry("ribbon", (int)ChartShapeType.Ribbon);
            gPresetToChartShapeTypeMap.AddEntry("ribbon2", (int)ChartShapeType.Ribbon2);
            gPresetToChartShapeTypeMap.AddEntry("rightArrow", (int)ChartShapeType.Arrow);
            gPresetToChartShapeTypeMap.AddEntry("rightArrowCallout", (int)ChartShapeType.RightArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("rightBrace", (int)ChartShapeType.RightBrace);
            gPresetToChartShapeTypeMap.AddEntry("rightBracket", (int)ChartShapeType.RightBracket);
            gPresetToChartShapeTypeMap.AddEntry("smileyFace", (int)ChartShapeType.SmileyFace);
            gPresetToChartShapeTypeMap.AddEntry("star16", (int)ChartShapeType.Seal16);
            gPresetToChartShapeTypeMap.AddEntry("star32", (int)ChartShapeType.Seal32);
            gPresetToChartShapeTypeMap.AddEntry("star4", (int)ChartShapeType.Seal4);
            gPresetToChartShapeTypeMap.AddEntry("star8", (int)ChartShapeType.Seal8);
            gPresetToChartShapeTypeMap.AddEntry("straightConnector1", (int)ChartShapeType.StraightConnector1);
            gPresetToChartShapeTypeMap.AddEntry("stripedRightArrow", (int)ChartShapeType.StripedRightArrow);
            gPresetToChartShapeTypeMap.AddEntry("sun", (int)ChartShapeType.Sun);
            gPresetToChartShapeTypeMap.AddEntry("triangle", (int)ChartShapeType.Triangle);
            gPresetToChartShapeTypeMap.AddEntry("upArrow", (int)ChartShapeType.UpArrow);
            gPresetToChartShapeTypeMap.AddEntry("upArrowCallout", (int)ChartShapeType.UpArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("upDownArrow", (int)ChartShapeType.UpDownArrow);
            gPresetToChartShapeTypeMap.AddEntry("upDownArrowCallout", (int)ChartShapeType.UpDownArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("verticalScroll", (int)ChartShapeType.VerticalScroll);
            gPresetToChartShapeTypeMap.AddEntry("wave", (int)ChartShapeType.Wave);
            gPresetToChartShapeTypeMap.AddEntry("wedgeEllipseCallout", (int)ChartShapeType.WedgeEllipseCallout);
            gPresetToChartShapeTypeMap.AddEntry("wedgeRectCallout", (int)ChartShapeType.WedgeRectCallout);
            gPresetToChartShapeTypeMap.AddEntry("wedgeRoundRectCallout", (int)ChartShapeType.WedgeRRectCallout);
            gPresetToChartShapeTypeMap.AddEntry("roundRect", (int)ChartShapeType.RoundRectangle);
            gPresetToChartShapeTypeMap.AddEntry("rtTriangle", (int)ChartShapeType.RightTriangle);
            gPresetToChartShapeTypeMap.AddEntry("quadArrowCallout", (int)ChartShapeType.QuadArrowCallout);
            gPresetToChartShapeTypeMap.AddEntry("quadArrow", (int)ChartShapeType.QuadArrow);
            gPresetToChartShapeTypeMap.AddEntry("leftUpArrow", (int)ChartShapeType.LeftUpArrow);
            gPresetToChartShapeTypeMap.AddEntry("bentArrow", (int)ChartShapeType.BentArrow);
            gPresetToChartShapeTypeMap.AddEntry("bentUpArrow", (int)ChartShapeType.BentUpArrow);
            gPresetToChartShapeTypeMap.AddEntry("star5", (int)ChartShapeType.Star);
            gPresetToChartShapeTypeMap.AddEntry("uturnArrow", (int)ChartShapeType.UturnArrow);
            gPresetToChartShapeTypeMap.AddEntry("trapezoid", (int)ChartShapeType.Trapezoid);
            gPresetToChartShapeTypeMap.AddEntry("leftRightUpArrow", (int)ChartShapeType.LeftRightUpArrow);
            gPresetToChartShapeTypeMap.AddEntry("heart", (int)ChartShapeType.Heart);
            gPresetToChartShapeTypeMap.AddEntry("circularArrow", (int)ChartShapeType.CircularArrow);
            gPresetToChartShapeTypeMap.AddEntry("blockArc", (int)ChartShapeType.BlockArc);
            gPresetToChartShapeTypeMap.AddEntry("arc", (int)ChartShapeType.Arc);
            gPresetToChartShapeTypeMap.AddEntry("heptagon", (int)ChartShapeType.Heptagon);
            gPresetToChartShapeTypeMap.AddEntry("cloud", (int)ChartShapeType.Cloud);
            gPresetToChartShapeTypeMap.AddEntry("star6", (int)ChartShapeType.Seal6);
            gPresetToChartShapeTypeMap.AddEntry("star7", (int)ChartShapeType.Seal7);
            gPresetToChartShapeTypeMap.AddEntry("star10", (int)ChartShapeType.Seal10);
            gPresetToChartShapeTypeMap.AddEntry("star12", (int)ChartShapeType.Seal12);
            gPresetToChartShapeTypeMap.AddEntry("star24", (int)ChartShapeType.Seal24);
            gPresetToChartShapeTypeMap.AddEntry("swooshArrow", (int)ChartShapeType.SwooshArrow);
            gPresetToChartShapeTypeMap.AddEntry("teardrop", (int)ChartShapeType.Teardrop);
            gPresetToChartShapeTypeMap.AddEntry("squareTabs", (int)ChartShapeType.SquareTabs);
            gPresetToChartShapeTypeMap.AddEntry("plaqueTabs", (int)ChartShapeType.PlaqueTabs);
            gPresetToChartShapeTypeMap.AddEntry("pie", (int)ChartShapeType.Pie);
            gPresetToChartShapeTypeMap.AddEntry("pieWedge", (int)ChartShapeType.WedgePie);
            gPresetToChartShapeTypeMap.AddEntry("lineInv", (int)ChartShapeType.InverseLine);
            gPresetToChartShapeTypeMap.AddEntry("mathDivide", (int)ChartShapeType.MathDivide);
            gPresetToChartShapeTypeMap.AddEntry("mathEqual", (int)ChartShapeType.MathEqual);
            gPresetToChartShapeTypeMap.AddEntry("mathMinus", (int)ChartShapeType.MathMinus);
            gPresetToChartShapeTypeMap.AddEntry("mathMultiply", (int)ChartShapeType.MathMultiply);
            gPresetToChartShapeTypeMap.AddEntry("mathNotEqual", (int)ChartShapeType.MathNotEqual);
            gPresetToChartShapeTypeMap.AddEntry("mathPlus", (int)ChartShapeType.MathPlus);
            gPresetToChartShapeTypeMap.AddEntry("nonIsoscelesTrapezoid", (int)ChartShapeType.NonIsoscelesTrapezoid);
            gPresetToChartShapeTypeMap.AddEntry("leftRightCircularArrow", (int)ChartShapeType.LeftRightCircularArrow);
            gPresetToChartShapeTypeMap.AddEntry("leftRightRibbon", (int)ChartShapeType.LeftRightRibbon);
            gPresetToChartShapeTypeMap.AddEntry("leftCircularArrow", (int)ChartShapeType.LeftCircularArrow);
            gPresetToChartShapeTypeMap.AddEntry("frame", (int)ChartShapeType.Frame);
            gPresetToChartShapeTypeMap.AddEntry("funnel", (int)ChartShapeType.Funnel);
            gPresetToChartShapeTypeMap.AddEntry("gear6", (int)ChartShapeType.Gear6);
            gPresetToChartShapeTypeMap.AddEntry("gear9", (int)ChartShapeType.Gear9);
            gPresetToChartShapeTypeMap.AddEntry("halfFrame", (int)ChartShapeType.HalfFrame);
            gPresetToChartShapeTypeMap.AddEntry("decagon", (int)ChartShapeType.Decagon);
            gPresetToChartShapeTypeMap.AddEntry("diagStripe", (int)ChartShapeType.DiagonalStripe);
            gPresetToChartShapeTypeMap.AddEntry("dodecagon", (int)ChartShapeType.Dodecagon);
            gPresetToChartShapeTypeMap.AddEntry("corner", (int)ChartShapeType.Corner);
            gPresetToChartShapeTypeMap.AddEntry("cornerTabs", (int)ChartShapeType.CornerTabs);
            gPresetToChartShapeTypeMap.AddEntry("chord", (int)ChartShapeType.Chord);
            gPresetToChartShapeTypeMap.AddEntry("chartPlus", (int)ChartShapeType.ChartPlus);
            gPresetToChartShapeTypeMap.AddEntry("chartStar", (int)ChartShapeType.ChartStar);
            gPresetToChartShapeTypeMap.AddEntry("chartX", (int)ChartShapeType.ChartX);
            gPresetToChartShapeTypeMap.AddEntry("snip1Rect", (int)ChartShapeType.SingleCornerSnipped);
            gPresetToChartShapeTypeMap.AddEntry("snip2DiagRect", (int)ChartShapeType.DiagonalCornersSnipped);
            gPresetToChartShapeTypeMap.AddEntry("snip2SameRect", (int)ChartShapeType.TopCornersSnipped);
            gPresetToChartShapeTypeMap.AddEntry("snipRoundRect", (int)ChartShapeType.TopCornersOneRoundedOneSnipped);
            gPresetToChartShapeTypeMap.AddEntry("round1Rect", (int)ChartShapeType.SingleCornerRounded);
            gPresetToChartShapeTypeMap.AddEntry("round2DiagRect", (int)ChartShapeType.DiagonalCornersRounded);
            gPresetToChartShapeTypeMap.AddEntry("round2SameRect", (int)ChartShapeType.TopCornersRounded);
        }

        private static readonly StringToIntBidirectionalMap gPresetToChartShapeTypeMap = new StringToIntBidirectionalMap();
    }
}
