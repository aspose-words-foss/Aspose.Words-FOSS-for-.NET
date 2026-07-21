// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/11/2014 by Dmitry Matveenko

using System;
using System.Text;
using Aspose.Words.TableLayout;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents cell for table grid calculation.
    /// </summary>
    /// <remarks>
    /// The class represents all table cell attributes needed for grid construction.
    /// It also allows to update them independently of the original cell attributes (span count may be updated).
    /// If during grid construction it is eventually discovered that construction failed
    /// and the grid cannot be applied, the original cell attributes remain unchanged.
    /// This allows to fall back to the "old" grid approach.
    /// </remarks>
    internal class CellSpan
    {
        /// <summary>
        /// Creates a new instance of the class that represents the given cell.
        /// </summary>
        internal CellSpan(Cell cell)
        {
            if (cell == null)
                throw new ArgumentNullException("cell");

            Debug.Assert(cell.CellPr != null);

            mMetrics =  new TableGridMetrics();

            // The property may be overridden for the first row,
            // or for the first-in-block row when a table with more than 64 columns is split into blocks.
            IsVerticallyMerged = (cell.CellPr.VerticalMerge == CellMerge.Previous);
            // Vertical merges in the model can be inconsistent.
            // In Test18261, all cells in a column are marked as merged.
            // Currently, tables with inconsistent vertical merges are "fixed" during validation in the model.
            // The logic does not match MS Word, and grid state is set to CalculationFailed for such cases.

            mMetrics.Minimum = GetMinimalWidth(cell);
            GridSpan = cell.CellPr.GridSpan;
            // For vertically merged cells, the grid span in the cell is still used.
            // MS Word actually allows contradicting grid spans in vertically merged cells.
            // There are such cells in Test22898 (in a rather complex table with almost 200 columns).

            // There is a manually created test, TestVMergeA() which currently works correctly
            // because a grid column created for the contradicting grid span is removed.

            bool adjustZeroTwips = (GridSpan == 1);
            mPreferred = EnsureInt16Value(cell.CellPr.PreferredWidth, adjustZeroTwips);
        }

        internal CellSpan(int gridSpan, PreferredWidth preferredWidth, TableGridMetrics metrics) : this(gridSpan, preferredWidth)
        {
            mMetrics = new TableGridMetrics(metrics);
        }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <remarks>
        /// This constructor is used to create a CellSpan that represents grid before or grid after for a row.
        /// It is convenient to represent them via CellSpan because they are often treated similar to cells during grid construction.
        /// </remarks>
        internal CellSpan(int gridSpan, PreferredWidth preferredWidth)
        {
            if (preferredWidth == null)
            {
                GridSpan = gridSpan;
                mPreferred = NoWidth;
                return;
            }

            bool adjustZeroTwips = (gridSpan == 1);
            mPreferred = EnsureInt16Value(preferredWidth, adjustZeroTwips);

            GridSpan = gridSpan;
            if ((preferredWidth.ValueRaw > 0) && (gridSpan <= 0))
            {
                TableGridDebugLogger.DebugWriteLine("Span count set to 1 for non-zero width before/after.");
                GridSpan = 1;
            }
        }

        private static PreferredWidth EnsureInt16Value(PreferredWidth preferred, bool adjustZeroTwips)
        {
            int rawValue = preferred.ValueRaw;
            int castValue = (short)rawValue;

            if (rawValue != castValue)
            {
                TableGridDebugLogger.DebugWriteLine("Raw preferred width not fitting into int16 cast to int16.");
                // TestTcwMax shows that values above 31680 (TableGridMetrics.MaxWidthTwips) but below int16.MaxValue should remain as is.
            }

            int adjustedValue = castValue;
            if (adjustedValue < 0)
            {
                TableGridDebugLogger.DebugWriteLine("Negative raw preferred width set to 0.");
                adjustedValue = 0;
            }

            if (adjustZeroTwips && preferred.IsFixed && adjustedValue == 0)
            {
                // A non-standard case when 0 width value is stored for dxa preferred width.
                // This should not normally happen, but sometimes it does, and the combination even survives re-saving via MS Word.
                // See TestTcw0Dxa*. There are other tests as well.
                // Set minimal twip value so that the cell is treated as having a preferred width assigned.
                adjustedValue = 1;
                // This adjusted value will not be applied for width before/after.
                // If the original width is 0, it will be removed. See ApplyGridBeforeAfter().
            }

            return (adjustedValue == rawValue)
                ? preferred
                : PreferredWidth.FromRawSafe(preferred.Type, adjustedValue);
        }

        // Generated by Resharper.
        // It was needed for TestDocCompareIssue.TestIssue20 to work.
        public bool Equals(CellSpan other)
        {
            return object.Equals(mPreferred, other.mPreferred) &&
                   mGridSpan == other.mGridSpan &&
                   mMetrics.Equals(other.mMetrics);
        }

        // Generated by Resharper.
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CellSpan)obj);
        }

        // Generated by Resharper together with Equals().
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (mPreferred != null ? mPreferred.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ mGridSpan;
                hashCode = (hashCode * 397) ^ mMetrics.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Cell grid span as specified by the cell properties in the model.
        /// </summary>
        /// <remarks>
        /// Horizontally merged cells are accounted for in the grid span of the first merged cell.
        /// </remarks>
        internal int GridSpan
        {
            get { return mGridSpan; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                mGridSpan = value;
            }
        }

        /// <summary>
        /// Preferred width as specified by cell properties.
        /// </summary>
        /// <remarks>
        /// It seems that for the fixed table layout, MS Word replaces Auto preferred width
        /// with the width taken from the table grid (if present) or with the default cell width.
        /// It is currently handled during grid construction.
        /// Cell preferred width in the model if updated together with the cell width if the new table grid is applied.
        /// </remarks>
        internal PreferredWidth PreferredWidth
        {
            get { return mPreferred; }
            set { mPreferred = value; }
        }

        internal int PreferredTwips
        {
            get
            {
                return PreferredWidth.IsFixed
                    ? PreferredWidth.ValueTwips
                    : 0;
            }
        }

        internal bool HasNoWidth
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return (PreferredWidth.Type == NoWidth.Type) && (PreferredWidth.ValueRaw == NoWidth.ValueRaw); }
        }

        /// <summary>
        /// Gets minimal cell width in twips.
        /// </summary>
        /// <remarks>
        /// Cell cannot be narrower than cell margins (or half of side border width, which is inside cell).
        /// Also, MS Word seems to add at least 1 point (20 twips) for the contents in fixed table layout.
        /// </remarks>
        internal int Minimum
        {
            get { return mMetrics.Minimum; }
        }

        internal int ContentMinimum
        {
            get { return mMetrics.ContentMinimum; }
        }

        internal int ContentMaximum
        {
            get { return mMetrics.ContentMaximum; }
        }

        internal TableGridMetrics Metrics
        {
            get { return mMetrics; }
        }

        /// <summary>
        /// Indicates if the cell is vertically merged.
        /// </summary>
        /// <remarks>
        /// For tables with more than 64 columns, the property may be reset to false,
        /// when the table is split into row blocks.
        /// In that case v-merged cells are treated as regular cells, and their preferred width affects the grid.
        /// </remarks>
        internal bool IsVerticallyMerged { get; set; }

        /// <summary>
        /// Indicates if "Wrap Text" option is disabled for the cell.
        /// </summary>
        /// <remarks>
        /// By default, "Wrap Text" is enabled in MS Word.
        /// </remarks>
        internal bool IsWrapTextDisabled { get; set; }

        private static int GetMinimalWidth(Cell cell)
        {
            int bordersAndPadding = Extensions.GetCellBordersAndPaddingTwips(cell);

            return TableGridMetrics.MinimalFixedLayoutCellContentWidthTwips + bordersAndPadding;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0,2} ", GridSpan);
            if (HasNoWidth)
                sb.Append("no width");
            else
            {
                sb.AppendFormat("width {0,4}", PreferredWidth.ValueRaw);
                if (PreferredWidth.Type == PreferredWidthType.Percent)
                    sb.Append(" percent");
            }
            return sb.ToString();
        }

        /// <summary>
        /// A special value meaning that no width was specified for a cell span.
        /// </summary>
        /// <remarks>
        /// It is used for widthBefore and widthAfter.
        /// Handling of 0 values is different from handling of not specified values for these attributes.
        /// </remarks>
        internal static readonly PreferredWidth NoWidth = PreferredWidth.FromRawSafe(PreferredWidthType.Auto, -1);

        private PreferredWidth mPreferred;
        private int mGridSpan;
        private readonly TableGridMetrics mMetrics;
    }
}
