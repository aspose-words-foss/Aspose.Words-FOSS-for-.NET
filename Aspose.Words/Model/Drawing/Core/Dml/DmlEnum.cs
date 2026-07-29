// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/02/2008 by Roman Korchagin

using System;
using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Text.Bullets;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Converts DrawingML enumerated values between enum and XML string.
    /// </summary>
    internal static class DmlEnum
    {
        internal static BWMode DmlToBWMode(string value)
        {
            return (BWMode)gBWModeMap.GetValue(value, (int)BWMode.Default);
        }

        internal static string BWModeToDml(BWMode value)
        {
            return gBWModeMap.GetValue((int)value, "");
        }

        internal static ShapeLineStyle DmlToShapeLineStyle(string value)
        {
            return (ShapeLineStyle)gShapeLineStyleMap.GetValue(value, (int)ShapeLineStyle.Default);
        }

        internal static string ShapeLineStyleToDml(ShapeLineStyle value)
        {
            return gShapeLineStyleMap.GetValue((int)value, "");
        }

        internal static EndCap DmlToEndCap(string value)
        {
            return (EndCap)gEndCapMap.GetValue(value, (int)EndCap.Flat);
        }

        internal static string EndCapToDml(EndCap value)
        {
            return gEndCapMap.GetValue((int)value, "");
        }

        internal static bool DmlToInsetPen(string value)
        {
            return "in".Equals(value, StringComparison.Ordinal);
        }

        internal static string InsetPenToDml(bool value)
        {
            return value ? "in" : "ctr";
        }

        internal static RelativeHorizontalPosition DmlToRelativeHorizontalPosition(string value)
        {
            return (RelativeHorizontalPosition)gRelativeHorizontalPositionMap.GetValue(value, (int)RelativeHorizontalPosition.Default);
        }

        internal static string RelativeHorizontalPositionToDml(object value)
        {
            RelativeHorizontalPosition enumValue = (value != null)
                ? (RelativeHorizontalPosition)value
                : RelativeHorizontalPosition.Default;

            return gRelativeHorizontalPositionMap.GetValue((int)enumValue);
        }

        internal static RelativeVerticalPosition DmlToRelativeVerticalPosition(string value)
        {
            return (RelativeVerticalPosition)gRelativeVerticalPositionMap.GetValue(value, (int)RelativeVerticalPosition.TextFrameDefault);
        }

        internal static string RelativeVerticalPositionToDml(object value)
        {
            RelativeVerticalPosition enumValue = (value != null)
                ? (RelativeVerticalPosition)value
                : RelativeVerticalPosition.TextFrameDefault;

            return gRelativeVerticalPositionMap.GetValue((int)enumValue);
        }

        internal static RelativeHorizontalSize DmlToRelativeWidth(string value)
        {
            return (RelativeHorizontalSize)gRelativeHorizontalSizeMap.GetValue(value, (int)RelativeHorizontalSize.Default);
        }

        internal static string RelativeWidthToDml(object value)
        {
            RelativeHorizontalSize enumValue = (value != null)
                ? (RelativeHorizontalSize)value
                : RelativeHorizontalSize.Default;

            return gRelativeHorizontalSizeMap.GetValue((int)enumValue);
        }

        internal static RelativeVerticalSize DmlToRelativeHeight(string value)
        {
            return (RelativeVerticalSize)gRelativeVerticalSizeMap.GetValue(value, (int)RelativeVerticalSize.Default);
        }

        internal static string RelativeHeightToDml(object value)
        {
            RelativeVerticalSize enumValue = (value != null)
                ? (RelativeVerticalSize)value
                : RelativeVerticalSize.Default;

            return gRelativeVerticalSizeMap.GetValue((int)enumValue);
        }

        internal static HorizontalAlignment DmlToHorizontalAlignment(string value)
        {
            return (HorizontalAlignment)gHorizontalAlignmentMap.GetValue(value, (int)HorizontalAlignment.Default);
        }

        internal static string HorizontalAlignmentToDml(object value)
        {
            HorizontalAlignment enumValue = (value != null)
                ? (HorizontalAlignment)value
                : HorizontalAlignment.Default;

            return gHorizontalAlignmentMap.GetValue((int)enumValue);
        }

        internal static VerticalAlignment DmlToVerticalAlignment(string value)
        {
            return (VerticalAlignment)gVerticalAlignmentMap.GetValue(value, (int)VerticalAlignment.Default);
        }

        internal static string VerticalAlignmentToDml(object value)
        {
            VerticalAlignment enumValue = (value != null)
                ? (VerticalAlignment)value
                : VerticalAlignment.Default;

            return gVerticalAlignmentMap.GetValue((int)enumValue);
        }

        internal static WrapSide DmlToWrapSide(string value)
        {
            return (WrapSide)gWrapSideMap.GetValue(value, (int)WrapSide.Default);
        }

        internal static string WrapSideToDml(WrapSide value)
        {
            return gWrapSideMap.GetValue((int)value, "");
        }

        internal static SchemeColor DmlToSchemeColor(string value)
        {
            return (SchemeColor)gSchemeColorMap.GetValue(value, (int)SchemeColor.Default);
        }

        internal static string SchemeColorToDml(SchemeColor value)
        {
            return gSchemeColorMap.GetValue((int)value, "");
        }

        internal static DashStyle DmlToDashStyle(string value)
        {
            return (DashStyle)gDashStyleMap.GetValue(value, (int)DashStyle.Default);
        }

        internal static string DashStyleToDml(DashStyle value)
        {
            return gDashStyleMap.GetValue((int)value, "");
        }

        internal static ArrowWidth DmlToArrowWidth(string value)
        {
            return (ArrowWidth)gArrowWidthMap.GetValue(value, (int)ArrowWidth.Medium); // In Dml by default Medium.
        }

        internal static string ArrowWidthToDml(ArrowWidth value)
        {
            return gArrowWidthMap.GetValue((int)value, "");
        }

        internal static ArrowLength DmlToArrowLength(string value)
        {
            return (ArrowLength)gArrowLengthMap.GetValue(value, (int)ArrowLength.Medium); // In Dml by default Medium.
        }

        internal static string ArrowLengthToDml(ArrowLength value)
        {
            return gArrowLengthMap.GetValue((int)value, "");
        }

        internal static ArrowType DmlToArrowType(string value)
        {
            return (ArrowType)gArrowTypeMap.GetValue(value, (int)ArrowType.Default);
        }

        internal static string ArrowTypeToDml(ArrowType value)
        {
            return gArrowTypeMap.GetValue((int)value, "");
        }

        internal static DmlTileFlipMode DmlToTileFlipMode(string value)
        {
            return (DmlTileFlipMode)gTileFlipModeMap.GetValue(value, (int)DmlTileFlipMode.None);
        }

        internal static string TileFlipModeToDml(DmlTileFlipMode value)
        {
            return gTileFlipModeMap.GetValue((int)value, "");
        }

        internal static DmlPathShadeType DmlToPathShadeType(string value)
        {
            return (DmlPathShadeType)gPathShadeType.GetValue(value, (int)DmlPathShadeType.Shape);
        }

        internal static string PathShadeTypeToDml(DmlPathShadeType value)
        {
            return gPathShadeType.GetValue((int)value, "");
        }

        internal static EndCap DmlToLineEndingCapType(string value)
        {
            return (EndCap)gLineEndingCapTypeMap.GetValue(value, (int)EndCap.Flat);
        }

        internal static string LineEndingCapTypeToDml(EndCap value)
        {
            return gLineEndingCapTypeMap.GetValue((int)value, "");
        }

        internal static ShapeLineStyle DmlToCompoundLineType(string value)
        {
            return (ShapeLineStyle)gCompoundLineTypeMap.GetValue(value, (int)ShapeLineStyle.Single);
        }

        internal static string CompoundLineTypeToDml(ShapeLineStyle value)
        {
            return gCompoundLineTypeMap.GetValue((int)value, "");
        }

        internal static DashStyle DmlToPresetDashType(string value)
        {
            return (DashStyle)gPresetDashTypeMap.GetValue(value);
        }

        internal static string PresetDashTypeToDml(DashStyle value)
        {
            return gPresetDashTypeMap.GetValue((int)value, "");
        }

        internal static ShapeTextOrientation DmlToShapeTextOrientation(string value)
        {
            return (ShapeTextOrientation)gTextOrientationMap.GetValue(value, (int)ShapeTextOrientation.Horizontal);
        }

        internal static string ShapeTextOrientationToDml(ShapeTextOrientation value)
        {
            return gTextOrientationMap.GetValue((int)value, "");
        }

        internal static DmlTextHorizontalOverflowType DmlToTextHorizontalOverflowType(string value)
        {
            return (DmlTextHorizontalOverflowType)gTextHorizontalOverflowTypeMap.GetValue(value, (int)DmlTextHorizontalOverflowType.Overflow);
        }

        internal static string TextHorizontalOverflowTypeToDml(DmlTextHorizontalOverflowType value)
        {
            return gTextHorizontalOverflowTypeMap.GetValue((int)value, "");
        }

        internal static DmlTextVerticalOverflowType DmlToTextVerticalOverflowType(string value)
        {
            return (DmlTextVerticalOverflowType)gTextVerticalOverflowTypeMap.GetValue(value, (int)DmlTextVerticalOverflowType.Overflow);
        }

        internal static string TextVerticalOverflowTypeToDml(DmlTextVerticalOverflowType value)
        {
            return gTextVerticalOverflowTypeMap.GetValue((int)value, "");
        }

        internal static DmlTextAnchoringType DmlToTextAnchoringType(string value)
        {
            return (DmlTextAnchoringType)gTextAnchoringTypeMap.GetValue(value, (int)DmlTextAnchoringType.Top);
        }

        internal static string TextAnchoringTypeToDml(DmlTextAnchoringType value)
        {
            return gTextAnchoringTypeMap.GetValue((int)value, "");
        }

        internal static TextBoxWrapMode DmlToTextWrappingType(string value)
        {
            return (TextBoxWrapMode)gTextWrappingTypeMap.GetValue(value, (int)TextBoxWrapMode.Square);
        }

        internal static string TextWrappingTypeToDml(TextBoxWrapMode value)
        {
            return gTextWrappingTypeMap.GetValue((int)value, "");
        }

        internal static ParagraphAlignment DmlToTextAlignment(string value)
        {
            return (ParagraphAlignment)gTextAlignmentMap.GetValue(value, (int)ParagraphAlignment.Left);
        }

        internal static string TextAlignmentToDml(ParagraphAlignment value)
        {
            return gTextAlignmentMap.GetValue((int)FixUnsupportedValue(value), "");
        }

        /// <summary>
        /// Returns <c>true</c> if the specified paragraph alignment is not supported by DML paragraphs.
        /// </summary>
        internal static bool IsNotSupportedValue(ParagraphAlignment value)
        {
            return FixUnsupportedValue(value) != value;
        }

        /// <summary>
        /// Returns supported value for the specified text alignment of a DML paragraph.
        /// </summary>
        internal static ParagraphAlignment FixUnsupportedValue(ParagraphAlignment value)
        {
            switch (value)
            {
                // These items of ParagraphAlignment are not supported by DML, convert them to nearest by behaviour.
                case ParagraphAlignment.ArabicMediumKashida:
                case ParagraphAlignment.ArabicHighKashida:
                    return ParagraphAlignment.Justify;
                default:
                    return value;
            }
        }

        internal static DmlFontAlignment DmlToFontAlignment(string value)
        {
            return (DmlFontAlignment)gFontAlignmentMap.GetValue(value, (int)DmlFontAlignment.Baseline);
        }

        internal static string FontAlignmentToDml(DmlFontAlignment value)
        {
            return gFontAlignmentMap.GetValue((int)value, "");
        }

        internal static DmlCapitalization DmlToCapitalization(string value)
        {
            return (DmlCapitalization)gCapitalizationMap.GetValue(value, (int)DmlCapitalization.None);
        }

        internal static string CapitalizationToDml(DmlCapitalization value)
        {
            return gCapitalizationMap.GetValue((int)value, "");
        }

        internal static DmlTextStrike DmlToTextStrike(string value)
        {
            return (DmlTextStrike)gTextStrikeMap.GetValue(value, (int)DmlTextStrike.No);
        }

        internal static string TextStrikeToDml(DmlTextStrike value)
        {
            return gTextStrikeMap.GetValue((int)value, "");
        }

        internal static Underline DmlToTextUnderlineType(string value)
        {
            return (Underline)gTextUnderlineTypeMap.GetValue(value, (int)Underline.None);
        }

        internal static string TextUnderlineTypeToDml(Underline value)
        {
            return gTextUnderlineTypeMap.GetValue((int)value, "");
        }

        internal static PatternType DmlToPatternType(string value)
        {
            return (PatternType)gPatternTypeMap.GetValue(value, 0);
        }

        internal static string PatternTypeToDml(PatternType value)
        {
            return gPatternTypeMap.GetValue((int)value, "");
        }

        internal static DmlRectangleAlignment DmlToRectangleAlignment(string value)
        {
            return (DmlRectangleAlignment)gRectangleAlignmentMap.GetValue(value, (int)DmlRectangleAlignment.TopLeft);
        }

        internal static string RectangleAlignmentToDml(DmlRectangleAlignment value)
        {
            return gRectangleAlignmentMap.GetValue((int)value, "");
        }

        internal static DmlCompressionState DmlToCompressionState(string value)
        {
            return (DmlCompressionState)gCompressionStateMap.GetValue(value, (int)DmlCompressionState.Email);
        }

        internal static string CompressionStateToDml(DmlCompressionState value)
        {
            return gCompressionStateMap.GetValue((int)value, "");
        }

        internal static DmlEffectBlendMode DmlToEffectBlendMode(string value)
        {
            return (DmlEffectBlendMode)gEffectBlendModeMap.GetValue(value, (int)DmlEffectBlendMode.Overlay);
        }

        internal static string EffectBlendModeToDml(DmlEffectBlendMode value)
        {
            return gEffectBlendModeMap.GetValue((int)value, "");
        }

        internal static DmlPathFillMode DmlToPathFillMode(string value)
        {
            return (DmlPathFillMode)gPathFillModeMap.GetValue(value, (int)DmlPathFillMode.Norm);
        }

        internal static string PathFillModeToDml(DmlPathFillMode value)
        {
            return gPathFillModeMap.GetValue((int)value, "");
        }

        internal static DmlPresetShadow DmlToPresetShadow(string value)
        {
            return (DmlPresetShadow)gPresetShadowMap.GetValue(value, (int)DmlPresetShadow.TopLeftDropShadow);
        }

        internal static string PresetShadowToDml(DmlPresetShadow value)
        {
            return gPresetShadowMap.GetValue((int)value, "");
        }

        internal static DmlFontCollectionIndex DmlToFontCollectionIndex(string value)
        {
            return (DmlFontCollectionIndex)gFontCollectionIndexMap.GetValue(value, (int)DmlFontCollectionIndex.Major);
        }

        internal static string FontCollectionIndexToDml(DmlFontCollectionIndex value)
        {
            return gFontCollectionIndexMap.GetValue((int)value, "");
        }

        internal static DmlTextShapeType DmlToTextShapeType(string value)
        {
            return (DmlTextShapeType)gTextShapeTypeMap.GetValue(value, (int)DmlTextShapeType.TextNoShape);
        }

        internal static string TextShapeTypeToDml(DmlTextShapeType value)
        {
            return gTextShapeTypeMap.GetValue((int)value, "");
        }

        internal static DmlPresetCameraType DmlToPresetCameraType(string value)
        {
            return (DmlPresetCameraType)gCameraTypeMap.GetValue(value, (int)DmlPresetCameraType.OrthographicFront);
        }

        internal static string PresetCameraTypeToDml(DmlPresetCameraType value)
        {
            return gCameraTypeMap.GetValue((int)value, "");
        }

        internal static DmlLightRigType DmlToLightRigType(string value)
        {
            return (DmlLightRigType)gLightRigTypeMap.GetValue(value, (int)DmlLightRigType.Balanced);
        }

        internal static string LightRigTypeDml(DmlLightRigType value)
        {
            return gLightRigTypeMap.GetValue((int)value, "");
        }

        internal static DmlLightRigDirection DmlToLightRigDirection(string value)
        {
            return (DmlLightRigDirection)gLightRigDirectionMap.GetValue(value, (int)DmlLightRigDirection.Top);
        }

        internal static string LightRigDirectionToDml(DmlLightRigDirection value)
        {
            return gLightRigDirectionMap.GetValue((int)value, "");
        }

        internal static DmlPresetMaterialType DmlToPresetMaterial(string value)
        {
            return (DmlPresetMaterialType)gPresetMaterialMap.GetValue(value, (int)DmlPresetMaterialType.WarmMatte);
        }

        internal static string PresetMaterialToDml(DmlPresetMaterialType value)
        {
            return gPresetMaterialMap.GetValue((int)value, "");
        }

        internal static DmlBevelPresetType DmlToBevelPresetType(string value)
        {
            return (DmlBevelPresetType)gBevelPresetTypeMap.GetValue(value, (int)DmlBevelPresetType.Circle);
        }

        internal static string BevelPresetTypeToDml(DmlBevelPresetType value)
        {
            return gBevelPresetTypeMap.GetValue((int)value, "");
        }

        internal static DmlTextBulletType DmlToTextBulletType(string value)
        {
            return (DmlTextBulletType)gTextBulletTypeMap.GetValue(value, (int)DmlTextBulletType.ArabicPeriod);
        }

        internal static string TextBulletTypeToDml(DmlTextBulletType value)
        {
            return gTextBulletTypeMap.GetValue((int)value, "");
        }

        internal static TabAlignment DmlToTabAlignment(string value)
        {
            return (TabAlignment)gTabAlignmentMap.GetValue(value, (int)TabAlignment.Center);
        }

        internal static string TabAlignmentToDml(TabAlignment value)
        {
            return gTabAlignmentMap.GetValue((int)value, "");
        }

        internal static ShapeType DmlPresetGeomToShapeType(string presetName)
        {
            if (!gPresetToShapeTypeMap.ContainsKey(presetName))
                return ShapeType.CustomShape;

            return (ShapeType)gPresetToShapeTypeMap[presetName];
        }

        internal static string ShapeTypeToDmlPresetGeom(ShapeType value)
        {
            if (!gShapeTypeToPresetMap.ContainsKey((int)value))
                return "rect";

            return gShapeTypeToPresetMap[(int)value];
        }

        internal static string EffectDagTypeToDml(EffectDagType value)
        {
            return gEffectDagTypeMap.GetValue((int)value, "sib");
        }

        internal static EffectDagType DmlToEffectDagType(string value)
        {
            return (EffectDagType)gEffectDagTypeMap.GetValue(value, (int)EffectDagType.Sibling);
        }

        internal static DmlTextShapeType ShapeTypeToDmlPresetTextWrapType(ShapeType value)
        {
            if (!gShapeTypeToPresetTextWrapTypeMap.ContainsKey((int)value))
                return DmlTextShapeType.None;

            return (DmlTextShapeType)gShapeTypeToPresetTextWrapTypeMap[(int)value];
        }

        /// <summary>
        /// True, when specified shape type has appropriate preset VML geometry.
        /// </summary>
        internal static bool HasVmlPresetGeometry(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.NonPrimitive:
                case ShapeType.CustomShape:
                case ShapeType.SingleCornerSnipped:
                case ShapeType.DiagonalCornersSnipped:
                case ShapeType.TopCornersSnipped:
                case ShapeType.TopCornersOneRoundedOneSnipped:
                case ShapeType.SingleCornerRounded:
                case ShapeType.DiagonalCornersRounded:
                case ShapeType.TopCornersRounded:
                case ShapeType.Heptagon:
                case ShapeType.Cloud:
                case ShapeType.Seal6:
                case ShapeType.Seal7:
                case ShapeType.Seal10:
                case ShapeType.Seal12:
                case ShapeType.SwooshArrow:
                case ShapeType.Teardrop:
                case ShapeType.SquareTabs:
                case ShapeType.PlaqueTabs:
                case ShapeType.Pie:
                case ShapeType.WedgePie:
                case ShapeType.InverseLine:
                case ShapeType.MathDivide:
                case ShapeType.MathEqual:
                case ShapeType.MathMinus:
                case ShapeType.MathMultiply:
                case ShapeType.MathNotEqual:
                case ShapeType.MathPlus:
                case ShapeType.NonIsoscelesTrapezoid:
                case ShapeType.LeftRightCircularArrow:
                case ShapeType.LeftRightRibbon:
                case ShapeType.LeftCircularArrow:
                case ShapeType.Frame:
                case ShapeType.Funnel:
                case ShapeType.Gear6:
                case ShapeType.Gear9:
                case ShapeType.HalfFrame:
                case ShapeType.Decagon:
                case ShapeType.DiagonalStripe:
                case ShapeType.Dodecagon:
                case ShapeType.Corner:
                case ShapeType.CornerTabs:
                case ShapeType.Chord:
                case ShapeType.ChartPlus:
                case ShapeType.ChartStar:
                case ShapeType.ChartX:
                // Probably, DML has not appropriate analogue for shape types below.
                case ShapeType.OleControl:
                case ShapeType.OleObject:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether MS Word generates a custom geometry fallback shape for a DML shape of
        /// the specified type.
        /// </summary>
        internal static bool IsCustomGeometryShapeFallback(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.QuadArrowCallout:
                case ShapeType.QuadArrow:
                case ShapeType.LeftUpArrow:
                case ShapeType.BentArrow:
                case ShapeType.BentUpArrow:
                case ShapeType.UturnArrow:
                case ShapeType.Trapezoid:
                case ShapeType.LeftRightUpArrow:
                case ShapeType.Heart:
                case ShapeType.CircularArrow:
                case ShapeType.BlockArc:
                case ShapeType.Arc:
                case ShapeType.Star:
                    return true;
                default:
                    return !HasVmlPresetGeometry(shapeType);
            }
        }

        static DmlEnum()
        {
            gBWModeMap.AddEntry("auto", (int)BWMode.Automatic);
            gBWModeMap.AddEntry("black", (int)BWMode.Black);
            gBWModeMap.AddEntry("blackGray", (int)BWMode.BlackTextLine);
            gBWModeMap.AddEntry("blackWhite", (int)BWMode.HighContrast);
            gBWModeMap.AddEntry("clr", (int)BWMode.Color);
            gBWModeMap.AddEntry("gray", (int)BWMode.GrayScale);
            gBWModeMap.AddEntry("grayWhite", (int)BWMode.GrayOutline);
            gBWModeMap.AddEntry("hidden", (int)BWMode.DontShow);
            gBWModeMap.AddEntry("invGray", (int)BWMode.InverseGray);
            gBWModeMap.AddEntry("ltGray", (int)BWMode.LightGrayScale);
            gBWModeMap.AddEntry("white", (int)BWMode.White);

            gShapeLineStyleMap.AddEntry("sng", (int)ShapeLineStyle.Single);
            gShapeLineStyleMap.AddEntry("dbl", (int)ShapeLineStyle.Double);
            gShapeLineStyleMap.AddEntry("thinThick", (int)ShapeLineStyle.ThinThick);
            gShapeLineStyleMap.AddEntry("thickThin", (int)ShapeLineStyle.ThickThin);
            gShapeLineStyleMap.AddEntry("tri", (int)ShapeLineStyle.Triple);

            gEndCapMap.AddEntry("flat", (int)EndCap.Flat);
            gEndCapMap.AddEntry("sq", (int)EndCap.Square);
            gEndCapMap.AddEntry("rnd", (int)EndCap.Round);


            gRelativeHorizontalSizeMap.AddEntry("insideMargin", (int)RelativeHorizontalSize.InnerMargin);
            gRelativeHorizontalSizeMap.AddEntry("leftMargin", (int)RelativeHorizontalSize.LeftMargin);
            gRelativeHorizontalSizeMap.AddEntry("margin", (int)RelativeHorizontalSize.Margin);
            gRelativeHorizontalSizeMap.AddEntry("outsideMargin", (int)RelativeHorizontalSize.OuterMargin);
            gRelativeHorizontalSizeMap.AddEntry("page", (int)RelativeHorizontalSize.Page);
            gRelativeHorizontalSizeMap.AddEntry("rightMargin", (int)RelativeHorizontalSize.RightMargin);

            gRelativeVerticalSizeMap.AddEntry("bottomMargin", (int)RelativeVerticalSize.BottomMargin);
            gRelativeVerticalSizeMap.AddEntry("insideMargin", (int)RelativeVerticalSize.InnerMargin);
            gRelativeVerticalSizeMap.AddEntry("margin", (int)RelativeVerticalSize.Margin);
            gRelativeVerticalSizeMap.AddEntry("outsideMargin", (int)RelativeVerticalSize.OuterMargin);
            gRelativeVerticalSizeMap.AddEntry("page", (int)RelativeVerticalSize.Page);
            gRelativeVerticalSizeMap.AddEntry("topMargin", (int)RelativeVerticalSize.TopMargin);

            gRelativeHorizontalPositionMap.AddEntry("character", (int)RelativeHorizontalPosition.Character);
            gRelativeHorizontalPositionMap.AddEntry("column", (int)RelativeHorizontalPosition.Column);
            gRelativeHorizontalPositionMap.AddEntry("insideMargin", (int)RelativeHorizontalPosition.InsideMargin);
            gRelativeHorizontalPositionMap.AddEntry("leftMargin", (int)RelativeHorizontalPosition.LeftMargin);
            gRelativeHorizontalPositionMap.AddEntry("margin", (int)RelativeHorizontalPosition.Margin);
            gRelativeHorizontalPositionMap.AddEntry("outsideMargin", (int)RelativeHorizontalPosition.OutsideMargin);
            gRelativeHorizontalPositionMap.AddEntry("page", (int)RelativeHorizontalPosition.Page);
            gRelativeHorizontalPositionMap.AddEntry("rightMargin", (int)RelativeHorizontalPosition.RightMargin);

            gRelativeVerticalPositionMap.AddEntry("bottomMargin", (int)RelativeVerticalPosition.BottomMargin);
            gRelativeVerticalPositionMap.AddEntry("insideMargin", (int)RelativeVerticalPosition.InsideMargin);
            gRelativeVerticalPositionMap.AddEntry("line", (int)RelativeVerticalPosition.Line);
            gRelativeVerticalPositionMap.AddEntry("margin", (int)RelativeVerticalPosition.Margin);
            gRelativeVerticalPositionMap.AddEntry("outsideMargin", (int)RelativeVerticalPosition.OutsideMargin);
            gRelativeVerticalPositionMap.AddEntry("page", (int)RelativeVerticalPosition.Page);
            gRelativeVerticalPositionMap.AddEntry("paragraph", (int)RelativeVerticalPosition.Paragraph);
            gRelativeVerticalPositionMap.AddEntry("topMargin", (int)RelativeVerticalPosition.TopMargin);

            gHorizontalAlignmentMap.AddEntry("center", (int)HorizontalAlignment.Center);
            gHorizontalAlignmentMap.AddEntry("inside", (int)HorizontalAlignment.Inside);
            gHorizontalAlignmentMap.AddEntry("left", (int)HorizontalAlignment.Left);
            gHorizontalAlignmentMap.AddEntry("outside", (int)HorizontalAlignment.Outside);
            gHorizontalAlignmentMap.AddEntry("right", (int)HorizontalAlignment.Right);

            gVerticalAlignmentMap.AddEntry("bottom", (int)VerticalAlignment.Bottom);
            gVerticalAlignmentMap.AddEntry("center", (int)VerticalAlignment.Center);
            gVerticalAlignmentMap.AddEntry("inside", (int)VerticalAlignment.Inside);
            gVerticalAlignmentMap.AddEntry("outside", (int)VerticalAlignment.Outside);
            gVerticalAlignmentMap.AddEntry("top", (int)VerticalAlignment.Top);

            gWrapSideMap.AddEntry("bothSides", (int)WrapSide.Both);
            gWrapSideMap.AddEntry("largest", (int)WrapSide.Largest);
            gWrapSideMap.AddEntry("left", (int)WrapSide.Left);
            gWrapSideMap.AddEntry("right", (int)WrapSide.Right);

            gSchemeColorMap.AddEntry("accent1", (int)SchemeColor.Accent1);
            gSchemeColorMap.AddEntry("accent2", (int)SchemeColor.Accent2);
            gSchemeColorMap.AddEntry("accent3", (int)SchemeColor.Accent3);
            gSchemeColorMap.AddEntry("accent4", (int)SchemeColor.Accent4);
            gSchemeColorMap.AddEntry("accent5", (int)SchemeColor.Accent5);
            gSchemeColorMap.AddEntry("accent6", (int)SchemeColor.Accent6);
            gSchemeColorMap.AddEntry("bg1", (int)SchemeColor.Background1);
            gSchemeColorMap.AddEntry("bg2", (int)SchemeColor.Background2);
            gSchemeColorMap.AddEntry("dk1", (int)SchemeColor.Dark1);
            gSchemeColorMap.AddEntry("dk2", (int)SchemeColor.Dark2);
            gSchemeColorMap.AddEntry("folHlink", (int)SchemeColor.FollowedHyperlink);
            gSchemeColorMap.AddEntry("hlink", (int)SchemeColor.Hyperlink);
            gSchemeColorMap.AddEntry("lt1", (int)SchemeColor.Light1);
            gSchemeColorMap.AddEntry("lt2", (int)SchemeColor.Light2);
            gSchemeColorMap.AddEntry("phClr", (int)SchemeColor.Style);
            gSchemeColorMap.AddEntry("tx1", (int)SchemeColor.Text1);
            gSchemeColorMap.AddEntry("tx2", (int)SchemeColor.Text2);

            gDashStyleMap.AddEntry("dash", (int)DashStyle.Dash);
            gDashStyleMap.AddEntry("dashDot", (int)DashStyle.DashDot);
            gDashStyleMap.AddEntry("dot", (int)DashStyle.Dot);
            gDashStyleMap.AddEntry("lgDash", (int)DashStyle.LongDash);
            gDashStyleMap.AddEntry("lgDashDot", (int)DashStyle.LongDashDot);
            gDashStyleMap.AddEntry("lgDashDotDot", (int)DashStyle.LongDashDotDot);
            gDashStyleMap.AddEntry("solid", (int)DashStyle.Solid);
            gDashStyleMap.AddEntry("sysDash", (int)DashStyle.ShortDash);
            gDashStyleMap.AddEntry("sysDashDot", (int)DashStyle.ShortDashDot);
            gDashStyleMap.AddEntry("sysDashDotDot", (int)DashStyle.ShortDashDotDot);
            gDashStyleMap.AddEntry("sysDot", (int)DashStyle.ShortDot);

            gArrowTypeMap.AddEntry("arrow", (int)ArrowType.Open);
            gArrowTypeMap.AddEntry("diamond", (int)ArrowType.Diamond);
            gArrowTypeMap.AddEntry("none", (int)ArrowType.None);
            gArrowTypeMap.AddEntry("oval", (int)ArrowType.Oval);
            gArrowTypeMap.AddEntry("stealth", (int)ArrowType.Stealth);
            gArrowTypeMap.AddEntry("triangle", (int)ArrowType.Arrow);

            gArrowLengthMap.AddEntry("lg", (int)ArrowLength.Long);
            gArrowLengthMap.AddEntry("med", (int)ArrowLength.Medium);
            gArrowLengthMap.AddEntry("sm", (int)ArrowLength.Short);

            gArrowWidthMap.AddEntry("lg", (int)ArrowWidth.Wide);
            gArrowWidthMap.AddEntry("med", (int)ArrowWidth.Medium);
            gArrowWidthMap.AddEntry("sm", (int)ArrowWidth.Narrow);

            gTileFlipModeMap.AddEntry("none", (int)DmlTileFlipMode.None);
            gTileFlipModeMap.AddEntry("x", (int)DmlTileFlipMode.Horizontal);
            gTileFlipModeMap.AddEntry("y", (int)DmlTileFlipMode.Vertical);
            gTileFlipModeMap.AddEntry("xy", (int)DmlTileFlipMode.HorizontalAndVertical);

            gPathShadeType.AddEntry("circle", (int)DmlPathShadeType.Circle);
            gPathShadeType.AddEntry("rect", (int)DmlPathShadeType.Rectangle);
            gPathShadeType.AddEntry("shape", (int)DmlPathShadeType.Shape);

            gLineEndingCapTypeMap.AddEntry("flat", (int)EndCap.Flat);
            gLineEndingCapTypeMap.AddEntry("rnd", (int)EndCap.Round);
            gLineEndingCapTypeMap.AddEntry("sq", (int)EndCap.Square);

            gCompoundLineTypeMap.AddEntry("sng", (int)ShapeLineStyle.Single);
            gCompoundLineTypeMap.AddEntry("dbl", (int)ShapeLineStyle.Double);
            gCompoundLineTypeMap.AddEntry("thickThin", (int)ShapeLineStyle.ThickThin);
            gCompoundLineTypeMap.AddEntry("thinThick", (int)ShapeLineStyle.ThinThick);
            gCompoundLineTypeMap.AddEntry("tri", (int)ShapeLineStyle.Triple);

            gPresetDashTypeMap.AddEntry("solid", (int)DashStyle.Solid);
            gPresetDashTypeMap.AddEntry("dot", (int)DashStyle.Dot);
            gPresetDashTypeMap.AddEntry("dash", (int)DashStyle.Dash);
            gPresetDashTypeMap.AddEntry("lgDash", (int)DashStyle.LongDash);
            gPresetDashTypeMap.AddEntry("dashDot", (int)DashStyle.DashDot);
            gPresetDashTypeMap.AddEntry("lgDashDot", (int)DashStyle.LongDashDot);
            gPresetDashTypeMap.AddEntry("lgDashDotDot", (int)DashStyle.LongDashDotDot);
            gPresetDashTypeMap.AddEntry("sysDash", (int)DashStyle.ShortDash);
            gPresetDashTypeMap.AddEntry("sysDot", (int)DashStyle.ShortDot);
            gPresetDashTypeMap.AddEntry("sysDashDot", (int)DashStyle.ShortDashDot);
            gPresetDashTypeMap.AddEntry("sysDashDotDot", (int)DashStyle.ShortDashDotDot);

            gTextOrientationMap.AddEntry("eaVert", (int)ShapeTextOrientation.VerticalFarEast);
            gTextOrientationMap.AddEntry("horz", (int)ShapeTextOrientation.Horizontal);
            gTextOrientationMap.AddEntry("mongolianVert", (int)ShapeTextOrientation.VerticalRotatedFarEast);
            gTextOrientationMap.AddEntry("vert", (int)ShapeTextOrientation.Downward);
            gTextOrientationMap.AddEntry("vert270", (int)ShapeTextOrientation.Upward);
            gTextOrientationMap.AddEntry("wordArtVert", (int)ShapeTextOrientation.WordArtVertical);
            gTextOrientationMap.AddEntry("wordArtVertRtl", (int)ShapeTextOrientation.WordArtVerticalRightToLeft);

            gTextHorizontalOverflowTypeMap.AddEntry("overflow", (int)DmlTextHorizontalOverflowType.Overflow);
            gTextHorizontalOverflowTypeMap.AddEntry("clip", (int)DmlTextHorizontalOverflowType.Clip);

            gTextVerticalOverflowTypeMap.AddEntry("overflow", (int)DmlTextVerticalOverflowType.Overflow);
            gTextVerticalOverflowTypeMap.AddEntry("ellipsis", (int)DmlTextVerticalOverflowType.Ellipsis);
            gTextVerticalOverflowTypeMap.AddEntry("clip", (int)DmlTextVerticalOverflowType.Clip);

            gTextAnchoringTypeMap.AddEntry("b", (int)DmlTextAnchoringType.Bottom);
            gTextAnchoringTypeMap.AddEntry("ctr", (int)DmlTextAnchoringType.Center);
            gTextAnchoringTypeMap.AddEntry("dist", (int)DmlTextAnchoringType.Distributed);
            gTextAnchoringTypeMap.AddEntry("just", (int)DmlTextAnchoringType.Justified);
            gTextAnchoringTypeMap.AddEntry("t", (int)DmlTextAnchoringType.Top);

            gTextWrappingTypeMap.AddEntry("none", (int)TextBoxWrapMode.None);
            gTextWrappingTypeMap.AddEntry("square", (int)TextBoxWrapMode.Square);

            gTextAlignmentMap.AddEntry("ctr", (int)ParagraphAlignment.Center);
            gTextAlignmentMap.AddEntry("dist", (int)ParagraphAlignment.Distributed);
            gTextAlignmentMap.AddEntry("just", (int)ParagraphAlignment.Justify);
            gTextAlignmentMap.AddEntry("justLow", (int)ParagraphAlignment.ArabicLowKashida);
            gTextAlignmentMap.AddEntry("l", (int)ParagraphAlignment.Left);
            gTextAlignmentMap.AddEntry("r", (int)ParagraphAlignment.Right);
            gTextAlignmentMap.AddEntry("thaiDist", (int)ParagraphAlignment.ThaiDistributed);

            gFontAlignmentMap.AddEntry("auto", (int)DmlFontAlignment.Automatic);
            gFontAlignmentMap.AddEntry("base", (int)DmlFontAlignment.Baseline);
            gFontAlignmentMap.AddEntry("b", (int)DmlFontAlignment.Bottom);
            gFontAlignmentMap.AddEntry("ctr", (int)DmlFontAlignment.Center);
            gFontAlignmentMap.AddEntry("t", (int)DmlFontAlignment.Top);

            gCapitalizationMap.AddEntry("all", (int)DmlCapitalization.All);
            gCapitalizationMap.AddEntry("none", (int)DmlCapitalization.None);
            gCapitalizationMap.AddEntry("small", (int)DmlCapitalization.Small);

            gTextStrikeMap.AddEntry("dblStrike", (int)DmlTextStrike.Double);
            gTextStrikeMap.AddEntry("noStrike", (int)DmlTextStrike.No);
            gTextStrikeMap.AddEntry("sngStrike", (int)DmlTextStrike.Single);

            gTextUnderlineTypeMap.AddEntry("none", (int)Underline.None);
            gTextUnderlineTypeMap.AddEntry("words", (int)Underline.Words);
            gTextUnderlineTypeMap.AddEntry("sng", (int)Underline.Single);
            gTextUnderlineTypeMap.AddEntry("dbl", (int)Underline.Double);
            gTextUnderlineTypeMap.AddEntry("heavy", (int)Underline.Thick);
            gTextUnderlineTypeMap.AddEntry("dotted", (int)Underline.Dotted);
            gTextUnderlineTypeMap.AddEntry("dottedHeavy", (int)Underline.DottedHeavy);
            gTextUnderlineTypeMap.AddEntry("dash", (int)Underline.Dash);
            gTextUnderlineTypeMap.AddEntry("dashHeavy", (int)Underline.DashHeavy);
            gTextUnderlineTypeMap.AddEntry("dashLong", (int)Underline.DashLong);
            gTextUnderlineTypeMap.AddEntry("dashLongHeavy", (int)Underline.DashLongHeavy);
            gTextUnderlineTypeMap.AddEntry("dotDash", (int)Underline.DotDash);
            gTextUnderlineTypeMap.AddEntry("dotDashHeavy", (int)Underline.DotDashHeavy);
            gTextUnderlineTypeMap.AddEntry("dotDotDash", (int)Underline.DotDotDash);
            gTextUnderlineTypeMap.AddEntry("dotDotDashHeavy", (int)Underline.DotDotDashHeavy);
            gTextUnderlineTypeMap.AddEntry("wavy", (int)Underline.Wavy);
            gTextUnderlineTypeMap.AddEntry("wavyHeavy", (int)Underline.WavyHeavy);
            gTextUnderlineTypeMap.AddEntry("wavyDbl", (int)Underline.WavyDouble);

            gPatternTypeMap.AddEntry("pct5", (int)PatternType.Percent5);
            gPatternTypeMap.AddEntry("pct10", (int)PatternType.Percent10);
            gPatternTypeMap.AddEntry("pct20", (int)PatternType.Percent20);
            gPatternTypeMap.AddEntry("pct25", (int)PatternType.Percent25);
            gPatternTypeMap.AddEntry("pct30", (int)PatternType.Percent30);
            gPatternTypeMap.AddEntry("pct40", (int)PatternType.Percent40);
            gPatternTypeMap.AddEntry("pct50", (int)PatternType.Percent50);
            gPatternTypeMap.AddEntry("pct60", (int)PatternType.Percent60);
            gPatternTypeMap.AddEntry("pct70", (int)PatternType.Percent70);
            gPatternTypeMap.AddEntry("pct75", (int)PatternType.Percent75);
            gPatternTypeMap.AddEntry("pct80", (int)PatternType.Percent80);
            gPatternTypeMap.AddEntry("pct90", (int)PatternType.Percent90);
            gPatternTypeMap.AddEntry("horz", (int)PatternType.Horizontal);
            gPatternTypeMap.AddEntry("vert", (int)PatternType.Vertical);
            gPatternTypeMap.AddEntry("ltHorz", (int)PatternType.LightHorizontal);
            gPatternTypeMap.AddEntry("ltVert", (int)PatternType.LightVertical);
            gPatternTypeMap.AddEntry("dkHorz", (int)PatternType.DarkHorizontal);
            gPatternTypeMap.AddEntry("dkVert", (int)PatternType.DarkVertical);
            gPatternTypeMap.AddEntry("narHorz", (int)PatternType.NarrowHorizontal);
            gPatternTypeMap.AddEntry("narVert", (int)PatternType.NarrowVertical);
            gPatternTypeMap.AddEntry("dashHorz", (int)PatternType.DashedHorizontal);
            gPatternTypeMap.AddEntry("dashVert", (int)PatternType.DashedVertical);
            gPatternTypeMap.AddEntry("cross", (int)PatternType.Cross);
            gPatternTypeMap.AddEntry("dnDiag", (int)PatternType.DownwardDiagonal);
            gPatternTypeMap.AddEntry("upDiag", (int)PatternType.UpwardDiagonal);
            gPatternTypeMap.AddEntry("ltDnDiag", (int)PatternType.LightDownwardDiagonal);
            gPatternTypeMap.AddEntry("ltUpDiag", (int)PatternType.LightUpwardDiagonal);
            gPatternTypeMap.AddEntry("dkDnDiag", (int)PatternType.DarkDownwardDiagonal);
            gPatternTypeMap.AddEntry("dkUpDiag", (int)PatternType.DarkUpwardDiagonal);
            gPatternTypeMap.AddEntry("wdDnDiag", (int)PatternType.WideDownwardDiagonal);
            gPatternTypeMap.AddEntry("wdUpDiag", (int)PatternType.WideUpwardDiagonal);
            gPatternTypeMap.AddEntry("dashDnDiag", (int)PatternType.DashedDownwardDiagonal);
            gPatternTypeMap.AddEntry("dashUpDiag", (int)PatternType.DashedUpwardDiagonal);
            gPatternTypeMap.AddEntry("diagCross", (int)PatternType.DiagonalCross);
            gPatternTypeMap.AddEntry("smCheck", (int)PatternType.SmallCheckerBoard);
            gPatternTypeMap.AddEntry("lgCheck", (int)PatternType.LargeCheckerBoard);
            gPatternTypeMap.AddEntry("smGrid", (int)PatternType.SmallGrid);
            gPatternTypeMap.AddEntry("lgGrid", (int)PatternType.LargeGrid);
            gPatternTypeMap.AddEntry("dotGrid", (int)PatternType.DottedGrid);
            gPatternTypeMap.AddEntry("smConfetti", (int)PatternType.SmallConfetti);
            gPatternTypeMap.AddEntry("lgConfetti", (int)PatternType.LargeConfetti);
            gPatternTypeMap.AddEntry("horzBrick", (int)PatternType.HorizontalBrick);
            gPatternTypeMap.AddEntry("diagBrick", (int)PatternType.DiagonalBrick);
            gPatternTypeMap.AddEntry("solidDmnd", (int)PatternType.SolidDiamond);
            gPatternTypeMap.AddEntry("openDmnd", (int)PatternType.OutlinedDiamond);
            gPatternTypeMap.AddEntry("dotDmnd", (int)PatternType.DottedDiamond);
            gPatternTypeMap.AddEntry("plaid", (int)PatternType.Plaid);
            gPatternTypeMap.AddEntry("sphere", (int)PatternType.Sphere);
            gPatternTypeMap.AddEntry("weave", (int)PatternType.Weave);
            gPatternTypeMap.AddEntry("divot", (int)PatternType.Divot);
            gPatternTypeMap.AddEntry("shingle", (int)PatternType.Shingle);
            gPatternTypeMap.AddEntry("wave", (int)PatternType.Wave);
            gPatternTypeMap.AddEntry("trellis", (int)PatternType.Trellis);
            gPatternTypeMap.AddEntry("zigZag", (int)PatternType.ZigZag);

            gRectangleAlignmentMap.AddEntry("tl", (int)DmlRectangleAlignment.TopLeft);
            gRectangleAlignmentMap.AddEntry("t", (int)DmlRectangleAlignment.Top);
            gRectangleAlignmentMap.AddEntry("tr", (int)DmlRectangleAlignment.TopRight);
            gRectangleAlignmentMap.AddEntry("l", (int)DmlRectangleAlignment.Left);
            gRectangleAlignmentMap.AddEntry("ctr", (int)DmlRectangleAlignment.Center);
            gRectangleAlignmentMap.AddEntry("r", (int)DmlRectangleAlignment.Right);
            gRectangleAlignmentMap.AddEntry("bl", (int)DmlRectangleAlignment.BottomLeft);
            gRectangleAlignmentMap.AddEntry("b", (int)DmlRectangleAlignment.Bottom);
            gRectangleAlignmentMap.AddEntry("br", (int)DmlRectangleAlignment.BottomRight);
            gRectangleAlignmentMap.AddEntry("none", (int)DmlRectangleAlignment.None);

            gCompressionStateMap.AddEntry("email", (int)DmlCompressionState.Email);
            gCompressionStateMap.AddEntry("screen", (int)DmlCompressionState.Screen);
            gCompressionStateMap.AddEntry("print", (int)DmlCompressionState.Print);
            gCompressionStateMap.AddEntry("hqprint", (int)DmlCompressionState.HighQualityPrint);
            gCompressionStateMap.AddEntry("none", (int)DmlCompressionState.None);

            gEffectBlendModeMap.AddEntry("darken", (int)DmlEffectBlendMode.Darken);
            gEffectBlendModeMap.AddEntry("lighten", (int)DmlEffectBlendMode.Lighten);
            gEffectBlendModeMap.AddEntry("mult", (int)DmlEffectBlendMode.Multiply);
            gEffectBlendModeMap.AddEntry("over", (int)DmlEffectBlendMode.Overlay);
            gEffectBlendModeMap.AddEntry("screen", (int)DmlEffectBlendMode.Screen);

            gPathFillModeMap.AddEntry("darken", (int)DmlPathFillMode.Darken);
            gPathFillModeMap.AddEntry("darkenLess", (int)DmlPathFillMode.DarkenLess);
            gPathFillModeMap.AddEntry("lighten", (int)DmlPathFillMode.Lighten);
            gPathFillModeMap.AddEntry("lightenLess", (int)DmlPathFillMode.LightenLess);
            gPathFillModeMap.AddEntry("norm", (int)DmlPathFillMode.Norm);
            gPathFillModeMap.AddEntry("none", (int)DmlPathFillMode.None);

            gPresetShadowMap.AddEntry("shdw1", (int)DmlPresetShadow.TopLeftDropShadow);
            gPresetShadowMap.AddEntry("shdw10", (int)DmlPresetShadow.TopLeftLargeDropShadow);
            gPresetShadowMap.AddEntry("shdw11", (int)DmlPresetShadow.BackLeftLongPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw12", (int)DmlPresetShadow.BackRightLongPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw13", (int)DmlPresetShadow.TopLeftDoubleDropShadow);
            gPresetShadowMap.AddEntry("shdw14", (int)DmlPresetShadow.BottomRightSmallDropShadow);
            gPresetShadowMap.AddEntry("shdw15", (int)DmlPresetShadow.FrontLeftLongPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw16", (int)DmlPresetShadow.FrontRightLongPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw17", (int)DmlPresetShadow.OuterBoxShadow3D);
            gPresetShadowMap.AddEntry("shdw18", (int)DmlPresetShadow.InnerBoxShadow3D);
            gPresetShadowMap.AddEntry("shdw19", (int)DmlPresetShadow.BackCenterPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw2", (int)DmlPresetShadow.TopRightDropShadow);
            gPresetShadowMap.AddEntry("shdw20", (int)DmlPresetShadow.FrontBottomShadow);
            gPresetShadowMap.AddEntry("shdw3", (int)DmlPresetShadow.BackLeftPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw4", (int)DmlPresetShadow.BackRightPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw5", (int)DmlPresetShadow.BottomLeftDropShadow);
            gPresetShadowMap.AddEntry("shdw6", (int)DmlPresetShadow.BottomRightDropShadow);
            gPresetShadowMap.AddEntry("shdw7", (int)DmlPresetShadow.FrontLeftPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw8", (int)DmlPresetShadow.FrontRightPerspectiveShadow);
            gPresetShadowMap.AddEntry("shdw9", (int)DmlPresetShadow.TopLeftSmallDropShadow);

            gFontCollectionIndexMap.AddEntry("major", (int)DmlFontCollectionIndex.Major);
            gFontCollectionIndexMap.AddEntry("minor", (int)DmlFontCollectionIndex.Minor);
            gFontCollectionIndexMap.AddEntry("none", (int)DmlFontCollectionIndex.None);

            gTextShapeTypeMap.AddEntry("textArchDown", (int)DmlTextShapeType.TextArchDown);
            gTextShapeTypeMap.AddEntry("textArchDownPour", (int)DmlTextShapeType.TextArchDownPour);
            gTextShapeTypeMap.AddEntry("textArchUp", (int)DmlTextShapeType.TextArchUp);
            gTextShapeTypeMap.AddEntry("textArchUpPour", (int)DmlTextShapeType.TextArchUpPour);
            gTextShapeTypeMap.AddEntry("textButton", (int)DmlTextShapeType.TextButton);
            gTextShapeTypeMap.AddEntry("textButtonPour", (int)DmlTextShapeType.TextButtonPour);
            gTextShapeTypeMap.AddEntry("textCascadeDown", (int)DmlTextShapeType.TextCanDown);
            gTextShapeTypeMap.AddEntry("textCanUp", (int)DmlTextShapeType.TextCanUp);
            gTextShapeTypeMap.AddEntry("textCanDown", (int)DmlTextShapeType.TextCascadeDown);
            gTextShapeTypeMap.AddEntry("textCascadeUp", (int)DmlTextShapeType.TextCascadeUp);
            gTextShapeTypeMap.AddEntry("textChevron", (int)DmlTextShapeType.TextChevron);
            gTextShapeTypeMap.AddEntry("textChevronInverted", (int)DmlTextShapeType.TextChevronInverted);
            gTextShapeTypeMap.AddEntry("textCircle", (int)DmlTextShapeType.TextCircle);
            gTextShapeTypeMap.AddEntry("textCirclePour", (int)DmlTextShapeType.TextCirclePour);
            gTextShapeTypeMap.AddEntry("textCurveDown", (int)DmlTextShapeType.TextCurveDown);
            gTextShapeTypeMap.AddEntry("textCurveUp", (int)DmlTextShapeType.TextCurveUp);
            gTextShapeTypeMap.AddEntry("textDeflate", (int)DmlTextShapeType.TextDeflate);
            gTextShapeTypeMap.AddEntry("textDeflateBottom", (int)DmlTextShapeType.TextDeflateBottom);
            gTextShapeTypeMap.AddEntry("textDeflateInflate", (int)DmlTextShapeType.TextDeflateInflate);
            gTextShapeTypeMap.AddEntry("textDeflateInflateDeflate", (int)DmlTextShapeType.TextDeflateInflateDeflate);
            gTextShapeTypeMap.AddEntry("textDeflateTop", (int)DmlTextShapeType.TextDeflateTop);
            gTextShapeTypeMap.AddEntry("textDoubleWave1", (int)DmlTextShapeType.TextDoubleWave1);
            gTextShapeTypeMap.AddEntry("textFadeDown", (int)DmlTextShapeType.TextFadeDown);
            gTextShapeTypeMap.AddEntry("textFadeLeft", (int)DmlTextShapeType.TextFadeLeft);
            gTextShapeTypeMap.AddEntry("textFadeRight", (int)DmlTextShapeType.TextFadeRight);
            gTextShapeTypeMap.AddEntry("textFadeUp", (int)DmlTextShapeType.TextFadeUp);
            gTextShapeTypeMap.AddEntry("textInflate", (int)DmlTextShapeType.TextInflate);
            gTextShapeTypeMap.AddEntry("textInflateBottom", (int)DmlTextShapeType.TextInflateBottom);
            gTextShapeTypeMap.AddEntry("textInflateTop", (int)DmlTextShapeType.TextInflateTop);
            gTextShapeTypeMap.AddEntry("textNoShape", (int)DmlTextShapeType.TextNoShape);
            gTextShapeTypeMap.AddEntry("textPlain", (int)DmlTextShapeType.TextPlain);
            gTextShapeTypeMap.AddEntry("textRingInside", (int)DmlTextShapeType.TextRingInside);
            gTextShapeTypeMap.AddEntry("textRingOutside", (int)DmlTextShapeType.TextRingOutside);
            gTextShapeTypeMap.AddEntry("textSlantDown", (int)DmlTextShapeType.TextSlantDown);
            gTextShapeTypeMap.AddEntry("textSlantUp", (int)DmlTextShapeType.TextSlantUp);
            gTextShapeTypeMap.AddEntry("textStop", (int)DmlTextShapeType.TextStop);
            gTextShapeTypeMap.AddEntry("textTriangle", (int)DmlTextShapeType.TextTriangle);
            gTextShapeTypeMap.AddEntry("textTriangleInverted", (int)DmlTextShapeType.TextTriangleInverted);
            gTextShapeTypeMap.AddEntry("textWave1", (int)DmlTextShapeType.TextWave1);
            gTextShapeTypeMap.AddEntry("textWave2", (int)DmlTextShapeType.TextWave2);
            gTextShapeTypeMap.AddEntry("textWave4", (int)DmlTextShapeType.TextWave4);

            gCameraTypeMap.AddEntry("legacyObliqueTopLeft", (int)DmlPresetCameraType.LegacyObliqueTopLeft);
            gCameraTypeMap.AddEntry("legacyObliqueTop", (int)DmlPresetCameraType.LegacyObliqueTop);
            gCameraTypeMap.AddEntry("legacyObliqueTopRight", (int)DmlPresetCameraType.LegacyObliqueTopRight);
            gCameraTypeMap.AddEntry("legacyObliqueLeft", (int)DmlPresetCameraType.LegacyObliqueLeft);
            gCameraTypeMap.AddEntry("legacyObliqueFront", (int)DmlPresetCameraType.LegacyObliqueFront);
            gCameraTypeMap.AddEntry("legacyObliqueRight", (int)DmlPresetCameraType.LegacyObliqueRight);
            gCameraTypeMap.AddEntry("legacyObliqueBottomLeft", (int)DmlPresetCameraType.LegacyObliqueBottomLeft);
            gCameraTypeMap.AddEntry("legacyObliqueBottom", (int)DmlPresetCameraType.LegacyObliqueBottom);
            gCameraTypeMap.AddEntry("legacyObliqueBottomRight", (int)DmlPresetCameraType.LegacyObliqueBottomRight);
            gCameraTypeMap.AddEntry("legacyPerspectiveTopLeft", (int)DmlPresetCameraType.LegacyPerspectiveTopLeft);
            gCameraTypeMap.AddEntry("legacyPerspectiveTop", (int)DmlPresetCameraType.LegacyPerspectiveTop);
            gCameraTypeMap.AddEntry("legacyPerspectiveTopRight", (int)DmlPresetCameraType.LegacyPerspectiveTopRight);
            gCameraTypeMap.AddEntry("legacyPerspectiveLeft", (int)DmlPresetCameraType.LegacyPerspectiveLeft);
            gCameraTypeMap.AddEntry("legacyPerspectiveFront", (int)DmlPresetCameraType.LegacyPerspectiveFront);
            gCameraTypeMap.AddEntry("legacyPerspectiveRight", (int)DmlPresetCameraType.LegacyPerspectiveRight);
            gCameraTypeMap.AddEntry("legacyPerspectiveBottomLeft", (int)DmlPresetCameraType.LegacyPerspectiveBottomLeft);
            gCameraTypeMap.AddEntry("legacyPerspectiveBottom", (int)DmlPresetCameraType.LegacyPerspectiveBottom);
            gCameraTypeMap.AddEntry("legacyPerspectiveBottomRight", (int)DmlPresetCameraType.LegacyPerspectiveBottomRight);
            gCameraTypeMap.AddEntry("orthographicFront", (int)DmlPresetCameraType.OrthographicFront);
            gCameraTypeMap.AddEntry("isometricTopUp", (int)DmlPresetCameraType.IsometricTopUp);
            gCameraTypeMap.AddEntry("isometricTopDown", (int)DmlPresetCameraType.IsometricTopDown);
            gCameraTypeMap.AddEntry("isometricBottomUp", (int)DmlPresetCameraType.IsometricBottomUp);
            gCameraTypeMap.AddEntry("isometricBottomDown", (int)DmlPresetCameraType.IsometricBottomDown);
            gCameraTypeMap.AddEntry("isometricLeftUp", (int)DmlPresetCameraType.IsometricLeftUp);
            gCameraTypeMap.AddEntry("isometricLeftDown", (int)DmlPresetCameraType.IsometricLeftDown);
            gCameraTypeMap.AddEntry("isometricRightUp", (int)DmlPresetCameraType.IsometricRightUp);
            gCameraTypeMap.AddEntry("isometricRightDown", (int)DmlPresetCameraType.IsometricRightDown);
            gCameraTypeMap.AddEntry("isometricOffAxis1Left", (int)DmlPresetCameraType.IsometricOffAxis1Left);
            gCameraTypeMap.AddEntry("isometricOffAxis1Right", (int)DmlPresetCameraType.IsometricOffAxis1Right);
            gCameraTypeMap.AddEntry("isometricOffAxis1Top", (int)DmlPresetCameraType.IsometricOffAxis1Top);
            gCameraTypeMap.AddEntry("isometricOffAxis2Left", (int)DmlPresetCameraType.IsometricOffAxis2Left);
            gCameraTypeMap.AddEntry("isometricOffAxis2Right", (int)DmlPresetCameraType.IsometricOffAxis2Right);
            gCameraTypeMap.AddEntry("isometricOffAxis2Top", (int)DmlPresetCameraType.IsometricOffAxis2Top);
            gCameraTypeMap.AddEntry("isometricOffAxis3Left", (int)DmlPresetCameraType.IsometricOffAxis3Left);
            gCameraTypeMap.AddEntry("isometricOffAxis3Right", (int)DmlPresetCameraType.IsometricOffAxis3Right);
            gCameraTypeMap.AddEntry("isometricOffAxis3Bottom", (int)DmlPresetCameraType.IsometricOffAxis3Bottom);
            gCameraTypeMap.AddEntry("isometricOffAxis4Left", (int)DmlPresetCameraType.IsometricOffAxis4Left);
            gCameraTypeMap.AddEntry("isometricOffAxis4Right", (int)DmlPresetCameraType.IsometricOffAxis4Right);
            gCameraTypeMap.AddEntry("isometricOffAxis4Bottom", (int)DmlPresetCameraType.IsometricOffAxis4Bottom);
            gCameraTypeMap.AddEntry("obliqueTopLeft", (int)DmlPresetCameraType.ObliqueTopLeft);
            gCameraTypeMap.AddEntry("obliqueTop", (int)DmlPresetCameraType.ObliqueTop);
            gCameraTypeMap.AddEntry("obliqueTopRight", (int)DmlPresetCameraType.ObliqueTopRight);
            gCameraTypeMap.AddEntry("obliqueLeft", (int)DmlPresetCameraType.ObliqueLeft);
            gCameraTypeMap.AddEntry("obliqueRight", (int)DmlPresetCameraType.ObliqueRight);
            gCameraTypeMap.AddEntry("obliqueBottomLeft", (int)DmlPresetCameraType.ObliqueBottomLeft);
            gCameraTypeMap.AddEntry("obliqueBottom", (int)DmlPresetCameraType.ObliqueBottom);
            gCameraTypeMap.AddEntry("obliqueBottomRight", (int)DmlPresetCameraType.ObliqueBottomRight);
            gCameraTypeMap.AddEntry("perspectiveFront", (int)DmlPresetCameraType.PerspectiveFront);
            gCameraTypeMap.AddEntry("perspectiveLeft", (int)DmlPresetCameraType.PerspectiveLeft);
            gCameraTypeMap.AddEntry("perspectiveRight", (int)DmlPresetCameraType.PerspectiveRight);
            gCameraTypeMap.AddEntry("perspectiveAbove", (int)DmlPresetCameraType.PerspectiveAbove);
            gCameraTypeMap.AddEntry("perspectiveBelow", (int)DmlPresetCameraType.PerspectiveBelow);
            gCameraTypeMap.AddEntry("perspectiveAboveLeftFacing", (int)DmlPresetCameraType.PerspectiveAboveLeftFacing);
            gCameraTypeMap.AddEntry("perspectiveAboveRightFacing", (int)DmlPresetCameraType.PerspectiveAboveRightFacing);
            gCameraTypeMap.AddEntry("perspectiveContrastingLeftFacing",
                (int)DmlPresetCameraType.PerspectiveContrastingLeftFacing);
            gCameraTypeMap.AddEntry("perspectiveContrastingRightFacing",
                (int)DmlPresetCameraType.PerspectiveContrastingRightFacing);
            gCameraTypeMap.AddEntry("perspectiveHeroicLeftFacing", (int)DmlPresetCameraType.PerspectiveHeroicLeftFacing);
            gCameraTypeMap.AddEntry("perspectiveHeroicRightFacing", (int)DmlPresetCameraType.PerspectiveHeroicRightFacing);
            gCameraTypeMap.AddEntry("perspectiveHeroicExtremeLeftFacing",
                (int)DmlPresetCameraType.PerspectiveHeroicExtremeLeftFacing);
            gCameraTypeMap.AddEntry("perspectiveHeroicExtremeRightFacing",
                (int)DmlPresetCameraType.PerspectiveHeroicExtremeRightFacing);
            gCameraTypeMap.AddEntry("perspectiveRelaxed", (int)DmlPresetCameraType.PerspectiveRelaxed);
            gCameraTypeMap.AddEntry("perspectiveRelaxedModerately", (int)DmlPresetCameraType.PerspectiveRelaxedModerately);

            gLightRigDirectionMap.AddEntry("tl", (int)DmlLightRigDirection.TopLeft);
            gLightRigDirectionMap.AddEntry("t", (int)DmlLightRigDirection.Top);
            gLightRigDirectionMap.AddEntry("tr", (int)DmlLightRigDirection.TopRight);
            gLightRigDirectionMap.AddEntry("l", (int)DmlLightRigDirection.Left);
            gLightRigDirectionMap.AddEntry("r", (int)DmlLightRigDirection.Right);
            gLightRigDirectionMap.AddEntry("bl", (int)DmlLightRigDirection.BottomLeft);
            gLightRigDirectionMap.AddEntry("b", (int)DmlLightRigDirection.Bottom);
            gLightRigDirectionMap.AddEntry("br", (int)DmlLightRigDirection.BottomRight);

            gLightRigTypeMap.AddEntry("legacyFlat1", (int)DmlLightRigType.LegacyFlat1);
            gLightRigTypeMap.AddEntry("legacyFlat2", (int)DmlLightRigType.LegacyFlat2);
            gLightRigTypeMap.AddEntry("legacyFlat3", (int)DmlLightRigType.LegacyFlat3);
            gLightRigTypeMap.AddEntry("legacyFlat4", (int)DmlLightRigType.LegacyFlat4);
            gLightRigTypeMap.AddEntry("legacyNormal1", (int)DmlLightRigType.LegacyNormal1);
            gLightRigTypeMap.AddEntry("legacyNormal2", (int)DmlLightRigType.LegacyNormal2);
            gLightRigTypeMap.AddEntry("legacyNormal3", (int)DmlLightRigType.LegacyNormal3);
            gLightRigTypeMap.AddEntry("legacyNormal4", (int)DmlLightRigType.LegacyNormal4);
            gLightRigTypeMap.AddEntry("legacyHarsh1", (int)DmlLightRigType.LegacyHarsh1);
            gLightRigTypeMap.AddEntry("legacyHarsh2", (int)DmlLightRigType.LegacyHarsh2);
            gLightRigTypeMap.AddEntry("legacyHarsh3", (int)DmlLightRigType.LegacyHarsh3);
            gLightRigTypeMap.AddEntry("legacyHarsh4", (int)DmlLightRigType.LegacyHarsh4);
            gLightRigTypeMap.AddEntry("threePt", (int)DmlLightRigType.ThreePt);
            gLightRigTypeMap.AddEntry("balanced", (int)DmlLightRigType.Balanced);
            gLightRigTypeMap.AddEntry("soft", (int)DmlLightRigType.Soft);
            gLightRigTypeMap.AddEntry("harsh", (int)DmlLightRigType.Harsh);
            gLightRigTypeMap.AddEntry("flood", (int)DmlLightRigType.Flood);
            gLightRigTypeMap.AddEntry("contrasting", (int)DmlLightRigType.Contrasting);
            gLightRigTypeMap.AddEntry("morning", (int)DmlLightRigType.Morning);
            gLightRigTypeMap.AddEntry("sunrise", (int)DmlLightRigType.Sunrise);
            gLightRigTypeMap.AddEntry("sunset", (int)DmlLightRigType.Sunset);
            gLightRigTypeMap.AddEntry("chilly", (int)DmlLightRigType.Chilly);
            gLightRigTypeMap.AddEntry("freezing", (int)DmlLightRigType.Freezing);
            gLightRigTypeMap.AddEntry("flat", (int)DmlLightRigType.Flat);
            gLightRigTypeMap.AddEntry("twoPt", (int)DmlLightRigType.TwoPt);
            gLightRigTypeMap.AddEntry("glow", (int)DmlLightRigType.Glow);
            gLightRigTypeMap.AddEntry("brightRoom", (int)DmlLightRigType.BrightRoom);

            gPresetMaterialMap.AddEntry("legacyMatte", (int)DmlPresetMaterialType.LegacyMatte);
            gPresetMaterialMap.AddEntry("legacyPlastic", (int)DmlPresetMaterialType.LegacyPlastic);
            gPresetMaterialMap.AddEntry("legacyMetal", (int)DmlPresetMaterialType.LegacyMetal);
            gPresetMaterialMap.AddEntry("legacyWireframe", (int)DmlPresetMaterialType.LegacyWireframe);
            gPresetMaterialMap.AddEntry("matte", (int)DmlPresetMaterialType.Matte);
            gPresetMaterialMap.AddEntry("plastic", (int)DmlPresetMaterialType.Plastic);
            gPresetMaterialMap.AddEntry("metal", (int)DmlPresetMaterialType.Metal);
            gPresetMaterialMap.AddEntry("warmMatte", (int)DmlPresetMaterialType.WarmMatte);
            gPresetMaterialMap.AddEntry("translucentPowder", (int)DmlPresetMaterialType.TranslucentPowder);
            gPresetMaterialMap.AddEntry("powder", (int)DmlPresetMaterialType.Powder);
            gPresetMaterialMap.AddEntry("dkEdge", (int)DmlPresetMaterialType.DarkEdge);
            gPresetMaterialMap.AddEntry("softEdge", (int)DmlPresetMaterialType.SoftEdge);
            gPresetMaterialMap.AddEntry("clear", (int)DmlPresetMaterialType.Clear);
            gPresetMaterialMap.AddEntry("flat", (int)DmlPresetMaterialType.Flat);
            gPresetMaterialMap.AddEntry("softmetal", (int)DmlPresetMaterialType.SoftMetal);

            gBevelPresetTypeMap.AddEntry("relaxedInset", (int)DmlBevelPresetType.RelaxedInset);
            gBevelPresetTypeMap.AddEntry("circle", (int)DmlBevelPresetType.Circle);
            gBevelPresetTypeMap.AddEntry("slope", (int)DmlBevelPresetType.Slope);
            gBevelPresetTypeMap.AddEntry("cross", (int)DmlBevelPresetType.Cross);
            gBevelPresetTypeMap.AddEntry("angle", (int)DmlBevelPresetType.Angle);
            gBevelPresetTypeMap.AddEntry("softRound", (int)DmlBevelPresetType.SoftRound);
            gBevelPresetTypeMap.AddEntry("convex", (int)DmlBevelPresetType.Convex);
            gBevelPresetTypeMap.AddEntry("coolSlant", (int)DmlBevelPresetType.CoolSlant);
            gBevelPresetTypeMap.AddEntry("divot", (int)DmlBevelPresetType.Divot);
            gBevelPresetTypeMap.AddEntry("riblet", (int)DmlBevelPresetType.Riblet);
            gBevelPresetTypeMap.AddEntry("hardEdge", (int)DmlBevelPresetType.HardEdge);
            gBevelPresetTypeMap.AddEntry("artDeco", (int)DmlBevelPresetType.ArtDeco);

            gTextBulletTypeMap.AddEntry("alphaLcParenBoth", (int)DmlTextBulletType.AlphaLcParenBoth);
            gTextBulletTypeMap.AddEntry("alphaUcParenBoth", (int)DmlTextBulletType.AlphaUcParenBoth);
            gTextBulletTypeMap.AddEntry("alphaLcParenR", (int)DmlTextBulletType.AlphaLcParenR);
            gTextBulletTypeMap.AddEntry("alphaUcParenR", (int)DmlTextBulletType.AlphaUcParenR);
            gTextBulletTypeMap.AddEntry("alphaLcPeriod", (int)DmlTextBulletType.AlphaLcPeriod);
            gTextBulletTypeMap.AddEntry("alphaUcPeriod", (int)DmlTextBulletType.AlphaUcPeriod);
            gTextBulletTypeMap.AddEntry("arabicParenBoth", (int)DmlTextBulletType.ArabicParenBoth);
            gTextBulletTypeMap.AddEntry("arabicParenR", (int)DmlTextBulletType.ArabicParenR);
            gTextBulletTypeMap.AddEntry("arabicPeriod", (int)DmlTextBulletType.ArabicPeriod);
            gTextBulletTypeMap.AddEntry("arabicPlain", (int)DmlTextBulletType.ArabicPlain);
            gTextBulletTypeMap.AddEntry("romanLcParenBoth", (int)DmlTextBulletType.RomanLcParenBoth);
            gTextBulletTypeMap.AddEntry("romanUcParenBoth", (int)DmlTextBulletType.RomanUcParenBoth);
            gTextBulletTypeMap.AddEntry("romanLcParenR", (int)DmlTextBulletType.RomanLcParenR);
            gTextBulletTypeMap.AddEntry("romanUcParenR", (int)DmlTextBulletType.RomanUcParenR);
            gTextBulletTypeMap.AddEntry("romanLcPeriod", (int)DmlTextBulletType.RomanLcPeriod);
            gTextBulletTypeMap.AddEntry("romanUcPeriod", (int)DmlTextBulletType.RomanUcPeriod);
            gTextBulletTypeMap.AddEntry("circleNumDbPlain", (int)DmlTextBulletType.CircleNumDbPlain);
            gTextBulletTypeMap.AddEntry("circleNumWdBlackPlain", (int)DmlTextBulletType.CircleNumWdBlackPlain);
            gTextBulletTypeMap.AddEntry("circleNumWdWhitePlain", (int)DmlTextBulletType.CircleNumWdWhitePlain);
            gTextBulletTypeMap.AddEntry("arabicDbPeriod", (int)DmlTextBulletType.ArabicDbPeriod);
            gTextBulletTypeMap.AddEntry("arabicDbPlain", (int)DmlTextBulletType.ArabicDbPlain);
            gTextBulletTypeMap.AddEntry("ea1ChsPeriod", (int)DmlTextBulletType.Ea1ChsPeriod);
            gTextBulletTypeMap.AddEntry("ea1ChsPlain", (int)DmlTextBulletType.Ea1ChsPlain);
            gTextBulletTypeMap.AddEntry("ea1ChtPeriod", (int)DmlTextBulletType.Ea1ChtPeriod);
            gTextBulletTypeMap.AddEntry("ea1ChtPlain", (int)DmlTextBulletType.Ea1ChtPlain);
            gTextBulletTypeMap.AddEntry("ea1JpnChsDbPeriod", (int)DmlTextBulletType.Ea1JpnChsDbPeriod);
            gTextBulletTypeMap.AddEntry("ea1JpnKorPlain", (int)DmlTextBulletType.Ea1JpnKorPlain);
            gTextBulletTypeMap.AddEntry("ea1JpnKorPeriod", (int)DmlTextBulletType.Ea1JpnKorPeriod);
            gTextBulletTypeMap.AddEntry("arabic1Minus", (int)DmlTextBulletType.Arabic1Minus);
            gTextBulletTypeMap.AddEntry("arabic2Minus", (int)DmlTextBulletType.Arabic2Minus);
            gTextBulletTypeMap.AddEntry("hebrew2Minus", (int)DmlTextBulletType.Hebrew2Minus);
            gTextBulletTypeMap.AddEntry("thaiAlphaPeriod", (int)DmlTextBulletType.ThaiAlphaPeriod);
            gTextBulletTypeMap.AddEntry("thaiAlphaParenR", (int)DmlTextBulletType.ThaiAlphaParenR);
            gTextBulletTypeMap.AddEntry("thaiAlphaParenBoth", (int)DmlTextBulletType.ThaiAlphaParenBoth);
            gTextBulletTypeMap.AddEntry("thaiNumPeriod", (int)DmlTextBulletType.ThaiNumPeriod);
            gTextBulletTypeMap.AddEntry("thaiNumParenR", (int)DmlTextBulletType.ThaiNumParenR);
            gTextBulletTypeMap.AddEntry("thaiNumParenBoth", (int)DmlTextBulletType.ThaiNumParenBoth);
            gTextBulletTypeMap.AddEntry("hindiAlphaPeriod", (int)DmlTextBulletType.HindiAlphaPeriod);
            gTextBulletTypeMap.AddEntry("hindiNumPeriod", (int)DmlTextBulletType.HindiNumPeriod);
            gTextBulletTypeMap.AddEntry("hindiNumParenR", (int)DmlTextBulletType.HindiNumParenR);
            gTextBulletTypeMap.AddEntry("hindiAlpha1Period", (int)DmlTextBulletType.HindiAlpha1Period);

            gTabAlignmentMap.AddEntry("ctr", (int)TabAlignment.Center);
            gTabAlignmentMap.AddEntry("dec", (int)TabAlignment.Decimal);
            gTabAlignmentMap.AddEntry("l", (int)TabAlignment.Left);
            gTabAlignmentMap.AddEntry("r", (int)TabAlignment.Right);
            gPresetToShapeTypeMap.Add("accentBorderCallout1", (int)ShapeType.AccentBorderCallout1);
            gPresetToShapeTypeMap.Add("accentBorderCallout2", (int)ShapeType.AccentBorderCallout2);
            gPresetToShapeTypeMap.Add("accentBorderCallout3", (int)ShapeType.AccentBorderCallout3);

            gEffectDagTypeMap.AddEntry("sib", (int)EffectDagType.Sibling);
            gEffectDagTypeMap.AddEntry("tree", (int)EffectDagType.Tree);
            gPresetToShapeTypeMap.Add("accentCallout1", (int)ShapeType.AccentCallout1);
            gPresetToShapeTypeMap.Add("accentCallout2", (int)ShapeType.AccentCallout2);
            gPresetToShapeTypeMap.Add("accentCallout3", (int)ShapeType.AccentCallout3);
            gPresetToShapeTypeMap.Add("actionButtonBackPrevious", (int)ShapeType.ActionButtonBackPrevious);
            gPresetToShapeTypeMap.Add("actionButtonBeginning", (int)ShapeType.ActionButtonBeginning);
            gPresetToShapeTypeMap.Add("actionButtonBlank", (int)ShapeType.ActionButtonBlank);
            gPresetToShapeTypeMap.Add("actionButtonDocument", (int)ShapeType.ActionButtonDocument);
            gPresetToShapeTypeMap.Add("actionButtonEnd", (int)ShapeType.ActionButtonEnd);
            gPresetToShapeTypeMap.Add("actionButtonForwardNext", (int)ShapeType.ActionButtonForwardNext);
            gPresetToShapeTypeMap.Add("actionButtonHelp", (int)ShapeType.ActionButtonHelp);
            gPresetToShapeTypeMap.Add("actionButtonHome", (int)ShapeType.ActionButtonHome);
            gPresetToShapeTypeMap.Add("actionButtonInformation", (int)ShapeType.ActionButtonInformation);
            gPresetToShapeTypeMap.Add("actionButtonMovie", (int)ShapeType.ActionButtonMovie);
            gPresetToShapeTypeMap.Add("actionButtonReturn", (int)ShapeType.ActionButtonReturn);
            gPresetToShapeTypeMap.Add("actionButtonSound", (int)ShapeType.ActionButtonSound);
            gPresetToShapeTypeMap.Add("bentConnector2", (int)ShapeType.BentConnector2);
            gPresetToShapeTypeMap.Add("bentConnector3", (int)ShapeType.BentConnector3);
            gPresetToShapeTypeMap.Add("bentConnector4", (int)ShapeType.BentConnector4);
            gPresetToShapeTypeMap.Add("bentConnector5", (int)ShapeType.BentConnector5);
            gPresetToShapeTypeMap.Add("bevel", (int)ShapeType.Bevel);
            gPresetToShapeTypeMap.Add("borderCallout1", (int)ShapeType.BorderCallout1);
            gPresetToShapeTypeMap.Add("borderCallout2", (int)ShapeType.BorderCallout2);
            gPresetToShapeTypeMap.Add("borderCallout3", (int)ShapeType.BorderCallout3);
            gPresetToShapeTypeMap.Add("bracePair", (int)ShapeType.BracePair);
            gPresetToShapeTypeMap.Add("bracketPair", (int)ShapeType.BracketPair);
            gPresetToShapeTypeMap.Add("callout1", (int)ShapeType.Callout1);
            gPresetToShapeTypeMap.Add("callout2", (int)ShapeType.Callout2);
            gPresetToShapeTypeMap.Add("callout3", (int)ShapeType.Callout3);
            gPresetToShapeTypeMap.Add("can", (int)ShapeType.Can);
            gPresetToShapeTypeMap.Add("chevron", (int)ShapeType.Chevron);
            gPresetToShapeTypeMap.Add("cloudCallout", (int)ShapeType.CloudCallout);
            gPresetToShapeTypeMap.Add("cube", (int)ShapeType.Cube);
            gPresetToShapeTypeMap.Add("curvedConnector2", (int)ShapeType.CurvedConnector2);
            gPresetToShapeTypeMap.Add("curvedConnector3", (int)ShapeType.CurvedConnector3);
            gPresetToShapeTypeMap.Add("curvedConnector4", (int)ShapeType.CurvedConnector4);
            gPresetToShapeTypeMap.Add("curvedConnector5", (int)ShapeType.CurvedConnector5);
            gPresetToShapeTypeMap.Add("curvedDownArrow", (int)ShapeType.CurvedDownArrow);
            gPresetToShapeTypeMap.Add("curvedLeftArrow", (int)ShapeType.CurvedLeftArrow);
            gPresetToShapeTypeMap.Add("curvedRightArrow", (int)ShapeType.CurvedRightArrow);
            gPresetToShapeTypeMap.Add("curvedUpArrow", (int)ShapeType.CurvedUpArrow);
            gPresetToShapeTypeMap.Add("diamond", (int)ShapeType.Diamond);
            gPresetToShapeTypeMap.Add("donut", (int)ShapeType.Donut);
            gPresetToShapeTypeMap.Add("doubleWave", (int)ShapeType.DoubleWave);
            gPresetToShapeTypeMap.Add("downArrow", (int)ShapeType.DownArrow);
            gPresetToShapeTypeMap.Add("downArrowCallout", (int)ShapeType.DownArrowCallout);
            gPresetToShapeTypeMap.Add("ellipse", (int)ShapeType.Ellipse);
            gPresetToShapeTypeMap.Add("ellipseRibbon", (int)ShapeType.EllipseRibbon);
            gPresetToShapeTypeMap.Add("ellipseRibbon2", (int)ShapeType.EllipseRibbon2);
            gPresetToShapeTypeMap.Add("flowChartAlternateProcess", (int)ShapeType.FlowChartAlternateProcess);
            gPresetToShapeTypeMap.Add("flowChartCollate", (int)ShapeType.FlowChartCollate);
            gPresetToShapeTypeMap.Add("flowChartConnector", (int)ShapeType.FlowChartConnector);
            gPresetToShapeTypeMap.Add("flowChartDecision", (int)ShapeType.FlowChartDecision);
            gPresetToShapeTypeMap.Add("flowChartDelay", (int)ShapeType.FlowChartDelay);
            gPresetToShapeTypeMap.Add("flowChartDisplay", (int)ShapeType.FlowChartDisplay);
            gPresetToShapeTypeMap.Add("flowChartDocument", (int)ShapeType.FlowChartDocument);
            gPresetToShapeTypeMap.Add("flowChartExtract", (int)ShapeType.FlowChartExtract);
            gPresetToShapeTypeMap.Add("flowChartInputOutput", (int)ShapeType.FlowChartInputOutput);
            gPresetToShapeTypeMap.Add("flowChartInternalStorage", (int)ShapeType.FlowChartInternalStorage);
            gPresetToShapeTypeMap.Add("flowChartMagneticDisk", (int)ShapeType.FlowChartMagneticDisk);
            gPresetToShapeTypeMap.Add("flowChartMagneticDrum", (int)ShapeType.FlowChartMagneticDrum);
            gPresetToShapeTypeMap.Add("flowChartMagneticTape", (int)ShapeType.FlowChartMagneticTape);
            gPresetToShapeTypeMap.Add("flowChartManualInput", (int)ShapeType.FlowChartManualInput);
            gPresetToShapeTypeMap.Add("flowChartManualOperation", (int)ShapeType.FlowChartManualOperation);
            gPresetToShapeTypeMap.Add("flowChartMerge", (int)ShapeType.FlowChartMerge);
            gPresetToShapeTypeMap.Add("flowChartMultidocument", (int)ShapeType.FlowChartMultidocument);
            gPresetToShapeTypeMap.Add("flowChartOfflineStorage", (int)ShapeType.FlowChartOfflineStorage);
            gPresetToShapeTypeMap.Add("flowChartOffpageConnector", (int)ShapeType.FlowChartOffpageConnector);
            gPresetToShapeTypeMap.Add("flowChartOnlineStorage", (int)ShapeType.FlowChartOnlineStorage);
            gPresetToShapeTypeMap.Add("flowChartOr", (int)ShapeType.FlowChartOr);
            gPresetToShapeTypeMap.Add("flowChartPredefinedProcess", (int)ShapeType.FlowChartPredefinedProcess);
            gPresetToShapeTypeMap.Add("flowChartPreparation", (int)ShapeType.FlowChartPreparation);
            gPresetToShapeTypeMap.Add("flowChartProcess", (int)ShapeType.FlowChartProcess);
            gPresetToShapeTypeMap.Add("flowChartPunchedCard", (int)ShapeType.FlowChartPunchedCard);
            gPresetToShapeTypeMap.Add("flowChartPunchedTape", (int)ShapeType.FlowChartPunchedTape);
            gPresetToShapeTypeMap.Add("flowChartSort", (int)ShapeType.FlowChartSort);
            gPresetToShapeTypeMap.Add("flowChartSummingJunction", (int)ShapeType.FlowChartSummingJunction);
            gPresetToShapeTypeMap.Add("flowChartTerminator", (int)ShapeType.FlowChartTerminator);
            gPresetToShapeTypeMap.Add("foldedCorner", (int)ShapeType.FoldedCorner);
            gPresetToShapeTypeMap.Add("hexagon", (int)ShapeType.Hexagon);
            gPresetToShapeTypeMap.Add("homePlate", (int)ShapeType.HomePlate);
            gPresetToShapeTypeMap.Add("horizontalScroll", (int)ShapeType.HorizontalScroll);
            gPresetToShapeTypeMap.Add("irregularSeal1", (int)ShapeType.IrregularSeal1);
            gPresetToShapeTypeMap.Add("irregularSeal2", (int)ShapeType.IrregularSeal2);
            gPresetToShapeTypeMap.Add("leftArrow", (int)ShapeType.LeftArrow);
            gPresetToShapeTypeMap.Add("leftArrowCallout", (int)ShapeType.LeftArrowCallout);
            gPresetToShapeTypeMap.Add("leftBrace", (int)ShapeType.LeftBrace);
            gPresetToShapeTypeMap.Add("leftBracket", (int)ShapeType.LeftBracket);
            gPresetToShapeTypeMap.Add("leftRightArrow", (int)ShapeType.LeftRightArrow);
            gPresetToShapeTypeMap.Add("leftRightArrowCallout", (int)ShapeType.LeftRightArrowCallout);
            gPresetToShapeTypeMap.Add("lightningBolt", (int)ShapeType.LightningBolt);
            gPresetToShapeTypeMap.Add("line", (int)ShapeType.Line);
            gPresetToShapeTypeMap.Add("moon", (int)ShapeType.Moon);
            gPresetToShapeTypeMap.Add("noSmoking", (int)ShapeType.NoSmoking);
            gPresetToShapeTypeMap.Add("notchedRightArrow", (int)ShapeType.NotchedRightArrow);
            gPresetToShapeTypeMap.Add("octagon", (int)ShapeType.Octagon);
            gPresetToShapeTypeMap.Add("parallelogram", (int)ShapeType.Parallelogram);
            gPresetToShapeTypeMap.Add("pentagon", (int)ShapeType.Pentagon);
            gPresetToShapeTypeMap.Add("plaque", (int)ShapeType.Plaque);
            gPresetToShapeTypeMap.Add("plus", (int)ShapeType.Plus);
            gPresetToShapeTypeMap.Add("rect", (int)ShapeType.Rectangle);
            gPresetToShapeTypeMap.Add("ribbon", (int)ShapeType.Ribbon);
            gPresetToShapeTypeMap.Add("ribbon2", (int)ShapeType.Ribbon2);
            gPresetToShapeTypeMap.Add("rightArrow", (int)ShapeType.Arrow);
            gPresetToShapeTypeMap.Add("rightArrowCallout", (int)ShapeType.RightArrowCallout);
            gPresetToShapeTypeMap.Add("rightBrace", (int)ShapeType.RightBrace);
            gPresetToShapeTypeMap.Add("rightBracket", (int)ShapeType.RightBracket);
            gPresetToShapeTypeMap.Add("smileyFace", (int)ShapeType.SmileyFace);
            gPresetToShapeTypeMap.Add("star16", (int)ShapeType.Seal16);
            gPresetToShapeTypeMap.Add("star32", (int)ShapeType.Seal32);
            gPresetToShapeTypeMap.Add("star4", (int)ShapeType.Seal4);
            gPresetToShapeTypeMap.Add("star8", (int)ShapeType.Seal8);
            gPresetToShapeTypeMap.Add("straightConnector1", (int)ShapeType.StraightConnector1);
            gPresetToShapeTypeMap.Add("stripedRightArrow", (int)ShapeType.StripedRightArrow);
            gPresetToShapeTypeMap.Add("sun", (int)ShapeType.Sun);
            gPresetToShapeTypeMap.Add("triangle", (int)ShapeType.Triangle);
            gPresetToShapeTypeMap.Add("upArrow", (int)ShapeType.UpArrow);
            gPresetToShapeTypeMap.Add("upArrowCallout", (int)ShapeType.UpArrowCallout);
            gPresetToShapeTypeMap.Add("upDownArrow", (int)ShapeType.UpDownArrow);
            gPresetToShapeTypeMap.Add("upDownArrowCallout", (int)ShapeType.UpDownArrowCallout);
            gPresetToShapeTypeMap.Add("verticalScroll", (int)ShapeType.VerticalScroll);
            gPresetToShapeTypeMap.Add("wave", (int)ShapeType.Wave);
            gPresetToShapeTypeMap.Add("wedgeEllipseCallout", (int)ShapeType.WedgeEllipseCallout);
            gPresetToShapeTypeMap.Add("wedgeRectCallout", (int)ShapeType.WedgeRectCallout);
            gPresetToShapeTypeMap.Add("wedgeRoundRectCallout", (int)ShapeType.WedgeRRectCallout);
            gPresetToShapeTypeMap.Add("roundRect", (int)ShapeType.RoundRectangle);
            gPresetToShapeTypeMap.Add("rtTriangle", (int)ShapeType.RightTriangle);

            // MS Word writes fallback as a custom shape for these preset geometries.
            gPresetToShapeTypeMap.Add("quadArrowCallout", (int)ShapeType.QuadArrowCallout);
            gPresetToShapeTypeMap.Add("quadArrow", (int)ShapeType.QuadArrow);
            gPresetToShapeTypeMap.Add("leftUpArrow", (int)ShapeType.LeftUpArrow);
            gPresetToShapeTypeMap.Add("bentArrow", (int)ShapeType.BentArrow);
            gPresetToShapeTypeMap.Add("bentUpArrow", (int)ShapeType.BentUpArrow);
            gPresetToShapeTypeMap.Add("star5", (int)ShapeType.Star);
            gPresetToShapeTypeMap.Add("uturnArrow", (int)ShapeType.UturnArrow);
            gPresetToShapeTypeMap.Add("trapezoid", (int)ShapeType.Trapezoid);
            gPresetToShapeTypeMap.Add("leftRightUpArrow", (int)ShapeType.LeftRightUpArrow);
            gPresetToShapeTypeMap.Add("heart", (int)ShapeType.Heart);
            gPresetToShapeTypeMap.Add("circularArrow", (int)ShapeType.CircularArrow);
            gPresetToShapeTypeMap.Add("blockArc", (int)ShapeType.BlockArc);
            gPresetToShapeTypeMap.Add("arc", (int)ShapeType.Arc);

            // Actually geometries below have these mapping only for DML shapes. VML shapes have "NonPrimitive"
            // shapes types for geometries below.
            gPresetToShapeTypeMap.Add("heptagon", (int)ShapeType.Heptagon);
            gPresetToShapeTypeMap.Add("cloud", (int)ShapeType.Cloud);
            gPresetToShapeTypeMap.Add("star6", (int)ShapeType.Seal6);
            gPresetToShapeTypeMap.Add("star7", (int)ShapeType.Seal7);
            gPresetToShapeTypeMap.Add("star10", (int)ShapeType.Seal10);
            gPresetToShapeTypeMap.Add("star12", (int)ShapeType.Seal12);
            gPresetToShapeTypeMap.Add("star24", (int)ShapeType.Seal24);
            gPresetToShapeTypeMap.Add("swooshArrow", (int)ShapeType.SwooshArrow);
            gPresetToShapeTypeMap.Add("teardrop", (int)ShapeType.Teardrop);
            gPresetToShapeTypeMap.Add("squareTabs", (int)ShapeType.SquareTabs);
            gPresetToShapeTypeMap.Add("plaqueTabs", (int)ShapeType.PlaqueTabs);
            gPresetToShapeTypeMap.Add("pie", (int)ShapeType.Pie);
            gPresetToShapeTypeMap.Add("pieWedge", (int)ShapeType.WedgePie);
            gPresetToShapeTypeMap.Add("lineInv", (int)ShapeType.InverseLine);
            gPresetToShapeTypeMap.Add("mathDivide", (int)ShapeType.MathDivide);
            gPresetToShapeTypeMap.Add("mathEqual", (int)ShapeType.MathEqual);
            gPresetToShapeTypeMap.Add("mathMinus", (int)ShapeType.MathMinus);
            gPresetToShapeTypeMap.Add("mathMultiply", (int)ShapeType.MathMultiply);
            gPresetToShapeTypeMap.Add("mathNotEqual", (int)ShapeType.MathNotEqual);
            gPresetToShapeTypeMap.Add("mathPlus", (int)ShapeType.MathPlus);
            gPresetToShapeTypeMap.Add("nonIsoscelesTrapezoid", (int)ShapeType.NonIsoscelesTrapezoid);
            gPresetToShapeTypeMap.Add("leftRightCircularArrow", (int)ShapeType.LeftRightCircularArrow);
            gPresetToShapeTypeMap.Add("leftRightRibbon", (int)ShapeType.LeftRightRibbon);
            gPresetToShapeTypeMap.Add("leftCircularArrow", (int)ShapeType.LeftCircularArrow);
            gPresetToShapeTypeMap.Add("frame", (int)ShapeType.Frame);
            gPresetToShapeTypeMap.Add("funnel", (int)ShapeType.Funnel);
            gPresetToShapeTypeMap.Add("gear6", (int)ShapeType.Gear6);
            gPresetToShapeTypeMap.Add("gear9", (int)ShapeType.Gear9);
            gPresetToShapeTypeMap.Add("halfFrame", (int)ShapeType.HalfFrame);
            gPresetToShapeTypeMap.Add("decagon", (int)ShapeType.Decagon);
            gPresetToShapeTypeMap.Add("diagStripe", (int)ShapeType.DiagonalStripe);
            gPresetToShapeTypeMap.Add("dodecagon", (int)ShapeType.Dodecagon);
            gPresetToShapeTypeMap.Add("corner", (int)ShapeType.Corner);
            gPresetToShapeTypeMap.Add("cornerTabs", (int)ShapeType.CornerTabs);
            gPresetToShapeTypeMap.Add("chord", (int)ShapeType.Chord);
            gPresetToShapeTypeMap.Add("chartPlus", (int)ShapeType.ChartPlus);
            gPresetToShapeTypeMap.Add("chartStar", (int)ShapeType.ChartStar);
            gPresetToShapeTypeMap.Add("chartX", (int)ShapeType.ChartX);
            gPresetToShapeTypeMap.Add("snip1Rect", (int)ShapeType.SingleCornerSnipped);
            gPresetToShapeTypeMap.Add("snip2DiagRect", (int)ShapeType.DiagonalCornersSnipped);
            gPresetToShapeTypeMap.Add("snip2SameRect", (int)ShapeType.TopCornersSnipped);
            gPresetToShapeTypeMap.Add("snipRoundRect", (int)ShapeType.TopCornersOneRoundedOneSnipped);
            gPresetToShapeTypeMap.Add("round1Rect", (int)ShapeType.SingleCornerRounded);
            gPresetToShapeTypeMap.Add("round2DiagRect", (int)ShapeType.DiagonalCornersRounded);
            gPresetToShapeTypeMap.Add("round2SameRect", (int)ShapeType.TopCornersRounded);

            gShapeTypeToPresetMap.Add((int)ShapeType.AccentBorderCallout1, "accentBorderCallout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentBorderCallout2, "accentBorderCallout2");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentBorderCallout3, "accentBorderCallout3");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentCallout1, "accentCallout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentCallout2, "accentCallout2");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentCallout3, "accentCallout3");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonBackPrevious, "actionButtonBackPrevious");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonBeginning, "actionButtonBeginning");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonBlank, "actionButtonBlank");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonDocument, "actionButtonDocument");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonEnd, "actionButtonEnd");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonForwardNext, "actionButtonForwardNext");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonHelp, "actionButtonHelp");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonHome, "actionButtonHome");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonInformation, "actionButtonInformation");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonMovie, "actionButtonMovie");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonReturn, "actionButtonReturn");
            gShapeTypeToPresetMap.Add((int)ShapeType.ActionButtonSound, "actionButtonSound");
            gShapeTypeToPresetMap.Add((int)ShapeType.Arc, "arc");
            // rightArrow preset name are same for ThickArrow and Arrow.
            gShapeTypeToPresetMap.Add((int)ShapeType.ThickArrow, "rightArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Arrow, "rightArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.BentArrow, "bentArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.BentConnector2, "bentConnector2");
            gShapeTypeToPresetMap.Add((int)ShapeType.BentConnector3, "bentConnector3");
            gShapeTypeToPresetMap.Add((int)ShapeType.BentConnector4, "bentConnector4");
            gShapeTypeToPresetMap.Add((int)ShapeType.BentConnector5, "bentConnector5");
            gShapeTypeToPresetMap.Add((int)ShapeType.BentUpArrow, "bentUpArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Bevel, "bevel");
            gShapeTypeToPresetMap.Add((int)ShapeType.BlockArc, "blockArc");
            // borderCallout1 preset name are same for BorderCallout1, BorderCallout90 and AccentBorderCallout90.
            gShapeTypeToPresetMap.Add((int)ShapeType.BorderCallout1, "borderCallout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.BorderCallout90, "borderCallout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentBorderCallout90, "borderCallout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.BorderCallout2, "borderCallout2");
            gShapeTypeToPresetMap.Add((int)ShapeType.BorderCallout3, "borderCallout3");
            gShapeTypeToPresetMap.Add((int)ShapeType.BracePair, "bracePair");
            gShapeTypeToPresetMap.Add((int)ShapeType.BracketPair, "bracketPair");
            // callout1 preset name are same for Callout1, AccentCallout90 and Callout90.
            gShapeTypeToPresetMap.Add((int)ShapeType.Callout1, "callout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.Callout90, "callout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.AccentCallout90, "callout1");
            gShapeTypeToPresetMap.Add((int)ShapeType.Callout2, "callout2");
            gShapeTypeToPresetMap.Add((int)ShapeType.Callout3, "callout3");
            gShapeTypeToPresetMap.Add((int)ShapeType.Can, "can");
            gShapeTypeToPresetMap.Add((int)ShapeType.Chevron, "chevron");
            gShapeTypeToPresetMap.Add((int)ShapeType.CircularArrow, "circularArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.CloudCallout, "cloudCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.Cube, "cube");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedConnector2, "curvedConnector2");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedConnector3, "curvedConnector3");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedConnector4, "curvedConnector4");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedConnector5, "curvedConnector5");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedDownArrow, "curvedDownArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedLeftArrow, "curvedLeftArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedRightArrow, "curvedRightArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.CurvedUpArrow, "curvedUpArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Diamond, "diamond");
            gShapeTypeToPresetMap.Add((int)ShapeType.Donut, "donut");
            gShapeTypeToPresetMap.Add((int)ShapeType.DoubleWave, "doubleWave");
            gShapeTypeToPresetMap.Add((int)ShapeType.DownArrow, "downArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.DownArrowCallout, "downArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.Ellipse, "ellipse");
            gShapeTypeToPresetMap.Add((int)ShapeType.EllipseRibbon, "ellipseRibbon");
            gShapeTypeToPresetMap.Add((int)ShapeType.EllipseRibbon2, "ellipseRibbon2");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartAlternateProcess, "flowChartAlternateProcess");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartCollate, "flowChartCollate");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartConnector, "flowChartConnector");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartDecision, "flowChartDecision");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartDelay, "flowChartDelay");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartDisplay, "flowChartDisplay");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartDocument, "flowChartDocument");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartExtract, "flowChartExtract");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartInputOutput, "flowChartInputOutput");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartInternalStorage, "flowChartInternalStorage");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartMagneticDisk, "flowChartMagneticDisk");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartMagneticDrum, "flowChartMagneticDrum");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartMagneticTape, "flowChartMagneticTape");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartManualInput, "flowChartManualInput");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartManualOperation, "flowChartManualOperation");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartMerge, "flowChartMerge");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartMultidocument, "flowChartMultidocument");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartOfflineStorage, "flowChartOfflineStorage");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartOffpageConnector, "flowChartOffpageConnector");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartOnlineStorage, "flowChartOnlineStorage");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartOr, "flowChartOr");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartPredefinedProcess, "flowChartPredefinedProcess");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartPreparation, "flowChartPreparation");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartProcess, "flowChartProcess");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartPunchedCard, "flowChartPunchedCard");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartPunchedTape, "flowChartPunchedTape");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartSort, "flowChartSort");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartSummingJunction, "flowChartSummingJunction");
            gShapeTypeToPresetMap.Add((int)ShapeType.FlowChartTerminator, "flowChartTerminator");
            gShapeTypeToPresetMap.Add((int)ShapeType.FoldedCorner, "foldedCorner");
            gShapeTypeToPresetMap.Add((int)ShapeType.Heart, "heart");
            gShapeTypeToPresetMap.Add((int)ShapeType.Hexagon, "hexagon");
            gShapeTypeToPresetMap.Add((int)ShapeType.HomePlate, "homePlate");
            gShapeTypeToPresetMap.Add((int)ShapeType.HorizontalScroll, "horizontalScroll");
            gShapeTypeToPresetMap.Add((int)ShapeType.IrregularSeal1, "irregularSeal1");
            gShapeTypeToPresetMap.Add((int)ShapeType.IrregularSeal2, "irregularSeal2");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftArrow, "leftArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftArrowCallout, "leftArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftBrace, "leftBrace");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftBracket, "leftBracket");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftRightArrow, "leftRightArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftRightArrowCallout, "leftRightArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftRightUpArrow, "leftRightUpArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftUpArrow, "leftUpArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.LightningBolt, "lightningBolt");
            gShapeTypeToPresetMap.Add((int)ShapeType.Line, "line");
            gShapeTypeToPresetMap.Add((int)ShapeType.Moon, "moon");
            gShapeTypeToPresetMap.Add((int)ShapeType.NoSmoking, "noSmoking");
            gShapeTypeToPresetMap.Add((int)ShapeType.NotchedRightArrow, "notchedRightArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Octagon, "octagon");
            gShapeTypeToPresetMap.Add((int)ShapeType.Parallelogram, "parallelogram");
            gShapeTypeToPresetMap.Add((int)ShapeType.Pentagon, "pentagon");
            gShapeTypeToPresetMap.Add((int)ShapeType.Plaque, "plaque");
            gShapeTypeToPresetMap.Add((int)ShapeType.Plus, "plus");
            gShapeTypeToPresetMap.Add((int)ShapeType.QuadArrow, "quadArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.QuadArrowCallout, "quadArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.Rectangle, "rect");
            gShapeTypeToPresetMap.Add((int)ShapeType.Ribbon, "ribbon");
            gShapeTypeToPresetMap.Add((int)ShapeType.Ribbon2, "ribbon2");
            gShapeTypeToPresetMap.Add((int)ShapeType.RightArrowCallout, "rightArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.RightBrace, "rightBrace");
            gShapeTypeToPresetMap.Add((int)ShapeType.RightBracket, "rightBracket");
            gShapeTypeToPresetMap.Add((int)ShapeType.RoundRectangle, "roundRect");
            gShapeTypeToPresetMap.Add((int)ShapeType.RightTriangle, "rtTriangle");
            gShapeTypeToPresetMap.Add((int)ShapeType.SmileyFace, "smileyFace");
            gShapeTypeToPresetMap.Add((int)ShapeType.Star, "star5");
            gShapeTypeToPresetMap.Add((int)ShapeType.StraightConnector1, "straightConnector1");
            gShapeTypeToPresetMap.Add((int)ShapeType.StripedRightArrow, "stripedRightArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Sun, "sun");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal, "star16");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal4, "star4");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal8, "star8");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal16, "star16");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal24, "star24");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal32, "star32");
            gShapeTypeToPresetMap.Add((int)ShapeType.Trapezoid, "trapezoid"); // In MSW it is custGeom that is why the angles are different.
            gShapeTypeToPresetMap.Add((int)ShapeType.Triangle, "triangle");
            gShapeTypeToPresetMap.Add((int)ShapeType.UpArrow, "upArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.UpArrowCallout, "upArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.UpDownArrow, "upDownArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.UpDownArrowCallout, "upDownArrowCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.UturnArrow, "uturnArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.VerticalScroll, "verticalScroll");
            gShapeTypeToPresetMap.Add((int)ShapeType.Wave, "wave");
            gShapeTypeToPresetMap.Add((int)ShapeType.WedgeEllipseCallout, "wedgeEllipseCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.WedgeRectCallout, "wedgeRectCallout");
            // wedgeRoundRectCallout preset name are same for WedgeRRectCallout and Balloon.
            gShapeTypeToPresetMap.Add((int)ShapeType.WedgeRRectCallout, "wedgeRoundRectCallout");
            gShapeTypeToPresetMap.Add((int)ShapeType.Balloon, "wedgeRoundRectCallout");
            // The following types can not be set for VML shape. These shape types has
            // to be converted to "NonPrimitive" shape type with custom geometry.
            gShapeTypeToPresetMap.Add((int)ShapeType.Heptagon, "heptagon");
            gShapeTypeToPresetMap.Add((int)ShapeType.Cloud, "cloud");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal6, "star6");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal7, "star7");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal10, "star10");
            gShapeTypeToPresetMap.Add((int)ShapeType.Seal12, "star12");
            gShapeTypeToPresetMap.Add((int)ShapeType.SwooshArrow, "swooshArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Teardrop, "teardrop");
            gShapeTypeToPresetMap.Add((int)ShapeType.SquareTabs, "squareTabs");
            gShapeTypeToPresetMap.Add((int)ShapeType.PlaqueTabs, "plaqueTabs");
            gShapeTypeToPresetMap.Add((int)ShapeType.Pie, "pie");
            gShapeTypeToPresetMap.Add((int)ShapeType.WedgePie, "pieWedge");
            gShapeTypeToPresetMap.Add((int)ShapeType.InverseLine, "lineInv");
            gShapeTypeToPresetMap.Add((int)ShapeType.MathDivide, "mathDivide");
            gShapeTypeToPresetMap.Add((int)ShapeType.MathEqual, "mathEqual");
            gShapeTypeToPresetMap.Add((int)ShapeType.MathMinus, "mathMinus");
            gShapeTypeToPresetMap.Add((int)ShapeType.MathMultiply, "mathMultiply");
            gShapeTypeToPresetMap.Add((int)ShapeType.MathNotEqual, "mathNotEqual");
            gShapeTypeToPresetMap.Add((int)ShapeType.MathPlus, "mathPlus");
            gShapeTypeToPresetMap.Add((int)ShapeType.NonIsoscelesTrapezoid, "nonIsoscelesTrapezoid");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftRightCircularArrow, "leftRightCircularArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftRightRibbon, "leftRightRibbon");
            gShapeTypeToPresetMap.Add((int)ShapeType.LeftCircularArrow, "leftCircularArrow");
            gShapeTypeToPresetMap.Add((int)ShapeType.Frame, "frame");
            gShapeTypeToPresetMap.Add((int)ShapeType.Funnel, "funnel");
            gShapeTypeToPresetMap.Add((int)ShapeType.Gear6, "gear6");
            gShapeTypeToPresetMap.Add((int)ShapeType.Gear9, "gear9");
            gShapeTypeToPresetMap.Add((int)ShapeType.HalfFrame, "halfFrame");
            gShapeTypeToPresetMap.Add((int)ShapeType.Decagon, "decagon");
            gShapeTypeToPresetMap.Add((int)ShapeType.DiagonalStripe, "diagStripe");
            gShapeTypeToPresetMap.Add((int)ShapeType.Dodecagon, "dodecagon");
            gShapeTypeToPresetMap.Add((int)ShapeType.Corner, "corner");
            gShapeTypeToPresetMap.Add((int)ShapeType.CornerTabs, "cornerTabs");
            gShapeTypeToPresetMap.Add((int)ShapeType.Chord, "chord");
            gShapeTypeToPresetMap.Add((int)ShapeType.ChartPlus, "chartPlus");
            gShapeTypeToPresetMap.Add((int)ShapeType.ChartStar, "chartStar");
            gShapeTypeToPresetMap.Add((int)ShapeType.ChartX, "chartX");
            gShapeTypeToPresetMap.Add((int)ShapeType.SingleCornerSnipped, "snip1Rect");
            gShapeTypeToPresetMap.Add((int)ShapeType.TopCornersSnipped, "snip2SameRect");
            gShapeTypeToPresetMap.Add((int)ShapeType.DiagonalCornersSnipped, "snip2DiagRect");
            gShapeTypeToPresetMap.Add((int)ShapeType.TopCornersOneRoundedOneSnipped, "snipRoundRect");
            gShapeTypeToPresetMap.Add((int)ShapeType.SingleCornerRounded, "round1Rect");
            gShapeTypeToPresetMap.Add((int)ShapeType.TopCornersRounded, "round2SameRect");
            gShapeTypeToPresetMap.Add((int)ShapeType.DiagonalCornersRounded, "round2DiagRect");

            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextSimple, (int)DmlTextShapeType.TextPlain);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextOctagon, (int)DmlTextShapeType.TextStop);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextHexagon, (int)DmlTextShapeType.TextTriangle);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCurve, (int)DmlTextShapeType.TextCascadeDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextWave, (int)DmlTextShapeType.TextWave1);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextRing, (int)DmlTextShapeType.TextArchUpPour);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextOnCurve, (int)DmlTextShapeType.TextCascadeDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextOnRing, (int)DmlTextShapeType.TextArchUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextPlainText, (int)DmlTextShapeType.TextPlain);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextStop, (int)DmlTextShapeType.TextStop);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextTriangle, (int)DmlTextShapeType.TextTriangle);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextTriangleInverted, (int)DmlTextShapeType.TextTriangleInverted);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextChevron, (int)DmlTextShapeType.TextChevron);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextChevronInverted, (int)DmlTextShapeType.TextChevronInverted);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextRingInside, (int)DmlTextShapeType.TextRingInside);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextRingOutside, (int)DmlTextShapeType.TextRingOutside);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextArchUpCurve, (int)DmlTextShapeType.TextArchUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextArchDownCurve, (int)DmlTextShapeType.TextArchDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCircleCurve, (int)DmlTextShapeType.TextCircle);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextButtonCurve, (int)DmlTextShapeType.TextButton);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextArchUpPour, (int)DmlTextShapeType.TextArchUpPour);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextArchDownPour, (int)DmlTextShapeType.TextArchDownPour);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCirclePour, (int)DmlTextShapeType.TextCirclePour);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextButtonPour, (int)DmlTextShapeType.TextButtonPour);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCurveUp, (int)DmlTextShapeType.TextCurveUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCurveDown, (int)DmlTextShapeType.TextCurveDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCascadeUp, (int)DmlTextShapeType.TextCascadeUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCascadeDown, (int)DmlTextShapeType.TextCanDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextWave1, (int)DmlTextShapeType.TextWave1);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextWave2, (int)DmlTextShapeType.TextWave2);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextWave3, (int)DmlTextShapeType.TextDoubleWave1);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextWave4, (int)DmlTextShapeType.TextWave4);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextInflate, (int)DmlTextShapeType.TextInflate);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextDeflate, (int)DmlTextShapeType.TextDeflate);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextInflateBottom, (int)DmlTextShapeType.TextInflateBottom);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextDeflateBottom, (int)DmlTextShapeType.TextDeflateBottom);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextInflateTop, (int)DmlTextShapeType.TextInflateTop);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextDeflateTop, (int)DmlTextShapeType.TextDeflateTop);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextDeflateInflate, (int)DmlTextShapeType.TextDeflateInflate);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextDeflateInflateDeflate, (int)DmlTextShapeType.TextDeflateInflateDeflate);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextFadeRight, (int)DmlTextShapeType.TextFadeRight);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextFadeLeft, (int)DmlTextShapeType.TextFadeLeft);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextFadeUp, (int)DmlTextShapeType.TextFadeUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextFadeDown, (int)DmlTextShapeType.TextFadeDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextSlantUp, (int)DmlTextShapeType.TextSlantUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextSlantDown, (int)DmlTextShapeType.TextSlantDown);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCanUp, (int)DmlTextShapeType.TextCanUp);
            gShapeTypeToPresetTextWrapTypeMap.Add((int)ShapeType.TextCanDown, (int)DmlTextShapeType.TextCascadeDown);
        }

        private static readonly IntToIntDictionary gShapeTypeToPresetTextWrapTypeMap = new IntToIntDictionary();

        // There are two collections because there is not only one to one relations between ShapeType and Preset geometry name.
        private static readonly IntToObjDictionary<string> gShapeTypeToPresetMap = new IntToObjDictionary<string>();
        private static readonly StringToIntDictionary gPresetToShapeTypeMap = new StringToIntDictionary();

        private static readonly StringToIntBidirectionalMap gBWModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gShapeLineStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEndCapMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeHorizontalSizeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeVerticalSizeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeHorizontalPositionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRelativeVerticalPositionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gHorizontalAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gVerticalAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gWrapSideMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gSchemeColorMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gDashStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gArrowWidthMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gArrowLengthMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gArrowTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTileFlipModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPathShadeType = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLineEndingCapTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCompoundLineTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPresetDashTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextOrientationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextVerticalOverflowTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextHorizontalOverflowTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextAnchoringTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextWrappingTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gFontAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCapitalizationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextStrikeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextUnderlineTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPatternTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gRectangleAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCompressionStateMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEffectBlendModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPathFillModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPresetShadowMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gFontCollectionIndexMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextShapeTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCameraTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLightRigDirectionMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLightRigTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPresetMaterialMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gBevelPresetTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextBulletTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTabAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEffectDagTypeMap = new StringToIntBidirectionalMap();
    }
}
