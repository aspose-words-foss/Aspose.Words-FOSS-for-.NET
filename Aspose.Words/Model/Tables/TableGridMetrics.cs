// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Dmitry Matveenko

using System;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents metrics used for table cells and grid columns during grid construction.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class TableGridMetrics
    {
        /// <summary>
        /// Makes a new instance with zero metrics.
        /// </summary>
        internal TableGridMetrics() { }

        /// <summary>
        /// Makes a new TableGridMetrics object initializing it with the values from the given object.
        /// </summary>
        internal TableGridMetrics(TableGridMetrics metrics)
        {
            Minimum = metrics.Minimum;
            ContentMinimum = metrics.ContentMinimum;
            ContentMaximum = metrics.ContentMaximum;
        }

        /// <summary>
        /// Makes a new TableGridMetrics object with the given values.
        /// </summary>
        internal TableGridMetrics(int minimum, int contentMinimum, int contentMaximum)
        {
            Minimum = minimum;
            ContentMinimum = contentMinimum;
            ContentMaximum = contentMaximum;
        }

        internal void Update(CellSpan cell)
        {
            TableGridMetrics cellMetrics = cell.Metrics;
            Minimum = System.Math.Max(Minimum, cellMetrics.Minimum);
            ContentMinimum = System.Math.Max(ContentMinimum, cellMetrics.ContentMinimum);

            // Disregard content maximums for cells with preferred widths in twips.
            // Content maximums for auto cells may be disregarded as well. It is handled by special logic during grid construction.
            // For now, only cpt cells maximum is used unconditionally.
            int cellMaximum = cell.PreferredWidth.IsPercent
                ? cellMetrics.ContentMaximum
                : cellMetrics.ContentMinimum;

            ContentMaximum = System.Math.Max(ContentMaximum, cellMaximum);
            ContentMaximum = System.Math.Max(ContentMaximum, ContentMinimum);
        }

        /// <summary>
        /// Minimal column width, in twips.
        /// </summary>
        /// <remarks>
        /// A column corresponding to a table cell cannot be narrower than cell margins or half of side borders whatever, is greater.
        /// MS Word also seems to add 1 point for the contents in fixed layout always.
        /// In case of auto-fit tables this metric is used to squeeze the table when content minimums do not fit into container.
        /// </remarks>
        internal int Minimum
        {
            get { return mMinimum; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                mMinimum = value;
            }
        }

        /// <summary>
        /// Minimum content width.
        /// </summary>
        /// <remarks>
        /// It is the width of the column required to accommodate longest word of the contents.
        /// So if the column is greater or equal to this, no words have to be broken to fit into the column.
        /// For a grid column, the maximum of content minimums among all cells in this column is stored.
        /// It is greater or equal to Minimum above.
        /// It is used for auto-fit tables only.
        /// </remarks>
        internal int ContentMinimum
        {
            get { return mContentMinimum; }
            set
            {
                mContentMinimum = value;
                if (ContentMaximum < ContentMinimum)
                    ContentMaximum = ContentMinimum;
            }
        }

        /// <summary>
        /// Maximum content width.
        /// </summary>
        /// <remarks>
        /// It is the width to accommodate longest line without wrapping.
        /// So only explicit line/paragraph breaks etc cause wrapping.
        /// For a grid column, a maximum of content maximums among all cells in this column is stored.
        /// It is used for auto-fit tables only.
        /// </remarks>
        internal int ContentMaximum
        {
            get { return mContentMaximum; }
            set { mContentMaximum = value; }
        }

        /// <summary>
        /// Ensures that the metrics do not exceed MS Word upper boundaries.
        /// </summary>
        internal void EnsureUpperLimit()
        {
            Minimum = System.Math.Min(Minimum, MaxWidthTwips);
            ContentMinimum = System.Math.Min(ContentMinimum, MaxWidthTwips);
            ContentMaximum = System.Math.Min(ContentMaximum, MaxWidthTwips);
        }

        internal const int HundredPercent = 5000;

        /// <summary>
        /// Minimal grid column width used during table grid construction.
        /// </summary>
        internal const int MinimalWidthTwips = 5;

        /// <summary>
        /// It seems, for auto-fit layout MS Word uses 6 twips as a minimal cell contents width metrics for empty cells.
        /// </summary>
        internal const int MinimumContentWidthAuto = 6;
        // The constant may be the linked to MinimalWidthTwips. Keep them separate for now.

        internal const int MinimalFixedLayoutCellContentWidthTwips = 20;
        internal const int DefaultFixedColumnWidthTwips = 360;
        internal const int MaxWidthTwips = 31680;
        /// <summary>
        /// It is used to indicate that spacing is disabled when spacing value is passed via int.
        /// </summary>
        /// <remarks>
        /// "spacing disabled" and "spacing enabled, but 0" are different settings.
        /// With 0 spacing, table borders and cell borders are both rendered. Also cells do not share borders.
        /// </remarks>
        internal const int SpacingDisabled = -1;

        private int mMinimum;
        private int mContentMaximum;
        private int mContentMinimum;
    }
}
