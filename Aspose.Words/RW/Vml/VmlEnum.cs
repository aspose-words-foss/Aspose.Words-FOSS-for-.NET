// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2006 by Vladimir Averkin

using System;
using System.Drawing.Drawing2D;
using Aspose.Collections;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.Images.Pal;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using DashStyle = Aspose.Words.Drawing.DashStyle;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Converts shape enumerated types between enum and WML string.
    /// </summary>
    internal class VmlEnum
    {
        internal static ScriptLanguage VmlToScriptLanguage(string value)
        {
            return (ScriptLanguage)gScriptLanguageMap.GetValue(value, (int)ScriptLanguage.None);
        }

        internal static string ScriptLanguageToVml(ScriptLanguage value)
        {
            return gScriptLanguageMap.GetValue((int)value, "");
        }

        internal static string ScriptLanguageToVml(object value)
        {
            // "JavaScript" is a default value.
            if (value == null)
                return "JavaScript";

            if (!(value is int))
                throw new InvalidOperationException("Invalid value for script language attribute.");

            return ScriptLanguageToVml((ScriptLanguage)(int)value);
        }

        internal static Operation VmlToOperation(string value)
        {
            return (Operation)gOperationMap.GetValue(value, (int)Operation.Sum);
        }

        internal static string OperationToVml(Operation value)
        {
            return gOperationMap.GetValue((int)value, "");
        }

        internal static BWMode VmlToBWMode(string value)
        {
            return (BWMode)gBWModeMap.GetValue(value, (int)BWMode.Automatic);
        }

        internal static string BWModeToVml(BWMode value)
        {
            return gBWModeMap.GetValue((int)value, "");
        }

        internal static OleLinkType VmlToOleLinkType(string value)
        {
            return (OleLinkType)gOleLinkTypeMap.GetValue(value, (int)OleLinkType.Default);
        }

        internal static string OleLinkTypeToVml(OleLinkType value)
        {
            return gOleLinkTypeMap.GetValue((int)value, "");
        }

        internal static HorizontalRuleAlignment VmlToHRAlign(string value)
        {
            return (HorizontalRuleAlignment)gHRAlignMap.GetValue(value, (int)HorizontalRuleAlignment.Left);
        }

        internal static string HRAlignToVml(HorizontalRuleAlignment value)
        {
            return gHRAlignMap.GetValue((int)value, "");
        }

        internal static TextPathAlignment VmlToTextPathAlignment(string value)
        {
            return (TextPathAlignment)gTextPathAlignmentMap.GetValue(value, (int)TextPathAlignment.Left);
        }

        internal static string TextPathAlignmentToVml(TextPathAlignment value)
        {
            return gTextPathAlignmentMap.GetValue((int)value, "");
        }

        internal static ConnectionSiteType VmlToConnectionSiteType(string value)
        {
            return (ConnectionSiteType)gConnectionSiteTypeMap.GetValue(value, (int)ConnectionSiteType.None);
        }

        internal static string ConnectionSiteTypeToVml(ConnectionSiteType value)
        {
            return gConnectionSiteTypeMap.GetValue((int)value, "");
        }

        internal static LineStyle VmlToLineStyle(string value)
        {
            return (LineStyle)gLineStyleMap.GetValue(value, (int)LineStyle.None);
        }

        internal static string LineStyleToVml(LineStyle value)
        {
            return gLineStyleMap.GetValue((int)value, "");
        }

        internal static CalloutDropType VmlToCalloutDropType(string value)
        {
            return (CalloutDropType)gCalloutDropTypeMap.GetValue(value, (int)CalloutDropType.Specified);
        }

        internal static string CalloutDropTypeToVml(CalloutDropType value)
        {
            return gCalloutDropTypeMap.GetValue((int)value, "");
        }

        internal static CalloutAngle VmlToCalloutAngle(string value)
        {
            return (CalloutAngle)gCalloutAngleMap.GetValue(value, (int)CalloutAngle.Any);
        }

        internal static string CalloutAngleToVml(CalloutAngle value)
        {
            return gCalloutAngleMap.GetValue((int)value, "");
        }

        internal static DashStyle VmlToDashStyle(string value)
        {
            return (DashStyle)gDashStyleMap.GetValue(value, (int)DashStyle.Solid);
        }

        internal static string DashStyleToVml(DashStyle value)
        {
            return gDashStyleMap.GetValue((int)value, "");
        }

        internal static JoinStyle VmlToJoinStyle(string value)
        {
            return (JoinStyle)gJoinStyleMap.GetValue(value, (int)JoinStyle.Round);
        }

        internal static string JoinStyleToVml(JoinStyle value)
        {
            return gJoinStyleMap.GetValue((int)value, "");
        }

        internal static ShapeLineStyle VmlToShapeLineStyle(string value)
        {
            return (ShapeLineStyle)gShapeLineStyleMap.GetValue(value, (int)ShapeLineStyle.Single);
        }

        internal static string ShapeLineStyleToVml(ShapeLineStyle value)
        {
            return gShapeLineStyleMap.GetValue((int)value, "");
        }

        internal static EndCap VmlToEndCap(string value)
        {
            return (EndCap)gEndCapMap.GetValue(value, (int)EndCap.Flat);
        }

        internal static string EndCapToVml(EndCap value)
        {
            return gEndCapMap.GetValue((int)value, "");
        }

        internal static ArrowWidth VmlToArrowWidth(string value)
        {
            return (ArrowWidth)gArrowWidthMap.GetValue(value, (int)ArrowWidth.Medium);
        }

        internal static string ArrowWidthToVml(ArrowWidth value)
        {
            return gArrowWidthMap.GetValue((int)value, "");
        }

        internal static ArrowLength VmlToArrowLength(string value)
        {
            return (ArrowLength)gArrowLengthMap.GetValue(value, (int)ArrowLength.Medium);
        }

        internal static string ArrowLengthToVml(ArrowLength value)
        {
            return gArrowLengthMap.GetValue((int)value, "");
        }

        internal static ArrowType VmlToArrowType(string value)
        {
            return (ArrowType)gArrowTypeMap.GetValue(value, (int)ArrowType.None);
        }

        internal static string ArrowTypeToVml(ArrowType value)
        {
            return gArrowTypeMap.GetValue((int)value, "");
        }

        internal static LineFillType VmlToLineFillType(string value)
        {
            return (LineFillType)gLineFillTypeMap.GetValue(value, (int)LineFillType.Solid);
        }

        internal static string LineFillTypeToVml(LineFillType value)
        {
            return gLineFillTypeMap.GetValue((int)value, "");
        }

        internal static PlaneType VmlToPlaneType(string value)
        {
            return (PlaneType)gPlaneTypeMap.GetValue(value, (int)PlaneType.XY);
        }

        internal static string PlaneTypeToVml(PlaneType value)
        {
            return gPlaneTypeMap.GetValue((int)value, "");
        }

        internal static ThreeDRenderMode VmlToThreeDRenderMode(string value)
        {
            return (ThreeDRenderMode)gThreeDRenderModeMap.GetValue(value, (int)ThreeDRenderMode.FullRender);
        }

        internal static string ThreeDRenderModeToVml(ThreeDRenderMode value)
        {
            return gThreeDRenderModeMap.GetValue((int)value, "");
        }

        internal static ShadowTypeCore VmlToShadowType(string value)
        {
            return (ShadowTypeCore)gShadowTypeMap.GetValue(value, (int)ShadowTypeCore.Offset);
        }

        internal static string ShadowTypeToVml(ShadowTypeCore value)
        {
            return gShadowTypeMap.GetValue((int)value, "");
        }

        internal static CalloutType VmlToCalloutType(string value)
        {
            return (CalloutType)gCalloutTypeMap.GetValue(value, (int)CalloutType.OneSegment);
        }

        internal static string CalloutTypeToVml(CalloutType value)
        {
            return gCalloutTypeMap.GetValue((int)value, "");
        }

        internal static LayoutFlow VmlToLayoutFlow(string value)
        {
            return (LayoutFlow)gLayoutFlowMap.GetValue(value, (int)LayoutFlow.Horizontal);
        }

        internal static string LayoutFlowToVml(LayoutFlow value)
        {
            return gLayoutFlowMap.GetValue((int)value, "");
        }

        internal static TextBoxAnchor VmlToTextboxAnchor(string value)
        {
            return (TextBoxAnchor)gTextboxAnchorMap.GetValue(value, (int)TextBoxAnchor.Top);
        }

        internal static string TextboxAnchorToVml(TextBoxAnchor value)
        {
            return gTextboxAnchorMap.GetValue((int)value, "");
        }

        internal static TextBoxWrapMode VmlToTextboxWrapMode(string value)
        {
            // WORDSNET-10078.
            // TextBoxWrapMode.Square seems to be the default value for MW.
            return (TextBoxWrapMode)gTextboxWrapModeMap.GetValue(value, (int)TextBoxWrapMode.Square);
        }

        internal static string TextboxWrapModeToVml(TextBoxWrapMode value)
        {
            return gTextboxWrapModeMap.GetValue((int)value, "");
        }

        internal static WrapSide VmlToWrapSide(string value)
        {
            return (WrapSide)gWrapSideMap.GetValue(value, (int)WrapSide.Both);
        }

        internal static string WrapSideToVml(WrapSide value)
        {
            return gWrapSideMap.GetValue((int)value, "");
        }

        internal static ConnectorType VmlToConnectorType(string value)
        {
            return (ConnectorType)gConnectorTypeMap.GetValue(value, (int)ConnectorType.None);
        }

        internal static string ConnectorTypeToVml(ConnectorType value)
        {
            return gConnectorTypeMap.GetValue((int)value, "");
        }

        internal static string DzTypeToVml(FillDimensionType value)
        {
            switch (value)
            {
                case FillDimensionType.FixedRatioEnlarge:
                    return "atLeast";
                case FillDimensionType.FixedAspect:
                    return "atMost";
                default:
                    return "";
            }
        }

        //JAVA: declarations moved here to exclude java's illegal forward reference.

        private static readonly StringToIntBidirectionalMap gScriptLanguageMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gOperationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gBWModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gOleLinkTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gHRAlignMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextPathAlignmentMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gConnectionSiteTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLineStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCalloutDropTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCalloutAngleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gDashStyleMap = new StringToIntBidirectionalMap(false);
        private static readonly StringToIntBidirectionalMap gJoinStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gShapeLineStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEndCapMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gArrowWidthMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gArrowLengthMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gArrowTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLineFillTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPlaneTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gThreeDRenderModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gShadowTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gCalloutTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gLayoutFlowMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextboxAnchorMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gTextboxWrapModeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gWrapSideMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gConnectorTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gEditAsMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gFlipOrientationMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gWrapTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gPathTypeMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gVmlHatchStyleMap = new StringToIntBidirectionalMap();
        private static readonly StringToIntBidirectionalMap gVmlFillBlipNameMap = new StringToIntBidirectionalMap();
        private static readonly IntToIntBidirectionalMap gLocksMap = new IntToIntBidirectionalMap();

        internal static FillDimensionType VmlToDzType(string value)
        {
            switch (value)
            {
                case "atLeast":
                    return FillDimensionType.FixedRatioEnlarge;
                case "atMost":
                    return FillDimensionType.FixedAspect;
                default:
                    return FillDimensionType.Default;
            }
        }

        internal static EditAs VmlToEditAs(string value)
        {
            return (EditAs)gEditAsMap.GetValue(value, (int)EditAs.Group);
        }

        internal static string EditAsToVml(EditAs value)
        {
            return gEditAsMap.GetValue((int)value, "");
        }

        internal static string FillTypeToVml(FillTypeCore value)
        {
            // We don't use EnumUtil here, because the relationship is not one-to-one.
            switch (value)
            {
                case FillTypeCore.Solid:
                    return "solid";
                case FillTypeCore.ShadeShape:
                case FillTypeCore.ShadeCenter:
                    return "gradientRadial";
                case FillTypeCore.ShadeScale:
                case FillTypeCore.ShadeUnscale:
                // WORDSNET-18702 The analogue of FillType.Shade in VML is fill type 'gradient'.
                case FillTypeCore.Shade:
                    return "gradient";
                case FillTypeCore.Pattern:
                    return "pattern";
                case FillTypeCore.Picture:
                    return "frame";
                case FillTypeCore.Texture:
                    return "tile";
                default:
                    return "";
            }
        }

        internal static FillTypeCore VmlToFillType(string value)
        {
            // We don't use EnumUtil here, because the relationship is not one-to-one.
            switch (value)
            {
                case "solid":
                    return FillTypeCore.Solid;
                case "gradientRadial":
                    // Can also be ShadeCenter, to find out additionally check o:fill child of v:fill element.
                    return FillTypeCore.ShadeShape;
                case "gradient":
                    // andrnosk: WORDSNET-4146 Can also be ShadeUnscale, to find out additionally check o:fill child of v:fill element.
                    return FillTypeCore.ShadeScale;
                case "pattern":
                    return FillTypeCore.Pattern;
                case "frame":
                    return FillTypeCore.Picture;
                case "tile":
                    return FillTypeCore.Texture;
                default:
                    return FillTypeCore.Solid;
            }
        }

        internal static FlipOrientation VmlToFlipOrientation(string value)
        {
            return (FlipOrientation)gFlipOrientationMap.GetValue(value, (int)FlipOrientation.None);
        }

        internal static string FlipOrientationToVml(FlipOrientation value)
        {
            return gFlipOrientationMap.GetValue((int)value, "");
        }

        internal static WrapType VmlToWrapType(string value)
        {
            return (WrapType)gWrapTypeMap.GetValue(value, (int)WrapType.None);
        }

        internal static string WrapTypeToVml(WrapType value)
        {
            // WrapType.None and WrapType.Inline are specified via the shape "position".
            return (value == WrapType.None || value == WrapType.Inline)
                ? string.Empty
                : gWrapTypeMap.GetValue((int)value, "");
        }

        internal static PathType VmlToPathType(string value)
        {
            return (PathType)gPathTypeMap.GetValue(value, (int)PathType.Unknown);
        }

        internal static string PathTypeToVml(PathType value)
        {
            return gPathTypeMap.GetValue((int)value, "");
        }

        internal static string HatchStyleToVml(HatchStyle value)
        {
            if (value == HatchStyle.LargeGrid)
                return gLGridFillName;

            return gVmlHatchStyleMap.GetValue((int)value, "");
        }

        /// <summary>
        /// Determines hatch style using specified image data.
        /// </summary>
        /// <param name="binData">Image data to determine hatch style.</param>
        /// <returns>Hatch style.</returns>
        internal static HatchStyle GetHatchStyle(byte[] binData)
        {
            if (binData == null)
                return HatchStyle.Min;

            DrPixels pixels;

            // Use colors of the bitonal image to determine hatch style.
            using (BitmapPal bitmap = new BitmapPal(binData))
                pixels = bitmap.GetPixels(true);

            // alexnosk: In netstandard, .net and mono different algorithms are used to calculate hash code of Guid.
            // This leads into different key values. To fix this use code from .NET to calculate hash code of Guid.
            int hash = HashUtil.GetSHA512Hash(pixels.ColorValues).GetHashCode();
            string fillName = gVmlFillBlipNameMap.GetValue(hash, "").ToLower();

            if (fillName == gLGridFillName)
                return HatchStyle.LargeGrid;

            return (HatchStyle)gVmlHatchStyleMap.GetValue(fillName, (int)HatchStyle.Min);
        }

        /// <summary>
        /// Determines PatternType using specified image data.
        /// </summary>
        /// <param name="binData">Image data to determine hatch style.</param>
        /// <returns><see cref="PatternType"/></returns>
        internal static PatternType GetPatternType(byte[] binData)
        {
            HatchStyle style = GetHatchStyle(binData);
            return FillUtil.HatchStyleToPatternType(style);
        }

        internal static DmlLock VmlToDmlLock(int lockKey)
        {
            return (DmlLock)gLocksMap.GetValueDirect(lockKey, (int)DmlLock.NoChangeAspect);
        }

        /// <summary>
        /// Add values to hatch mapping collections.
        /// </summary>
        /// <param name="fName">Hatch name.</param>
        /// <param name="hStyle">Hatch value.</param>
        /// <param name="hash">Hash code of the gif image for specified hatch style name.</param>
        private static void AddToFillMap(string fName, HatchStyle hStyle, int hash)
        {
            if (fName.ToLower() != gLGridFillName)
                gVmlHatchStyleMap.AddEntry(fName, (int)hStyle);

            if (hash != 0)
                gVmlFillBlipNameMap.AddEntry(fName, hash);
        }

        static VmlEnum()
        {
            gWrapTypeMap.AddEntry("none", (int)WrapType.None);
            gWrapTypeMap.AddEntry("inline", (int)WrapType.Inline);
            gWrapTypeMap.AddEntry("square", (int)WrapType.Square);
            gWrapTypeMap.AddEntry("tight", (int)WrapType.Tight);
            gWrapTypeMap.AddEntry("topAndBottom", (int)WrapType.TopBottom);
            gWrapTypeMap.AddEntry("through", (int)WrapType.Through);

            gFlipOrientationMap.AddEntry("x", (int)FlipOrientation.Horizontal);
            gFlipOrientationMap.AddEntry("y", (int)FlipOrientation.Vertical);
            gFlipOrientationMap.AddEntry("x y", (int)FlipOrientation.Both);

            gEditAsMap.AddEntry("canvas", (int)EditAs.Canvas);
            gEditAsMap.AddEntry("orgchart", (int)EditAs.OrganizationalChart);
            gEditAsMap.AddEntry("radial", (int)EditAs.Radial);
            gEditAsMap.AddEntry("cycle", (int)EditAs.Cycle);
            gEditAsMap.AddEntry("stacked", (int)EditAs.Pyramid);
            gEditAsMap.AddEntry("venn", (int)EditAs.Venn);
            gEditAsMap.AddEntry("bullseye", (int)EditAs.Target);

            gConnectorTypeMap.AddEntry("none", (int)ConnectorType.None);
            gConnectorTypeMap.AddEntry("straight", (int)ConnectorType.Straight);
            gConnectorTypeMap.AddEntry("elbow", (int)ConnectorType.Bent);
            gConnectorTypeMap.AddEntry("curved", (int)ConnectorType.Curved);

            gWrapSideMap.AddEntry("", (int)WrapSide.Both);
            gWrapSideMap.AddEntry("left", (int)WrapSide.Left);
            gWrapSideMap.AddEntry("right", (int)WrapSide.Right);
            gWrapSideMap.AddEntry("largest", (int)WrapSide.Largest);

            gTextboxWrapModeMap.AddEntry("none", (int)TextBoxWrapMode.None);
            gTextboxWrapModeMap.AddEntry("square", (int)TextBoxWrapMode.Square);

            gTextboxAnchorMap.AddEntry("top", (int)TextBoxAnchor.Top);
            gTextboxAnchorMap.AddEntry("middle", (int)TextBoxAnchor.Middle);
            gTextboxAnchorMap.AddEntry("bottom", (int)TextBoxAnchor.Bottom);
            gTextboxAnchorMap.AddEntry("top-center", (int)TextBoxAnchor.TopCentered);
            gTextboxAnchorMap.AddEntry("middle-center", (int)TextBoxAnchor.MiddleCentered);
            gTextboxAnchorMap.AddEntry("bottom-center", (int)TextBoxAnchor.BottomCentered);
            gTextboxAnchorMap.AddEntry("top-baseline", (int)TextBoxAnchor.TopBaseline);
            gTextboxAnchorMap.AddEntry("bottom-baseline", (int)TextBoxAnchor.BottomBaseline);
            gTextboxAnchorMap.AddEntry("top-center-baseline", (int)TextBoxAnchor.TopCenteredBaseline);
            gTextboxAnchorMap.AddEntry("bottom-center-baseline", (int)TextBoxAnchor.BottomCenteredBaseline);

            gLayoutFlowMap.AddEntry("horizontal", (int)LayoutFlow.Horizontal);
            gLayoutFlowMap.AddEntry("vertical", (int)LayoutFlow.TopToBottom);
            gLayoutFlowMap.AddEntry("vertical-ideographic", (int)LayoutFlow.TopToBottomIdeographic);
            gLayoutFlowMap.AddEntry("horizontal-ideographic", (int)LayoutFlow.HorizontalIdeographic);

            gCalloutTypeMap.AddEntry("oneSegment", (int)CalloutType.OneSegment);
            gCalloutTypeMap.AddEntry("twoSegment", (int)CalloutType.TwoSegment);
            gCalloutTypeMap.AddEntry("threeSegment", (int)CalloutType.ThreeSegment);
            gCalloutTypeMap.AddEntry("rightAngle", (int)CalloutType.RightAngle);

            gShadowTypeMap.AddEntry("single", (int)ShadowTypeCore.Offset);
            gShadowTypeMap.AddEntry("double", (int)ShadowTypeCore.Double);
            gShadowTypeMap.AddEntry("perspective", (int)ShadowTypeCore.Rich);
            gShadowTypeMap.AddEntry("shapeRelative", (int)ShadowTypeCore.Shape);
            gShadowTypeMap.AddEntry("drawingRelative", (int)ShadowTypeCore.Drawing);
            gShadowTypeMap.AddEntry("emboss", (int)ShadowTypeCore.EmbossOrEngrave);

            gThreeDRenderModeMap.AddEntry("solid", (int)ThreeDRenderMode.FullRender);
            gThreeDRenderModeMap.AddEntry("wireFrame", (int)ThreeDRenderMode.Wireframe);
            gThreeDRenderModeMap.AddEntry("boundingCube", (int)ThreeDRenderMode.BoundingCube);

            gPlaneTypeMap.AddEntry("XY", (int)PlaneType.XY);
            gPlaneTypeMap.AddEntry("ZX", (int)PlaneType.ZX);
            gPlaneTypeMap.AddEntry("YZ", (int)PlaneType.YZ);

            gLineFillTypeMap.AddEntry("solid", (int)LineFillType.Solid);
            gLineFillTypeMap.AddEntry("tile", (int)LineFillType.Texture);
            gLineFillTypeMap.AddEntry("pattern", (int)LineFillType.Pattern);
            gLineFillTypeMap.AddEntry("frame", (int)LineFillType.Picture);

            gArrowTypeMap.AddEntry("none", (int)ArrowType.None);
            gArrowTypeMap.AddEntry("classic", (int)ArrowType.Stealth);
            gArrowTypeMap.AddEntry("block", (int)ArrowType.Arrow);
            gArrowTypeMap.AddEntry("diamond", (int)ArrowType.Diamond);
            gArrowTypeMap.AddEntry("oval", (int)ArrowType.Oval);
            gArrowTypeMap.AddEntry("open", (int)ArrowType.Open);

            gArrowLengthMap.AddEntry("short", (int)ArrowLength.Short);
            gArrowLengthMap.AddEntry("medium", (int)ArrowLength.Medium);
            gArrowLengthMap.AddEntry("long", (int)ArrowLength.Long);

            gArrowWidthMap.AddEntry("narrow", (int)ArrowWidth.Narrow);
            gArrowWidthMap.AddEntry("medium", (int)ArrowWidth.Medium);
            gArrowWidthMap.AddEntry("wide", (int)ArrowWidth.Wide);

            gEndCapMap.AddEntry("flat", (int)EndCap.Flat);
            gEndCapMap.AddEntry("square", (int)EndCap.Square);
            gEndCapMap.AddEntry("round", (int)EndCap.Round);

            gShapeLineStyleMap.AddEntry("single", (int)ShapeLineStyle.Single);
            gShapeLineStyleMap.AddEntry("thinThin", (int)ShapeLineStyle.Double);
            gShapeLineStyleMap.AddEntry("thinThick", (int)ShapeLineStyle.ThinThick);
            gShapeLineStyleMap.AddEntry("thickThin", (int)ShapeLineStyle.ThickThin);
            gShapeLineStyleMap.AddEntry("thickBetweenThin", (int)ShapeLineStyle.Triple);

            gJoinStyleMap.AddEntry("round", (int)JoinStyle.Round);
            gJoinStyleMap.AddEntry("bevel", (int)JoinStyle.Bevel);
            gJoinStyleMap.AddEntry("miter", (int)JoinStyle.Miter);

            gDashStyleMap.AddEntry("solid", (int)DashStyle.Solid);
            gDashStyleMap.AddEntry("shortDash", (int)DashStyle.ShortDash);
            gDashStyleMap.AddEntry("1 1", (int)DashStyle.ShortDot);
            gDashStyleMap.AddEntry("shortDashDot", (int)DashStyle.ShortDashDot);
            gDashStyleMap.AddEntry("shortDashDotDot", (int)DashStyle.ShortDashDotDot);
            gDashStyleMap.AddEntry("dot", (int)DashStyle.Dot);
            gDashStyleMap.AddEntry("dash", (int)DashStyle.Dash);
            gDashStyleMap.AddEntry("longDash", (int)DashStyle.LongDash);
            gDashStyleMap.AddEntry("dashDot", (int)DashStyle.DashDot);
            gDashStyleMap.AddEntry("longDashDot", (int)DashStyle.LongDashDot);
            gDashStyleMap.AddEntry("longDashDotDot", (int)DashStyle.LongDashDotDot);

            gCalloutAngleMap.AddEntry("auto", (int)CalloutAngle.Any);
            gCalloutAngleMap.AddEntry("30", (int)CalloutAngle.Angle30);
            gCalloutAngleMap.AddEntry("45", (int)CalloutAngle.Angle45);
            gCalloutAngleMap.AddEntry("60", (int)CalloutAngle.Angle60);
            gCalloutAngleMap.AddEntry("90", (int)CalloutAngle.Angle90);

            gCalloutDropTypeMap.AddEntry("top", (int)CalloutDropType.Top);
            gCalloutDropTypeMap.AddEntry("center", (int)CalloutDropType.Center);
            gCalloutDropTypeMap.AddEntry("bottom", (int)CalloutDropType.Bottom);

            gLineStyleMap.AddEntry("none", (int)LineStyle.None);
            gLineStyleMap.AddEntry("single", (int)LineStyle.Single);
            gLineStyleMap.AddEntry("thick", (int)LineStyle.Thick);
            gLineStyleMap.AddEntry("double", (int)LineStyle.Double);
            gLineStyleMap.AddEntry("hairline", (int)LineStyle.Hairline);
            gLineStyleMap.AddEntry("dot", (int)LineStyle.Dot);
            gLineStyleMap.AddEntry("dash", (int)LineStyle.DashLargeGap);
            gLineStyleMap.AddEntry("dotDash", (int)LineStyle.DotDash);
            gLineStyleMap.AddEntry("dashDotDot", (int)LineStyle.DotDotDash);
            gLineStyleMap.AddEntry("triple", (int)LineStyle.Triple);
            gLineStyleMap.AddEntry("thinThickSmall", (int)LineStyle.ThinThickSmallGap);
            gLineStyleMap.AddEntry("thickThinSmall", (int)LineStyle.ThickThinSmallGap);
            gLineStyleMap.AddEntry("thickBetweenThinSmall", (int)LineStyle.ThinThickThinSmallGap);
            gLineStyleMap.AddEntry("thinThick", (int)LineStyle.ThinThickMediumGap);
            gLineStyleMap.AddEntry("thickThin", (int)LineStyle.ThickThinMediumGap);
            gLineStyleMap.AddEntry("thickBetweenThin", (int)LineStyle.ThinThickThinMediumGap);
            gLineStyleMap.AddEntry("thinThickLarge", (int)LineStyle.ThinThickLargeGap);
            gLineStyleMap.AddEntry("thickThinLarge", (int)LineStyle.ThickThinLargeGap);
            gLineStyleMap.AddEntry("thickBetweenThinLarge", (int)LineStyle.ThinThickThinLargeGap);
            gLineStyleMap.AddEntry("wave", (int)LineStyle.Wave);
            gLineStyleMap.AddEntry("doubleWave", (int)LineStyle.DoubleWave);
            gLineStyleMap.AddEntry("dashedSmall", (int)LineStyle.DashSmallGap);
            gLineStyleMap.AddEntry("dashDotStroked", (int)LineStyle.DashDotStroker);
            gLineStyleMap.AddEntry("threeDEmboss", (int)LineStyle.Emboss3D);
            gLineStyleMap.AddEntry("threeDEngrave", (int)LineStyle.Engrave3D);
            gLineStyleMap.AddEntry("HTMLOutset", (int)LineStyle.Outset);
            gLineStyleMap.AddEntry("HTMLInset", (int)LineStyle.Inset);

            gConnectionSiteTypeMap.AddEntry("none", (int)ConnectionSiteType.None);
            gConnectionSiteTypeMap.AddEntry("rect", (int)ConnectionSiteType.Rect);
            gConnectionSiteTypeMap.AddEntry("segments", (int)ConnectionSiteType.Segments);
            gConnectionSiteTypeMap.AddEntry("custom", (int)ConnectionSiteType.Custom);

            gTextPathAlignmentMap.AddEntry("left", (int)TextPathAlignment.Left);
            gTextPathAlignmentMap.AddEntry("right", (int)TextPathAlignment.Right);
            gTextPathAlignmentMap.AddEntry("center", (int)TextPathAlignment.Center);
            gTextPathAlignmentMap.AddEntry("justify", (int)TextPathAlignment.WordJustify);
            gTextPathAlignmentMap.AddEntry("letter-justify", (int)TextPathAlignment.LetterJustify);
            gTextPathAlignmentMap.AddEntry("stretch-justify", (int)TextPathAlignment.Stretch);

            gHRAlignMap.AddEntry("left", (int)HorizontalRuleAlignment.Left);
            gHRAlignMap.AddEntry("center", (int)HorizontalRuleAlignment.Center);
            gHRAlignMap.AddEntry("right", (int)HorizontalRuleAlignment.Right);

            gOleLinkTypeMap.AddEntry("Bitmap", (int)OleLinkType.Bitmap);
            gOleLinkTypeMap.AddEntry("Html", (int)OleLinkType.Html);
            gOleLinkTypeMap.AddEntry("Picture", (int)OleLinkType.Picture);
            gOleLinkTypeMap.AddEntry("Rtf", (int)OleLinkType.Rtf);
            gOleLinkTypeMap.AddEntry("Text", (int)OleLinkType.Text);
            gOleLinkTypeMap.AddEntry("Unicode", (int)OleLinkType.Unicode);

            // Names taken from VML Specification http://msdn.microsoft.com/library/default.asp?url=/workshop/author/vml/SHAPE/introduction.asp?frame=true
            // First letters moved to low register.
            // Several names were tested in MS Word.
            // For DOCX other names should be used, defined in ECMA TC 45 5.1.12.10 ST_BlackWhiteMode (Black and White Mode)
            gBWModeMap.AddEntry("auto", (int)BWMode.Automatic);
            gBWModeMap.AddEntry("black", (int)BWMode.Black);
            gBWModeMap.AddEntry("blackTextAndLines", (int)BWMode.BlackTextLine);
            gBWModeMap.AddEntry("highContrast", (int)BWMode.HighContrast);
            gBWModeMap.AddEntry("color", (int)BWMode.Color);
            gBWModeMap.AddEntry("grayScale", (int)BWMode.GrayScale);
            gBWModeMap.AddEntry("grayOutline", (int)BWMode.GrayOutline);
            gBWModeMap.AddEntry("undrawn", (int)BWMode.DontShow);
            gBWModeMap.AddEntry("inverseGray", (int)BWMode.InverseGray);
            gBWModeMap.AddEntry("lightGrayScale", (int)BWMode.LightGrayScale);
            gBWModeMap.AddEntry("white", (int)BWMode.White);

            gOperationMap.AddEntry("abs", (int)Operation.Abs);
            gOperationMap.AddEntry("atan2", (int)Operation.Atan2);
            gOperationMap.AddEntry("cos", (int)Operation.Cos);
            gOperationMap.AddEntry("cosatan2", (int)Operation.CosAtan2);
            gOperationMap.AddEntry("ellipse", (int)Operation.Ellipse);
            gOperationMap.AddEntry("if", (int)Operation.If);
            gOperationMap.AddEntry("max", (int)Operation.Max);
            gOperationMap.AddEntry("mid", (int)Operation.Mid);
            gOperationMap.AddEntry("min", (int)Operation.Min);
            gOperationMap.AddEntry("mod", (int)Operation.Mod);
            gOperationMap.AddEntry("prod", (int)Operation.Prod);
            gOperationMap.AddEntry("sin", (int)Operation.Sin);
            gOperationMap.AddEntry("sinatan2", (int)Operation.SinAtan2);
            gOperationMap.AddEntry("sqrt", (int)Operation.Sqrt);
            gOperationMap.AddEntry("sum", (int)Operation.Sum);
            gOperationMap.AddEntry("sumangle", (int)Operation.SumAngle);
            gOperationMap.AddEntry("tan", (int)Operation.Tan);

            gScriptLanguageMap.AddEntry("JavaScript", (int)ScriptLanguage.JavaScript);

            gPathTypeMap.AddEntry("m", (int)PathType.MoveTo);
            gPathTypeMap.AddEntry("l", (int)PathType.LineTo);
            gPathTypeMap.AddEntry("c", (int)PathType.CurveTo);
            gPathTypeMap.AddEntry("x", (int)PathType.Close);
            gPathTypeMap.AddEntry("e", (int)PathType.End);
            gPathTypeMap.AddEntry("nf", (int)PathType.NoFill);
            gPathTypeMap.AddEntry("ns", (int)PathType.NoLine);
            gPathTypeMap.AddEntry("ae", (int)PathType.AngleEllipseTo);
            gPathTypeMap.AddEntry("al", (int)PathType.AngleEllipse);
            gPathTypeMap.AddEntry("at", (int)PathType.ArcTo);
            gPathTypeMap.AddEntry("ar", (int)PathType.Arc);
            gPathTypeMap.AddEntry("wa", (int)PathType.ClockwiseArcTo);
            gPathTypeMap.AddEntry("wr", (int)PathType.ClockwiseArc);
            gPathTypeMap.AddEntry("qx", (int)PathType.EllipticalQuadrantX);
            gPathTypeMap.AddEntry("qy", (int)PathType.EllipticalQuadrantY);
            gPathTypeMap.AddEntry("qb", (int)PathType.QuadraticBezier);
            gPathTypeMap.AddEntry("ha", (int)PathType.EscapeAutoLine);
            gPathTypeMap.AddEntry("hb", (int)PathType.EscapeAutoCurve);
            gPathTypeMap.AddEntry("hc", (int)PathType.EscapeCornerLine);
            gPathTypeMap.AddEntry("hd", (int)PathType.EscapeCornerCurve);
            gPathTypeMap.AddEntry("he", (int)PathType.EscapeSmoothLine);
            gPathTypeMap.AddEntry("hf", (int)PathType.EscapeSmoothCurve);
            gPathTypeMap.AddEntry("hg", (int)PathType.EscapeSymmetricLine);
            gPathTypeMap.AddEntry("hh", (int)PathType.EscapeSymmetricCurve);
            gPathTypeMap.AddEntry("hi", (int)PathType.EscapeFreeForm);
            gPathTypeMap.AddEntry("hj", (int)PathType.FillColor);
            gPathTypeMap.AddEntry("hk", (int)PathType.LineColor);

            gLocksMap.AddEntry(ShapeAttr.LockAgainstGrouping, (int)DmlLock.NoGroup);
            gLocksMap.AddEntry(ShapeAttr.LockAgainstSelect,(int)DmlLock.NoSelect);
            gLocksMap.AddEntry(ShapeAttr.LockRotation, (int)DmlLock.NoRotation);
            gLocksMap.AddEntry(ShapeAttr.LockAspectRatio, (int)DmlLock.NoChangeAspect);
            gLocksMap.AddEntry(ShapeAttr.LockPosition, (int)DmlLock.NoMove);
            gLocksMap.AddEntry(ShapeAttr.LockVertices, (int)DmlLock.NoEditPoints);
            gLocksMap.AddEntry(ShapeAttr.LockAdjustHandles, (int)DmlLock.NoAdjustHandles);
            gLocksMap.AddEntry(ShapeAttr.LockShapeType, (int)DmlLock.NoChangeShapeType);
            gLocksMap.AddEntry(ShapeAttr.LockAgainstUngrouping, (int)DmlLock.NoUngroup);
            gLocksMap.AddEntry(ShapeAttr.LockCropping, (int)DmlLock.NoCrop);
            gLocksMap.AddEntry(ShapeAttr.LockText, (int)DmlLock.NoTextEdit);
            // These locks DML related only.
            gLocksMap.AddEntry(ShapeAttr.DmlLockArrowHeads, (int)DmlLock.NoChangeArrowheads);
            gLocksMap.AddEntry(ShapeAttr.DmlLockResize, (int)DmlLock.NoResize);
            gLocksMap.AddEntry(ShapeAttr.DmlLockDrillDown, (int)DmlLock.NoDrilldown);
            // These locks VML related only, there are no match in DML.
            // ShapeAttr.AnchorLocked
            // ShapeAttr.OleLocked
            // Vml codes for hatch types were determined experimentally.
            AddToFillMap("cross", HatchStyle.Cross, 0);
            AddToFillMap("dashed downward diagonal", HatchStyle.DashedDownwardDiagonal, 708767958);
            AddToFillMap("dashed horizontal", HatchStyle.DashedHorizontal, -902166618);
            AddToFillMap("dashed upward diagonal", HatchStyle.DashedUpwardDiagonal, 1943013054);
            AddToFillMap("dashed vertical", HatchStyle.DashedVertical, 907995120);
            AddToFillMap("diagonal brick", HatchStyle.DiagonalBrick, 1442594541);
            AddToFillMap("diagonal cross", HatchStyle.DiagonalCross, 0);
            AddToFillMap("divot", HatchStyle.Divot, 584574198);
            AddToFillMap("dark downward diagonal", HatchStyle.DarkDownwardDiagonal, 703509273);
            AddToFillMap("dark horizontal", HatchStyle.DarkHorizontal, -1645040544);
            AddToFillMap("dark upward diagonal", HatchStyle.DarkUpwardDiagonal, 1516936182);
            AddToFillMap("dark vertical", HatchStyle.DarkVertical, -1484066896);
            AddToFillMap("downward diagonal", HatchStyle.BackwardDiagonal, 0);
            AddToFillMap("dotted diamond", HatchStyle.DottedDiamond, -738923971);
            AddToFillMap("dotted grid", HatchStyle.DottedGrid, 1695561988);
            AddToFillMap("horizontal", HatchStyle.Horizontal, 0);
            AddToFillMap("horizontal brick", HatchStyle.HorizontalBrick, 1723978230);
            AddToFillMap("large checker board", HatchStyle.LargeCheckerBoard, 577586775);
            AddToFillMap("large confetti", HatchStyle.LargeConfetti, 400097116);
            // Strange thing HatchStyle.LargeGrid has the same integer value as HatchStyle.Cross.
            // That is why skip this value. handle it manually in the converter method.
            // "large grid" = HatchStyle.LargeGrid
            AddToFillMap("light downward diagonal", HatchStyle.LightDownwardDiagonal, -233870241);
            AddToFillMap("light horizontal", HatchStyle.LightHorizontal, -2144098250);
            AddToFillMap("light upward diagonal", HatchStyle.LightUpwardDiagonal, -516687836);
            AddToFillMap("light vertical", HatchStyle.LightVertical, -1060290979);
            AddToFillMap("narrow horizontal", HatchStyle.NarrowHorizontal, 811627745);
            AddToFillMap("narrow vertical", HatchStyle.NarrowVertical, 971821860);
            AddToFillMap("open diamond", HatchStyle.OutlinedDiamond, -1273002823);
            AddToFillMap("5%", HatchStyle.Percent05, -612221790);
            AddToFillMap("10%", HatchStyle.Percent10, -120615383);
            AddToFillMap("20%", HatchStyle.Percent20, -1500494643);
            AddToFillMap("25%", HatchStyle.Percent25, 2062819310);
            AddToFillMap("30%", HatchStyle.Percent30, -1266175546);
            AddToFillMap("40%", HatchStyle.Percent40, 1597532706);
            AddToFillMap("50%", HatchStyle.Percent50, 146870144);
            AddToFillMap("60%", HatchStyle.Percent60, -1074143645);
            AddToFillMap("70%", HatchStyle.Percent70, 1811227497);
            AddToFillMap("75%", HatchStyle.Percent75, -69754362);
            AddToFillMap("80%", HatchStyle.Percent80, 732808692);
            AddToFillMap("90%", HatchStyle.Percent90, -679324745);
            AddToFillMap("plaid", HatchStyle.Plaid, -842645683);
            AddToFillMap("shingle", HatchStyle.Shingle, 1346777725);
            AddToFillMap("small checker board", HatchStyle.SmallCheckerBoard, -1374783248);
            AddToFillMap("small confetti", HatchStyle.SmallConfetti, 1379582820);
            AddToFillMap("small grid", HatchStyle.SmallGrid, -1324759092);
            AddToFillMap("solid diamond", HatchStyle.SolidDiamond, 1673964194);
            AddToFillMap("sphere", HatchStyle.Sphere, -1815305409);
            AddToFillMap("trellis", HatchStyle.Trellis, 1984576894);
            AddToFillMap("upward diagonal", HatchStyle.ForwardDiagonal, 0);
            AddToFillMap("vertical", HatchStyle.Vertical, 0);
            AddToFillMap("wave", HatchStyle.Wave, 652020124);
            AddToFillMap("wide downward diagonal", HatchStyle.WideDownwardDiagonal, 136926604);
            AddToFillMap("wide upward diagonal", HatchStyle.WideUpwardDiagonal, 1944184317);
            AddToFillMap("weave", HatchStyle.Weave, -1278091726);
            AddToFillMap("zig zag", HatchStyle.ZigZag, -1753090367);

            // Vml shapes uses "large grid" hatch name instead of "cross"
            AddToFillMap("large grid", HatchStyle.LargeGrid, -550909115);
        }

        internal const GradientType LinearSigmaGradient = (GradientType)0x4000000b;
        private static string gLGridFillName = "large grid";

    }
}
