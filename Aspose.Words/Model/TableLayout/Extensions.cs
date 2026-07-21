// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2009 by Roman Korchagin

using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Utility methods that operate on model nodes, but I don't want them in the model classes because
    /// these methods are for this table layouter module only and it might later be deprecated.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Calculates maximum table width taking container width, cell borders and margins into account,
        /// and also resolves table preferred width in percent units.
        /// </summary>
        internal static int GetMaxAllowedWidthResolveTblwPct(Table table)
        {
            int containerWidthDelta = GetContainerWidthDeltaLegacy(table);
            int tableWidth = GetMaxAllowedWidthFromContainer(table, containerWidthDelta);

            // Resolve tblw in percent units.
            if ((table.PreferredWidth.IsPercent) && (table.PreferredWidth.ValueRaw > 0))
            {
                // WORDSNET-19625 Add back table indent for calculation of table preferred width in percent units.
                tableWidth += ConvertUtilCore.PointToTwip(table.LeftIndent);
                tableWidth = (int)System.Math.Round(tableWidth * table.PreferredWidth.Value / 100d);
            }

            tableWidth = System.Math.Min(tableWidth, MaxTableWidthTwips);
            tableWidth = System.Math.Max(tableWidth, ConvertUtilCore.MinSizeTwip);
            return tableWidth;
        }

        /// <summary>
        /// Calculates maximum table width taking container width, cell borders and margins into account.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For fixed table layout, the calculated value is used for percent table width only.
        /// In other cases, fixed tables ignore the container width.
        /// </para>
        /// </remarks>
        internal static int GetMaxAllowedWidthFromContainer(Table table, int containerWidthDelta)
        {
            // DM: An unnecessary conversion twips->points->twips here may possibly be a source of a minor inaccuracy.
            int containerWidth = ConvertUtilCore.PointToTwip(TableLayouter.GetContainerWidth(table.FirstNonMarkupParentNode));
            // The result may be negative, but as the caller might add indent for pct tables, it should not be set to 0.
            return containerWidth + containerWidthDelta;
        }

        /// <summary>
        /// Gets the difference between container width and maximum width available for the table in the container
        /// for the legacy table layouter.
        /// </summary>
        /// <remarks>
        /// When spacing was implemented per WORDSNET-4794, it turned out that the result of old table layouter
        /// may look worse if metrics that take spacing into accounts are used.
        /// So this method preserves the old logic to avoid possible breakages older table layout logic.
        /// </remarks>
        private static int GetContainerWidthDeltaLegacy(Table table)
        {
            bool useLegacyLogic = true;
            return GetContainerWidthDeltaImpl(table, useLegacyLogic);
        }

        /// <summary>
        /// Gets the difference between container width and maximum width available for the table in the container.
        /// </summary>
        /// <remarks>
        /// For auto-fit tables, container width may not be known until layout is built.
        /// The difference is computed the same way for auto-fit and layout tables, so this method is used in both cases.
        /// </remarks>
        internal static int GetContainerWidthDelta(Table table)
        {
            bool useLegacyLogic = false;
            return GetContainerWidthDeltaImpl(table, useLegacyLogic);
        }

        /// <summary>
        /// Implements <see cref="GetCellSpacingWidthDelta(Cell)"/> preserving the legacy logic depending on <paramref name="useLegacyLogic"/>.
        /// </summary>
        private static int GetContainerWidthDeltaImpl(Table table, bool useLegacyLogic)
        {
            Cell firstCell = table.FirstRow.FirstCell;
            // AM. Here and below is a little optimization.
            // This code partially duplicates Cell.CellFormat.Borders and Cell.CellFormat
            // but avoids creation of CellFormat and inherited Border objects for every cell involved into calculation.
            int leftBorderTwips = GetLeftBorderTwips(firstCell);
            int leftBorderTwipsHalf = MathUtil.Divide(leftBorderTwips, 2);
            int leftPadding = ResolvedLeftPadding(firstCell);

            Cell lastCell = GetLastCell(table.FirstRow);
            //The same for the last cell.
            int rightBorderTwips = GetRightBorderTwips(lastCell);
            int rightBorderTwipsHalf = MathUtil.Divide(rightBorderTwips, 2);
            int rightPadding = ResolvedRightPadding(lastCell);

            int spacingDeltaLegacy = GetSpacingDeltaLegacy(table, leftBorderTwips, rightBorderTwips);

            int delta;
            if (table.IsNested)
            {
                // Outer part of the nested table borders do not overlap the container cell margins/borders.
                // So they decrease available width, too:
                delta = -(leftBorderTwipsHalf + rightBorderTwipsHalf);
                // TestNestedTblwBorderA() confirms that the nested table border half are rounded to twips separately.
                // The line does not look logical for cell spacing as the "external" table border is not included in the nested width.
                // Still, TestSpacingTblwNestedC seems to confirm the logic.

                // Container border and margins decrease the available width.
                Cell parentCell = (Cell)table.FirstNonMarkupParentNode;
                int parentPaddingSum = ResolvedLeftPadding(parentCell) + ResolvedRightPadding(parentCell);
                int parentBorderSum = GetLeftBorderTwips(parentCell) + GetRightBorderTwips(parentCell);
                int parentBorderHalfSum = MathUtil.Divide(parentBorderSum, 2);
                // TestNestedTblwBorder() confirms that sum of the parent border halves is rounded in this case,
                // not each border half individually.

                if (useLegacyLogic)
                    delta += spacingDeltaLegacy;
                else
                    delta -= GetCellSpacingWidthDelta(parentCell);

                bool useNewSpacingLogic = parentCell.ParentTable.AllowCellSpacing && !useLegacyLogic;
                delta -= useNewSpacingLogic
                    ? parentPaddingSum + parentBorderSum
                    : System.Math.Max(parentPaddingSum, parentBorderHalfSum);
                // The above line imitates a faulty MS Word behavior confirmed by TestNestedTblwPct*, TestNestedBorderOverlap*.
                // When one of the border halves is less than the corresponding margin, and on the other the margin is greater,
                // this approach causes a not-quite logical calculation that allows the right border of the nested table to overlap
                // container cell margin or border.
            }
            else
            {
                // Word positions a table slightly extended to margins in compatibility mode.
                // In 2013 mode, borders are inside the container, so the table width is reduced.
                if (table.Document.DocPr.CompatibilityOptions.IsWord2013OrLaterCompatible)
                {
                    // TestSpacingTblwAuto() confirms that the same logic is used both with and without spacing.
                    delta = -(leftBorderTwipsHalf + rightBorderTwipsHalf);
                    // This does not look correct: with spacing, full border width for 2013 mode would be more logical.
                    // But it appears that MS Word works this way.
                }
                else
                {
                    bool useNewSpacingLogic = table.AllowCellSpacing && !useLegacyLogic;
                    delta = useNewSpacingLogic
                        ? table.FirstRow.CellSpacing * 4 + leftPadding + rightPadding + leftBorderTwips + rightBorderTwips
                        : System.Math.Max(leftBorderTwipsHalf, leftPadding) +
                            System.Math.Max(rightBorderTwipsHalf, rightPadding);
                    // Strangely enough, the border vs padding handling seems to be different for table width and for cell width.
                    // For table, the maximum of border/2 and padding is taken for the left and right side independently, as above.
                    // For cell, the maximum is taken between the sums of borders and padding, as in GetCellBordersAndPaddingTwips() below.
                    // TestTblwBorder() confirms that each border half is rounded separately.

                    if (useLegacyLogic)
                        delta += spacingDeltaLegacy;
                }
            }

            // Table indent squeezes the table width when it does not fit into container.
            // For auto-fit tables, nested table indent also increases content minimum for the container cell,
            // regardless of the nested table tblw type.
            // Include the indent in the delta, this way it can be used in most cases without extra conditions.
            delta -= ConvertUtilCore.PointToTwip(table.LeftIndent);

            delta += GetShapeContainerWidthAdjustmentTwips(table);

            return delta;
        }

        /// <summary>
        /// Gets container width adjustment to add to the table container width when calculating nested table width.
        /// </summary>
        /// <remarks>
        /// Currently it just returns 0, or magical 15 twips for tables inside shapes, under certain conditions.
        /// </remarks>
        internal static int GetShapeContainerWidthAdjustmentTwips(Table table)
        {
            // Shape container width is only adjusted for table having tblw in percent units: TestTextBoxContainer.
            if (!table.PreferredWidth.IsPercent)
                return 0;

            CompositeNode tableContainer = table.FirstMeaningfulParentNode;
            if (tableContainer.NodeType != NodeType.Shape)
                return 0;

            // The adjustment is only needed for shapes that cannot grow to accommodate contents.
            ShapeAdaptor shapeAdaptor = new ShapeAdaptor((Shape)tableContainer);
            if (shapeAdaptor.WrapMode == TextBoxWrapMode.None)
                return 0;

            // MS Word seems to add 15 twips to the text box width in compatibility mode. See TestTblwPctInTextBox.
            const int Magical15Twips = 15;
            bool isIn2013Mode = (tableContainer.Document != null) &&
                                tableContainer.Document.DocPr.CompatibilityOptions.IsWord2013OrLaterCompatible;
            bool isNestedInTableCell = (tableContainer.GetAncestor(NodeType.Cell) != null);
            // It seems, 15 twips are still used in 2013 mode for nested shapes. (TestLayoutTableGridFixed2013.TestTblwPctInTextBoxNested).
            bool addMagicalTwips = !isIn2013Mode || isNestedInTableCell;
            return addMagicalTwips
                ? Magical15Twips
                : 0;
        }

        /// <summary>
        /// Computes the width of the cell spacing included in the cell width.
        /// </summary>
        /// <remarks>
        /// Cell spacing width depends on the cell position in the row (more spacing in cells on the edge).
        /// </remarks>
        private static int GetCellSpacingWidthDelta(Cell cell)
        {
            int halfSpacing = cell.ParentRow.CellSpacing;

            // Parent cell spacing depends on cell position in a row.
            // Half of the actual table spacing is stored in the model.
            // Cells in middle columns have half of the spacing on each side included in the width.
            // Cells on the edge have full spacing on the outer edge.
            int delta = halfSpacing * 2;

            bool isInLeftmostColumn = (cell.PreviousCell == null);
            if (isInLeftmostColumn)
                delta += halfSpacing;

            // TestSpacingTblwNestedB: no need to analyze gridAfter (and probably gridBefore),
            // spacing correction depends on presence of other cells before/after this in the row,
            // and not on the cell column being actually first or last in the table.
            // No need to analyze other rows.

            // This may not work correctly with horizontally merged cells,
            // But the method is only used with the new table grid computation that normalizes horizontal merges.
            Debug.Assert(cell.CellPr.HorizontalMerge != CellMerge.Previous);
            bool isInRightmostColumn = (cell.NextCell == null);
            if (isInRightmostColumn)
                delta += halfSpacing;

            return delta;
        }

        /// <summary>
        /// Gets last cell from the row skipping horizontally merged cells.
        /// </summary>
        private static Cell GetLastCell(Row row)
        {
            Cell last = row.LastCell;
            while ((last != null) && (last.CellPr.HorizontalMerge == CellMerge.Previous))
                last = last.PreviousCell;

            return last;
        }

        /// <summary>
        /// A somewhat strange cell spacing computation method used for the legacy table layouter.
        /// </summary>
        /// <remarks>
        /// The method combines spacing and borders for some reason.
        /// Leave it as is to preserve the legacy logic intact.
        /// </remarks>
        private static int GetSpacingDeltaLegacy(Table table, int leftBorderWidth, int rightBorderWidth)
        {
            int spacing = table.FirstRow.TablePr.CellSpacing.ValueRaw;
            return (spacing != 0) ? (leftBorderWidth + rightBorderWidth + spacing * 4) : 0;
        }

        /// <summary>
        /// Return the number of columns occupied by this cell.
        /// </summary>
        internal static int GetHorizontalMergeSpan(Cell cell)
        {
            int span = 0;
            Cell nextCell = cell;
            do
            {
                span += nextCell.CellPr.GridSpan;
                nextCell = nextCell.NextCell;
            } while ((nextCell != null) && (nextCell.CellPr.HorizontalMerge == CellMerge.Previous));

            return span;
        }

        /// <summary>
        /// Returns the total width of left and right paddings and left and right borders.
        /// VSazh: Maybe we need to divide borders by 2?
        /// </summary>
        /// <remarks>
        /// This method is only used by the old <see cref="TableLayouter"/>.
        /// </remarks>
        internal static double GetPaddingAndBorderWidth(ICellAttrSource cellAttrSource)
        {
            Border leftBorder = (Border)cellAttrSource.FetchCellAttr(CellAttr.BorderLeft);
            Border rightBorder = (Border)cellAttrSource.FetchCellAttr(CellAttr.BorderRight);

            double leftPadding = ConvertUtilCore.TwipToPoint((int)cellAttrSource.FetchCellAttr(CellAttr.LeftPadding));
            double rightPadding = ConvertUtilCore.TwipToPoint((int)cellAttrSource.FetchCellAttr(CellAttr.RightPadding));

            return leftPadding + rightPadding + leftBorder.BorderWidth + rightBorder.BorderWidth;
        }

        /// <summary>
        /// Returns total width of left and right first row borders and cell spacing in points.
        /// </summary>
        /// <remarks>
        /// This method is only used by the old <see cref="TableLayouter"/>.
        /// </remarks>
        internal static double GetTotalBorderAndSpacingWidth(Table table)
        {
            if (table.Rows.Count == 0)
                return 0;

            // Getting inherited attribute this way we avoid to create inherited borders.
            IRowAttrSource firstRow = table.FirstRow;
            Border leftBorder = (Border)firstRow.FetchRowAttr(TableAttr.BorderLeft);
            Border rightBorder = (Border)firstRow.FetchRowAttr(TableAttr.BorderRight);

            return leftBorder.BorderWidth + rightBorder.BorderWidth + table.CellSpacing * (table.GetColumnCount() + 1);
        }

        /// <summary>
        /// Gets left cell border width, in twips, taking possible adjacent cell border conflict into account.
        /// </summary>
        internal static int GetLeftBorderTwips(Cell cell)
        {
            // TestJira16768 passes null here when validating a table with no cells in row 1.
            // I think something is wrong with the validation sequence, but I will handle it here for now.
            if (cell == null)
                return 0;

            bool isMergedToPrevious = (cell.CellPr.HorizontalMerge == CellMerge.Previous) &&
                (cell.PreviousCell != null) &&
                (cell.PreviousCell.CellPr.HorizontalMerge != CellMerge.None);
            // Merge restarts if "Previous" follows "None".

            // So, left border will be returned only for the first-in merge cell.
            // This is OK as getting the left border for a cell in the middle is never actually needed.
            return isMergedToPrevious
                ? 0
                : GetCellBorderTwips(cell, CellAttr.BorderLeft);
        }

        /// <summary>
        /// Gets right cell border width, in twips, taking horizontal merges
        /// and possible adjacent cell border conflict into account.
        /// </summary>
        private static int GetRightBorderTwips(Cell cell)
        {
            // TestJira16768 passes null here when validating a table with no cells in row 1.
            // I think something is wrong with the validation sequence, but I will handle it here for now.
            if (cell == null)
                return 0;

            // TestBorderConflictB() shows that for a series of horizontally merged cells,
            // right border from the last-in-merge cell is used.
            Cell lastInMerge = cell;
            if (cell.CellPr.HorizontalMerge != CellMerge.None)
            {
                for (Cell nextCell = lastInMerge.NextCell;
                    (nextCell != null) && (nextCell.CellPr.HorizontalMerge == CellMerge.Previous);
                    nextCell = nextCell.NextCell)
                    lastInMerge = nextCell;
            }

            // This will return the right border of the last-in-merge cell for any of the merged cells.
            return GetCellBorderTwips(lastInMerge, CellAttr.BorderRight);
            // Actually, it is only used for the first-in-merge cell and handling it here simplifies the caller logic.
            // The logic is different from GetLeftBorderTwips() but there is no reason to make the methods uniform for now.
        }

        /// <summary>
        /// Gets the specified cell border widths taking adjacent cell border conflict into account.
        /// </summary>
        private static int GetCellBorderTwips(Cell cell, int borderAttrKey)
        {
            Debug.Assert(borderAttrKey == CellAttr.BorderLeft || borderAttrKey == CellAttr.BorderRight);

            Border border = (Border)((ICellAttrSource)cell).FetchCellAttr(borderAttrKey);
            Border conflictingBorder = GetConflictingBorder(cell, borderAttrKey);
            border = Border.GetWinningBorder(border, conflictingBorder);

            // Cannot use border.BorderWidth as it returns 0.25 for zero width.
            float points = Border.GetActualWidth(border.LineStyle, (float)border.LineWidth);
            int twips = ConvertUtilCore.PointToTwip(points);

            // The above logic may not work correctly for borders like 0.125pt.
            // Assertion below fires for Test23326 and some other tests.
            // Debug.Assert(System.Math.Abs((double)twips / ConvertUtilCore.TwipsPerPoint - points) < 0.01)
            // I've submitted WORDSNET-26272 for the issue.

            return twips;
        }

        /// <summary>
        /// Gets a border from an adjacent cell that may conflict with the specified cell border.
        /// </summary>
        private static Border GetConflictingBorder(Cell cell, int borderAttrKey)
        {
            Debug.Assert(borderAttrKey == CellAttr.BorderLeft || borderAttrKey == CellAttr.BorderRight);

            // The logic is not needed for tables with cell spacing (adjacent borders do not conflict).
            if (cell.ParentRow != null && cell.ParentTable != null && cell.ParentTable.AllowCellSpacing)
                return Border.Empty;

            // The behavior with html blocks was not tested (grid re-calculation does not support html blocks anyways).

            // Horizontal cell merges are handled by the caller.
            Cell adjacentCell;
            int conflictingBorderAttr;
            switch (borderAttrKey)
            {
                case CellAttr.BorderLeft:
                    conflictingBorderAttr = CellAttr.BorderRight;
                    adjacentCell = cell.PreviousCell;
                    break;
                case CellAttr.BorderRight:
                    conflictingBorderAttr = CellAttr.BorderLeft;
                    adjacentCell = cell.NextCell;
                    break;
                default:
                    return Border.Empty;
            }

            // Do not resolve conflicts with merged cells.
            if ((adjacentCell == null))
                return Border.Empty;

            Border conflictingBorder = (Border)((ICellAttrSource)adjacentCell).FetchCellAttr(conflictingBorderAttr);
            return conflictingBorder;
        }

        internal static int GetCellBordersAndPaddingTwips(Cell cell)
        {
            int leftBorder = GetLeftBorderTwips(cell);
            int leftPadding = ResolvedLeftPadding(cell);

            int rightBorder = GetRightBorderTwips(cell);
            int rightPadding = ResolvedRightPadding(cell);

            int borderWidth = leftBorder + rightBorder;
            int paddingWidth = leftPadding + rightPadding;

            // Without cell spacing, half of the border is included in the cell minimum
            // (as the other half is inside an adjacent cell or outside the table width if the cell is on the edge).
            // With cell spacing, full border width is included: TestSpacingMin().
            int borderAndPadding = cell.ParentTable.AllowCellSpacing
                ? paddingWidth + borderWidth
                : System.Math.Max(paddingWidth, borderWidth / 2);
            // Strangely enough, the border vs padding handling seems to be different for table width and for cell width.
            // For table, the maximum of border/2 and padding is taken for the left and right side independently, as in GetMaxAllowedWidth() above.
            // For cell, the maximum is taken between the sums of borders and padding, as in this method.

            return borderAndPadding;
        }

        internal static int ResolvedLeftPadding(Cell cell)
        {
            if (cell == null)
                return 0;

            return (int) ((ICellAttrSource)cell).FetchCellAttr(CellAttr.LeftPadding);
        }

        internal static int ResolvedRightPadding(Cell cell)
        {
            if (cell == null)
                return 0;

            return (int) ((ICellAttrSource)cell).FetchCellAttr(CellAttr.RightPadding);
        }

        /// <summary>
        /// Maximum table width.
        /// </summary>
        /// <remarks>
        /// AM. We agreed to do not limit table width to <see cref="ConvertUtilCore.MaxSizeTwip" /> when document is not saved to Word format but
        /// we need another value for limit anyway as very big value might cause GDI throw for example.
        /// I think we can start with value from WORDSNET-12477 (6000pt) and increase it later on demand.
        /// </remarks>
        internal const int MaxTableWidthTwips = (int)(6000 * ConvertUtilCore.TwipsPerPoint);
    }
}
