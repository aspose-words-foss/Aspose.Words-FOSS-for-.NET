// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin


using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Attributes that can be defined for a paragraph.
    ///
    /// Note the constant values make sure the attributes are written
    /// into a binary file in a specific order and the order is important.
    /// </summary>
    [CppConstexpr]
    internal static class ParaAttr
    {
        /// <summary>
        /// int, istd of the paragraph style
        /// </summary>
        internal const int Istd = 1000;
        /// <summary>
        /// enum
        /// </summary>
        internal const int Alignment = 1020;
        /// <summary>
        /// bool
        /// </summary>
        internal const int NoSpaceBetweenSameStyle = 1022;
        /// <summary>
        /// bool, not documented well
        /// </summary>
        internal const int SideBySide = 1030;
        /// <summary>
        /// bool
        /// </summary>
        internal const int KeepTogether = 1040;
        /// <summary>
        /// bool
        /// </summary>
        internal const int KeepWithNext = 1050;
        /// <summary>
        /// bool
        /// </summary>
        internal const int PageBreakBefore = 1060;
        /// <summary>
        /// bool, asian
        /// </summary>
        internal const int FarEastLineBreakControl = 1070;
        /// <summary>
        /// bool, asian
        /// </summary>
        internal const int WordWrap = 1080;
        /// <summary>
        /// bool, asian
        /// </summary>
        internal const int HangingPunctuation = 1090;
        /// <summary>
        /// bool, asian
        /// </summary>
        internal const int TopLinePunctuation = 1100;
        /// <summary>
        /// int
        /// </summary>
        internal const int ListLevel = 1110;
        /// <summary>
        /// int
        /// </summary>
        internal const int ListId = 1120;

        /// <summary>
        /// bool
        /// </summary>
        internal const int SuppressLineNumbers = 1130;
        /// <summary>
        /// TabStopCollection, complex, at the moment cannot be inherited.
        /// </summary>
        internal const int TabStops = 1140;
        /// <summary>
        /// bool
        /// </summary>
        internal const int MirrorIndents = 1145;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int RightIndent = 1150;
        /// <summary>
        /// int, 1/100 of character unit.
        /// </summary>
        internal const int RightIndentUnits = 1155;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int LeftIndent = 1160;
        /// <summary>
        /// int, 1/100 of character unit.
        /// </summary>
        internal const int LeftIndentUnits = 1165;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int FirstLineIndent = 1170;
        /// <summary>
        /// int, 1/100 of character unit.
        /// </summary>
        internal const int FirstLineIndentUnits = 1175;
        /// <summary>
        /// line spacing, complex.
        /// </summary>
        internal const int LineSpacing = 1650;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int SpaceBefore = 1200;
        /// <summary>
        /// int, 1/100 of line unit.
        /// </summary>
        internal const int SpaceBeforeUnits = 1205;
        /// <summary>
        /// bool
        /// </summary>
        internal const int SpaceBeforeAuto = 1210;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int SpaceAfter = 1220;
        /// <summary>
        /// int, 1/100 of line unit.
        /// </summary>
        internal const int SpaceAfterUnits = 1225;
        /// <summary>
        /// bool
        /// </summary>
        internal const int SpaceAfterAuto = 1230;
        /// <summary>
        /// bool, asian
        /// </summary>
        internal const int AddSpaceBetweenFarEastAndAlpha = 1240;
        /// <summary>
        /// bool, asian
        /// </summary>
        internal const int AddSpaceBetweenFarEastAndDigit = 1250;
        /// <summary>
        /// bool
        /// </summary>
        internal const int SnapToGrid = 1260;
        /// <summary>
        /// bool
        /// </summary>
        internal const int AutoAdjustRightIndent = 1270;
        /// <summary>
        /// OutlineLevel
        /// </summary>
        internal const int OutlineLevel = 1280;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameHorizontalAlignment = 1290;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameLeft = 1292;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameVerticalAlignment = 1300;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameTop = 1302;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameWidth = 1310;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameRelativeHorizontalPosition = 1320;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameRelativeVerticalPosition = 1330;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameWrapType = 1340;
        /// <summary>
        /// bool. Text frame.
        /// </summary>
        internal const int FrameSuppressOverlap = 1660;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderTop = 1350;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderLeft = 1360;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderBottom = 1370;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderRight = 1380;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderBetween = 1390;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderBar = 1400;
        /// <summary>
        /// bool
        /// </summary>
        internal const int SuppressAutoHyphens = 1410;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameHeight = 1430;
        /// <summary>
        /// enum
        /// </summary>
        internal const int DropCapPosition = 1440;
        /// <summary>
        /// int
        /// </summary>
        internal const int DropCapLinesToDrop = 1450;
        /// <summary>
        /// Shading, complex, can be inherited
        /// </summary>
        internal const int Shading = 1460;
        /// <summary>
        /// bool
        /// </summary>
        internal const int WidowControl = 1470;
        /// <summary>
        /// enum. Text frame.
        /// </summary>
        internal const int FrameTextOrientation = 1480;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameVerticalDistanceFromText = 1490;
        /// <summary>
        /// int, twips. Text frame.
        /// </summary>
        internal const int FrameHorizontalDistanceFromText = 1500;
        /// <summary>
        /// enum
        /// </summary>
        internal const int BaselineAlignment = 1510;
        /// <summary>
        /// bool. Text frame.
        /// </summary>
        internal const int FrameLockAnchor = 1520;
        /// <summary>
        /// bool
        /// </summary>
        internal const int Bidi = 1560;
        /// <summary>
        /// int.
        /// Specifies a unique identifier used to track the editing session when the
        /// paragraph's properties were last modified in this document.
        /// </summary>
        internal const int RsidP = 1580;

        /// <summary>
        /// HTML related information <see cref="HtmlBlock" /> is applied to this paragraph.
        /// </summary>
        internal const int HtmlBlockId = 1585;

        /// <summary>
        /// Specifies text wrapping options <see cref="TextboxTightWrap"/> for a text box when tight wrapping is set.
        /// </summary>
        internal const int TextboxTightWrap = 1590;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> left margin.
        /// </summary>
        internal const int HtmlMarginLeft = 1592;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> right margin.
        /// </summary>
        internal const int HtmlMarginRight = 1593;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> top margin.
        /// </summary>
        internal const int HtmlMarginTop = 1594;

        /// <summary>
        /// HTML block <see cref="HtmlBlock" /> bottom margin.
        /// </summary>
        internal const int HtmlMarginBottom = 1595;

        /// <summary>
        /// Not in the model.
        /// Used to temporarily store a non bidi aware Word 97 attribute that is resolved into a model attribute.
        /// </summary>
        internal const int Sys_Alignment97 = 1610;
        /// <summary>
        /// Not in the model.
        /// Used to temporarily store a non bidi aware Word 97 attribute that is resolved into a model attribute.
        /// </summary>
        internal const int Sys_LeftIndent97 = 1620;
        /// <summary>
        /// Not in the model.
        /// Used to temporarily store a non bidi aware Word 97 attribute that is resolved into a model attribute.
        /// </summary>
        internal const int Sys_RightIndent97 = 1630;

        internal const int Sys_FirstLineIndent97 = 1640;

        /// <summary>
        /// Word60 list definition. Used only during import of Word95 files.
        /// </summary>
        internal const int Sys_Anld = 1641;

        /// <summary>
        /// Word60 list definition. Used only during import of Word95 files.
        /// </summary>
        internal const int Sys_LvlAnm = 1642;

        /// <summary>
        /// Not in the model.
        /// Used to temporarily mark a list item paragraph for restarting numbering that will be performed later
        /// for entire document.
        /// </summary>
        internal const int Sys_RestartNumbering = 1643;

        /// <summary>
        /// Collapsed state of a paragraph. MS Word 2013 and higher.
        /// </summary>
        internal const int Collapsed = 1670;
    }
}
