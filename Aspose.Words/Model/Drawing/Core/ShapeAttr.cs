// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2006 by Roman Korchagin

using Aspose.Words.Drawing.Ole.Core;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Defines shape attributes that occur in the model.
    /// For more info see VML, OOXML and Word 2007 Drawing Format specifications.
    /// </summary>
    [CppConstexpr]
    internal static class ShapeAttr
    {
        // 0x0000 - 0x003f
        #region Transform Attributes

        /// <summary>
        /// Fixed. Degrees. Default 0.
        /// Positive value corresponds to clockwise rotation angle.
        /// </summary>
        internal const int TransformRotation = 4;

        #endregion

        // 0x0040 - 0x007f
        #region Protection Attributes

        /// <summary>
        /// Default false.
        /// </summary>
        internal const int LockAgainstUngrouping = 118;
        /// <summary>
        /// No rotation. Default false.
        /// </summary>
        internal const int LockRotation = 119;
        /// <summary>
        /// Don't allow changes in aspect ratio. Default false.
        /// </summary>
        internal const int LockAspectRatio = 120;
        /// <summary>
        /// Don't allow the shape to be moved. Default false.
        /// </summary>
        internal const int LockPosition = 121;
        /// <summary>
        /// Shape may not be selected. Default false.
        /// </summary>
        internal const int LockAgainstSelect = 122;
        /// <summary>
        /// No cropping this shape. Default false.
        /// </summary>
        internal const int LockCropping = 123;
        /// <summary>
        /// Edit Points not allowed. Default false.
        /// </summary>
        internal const int LockVertices = 124;
        /// <summary>
        /// Do not edit text. Default false.
        /// </summary>
        internal const int LockText = 125;
        /// <summary>
        /// Do not adjust. Default false.
        /// </summary>
        internal const int LockAdjustHandles = 126;
        /// <summary>
        /// Do not group this shape. Default false.
        /// </summary>
        internal const int LockAgainstGrouping = 127;

        #endregion

        // 0x0080 - 0x00bf
        #region Textbox Attributes

        /// <summary>
        /// int. id for the textbox.
        ///
        /// Although this attribute will be present in the model during write to DOC,
        /// it does not have any meaning for the model.
        ///
        /// This is an index into plcfTxbxs, but need to deduct one and shift right by 16 bits.
        /// int txid = (int)((txbxIdx + 1) shl 16);
        ///
        /// The lower 16 bits indicate the index of the linked checkbox.
        ///
        /// This attribute is only used during reading and writing into binary DOC.
        /// There is a two-way link between shape and txbx in a DOC file and during read.
        ///
        /// Also see OleTxid. OleTxid is written instead of TextboxTxid for OLE objects.
        /// </summary>
        internal const int TextboxTxid = 128;
        /// <summary>
        /// int. margins relative to shape's inscribed text rectangle (in EMUs)
        /// Default 1/10inch
        /// </summary>
        internal const int TextboxLeft = 129;
        /// <summary>
        /// int. margins relative to shape's inscribed text rectangle (in EMUs)
        /// Default 1/20inch
        /// </summary>
        internal const int TextboxTop = 130;
        /// <summary>
        /// int. margins relative to shape's inscribed text rectangle (in EMUs)
        /// Default 1/10inch
        /// </summary>
        internal const int TextboxRight = 131;
        /// <summary>
        /// int. margins relative to shape's inscribed text rectangle (in EMUs)
        /// Default 1/20inch
        /// </summary>
        internal const int TextboxBottom = 132;
        /// <summary>
        /// <see cref="Aspose.Words.Drawing.TextBoxWrapMode"/>. Specifies how text wraps inside the shape.
        /// </summary>
        internal const int TextboxWrapMode = 133;
        /// <summary>
        /// int. Text zoom/scale (used if fFitTextToShape), default 0.
        /// </summary>
        internal const int TextboxZoom = 134;
        /// <summary>
        /// <see cref="TextBoxAnchor"/>. How to anchor the text. Default Top.
        /// </summary>
        internal const int TextboxAnchor = 135;
        /// <summary>
        /// <see cref="LayoutFlow"/>. Default HorZ
        /// </summary>
        internal const int TextboxLayoutFlow = 136;
        /// <summary>
        /// <see cref="FontRotation"/>. Font rotation. Default Right.
        /// </summary>
        internal const int TextboxFontRotation = 137;
        /// <summary>
        /// int. ID of the next shape (used by Word for linked textboxes).
        /// </summary>
        internal const int TextboxNextShapeId = 138;
        /// <summary>
        /// int. Bi-Di Text direction. Default LTR.
        /// </summary>
        internal const int TextDirection = 139;
        /// <summary>
        /// int. Seen in TestCommunity15706.doc, value 2.
        /// Untested.
        /// Default 1.
        /// Count of columns.
        /// Ver: XP
        /// </summary>
        internal const int TextboxColumns = 140;
        /// <summary>
        /// int. Seen BusinessBrochure.doc, value 73152.
        /// Untested.
        /// Default 91440.
        /// EMUs, column margin on both sides.
        /// Ver: XP
        /// </summary>
        internal const int TextboxColumnMargin = 141;
        /// <summary>
        /// string. Original ID of the next shape (used by Word for linked textboxes).
        /// It is a temporary attribute that should never appear in AW model after load.
        /// </summary>
        internal const int Sys_TextboxNextShapeIdRaw = 142;
        /// <summary>
        /// TRUE if single click selects text, FALSE if two clicks. Default true.
        /// </summary>
        internal const int TextboxSelectText = 187;
        /// <summary>
        /// use host's margin calculations. Default false.
        /// </summary>
        internal const int TextboxAutoTextMargin = 188;
        /// <summary>
        /// Rotate text with shape. Default false.
        /// </summary>
        internal const int TextboxRotateText = 189;
        /// <summary>
        /// Size shape to fit text size. Default false.
        /// </summary>
        internal const int TextboxFitShapeToText = 190;
        /// <summary>
        /// Size text to fit shape size. Default false.
        /// </summary>
        internal const int TextboxFitTextToShape = 191;

        #endregion

        // 0x00c0 - 0x00ff
        #region GeoText (WordArt) Attributes

        /// <summary>
        /// UNICODE text string
        /// </summary>
        internal const int GeoTextText = 192;
        /// <summary>
        /// RTF text string
        /// </summary>
        internal const int GeoTextRtf = 193;
        /// <summary>
        /// <see cref="TextPathAlignment"/>. alignment on curve
        /// </summary>
        internal const int GeoTextAlign = 194;
        /// <summary>
        /// Fixed. Font size in points.
        /// </summary>
        internal const int GeoTextSize = 195;
        /// <summary>
        /// Fixed. Adjust the spacing between characters (1.0 is normal).
        /// </summary>
        internal const int GeoTextSpacing = 196;
        /// <summary>
        /// String. Font name.
        /// </summary>
        internal const int GeoTextFont = 197;
        /// <summary>
        /// Untested.
        /// Ver: 2000
        /// String. To preserve CSS font selectors.
        /// Default empty string.
        /// </summary>
        internal const int GeoTextCssFont = 198;
        /// <summary>
        /// bool. Reverse row order.
        /// By default multiple rows of text are laid out with the first at the top for horizontal
        /// text and with the first at the left for vertical text, this flag reverses that behavior
        /// (bottom to top or right to left).
        /// Default false.
        /// </summary>
        internal const int GeoTextReverseRows = 240;
        /// <summary>
        /// bool. True if the text effect properties (gtext*) are used.
        /// False if these properties are ignored.
        /// </summary>
        internal const int GeoTextOn = 241;
        /// <summary>
        /// bool. Rotate characters
        /// </summary>
        internal const int GeoTextVertical = 242;
        /// <summary>
        /// bool. Kern characters
        /// </summary>
        internal const int GeoTextKerning = 243;
        /// <summary>
        /// bool. Tightening or tracking is on.
        /// </summary>
        internal const int GeoTextTight = 244;
        /// <summary>
        /// bool. Stretch to fit shape
        /// </summary>
        internal const int GeoTextStretch = 245;
        /// <summary>
        /// bool. Char bounding box
        /// </summary>
        internal const int GeoTextShrinkFit = 246;
        /// <summary>
        /// bool. Scale text-on-path
        /// </summary>
        internal const int GeoTextBestFit = 247;
        /// <summary>
        /// bool. Stretch char height. Mall characters same height.
        /// </summary>
        internal const int GeoTextNormalize = 248;
        /// <summary>
        /// bool. Do not measure along path.
        /// </summary>
        internal const int GeoTextDxMeasure  = 249;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int GeoTextBold = 250;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int GeoTextItalic = 251;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int GeoTextUnderline = 252;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int GeoTextShadow = 253;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int GeoTextSmallCaps = 254;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int GeoTextStrikeThrough = 255;

        #endregion

        // 0x0100 - 0x013f
        #region Image (Blip) Attributes

        /// <summary>
        /// Fixed. Fraction of the image width or height.
        /// </summary>
        internal const int ImageCropTop = 256;
        internal const int ImageCropBottom = 257;
        internal const int ImageCropLeft = 258;
        internal const int ImageCropRight = 259;

        /// <summary>
        /// Specifies the width of the extents rectangle in EMUs. This rectangle shall dictate the size
        /// of the object as displayed (the result of any scaling to the original object).
        /// </summary>
        internal const int GraphicFrameExtWidth = 260;

        /// <summary>
        /// Specifies the length of the extents rectangle in EMUs. This rectangle shall dictate the size
        /// of the object as displayed (the result of any scaling to the original object).
        /// </summary>
        internal const int GraphicFrameExtHeight = 261;

        /// <summary>
        /// Color. transparent color.
        /// The default value for this property is 0xFFFFFFFF, but we don't yet support 0xff in the top byte
        /// that means "ignore this color".
        /// </summary>
        internal const int ImageTransparent = 263;
        /// <summary>
        /// Fixed. Contrast setting. Min value is 0, max value is 0x7fffffff, but it is not linear.
        /// There is no documentation for the algorithm, but I guess it in <see cref="ImageData.ContrastToPercent"/>.
        /// </summary>
        internal const int ImageContrast = 264;
        /// <summary>
        /// Fixed. Brightness setting. Has to be a fixed value -0.5 to 0.5.
        /// </summary>
        internal const int ImageBrightness = 265;
        /// <summary>
        /// Fixed. gamma. Have never seen it in MS Word.
        /// Actually, this is not even in the SPEC.
        /// </summary>
        internal const int ImageGamma = 266;
        /// <summary>
        /// Int. Host-defined ID for OLE objects. MS Word spec says this is "pictureId", but
        /// I've discovered this is actually TXID for floating OLE objects.
        /// This property is ignored during read and synthesized on write.
        /// Also see TextboxTxid.
        /// </summary>
        internal const int OleTxid = 267;
        /// <summary>
        /// Int. This property specifies a shadow color for pictures with a double shadow
        /// used in black-and-white display mode.
        /// The SPEC says: This value SHOULD be ignored.
        /// The default value for this property is 0x100000F4, but we don't support color flags yet.
        /// Seen in TestShadow.doc. Seen 0x100000f4 in TestDefect1402.doc.
        /// MSOCLR type.
        /// </summary>
        internal const int ImageDblCrMod = 268;
        /// <summary>
        /// Int. This property specifies the fill color modification used in black-and-white display mode.
        /// Seen in TestDefect1402.doc.
        /// MSOCLR
        /// </summary>
        internal const int ImageFillCrMod = 269;
        /// <summary>
        /// Int. This property specifies the line color modification used in black-and-white display mode.
        /// Seen in TestDefect1402.doc.
        /// MSOCLR
        /// </summary>
        internal const int ImageLineCrMod = 270;
        /// <summary>
        /// This property specifies the transparent pixel extended color.
        /// </summary>
        internal const int ImageTransparentExt = 277;
        /// <summary>
        /// This property specifies the color modification of the transparent pixel extended color.
        /// </summary>
        internal const int ImageTransparentExtMod = 279;
        /// <summary>
        /// Int. This property specifies the color used to recolor the image.
        /// Seen 0xffffffff in TestDefect1402.doc
        /// Ver: XP
        /// </summary>
        internal const int ImageRecolor = 282;
        /// <summary>
        /// Int. This property specifies the extended color used to recolor the image.
        /// Seen 0xffffffff in TestDefect1402.doc
        /// </summary>
        internal const int ImageRecolorExt = 283;
        /// <summary>
        /// Int. MUST be 0xFFFFFFFF, and MUST be ignored.
        /// Seen 0xffffffff in TestDefect1402.doc
        /// </summary>
        internal const int Undocumented284 = 284;
        /// <summary>
        /// Int. This property specifies the color modification of the extended color used to recolor the image.
        /// Seen 0x20000000 in TestDefect1402.doc
        /// </summary>
        internal const int ImageRecolorExtCrMod = 285;
        /// <summary>
        /// Seen in 1402. MUST be zero, and MUST be ignored.
        /// Complex property, no data.
        /// </summary>
        internal const int Undocumented286 = 286;
        /// <summary>
        /// Int. MUST be 0xFFFFFFFF, and MUST be ignored.
        /// Seen 0xffffffff in TestDefect1402.doc
        /// </summary>
        internal const int Undocumented287 = 287;
        /// <summary>
        /// bool. Seen in TestDefect1402.doc in extended flags. Default false? 0x0040
        /// When doing a color modification to a picture, leave grays unmodified.
        /// Ver: XP
        /// </summary>
        internal const int ImagePreserveGrays = 313;
        /// <summary>
        /// bool, Do not hit test the picture, default false. 0x0008
        /// </summary>
        internal const int ImageNoHitTest = 316;
        /// <summary>
        /// bool, grayscale display, default false. 0x0004
        /// </summary>
        internal const int ImageGrayScale = 317;
        /// <summary>
        /// bool, bi-level display, default false. 0x0002
        /// </summary>
        internal const int ImageBiLevel = 318;
        /// <summary>
        /// bool, Server is active (OLE objects only), default false. 0x0001
        /// </summary>
        internal const int ImageActive = 319;

        #endregion


        // This property set specifies the structure of a shape, including its points and how those points
        // are connected. These properties also specify how the shape scales in size and how the user may
        // manipulate that scaling.
        // 0x0140 - 0x017f
        #region Geometry Attributes

        /// <summary>
        /// Array of <see cref="PathPoint"/>.
        /// Specifies the set of points that make up this shape.
        /// Corresponds to 2.3.6.7 pVertices_complex in the DOC SPEC.
        /// </summary>
        internal const int GeometryVertices = 325;
        /// <summary>
        /// Array of <see cref="PathInfo"/>.
        /// Specifies how the vertices specified by the <see cref="GeometryVertices"/> property are connected.
        /// Corresponds to 2.3.6.8 pSegmentInfo in the DOC SPEC.
        /// This attribute MUST be present and be up to date with GeometryVertices.
        /// </summary>
        internal const int GeometrySegmentInfo = 326;
        /// <summary>
        /// Int. Adjustment values corresponding to the positions of the adjust handles of the shape.
        /// The number of values used and their allowable ranges vary from shape type to shape type.
        /// Default 0.
        /// </summary>
        internal const int GeometryAdjust1 = 327;
        internal const int GeometryAdjust2 = 328;
        internal const int GeometryAdjust3 = 329;
        internal const int GeometryAdjust4 = 330;
        internal const int GeometryAdjust5 = 331;
        internal const int GeometryAdjust6 = 332;
        internal const int GeometryAdjust7 = 333;
        internal const int GeometryAdjust8 = 334;
        internal const int GeometryAdjust9 = 335;
        internal const int GeometryAdjust10 = 336;
        /// <summary>
        /// Array of <see cref="PathPoint"/>. These are G unit values.
        /// Connector locations for shapes imported from clipart.
        /// </summary>
        internal const int GeometryConnectLocs = 337;
        /// <summary>
        /// This property specifies an array of angles corresponding to the connection sites
        /// in the pConnectionSites_complex property that are used to determine the direction
        /// that a connector links to the corresponding connection site.
        ///
        /// Array of <see cref="int"/>. But these are actually Fixed values.
        /// </summary>
        internal const int GeometryConnectAngles = 338;
        /// <summary>
        /// int. A signed integer x coordinate in geometry space that specifies a value above which limousine
        /// scaling is used.
        /// The spec says default is 0x8000000, but we default to 0 at the moment!
        ///
        /// Limo stretches are points on a shape's edge that define where and how a shape may be stretched
        /// by a user in a graphical editor.
        ///
        /// 2.3.6.22 xLimo in the DOC SPEC
        /// </summary>
        internal const int GeometryXLimo = 339;
        /// <summary>
        /// int. A signed integer y coordinate in geometry space that specifies a value above which limousine
        /// scaling is used.
        /// The spec says default is 0x8000000, but we default to 0 at the moment!
        /// 2.3.6.23 yLimo in the DOC SPEC.
        /// </summary>
        internal const int GeometryYLimo = 340;
        /// <summary>
        /// This property specifies an array of adjust handles which allow a user to manipulate the
        /// geometry of this shape.
        /// Array of <see cref="Handle"/>.
        /// </summary>
        internal const int GeometryHandles = 341;
        /// <summary>
        /// This property specifies a set of formulas used to calculate values for use in defining
        /// the geometry of this shape.
        /// Array of <see cref="Formula"/>.
        /// </summary>
        internal const int GeometryFormulas = 342;
        /// <summary>
        /// This property specifies an array of rectangles specifying how text should be inscribed
        /// within this shape.
        /// 2.3.6.28 pInscribe in the DOC SPEC.
        /// Array of <see cref="PathRectangle"/>. Corresponds to path.textboxrect in VML.
        /// </summary>
        internal const int GeometryPathTextBoxRects = 343;
        /// <summary>
        /// This property specifies where connection points are on the shape.
        /// <see cref="ConnectionSiteType"/>. Corresponds to o:connecttype in WordML.
        /// </summary>
        internal const int GeometryConnectionSiteType = 344;
        /// <summary>
        /// A bit that specifies whether to display the shadow of this shape. The value SHOULD be ignored.
        /// Default true.
        /// </summary>
        internal const int GeometryShadowOK = 378;
        /// <summary>
        /// A bit that specifies whether to display the extrusion effect of this shape.
        /// Default true.
        /// </summary>
        internal const int GeometryThreeDOK = 379;
        /// <summary>
        /// A bit that specifies whether to display the line of this shape.
        /// Default true.
        /// </summary>
        internal const int GeometryLineOK = 380;
        /// <summary>
        /// A bit that specifies whether to display the text aligned to this shape.
        /// Default false.
        /// </summary>
        internal const int GeometryGTextOK = 381;
        /// <summary>
        /// A bit that specifies whether the fill is aligned to the shape. A value of 0x0 specifies that
        /// the fill is aligned to the bounding rectangle of the shape. A value of 0x1 specifies that
        /// the fill is aligned to the shape itself.
        /// Default false.
        /// </summary>
        internal const int GeometryFillShadeShapeOK = 382;
        /// <summary>
        /// A bit that specifies whether to display the fill of this shape.
        /// Default true.
        /// </summary>
        internal const int GeometryFillOK = 383;

        #endregion


        // 0x0180 - 0x01bf
        #region Fill Attributes

        /// <summary>
        /// <see cref="FillType"/> enumeration. Type of fill. Default Solid.
        /// </summary>
        internal const int FillType = 384;
        /// <summary>
        /// Foreground fill color. Default white.
        /// </summary>
        internal const int FillColor = 385;
        /// <summary>
        /// Fixed.
        /// </summary>
        internal const int FillOpacity = 386;
        /// <summary>
        /// Background color. Default white.
        /// Can have special values such as 0xeff00266 which means "fill lighten(102)",
        /// where I guess 0x66 means "102" and 0x00200 means "lighten".
        /// VA: VmlColor.ColorToVml decodes such values.
        /// </summary>
        internal const int FillBackColor = 387;
        /// <summary>
        /// Fixed. Used only for gradient fills.
        /// </summary>
        internal const int FillBackOpacity = 388;
        /// <summary>
        /// Color. Modification for BW views? Undocumented. Have not seen in files.
        /// </summary>
        internal const int FillCrMod = 389;
        /// <summary>
        /// string.
        /// </summary>
        internal const int FillBlipName = 391;
        /// <summary>
        /// int.
        /// </summary>
        internal const int FillBlipNameFlags = 392;
        /// <summary>
        /// int? Never seen in files.
        /// </summary>
        internal const int FillWidth = 393;
        /// <summary>
        /// int? Never seen in files.
        /// </summary>
        internal const int FillHeight = 394;
        /// <summary>
        /// Fixed. Gradient angle in degrees.
        /// </summary>
        internal const int FillAngle = 395;
        /// <summary>
        /// int. Linear gradient fill focus. Integer percent -100..100
        /// </summary>
        internal const int FillFocus = 396;
        /// <summary>
        /// Fixed. Focus position. Fraction of the shape size.
        /// </summary>
        internal const int FillToLeft = 397;
        /// <summary>
        /// Fixed. Focus position. Fraction of the shape size.
        /// </summary>
        internal const int FillToTop = 398;
        /// <summary>
        /// Fixed. Focus position. Fraction of the shape size.
        /// </summary>
        internal const int FillToRight = 399;
        /// <summary>
        /// Fixed. Focus position. Fraction of the shape size.
        /// </summary>
        internal const int FillToBottom = 400;
        /// <summary>
        /// Fixed. Seen in TestDefect1402.doc
        /// </summary>
        internal const int FillRectLeft = 401;
        /// <summary>
        /// Fixed. Seen in TestDefect1402.doc
        /// </summary>
        internal const int FillRectTop = 402;
        /// <summary>
        /// Fixed. Seen in TestDefect1402.doc
        /// </summary>
        internal const int FillRectRight = 403;
        /// <summary>
        /// Fixed. Seen in TestDefect1402.doc
        /// </summary>
        internal const int FillRectBottom = 404;
        /// <summary>
        /// <see cref="FillDimensionType"/>. Specifies units for fill dimensions.
        /// </summary>
        internal const int FillDimensionType = 405;
        /// <summary>
        /// int. Identifies one of the preset gradients. Exists in DOC and RTF only. WML and DOCX don't have it.
        /// MS Word figures this value out itself, therefore we ignore it. Does not occur in the model.
        /// </summary>
        internal const int FillShadePreset = 406;
        /// <summary>
        /// Array of <see cref="GradientColor"/>.
        /// </summary>
        internal const int FillShadeColors = 407;
        /// <summary>
        /// int. The units are according to FillDzType. Undocumented.
        /// </summary>
        internal const int FillOriginX = 408;
        /// <summary>
        /// int. The units are according to FillDzType. Undocumented.
        /// </summary>
        internal const int FillOriginY = 409;
        /// <summary>
        /// int. The units are according to FillDzType. Undocumented.
        /// </summary>
        internal const int FillShapeOriginX = 410;
        /// <summary>
        /// int. The units are according to FillDzType. Undocumented.
        /// </summary>
        internal const int FillShapeOriginY = 411;
        /// <summary>
        /// <see cref="GradientType"/>. Type of shading for gradient fill.
        /// </summary>
        internal const int FillShadeType = 412;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.7.33 fillColorExt.
        /// This property specifies the extended foreground color.
        /// </summary>
        /// <remarks>
        /// The RGB value of the base foreground color before modification.
        /// <para>In WordML this is the value inside rgb() brackets.</para>
        /// <para>ex: "fillcolor="#228bb5 [rgb(34,139,181) lighten(102)]"</para>
        /// </remarks>
        internal const int FillColorExt = 414;
        /// <summary>
        /// Undocumented.
        /// Seen in TestGeometry.doc, value -1. In extended properties.
        /// Seein BusinessBrochureBrief.doc, value 0xff20.
        /// </summary>
        internal const int FillColorExtCMY = 415;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.7.35 fillColorExtMod.
        /// This property specifies the color modification of the extended foreground color.
        /// </summary>
        /// <remarks>
        /// Degree of the foreground color lightening or darkening.
        /// Calculated as (1 - MsoTintShade) * 255, where MsoTintShade is the shading double value between 0 and 1.
        /// <para>In WordML this is the value inside brackets with the modifier name (lighten or darken).</para>
        /// <para>ex1: "fillcolor="#228bb5 [rgb(34,139,181) lighten(102)]"</para>
        /// <para>ex2: "fillcolor="#228bb5 [rgb(34,139,181) darken(179)]"</para>
        /// </remarks>
        internal const int FillColorExtMod = 416;
        /// <summary>
        /// Undocumented. Seen in TestGeometry.doc. Complex property. Seen empty value only. In extended properties.
        /// </summary>
        internal const int Undocumented417 = 417;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.7.37 fillBackColorExt.
        /// This property specifies the extended background color.
        /// </summary>
        /// <remarks>
        /// Similar with <see cref="FillColorExt"/>.
        /// </remarks>
        internal const int FillBackColorExt = 418;
        /// <summary>
        /// Undocumented. Seen in BusinessBrochure.doc, value 8. Simple.
        /// </summary>
        internal const int FillBackColorExtCMY = 419;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.7.39 fillBackColorExtMod.
        /// This property specifies the color modification of the extended background color.
        /// </summary>
        /// <remarks>
        /// Similar with <see cref="FillColorExtMod"/>.
        /// </remarks>
        internal const int FillBackColorExtMod = 420;
        /// <summary>
        /// Undocumented. Seen in TestWatermark.doc.
        /// </summary>
        internal const int Undocumented421 = 421;
        /// <summary>
        /// Undocumented. Seen in TestGeometry.doc, value -1. In extended properties.
        /// Seen in BusinessBrochure.doc, value 0.
        /// </summary>
        internal const int FillColorExtK = 422;
        /// <summary>
        /// Undocumented. Seen in BusinessBrochure.doc, value 0.
        /// </summary>
        internal const int FillBackColorExtK = 423;
        /// <summary>
        /// bool. Extended fill flag. Not exactly sure what it means.
        /// </summary>
        internal const int FillRecolorAsPicture = 441;
        /// <summary>
        /// bool. Extended fill flag. Corresponds to 'Fill Effects | Gradient | Rotate fill effect with shape' setting in MS Word.
        /// </summary>
        internal const int FillUseShapeAnchor = 442;
        /// <summary>
        /// bool. Is shape filled?
        /// </summary>
        internal const int Filled = 443;
        /// <summary>
        /// bool. undocumented.
        /// </summary>
        internal const int FillHitTest = 444;
        /// <summary>
        /// bool. undocumented.
        /// </summary>
        internal const int FillShape = 445;
        /// <summary>
        /// bool. undocumented.
        /// </summary>
        internal const int FillUseRect = 446;
        /// <summary>
        /// bool. undocumented.
        /// Corresponds to o:detectmouseclick in WordML
        /// </summary>
        internal const int FillNoFillHitTest = 447;

        #endregion


        // 0x01c0 - 0x01ff
        #region Line Attributes

        /// <summary>
        /// Color of line. Default is black.
        /// </summary>
        internal const int LineColor = 448;
        /// <summary>
        /// Fixed. Line color opacity.
        /// </summary>
        internal const int LineOpacity = 449;
        /// <summary>
        /// Background color, default is white.
        /// </summary>
        internal const int LineBackColor = 450;
        /// <summary>
        /// Color. Modification for BW Views. never seen.
        /// </summary>
        internal const int LineCrMod = 451;
        /// <summary>
        /// <see cref="Core.LineFillType"/> enumeration. Type of line, default is solid.
        /// </summary>
        internal const int LineFillType = 452;
        /// <summary>
        /// String. Seen "" in TestDefect1402.doc. Never seen, unsupported at the moment.
        /// </summary>
        internal const int LineFillBlipName = 454;
        /// <summary>
        /// Int. Seen 0 in TestDefect1402.doc.
        /// </summary>
        internal const int LineFillBlipNameFlags = 455;
        /// <summary>
        /// Int? Seen 0 in TestDefect1402.doc.
        /// </summary>
        internal const int LineFillWidth = 456;
        /// <summary>
        /// Int? Seen 0 in TestDefect1402.doc.
        /// </summary>
        internal const int LineFillHeight = 457;
        /// <summary>
        /// <see cref="FillDimensionType"/>. Specifies units for fill dimensions.
        /// </summary>
        internal const int LineFillDimensionType = 458;
        /// <summary>
        /// EMUs, line width. 1pt == 12700 EMUs. Default 9525 (0.75pt). Stored as EMUs in the model.
        /// </summary>
        internal const int LineWidth = 459;
        /// <summary>
        /// Fixed. ratio.
        /// </summary>
        internal const int LineMiterLimit = 460;
        /// <summary>
        /// <see cref="ShapeLineStyle"/> enumeration. Default single.
        /// </summary>
        internal const int LineStyle = 461;
        /// <summary>
        /// <see cref="DashStyle"/>. Default Solid. Can be overridden by LineDashStyle.
        /// </summary>
        internal const int LineDashStyle = 462;
        /// <summary>
        /// Complex property. An array that defines a custom dash pattern. Never seen, not supported at the moment.
        /// </summary>
        internal const int LineDashData = 463;
        /// <summary>
        /// <see cref="ArrowType"/>. Arrow at start, default NoEnd
        /// </summary>
        internal const int LineStartArrow = 464;
        /// <summary>
        /// <see cref="ArrowType"/>. Arrow at end, default NoEnd
        /// </summary>
        internal const int LineEndArrow = 465;
        /// <summary>
        /// <see cref="ArrowWidth"/>.
        /// </summary>
        internal const int LineStartArrowWidth = 466;
        /// <summary>
        /// <see cref="ArrowLength"/>.
        /// </summary>
        internal const int LineStartArrowLength = 467;
        /// <summary>
        /// <see cref="ArrowWidth"/>.
        /// </summary>
        internal const int LineEndArrowWidth = 468;
        /// <summary>
        /// <see cref="ArrowLength"/>.
        /// </summary>
        internal const int LineEndArrowLength = 469;
        /// <summary>
        /// <see cref="JoinStyle"/>. Defines the join style of a polyline.
        /// </summary>
        internal const int LineJoinStyle = 470;
        /// <summary>
        /// <see cref="EndCap"/>. How to end lines.
        /// </summary>
        internal const int LineEndCapStyle = 471;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.8.28 lineColorExt.
        /// This property specifies the extended foreground color of the shape outline.
        /// </summary>
        /// <remarks>
        /// Similar with <see cref="FillColorExt"/>.
        /// </remarks>
        internal const int LineColorExt = 473;
        /// <summary>
        /// Undocumented. Seen in TestLeorMeller.doc int 0xffffffff
        /// </summary>
        internal const int LineColorExtCMY = 474;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.8.30 lineColorExtMod.
        /// This property specifies the color modification of the outline extended foreground color.
        /// </summary>
        /// <remarks>
        /// Similar with <see cref="FillColorExtMod"/>.
        /// </remarks>
        internal const int LineColorExtMod = 475;
        /// <summary>
        /// 2.3.8.31 reserved476. MUST be zero, and MUST be ignored.
        /// Undocumented. Seen in TestWatermark.doc. Complex property. Seen simple in arabic 4129.doc
        /// </summary>
        internal const int Undocumented476 = 476;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.8.32 lineBackColorExt.
        /// This property specifies the extended background color of the shape outline.
        /// </summary>
        /// <remarks>
        /// Similar with <see cref="FillColorExt"/>.
        /// </remarks>
        internal const int LineBackColorExt = 477;
        /// <summary>
        /// This is line background color in the "cmyk" notation.
        /// Seen in BusinessBrochureBrief.doc, 0x00feffff.
        /// In WordML this is v:stroke color2="#fffffe [rgb(255,255,254) cmyk(0,0,0,0)]"
        /// </summary>
        internal const int LineBackColorExtCMY = 478;
        /// <summary>
        /// SPEC: [MS-ODRAW]. 2.3.8.34 lineBackColorExtMod.
        /// This property specifies the color modification of the outline extended background color.
        /// </summary>
        /// <remarks>
        /// Similar with <see cref="FillColorExtMod"/>.
        /// </remarks>
        internal const int LineBackColorExtMod = 479;
        /// <summary>
        /// Undocumented. Seen in TestWatermark.doc. Complex property, no data.
        /// </summary>
        internal const int Undocumented480 = 480;
        /// <summary>
        /// Undocumented. Seen in TestLeorMeller.doc int 0xffffffff
        /// </summary>
        internal const int LineColorExtK = 481;
        /// <summary>
        /// Undocumented. Seen in BusinessBrochure.doc, value 0.
        /// </summary>
        internal const int LineBackColorExtK = 482;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int LineRecolorFillAsPicture = 503;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int LineUseShapeAnchor = 504;
        /// <summary>
        /// bool. True when Inset pen (the pen is aligned on the inside of the edge of the path).
        /// False when Center pen (line drawn at center of path stroke).
        /// If LineInsetPenOK is false, this value MUST be ignored.
        /// </summary>
        internal const int LineInsetPen = 505;
        /// <summary>
        /// bool. Specifies whether insetting the pen is allowed.
        /// </summary>
        internal const int LineInsetPenOk = 506;
        /// <summary>
        /// bool. Allow arrowheads if prop. is set
        /// </summary>
        internal const int LineArrowHeadsOK = 507;
        /// <summary>
        /// bool. Any line? Probably Stroked in VML.
        /// </summary>
        internal const int LineOn = 508;
        /// <summary>
        /// bool. Should we hit test lines? Undocumented.
        /// </summary>
        internal const int LineHitTest = 509;
        /// <summary>
        /// bool. Register pattern on shape. Undocumented.
        /// </summary>
        internal const int LineFillShape = 510;
        /// <summary>
        /// bool. Draw a dashed line if no line. Probably ForceDash in VML.
        /// </summary>
        internal const int LineNoLineDrawDash = 511;

        #endregion


        // 0x0200 - 0x023f
        #region Shadow Attributes

        /// <summary>
        /// <see cref="ShadowType"/>.
        /// </summary>
        internal const int ShadowType = 512;
        /// <summary>
        /// Color. Foreground shadow color.
        /// </summary>
        internal const int ShadowColor = 513;
        /// <summary>
        /// Color. Embossed color.
        /// </summary>
        internal const int ShadowHighlight = 514;
        /// <summary>
        /// Color. Modification for BW views. undocumented.
        /// </summary>
        internal const int ShadowCrMod = 515;
        /// <summary>
        /// Fixed.
        /// </summary>
        internal const int ShadowOpacity = 516;
        /// <summary>
        /// int. Shadow offset. EMUs.
        /// </summary>
        internal const int ShadowOffsetX = 517;
        /// <summary>
        /// int. Shadow offset. EMUs.
        /// </summary>
        internal const int ShadowOffsetY = 518;
        /// <summary>
        /// int. Double shadow offset. EMUs.
        /// </summary>
        internal const int ShadowSecondOffsetX = 519;
        /// <summary>
        /// int. Double shadow offset. EMUs.
        /// </summary>
        internal const int ShadowSecondOffsetY = 520;
        /// <summary>
        /// Fixed. undocumented.
        /// </summary>
        internal const int ShadowScaleXtoX = 521;
        /// <summary>
        /// Fixed. undocumented.
        /// </summary>
        internal const int ShadowScaleYtoX = 522;
        /// <summary>
        /// Fixed. undocumented.
        /// </summary>
        internal const int ShadowScaleXtoY = 523;
        /// <summary>
        /// Fixed. undocumented.
        /// </summary>
        internal const int ShadowScaleYtoY = 524;
        /// <summary>
        /// Fixed. Ratio of weight.
        /// </summary>
        internal const int ShadowPerspectiveX = 525;
        /// <summary>
        /// Fixed. Ratio of weight.
        /// </summary>
        internal const int ShadowPerspectiveY = 526;
        /// <summary>
        /// What units?
        /// </summary>
        internal const int ShadowWeight = 527;
        /// <summary>
        /// int? Undocumented.
        /// </summary>
        internal const int ShadowOriginX = 528;
        /// <summary>
        /// int? Undocumented.
        /// </summary>
        internal const int ShadowOriginY = 529;
        /// <summary>
        /// Undocumented. Seen in TestLeorMeller.doc int 0xffffffff
        /// </summary>
        internal const int ShadowColorExt = 530;
        /// <summary>
        /// Undocumented. Seen in TestLeorMeller.doc int 0xffffffff
        /// </summary>
        internal const int ShadowColorExtCMY = 531;
        /// <summary>
        /// Undocumented. Seen in TestLeorMeller.doc int 0x20000000
        /// </summary>
        internal const int ShadowColorExtMod = 532;
        /// <summary>
        /// Undocumented. Seen in TestWatermark.doc. Complex property.
        /// </summary>
        internal const int Undocumented533 = 533;
        /// <summary>
        /// This is shadow background color in the "cmyk" notation.
        /// Seen in BusinessBrochureBrief.doc, 0xdsd5db.
        /// In WordML this is color2="#dbd5d3 [rgb(219,213,211) cmyk(12.5,9.8,8.63,3.14)]"
        /// </summary>
        internal const int ShadowHighlightExt = 534;
        /// <summary>
        /// Undocumented. In extended properties.
        /// Seen in BusinessBrochure.doc, value 0x2c3241e8.
        /// </summary>
        internal const int ShadowHighlightExtCMY = 535;
        /// <summary>
        /// Seen in TestDefect1402.doc. 0xffffffff
        /// </summary>
        internal const int ShadowHighlightExtMod = 536;
        /// <summary>
        /// Undocumented. Seen in TestWatermark.doc. Complex property.
        /// </summary>
        internal const int Undocumented537 = 537;
        /// <summary>
        /// Undocumented. Seen in TestLeorMeller.doc int 0xffffffff
        /// </summary>
        internal const int ShadowColorExtK = 538;
        /// <summary>
        /// Undocumented. In extended properties.
        /// Seen in BusinessBrochure.doc, value 0x20.
        /// </summary>
        internal const int ShadowHighlightExtK = 539;
        /// <summary>
        /// bool. Shadow is on or not.
        /// </summary>
        internal const int ShadowOn = 574;
        /// <summary>
        /// bool. Excel5-style shadow
        /// </summary>
        internal const int ShadowObscured = 575;

        #endregion


        // 0x0240 - 0x027f
        #region Perspective Attributes

        /// <summary>
        /// <see cref="PerspectiveType"/>. Where transform applies.
        /// </summary>
        internal const int PerspectiveType = 576;
        /// <summary>
        /// int. The LONG values define a transformation matrix, effectively, each value is scaled by the perspectiveWeight parameter.
        /// </summary>
        internal const int PerspectiveOffsetX = 577;
        /// <summary>
        /// int. The LONG values define a transformation matrix, effectively, each value is scaled by the perspectiveWeight parameter.
        /// </summary>
        internal const int PerspectiveOffsetY = 578;
        /// <summary>
        /// int. The LONG values define a transformation matrix, effectively, each value is scaled by the perspectiveWeight parameter.
        /// </summary>
        internal const int PerspectiveScaleXToX = 579;
        /// <summary>
        /// int. The LONG values define a transformation matrix, effectively, each value is scaled by the perspectiveWeight parameter.
        /// </summary>
        internal const int PerspectiveScaleYToX = 580;
        /// <summary>
        /// int. The LONG values define a transformation matrix, effectively, each value is scaled by the perspectiveWeight parameter.
        /// </summary>
        internal const int PerspectiveScaleXToY = 581;
        /// <summary>
        /// int. The LONG values define a transformation matrix, effectively, each value is scaled by the perspectiveWeight parameter.
        /// </summary>
        internal const int PerspectiveScaleYToY = 582;
        /// <summary>
        /// int. undocumented.
        /// </summary>
        internal const int PerspectiveX = 583;
        /// <summary>
        /// int. undocumented.
        /// </summary>
        internal const int PerspectiveY = 584;
        /// <summary>
        /// Fixed? Scaling factor.
        /// </summary>
        internal const int PerspectiveWeight = 585;
        /// <summary>
        /// Fixed. undocumented.
        /// </summary>
        internal const int PerspectiveOriginX = 586;
        /// <summary>
        /// Fixed undocumented.
        /// </summary>
        internal const int PerspectiveOriginY = 587;
        /// <summary>
        /// bool.
        /// </summary>
        internal const int PerspectiveOn = 639;

        #endregion


        // 0x0280 - 0x02bf
        #region 3D Object Attributes

        /// <summary>
        /// Fixed-point 16.16
        /// </summary>
        internal const int TDSpecularAmount = 640;
        /// <summary>
        /// Fixed-point 16.16
        /// </summary>
        internal const int TDDiffuseAmount = 641;
        /// <summary>
        /// int? Default gives OK results
        /// </summary>
        internal const int TDShininess = 642;
        /// <summary>
        /// int. Specular edge thickness in EMUs.
        /// </summary>
        internal const int TDEdgeThickness = 643;
        /// <summary>
        /// int. Distance of extrusion in EMUs
        /// </summary>
        internal const int TDExtrudeForward = 644;
        /// <summary>
        /// int. EMUs
        /// </summary>
        internal const int TDExtrudeBackward = 645;
        /// <summary>
        /// <see cref="PlaneType"/>. Extrusion direction
        /// </summary>
        internal const int TDExtrudePlane = 646;
        /// <summary>
        /// Color. Basic color of extruded part of shape; the lighting model used will determine
        /// the exact shades used when rendering.
        /// </summary>
        internal const int TDExtrusionColor = 647;
        /// <summary>
        /// Color. Modification for BW views
        /// </summary>
        internal const int TDCrMod = 648;
        /// <summary>
        /// Seen in TestDefect1402.doc, 0xffffffff
        /// </summary>
        internal const int TDExtrusionColorExt = 649;
        /// <summary>
        /// Seen in TestDefect1402.doc, 0xffffffff
        /// </summary>
        internal const int TDExtrusionColorExtCMY = 650;
        /// <summary>
        /// Seen in TestDefect1402.doc, 0x20000000 (CrMod?)
        /// </summary>
        internal const int TDExtrusionColorExtMod = 651;
        /// <summary>
        /// Undocumented. Seen in TestWatermark.doc. Complex property.
        /// </summary>
        internal const int Undocumented652 = 652;
        /// <summary>
        /// Seen in TestDefect1402.doc, 0xffffffff
        /// </summary>
        internal const int TDExtrusionColorExtK = 653;
        /// <summary>
        /// Undocumented. Seen in TestJira18071.doc
        /// </summary>
        internal const int Undocumented663 = 663;
        /// <summary>
        /// Undocumented. Seen in TestJira18071.doc
        /// </summary>
        internal const int Undocumented666 = 666;
        /// <summary>
        /// bool. Does this shape have a 3D effect.
        /// </summary>
        internal const int TDOn = 700;
        /// <summary>
        /// bool. Use metallic specularity.
        /// </summary>
        internal const int TDMetallic = 701;
        /// <summary>
        /// bool. Undocumented.
        /// </summary>
        internal const int TDUseExtrusionColor = 702;
        /// <summary>
        /// bool. Determines whether the front face of the extrusion will respond to changes in the lighting.
        /// </summary>
        internal const int TDLightFace = 703;

        #endregion


        // 0x02c0 - 0x03ff
        #region 3D Style Attributes

        /// <summary>
        /// Fixed. degrees (16.16) about y axis
        /// </summary>
        internal const int TDRotationAngleY = 704;
        /// <summary>
        /// Fixed. degrees (16.16) about x axis
        /// </summary>
        internal const int TDRotationAngleX = 705;
        /// <summary>
        /// int. These specify the rotation axis; only their relative magnitudes matter.
        /// </summary>
        internal const int TDRotationAxisX = 706;
        /// <summary>
        /// int. These specify the rotation axis; only their relative magnitudes matter.
        /// </summary>
        internal const int TDRotationAxisY = 707;
        /// <summary>
        /// int. These specify the rotation axis; only their relative magnitudes matter.
        /// </summary>
        internal const int TDRotationAxisZ = 708;
        /// <summary>
        /// Fixed. degrees (16.16) about axis
        /// </summary>
        internal const int TDRotationAngle = 709;
        /// <summary>
        /// Fixed. rotation center x (16.16 or g-units)
        /// </summary>
        internal const int TDRotationCenterX = 710;
        /// <summary>
        /// Fixed. rotation center y (16.16 or g-units)
        /// </summary>
        internal const int TDRotationCenterY = 711;
        /// <summary>
        /// int. rotation center z (absolute (emus))
        /// </summary>
        internal const int TDRotationCenterZ = 712;
        /// <summary>
        /// <see cref="ThreeDRenderMode"/>. Full, wireframe or bcube
        /// </summary>
        internal const int TDRenderMode = 713;
        /// <summary>
        /// Fixed. pixels (16.16). Defines the number of facets used to describe curved surfaces of an extrusion.
        /// </summary>
        internal const int TDTolerance = 714;
        /// <summary>
        /// Emus. Defines the viewpoint of the observer (X).
        /// </summary>
        internal const int TDViewpointX = 715;
        /// <summary>
        /// Emus. Defines the viewpoint of the observer (Y).
        /// </summary>
        internal const int TDViewpointY = 716;
        /// <summary>
        /// Emus. Z view distance.
        /// </summary>
        internal const int TDViewpointZ = 717;
        /// <summary>
        /// Fixed. Defines the origin X of the viewpoint within the bounding box of the shape.
        /// </summary>
        internal const int TDOriginX = 718;
        /// <summary>
        /// Fixed. Defines the origin Y of the viewpoint within the bounding box of the shape.
        /// </summary>
        internal const int TDOriginY = 719;
        /// <summary>
        /// Fixed. degree (16.16) skew angle
        /// </summary>
        internal const int TDSkewAngle = 720;
        /// <summary>
        /// int. Percentage skew amount
        /// </summary>
        internal const int TDSkewAmount = 721;
        /// <summary>
        /// Fixed. Fixed point intensity
        /// </summary>
        internal const int TDAmbientIntensity = 722;
        /// <summary>
        /// Int? Key light source direction; only their relative magnitudes matter.
        /// </summary>
        internal const int TDKeyX = 723;
        /// <summary>
        /// Int? Key light source direction; only their relative magnitudes matter.
        /// </summary>
        internal const int TDKeyY = 724;
        /// <summary>
        /// Int? Key light source direction; only their relative magnitudes matter.
        /// </summary>
        internal const int TDKeyZ = 725;
        /// <summary>
        /// Int? Fixed point intensity.
        /// </summary>
        internal const int TDKeyIntensity = 726;
        /// <summary>
        /// Int? Fill light source direction; only their relative magnitudes matter.
        /// </summary>
        internal const int TDFillX = 727;
        /// <summary>
        /// Int? Fill light source direction; only their relative magnitudes matter.
        /// </summary>
        internal const int TDFillY = 728;
        /// <summary>
        /// Int? Fill light source direction; only their relative magnitudes matter.
        /// </summary>
        internal const int TDFillZ = 729;
        /// <summary>
        /// Int? Fixed point intensity
        /// </summary>
        internal const int TDFillIntensity = 730;
        /// <summary>
        /// bool. Determines whether the rotation of the extruded object is specified by the RotationAngle attribute.
        /// </summary>
        internal const int TDConstrainRotation = 763;
        /// <summary>
        /// bool. Determines whether the center of rotation will be the geometric center of the extrusion.
        /// </summary>
        internal const int TDRotationCenterAuto = 764;
        /// <summary>
        /// bool. Parallel projection.
        /// </summary>
        internal const int TDParallel = 765;
        /// <summary>
        /// bool. Is key lighting harsh.
        /// </summary>
        internal const int TDKeyHarsh = 766;
        /// <summary>
        /// bool. Is fill lighting harsh.
        /// </summary>
        internal const int TDFillHarsh = 767;

        #endregion


        // 0x0300 - 0x033f
        #region Shape Attributes

        /// <summary>
        /// Test22389.doc
        /// </summary>
        internal const int Undocumented770 = 770;

        /// <summary>
        /// <see cref="ConnectorType"/>. Default none.
        /// </summary>
        internal const int ConnectorType = 771;
        /// <summary>
        /// <see cref="BWMode"/>. Default Automatic.
        /// </summary>
        internal const int BWMode = 772;
        /// <summary>
        /// <see cref="BWMode"/>. Default Automatic.
        /// </summary>
        internal const int BWPure = 773;
        /// <summary>
        /// <see cref="BWMode"/>. Default Automatic.
        /// </summary>
        internal const int BWNormal = 774;
        /// <summary>
        /// Test22389.doc
        /// </summary>
        internal const int Undocumented775 = 775;
        /// <summary>
        /// <see cref="Core.DiagramNodeLayout"/>.
        /// </summary>
        internal const int DiagramNodeLayout = 777;
        /// <summary>
        /// <see cref="Core.DiagramNodeKind"/>.
        /// This property specifies kind of node in a diagram.
        /// </summary>
        internal const int DiagramNodeKind = 778;
        /// <summary>
        /// MSODGMLO type. Most recently used layout for its child.
        /// Ver: XP.
        /// </summary>
        internal const int DiagramLayoutMRU = 779;
        /// <summary>
        /// Untested.
        /// String.
        /// This property is present if the shape represents an equation generated by Office 2007
        /// or later.  The property is a string of XML representing a Word 2003 XML document.
        /// The original equation is stored within the “oMathPara” tag within the document.
        /// Refer to the Office Open XML documentation for details on this XML representation
        /// of equations. If the document containing the shape is opened in Office 2007 or later,
        /// the shape is replaced with the equation in this document.
        /// Ver: 2007
        /// </summary>
        internal const int EquationXML = 780;

        /// <summary>
        /// byte[].
        /// Specifies a base-64 encoded package as defined in ISO/IEC 29500-2 that contains
        /// DrawingML content as defined in ISO/IEC 29500-1. [Rationale: This attribute allows an
        /// application to use VML to represent graphical content for a legacy document while still
        /// persisting DrawingML for consuming applications that support DrawingML.For example,
        /// a diagram stored within this attribute would have the four parts defined for a DrawingML
        /// diagram, as well as any number of application-defined parts and relationships.end
        /// rationale].
        /// </summary>
        internal const int GfxData = 781;

        /// <summary>
        /// Untested.
        /// Bool. Default false. Ver: 2007.
        /// True if the shape is a policy label representing metadata about a document.
        /// </summary>
        internal const int PolicyLabel = 822;
        /// <summary>
        /// Untested.
        /// Bool. Default false. Ver: 2007.
        /// True if the shape represents a barcode as part of a barcode policy for record management.
        /// </summary>
        internal const int PolicyBarCode = 823;
        /// <summary>
        /// Untested.
        /// Bool. Default false. Ver: XP.
        /// The value of this property should match the value of the fFlipH property
        /// (in the transform property set) if the pib property exists (in the blip property set.)
        /// </summary>
        internal const int FlipHQFE5152 = 824;
        /// <summary>
        /// Untested.
        /// Bool. Default false. Ver: XP.
        /// The value of this property should match the value of the fFlipV property
        /// (in the transform property set) if the pib property exists (in the blip property set.)
        /// </summary>
        internal const int FlipVQFE5152 = 825;
        /// <summary>
        /// bool. For OLE objects, whether the object is in icon form.
        /// What is stored in DOC escher does not seem very reliable, so I confirm it by extracting
        /// a bit from the OLE object data. For DOCX and WordML this is DrawAspect.
        /// </summary>
        internal const int OleIcon = 826;
        /// <summary>
        /// bool. For UI only. Prefer relative resizing.
        /// </summary>
        internal const int PreferRelativeResize = 827;
        /// <summary>
        /// bool. Lock the shape type (don't allow Change Shape)
        /// </summary>
        internal const int LockShapeType = 828;
        /// <summary>
        /// Bool. Set by the solver.
        /// </summary>
        internal const int Initiator = 829;
        /// <summary>
        /// bool. undocumented.
        /// </summary>
        internal const int DeleteAttachedObject = 830;

        #endregion


        // 0x0340 - 0x037f
        #region Callout Attributes

        /// <summary>
        /// <see cref="CalloutType"/>.
        /// </summary>
        internal const int CalloutType = 832;
        /// <summary>
        /// Int. Distance from box to first point.(EMUs)
        /// </summary>
        internal const int CalloutGap = 833;
        /// <summary>
        /// <see cref="CalloutAngle"/>.
        /// </summary>
        internal const int CalloutAngle = 834;
        /// <summary>
        /// <see cref="CalloutDropType"/>.
        /// </summary>
        internal const int CalloutDropType = 835;
        /// <summary>
        /// Int. if CalloutDropType.Specified, the actual drop distance. EMUs?
        /// </summary>
        internal const int CalloutDropDistance = 836;
        /// <summary>
        /// Int. if fCalloutLengthSpecified, the actual distance.
        /// </summary>
        internal const int CalloutLength = 837;
        /// <summary>
        /// Bool. Is the shape a callout.
        /// </summary>
        internal const int CalloutOn = 889;
        /// <summary>
        /// Bool. does callout have accent bar.
        /// </summary>
        internal const int CalloutAccentBar = 890;
        /// <summary>
        /// Bool. does callout have a text border
        /// </summary>
        internal const int CalloutTextBorder = 891;
        /// <summary>
        /// Bool. undocumented.
        /// </summary>
        internal const int CalloutMinusX = 892;
        /// <summary>
        /// Bool. undocumented.
        /// </summary>
        internal const int CalloutMinusY = 893;
        /// <summary>
        /// Bool. If true, then we occasionally invert the drop distance.
        /// </summary>
        internal const int CalloutDropAuto = 894;
        /// <summary>
        /// Bool. if true, we look at CalloutLength.
        /// </summary>
        internal const int CalloutLengthSpecified = 895;

        #endregion


        // 0x0380 - 0x03bf
        #region Group Shape Attributes

        /// <summary>
        /// Shape Name (present only if explicitly set).
        ///
        /// RK One scenario where this is used is to create relationships between diagram nodes.
        /// Instead of basing relationships on shape ids, the relationships are created based on shape names
        /// (in DOC, RTF and model based on hashes of shape names).
        /// Shape id might change (at least can change in Aspose.Words when document is saved), shape name,
        /// on the other hand remains the same and makes the life easier (no need to update diagram relations).
        /// Shape name is usually generated by MS Word based on the shape id.
        /// </summary>
        internal const int ShapeName = 896;
        /// <summary>
        /// Alternative text.
        /// </summary>
        internal const int ShapeDescription = 897;
        /// <summary>
        /// String. The Uri of the hyperlink.
        /// If the link is to a local bookmark, it will be prepended with "#" in the model.
        /// </summary>
        internal const int HyperlinkAddress = 898;
        /// <summary>
        /// Array of <see cref="PathPoint"/>. The polygon that text will be wrapped around (Word).
        /// In the DOC file this is actually just normal POINT array, but it is simpler for us to work with PathPoint.
        /// </summary>
        internal const int WrapPolygonVertices = 899;
        /// <summary>
        /// Left wrapping distance from text. Default 1/8 inch. EMUs.
        /// </summary>
        internal const int DistanceLeft = 900;
        /// <summary>
        /// Top wrapping distance from text. Default 0. EMUs.
        /// </summary>
        internal const int DistanceTop = 901;
        /// <summary>
        /// Right wrapping distance from text. Default 1/8 inch. EMUs.
        /// </summary>
        internal const int DistanceRight = 902;
        /// <summary>
        /// Bottom wrapping distance from text. Default 0. EMUs in Escher.
        /// </summary>
        internal const int DistanceBottom = 903;
        /// <summary>
        /// NOT IN THE MODEL. We ignore this.
        /// </summary>
        internal const int RegroupId = 904;
        /// <summary>
        /// String. The tooltip (also used for hyperlinks).
        /// </summary>
        internal const int ScreenTip = 909;
        /// <summary>
        /// String. Looks like can be java script.
        /// </summary>
        internal const int ScriptText = 910;
        /// <summary>
        /// <see cref="Aspose.Words.Drawing.HorizontalAlignment"/>
        ///
        /// When this attribute is specified, the shape is positioned using alignment relative to some object.
        /// The relative anchor is specified using the  relative horizontal position value, all values are valid.
        ///
        /// When the alignment value is Inside our Outside (in MS Word this is in the Book Layout dropdown),
        /// then MS Word allows the relative anchor to be only Margin or Page.
        ///
        /// Occurs in EsShapeOptionsEx record.
        /// Note the values in the SPEC are wrong (off by 1).
        /// </summary>
        internal const int HorizontalAlignment = 911;
        /// <summary>
        /// <see cref="Aspose.Words.Drawing.RelativeHorizontalPosition"/>
        /// Occurs in EsShapeOptionsEx record.
        /// Note the values in the SPEC are wrong (off by 1).
        /// </summary>
        internal const int RelativeHorizontalPosition = 912;
        /// <summary>
        /// <see cref="Aspose.Words.Drawing.VerticalAlignment"/>
        /// Also see comments above in <see cref="HorizontalAlignment"/>.
        ///
        /// Occurs in EsShapeOptionsEx record.
        /// Note the values in the SPEC are wrong (off by 1).
        /// </summary>
        internal const int VerticalAlignment = 913;
        /// <summary>
        /// <see cref="Aspose.Words.Drawing.RelativeVerticalPosition"/>
        /// Occurs in EsShapeOptionsEx record.
        /// Note the values in the SPEC are wrong (off by 1).
        /// </summary>
        internal const int RelativeVerticalPosition = 914;
        /// <summary>
        /// int. Defines the length of a horizontal rule as a percentage of page width.
        /// Note that value 500 means 50%.
        /// </summary>
        internal const int HRPct = 915;
        /// <summary>
        /// <see cref="HorizontalRuleAlignment"/>. Defines the alignment of a horizontal rule.
        /// </summary>
        internal const int HRAlign = 916;
        /// <summary>
        /// NOT IN THE MODEL. Seems this attribute provides more exactly shape height.
        /// We read this during DOC/RTF import and remove from model after shape height is adjusted.
        /// Int. Defines the thickness of a horizontal rule in twips.
        /// </summary>
        internal const int HRHeight = 917;
        /// <summary>
        /// NOT IN THE MODEL. We ignore it on read. On write we use shape width instead.
        /// Int. Defines the length of a horizontal rule in twips.
        /// </summary>
        internal const int HRWidth = 918;
        /// <summary>
        /// String. Looks like "mime" type of the script. I've seen a value " type="text/javascript"".
        /// </summary>
        internal const int ScriptType = 919;
        /// <summary>
        /// Int. Probably Id of the script language. 4 = JavaScript.
        /// </summary>
        internal const int ScriptLanguage = 920;
        /// <summary>
        /// Untested.
        /// Id script attribute of the script?
        /// </summary>
        internal const int ScriptIdAttr = 921;
        /// <summary>
        /// String. Looks like string name of the script language.
        /// </summary>
        internal const int ScriptLanguageName = 922;
        /// <summary>
        /// These are 32bit color for the inline picture borders.
        /// Occur in EsShapeOptionsEx record.
        /// We read this attribute into the model initially, so it could occur in the model.
        ///
        /// If the shape is an inline picture, then when the PICF structure is loaded,
        /// the 32bit colors specified by these attributes are applied to the borders.
        /// The border attributes BorderXXX are added to the model and these attributes are removed.
        /// </summary>
        internal const int BorderTopColor = 923;
        internal const int BorderLeftColor = 924;
        internal const int BorderBottomColor = 925;
        internal const int BorderRightColor = 926;
        /// <summary>
        /// Untested.
        /// Seen complex value in defect 3397.
        /// The shape’s 2007 representation in Office Open XML format.  The actual data is a package
        /// in Office XML format, which can simply be opened as a zip file.  This zip file contains
        /// an XML file with the root element “sp”.  Refer to the publically available Office
        /// Open XML documentation for more information about this data.  In case we lose any
        /// property when converting a 2007 Office Art shape to 2003 shape, we use this blob to
        /// retrieve the original Office Art property data when opening the file in 2007.
        /// See Appendix F for more information.
        /// Ver: 2007
        /// </summary>
        internal const int MetroBlob = 937;
        /// <summary>
        /// Untested.
        /// The shape’s unique z-order, ranging from 1 to 503316479.  The higher the number,
        /// the closer to “in-front” the shape is.
        /// Ver: 2007.
        /// </summary>
        internal const int Dhgt = 938;
        /// <summary>
        /// 0x8000 flag. bool. Allows shape to anchor and position inside table cells.
        /// Occurs in extended attributes.
        /// Not sure about default value. From DOC files looks like default is true, but RTF spec says its false.
        /// </summary>
        internal const int AllowInCell = 944;
        /// <summary>
        /// 0x4000 flag. Set to true to indicate this shape is a picture bullet.
        /// Occurs in extended properties.
        /// </summary>
        internal const int PictureBullet = 945;
        /// <summary>
        /// 0x2000 flag. Set to true for a "standard" horizontal rule shape.
        /// This means the horizontal rule does not have a picture.
        /// </summary>
        internal const int HRStandard = 946;
        /// <summary>
        /// 0x1000 flag. Set to true to turn a shadow on the horizontal rule shape off.
        /// </summary>
        internal const int HRNoShade = 947;
        /// <summary>
        /// 0x0800 flag. Set to true for a shape that is a horizontal rule.
        /// </summary>
        internal const int HROn = 948;
        /// <summary>
        /// 0x0400 flag.
        /// Seems to be PowerPoint related.
        /// </summary>
        internal const int UserDrawn = 949;
        /// <summary>
        /// 0x0200 flag. bool. Default is true. Occurs in extended attributes.
        /// Allows shape to overlap other shapes unless it is a shape with None wrapping (\shpwr3),
        /// in which case it can always overlap an object with other types of wrapping and vice-versa.
        /// </summary>
        internal const int AllowOverlap = 950;
        /// <summary>
        /// 0x0100 flag. Occurs in extended attributes.
        /// </summary>
        internal const int ReallyHidden = 951;
        /// <summary>
        /// 0x0080 flag.
        /// </summary>
        internal const int ScriptAnchor = 952;
        /// <summary>
        /// Has the wrap polygon been edited? Default false.
        /// </summary>
        internal const int EditedWrap = 953;
        /// <summary>
        /// Word-only (shape is behind text), default false.
        /// </summary>
        internal const int BehindText = 954;
        /// <summary>
        /// Notify client on a double click, default false.
        /// </summary>
        internal const int OnDblClickNotify = 955;
        /// <summary>
        /// A button shape (i.e., clicking performs an action). Set for shapes with attached hyperlinks or macros.
        /// Default false.
        /// </summary>
        internal const int Button = 956;
        /// <summary>
        /// 1D adjustment, default false.
        /// </summary>
        internal const int OneD = 957;
        /// <summary>
        /// Do not display. default false.
        /// </summary>
        internal const int Hidden = 958;
        /// <summary>
        /// Print this shape, default true.
        /// </summary>
        internal const int Print = 959;

        #endregion


        // 0x0400 - 0x43f - ID properties from generic HTML. Whatever this means.
        #region Unknown HTML Attributes

        /// <summary>
        /// Seen in TobyHenderson6.doc and in TestInlinePictures.doc. Looks like another set of flags.
        /// </summary>
        internal const int UnknownHtmlFlags = 1087;

        #endregion


        // 0x0500 - 0x053f
        #region Diagram Attributes

        /// <summary>
        /// Enum <see cref="Core.EditAs"/>.
        /// This property could be set for a group shape to identify it as a canvas or a diagram.
        /// </summary>
        internal const int EditAs = 1280;
        /// <summary>
        /// Int. In the RTF specification there is a detailed info which value means what diagram style,
        /// but the problem is that different diagram types have different sets of diagram styles,
        /// so I really prefer to store this just as integer in the model. WordML does the same.
        /// </summary>
        internal const int DiagramStyle = 1281;
        internal const int Undocumented1282 = 1282;
        internal const int Undocumented1283 = 1283;
        /// <summary>
        /// Array of <see cref="DiagramNodeRelation"/>.
        /// It is important to write this property, otherwise MS Word does not recognize the shape as a diagram.
        /// </summary>
        internal const int DiagramRelationsTable = 1284;
        /// <summary>
        /// Fixed. The scale factor for width of a diagram.
        /// </summary>
        internal const int DiagramScaleX = 1285;
        /// <summary>
        /// Fixed. The scale factor for height of a diagram.
        /// </summary>
        internal const int DiagramScaleY = 1286;
        /// <summary>
        /// Int. Specifies the font size in points of text for new nodes in a diagram.
        /// </summary>
        internal const int DiagramFontSize = 1287;
        /// <summary>
        /// int[] consisting of four items. Specifies the bounds that the diagram nodes are constrained to.
        /// </summary>
        internal const int DiagramConstrainBounds = 1288;
        /// <summary>
        /// This property specifies the amount to scale text. This property MAY be ignored.
        /// A FixedPoint, as specified in [MS-OSHARED] section 2.2.1.6, that specifies the amount to scale text.
        /// </summary>
        internal const int DiagramBaseTextScale = 1289;
        /// <summary>
        /// Bool. True when diagram uses auto format.
        /// </summary>
        internal const int DiagramAutoFormat = 1340;
        /// <summary>
        /// Bool. Not fully sure, but looks very much reverse flag.
        /// </summary>
        internal const int DiagramReverse = 1341;
        /// <summary>
        /// Bool.
        /// </summary>
        internal const int DiagramAutoLayout = 1342;
        /// <summary>
        /// Bool. I guess this corresponds to fPseudoInline in RTF. This is set for inline shapes.
        /// Affects not only diagrams, but all shapes.
        ///
        /// The shape is pseudo-inline, meaning it behaves like an inline image as far as positioning goes,
        /// but has the features of shapes.
        /// </summary>
        internal const int PseudoInline = 1343;

        #endregion

        #region Line Side Attributes

        // 0x0540 - Line Left Style
        // 0x0580 - Line Top Style
        // 0x05c0 - Line Right Style
        // 0x0600 - Line Bottom Style
        // 0x0640 - Line Column Style
        // Attributes from 0x0540 to 0x067f are same as line attributes, but for
        // the left, top, right, bottom and column sides of the stroke.

        #endregion

        // 0x680 - 0x6bf Properties related to Web Component Functionality.

        // 0x6c0 - 0x6ff This property set defines an additional clipping path for a shape or group.
        #region Clip Attributes
        /// <summary>
        /// Untested.
        /// Seen in TestArtSchool8039.doc. Complex, contains 0x2e bytes of data.
        /// </summary>
        internal const int ClipVertices = 1728;
        /// <summary>
        /// Untested.
        /// </summary>
        internal const int ClipSegmentInfo = 1729;
        /// <summary>
        /// Untested.
        /// </summary>
        internal const int ClipShapePath = 1730;
        /// <summary>
        /// Untested. bool.
        /// </summary>
        internal const int ClipToWrap = 1790;
        /// <summary>
        /// Untested. bool. Seen in BusinessBrochure.doc.
        /// </summary>
        internal const int ClipOK = 1791;

        #endregion


        // 0x700 - 0x73f
        #region TablePC Ink Attributes
        /// <summary>
        /// InkData as byte array. Complex.
        /// </summary>
        internal const int InkData = 1792;
        /// <summary>
        /// Specifies that Ink should be treated as annotation.
        /// </summary>
        internal const int InkAnnotation = 1855;
        #endregion

        // 0x740 - 0x77f unused

        // 0x780 - 0x7bf Signature. These properties specify that the shape is a signature
        // line a document generated by Office 2007 or later.  A signature line provides
        // a visual representation of a signature in a document that is digitally signed.
        #region Signature Line Attributes

        /// <summary>
        /// This property specifies the GUID of the signature line.
        /// </summary>
        internal const int SigSetupId = 0x781;
        /// <summary>
        /// This property specifies the GUID of the provider that creates the signature line.
        /// </summary>
        internal const int SigSetupProvId = 0x782;
        /// <summary>
        /// This property specifies the name of the suggested signer.
        /// </summary>
        internal const int SigSetupSuggSigner = 0x783;
        /// <summary>
        /// This property specifies the title or additional information of the suggested signer.
        /// </summary>
        internal const int SigSetupSuggSigner2 = 0x784;
        /// <summary>
        /// This property specifies the e-mail address of the suggested signer.
        /// </summary>
        internal const int SigSetupSuggSignerEmail = 0x785;
        /// <summary>
        /// This property specifies the signing instruction displayed to the signer.
        /// </summary>
        internal const int SigSetupSignInst = 0x786;
        /// <summary>
        /// 2.3.27.13 wzSigSetupAddlXml
        /// This property specifies the additional information in XML format provided by the provider that creates the signature line.
        /// </summary>
        internal const int SigSetupAddlXml = 0x787;
        /// <summary>
        /// 2.3.27.15 wzSigSetupProvUrl
        /// This property specifies the URL of the provider that creates the signature line.
        /// </summary>
        internal const int SigSetupProvUrl = 0x788;

        /// <summary>
        /// bool. Specifies whether the signing date is displayed.
        /// </summary>
        internal const int SigSetupShowSignDate = 0x07bc;
        /// <summary>
        /// bool. Specifies whether the comment from the signer is allowed.
        /// </summary>
        internal const int SigSetupAllowComments = 0x7bd;
        /// <summary>
        /// bool. Specifies whether the signing instruction is overridden.
        /// </summary>
        internal const int SigSetupSignInstSet = 0x7be;
        /// <summary>
        /// bool. Specifies whether the shape is a signature line.
        /// </summary>
        internal const int IsSignatureLine = 0x7bf;

        #endregion

        // 0x07c0 - 0x07ff Relative position and size properties for shapes and textboxes in Word.
        #region Group Shape 2 Attributes
        /// <summary>
        /// int. Relative left position of the shape in 1/10th of percent.
        /// When present, logically overrides the <see cref="Left"/> attribute.
        /// Available since Word 2007.
        /// </summary>
        internal const int LeftPercent = 1986;
        /// <summary>
        /// int. Relative top position of the shape in 1/10th of percent.
        /// When present, logically overrides the <see cref="Top"/> attribute.
        /// Available since Word 2007.
        /// </summary>
        internal const int TopPercent = 1987;
        /// <summary>
        /// int. Relative width of the shape in 1/10th of percent.
        /// When present, logically overrides the <see cref="Width"/> attribute.
        /// Available since Word 2007.
        /// </summary>
        internal const int WidthPercent = 1984;
        /// <summary>
        /// int. Relative height of the shape in 1/10th of percent.
        /// When present, logically overrides the <see cref="Height"/> attribute.
        /// Available since Word 2007.
        /// </summary>
        internal const int HeightPercent = 1985;
        /// <summary>
        /// <see cref="RelativeHorizontalSize"/> 
        /// Specifies what shape width is relative to.
        /// Available since Word 2007.
        /// </summary>
        internal const int RelativeWidth = 1988;
        /// <summary>
        /// <see cref="RelativeVerticalSize"/> 
        /// Specifies what shape width is relative to.
        /// Available since Word 2007.
        /// </summary>
        internal const int RelativeHeight = 1989;
        /// <summary>
        /// Untested.
        /// Starting column.
        /// </summary>
        internal const int ColStart = 1990;
        /// <summary>
        /// Untested.
        /// Number of columns to span.
        /// </summary>
        internal const int ColSpan = 1991;

        #endregion

        // 0x1000 - 0x103f
        #region Aspose Attributes

        /// <summary>
        /// <see cref="FlipOrientation"/>. Not stored as a property in Escher, but stored in the EsShape record.
        /// </summary>
        internal const int Flip = 4096;
        /// <summary>
        /// <see cref="WrapType"/>. Not stored as a property in Escher, but stored in the FSPA structure.
        /// </summary>
        internal const int WrapType = 4097;
        /// <summary>
        /// <see cref="WrapSide"/>. Not stored as a property in Escher, but stored in the FSPA structure.
        /// </summary>
        internal const int WrapSide = 4098;
        /// <summary>
        /// bool in the model. Not stored as a property in Escher, but stored in the FSPA structure.
        /// </summary>
        internal const int AnchorLocked = 4099;
        /// <summary>
        /// byte[] in the model. Not stored as a property in Escher.
        /// </summary>
        internal const int ImageBytes = 4102;
        /// <summary>
        /// string in the model. Not stored as a property in Escher.
        /// If present, specifies a "comment" for a non-linked image.
        /// Usually, it is the original name of the image, sometimes without the extension.
        /// </summary>
        internal const int ImageTitle = 4103;
        /// <summary>
        /// string in the model. Not stored as a property in Escher.
        /// If present, this attribute specifies the URL to the linked image.
        /// </summary>
        internal const int ImageSourceFullName = 4104;
        /// <summary>
        /// Border in the model. Not stored as a property in Escher. Stored in PICF for inline pictures only.
        /// </summary>
        internal const int BorderTop = 4106;
        /// <summary>
        /// Border in the model. Not stored as a property in Escher. Stored in PICF for inline pictures only.
        /// </summary>
        internal const int BorderLeft = 4107;
        /// <summary>
        /// Border in the model. Not stored as a property in Escher. Stored in PICF for inline pictures only.
        /// </summary>
        internal const int BorderBottom = 4108;
        /// <summary>
        /// Border in the model. Not stored as a property in Escher. Stored in PICF for inline pictures only.
        /// </summary>
        internal const int BorderRight = 4109;
        /// <summary>
        /// byte[] in the model. Not stored as a property in Escher.
        /// </summary>
        /// <remarks>
        /// WORDSNET-16360 The key values for FillImageBytes and LineImageBytes is important for DOC writer.
        /// Interchanged, so FillImageBytes goes before LineImageBytes.
        /// </remarks>
        internal const int FillImageBytes = 4110;
        /// <summary>
        /// byte[] in the model. Not stored as a property in Escher.
        /// </summary>
        internal const int LineImageBytes = 4111;
        /// <summary>
        /// The instance of <see cref="IEmbeddedObject"/> object.
        /// </summary>
        internal const int OleObject = 4112;
        /// <summary>
        /// String that specifies ProgID of the OLE object.
        /// </summary>
        internal const int OleProgID = 4113;
        /// <summary>
        /// String that specifies full name of the linked file.
        /// </summary>
        internal const int OleSourceFullName = 4114;
        /// <summary>
        /// String that specifies the range within the linked file. Optional.
        /// </summary>
        internal const int OleSourceItem = 4115;
        /// <summary>
        /// Bool that specifies whether the link is auto updated or not.
        /// </summary>
        internal const int OleAutoUpdate = 4116;
        /// <summary>
        /// Bool that specifies whether the OLE link is locked for updates or not.
        /// </summary>
        internal const int OleLocked = 4117;
        /// <summary>
        /// OleLinkType value. Specifies type of the OLE link.
        /// </summary>
        internal const int OleLinkType = 4118;
        /// <summary>
        /// Int. Corresponds to the value for the \f field switch in OLE fields.
        /// </summary>
        internal const int OleFormatUpdateType = 4119;
        /// <summary>
        /// string that specifies the hyperlink target frame.
        /// </summary>
        internal const int HyperlinkTarget = 4120;
        /// <summary>
        /// <see cref="PresetTexture"/>, specifies the id of the built-in texture for the stroke fill pattern.
        /// </summary>
        internal const int LineFillPresetTexture = 4121;
        /// <summary>
        /// <see cref="PresetTexture"/>, specifies the id of the built-in texture for the fill pattern.
        /// </summary>
        internal const int FillPresetTexture = 4122;

        /// <summary>
        /// int.
        /// Shape id as loaded from the file or generated when a shape is created.
        /// All shape identifiers are regenerated into MS Word-like sequence before a document is saved.
        /// </summary>
        /// <remarks>
        /// Note, DrawingML id is of unsigned int type.
        /// </remarks>
        internal const int ShapeId = 4124;

        /// <summary>
        /// CoordOrigin.X in coordinate units.
        /// Normally occurs for group shapes and also shapes with custom geometry.
        /// </summary>
        internal const int CoordOriginX = 4125;
        /// <summary>
        /// CoordOrigin.Y in coordinate units.
        /// Normally occurs for group shapes and also shapes with custom geometry.
        /// </summary>
        internal const int CoordOriginY = 4126;
        /// <summary>
        /// CoordSize.Width in coordinate units.
        /// Normally occurs for group shapes and also shapes with custom geometry.
        /// </summary>
        internal const int CoordSizeWidth = 4127;
        /// <summary>
        /// CoordSize.Height in coordinate units.
        /// Normally occurs for group shapes and also shapes with custom geometry.
        /// </summary>
        internal const int CoordSizeHeight = 4128;

        /// <summary>
        /// double.
        /// Left position of the shape. Has effect for floating shapes only.
        /// For top level shapes these are points, for child shapes these are in the parent coordinate system.
        /// </summary>
        internal const int Left = 4129;
        /// <summary>
        /// double.
        /// Top position of the shape. Has effect for floating shapes only.
        /// For top level shapes these are points, for child shapes these are in the parent coordinate system.
        /// </summary>
        internal const int Top = 4130;
        /// <summary>
        /// double. Width of the shape.
        /// For top level shapes these are points, for child shapes these are in the parent coordinate system.
        /// </summary>
        internal const int Width = 4131;
        /// <summary>
        /// double. Height of the shape.
        /// For top level shapes these are points, for child shapes these are in the parent coordinate system.
        /// </summary>
        internal const int Height = 4132;

        /// <summary>
        /// Boolean. DrawingML property.
        /// </summary>
        internal const int DmlLockArrowHeads = 4140;
        /// <summary>
        /// Boolean. DrawingML property.
        /// </summary>
        internal const int DmlLockResize = 4141;
        /// <summary>
        /// Boolean. DrawingML property.
        /// </summary>
        internal const int DmlLockDrillDown = 4142;

        /// <summary>
        /// int. EMUs. DrawingML property.
        /// </summary>
        internal const int DmlEffectExtentLeft = 4143;
        /// <summary>
        /// int. EMUs. DrawingML property.
        /// </summary>
        internal const int DmlEffectExtentTop = 4144;
        /// <summary>
        /// int. EMUs. DrawingML property.
        /// </summary>
        internal const int DmlEffectExtentRight = 4145;
        /// <summary>
        /// int. EMUs. DrawingML property.
        /// </summary>
        internal const int DmlEffectExtentBottom = 4146;

        /// <summary>
        /// At the moment this is an unparsed string value.
        /// </summary>
        internal const int ThemeColor = 4147;
        /// <summary>
        /// At the moment this is an unparsed string value.
        /// </summary>
        internal const int ThemeShade = 4148;
        /// <summary>
        /// At the moment this is an unparsed string value.
        /// </summary>
        internal const int ThemeTint = 4149;

        /// <summary>
        /// int.
        /// Determines the display order of overlapping shapes.
        /// </summary>
        internal const int ZOrder = 4154;

        /// <summary>
        /// <see cref="Aspose.Words.Drawing.ShapeType"/>.
        /// </summary>
        internal const int ShapeType = 4155;

        /// <summary>
        /// <see cref="ConnectorRule" /> object.
        /// </summary>
        internal const int ConnectorRule = 4156;

        /// <summary>
        /// String. Shape title (caption).
        /// </summary>
        internal const int ShapeTitle = 4157;

        /// <summary>
        /// Shape width used by Word when document is layouted. See TestJira13258().
        /// </summary>
        internal const int Sys_LayoutWidth = 4160;

        /// <summary>
        /// Shape height used by Word when document is layouted. See TestJira13258().
        /// </summary>
        internal const int Sys_LayoutHeight = 4161;

        #endregion
    }
}
