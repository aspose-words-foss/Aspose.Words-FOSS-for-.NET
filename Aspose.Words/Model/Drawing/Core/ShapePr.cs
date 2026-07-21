// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2006 by Roman Korchagin
using System;
using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Attributes of a shape.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class ShapePr : AttrCollection
    {
        internal bool AllowInCell
        {
            get { return (bool)FetchAttr(ShapeAttr.AllowInCell); }
        }

        internal bool BehindText
        {
            get { return (bool)FetchAttr(ShapeAttr.BehindText); }
        }

        internal int ZOrder
        {
            get { return (int)FetchAttr(ShapeAttr.ZOrder); }
        }

        internal WrapType WrapType
        {
            get { return (WrapType)FetchAttr(ShapeAttr.WrapType); }
        }

        internal WrapSide WrapSide
        {
            get { return (WrapSide)FetchAttr(ShapeAttr.WrapSide); }
        }

        internal HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)FetchAttr(ShapeAttr.HorizontalAlignment); }
        }

        internal VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)FetchAttr(ShapeAttr.VerticalAlignment); }
        }

        internal RelativeHorizontalPosition RelativeHorizontalPosition
        {
            get { return (RelativeHorizontalPosition)FetchAttr(ShapeAttr.RelativeHorizontalPosition); }
        }

        internal RelativeVerticalPosition RelativeVerticalPosition
        {
            get { return (RelativeVerticalPosition)FetchAttr(ShapeAttr.RelativeVerticalPosition); }
        }

        internal double Left
        {
            get { return (double)FetchAttr(ShapeAttr.Left); }
        }

        internal double Top
        {
            get { return (double)FetchAttr(ShapeAttr.Top); }
        }

        internal bool IsInline
        {
            get { return (WrapType == WrapType.Inline); }
        }

        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        internal static object FetchDefaultAttr(int key)
        {
            return gDefaults.FetchAttr(key);
        }

        static ShapePr()
        {
            gDefaults = new ShapePr();

            // Transform properties
            gDefaults.Add(ShapeAttr.TransformRotation, 0);

            // Protection properties
            gDefaults.Add(ShapeAttr.LockRotation, false);
            gDefaults.Add(ShapeAttr.LockAspectRatio, false);
            gDefaults.Add(ShapeAttr.LockPosition, false);
            gDefaults.Add(ShapeAttr.LockAgainstSelect, false);
            gDefaults.Add(ShapeAttr.LockCropping, false);
            gDefaults.Add(ShapeAttr.LockVertices, false);
            gDefaults.Add(ShapeAttr.LockText, false);
            gDefaults.Add(ShapeAttr.LockAdjustHandles, false);
            gDefaults.Add(ShapeAttr.LockAgainstGrouping, false);

            // Text properties
            gDefaults.Add(ShapeAttr.TextboxTxid, 0);
            gDefaults.Add(ShapeAttr.TextboxLeft, ConvertUtilCore.PointToEmu(7.2)); // 1/10 inch
            gDefaults.Add(ShapeAttr.TextboxRight, ConvertUtilCore.PointToEmu(7.2)); // 1/10 inch
            gDefaults.Add(ShapeAttr.TextboxTop, ConvertUtilCore.PointToEmu(3.6)); // 1/20 inch
            gDefaults.Add(ShapeAttr.TextboxBottom, ConvertUtilCore.PointToEmu(3.6)); // 1/20 inch
            gDefaults.Add(ShapeAttr.TextboxWrapMode, TextBoxWrapMode.Square);
            gDefaults.Add(ShapeAttr.TextboxZoom, 0);
            gDefaults.Add(ShapeAttr.TextboxAnchor, TextBoxAnchor.Top);
            gDefaults.Add(ShapeAttr.TextboxLayoutFlow, LayoutFlow.Horizontal);
            gDefaults.Add(ShapeAttr.TextboxFontRotation, FontRotation.Right);
            gDefaults.Add(ShapeAttr.TextboxNextShapeId, 0);
            gDefaults.Add(ShapeAttr.TextboxSelectText, true);
            gDefaults.Add(ShapeAttr.TextboxAutoTextMargin, false);
            gDefaults.Add(ShapeAttr.TextboxRotateText, false);
            gDefaults.Add(ShapeAttr.TextboxFitShapeToText, false);
            gDefaults.Add(ShapeAttr.TextboxFitTextToShape, false);

            // GeoText (WordArt) properties.
            gDefaults.Add(ShapeAttr.GeoTextText, String.Empty);
            gDefaults.Add(ShapeAttr.GeoTextRtf, String.Empty);
            gDefaults.Add(ShapeAttr.GeoTextAlign, TextPathAlignment.Center);
            gDefaults.Add(ShapeAttr.GeoTextSize, ConvertUtilCore.DoubleToFixed(GeoTextSizeDefault));
            gDefaults.Add(ShapeAttr.GeoTextSpacing, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.GeoTextFont, "Arial");    // MS Word says default is null, but I don't think its good.
            gDefaults.Add(ShapeAttr.GeoTextReverseRows, false);
            gDefaults.Add(ShapeAttr.GeoTextOn, false);
            gDefaults.Add(ShapeAttr.GeoTextVertical, false);
            gDefaults.Add(ShapeAttr.GeoTextKerning, false);
            gDefaults.Add(ShapeAttr.GeoTextTight, false);
            gDefaults.Add(ShapeAttr.GeoTextStretch, false);
            gDefaults.Add(ShapeAttr.GeoTextShrinkFit, false);
            gDefaults.Add(ShapeAttr.GeoTextBestFit, false);
            gDefaults.Add(ShapeAttr.GeoTextNormalize, false);
            gDefaults.Add(ShapeAttr.GeoTextDxMeasure, false);
            gDefaults.Add(ShapeAttr.GeoTextBold, false);
            gDefaults.Add(ShapeAttr.GeoTextItalic, false);
            gDefaults.Add(ShapeAttr.GeoTextUnderline, false);
            gDefaults.Add(ShapeAttr.GeoTextShadow, false);
            gDefaults.Add(ShapeAttr.GeoTextSmallCaps, false);
            gDefaults.Add(ShapeAttr.GeoTextStrikeThrough, false);

            // Blip properties
            gDefaults.Add(ShapeAttr.ImageCropTop, 0);
            gDefaults.Add(ShapeAttr.ImageCropBottom, 0);
            gDefaults.Add(ShapeAttr.ImageCropLeft, 0);
            gDefaults.Add(ShapeAttr.ImageCropRight, 0);

            gDefaults.Add(ShapeAttr.GraphicFrameExtHeight, 0);
            gDefaults.Add(ShapeAttr.GraphicFrameExtWidth, 0);

            gDefaults.Add(ShapeAttr.FillRectBottom, 0);
            gDefaults.Add(ShapeAttr.FillRectTop, 0);
            gDefaults.Add(ShapeAttr.FillRectLeft, 0);
            gDefaults.Add(ShapeAttr.FillRectRight, 0);
            gDefaults.Add(ShapeAttr.FillBlipName, string.Empty);

            // No defaults for ImageBlipId, ImageName, ImageNameFlags.
            // According to the SPEC it should be 0xFFFFFFFF, but in Escher color top 8 bits are special
            // and we don't handle them yet, therefore let's make it just empty color for now.
            gDefaults.Add(ShapeAttr.ImageTransparent, DrColor.Empty);
            gDefaults.Add(ShapeAttr.ImageContrast, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.ImageBrightness, 0);
            gDefaults.Add(ShapeAttr.ImageGamma, 0);
            // No default for OleTxid
            // No defaults for ImageDblCrMod, ImageFillCrMod, ImageLineCrMod
            // No defaults for ImagePrint, ImagePrintName, ImagePrintNameFlags
            gDefaults.Add(ShapeAttr.ImageNoHitTest, false);
            gDefaults.Add(ShapeAttr.ImageGrayScale, false);
            gDefaults.Add(ShapeAttr.ImageBiLevel, false);
            gDefaults.Add(ShapeAttr.ImageActive, false);

            // Geometry properties.
            gDefaults.Add(ShapeAttr.GeometryVertices, null);
            gDefaults.Add(ShapeAttr.GeometrySegmentInfo, null);
            gDefaults.Add(ShapeAttr.GeometryAdjust1, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust2, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust3, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust4, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust5, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust6, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust7, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust8, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust9, 0);
            gDefaults.Add(ShapeAttr.GeometryAdjust10, 0);
            gDefaults.Add(ShapeAttr.GeometryConnectLocs, null);
            gDefaults.Add(ShapeAttr.GeometryConnectAngles, null);
            gDefaults.Add(ShapeAttr.GeometryXLimo, 0);
            gDefaults.Add(ShapeAttr.GeometryYLimo, 0);
            gDefaults.Add(ShapeAttr.GeometryHandles, null);
            gDefaults.Add(ShapeAttr.GeometryFormulas, null);

            gDefaults.Add(ShapeAttr.GeometryPathTextBoxRects, null);
            gDefaults.Add(ShapeAttr.GeometryConnectionSiteType, ConnectionSiteType.None);
            gDefaults.Add(ShapeAttr.GeometryShadowOK, true);
            gDefaults.Add(ShapeAttr.GeometryThreeDOK, true);
            gDefaults.Add(ShapeAttr.GeometryLineOK, true);
            gDefaults.Add(ShapeAttr.GeometryGTextOK, false);
            gDefaults.Add(ShapeAttr.GeometryFillShadeShapeOK, false);
            gDefaults.Add(ShapeAttr.GeometryFillOK, true);

            // Fill properties.
            gDefaults.Add(ShapeAttr.FillType, FillTypeCore.Solid);
            gDefaults.Add(ShapeAttr.FillColor, DrColor.White);
            gDefaults.Add(ShapeAttr.FillOpacity, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.FillBackColor, DrColor.White);
            gDefaults.Add(ShapeAttr.FillBackOpacity, ConvertUtilCore.DoubleToFixed(1.0));
            // No default for FillCrMod, FillBlipId, FillBlipName, FillBlipNameFlags.
            // No default for FillWidth, FillHeight
            gDefaults.Add(ShapeAttr.FillAngle, 0);
            gDefaults.Add(ShapeAttr.FillFocus, 0);
            gDefaults.Add(ShapeAttr.FillToLeft, 0);
            gDefaults.Add(ShapeAttr.FillToTop, 0);
            gDefaults.Add(ShapeAttr.FillToRight, 0);
            gDefaults.Add(ShapeAttr.FillToBottom, 0);
            gDefaults.Add(ShapeAttr.FillDimensionType, FillDimensionType.Default);
            gDefaults.Add(ShapeAttr.FillShadePreset, 0);
            gDefaults.Add(ShapeAttr.FillShadeColors, null);
            gDefaults.Add(ShapeAttr.FillOriginX, 0);
            gDefaults.Add(ShapeAttr.FillOriginY, 0);
            gDefaults.Add(ShapeAttr.FillShapeOriginX, 0);
            gDefaults.Add(ShapeAttr.FillShapeOriginY, 0);
            // No default FillShadeType.
            gDefaults.Add(ShapeAttr.FillRecolorAsPicture, false);
            gDefaults.Add(ShapeAttr.FillUseShapeAnchor, false); // DB: Found experimentally.
            gDefaults.Add(ShapeAttr.Filled, true);
            gDefaults.Add(ShapeAttr.FillHitTest, true);
            gDefaults.Add(ShapeAttr.FillShape, true);
            gDefaults.Add(ShapeAttr.FillUseRect, false);
            gDefaults.Add(ShapeAttr.FillNoFillHitTest, false);

            // Stroke properties.
            gDefaults.Add(ShapeAttr.LineColor, DrColor.Black);
            gDefaults.Add(ShapeAttr.LineOpacity, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.LineBackColor, DrColor.White);
            gDefaults.Add(ShapeAttr.LineFillType, LineFillType.Solid);
            // No default for LineFillBlipId, LineFillBlipName, LineFillBlipNameFlags.
            // No default for LineFillWidth, LineFillHeight
            gDefaults.Add(ShapeAttr.LineFillDimensionType, FillDimensionType.Default);
            gDefaults.Add(ShapeAttr.LineWidth, 9525);
            // No default for LineMiterLimit.
            gDefaults.Add(ShapeAttr.LineStyle, ShapeLineStyle.Default);
            gDefaults.Add(ShapeAttr.LineDashStyle, DashStyle.Solid);
            gDefaults.Add(ShapeAttr.LineDashData, null);
            gDefaults.Add(ShapeAttr.LineStartArrow, ArrowType.None);
            gDefaults.Add(ShapeAttr.LineEndArrow, ArrowType.None);
            gDefaults.Add(ShapeAttr.LineStartArrowWidth, ArrowWidth.Medium);
            gDefaults.Add(ShapeAttr.LineStartArrowLength, ArrowLength.Medium);
            gDefaults.Add(ShapeAttr.LineEndArrowWidth, ArrowWidth.Medium);
            gDefaults.Add(ShapeAttr.LineEndArrowLength, ArrowLength.Medium);
            gDefaults.Add(ShapeAttr.LineJoinStyle, JoinStyle.Round);
            gDefaults.Add(ShapeAttr.LineEndCapStyle, EndCap.Default);
            gDefaults.Add(ShapeAttr.LineArrowHeadsOK, false);
            gDefaults.Add(ShapeAttr.LineOn, true);
            gDefaults.Add(ShapeAttr.LineHitTest, true);
            gDefaults.Add(ShapeAttr.LineFillShape, true);
            gDefaults.Add(ShapeAttr.LineNoLineDrawDash, false);
            gDefaults.Add(ShapeAttr.LineInsetPen, false);

            // Shadow properties
            gDefaults.Add(ShapeAttr.ShadowType, ShadowTypeCore.Offset);
            gDefaults.Add(ShapeAttr.ShadowColor, new DrColor(0x80, 0x80, 0x80));
            gDefaults.Add(ShapeAttr.ShadowHighlight, new DrColor(0xcb, 0xcb, 0xcb));
            // No default for ShadowCrMod.
            gDefaults.Add(ShapeAttr.ShadowOpacity, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.ShadowOffsetX, 25400);
            gDefaults.Add(ShapeAttr.ShadowOffsetY, 25400);
            gDefaults.Add(ShapeAttr.ShadowSecondOffsetX, 0);
            gDefaults.Add(ShapeAttr.ShadowSecondOffsetY, 0);
            gDefaults.Add(ShapeAttr.ShadowScaleXtoX, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.ShadowScaleYtoX, 0);
            gDefaults.Add(ShapeAttr.ShadowScaleXtoY, 0);
            gDefaults.Add(ShapeAttr.ShadowScaleYtoY, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.ShadowPerspectiveX, 0);
            gDefaults.Add(ShapeAttr.ShadowPerspectiveY, 0);
            gDefaults.Add(ShapeAttr.ShadowWeight, 256);
            gDefaults.Add(ShapeAttr.ShadowOriginX, 0);
            gDefaults.Add(ShapeAttr.ShadowOriginY, 0);
            gDefaults.Add(ShapeAttr.ShadowOn, false);
            gDefaults.Add(ShapeAttr.ShadowObscured, false);

            // Perspective properties
            gDefaults.Add(ShapeAttr.PerspectiveType, PerspectiveType.Shape);
            gDefaults.Add(ShapeAttr.PerspectiveOffsetX, 0);
            gDefaults.Add(ShapeAttr.PerspectiveOffsetY, 0);
            gDefaults.Add(ShapeAttr.PerspectiveScaleXToX, 65536);
            gDefaults.Add(ShapeAttr.PerspectiveScaleYToX, 0);
            gDefaults.Add(ShapeAttr.PerspectiveScaleXToY, 0);
            gDefaults.Add(ShapeAttr.PerspectiveScaleYToY, 65536);
            gDefaults.Add(ShapeAttr.PerspectiveX, 0);
            gDefaults.Add(ShapeAttr.PerspectiveY, 0);
            gDefaults.Add(ShapeAttr.PerspectiveWeight, 256);
            gDefaults.Add(ShapeAttr.PerspectiveOriginX, 32768);
            gDefaults.Add(ShapeAttr.PerspectiveOriginY, 32768);
            gDefaults.Add(ShapeAttr.PerspectiveOn, false);

            // 3D object properties
            gDefaults.Add(ShapeAttr.TDSpecularAmount, 0);
            gDefaults.Add(ShapeAttr.TDDiffuseAmount, 65536);
            gDefaults.Add(ShapeAttr.TDShininess, 5);
            gDefaults.Add(ShapeAttr.TDEdgeThickness, 12700);
            gDefaults.Add(ShapeAttr.TDExtrudeForward, 0);
            gDefaults.Add(ShapeAttr.TDExtrudeBackward, 457200);
            // See "[MS-OI29500] 2.1.1799 Part 4 Section 14.2.2.11, extrusion (3D Extrusion)".
            // Word does not support the plane attribute.
            gDefaults.Add(ShapeAttr.TDExtrudePlane, PlaneType.XY);
            // According to the spec the default is 0x100000F7, but we can't represent this in the model yet.
            gDefaults.Add(ShapeAttr.TDExtrusionColor, DrColor.Empty);
            // According to the spec the default is 0x20000000, but we can't represent this in the model yet.
            gDefaults.Add(ShapeAttr.TDCrMod, DrColor.Empty);
            gDefaults.Add(ShapeAttr.TDOn, false);
            gDefaults.Add(ShapeAttr.TDMetallic, false);
            gDefaults.Add(ShapeAttr.TDUseExtrusionColor, false);
            gDefaults.Add(ShapeAttr.TDLightFace, true);

            // 3D style properties
            gDefaults.Add(ShapeAttr.TDRotationAngleY, 0);
            gDefaults.Add(ShapeAttr.TDRotationAngleX, 0);
            gDefaults.Add(ShapeAttr.TDRotationAxisX, 100);
            gDefaults.Add(ShapeAttr.TDRotationAxisY, 0);
            gDefaults.Add(ShapeAttr.TDRotationAxisZ, 0);
            gDefaults.Add(ShapeAttr.TDRotationAngle, 0);
            gDefaults.Add(ShapeAttr.TDRotationCenterX, 0);
            gDefaults.Add(ShapeAttr.TDRotationCenterY, 0);
            gDefaults.Add(ShapeAttr.TDRotationCenterZ, 0);
            gDefaults.Add(ShapeAttr.TDRenderMode, ThreeDRenderMode.FullRender);
            gDefaults.Add(ShapeAttr.TDTolerance, 30000);
            gDefaults.Add(ShapeAttr.TDViewpointX, 1250000);
            gDefaults.Add(ShapeAttr.TDViewpointY, -1250000);
            gDefaults.Add(ShapeAttr.TDViewpointZ, 9000000);
            gDefaults.Add(ShapeAttr.TDOriginX, 32768);
            gDefaults.Add(ShapeAttr.TDOriginY, -32768);
            gDefaults.Add(ShapeAttr.TDSkewAngle, -8847360);
            gDefaults.Add(ShapeAttr.TDSkewAmount, 50);
            gDefaults.Add(ShapeAttr.TDAmbientIntensity, 20000);
            gDefaults.Add(ShapeAttr.TDKeyX, 50000);
            gDefaults.Add(ShapeAttr.TDKeyY, 0);
            gDefaults.Add(ShapeAttr.TDKeyZ, 10000);
            gDefaults.Add(ShapeAttr.TDKeyIntensity, 38000);
            gDefaults.Add(ShapeAttr.TDFillX, -50000);
            gDefaults.Add(ShapeAttr.TDFillY, 0);
            gDefaults.Add(ShapeAttr.TDFillZ, 10000);
            gDefaults.Add(ShapeAttr.TDFillIntensity, 38000);
            gDefaults.Add(ShapeAttr.TDConstrainRotation, true);
            gDefaults.Add(ShapeAttr.TDRotationCenterAuto, false);
            gDefaults.Add(ShapeAttr.TDParallel, true);
            gDefaults.Add(ShapeAttr.TDKeyHarsh, true);
            gDefaults.Add(ShapeAttr.TDFillHarsh, false);

            // Shape properties (non group shape only)
            gDefaults.Add(ShapeAttr.ConnectorType, ConnectorType.None);
            gDefaults.Add(ShapeAttr.BWMode, BWMode.Default);
            gDefaults.Add(ShapeAttr.BWPure, BWMode.Default);
            gDefaults.Add(ShapeAttr.BWNormal, BWMode.Default);
            gDefaults.Add(ShapeAttr.DiagramNodeLayout, DiagramNodeLayout.Standard);
            gDefaults.Add(ShapeAttr.DiagramNodeKind, DiagramNodeKind.Node);
            gDefaults.Add(ShapeAttr.OleIcon, false);
            gDefaults.Add(ShapeAttr.PreferRelativeResize, false);
            gDefaults.Add(ShapeAttr.LockShapeType, false);
            gDefaults.Add(ShapeAttr.DeleteAttachedObject, false);
            // No default for Background.

            // Callout properties
            gDefaults.Add(ShapeAttr.CalloutType, CalloutType.TwoSegment);
            gDefaults.Add(ShapeAttr.CalloutGap, ConvertUtilCore.PointToEmu(6));    //1/12 inch.
            gDefaults.Add(ShapeAttr.CalloutAngle, CalloutAngle.Any);
            gDefaults.Add(ShapeAttr.CalloutDropType, CalloutDropType.Specified);
            gDefaults.Add(ShapeAttr.CalloutDropDistance, ConvertUtilCore.PointToEmu(9));
            gDefaults.Add(ShapeAttr.CalloutLength, 0);
            gDefaults.Add(ShapeAttr.CalloutOn, false);
            gDefaults.Add(ShapeAttr.CalloutAccentBar, false);
            gDefaults.Add(ShapeAttr.CalloutTextBorder, true);
            gDefaults.Add(ShapeAttr.CalloutMinusX, false);
            gDefaults.Add(ShapeAttr.CalloutMinusY, false);
            gDefaults.Add(ShapeAttr.CalloutDropAuto, false);
            gDefaults.Add(ShapeAttr.CalloutLengthSpecified, false);

            // Group shape properties (applicable to normal and group shapes).
            gDefaults.Add(ShapeAttr.ShapeName, ShapeNameDefault);
            gDefaults.Add(ShapeAttr.ShapeDescription, String.Empty);
            gDefaults.Add(ShapeAttr.ShapeTitle, String.Empty);
            gDefaults.Add(ShapeAttr.HyperlinkAddress, String.Empty);
            gDefaults.Add(ShapeAttr.WrapPolygonVertices, null);
            gDefaults.Add(ShapeAttr.DistanceLeft, ConvertUtilCore.PointToEmu(9));    // 1/8inch
            gDefaults.Add(ShapeAttr.DistanceTop, 0);
            gDefaults.Add(ShapeAttr.DistanceRight, ConvertUtilCore.PointToEmu(9));    // 1/8inch
            gDefaults.Add(ShapeAttr.DistanceBottom, 0);
            gDefaults.Add(ShapeAttr.ScreenTip, String.Empty);
            gDefaults.Add(ShapeAttr.ScriptText, String.Empty);
            gDefaults.Add(ShapeAttr.HorizontalAlignment, HorizontalAlignmentDefault);
            gDefaults.Add(ShapeAttr.VerticalAlignment, VerticalAlignmentDefault);
            gDefaults.Add(ShapeAttr.RelativeHorizontalPosition, RelativeHorizontalPositionDefault);
            gDefaults.Add(ShapeAttr.RelativeVerticalPosition, RelativeVerticalPositionDefault);
            // No default for BorderXXXColor
            gDefaults.Add(ShapeAttr.HRPct, 1000);
            gDefaults.Add(ShapeAttr.HRAlign, HorizontalRuleAlignment.Left);
            // No default for HRHeight and HRWidth because we don't have them in the model.
            gDefaults.Add(ShapeAttr.ScriptType, String.Empty);
            gDefaults.Add(ShapeAttr.AllowInCell, AllowInCellDefault);
            gDefaults.Add(ShapeAttr.PictureBullet, false);
            gDefaults.Add(ShapeAttr.HRStandard, false);
            gDefaults.Add(ShapeAttr.HRNoShade, false);
            gDefaults.Add(ShapeAttr.HROn, false);
            gDefaults.Add(ShapeAttr.AllowOverlap, AllowOverlapDefault);
            gDefaults.Add(ShapeAttr.ReallyHidden, false);
            gDefaults.Add(ShapeAttr.ScriptAnchor, false);
            gDefaults.Add(ShapeAttr.EditedWrap, false);
            gDefaults.Add(ShapeAttr.BehindText, BehindTextDefault);
            gDefaults.Add(ShapeAttr.OnDblClickNotify, false);
            gDefaults.Add(ShapeAttr.Button, false);
            gDefaults.Add(ShapeAttr.OneD, false);
            gDefaults.Add(ShapeAttr.Hidden, HiddenDefault);
            gDefaults.Add(ShapeAttr.Print, true);

            // Diagram attributes.
            gDefaults.Add(ShapeAttr.EditAs, EditAs.Group);
            gDefaults.Add(ShapeAttr.DiagramStyle, 0);
            gDefaults.Add(ShapeAttr.DiagramRelationsTable, null);
            gDefaults.Add(ShapeAttr.DiagramScaleX, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.DiagramScaleY, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.DiagramFontSize, 12);
            gDefaults.Add(ShapeAttr.DiagramConstrainBounds, null);
            gDefaults.Add(ShapeAttr.DiagramBaseTextScale, ConvertUtilCore.DoubleToFixed(1.0));
            gDefaults.Add(ShapeAttr.DiagramAutoFormat, false);
            gDefaults.Add(ShapeAttr.DiagramReverse, false);
            gDefaults.Add(ShapeAttr.DiagramAutoLayout, false);
            gDefaults.Add(ShapeAttr.PseudoInline, false);

            // Attributes that are not like normal properties in Escher.
            gDefaults.Add(ShapeAttr.Flip, FlipOrientation.None);
            gDefaults.Add(ShapeAttr.WrapType, WrapTypeDefault);
            gDefaults.Add(ShapeAttr.WrapSide, WrapSide.Default);
            gDefaults.Add(ShapeAttr.AnchorLocked, AnchorLockedDefault);
            gDefaults.Add(ShapeAttr.CoordOriginX, 0);
            gDefaults.Add(ShapeAttr.CoordOriginY, 0);
            // This is the default value for coord size for all shapes except group. For group it is 1000.
            // I think it is okay to have only one default value here because all requests for coord size
            // when it is missing, will actually be satisfied from the shape definitions resource.
            gDefaults.Add(ShapeAttr.CoordSizeWidth, 21600);
            gDefaults.Add(ShapeAttr.CoordSizeHeight, 21600);
            gDefaults.Add(ShapeAttr.ImageBytes, null);
            gDefaults.Add(ShapeAttr.ImageTitle, String.Empty);
            gDefaults.Add(ShapeAttr.ImageSourceFullName, String.Empty);
            gDefaults.Add(ShapeAttr.BorderTop, new Border());
            gDefaults.Add(ShapeAttr.BorderLeft, new Border());
            gDefaults.Add(ShapeAttr.BorderBottom, new Border());
            gDefaults.Add(ShapeAttr.BorderRight, new Border());
            gDefaults.Add(ShapeAttr.LineImageBytes, null);
            gDefaults.Add(ShapeAttr.FillImageBytes, null);
            gDefaults.Add(ShapeAttr.OleObject, null);
            gDefaults.Add(ShapeAttr.OleProgID, String.Empty);
            gDefaults.Add(ShapeAttr.OleSourceFullName, String.Empty);
            gDefaults.Add(ShapeAttr.OleSourceItem, String.Empty);
            gDefaults.Add(ShapeAttr.OleAutoUpdate, false);
            gDefaults.Add(ShapeAttr.OleLocked, false);
            gDefaults.Add(ShapeAttr.OleLinkType, OleLinkType.Picture);
            gDefaults.Add(ShapeAttr.OleFormatUpdateType, 0);
            gDefaults.Add(ShapeAttr.HyperlinkTarget, String.Empty);
            gDefaults.Add(ShapeAttr.LineFillPresetTexture, PresetTextureCore.Custom);
            gDefaults.Add(ShapeAttr.FillPresetTexture, PresetTextureCore.Custom);
            gDefaults.Add(ShapeAttr.ShapeId, ShapeIdDefault);
            gDefaults.Add(ShapeAttr.Left, LeftDefault);
            gDefaults.Add(ShapeAttr.Top, TopDefault);
            gDefaults.Add(ShapeAttr.Width, WidthDefault);
            gDefaults.Add(ShapeAttr.Height, HeightDefault);
            gDefaults.Add(ShapeAttr.LeftPercent, LeftPercentDefault);
            gDefaults.Add(ShapeAttr.TopPercent, TopPercentDefault);
            gDefaults.Add(ShapeAttr.WidthPercent, WidthPercentDefault);
            gDefaults.Add(ShapeAttr.HeightPercent, HeightPercentDefault);
            gDefaults.Add(ShapeAttr.RelativeWidth, RelativeHorizontalSize.Default);
            gDefaults.Add(ShapeAttr.RelativeHeight, RelativeVerticalSize.Default);
            gDefaults.Add(ShapeAttr.ZOrder, 0);
            gDefaults.Add(ShapeAttr.ShapeType, ShapeType.NonPrimitive);
            gDefaults.Add(ShapeAttr.InkAnnotation, false);
            gDefaults.Add(ShapeAttr.DmlEffectExtentBottom, 0);
            gDefaults.Add(ShapeAttr.DmlEffectExtentLeft, 0);
            gDefaults.Add(ShapeAttr.DmlEffectExtentRight, 0);
            gDefaults.Add(ShapeAttr.DmlEffectExtentTop, 0);


            gDefaults.Add(ShapeAttr.SigSetupAddlXml, null);
            gDefaults.Add(ShapeAttr.SigSetupAllowComments, false);
            gDefaults.Add(ShapeAttr.SigSetupId, null);
            gDefaults.Add(ShapeAttr.IsSignatureLine, false);
            gDefaults.Add(ShapeAttr.SigSetupProvId, null);
            gDefaults.Add(ShapeAttr.SigSetupShowSignDate, true);
            gDefaults.Add(ShapeAttr.SigSetupSignInst, null);
            gDefaults.Add(ShapeAttr.SigSetupSignInstSet, false);
            gDefaults.Add(ShapeAttr.SigSetupProvUrl, null);
            gDefaults.Add(ShapeAttr.SigSetupSuggSigner, null);
            gDefaults.Add(ShapeAttr.SigSetupSuggSigner2, null);
            gDefaults.Add(ShapeAttr.SigSetupSuggSignerEmail, null);

            gDefaults.Add(ShapeAttr.Sys_LayoutWidth, WidthDefault);
            gDefaults.Add(ShapeAttr.Sys_LayoutHeight, HeightDefault);
        }

        /// <summary>
        /// Stores default values for all shape attributes.
        /// </summary>
        private static readonly ShapePr gDefaults;

        internal const bool BehindTextDefault = false;
        internal const bool AnchorLockedDefault = false;
        internal const bool AllowInCellDefault = true;
        internal const bool HiddenDefault = false;
        internal const bool AllowOverlapDefault = true;

        internal const int DmlEffectExtentLeftDefault = 0;
        internal const int DmlEffectExtentTopDefault = 0;
        internal const int DmlEffectExtentRightDefault = 0;
        internal const int DmlEffectExtentBottomDefault = 0;

        /// <summary>
        /// In points.
        /// </summary>
        internal const double GeoTextSizeDefault = 36.0;

        internal const int ShapeIdDefault = 0;
        internal const string ShapeNameDefault = "";

        internal const HorizontalAlignment HorizontalAlignmentDefault = HorizontalAlignment.None;
        internal const VerticalAlignment VerticalAlignmentDefault = VerticalAlignment.None;
        internal const RelativeHorizontalPosition RelativeHorizontalPositionDefault = RelativeHorizontalPosition.Default;
        internal const RelativeVerticalPosition RelativeVerticalPositionDefault = RelativeVerticalPosition.TextFrameDefault;

        internal const WrapType WrapTypeDefault = WrapType.None;

        internal const double LeftDefault = 0.0;
        internal const double TopDefault = 0.0;
        internal const double WidthDefault = 0.0;
        internal const double HeightDefault = 0.0;

        internal const int LeftPercentDefault = 0;
        internal const int TopPercentDefault = 0;
        internal const int WidthPercentDefault = 0;
        internal const int HeightPercentDefault = 0;
    }
}
