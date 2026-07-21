// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A margin of a box in the CSS box model.
    /// </summary>
    /// <remarks>
    /// This class is immutable (read-only).
    /// </remarks>
    internal class CssBoxMargin
    {
        private CssBoxMargin(
            CssLength length,
            CssBoxMarginCollapsing collapsingWithParentMargin,
            bool zeroDefaultCollapsedMargins)
        {
            mLength = length;
            mCollapsingWithParentMargin = collapsingWithParentMargin;
            mZeroDefaultCollapsedMargins = zeroDefaultCollapsedMargins;
        }

        internal static CssBoxMargin Create(
            CssLength length,
            CssBoxMarginCollapsing collapsingWithParentMargin,
            bool zeroDefaultCollapsedMargins)
        {
            if ((length == CssLength.ZeroDefault) && !zeroDefaultCollapsedMargins)
            {
                // Memory optimization: use instanced objects to reduce memory allocations.
                switch (collapsingWithParentMargin)
                {
                    case CssBoxMarginCollapsing.Add:
                        return gZeroAdd;
                    case CssBoxMarginCollapsing.Collapse:
                        return gZeroCollapse;
                    case CssBoxMarginCollapsing.Separate:
                        return gZeroSeparate;
                    default:
                        Debug.Assert(false);
                        return gZeroSeparate;
                }
            }

            return new CssBoxMargin(length, collapsingWithParentMargin, zeroDefaultCollapsedMargins);
        }

        /// <summary>
        /// Margin size.
        /// </summary>
        internal CssLength Length
        {
            get { return mLength; }
        }

        /// <summary>
        /// Defines how this margin is collapsed with the margin of the parent box.
        /// </summary>
        internal CssBoxMarginCollapsing CollapsingWithParentMargin
        {
            get { return mCollapsingWithParentMargin; }
        }

        /// <summary>
        /// Indicates whether all default margins are zeroed when collapsed with this margin.
        /// </summary>
        internal bool ZeroDefaultCollapsedMargins
        {
            get { return mZeroDefaultCollapsedMargins; }
        }

        private readonly CssLength mLength;

        private readonly CssBoxMarginCollapsing mCollapsingWithParentMargin;

        private readonly bool mZeroDefaultCollapsedMargins;

        private static readonly CssBoxMargin gZeroAdd = new CssBoxMargin(CssLength.ZeroDefault, CssBoxMarginCollapsing.Add, false);
        private static readonly CssBoxMargin gZeroCollapse = new CssBoxMargin(CssLength.ZeroDefault, CssBoxMarginCollapsing.Collapse, false);
        private static readonly CssBoxMargin gZeroSeparate = new CssBoxMargin(CssLength.ZeroDefault, CssBoxMarginCollapsing.Separate, false);
    }
}
