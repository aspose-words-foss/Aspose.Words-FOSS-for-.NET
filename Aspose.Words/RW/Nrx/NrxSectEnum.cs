// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2009 by Roman Korchagin

using System;
using Aspose.Words.Notes;

namespace Aspose.Words.RW.Nrx
{
    internal static class NrxSectEnum
    {
        internal static FootnoteNumberingRule XmlToNoteNumberingRule(string value)
        {
            switch (value)
            {
                case "continuous": return FootnoteNumberingRule.Continuous;
                case "eachSect": return FootnoteNumberingRule.RestartSection;
                case "each-sect": return FootnoteNumberingRule.RestartSection;
                case "eachPage": return FootnoteNumberingRule.RestartPage;
                case "each-page": return FootnoteNumberingRule.RestartPage;
                default: return FootnoteNumberingRule.Default;
            }
        }

        internal static string NoteNumberingRuleToXml(FootnoteNumberingRule value, bool isDocx)
        {
            if (value == FootnoteNumberingRule.Default)
                return "";

            switch (value)
            {
                case FootnoteNumberingRule.Continuous: return "continuous";
                case FootnoteNumberingRule.RestartSection: return (isDocx) ? "eachSect" : "each-sect";
                case FootnoteNumberingRule.RestartPage: return (isDocx) ? "eachPage" : "each-page";
                default: return "";
            }
        }

        internal static SectionLayoutMode XmlToDocGridType(string value)
        {
            switch (value)
            {
                case "linesAndChars":
                case "lines-and-chars":
                    return SectionLayoutMode.Grid;
                case "lines":
                    return SectionLayoutMode.LineGrid;
                case "snapToChars":
                case "snap-to-chars":
                    return SectionLayoutMode.SnapToChars;
                default:
                    return SectionLayoutMode.Default;
            }
        }

        internal static string DocGridTypeToXml(SectionLayoutMode value, bool isDocx)
        {
            switch (value)
            {
                case SectionLayoutMode.Grid:
                    return isDocx ? "linesAndChars" : "lines-and-chars";
                case SectionLayoutMode.LineGrid:
                    return "lines";
                case SectionLayoutMode.SnapToChars:
                    return isDocx ? "snapToChars" : "snap-to-chars";
                default:
                    return "";
            }
        }

        internal static HeaderFooterType XmlToHeaderFooterType(string value, bool isHeader)
        {
            switch (value)
            {
                case "even":
                    if (isHeader)
                        return HeaderFooterType.HeaderEven;
                    else
                        return HeaderFooterType.FooterEven;
                case "first":
                    if (isHeader)
                        return HeaderFooterType.HeaderFirst;
                    else
                        return HeaderFooterType.FooterFirst;
                default:
                    if (isHeader)
                        return HeaderFooterType.HeaderPrimary;
                    else
                        return HeaderFooterType.FooterPrimary;
            }
        }

        internal static string HeaderFooterTypeToXml(HeaderFooterType value, bool isDocx)
        {
            switch (value)
            {
                case HeaderFooterType.HeaderEven:
                case HeaderFooterType.FooterEven:
                    return "even";
                case HeaderFooterType.HeaderFirst:
                case HeaderFooterType.FooterFirst:
                    return "first";
                case HeaderFooterType.HeaderPrimary:
                case HeaderFooterType.FooterPrimary:
                    return isDocx ? "default" : "odd";
                default:
                    throw new InvalidOperationException("Unknown header footer type.");
            }
        }

        internal static FootnotePosition XmlToFootnotePosition(string value)
        {
            switch (value)
            {
                case "pageBottom":
                case "page-bottom":
                    return FootnotePosition.BottomOfPage;
                case "beneathText":
                case "beneath-text":
                    return FootnotePosition.BeneathText;
                default:
                    return FootnotePosition.BottomOfPage;
            }
        }

        internal static EndnotePosition XmlToEndnotePosition(string value)
        {
            switch (value)
            {
                case "sectEnd":
                case "sect-end":
                    return EndnotePosition.EndOfSection;
                case "docEnd":
                case "doc-end":
                    return EndnotePosition.EndOfDocument;
                default:
                    return EndnotePosition.EndOfDocument;
            }
        }

        internal static string FootnotePositionToXml(FootnotePosition value, bool isDocx)
        {
            switch (value)
            {
                case FootnotePosition.BottomOfPage: return isDocx ? "pageBottom" : "page-bottom";
                case FootnotePosition.BeneathText: return isDocx ? "beneathText" : "beneath-text";
                default: return "";
            }
        }

        internal static string EndnotePositionToXml(EndnotePosition value, bool isDocx)
        {
            switch (value)
            {
                case EndnotePosition.EndOfSection: return isDocx ? "sectEnd" : "sect-end";
                case EndnotePosition.EndOfDocument: return isDocx ? "docEnd" : "doc-end";
                default: return "";
            }
        }

        internal static PageVerticalAlignment XmlToPageVerticalAlignment(string value)
        {
            switch (value)
            {
                case "top": return PageVerticalAlignment.Top;
                case "center": return PageVerticalAlignment.Center;
                case "both": return PageVerticalAlignment.Justify;
                case "bottom": return PageVerticalAlignment.Bottom;
                default: return PageVerticalAlignment.Top;
            }
        }

        internal static string PageVerticalAlignmentToXml(PageVerticalAlignment value)
        {
            switch (value)
            {
                case PageVerticalAlignment.Top: return "top";
                case PageVerticalAlignment.Center: return "center";
                case PageVerticalAlignment.Justify: return "both";
                case PageVerticalAlignment.Bottom: return "bottom";
                default: return "";
            }
        }

        internal static ChapterPageSeparator XmlToChapterPageSeparator(string value)
        {
            switch (value)
            {
                case "hyphen":
                    return ChapterPageSeparator.Hyphen;
                case "period":
                    return ChapterPageSeparator.Period;
                case "colon":
                    return ChapterPageSeparator.Colon;
                case "emDash":
                case "em-dash":
                    return ChapterPageSeparator.EmDash;
                case "enDash":
                case "en-dash":
                    return ChapterPageSeparator.EnDash;
                default:
                    return ChapterPageSeparator.Hyphen;
            }
        }

        internal static string ChapterPageSeparatorToXml(ChapterPageSeparator value, bool isDocx)
        {
            switch (value)
            {
                case ChapterPageSeparator.Hyphen: return "hyphen";
                case ChapterPageSeparator.Period: return "period";
                case ChapterPageSeparator.Colon: return "colon";
                case ChapterPageSeparator.EmDash: return isDocx ? "emDash" : "em-dash";
                case ChapterPageSeparator.EnDash: return isDocx ? "enDash" : "en-dash";
                default: return "";
            }
        }

        internal static LineNumberRestartMode XmlToLineNumberRestartMode(string value)
        {
            switch (value)
            {
                case "continuous":
                    return LineNumberRestartMode.Continuous;
                case "newPage":
                case "new-page":
                    return LineNumberRestartMode.RestartPage;
                case "newSection":
                case "new-section":
                    return LineNumberRestartMode.RestartSection;
                default:
                    return LineNumberRestartMode.RestartPage;
            }
        }

        internal static string LineNumberRestartModeToXml(LineNumberRestartMode value, bool isDocx)
        {
            switch (value)
            {
                case LineNumberRestartMode.Continuous: return "continuous";
                case LineNumberRestartMode.RestartPage: return isDocx ? "newPage" : "new-page";
                case LineNumberRestartMode.RestartSection: return isDocx ? "newSection" : "new-section";
                default: return "";
            }
        }

        internal static PageBorderDistanceFrom XmlToPageBorderDistanceFrom(string value)
        {
            switch (value)
            {
                case "page": return PageBorderDistanceFrom.PageEdge;
                case "text": return PageBorderDistanceFrom.Text;
                default: return PageBorderDistanceFrom.PageEdge;
            }
        }

        internal static string PageBorderDistanceFromToXml(PageBorderDistanceFrom value)
        {
            switch (value)
            {
                case PageBorderDistanceFrom.PageEdge: return "page";
                case PageBorderDistanceFrom.Text: return "text";
                default: return "";
            }
        }

        internal static PageBorderAppliesTo XmlToPageBorderAppliesTo(string value)
        {
            switch (value)
            {
                case "allPages":
                case "all-pages":
                    return PageBorderAppliesTo.AllPages;
                case "firstPage":
                case "first-page":
                    return PageBorderAppliesTo.FirstPage;
                case "notFirstPage":
                case "not-first-page":
                    return PageBorderAppliesTo.OtherPages;
                default:
                    return PageBorderAppliesTo.AllPages;
            }
        }

        internal static string PageBorderAppliesToToXml(PageBorderAppliesTo value, bool isDocx)
        {
            switch (value)
            {
                case PageBorderAppliesTo.AllPages: return isDocx ? "allPages" : "all-pages";
                case PageBorderAppliesTo.FirstPage: return isDocx ? "firstPage" : "first-page";
                case PageBorderAppliesTo.OtherPages: return isDocx ? "notFirstPage" : "not-first-page";
                default: return "";
            }
        }

        internal static Orientation XmlToOrientation(string value)
        {
            switch (value)
            {
                case "landscape": return Orientation.Landscape;
                case "portrait": return Orientation.Portrait;
                default: return Orientation.Portrait;
            }
        }

        internal static string OrientationToXml(Orientation value)
        {
            switch (value)
            {
                case Orientation.Landscape: return "landscape";
                case Orientation.Portrait: return "portrait";
                default: return "";
            }
        }

        internal static SectionStart XmlToSectionStart(string value)
        {
            switch (value)
            {
                case "continuous":
                    return SectionStart.Continuous;
                case "evenPage":
                case "even-page":
                    return SectionStart.EvenPage;
                case "nextColumn":
                case "next-column":
                    return SectionStart.NewColumn;
                case "nextPage":
                case "next-page":
                    return SectionStart.NewPage;
                case "oddPage":
                case "odd-page":
                    return SectionStart.OddPage;
                default:
                    return SectionStart.NewPage;
            }
        }

        internal static string SectionStartToXml(SectionStart value, bool isDocx)
        {
            switch (value)
            {
                case SectionStart.Continuous: return "continuous";
                case SectionStart.EvenPage: return isDocx ? "evenPage" : "even-page";
                case SectionStart.NewColumn: return isDocx ? "nextColumn" : "next-column";
                case SectionStart.NewPage: return isDocx ? "nextPage" : "next-page";
                case SectionStart.OddPage: return isDocx ? "oddPage" : "odd-page";
                default: return "";
            }
        }
    }
}
