// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2013 by Dmitry Matveenko

using System;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents a table grid column during grid construction.
    /// </summary>
    internal class TableGridColumn
    {
        /// <summary>
        /// Gets a new instance with zero metrics.
        /// </summary>
        internal TableGridColumn()
        {
            Metrics = new TableGridMetrics();

            // Grid column is initialized as not matching a cell or cell end.
            // The property will be updated during grid construction.
            SingleCellMatch = new GridCellMatch();
        }

        /// <summary>
        /// Groups various min/max values calculated from cell and content properties.
        /// </summary>
        internal TableGridMetrics Metrics { get; }

        /// <summary>
        /// Gets a boolean value indicating if a column can be removed from the grid.
        /// </summary>
        /// <remarks>
        /// There is some tricky logic that decides if a narrow intersection column can be removed.
        /// Intersection column does not match any table cell, it is the result of overlapping of cells spanning several columns.
        /// MS Word normally keeps such intersections if they are wider than 5 twips.
        /// With "Use Word 2003 rules" compatibility option, there is a rather weird condition when
        /// narrow intersection columns having width 2 to 5 twips are not removed.
        /// </remarks>
        internal bool IsRemovable(bool keepNarrowIntersection, bool checkResizedWidth, int halfSpacing)
        {
            int width = checkResizedWidth
                ? Width
                : Twips;

            if ((width > 0) && keepNarrowIntersection)
                return false;

            // TestMinSplitPctASpacing : with spacing 2, the column of 0 width is still removed.
            // TODO Need to investigate how it works with different spacing on different rows.
            if ((width + halfSpacing * 2) > TableGridMetrics.MinimalWidthTwips)
                return false;

            // Do not remove columns having minimums greater than the default.
            // It is the result of a wider cell minimum distribution, so such columns must be preserved.
            if (Metrics.Minimum > TableGridMetrics.MinimalWidthTwips)
                return false;

            // Before resizing, do not remove columns with content maximum greater than minimum value or preferred pct specified.
            // It may be an intersection column where the value is the result of a wide cell handling.
            // The values may affects resizing.
            if (!checkResizedWidth)
                if ((Percent > 0) || (ContentMaximum > TableGridMetrics.MinimalWidthTwips))
                    return false;
            // After resizing, disregard content maximums or pct preferred width.
            // It should only affect intersection columns. They are removed or kept depending on the resized width.

            // Twip cell match type is now set for wide cells as well.
            bool isRemovable = !SingleCellMatch.IsSet(GridCellMatchType.Percent);
            return isRemovable;
        }

        /// <summary>
        /// Gets a new span initialized as 1 column with default width.
        /// </summary>
        internal static TableGridColumn GetNewDefaultFixedWidthColumn()
        {
            TableGridColumn newColumn = new TableGridColumn();
            newColumn.Twips = TableGridMetrics.DefaultFixedColumnWidthTwips;
            return newColumn;
        }

        /// <summary>
        /// Calculated column width, in twips.
        /// </summary>
        internal int Width
        {
            get { return mWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                mWidth = value;
            }
        }

        /// <summary>
        /// Preferred column width specified in twips.
        /// </summary>
        internal int Twips
        {
            get { return mTwips; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                mTwips = value;
            }
        }

        /// <summary>
        /// Indicates if a preferred value in absolute units (twips) was specified for the column.
        /// </summary>
        /// <remarks>
        /// This includes preferred width values remaining it the last column of a wide cell preferred width
        /// after deducting the preferred widths in the previous columns.
        /// </remarks>
        internal bool HasPreferredTwips
        {
            get
            {
                // With the currently implemented approach,
                // zero preferred width remainder in the last column of a wide cell apparently does not affect the logic.
                // Just check the assigned twip value.
                return (Twips > 0);
            }
        }

        /// <summary>
        /// Preferred column width specified in 1/50th of 1%.
        /// </summary>
        internal int Percent
        {
            get { return mPercent; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                mPercent = value;

                HasPreferredPercent = true;
            }
        }

        /// <summary>
        /// A boolean value indicating if a preferred width in pct units was set on the column.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="HasPreferredTwips"/>, the property also returns true after a zero value is set.
        /// It helps to emulate the logic when columns matching the end column of a wide cell with preferred width in pct units
        /// are treated as having a pct preferred width (even though no positive pct width can be assigned).
        /// </remarks>
        internal bool HasPreferredPercent { get; private set; }

        /// <summary>
        /// Indicates which types of single-column cell preferred width the column matches.
        /// </summary>
        internal GridCellMatch SingleCellMatch { get; }

        internal bool IsZeroWidthIntersection
        {
            get
            {
                return !HasPreferredPercent && SingleCellMatch.IsNone;
            }
        }

        /// <summary>
        /// A boolean value indicating if no preferred width was set on the column.
        /// </summary>
        internal bool IsAuto
        {
            get
            {
                return !(HasPreferredTwips || HasPreferredPercent);
            }
        }

        internal int TwipsOrMinimum
        {
            get { return System.Math.Max(Twips, Metrics.Minimum); }
        }

        internal int TwipsOrContentMinimum
        {
            get { return System.Math.Max(TwipsOrMinimum, Metrics.ContentMinimum); }
        }

        internal int TwipsOrContentMaximum
        {
            get { return System.Math.Max(TwipsOrMinimum, Metrics.ContentMaximum); }
        }

        /// <summary>
        /// A shortcut to get minimum from metrics.
        /// </summary>
        internal int Minimum
        {
            get { return Metrics.Minimum; }
        }

        /// <summary>
        /// A shortcut to get content minimum from metrics, used a lot.
        /// </summary>
        internal int ContentMinimum
        {
            get { return Metrics.ContentMinimum; }
        }

        /// <summary>
        /// A shortcut to get content maximum from metrics.
        /// </summary>
        internal int ContentMaximum
        {
            get { return Metrics.ContentMaximum; }
        }

        private int mPercent;
        private int mTwips;
        private int mWidth;
    }
}
