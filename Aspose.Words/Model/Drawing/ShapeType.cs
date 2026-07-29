// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the type of shape in a Microsoft Word document.
    /// </summary>
    /// <seealso cref="ShapeBase.ShapeType"/>
    [CppEnumEnableMetadata]
    public enum ShapeType
    {
        /// <summary>
        /// The shape is an image. 
        /// </summary>
        Image = 75,
        /// <summary>
        /// The shape is a textbox. Note that shapes of many other types can also have text inside them too.
        /// A shape does not have to have this type to contain text.
        /// </summary>
        TextBox = 202,
        /// <summary>
        /// <p>The shape is a group shape.</p>
        /// </summary>
        /// <dev>In DOC file, shape group has shape type NonPrimitive, we don't want this in the model,
        /// so in the model it is our own constant.</dev>
        Group = -1,
        /// <summary>
        /// The shape is an OLE object.
        /// <p>You cannot create shapes of this type in the document.</p>
        /// </summary>
        /// <dev>
        /// In Microsoft Word, shapes that represent OLE objects have shape type picture,
        /// but in our model, they are distinguished into their own shape type.
        /// </dev>
        OleObject = -2,
        /// <summary>
        /// The shape is an ActiveX control.
        /// <p>You cannot create shapes of this type in the document.</p>
        /// </summary>
        /// <dev>
        /// In DOC and RTF, shapes that represent ActiveX controls have shape type picture.
        /// In WordML, ActiveX controls have their own shape type and so is in the model.
        /// </dev>
        OleControl = 201,
        /// <summary>
        /// A shape drawn by user and consisting of multiple segments and/or vertices (curve, freeform or scribble).
        /// <p>You cannot create shapes of this type in the document.</p>
        /// </summary>
        NonPrimitive = 0,
        /// <summary>
        /// Rectangle.
        /// </summary>
        Rectangle = 1,
        /// <summary>
        /// Round rectangle.
        /// </summary>
        RoundRectangle = 2,
        /// <summary>
        /// Ellipse.
        /// </summary>
        Ellipse = 3,
        /// <summary>
        /// Diamond.
        /// </summary>
        Diamond = 4,
        /// <summary>
        /// Triangle.
        /// </summary>
        Triangle = 5,
        /// <summary>
        /// Right triangle.
        /// </summary>
        RightTriangle = 6,
        /// <summary>
        /// Parallelogram.
        /// </summary>
        Parallelogram = 7,
        /// <summary>
        /// Trapezoid.
        /// </summary>
        Trapezoid = 8,
        /// <summary>
        /// Hexagon.
        /// </summary>
        Hexagon = 9,
        /// <summary>
        /// Octagon.
        /// </summary>
        Octagon = 10,
        /// <summary>
        /// Plus.
        /// </summary>
        Plus = 11,
        /// <summary>
        /// Star.
        /// </summary>
        Star = 12,
        /// <summary>
        /// Arrow.
        /// </summary>
        Arrow = 13,
        /// <summary>
        /// Thick arrow.
        /// </summary>
        ThickArrow = 14,
        /// <summary>
        /// Home plate.
        /// </summary>
        HomePlate = 15,
        /// <summary>
        /// Cube.
        /// </summary>
        Cube = 16,
        /// <summary>
        /// Balloon.
        /// </summary>
        Balloon = 17,
        /// <summary>
        /// Seal.
        /// </summary>
        Seal = 18,
        /// <summary>
        /// Arc.
        /// </summary>
        Arc = 19,
        /// <summary>
        /// Line.
        /// </summary>
        Line = 20,
        /// <summary>
        /// Plaque.
        /// </summary>
        Plaque = 21,
        /// <summary>
        /// Can.
        /// </summary>
        Can = 22,
        /// <summary>
        /// Donut.
        /// </summary>
        Donut = 23,
        /// <summary>
        /// Text simple.
        /// </summary>
        TextSimple = 24,
        /// <summary>
        /// Text octagon.
        /// </summary>
        TextOctagon = 25,
        /// <summary>
        /// Text hexagon.
        /// </summary>
        TextHexagon = 26,
        /// <summary>
        /// Text curve.
        /// </summary>
        TextCurve = 27,
        /// <summary>
        /// Text wave.
        /// </summary>
        TextWave = 28,
        /// <summary>
        /// Text ring.
        /// </summary>
        TextRing = 29,
        /// <summary>
        /// Text on curve.
        /// </summary>
        TextOnCurve = 30,
        /// <summary>
        /// Text on ring.
        /// </summary>
        TextOnRing = 31,
        /// <summary>
        /// A straight connector shape.
        /// </summary>
        StraightConnector1 = 32,
        /// <summary>
        /// A bent connector shape with two segments.
        /// </summary>
        BentConnector2 = 33,
        /// <summary>
        /// A bent connector shape with three segments.
        /// </summary>
        BentConnector3 = 34,
        /// <summary>
        /// A bent connector shape with four segments.
        /// </summary>
        BentConnector4 = 35,
        /// <summary>
        /// A bent connector shape with five segments.
        /// </summary>
        BentConnector5 = 36,
        /// <summary>
        /// A curved connector shape with two segments.
        /// </summary>
        CurvedConnector2 = 37,
        /// <summary>
        /// A curved connector shape with three segments.
        /// </summary>
        CurvedConnector3 = 38,
        /// <summary>
        /// A curved connector shape with four segments.
        /// </summary>
        CurvedConnector4 = 39,
        /// <summary>
        /// A curved connector shape with five segments.
        /// </summary>
        CurvedConnector5 = 40,
        /// <summary>
        /// A callout shape with one arrow.
        /// </summary>
        Callout1 = 41,
        /// <summary>
        /// A callout shape with two arrows.
        /// </summary>
        Callout2 = 42,
        /// <summary>
        /// A callout shape with three arrows.
        /// </summary>
        Callout3 = 43,
        /// <summary>
        /// An accent callout shape with one arrow.
        /// </summary>
        AccentCallout1 = 44,
        /// <summary>
        /// An accent callout shape with two arrows.
        /// </summary>
        AccentCallout2 = 45,
        /// <summary>
        /// An accent callout shape with three arrows.
        /// </summary>
        AccentCallout3 = 46,
        /// <summary>
        /// Border callout 1.
        /// </summary>
        BorderCallout1 = 47,
        /// <summary>
        /// Border callout 2.
        /// </summary>
        BorderCallout2 = 48,
        /// <summary>
        /// Border callout 3.
        /// </summary>
        BorderCallout3 = 49,
        /// <summary>
        /// Accent border callout 1.
        /// </summary>
        AccentBorderCallout1 = 50,
        /// <summary>
        /// Accent border callout 2.
        /// </summary>
        AccentBorderCallout2 = 51,
        /// <summary>
        /// Accent border callout 3.
        /// </summary>
        AccentBorderCallout3 = 52,
        /// <summary>
        /// Ribbon.
        /// </summary>
        Ribbon = 53,
        /// <summary>
        /// Ribbon 2.
        /// </summary>
        Ribbon2 = 54,
        /// <summary>
        /// Chevron.
        /// </summary>
        Chevron = 55,
        /// <summary>
        /// Pentagon.
        /// </summary>
        Pentagon = 56,
        /// <summary>
        /// NoSmoking.
        /// </summary>
        NoSmoking = 57,
        /// <summary>
        /// Eight-pointed star.
        /// </summary>
        Seal8 = 58,
        /// <summary>
        /// 16-pointed star.
        /// </summary>
        Seal16 = 59,
        /// <summary>
        /// 32-pointed star.
        /// </summary>
        Seal32 = 60,
        /// <summary>
        /// Wedge rect callout.
        /// </summary>
        WedgeRectCallout = 61,
        /// <summary>
        /// Wedge R rect callout.
        /// </summary>
        WedgeRRectCallout = 62,
        /// <summary>
        /// Wedge ellipse callout.
        /// </summary>
        WedgeEllipseCallout = 63,
        /// <summary>
        /// Wave.
        /// </summary>
        Wave = 64,
        /// <summary>
        /// Folded corner.
        /// </summary>
        FoldedCorner = 65,
        /// <summary>
        /// Left arrow.
        /// </summary>
        LeftArrow = 66,
        /// <summary>
        /// Down arrow.
        /// </summary>
        DownArrow = 67,
        /// <summary>
        /// Up arrow.
        /// </summary>
        UpArrow = 68,
        /// <summary>
        /// Left right arrow.
        /// </summary>
        LeftRightArrow = 69,
        /// <summary>
        /// Up down arrow.
        /// </summary>
        UpDownArrow = 70,
        /// <summary>
        /// Irregular seal 1.
        /// </summary>
        IrregularSeal1 = 71,
        /// <summary>
        /// Irregular seal 2.
        /// </summary>
        IrregularSeal2 = 72,
        /// <summary>
        /// Lightning bolt.
        /// </summary>
        LightningBolt = 73,
        /// <summary>
        /// Heart.
        /// </summary>
        Heart = 74,
        /// <summary>
        /// Quad arrow.
        /// </summary>
        QuadArrow = 76,
        /// <summary>
        /// Left arrow callout.
        /// </summary>
        LeftArrowCallout = 77,
        /// <summary>
        /// Right arrow callout
        /// </summary>
        RightArrowCallout = 78,
        /// <summary>
        /// Up arrow callout.
        /// </summary>
        UpArrowCallout = 79,
        /// <summary>
        /// Down arrow callout.
        /// </summary>
        DownArrowCallout = 80,
        /// <summary>
        /// Left right arrow callout.
        /// </summary>
        LeftRightArrowCallout = 81,
        /// <summary>
        /// Up down arrow callout.
        /// </summary>
        UpDownArrowCallout = 82,
        /// <summary>
        /// Quad arrow callout.
        /// </summary>
        QuadArrowCallout = 83,
        /// <summary>
        /// Bevel.
        /// </summary>
        Bevel = 84,
        /// <summary>
        /// Left bracket.
        /// </summary>
        LeftBracket = 85,
        /// <summary>
        /// Right bracket.
        /// </summary>
        RightBracket = 86,
        /// <summary>
        /// Left brace.
        /// </summary>
        LeftBrace = 87,
        /// <summary>
        /// Right brace.
        /// </summary>
        RightBrace = 88,
        /// <summary>
        /// Left up arrow.
        /// </summary>
        LeftUpArrow = 89,
        /// <summary>
        /// Bent up arrow.
        /// </summary>
        BentUpArrow = 90,
        /// <summary>
        /// Bent arrow.
        /// </summary>
        BentArrow = 91,
        /// <summary>
        /// 24-pointed star.
        /// </summary>
        Seal24 = 92,
        /// <summary>
        /// Striped right arrow.
        /// </summary>
        StripedRightArrow = 93,
        /// <summary>
        /// Notched right arrow.
        /// </summary>
        NotchedRightArrow = 94,
        /// <summary>
        /// Block arc.
        /// </summary>
        BlockArc = 95,
        /// <summary>
        /// Smiley face.
        /// </summary>
        SmileyFace = 96,
        /// <summary>
        /// Vertical scroll.
        /// </summary>
        VerticalScroll = 97,
        /// <summary>
        /// Horizontal scroll.
        /// </summary>
        HorizontalScroll = 98,
        /// <summary>
        /// Circular arrow.
        /// </summary>
        CircularArrow = 99,
        /// <summary>
        /// This shape type seems to be set for shapes that are not part of the standard set of the
        /// auto shapes in Microsoft Word. For example, if you insert a new auto shape from ClipArt.
        /// <p>You cannot create shapes of this type in the document.</p>
        /// </summary>
        CustomShape = 100,
        /// <summary>
        /// Uturn arrow.
        /// </summary>
        UturnArrow = 101,
        /// <summary>
        /// Curved right arrow.
        /// </summary>
        CurvedRightArrow = 102,
        /// <summary>
        /// Curved left arrow.
        /// </summary>
        CurvedLeftArrow = 103,
        /// <summary>
        /// Curved up arrow
        /// </summary>
        CurvedUpArrow = 104,
        /// <summary>
        /// Curved down arrow.
        /// </summary>
        CurvedDownArrow = 105,
        /// <summary>
        /// Cloud callout.
        /// </summary>
        CloudCallout = 106,
        /// <summary>
        /// Ellipse ribbon.
        /// </summary>
        EllipseRibbon = 107,
        /// <summary>
        /// Ellipse ribbon 2.
        /// </summary>
        EllipseRibbon2 = 108,
        /// <summary>
        /// Flow chart process.
        /// </summary>
        FlowChartProcess = 109,
        /// <summary>
        /// Flow chart decision.
        /// </summary>
        FlowChartDecision = 110,
        /// <summary>
        /// Flow chart input output.
        /// </summary>
        FlowChartInputOutput = 111,
        /// <summary>
        /// Flow chart predefined process
        /// </summary>
        FlowChartPredefinedProcess = 112,
        /// <summary>
        /// Flow chart internal storage.
        /// </summary>
        FlowChartInternalStorage = 113,
        /// <summary>
        /// Flow chart document.
        /// </summary>
        FlowChartDocument = 114,
        /// <summary>
        /// Flow chart multi document.
        /// </summary>
        FlowChartMultidocument = 115,
        /// <summary>
        /// Flow chart terminator.
        /// </summary>
        FlowChartTerminator = 116,
        /// <summary>
        /// Flow chart preparation.
        /// </summary>
        FlowChartPreparation = 117,
        /// <summary>
        /// Flow chart manual input.
        /// </summary>
        FlowChartManualInput = 118,
        /// <summary>
        /// Flow chart manual operation.
        /// </summary>
        FlowChartManualOperation = 119,
        /// <summary>
        /// Flow chart connector.
        /// </summary>
        FlowChartConnector = 120,
        /// <summary>
        /// Flow chart punched card.
        /// </summary>
        FlowChartPunchedCard = 121,
        /// <summary>
        /// Flow chart punched tape.
        /// </summary>
        FlowChartPunchedTape = 122,
        /// <summary>
        /// Flow chart summing junction.
        /// </summary>
        FlowChartSummingJunction = 123,
        /// <summary>
        /// Flow chart or.
        /// </summary>
        FlowChartOr = 124,
        /// <summary>
        /// Flow chart collate.
        /// </summary>
        FlowChartCollate = 125,
        /// <summary>
        /// Flow chart sort.
        /// </summary>
        FlowChartSort = 126,
        /// <summary>
        /// Flow chart extract.
        /// </summary>
        FlowChartExtract = 127,
        /// <summary>
        /// Flow chart merge.
        /// </summary>
        FlowChartMerge = 128,
        /// <summary>
        /// Flow chart off-line storage.
        /// </summary>
        FlowChartOfflineStorage = 129,
        /// <summary>
        /// Flow chart on-line storage.
        /// </summary>
        FlowChartOnlineStorage = 130,
        /// <summary>
        /// Flow char magnetic tape.
        /// </summary>
        FlowChartMagneticTape = 131,
        /// <summary>
        /// Flow chart magnetic disk.
        /// </summary>
        FlowChartMagneticDisk = 132,
        /// <summary>
        /// Flow chart magnetic drum.
        /// </summary>
        FlowChartMagneticDrum = 133,
        /// <summary>
        /// Flow chart display.
        /// </summary>
        FlowChartDisplay = 134,
        /// <summary>
        /// Flow chart delay.
        /// </summary>
        FlowChartDelay = 135,
        /// <summary>
        /// Plain-text, WordArt object.
        /// </summary>
        TextPlainText = 136,
        /// <summary>
        /// Stop, WordArt object.
        /// </summary>
        TextStop = 137,
        /// <summary>
        /// Triangle, WordArt object.
        /// </summary>
        TextTriangle = 138,
        /// <summary>
        /// Triangle inverted, WordArt object.
        /// </summary>
        TextTriangleInverted = 139,
        /// <summary>
        /// Chevron, WordArt object.
        /// </summary>
        TextChevron = 140,
        /// <summary>
        /// Chevron inverted, WordArt object.
        /// </summary>
        TextChevronInverted = 141,
        /// <summary>
        /// Ring inside, WordArt object.
        /// </summary>
        TextRingInside = 142,
        /// <summary>
        /// Ring outside, WordArt object.
        /// </summary>
        TextRingOutside = 143,
        /// <summary>
        /// Arch up curve, WordArt object.
        /// </summary>
        TextArchUpCurve = 144,
        /// <summary>
        /// Arch down curve, WordArt object.
        /// </summary>
        TextArchDownCurve = 145,
        /// <summary>
        /// Circle curve, WordArt object.
        /// </summary>
        TextCircleCurve = 146,
        /// <summary>
        /// Button curve, WordArt object.
        /// </summary>
        TextButtonCurve = 147,
        /// <summary>
        /// Arch up pour, WordArt object.
        /// </summary>
        TextArchUpPour = 148,
        /// <summary>
        /// Arch down pour, WordArt object.
        /// </summary>
        TextArchDownPour = 149,
        /// <summary>
        /// Circle pour, WordArt object.
        /// </summary>
        TextCirclePour = 150,
        /// <summary>
        /// Button pour, WordArt object.
        /// </summary>
        TextButtonPour = 151,
        /// <summary>
        /// Curve up, WordArt object.
        /// </summary>
        TextCurveUp = 152,
        /// <summary>
        /// Curve down, WordArt object.
        /// </summary>
        TextCurveDown = 153,
        /// <summary>
        /// Cascade up, WordArt object.
        /// </summary>
        TextCascadeUp = 154,
        /// <summary>
        /// Cascade down, WordArt object.
        /// </summary>
        TextCascadeDown = 155,
        /// <summary>
        /// Wave 1, WordArt object.
        /// </summary>
        TextWave1 = 156,
        /// <summary>
        /// Wave 2, WordArt object.
        /// </summary>
        TextWave2 = 157,
        /// <summary>
        /// Wave 3, WordArt object.
        /// </summary>
        TextWave3 = 158,
        /// <summary>
        /// Wave 4, WordArt object.
        /// </summary>
        TextWave4 = 159,
        /// <summary>
        /// Inflate, WordArt object.
        /// </summary>
        TextInflate = 160,
        /// <summary>
        /// Deflate, WordArt object.
        /// </summary>
        TextDeflate = 161,
        /// <summary>
        /// Inflate bottom, WordArt object.
        /// </summary>
        TextInflateBottom = 162,
        /// <summary>
        /// Deflate bottom, WordArt object.
        /// </summary>
        TextDeflateBottom = 163,
        /// <summary>
        /// Inflate top, WordArt object.
        /// </summary>
        TextInflateTop = 164,
        /// <summary>
        /// Deflate top, WordArt object.
        /// </summary>
        TextDeflateTop = 165,
        /// <summary>
        /// Deflate inflate, WordArt object.
        /// </summary>
        TextDeflateInflate = 166,
        /// <summary>
        /// Deflate inflate deflate, WordArt object.
        /// </summary>
        TextDeflateInflateDeflate = 167,
        /// <summary>
        /// Fade right, WordArt object.
        /// </summary>
        TextFadeRight = 168,
        /// <summary>
        /// Fade left, WordArt object.
        /// </summary>
        TextFadeLeft = 169,
        /// <summary>
        /// Fade up, WordArt object.
        /// </summary>
        TextFadeUp = 170,
        /// <summary>
        /// Fade down, WordArt object.
        /// </summary>
        TextFadeDown = 171,
        /// <summary>
        /// Slant up, WordArt object.
        /// </summary>
        TextSlantUp = 172,
        /// <summary>
        /// Slant down, WordArt object.
        /// </summary>
        TextSlantDown = 173,
        /// <summary>
        /// Can up, WordArt object.
        /// </summary>
        TextCanUp = 174,
        /// <summary>
        /// Can down, WordArt object.
        /// </summary>
        TextCanDown = 175,
        /// <summary>
        /// Flow chart alternate process.
        /// </summary>
        FlowChartAlternateProcess = 176,
        /// <summary>
        /// Flow chart off page connector.
        /// </summary>
        FlowChartOffpageConnector = 177,
        /// <summary>
        /// Callout 90.
        /// </summary>
        Callout90 = 178,
        /// <summary>
        /// Accent callout 90.
        /// </summary>
        AccentCallout90 = 179,
        /// <summary>
        /// Border callout 90.
        /// </summary>
        BorderCallout90 = 180,
        /// <summary>
        /// Accent border callout 90.
        /// </summary>
        AccentBorderCallout90 = 181,
        /// <summary>
        /// Left right up arrow.
        /// </summary>
        LeftRightUpArrow = 182,
        /// <summary>
        /// Sun.
        /// </summary>
        Sun = 183,
        /// <summary>
        /// Moon.
        /// </summary>
        Moon = 184,
        /// <summary>
        /// Bracket pair.
        /// </summary>
        BracketPair = 185,
        /// <summary>
        /// Brace pair
        /// </summary>
        BracePair = 186,
        /// <summary>
        /// Four-pointed star.
        /// </summary>
        Seal4 = 187,
        /// <summary>
        /// Double wave.
        /// </summary>
        DoubleWave = 188,
        /// <summary>
        /// Action button blank.
        /// </summary>
        ActionButtonBlank = 189,
        /// <summary>
        /// Action button home.
        /// </summary>
        ActionButtonHome = 190,
        /// <summary>
        /// Action button help.
        /// </summary>
        ActionButtonHelp = 191,
        /// <summary>
        /// Action button information.
        /// </summary>
        ActionButtonInformation = 192,
        /// <summary>
        /// Action button forward next.
        /// </summary>
        ActionButtonForwardNext = 193,
        /// <summary>
        /// Action button back previous.
        /// </summary>
        ActionButtonBackPrevious = 194,
        /// <summary>
        /// Action button end.
        /// </summary>
        ActionButtonEnd = 195,
        /// <summary>
        /// Action button beginning.
        /// </summary>
        ActionButtonBeginning = 196,
        /// <summary>
        /// Action button return.
        /// </summary>
        ActionButtonReturn = 197,
        /// <summary>
        /// Action button document.
        /// </summary>
        ActionButtonDocument = 198,
        /// <summary>
        /// Action button sound.
        /// </summary>
        ActionButtonSound = 199,
        /// <summary>
        /// Action button movie.
        /// </summary>
        ActionButtonMovie = 200,
        /// <summary>
        /// Snip single corner rectangle object.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        SingleCornerSnipped = 203,
        /// <summary>
        /// Snip same side corner rectangle.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        TopCornersSnipped = 204,
        /// <summary>
        /// Snip diagonal corner rectangle.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        DiagonalCornersSnipped = 205,
        /// <summary>
        /// Snip and round single corner rectangle.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        TopCornersOneRoundedOneSnipped = 206,
        /// <summary>
        /// Round single corner rectangle.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        SingleCornerRounded = 207,
        /// <summary>
        /// Round same side corner rectangle.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        TopCornersRounded = 208,
        /// <summary>
        /// Round diagonal corner rectangle.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        DiagonalCornersRounded = 209,
        /// <summary>
        /// Heptagon.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Heptagon = 210,
        /// <summary>
        /// Cloud.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Cloud = 211,
        /// <summary>
        /// Six-pointed star.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Seal6 = 212,
        /// <summary>
        /// Seven-pointed star.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Seal7 = 213,
        /// <summary>
        /// Ten-pointed star.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Seal10 = 214,
        /// <summary>
        /// Twelve-pointed star.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Seal12 = 215,
        /// <summary>
        /// Swoosh arrow.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        SwooshArrow = 216,
        /// <summary>
        /// Teardrop.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Teardrop = 217,
        /// <summary>
        /// Square tabs.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        SquareTabs = 218,
        /// <summary>
        /// Plaque tabs.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        PlaqueTabs = 219,
        /// <summary>
        /// Pie.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Pie = 220,
        /// <summary>
        /// Wedge pie.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        WedgePie = 221,
        /// <summary>
        /// Inverse line.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        InverseLine = 222,
        /// <summary>
        /// Math plus.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        MathPlus = 223,
        /// <summary>
        /// Math minus.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        MathMinus = 224,
        /// <summary>
        /// Math multiply.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        MathMultiply = 225,
        /// <summary>
        /// Math divide.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        MathDivide = 226,
        /// <summary>
        /// Math equal.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        MathEqual = 227,
        /// <summary>
        /// Math not equal.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        MathNotEqual = 228,
        /// <summary>
        /// Non-isosceles trapezoid.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        NonIsoscelesTrapezoid = 229,
        /// <summary>
        /// Left-right circular arrow.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        LeftRightCircularArrow = 230,
        /// <summary>
        /// Left-right ribbon.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        LeftRightRibbon = 231,
        /// <summary>
        /// Left circular arrow.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        LeftCircularArrow = 232,
        /// <summary>
        /// Frame.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Frame = 233,
        /// <summary>
        /// Half frame.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        HalfFrame = 234,
        /// <summary>
        /// Funnel.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Funnel = 235,
        /// <summary>
        /// Six-tooth gear.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Gear6 = 236,
        /// <summary>
        /// Nine-tooth gear.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Gear9 = 237,
        /// <summary>
        /// Decagon.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Decagon = 238,
        /// <summary>
        /// Dodecagon.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Dodecagon = 239,
        /// <summary>
        /// Diagonal stripe.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        DiagonalStripe = 240,
        /// <summary>
        /// Corner.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Corner = 241,
        /// <summary>
        /// Corner tabs.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        CornerTabs = 242,
        /// <summary>
        /// Chord.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        Chord = 243,
        /// <summary>
        /// Chart plus.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        ChartPlus = 244,
        /// <summary>
        /// Chart star.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        ChartStar = 245,
        /// <summary>
        /// Chart X.
        /// </summary>
        /// <remarks>
        /// Applicable only for DML shapes.
        /// </remarks>
        ChartX = 246,
        /// <summary>
        /// Reserved for the system use.
        /// </summary>
        MinValue = OleObject
    }
}
