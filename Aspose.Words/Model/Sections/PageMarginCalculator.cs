// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2018 by Dmitry Matveenko

using System;
using System.Drawing;
using Aspose.Words.Settings;

namespace Aspose.Words
{
    /// <summary>
    /// Implements page margin calculation for different multiple page setups.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The approach is to save all properties affecting margins calculation to read-only fields.
    /// So only the constructor needs to be different to handle Model or Layout objects.
    /// The logic that handles the properties is the same.
    /// </para>
    /// <para>There is an inconsistent MS Word behavior for 2-page per sheet and book fold setups.
    /// The gutter is present both on top of the page and on the side for some page orientations.
    /// Also, side gutter is sometimes taken into account and some times it is not when this condition happen.
    /// The behavior was implemented per WORDSNET-16284</para>
    /// </remarks>
    internal class PageMarginCalculator
    {
        /// <summary>
        /// Initializes a new instance of this class by specifying properties with an object of
        /// the <see cref="PageProperties"/> class.
        /// </summary>
        internal PageMarginCalculator(PageProperties pageProperties)
        {
            LeftMargin = pageProperties.LeftMargin;
            RightMargin = pageProperties.RightMargin;

            // Negative margin is treated as positive for content positioning. Word behavior.
            // See PagePart.ContentTop for how negative margin is different from positive.
            TopMargin = System.Math.Abs(pageProperties.TopMargin);
            BottomMargin = System.Math.Abs(pageProperties.BottomMargin);

            Gutter = pageProperties.Gutter;

            MultiplePages = pageProperties.MultiplePages;
            DifferentOddEvenPageHeader = pageProperties.DifferentOddEvenPageHeader;

            Orientation = pageProperties.Orientation;
            OrientationMatchesFirstSection = pageProperties.OrientationMatchesFirstSection;

            IsGutterAtTopSet = pageProperties.IsGutterAtTopSet;
            IsRtlGutter = pageProperties.IsRtlGutter;
        }

        /// <summary>
        /// Initializes a new instance of the class section and document properties in the Model rather than from layout.
        /// </summary>
        internal PageMarginCalculator(ISectionAttrSource section, DocPr docPr, Orientation firstSectionOrientation)
        {
            // There is a unit difference between the two constructors.
            // This one actually gets margins in twips, and the other one in lis.
            // I think this is not a problem as long as the units match in the instance.
            LeftMargin = (int)section.FetchSectionAttr(SectAttr.LeftMargin);
            RightMargin = (int)section.FetchSectionAttr(SectAttr.RightMargin);

            // Negative margin is treated as positive for content positioning. Word behavior.
            // See PagePart.ContentTop for how negative margin is different from positive.
            TopMargin = System.Math.Abs((int)section.FetchSectionAttr(SectAttr.TopMargin));
            BottomMargin = System.Math.Abs((int)section.FetchSectionAttr(SectAttr.BottomMargin));

            Gutter = (int)section.FetchSectionAttr(SectAttr.Gutter);

            MultiplePages = docPr.MultiplePages;
            DifferentOddEvenPageHeader = docPr.EvenAndOddHeaders;

            Orientation = (Orientation)section.FetchSectionAttr(SectAttr.Orientation);
            OrientationMatchesFirstSection = (Orientation == firstSectionOrientation);

            IsGutterAtTopSet = docPr.GutterAtTop;
            IsRtlGutter = (bool)section.FetchSectionAttr(SectAttr.RtlGutter);
        }

        /// <summary>
        /// Gets the page margin and gutter on the given page side.
        /// </summary>
        internal int GetMarginWithGutter(PageSide side, bool isOddPage)
        {
            int margin;

            switch (side)
            {
                case PageSide.Top:
                    margin = GetTopMargin(isOddPage);
                    break;
                case PageSide.Bottom:
                    margin = GetBottomMargin(isOddPage);
                    break;
                case PageSide.Left:
                    margin = GetLeftMargin(isOddPage);
                    break;
                case PageSide.Right:
                    margin = GetRightMargin(isOddPage);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected page side value.");
            }

            int effectiveGutter = GetEffectiveGutter(side, isOddPage, GutterHandling.SideGutterNormal);
            return margin + effectiveGutter;
        }

        /// <summary>
        /// Gets a boolean value indicating if side gutter should taken into account for column width calculation.
        /// </summary>
        /// <remarks>
        /// There is a rare case for some of the multiple page setups when side gutter is present,
        /// but column text is allowed to overlap it. The method handles the condition (ignores side gutter if needed).
        /// </remarks>
        internal static bool GetPageHasSideGutter(SectPr sectionPr, DocPr docPr, Orientation firstSectionOrientation)
        {
            PageMarginCalculator calculator = new PageMarginCalculator(sectionPr, docPr, firstSectionOrientation);

            const bool anyPage = true;
            int sideGutter = calculator.GetEffectiveGutter(PageSide.Left, anyPage, GutterHandling.DisregardSideGutterForDoubleGutterCase) +
                calculator.GetEffectiveGutter(PageSide.Right, anyPage, GutterHandling.DisregardSideGutterForDoubleGutterCase);

            return (sideGutter > 0);
        }

        /// <summary>
        /// Implements width metrics calculation taking caller context specified via <paramref name="gutterBehaviorContext"/> into account.
        /// </summary>
        internal Point GetWidthMetrics(GutterBehavior gutterBehaviorContext, int pageWidth, bool isOddPage,
            bool isWord2013OrLaterCompatible)
        {
            bool disregardSideGutterForDoubleGutterCase;
            switch (gutterBehaviorContext)
            {
                case GutterBehavior.Normal:
                    disregardSideGutterForDoubleGutterCase = false;
                    break;
                case GutterBehavior.Column:
                    disregardSideGutterForDoubleGutterCase = true;
                    break;
                case GutterBehavior.HeaderFooter:
                    // Side gutter is disregarded in compatibility mode when there is a gutter on top as well.
                    disregardSideGutterForDoubleGutterCase = !isWord2013OrLaterCompatible;
                    break;
                case GutterBehavior.FloatingShapePositioning:
                    // Side gutter is disregarded in 2013 mode when there is a gutter on top as well.
                    disregardSideGutterForDoubleGutterCase = isWord2013OrLaterCompatible;
                    break;
                default:
                    throw new InvalidOperationException("Unknown gutter behavior");
            }

            return disregardSideGutterForDoubleGutterCase
                ? GetWidthMetricsWithSpecialGutterCondition(pageWidth, isOddPage)
                : GetWidthMetrics(pageWidth, isOddPage);
        }

        /// <summary>
        /// Implements page width metrics calculation for normal gutter behavior.
        /// </summary>
        private Point GetWidthMetrics(int pageWidth, bool isOddPage)
        {
            int leftMargin = GetMarginWithGutter(PageSide.Left, isOddPage);
            int rightMargin = GetMarginWithGutter(PageSide.Right, isOddPage);

            return new Point(leftMargin, pageWidth - rightMargin);
        }

        /// <summary>
        /// Gets page width metrics similar to <see cref="GetWidthMetrics(int, bool)"/>,
        /// but takes a special condition to ignore side gutter into account.
        /// </summary>
        /// <remarks>
        /// There is a rare case for some of the multiple page setups when side gutter is present,
        /// but column text is allowed to overlap it. The method handles the condition (ignores side gutter if needed).
        /// </remarks>
        private Point GetWidthMetricsWithSpecialGutterCondition(int pageWidth, bool isOddPage)
        {
            int leftMargin = GetLeftMargin(isOddPage);
            int rightMargin = GetRightMargin(isOddPage);

            int leftGutter = GetEffectiveGutter(PageSide.Left, isOddPage, GutterHandling.DisregardSideGutterForDoubleGutterCase);
            int rightGutter = GetEffectiveGutter(PageSide.Right, isOddPage, GutterHandling.DisregardSideGutterForDoubleGutterCase);

            int leftGutterNormal = GetEffectiveGutter(PageSide.Left, isOddPage, GutterHandling.AddSideGutterForDoubleGutterCase);

            int contentLeft = leftMargin + leftGutterNormal;
            int actualContentWidth = pageWidth - (leftMargin + leftGutter + rightMargin + rightGutter);
            int contentRight = contentLeft + actualContentWidth;

            return new Point(contentLeft, contentRight);
        }

        /// <summary>
        /// Gets the effective gutter for the given page side and gutter behavior.
        /// </summary>
        private int GetEffectiveGutter(PageSide side, bool isOddPage, GutterHandling gutterBehavior)
        {
            bool addGutter;
            bool isSideGutterMirrored;
            PageSide gutterPosition;
            switch (MultiplePages)
            {
                case MultiplePagesType.Normal:
                case MultiplePagesType.MirrorMargins:
                    bool pageHasTopBottomGutter = OrientationMatchesFirstSection
                        ? IsGutterAtTopSet
                        : !IsGutterAtTopSet;

                    if (pageHasTopBottomGutter)
                    {
                        bool gutterOnTop = OrientationMatchesFirstSection || (Orientation == Orientation.Landscape);

                        gutterPosition = gutterOnTop
                            ? PageSide.Top
                            : PageSide.Bottom;

                        addGutter = (side == gutterPosition);
                    }
                    else
                    {
                        isSideGutterMirrored = DifferentOddEvenPageHeader || (MultiplePages == MultiplePagesType.MirrorMargins);

                        // This is for left-to-right documents (left gutter is the default).
                        bool isLeftGutter = isOddPage || !isSideGutterMirrored;

                        if (IsRtlGutter)
                        {
                            // Everything is vice versa for Right gutter, which is normally set for RTL sections.
                            isLeftGutter = !isLeftGutter;
                        }

                        gutterPosition = isLeftGutter
                            ? PageSide.Left
                            : PageSide.Right;

                        addGutter = (side == gutterPosition);

                    }
                    break;
                case MultiplePagesType.TwoPagesPerSheet:
                case MultiplePagesType.BookFoldPrinting:
                case MultiplePagesType.BookFoldPrintingReverse:
                    if (Orientation == Orientation.Portrait)
                    {
                        // Gutter is on the top and it is mirrored on even pages.
                        gutterPosition = isOddPage
                            ? PageSide.Top
                            : PageSide.Bottom;

                        addGutter = (side == gutterPosition);
                    }
                    else
                    {
                        // Side gutter mirrors the same way for two pages per sheet and book fold.
                        // It is the other way around for reverse book fold.
                        isSideGutterMirrored = (MultiplePages != MultiplePagesType.BookFoldPrintingReverse)
                            ? isOddPage
                            : !isOddPage;

                        gutterPosition = isSideGutterMirrored
                            ? PageSide.Left
                            : PageSide.Right;

                        // Below is an implementation of a special MS Word behavior which looks like a glitch,
                        // The gutter is placed both on top and on the side.
                        bool secondGutterAtTop = (MultiplePages == MultiplePagesType.TwoPagesPerSheet)
                            ? OrientationMatchesFirstSection && IsGutterAtTopSet
                            : !OrientationMatchesFirstSection;
                        // It is strange that gutterAtTop only affects 2 pages per sheet setup, but it seems MS Word works this way.

                        // For column width calculation in presence of two gutters side gutter is disregarded.
                        bool isSideGutterPresent = !(secondGutterAtTop && (gutterBehavior == GutterHandling.DisregardSideGutterForDoubleGutterCase));
                        addGutter = secondGutterAtTop && (side == PageSide.Top) ||
                            isSideGutterPresent && (side == gutterPosition);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unexpected multiple page setup value.");
            }

            return addGutter ? Gutter : 0;
        }

        private int GetTopMargin(bool isOddPage)
        {
            // When top-bottom is mirrored, "outside" (bottom) margin is used for odd pages.
            bool mirrorTop = IsTopBottomMarginMirrored && isOddPage;
            int topMargin = mirrorTop
                ? BottomMargin
                : TopMargin;

            return topMargin;
        }

        private int GetBottomMargin(bool isOddPage)
        {
            // When top-bottom is mirrored, "outside", that is, bottom margin is used for odd pages.
            bool mirrorTop = IsTopBottomMarginMirrored && isOddPage;
            int bottomMargin = mirrorTop
                ? TopMargin
                : BottomMargin;

            return bottomMargin;
        }

        private int GetLeftMargin(bool isOddPage)
        {
            bool mirrorSide = IsSideMarginMirrored(isOddPage);
            int leftMargin = mirrorSide
                ? RightMargin
                : LeftMargin;

            return leftMargin;
        }

        private int GetRightMargin(bool isOddPage)
        {
            // When left-right is mirrored, left margin goes to the right for even pages.
            bool mirrorSide = IsSideMarginMirrored(isOddPage);
            int rightMargin = mirrorSide
                ? LeftMargin
                : RightMargin;

            return rightMargin;
        }

        private bool IsTopBottomMarginMirrored
        {
            get
            {
                bool isMirrored;
                switch (MultiplePages)
                {
                    case MultiplePagesType.Normal:
                    case MultiplePagesType.MirrorMargins:
                        isMirrored = false;
                        break;
                    case MultiplePagesType.TwoPagesPerSheet:
                    case MultiplePagesType.BookFoldPrinting:
                    case MultiplePagesType.BookFoldPrintingReverse:
                        // Top-bottom margins are mirrored in portrait.
                        // It means, the page that holds two sheets is portrait, but each sheet may be a landscape.
                        isMirrored = (Orientation == Orientation.Portrait);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected multiple page setup value.");
                }

                return isMirrored;
            }
        }

        private bool IsSideMarginMirrored(bool isOddPage)
        {
            bool isMirrored;
            switch (MultiplePages)
            {
                case MultiplePagesType.Normal:
                    isMirrored = false;
                    break;
                case MultiplePagesType.MirrorMargins:
                    // Left margin goes to the right for even pages.
                    isMirrored = OrientationMatchesFirstSection && !isOddPage;
                    break;
                case MultiplePagesType.TwoPagesPerSheet:
                case MultiplePagesType.BookFoldPrinting:
                    // Side margins are mirrored for landscape orientation on odd pages.
                    isMirrored = (Orientation == Orientation.Landscape) && isOddPage;
                    break;
                case MultiplePagesType.BookFoldPrintingReverse:
                    // Side margins are mirrored for landscape orientation on even pages.
                    isMirrored = (Orientation == Orientation.Landscape) && !isOddPage;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected multiple page setup value.");
            }

            return isMirrored;
        }

        /// <summary>
        /// Describes different gutter handling methods.
        /// </summary>
        private enum GutterHandling
        {
            SideGutterNormal,
            AddSideGutterForDoubleGutterCase = SideGutterNormal,
            DisregardSideGutterForDoubleGutterCase,
        }

        private readonly int LeftMargin;
        private readonly int RightMargin;
        private readonly int TopMargin;
        private readonly int BottomMargin;

        private readonly int Gutter;
        private readonly bool IsGutterAtTopSet;
        private readonly bool IsRtlGutter;

        private readonly MultiplePagesType MultiplePages;
        private readonly Orientation Orientation;
        private readonly bool OrientationMatchesFirstSection;

        private readonly bool DifferentOddEvenPageHeader;
    }
}
