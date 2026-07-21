// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/10/2021 by Dmitry Matveenko

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Implements the logic for table resizing to table preferred width.
    /// </summary>
    internal class TableGridResizer
    {
        internal TableGridResizer(TableGrid grid, bool useWord2003Rules,
            int maxAllowedWidthFromContainerRaw, int maxWidthAdjustment,
            PreferredWidth tblwRaw, int tableLeftIndentTwips,
            int halfSpacing)
        {
            Grid = grid;

            CellSpacing = halfSpacing * 2;

            // This is used for all purposes, except computing tblw in percent units.
            // There is a quirk with table indent there.
            MaxAllowedWidthFromContainer = System.Math.Max(0, maxAllowedWidthFromContainerRaw - TotalGridSpacing);
            TableGridDebugLogger.DebugWriteLine(string.Format("Max allowed table width for resizing: {0}", MaxAllowedWidthFromContainer));

            MaxWidthAdjustment = maxWidthAdjustment;

            PreferredWidth tblw = tblwRaw.GetCorrespondingValidTblwValue();

            // Get auto table width from percent columns.
            int autoTableWidth = (tblw.IsAuto)
                ? GetAutoTableWidth(Grid)
                : 0;
            // Auto table width should be calculated before percent columns are truncated by 100%: TestTblw0PctH.

            // Calculate totals and update percent columns that go beyond 100%.
            // Also set width and minimums for resizing where applicable.
            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];

                HardMinimumSum += column.Minimum;

                int columnMinimum = Grid.GetColumnMinimum(i);
                int columnWidth = Grid.GetColumnWidth(i);

                // TestTcwMax shows that values up to int16.MaxValue are OK to use.
                if (columnWidth > short.MaxValue)
                {
                    TableGridDebugLogger.DebugWriteLine("Unexpected calculated column width value.");
                    IsUnsupportedCaseDetected = true;
                }

                // This is needed for all methods below.
                if (column.Width < columnMinimum)
                    column.Width = columnMinimum;

                if (!column.HasPreferredPercent)
                {
                    column.Width = columnWidth;

                    bool treatAsAuto = Grid.IsAutoFit
                        ? column.IsAuto
                        : useWord2003Rules && column.SingleCellMatch.IsNone;
                    // In fixed layout 2003 mode, "intersection" columns not matching any single cell (and not having percent width)
                    // behave similar to columns matching Auto cells in auto-fit layout.

                    if (treatAsAuto)
                    {
                        AutoMinimumSum += columnMinimum;
                        AutoWidthSum += columnWidth;
                    }
                    else
                    {
                        TwipMinimumSum += columnMinimum;
                        TwipWidthSum += columnWidth;
                    }
                }
                else
                {
                    PctMinimumSum += columnMinimum;
                    // Though in most cases preferred width in twips set for a pct column is ignored,
                    // it may still be used when all pct preferred widths are zero. TestTcwMixedE2.
                    PctTwipWidthSum += columnWidth;

                    if (PctPreferredSum + column.Percent <= TableGridMetrics.HundredPercent)
                    {
                        PctPreferredSum += column.Percent;
                    }
                    else if (PctPreferredSum < TableGridMetrics.HundredPercent)
                    {
                        // Set the remaining pct width up to 100%.
                        column.Percent = TableGridMetrics.HundredPercent - PctPreferredSum;
                        PctPreferredSum = TableGridMetrics.HundredPercent;
                    }
                    else
                    {
                        // Set the column to minimum and percent to 0, it will not get anything from pct preferred.
                        column.Percent = 0;
                        column.Width = columnMinimum;
                    }
                }
            }

            Debug.Assert(PctPreferredSum <= TableGridMetrics.HundredPercent);
            Grid.DebugWrite("After initializing twips and percent values for resizing:");

            ResolvePreferredTableWidth(tblw, autoTableWidth, maxAllowedWidthFromContainerRaw, tableLeftIndentTwips);
            SetWidthSharesForColumnTypes();
        }

        /// <summary>
        /// Calculates auto table width from column preferred widths, including column preferred widths in pct units.
        /// </summary>
        /// <remarks>
        /// The method originally traversed all rows/cells (single-span cells, actually).
        /// But eventually it turned out that auto table width can be calculated from grid columns alone.
        /// However, it must be done before column preferred pct values are truncated by 100%.
        /// The original implementation is at git 7268038a1f2d28b150fb3c9eff828dba4d57d983, tagged dmatv_before_autoTblwFromPercent_rework.
        /// The method was called GetAutoTableWidthFromPercentCells().
        /// </remarks>
        internal static int GetAutoTableWidth(TableGrid grid)
        {
            int autoTableWidthFromPct = 0;
            int pctPreferredSum = 0;
            int pctTwipWidthSum = 0;
            int nonPctWidthSum = 0;

            // After wide cell minimum distribution grid columns may have minimums different from any cell in the column.
            // No cells traverse is needed to calculate auto table width, it can be done from grid columns only.
            for (int col = 0; col < grid.Count; ++col)
            {
                TableGridColumn column = grid[col];

                int columnMinimum = grid.GetColumnMinimum(col);
                int columnWidth = grid.GetColumnWidth(col);

                if (columnWidth < columnMinimum)
                    columnWidth = columnMinimum;

                // Calculating grid sums somewhat parallels resizer initialization
                // (but resizer initialization also updates percents and minimums in the grid).
                // This method is used for calculating nested table metrics before resizing.
                if (column.HasPreferredPercent)
                {
                    pctPreferredSum += column.Percent;
                    pctTwipWidthSum += columnWidth;
                    if (column.Percent > 0)
                    {
                        int autoTableWidthFromGridColumn = MathUtil.Divide(column.TwipsOrContentMaximum * TableGridMetrics.HundredPercent, column.Percent);

                        if (autoTableWidthFromGridColumn > autoTableWidthFromPct)
                            autoTableWidthFromPct = autoTableWidthFromGridColumn;
                    }
                }
                else
                    nonPctWidthSum += columnWidth;
            }

            pctPreferredSum = System.Math.Min(pctPreferredSum, TableGridMetrics.HundredPercent);
            TableGridDebugLogger.DebugWriteLine(string.Format("Table width calculated from pct cells: {0}", autoTableWidthFromPct));

            int autoTableWidth = ResolveAutoTableWidth(autoTableWidthFromPct, pctPreferredSum, pctTwipWidthSum, nonPctWidthSum);

            return autoTableWidth;
        }

        /// <summary>
        /// Computes the target resizing values for different column types.
        /// </summary>
        private void SetWidthSharesForColumnTypes()
        {
            if (TargetTableWidth > TableGridMetrics.MaxWidthTwips)
            {
                // TestTblw0Max shows that something similar to pct preferred handling exceeding 5000 pct units should happen in this case:
                // first columns are allowed to have the specified preferred widths,
                // and the columns are truncated or set to min after the sum exceeds magic 31660 (and not 31680).
                // Also the columns after the limit seem to be assigned a lower minimums.
                // The exact logic was not investigated, fall-back for now.
                TableGridDebugLogger.DebugWriteLine("Table width value is out of bounds.");
                IsUnsupportedCaseDetected = true;
            }

            bool isAbsoluteWidthColumnPresent = (NonPctMinimumSum > 0);
            // Determine the part of the table width that goes to pct columns.
            if (isAbsoluteWidthColumnPresent)
            {
                TableWidthForPctColumns = MathUtil.Divide(TargetTableWidth * PctPreferredSum, TableGridMetrics.HundredPercent);
                // TestTblw0PctG fails if this is not rounded.
            }
            else
            {
                // The whole table width is allocated to pct columns.
                TableWidthForPctColumns = TargetTableWidth;
            }

            // This fixes a case in TestTcwMixedD (table 1). The sum of minimums is distributed using pct preferred,
            // but as some column minimums are greater, an extra width is assigned to other pct columns.
            // The difference is taken from all columns after resizing the absolute columns. Strange logic.
            if (TableWidthForPctColumns < PctMinimumSum)
                TableWidthForPctColumns = PctMinimumSum;

            int tableWidthForNonPctColumns = TargetTableWidth - TableWidthForPctColumns;
            if (tableWidthForNonPctColumns < NonPctMinimumSum)
                tableWidthForNonPctColumns = NonPctMinimumSum;

            // MS Word seems to re-calculate grid percent total from the actual width available to pct columns.
            TableWidthForPctColumns = System.Math.Max(0, TargetTableWidth - tableWidthForNonPctColumns);
            TableWidthForTwipColumns = tableWidthForNonPctColumns;
        }

        /// <summary>
        /// Calculates auto table width from the grid, not taking the container width into account.
        /// </summary>
        private static int ResolveAutoTableWidth(int autoTableWidthFromPercent, int pctPreferredSum, int pctTwipWidthSum, int nonPctWidthSum)
        {
            // Resolve auto table width.
            int autoTableWidth;
            bool isNonZeroPercentWidthPresent = (pctPreferredSum > 0);
            if (isNonZeroPercentWidthPresent)
            {
                autoTableWidth = autoTableWidthFromPercent;
                // WORDSNET-19549, WORDSNET-18233: assign the percentage remaining from pct cells to non-pct cells and compute table width from them, too.
                int nonPctColumnsPctShare = TableGridMetrics.HundredPercent - pctPreferredSum;
                if (nonPctColumnsPctShare > 0)
                {
                    // Compute the table width as if non-pct cells occupy the width remaining after pct cells.
                    int autoTableWidthFromNonPctColumns = MathUtil.Divide(nonPctWidthSum * TableGridMetrics.HundredPercent, nonPctColumnsPctShare);
                    // Take the greater width.
                    autoTableWidth = System.Math.Max(autoTableWidthFromPercent, autoTableWidthFromNonPctColumns);
                }
                else if (nonPctWidthSum > 0)
                {
                    // There are non-pct cells, but 100% of table width is taken by pct cells preferred widths.
                    // Set the table width to maximum.
                    autoTableWidth = TableGridMetrics.MaxWidthTwips;
                    // This will make the table to fill 100% of the container.
                    // This way non-pct cells will be squeezed as much as possible, and pct cells will get as close to 100% as possible.
                    // The logic can be interpreted as computing an infinite auto width because there is 0% left for non-pct columns.
                }
            }
            else
            {
                // No columns with pct preferred greater than 0.
                // Set auto width from twip preferred and metrics.

                // TestTcwMixedE2: when *all* columns with pct types have zero width,
                // twip or minimum for those columns is taken into account, even for auto-fit tables.
                int gridWidthFromPreferredTwipsAndMetrics = pctTwipWidthSum + nonPctWidthSum;
                autoTableWidth = gridWidthFromPreferredTwipsAndMetrics;
            }

            autoTableWidth = System.Math.Min(TableGridMetrics.MaxWidthTwips, autoTableWidth);

            TableGridDebugLogger.DebugWriteLine("Auto table width before resizing: " + autoTableWidth);
            return autoTableWidth;
        }

        /// <summary>
        /// Calculates the actual table width for resizing from the preferred width.
        /// </summary>
        private void ResolvePreferredTableWidth(PreferredWidth tblw, int autoTableWidth, int containerWidthForTblwPct, int tableIndentTwips)
        {
            Debug.Assert(MaxAllowedWidthFromContainer >= 0);
            Debug.Assert(CellSpacing >= 0);

            // Get the target table width for resizing.
            int targetTableWidth;
            if (tblw.IsAuto)
            {
                targetTableWidth = autoTableWidth;
                bool isNonZeroPercentWidthPresent = (PctPreferredSum > 0);
                if (isNonZeroPercentWidthPresent || Grid.IsAutoFit)
                {
                    // Set the width to container width if it is less than the computed auto width.
                    targetTableWidth = System.Math.Min(targetTableWidth, MaxAllowedWidthFromContainer);
                    // In absence of pct cells container width squeezes auto width for auto-fit tables,
                    // but not for fixed layout.
                }

                if (!Grid.IsAutoFit)
                {
                    // In fixed layout, auto table width may be assigned from twip preferred even when all columns have pct preferred.
                    // It can also be greater than the container in that case.
                    // TestTblw0Mixed, Test22898.
                    int gridWidthFromPreferredTwipsAndMetrics = PctTwipWidthSum + NonPctWidthSum;
                    targetTableWidth = System.Math.Max(targetTableWidth, gridWidthFromPreferredTwipsAndMetrics);
                }
            }
            else if (tblw.IsPercent)
            {
                // Compute the target table width specified in percent units from the actual container width.

                // WORDSNET-19625 Tblw in percent units is not affected by the table indent.
                // Add the indent back as it was deducted for other tblw types.
                int maxTableWidthForPct = containerWidthForTblwPct + tableIndentTwips;
                // Do not try to use this value instead of MaxAllowedWidthFromContainer, it will break squeezing from minimums.
                maxTableWidthForPct = System.Math.Max(0, maxTableWidthForPct);

                targetTableWidth = MathUtil.Divide(maxTableWidthForPct * tblw.ValueRaw, TableGridMetrics.HundredPercent);
                targetTableWidth -= TotalGridSpacing;

                if (targetTableWidth > TableGridMetrics.MaxWidthTwips)
                {
                    // TestJira12477(): a special test that allows to violate MS Word logic for grid calculation.
                    // Let the fall-back algorithm handle it.
                    // Fall-back for computed table width greater that the max width in MS Word.
                    TableGridDebugLogger.DebugWriteLine("Fall-back for calculated table width greater than the maximum MS Word value.");
                    IsUnsupportedCaseDetected = true;
                    // This is how it works in MS Word:
                    targetTableWidth = TableGridMetrics.MaxWidthTwips;
                }
            }
            else
            {
                targetTableWidth = tblw.ValueTwips;
                targetTableWidth -= TotalGridSpacing;
            }

            // Subtracting spacing can make the target width negative. Use the minimum positive width in that case.
            const int MinimumPositiveWidth = 1;
            targetTableWidth = System.Math.Max(MinimumPositiveWidth, targetTableWidth);

            TargetTableWidth = targetTableWidth;
            TableGridDebugLogger.DebugWriteLine("Target table width for resizing (auto width resolved): " + targetTableWidth);
        }

        /// <summary>
        /// Implements the resizing logic using the values computed when instance was initialized.
        /// </summary>
        internal void Resize()
        {
            int resizedGridWidth = ResizePercentColumns();

            resizedGridWidth += ResizeFromColumnWidth();

            // Handle a marginal case when only percent columns are present and they all have zero width.
            if (HandleZeroPctColumnsOnly())
                resizedGridWidth = TableWidthForPctColumns;

            // Adjust for rounding errors and extra widths assigned to percent columns to honor minimums.
            AdjustAfterResizing(resizedGridWidth, HardMinimumSum);

            // Ensure that MS Word logic for maximum possible grid width is imitated.
            EnsureMaxTableWidth();
        }

        /// <summary>
        /// Ensures that the total width does not exceed the MS Word limitations.
        /// </summary>
        /// <remarks>
        /// This is an experimental method for checking the logic that was not fully investigated.
        /// Fall-back should happen for such cases.
        /// </remarks>
        private void EnsureMaxTableWidth()
        {
            if (TargetTableWidth <= TableGridMetrics.MaxWidthTwips)
                return;

            // Currently, fall-back should happen under the above condition.
            // The fall-back can be moved here when this logic is used for auto-fit layout as well.
            Debug.Assert(IsUnsupportedCaseDetected);

            // TestIntersectionVsGrid64columns shows that the maximum width does not include the left padding.
            // The logic was not thoroughly tested.
            // Additional tests will be required for releasing this behavior:
            // - Adjustment value (? Is negative adjustment possible?):
            //     - table indents;
            //     - nested tables;
            //     - 2013 mode;
            // - extra wide left padding (int16 overflow when padding is added to tblw);
            // - sum of column minimums is greater than the minimum tblw.

            // Recalculate the total grid width as it may not account for rounding adjustments.
            int gridWidth = 0;
            for (int column = 0; column < Grid.Count; ++column)
                gridWidth += Grid[column].Width;

            int effectiveMaxWidth = MaxWidthAdjustment + TableGridMetrics.MaxWidthTwips;
            int widthOverflow = gridWidth - effectiveMaxWidth;
            if (widthOverflow > 0)
            {
                int lastColumnIdx = Grid.Count - 1;
                int lastColumnMinimum = Grid.GetColumnMinimum(lastColumnIdx);
                // In TestTblw0Max MS Word assigns less than minimum defined by paddings to the last column, for the value of MaxWidthAdjustment.
                // I don't know how it should be rendered, but the experimental logic emulates the calculation.
                // The low boundary is pure fantasy, it was not tested.
                int lowMinimumBoundary = System.Math.Min(lastColumnMinimum, TableGridMetrics.MinimalFixedLayoutCellContentWidthTwips);
                int lastColumnMinimumAdjustment = System.Math.Min(MaxWidthAdjustment,
                    lastColumnMinimum - lowMinimumBoundary);

                // Adjust the columns by simply subtracting from the last columns while possible.
                for (int colIdx = lastColumnIdx; colIdx >= 0; --colIdx)
                {
                    TableGridColumn column = Grid[colIdx];
                    int columnMinimum = Grid.GetColumnMinimum(colIdx) - lastColumnMinimumAdjustment;
                    lastColumnMinimumAdjustment = 0;

                    int aboveMinimum = column.Width - columnMinimum;
                    if (aboveMinimum < widthOverflow)
                    {
                        // Set the column to minimum and proceed with the next column.
                        column.Width = columnMinimum;
                        widthOverflow -= aboveMinimum;
                    }
                    else
                    {
                        // Column adjustment fixes the remaining extra width.
                        column.Width -= widthOverflow;
                        break;
                    }
                }

                Grid.DebugWrite("After adjusting the to ensure the maximum table width.");
            }
        }

        /// <summary>
        /// Handle a special case when there are columns with percent preferred types but all their values are 0.
        /// </summary>
        /// <remarks>
        /// The method is needed because the total percent sum is zero, the share for a percent column cannot be computed from it.
        /// </remarks>
        internal bool HandleZeroPctColumnsOnly()
        {
            bool zeroPctColumnsOnly = (NonPctMinimumSum == 0) && (PctPreferredSum == 0);
            if (zeroPctColumnsOnly)
            {
                // The sum of percent preferred is 0 and no absolute columns. A marginal case,
                // but possible when all columns are assigned 0 pct as a result of wide cell distribution. TestTcwMixedE.
                // There must be columns with 0 pct values.
                if (PctMinimumSum > 0)
                {
                    // All columns are marked as having pct preferred, but the values are zero.
                    Debug.Assert(TableWidthForTwipColumns == 0);
                    // Expand all columns evenly from the current values.
                    ExpandEvenly(Grid, TableWidthForPctColumns);
                }
                else
                {
                    // This should not happen. It happened with TestJira12616.
                    TableGridDebugLogger.DebugWriteLine("Something is wrong: no cell width data.");
                    IsUnsupportedCaseDetected = true;
                }
            }

            return zeroPctColumnsOnly;
        }

        /// <summary>
        /// Resizes the grid columns with preferred width specified in percent units.
        /// </summary>
        /// <returns>The width of pct grid columns after resizing.</returns>
        /// <remarks>
        /// The idea is simply to assign to each column the percentage of the table width specified by the preferred width.
        /// Rounding issues and cases when the computed value is less than minimum are handled later.
        /// After non-percent columns are resized, the extra width is deducted from those columns where it is possible (see ShrinkEvenly()).
        /// </remarks>
        internal int ResizePercentColumns()
        {
            // Nothing to do.
            if (PctPreferredSum <= 0)
                return PctMinimumSum;

            // Table width for percent columns is converted back to pct units.
            // This leads to assigning an extra pct unit in TestTcwMixedG.
            int gridPercentTotalFromTblw = MathUtil.Divide(TableWidthForPctColumns * TableGridMetrics.HundredPercent, TargetTableWidth);

            // In absence of non-pct columns, percent sum is scaled to 100%.
            Debug.Assert(NonPctWidthSum > 0 || gridPercentTotalFromTblw == TableGridMetrics.HundredPercent);

            Debug.Assert(PctPreferredSum <= TableGridMetrics.HundredPercent);

            int pctColumnWidthSum = 0;
            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];

                if (!column.HasPreferredPercent)
                {
                    // We have a mix of columns with absolute and percent preferred width.
                    // Skip the columns with absolute width.
                    continue;
                }

                int columnPercent = column.Percent;
                if ((PctPreferredSum < TableGridMetrics.HundredPercent) ||
                    (gridPercentTotalFromTblw < TableGridMetrics.HundredPercent))
                {
                    // Resize the pct width proportionally to the percentage computed from table width. The values are rounded.
                    columnPercent = MathUtil.Divide(column.Percent * gridPercentTotalFromTblw, PctPreferredSum);
                }

                int columnMinimum = Grid.GetColumnMinimum(i);

                // Just get the chunk of table width which is specified by preferred width in percent.
                int columnWidth = MathUtil.Divide(TargetTableWidth * columnPercent, TableGridMetrics.HundredPercent);
                // Even though the sum of the rounded pct values may not be HunderedPercent, use HundredPercent for width calculation.
                // This strange logic fixes rounding in TestJira9110.
                // After the final rounding correction (adding 1 twips to first columns), the values match MS Word exactly.

                if (columnWidth <= columnMinimum)
                    columnWidth = columnMinimum;

                column.Width = columnWidth;
                pctColumnWidthSum += columnWidth;
            }

            Grid.DebugWrite("After assigning width to percent columns:");
            return pctColumnWidthSum;
        }

        /// <summary>
        /// Resizes table grid that does not have columns with width in percent to fit into table width.
        /// </summary>
        /// <returns>The sum of the widths assigned to non-percent columns after resizing. </returns>
        private int ResizeFromColumnWidth()
        {
            int gridWidth = NonPctWidthSum;
            int minimalGridSize = NonPctMinimumSum;

            if (TableWidthForTwipColumns == 0)
            {
                // Nothing to do, it means there are no non-pct columns.
                return 0;
            }

            if (gridWidth == TableWidthForTwipColumns)
                return TableWidthForTwipColumns;

            int resizedWidth;
            if (gridWidth < TableWidthForTwipColumns)
            {
                bool expandIntersectionColumns = (AutoMinimumSum > 0);
                if (expandIntersectionColumns)
                {
                    // This is similar to "expanding auto columns only" in auto-fit mode.
                    // Twip columns will remain at preferred width.
                    resizedWidth = TwipWidthSum;
                    // Intersection columns will be expanded to fill the remaining width.
                    resizedWidth += ExpandItersectionColumns();
                }
                else
                {
                    bool expandAutoColumnsOnly = false;
                    resizedWidth = ResizeAbsoluteColumnsProportionally(TableWidthForTwipColumns, gridWidth, expandAutoColumnsOnly);
                }
            }
            else
            {
                int totalGridAboveMinimum = gridWidth - minimalGridSize;
                if (totalGridAboveMinimum > 0)
                {
                    resizedWidth = ResizeFromMinimums(totalGridAboveMinimum, minimalGridSize, TableWidthForTwipColumns, false);
                }
                else
                {
                    // The table should be already at minimum, nothing to do.
                    Debug.Assert(gridWidth == minimalGridSize);
                    resizedWidth = minimalGridSize;
                }
            }

            return resizedWidth;
        }

        /// <summary>
        /// Resizes the columns with absolute width in the grid by simply multiplying all column widths by the same quotient.
        /// </summary>
        /// <returns>The sum of the widths assigned to non-percent columns after resizing. </returns>
        /// <param name="targetWidth">Total absolute column width target.</param>
        /// <param name="gridWidth">Sum of the resizable grid column widths.</param>
        /// <param name="expandAutoColumnsOnly">Indicates if only columns with auto width should be resized.
        /// It is currently used by auto-fit tables only.</param>
        /// <remarks><para>
        /// This is the simplest approach to resizing.
        /// However as column widths are integer the sum of the resized column widths may not equal the goal because of rounding.
        /// For example, if we have 3 columns of width 1000 and need a table of width 4000,
        /// the column width should be multiplied by 4/3.
        /// Now, if we round all the columns the same way we will get either 3 columns of 1333 or 3 columns of 1334, and the table will be 3999 or 4002, not 4000.
        /// We must tweak some columns in order to get exactly the requested table width.
        /// I made several attempts to reproduce MS Word logic for rounding, and eventually it seems that ShrinkEvenly reproduces the logic.
        /// It should be applied at the right moment though. Currently it is applied in the very end, after resizing both percent and absolute columns.
        /// </para>
        /// </remarks>
        internal int ResizeAbsoluteColumnsProportionally(int targetWidth, int gridWidth, bool expandAutoColumnsOnly)
        {
            int resizedAbsColumnSum = 0;
            // Increase the width of each non-percent column proportionally to the current width.
            Debug.Assert(targetWidth > gridWidth);
            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];

                if (column.HasPreferredPercent)
                    continue;

                if (expandAutoColumnsOnly && !column.IsAuto)
                    continue;

                int newSpanWidth = MathUtil.Divide(column.Width * targetWidth, gridWidth);

                column.Width = newSpanWidth;
                resizedAbsColumnSum += newSpanWidth;
            }

            Grid.DebugWrite("After resizing in ResizeAbsoluteColumnsProportionally:");

            // The sum of the resized values may not match the target because of rounding.
            return resizedAbsColumnSum;
        }

        /// <summary>
        /// Resizes the grid changing the difference between the minimal width and the preferred (current) width proportionally for each column.
        /// </summary>
        /// <returns>The sum of the widths assigned to the columns.</returns>
        /// <param name="gridAboveMinimum">Sum of current grid column widths minus sum of grid column minimums.</param>
        /// <param name="gridMinimum">Sum of grid column minimums.</param>
        /// <param name="targetWidth">Target table width.</param>
        /// <param name="updatePercentColumns">Indicates if grid columns with width specified in percent units should be updated.
        /// Their width is included in the return value depending on this parameter.</param>
        /// <remarks>
        /// This is the resizing which MS Word uses most often.
        /// </remarks>
        internal int ResizeFromMinimums(int gridAboveMinimum,
            int gridMinimum,
            int targetWidth,
            bool updatePercentColumns)
        {
            int extraWidth = gridMinimum + gridAboveMinimum - targetWidth;
            // Though the same approach could be used for expanding, it is used for squeezing only.
            Debug.Assert(extraWidth >= 0);
            // The calculation below may not work with negative extra width.

            // Scale the width above the minimal width proportionally for each cell.
            int resizedWidthSum = 0;
            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];
                if (column.HasPreferredPercent && !updatePercentColumns)
                    continue;

                int columnWidth = column.Width;

                int columnMinimum = Grid.IsAutoFit && !updatePercentColumns
                    ? column.ContentMinimum
                    : column.Minimum;

                int newValue;
                if (gridAboveMinimum > 0)
                {
                    int aboveMinimum = System.Math.Max(0, columnWidth - columnMinimum);
                    // Computing new extra width this way imitates MS Word logic
                    // and fixes 1-twip rounding differences for Test8442 and Test23360.
                    int newExtraWidth = MathUtil.MulDiv(extraWidth, aboveMinimum, gridAboveMinimum);

                    newValue = columnWidth - newExtraWidth;
                    newValue = System.Math.Max(columnMinimum, newValue);
                }
                else
                {
                    // Just set all columns to minimum as there is nothing to squeeze.
                    newValue = columnMinimum;
                }
                resizedWidthSum += newValue;

                Debug.Assert(newValue <= TableGridMetrics.MaxWidthTwips);

                column.Width = newValue;
            }

            Grid.DebugWrite("After resizing in ResizeFromMininums:");

            return resizedWidthSum;
        }

        /// <summary>
        /// Expands "intersection" columns only in a fixed layout table.
        /// </summary>
        /// <returns> The total width of the resized intersection columns.</returns>
        /// <remarks>
        /// The method is used for fixed layout table only.
        /// It is somewhat similar to "expanding auto columns only" in auto-fit layout.
        /// However, there differences:
        /// - this method is specific to 2003 mode only, it seems with 2003 mode off there is no such logic for fixed layout.
        /// - this method is neither "resize proportionally" nor "resize from minimums".
        ///        Instead, extra width is distributed proportionally to the current minimums.
        /// The principal test is TestTcwMixedK() for 2003 mode.
        /// </remarks>
        private int ExpandItersectionColumns()
        {
            // Leave columns with twip preferred at their preferred width.
            int tableWidthForIntersectionColumns = TableWidthForTwipColumns - TwipWidthSum;
            int intersectionMinimumSum = AutoMinimumSum;

            int extraWidth = tableWidthForIntersectionColumns - intersectionMinimumSum;
            if (extraWidth <= 0)
                return intersectionMinimumSum;

            int resizedIntersectionColumnSum = 0;
            // Increase the width of each intersection column proportionally to the current minimum.
            Debug.Assert(tableWidthForIntersectionColumns > intersectionMinimumSum);
            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];
                bool isIntersection = column.IsZeroWidthIntersection;
                if (!isIntersection)
                    continue;

                int columnShare = MathUtil.Divide(column.Minimum * extraWidth, intersectionMinimumSum);
                int newWidth = column.Minimum + columnShare;
                column.Width = newWidth;
                resizedIntersectionColumnSum += newWidth;
            }

            Grid.DebugWrite("After resizing intersection columns:");

            // The sum of the resized values may not match the target because of rounding.
            return resizedIntersectionColumnSum;
        }

        /// <summary>
        /// Shrinks the table grid by trying to subtract the same value from all columns.
        /// </summary>
        /// <param name="extraWidthOriginal">The extra width to be removed from the grid.</param>
        /// <remarks>
        /// The idea is simple, all columns are reduced by (extraWidth / squeezableColumns).
        /// However, for some columns the result would be less than minimum.
        /// If such column is met, the subsequent columns are reduced more to compensate.
        /// If at the last column it is found that extraWidth is still present, the process is repeated with the remaining extra width.
        /// This is how MS Word seems to work, though minor differences of 1 twip are still possible.
        /// </remarks>
        internal void ShrinkEvenly(int extraWidthOriginal)
        {
            if (extraWidthOriginal <= 0)
                return;

            // Count columns above minimum.
            int squeezableColumns = 0;
            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];
                int columnMinimum = Grid.GetColumnMinimum(i);

                Debug.Assert(column.Width >= columnMinimum);

                if (column.Width > columnMinimum)
                    squeezableColumns++;
            }

            int extraWidth = extraWidthOriginal;
            while (extraWidth > 0 && squeezableColumns > 0)
            {
                int squeezingShare = extraWidth / squeezableColumns;
                int squeezingShareRemainder = extraWidth % squeezableColumns;

                // If some columns cannot be squeezed, the extra width is accumulated here.
                extraWidth = 0;

                for (int i = 0; i < Grid.Count; ++i)
                {
                    TableGridColumn column = Grid[i];
                    int columnMinimum = Grid.GetColumnMinimum(i);

                    if (columnMinimum < column.Width)
                    {
                        // Subtract one more from the first squeezingShareRemainder columns
                        // in case extraWidth is not divisible by squeezableColumns.
                        int oneMore = (squeezingShareRemainder > 0) ? 1 : 0;
                        squeezingShareRemainder--;

                        // Greedy: always try to take all the extra width, including the width not taken from the previous columns.
                        // TestTcwPctLtMinimal confirms and explains the logic.
                        int newColumnWidth = column.Width - squeezingShare - oneMore - extraWidth;

                        if (newColumnWidth <= columnMinimum)
                        {
                            // What cannot be subtracted because of column minimum becomes the new extra width:
                            extraWidth = columnMinimum - newColumnWidth;

                            newColumnWidth = columnMinimum;
                            squeezableColumns--;
                        }
                        else
                        {
                            // Extra width compensated successfully.
                            extraWidth = 0;
                        }

                        column.Width = newColumnWidth;
                    }
                }
                // The process is repeated if extra width is positive after processing all columns
                // unless there are no more squeezable columns.
            }

            Grid.DebugWrite("After compensating extra width in ShrinkEvenly:");
        }

        /// <summary>
        /// Expands all grid columns evenly to get the requested tableWidth.
        /// </summary>
        // The method could be combined with ShrinkEvenly as the logic is somewhat parallel.
        // This one is simpler though, so I decided to keep the separate
        // for readability as combining would add more conditions inside.
        private static void ExpandEvenly(TableGrid tableGrid, int tableWidth)
        {
            // Count columns above minimum.
            int gridWidth = 0;
            for (int i = 0; i < tableGrid.Count; ++i)
            {
                TableGridColumn column = tableGrid[i];
                Debug.Assert(column.Width >= column.Minimum);
                gridWidth += column.Width;
            }

            int extraWidth = tableWidth - gridWidth;

            if (extraWidth <= 0)
                return;

            int resizableColumnCount = tableGrid.Count;
            int columnShare = extraWidth / resizableColumnCount;
            int columnShareRemainder = extraWidth % resizableColumnCount;

            for (int i = 0; i < tableGrid.Count; ++i)
            {
                TableGridColumn column = tableGrid[i];

                // Add one more to the first columnShareRemainder columns
                // in case extraWidth is not divisible by resizableColumnCount.
                int oneMore = (columnShareRemainder > 0) ? 1 : 0;
                columnShareRemainder--;

                int newColumnWidth = column.Width + columnShare + oneMore;
                column.Width = newColumnWidth;
            }

            tableGrid.DebugWrite("After compensating extra width in ExpandEvenly:");
        }

        /// <summary>
        /// Compensates for extra widths or rounding errors after the previous resizing steps.
        /// </summary>
        internal void AdjustAfterResizing(int resizedGridWidth, int totalMinimumAfterResizing)
        {
            // Extra width may be the result of pct column resizing when a greater width is assigned
            // because column content minimum is greater than the width calculated from column percent preferred.
            // In that case, other percent columns are reduced to compensate.
            int extraWidth = resizedGridWidth - TargetTableWidth;
            if (extraWidth > 0)
            {
                // Compensate for extra width.
                // This will also handle extra widths originating from rounding.
                ShrinkEvenly(extraWidth);
            }
            else
            {
                // Compensate when the actual width is less than intended because of rounding.
                FixRounding(resizedGridWidth, totalMinimumAfterResizing);
            }
        }

        /// <summary>
        /// Implements fixing the rounding by changing the width of first grid columns by 1.
        /// </summary>
        /// <remarks>
        /// It can bump the widths both up and down.
        /// </remarks>
        internal void FixRounding(int resizedWidth, int minimumSum)
        {
            // Do nothing for zero width, it is used in fixed layout to assign the preferred widths to the columns. No rounding is needed.
            if (TargetTableWidth == 0)
                return;

            if (TargetTableWidth < minimumSum)
            {
                Debug.Assert(resizedWidth == minimumSum);
                return;
            }

            int underFlow = TargetTableWidth - resizedWidth;
            if (underFlow == 0)
                return;

            // Extra width is handled earlier.
            Debug.Assert(underFlow > 0);

            // As it is supposed that rounding introduces no more than 1 twip error to each column,
            // the total error should be less than the number of columns.
            Debug.Assert(underFlow <= Grid.Count);

            // Bump up the computed width of the first N columns.
            TableGridConstructor.FixRounding(Grid, 0, Grid.Count, underFlow, GridRoundingColumn.Width);
        }

        /// <summary>
        /// Table grid being resized.
        /// </summary>
        internal TableGrid Grid { get; }

        /// <summary>
        /// The maximum table width specified by the table or container properties.
        /// </summary>
        internal int MaxAllowedWidthFromContainer { get; }

        /// <summary>
        /// This is used to adjust the grid when the table is so wide that it exceeds the maximum width allowed in MS Word.
        /// </summary>
        /// <remarks>
        /// The logic is still experimental.
        /// </remarks>
        private int MaxWidthAdjustment { get; }

        /// <summary>
        /// Spacing between cells, full value as displayed in MS Word UI.
        /// </summary>
        private int CellSpacing { get; }

        /// <summary>
        /// Total width of spacing for all grid columns.
        /// </summary>
        private int TotalGridSpacing
        {
            // Between each column and on the sides.
            get { return CellSpacing * (Grid.Count + 1); }
        }

        /// <summary>
        /// The target width to which the table is resized, in twips.
        /// </summary>
        internal int TargetTableWidth { get; set; }

        // The properties below could be read-only, but most of them are set in a method called from the constructor.

        /// <summary>
        /// The target width for columns not having percent preferred width specified.
        /// </summary>
        internal int TableWidthForTwipColumns { get; set; }

        /// <summary>
        /// The target width for columns having percent width specified.
        /// </summary>
        /// <remarks>
        /// This can be less than the minimum possible width for such columns, it is handled during resizing.
        /// </remarks>
        internal int TableWidthForPctColumns { get; set; }

        // The properties below represent sums of the grid column widths for certain column types.

        /// <summary>
        /// The sum of preferred widths specified in percent units.
        /// </summary>
        internal int PctPreferredSum { get; set; }

        /// <summary>
        /// The sum of minimum width for columns having preferred width in percent units.
        /// </summary>
        internal int PctMinimumSum{ get; set; }

        /// <summary>
        /// The sum of widths in twips assigned to columns with pct types.
        /// </summary>
        /// <remarks>
        /// It is only used to compute auto table width in sum cases.
        /// </remarks>
        internal int PctTwipWidthSum { get; set; }

        /// <summary>
        /// The sum of widths assigned to the columns not having twip or percent preferred specified.
        /// </summary>
        /// <remarks>
        /// For fixed layout, intersection columns not matching any single-column cells are treated in a way similar to auto columns in auto-fit.
        /// The property is used for them. The logic is only applicable to 2003 mode.
        /// </remarks>
        internal int AutoWidthSum{ get; set; }

        /// <summary>
        /// The sum of minimum widths assigned to the columns not having twip or percent preferred specified.
        /// </summary>
        /// <remarks>
        /// For fixed layout, intersection columns not matching any single-column cells are treated in a way similar to auto columns in auto-fit.
        /// The property is used for them. The logic is only applicable to 2003 mode.
        /// </remarks>
        internal int AutoMinimumSum{ get; set; }

        /// <summary>
        /// The sum of widths assigned to columns having preferred width in twips (and not having preferred width in percent units).
        /// </summary>
        internal int TwipWidthSum{ get; set; }
        /// <summary>
        /// The sum of minimums assigned to columns having preferred width in twips (and not having preferred width in percent units).
        /// </summary>
        internal int TwipMinimumSum{ get; set; }

        /// <summary>
        /// The total sum of minimums that does not take content into account.
        /// </summary>
        /// <remarks>
        /// For auto-fit column ContentMinimum which includes the minimum content width is normally used for resizing.
        /// But in case there is not enough space, the table may be squeezed further, from content minimums
        /// to "hard" minimums defined by cell margins and borders.
        /// A cell cannot be possibly narrower than the "hard" minimum defined by margins/borders.
        /// The sum represents such minimums, unlike all of the above which use content minimums for auto-fit.
        /// </remarks>
        internal int HardMinimumSum{ get; set; }

        /// <summary>
        /// The sum of width assigned to columns not having preferred width in percent units.
        /// </summary>
        internal int NonPctWidthSum { get { return AutoWidthSum + TwipWidthSum; } }

        /// <summary>
        /// The sum of minimums for columns not having preferred width in percent units.
        /// </summary>
        internal int NonPctMinimumSum { get { return AutoMinimumSum + TwipMinimumSum; } }

        /// <summary>
        /// Indicates if an unsupported case was encountered during resizing.
        /// </summary>
        internal bool IsUnsupportedCaseDetected { get; private set; }
    }
}
