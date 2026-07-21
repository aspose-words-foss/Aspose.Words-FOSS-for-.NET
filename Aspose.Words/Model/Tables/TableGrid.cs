// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2013 by Dmitry Matveenko

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aspose.Collections;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents table grid during grid calculation.
    /// </summary>
    internal class TableGrid
    {
        /// <summary>
        /// Creates a new empty grid instance with no columns.
        /// </summary>
        internal TableGrid(bool isAutoFit)
        {
            // No columns in a new grid.
            Columns = new List<TableGridColumn>();
            IsAutoFit = isAutoFit;
        }

        internal bool IsAutoFit { get; }

        private List<TableGridColumn> Columns { get; }

        internal int Count
        {
            get { return Columns.Count; }
        }

        internal TableGridColumn this[int index]
        {
            get { return Columns[index]; }
            set { Columns[index] = value; }
        }

        internal void Add(TableGridColumn span)
        {
            Debug.Assert(span != null);
            Columns.Add(span);
        }

        internal int GetColumnMinimum(int columnIdx)
        {
            TableGridColumn column = Columns[columnIdx];
            return IsAutoFit
                ? column.ContentMinimum
                : column.Minimum;
        }

        internal int GetColumnWidth(int columnIdx)
        {
            TableGridColumn column = Columns[columnIdx];
            return IsAutoFit
                ? column.TwipsOrContentMaximum
                : column.TwipsOrMinimum;
        }

        /// <summary>
        /// Gets sum of the widths of the columns in the specified range.
        /// </summary>
        /// <param name="firstColumn">First column in the range.</param>
        /// <param name="columnCount">Number of columns in the range. Can be 0.</param>
        /// <remarks>
        /// The loop inside may be eliminated by calculating an array of running sums beforehand.
        /// However, most ranges are just 1, for single-column cells, so I don't think the complication is justified.
        /// </remarks>
        internal int GetRangeWidth(int firstColumn, int columnCount)
        {
            CheckRangeArguments(firstColumn, columnCount);

            int rangeWidth = 0;
            int endOfRange = firstColumn + columnCount;
            for (int columnIndex = firstColumn; columnIndex < endOfRange; columnIndex++)
            {
                TableGridColumn column = this[columnIndex];
                rangeWidth += column.Width;
            }

            return rangeWidth;
        }

        private void CheckRangeArguments(int firstColumn, int columnCount)
        {
            if (!IsValidIndex(firstColumn))
                throw new ArgumentOutOfRangeException("firstColumn");

            if (!IsValidRange(firstColumn, columnCount))
                throw new ArgumentOutOfRangeException("columnCount");
        }

        /// <summary>
        /// Calculates the sums of grid column minimums.
        /// </summary>
        internal TableGridMetrics GetMetricsForTable()
        {
            int minimum = 0;
            int contentMinimum = 0;
            int contentMaximum = 0;

            for (int i = 0; i < Count; ++i)
            {
                TableGridColumn column = this[i];

                minimum += column.Minimum;
                int columnContentMinimum = System.Math.Max(column.Minimum, column.ContentMinimum);
                contentMinimum += columnContentMinimum;

                int columnMaximum = column.HasPreferredTwips
                                        ? column.Twips
                                        : column.ContentMaximum;

                columnMaximum = System.Math.Max(columnMaximum, columnContentMinimum);
                contentMaximum += columnMaximum;
            }

            return new TableGridMetrics(minimum, contentMinimum, contentMaximum);
        }

        private bool IsValidIndex(int index)
        {
            return (0 <= index) && (index < Count);
        }

        private bool IsValidRange(int firstColumn, int columnCount)
        {
            return IsValidIndex(firstColumn) &&
                   (0 < columnCount) &&
                   (firstColumn + columnCount <= Count);
        }

        internal void RemoveColumn(int columnIdx)
        {
            Columns.RemoveAt(columnIdx);
        }

        internal void RemoveAllColumns()
        {
            Columns.Clear();
        }

        internal IntList ToIntList()
        {
            IntList gridColumns = new IntList(Count);
            for (int i = 0; i < Count; ++i)
                gridColumns.Add(this[i].Width);

            return gridColumns;
        }

        [Conditional("DEBUG")]
        internal void DebugWrite(string context)
        {
            TableGridDebugLogger.DebugWriteGrid(context, this);
        }
    }
}
