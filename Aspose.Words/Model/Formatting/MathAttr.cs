// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2011 by Denis Darkin

using Aspose.Words.Math;

namespace Aspose.Words
{
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class MathAttr
    {
        /// <summary>
        /// <see cref="OfficeMathJustification"/>
        /// </summary>
        internal const int Justification = 15010;

        /// <summary>
        /// <see cref="MathPosition"/>
        /// </summary>
        internal const int Position = 15030;

        /// <summary>
        /// char
        /// </summary>
        /// <remarks>We could have generic Character attribute, but different math objects 
        /// have different default characters, so we have to introduce multiple XxxxCharacter attrs
        /// <see cref="GroupCharPosition"/> for example.</remarks>
        internal const int AccentCharacter = 15040;

        /// <summary>
        /// char
        /// </summary>
        internal const int NaryChar = 15045;

        /// <summary>
        /// bool
        /// </summary>
        internal const int HideBottom = 15050;

        /// <summary>
        /// bool
        /// </summary>
        internal const int HideTop = 15060;

        /// <summary>
        /// bool
        /// </summary>
        internal const int HideLeft = 15070;

        /// <summary>
        /// bool
        /// </summary>
        internal const int HideRight = 15080;

        /// <summary>
        /// bool
        /// </summary>
        internal const int StrikeBLTR = 15090;

        /// <summary>
        /// bool
        /// </summary>
        internal const int StrikeH = 15100;

        /// <summary>
        /// bool
        /// </summary>
        internal const int StrikeTLBR = 15110;

        /// <summary>
        /// bool
        /// </summary>
        internal const int StrikeV = 15120;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsOpEmu = 15130;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsAlignmentPoint = 15140;

        /// <summary>
        /// <see cref="LineBreak"/>
        /// </summary>
        internal const int LineBreak = 15150;
        
        /// <summary>
        /// bool
        /// </summary>
        internal const int NoBreaks = 15160;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsDifferential = 15170;

        /// <summary>
        /// char
        /// </summary>
        internal const int BeginChar = 15180;

        /// <summary>
        /// char
        /// </summary>
        internal const int EndChar = 15190;

        /// <summary>
        /// char
        /// </summary>
        internal const int SeparatorChar = 15200;

        /// <summary>
        /// bool
        /// </summary>
        internal const int GrowOperand = 15210;

        /// <summary>
        /// <see cref="MathDelimiterShape"/>
        /// </summary>
        internal const int DelimiterShape = 15220;

        /// <summary>
        /// <see cref="MathBaseJustification "/>
        /// </summary>
        internal const int BaseJustification = 15230;

        /// <summary>
        /// bool.
        /// </summary>
        internal const int MaxDist = 15240;

        /// <summary>
        /// bool
        /// </summary>
        internal const int ObjDist = 15250;

        /// <summary>
        /// int
        /// </summary>
        internal const int RowSpacing = 15260;

        /// <summary>
        /// <see cref="MathSpacingRule "/>
        /// </summary>
        internal const int RowSpacingRule = 15270;

        /// <summary>
        /// char
        /// </summary>
        internal const int GroupChar = 15280;

        /// <summary>
        /// <see cref="MathPosition"/>
        /// </summary>
        /// <remarks>Could have reused <see cref="MathPosition"/> but it seems to break MS Word order for docx writing.</remarks>
        internal const int GroupCharPosition = 15290;

        /// <summary>
        /// <see cref="MathVerticalJustification"/>
        /// </summary>
        internal const int VerticalJustification = 15300;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsShown = 15310;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsTransparent = 15320;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsZeroAscent = 15330;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsZeroDescent = 15340;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsZeroWidth = 15450;

        /// <summary>
        /// <see cref="FractionType"/>
        /// </summary>
        internal const int FractionType = 15460;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsHidePlaceholders = 15470;

        /// <summary>
        /// int
        /// </summary>
        internal const int MinColumnWidth = 15480;

        /// <summary>
        /// <see cref="MathSpacingRule"/>
        /// </summary>
        internal const int ColumnGapRule = 15490;

        /// <summary>
        /// int
        /// </summary>
        internal const int ColumnGap = 15500;


        /// <summary>
        /// <see cref="MathLimitLocation"/>
        /// </summary>
        internal const int LimitLocation = 15510;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsHideSubscript = 15520;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsHideSuperscript = 15530;

        /// <summary>
        /// bool
        /// </summary>
        internal const int DegreeHide = 15540;

        /// <summary>
        /// bool
        /// </summary>
        internal const int IsAlignScripts = 15550;
    }
}
