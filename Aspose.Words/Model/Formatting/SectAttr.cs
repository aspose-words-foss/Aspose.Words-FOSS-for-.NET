// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

using Aspose.Words.Notes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Attributes that can be defined for a section.
    ///
    /// Note the constant values make sure the attributes are written
    /// into a binary file in a specific order and the order is important.
    /// </summary>
    [CppConstexpr]
    internal class SectAttr
    {
        /// <summary>
        /// enum
        /// </summary>
        internal const int PageNumberStyle = 2010;
        /// <summary>
        /// enum
        /// </summary>
        internal const int ChapterPageSeparator = 2020;
        /// <summary>
        /// SectionStart as enum
        /// </summary>
        internal const int SectionStart = 2030;
        /// <summary>
        /// bool
        /// </summary>
        internal const int DifferentFirstPageHeaderFooter = 2040;
        /// <summary>
        /// bool
        /// </summary>
        internal const int RestartPageNumbering = 2050;
        /// <summary>
        /// bool
        /// </summary>
        internal const int ColumnsLineBetween = 2060;
        /// <summary>
        /// int
        /// </summary>
        internal const int FirstPageTray = 2070;
        /// <summary>
        /// int
        /// </summary>
        internal const int OtherPagesTray = 2080;
        /// <summary>
        /// int, undocumented
        /// </summary>
        internal const int PaperCode = 2090;
        /// <summary>
        /// bool
        /// </summary>
        internal const int SuppressEndnotes = 2100;
        /// <summary>
        /// enum
        /// </summary>
        internal const int LineNumberRestartMode = 2110;
        /// <summary>
        /// int
        /// </summary>
        internal const int LineNumberCountBy = 2120;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderTop = 2130;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderLeft = 2140;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderBottom = 2150;
        /// <summary>
        /// Border, complex, can be inherited
        /// </summary>
        internal const int BorderRight = 2160;
        /// <summary>
        /// int
        /// </summary>
        internal const int LinePitch = 2170;
        /// <summary>
        /// int
        /// </summary>
        internal const int LineStartingNumber = 2180;
        /// <summary>
        /// int
        /// </summary>
        internal const int HeadingLevelForChapter = 2190;
        /// <summary>
        /// int
        /// </summary>
        internal const int PageStartingNumber = 2200;
        /// <summary>
        /// enum
        /// </summary>
        internal const int Orientation = 2210;
        /// <summary>
        /// enum
        /// </summary>
        internal const int BorderAppliesTo = 2220;
        /// <summary>
        /// bool
        /// </summary>
        internal const int BorderAlwaysInFront = 2230;
        /// <summary>
        /// enum
        /// </summary>
        internal const int BorderDistanceFrom = 2240;
        /// <summary>
        /// Specifies a revision save ID for the section.
        /// </summary>
        /// <remarks>
        /// int, no default defined for this one.
        /// </remarks>
        internal const int Rsid = 2250;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int PageWidth = 2260;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int PageHeight = 2270;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int LeftMargin = 2280;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int RightMargin = 2290;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int TopMargin = 2300;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int BottomMargin = 2310;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int Gutter = 2312;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int HeaderDistance = 2320;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int FooterDistance = 2330;
        /// <summary>
        /// enum
        /// </summary>
        internal const int VerticalAlignment = 2340;
        /// <summary>
        /// int
        /// </summary>
        internal const int ColumnsCount = 2350;
        /// <summary>
        /// bool
        /// </summary>
        internal const int ColumnsEvenlySpaced = 2360;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int ColumnsSpacing = 2370;
        /// <summary>
        /// TextColumnCollection, complex attribute, cannot be inherited. No default for this one.
        /// </summary>
        internal const int Columns = 2380;
        /// <summary>
        /// bool
        /// </summary>
        internal const int Unlocked = 2390;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int LineNumberDistanceFromText = 2400;
        /// <summary>
        /// bool
        /// </summary>
        internal const int RtlGutter = 2410;
        /// <summary>
        /// int
        /// </summary>
        internal const int CharSpace = 2420;
        /// <summary>
        /// VA: I have implemented DocGridType enum.
        /// Test document is Section\TestDocGridType.doc.
        /// </summary>
        internal const int GridType = 2430;
        /// <summary>
        /// enum
        /// </summary>
        internal const int TextFlow = 2440;
        /// <summary>
        /// bool
        /// </summary>
        internal const int Bidi = 2450;

        /// <summary>
        /// Don't renumber these.
        /// enums <see cref="FootnotePosition"/>, <see cref="Words.Notes.FootnoteLocation"/>
        /// </summary>
        internal const int FootnoteLocation = 2500;
        /// <summary>
        /// enum <see cref="Words.Notes.FootnoteNumberingRule"/>
        /// </summary>
        internal const int FootnoteNumberingRule = 2510;
        /// <summary>
        /// int
        /// </summary>
        internal const int FootnoteStartNumber = 2520;
        /// <summary>
        /// enum <see cref="Aspose.Words.NumberStyle"/>
        /// </summary>
        internal const int FootnoteNumberStyle = 2530;
        /// <summary>
        /// Number of footnote columns.
        /// </summary>
        internal const int FootnoteColumns = 2540;

        /// <summary>
        /// Don't renumber these.
        /// This is only a document-wide setting in DocPr. If encountered in SectPr it should be ignored.
        /// enums <see cref="EndnotePosition"/>, <see cref="Words.Notes.FootnoteLocation"/>
        /// </summary>
        internal const int EndnoteLocation = 2600;
        /// <summary>
        /// enum <see cref="Aspose.Words.Notes.FootnoteNumberingRule"/>
        /// </summary>
        internal const int EndnoteNumberingRule = 2610;
        /// <summary>
        /// int
        /// </summary>
        internal const int EndnoteStartNumber = 2620;
        /// <summary>
        /// enum <see cref="Aspose.Words.NumberStyle"/>
        /// </summary>
        internal const int EndnoteNumberStyle = 2630;

        /// <summary>
        /// Keys for endnote attributes are same as footnotes, just need to add this value.
        /// </summary>
        internal const int EndnoteKeyDelta = SectAttr.EndnoteLocation - SectAttr.FootnoteLocation;

        /// <summary>
        /// Header/Footer presence flags. Used only during Word 6.0 document import.
        /// </summary>
        internal const int Sys_GprfIhdt = 2650;
    }
}
