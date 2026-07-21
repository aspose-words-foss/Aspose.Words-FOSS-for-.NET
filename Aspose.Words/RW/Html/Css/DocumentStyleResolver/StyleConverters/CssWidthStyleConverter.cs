// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS width style to a model.
    /// </summary>
    internal static class CssWidthStyleConverter
    {
        /// <summary>
        /// Applies CSS width style to a model table.
        /// </summary>
        internal static void ToTable(
            CssDeclarationCollection declarations,
            Table table,
            CssBorder appliedLeftBorder,
            CssBorder appliedRightBorder)
        {
            CssDeclaration widthDeclaration = declarations["width"];
            if (widthDeclaration == null)
                return;

            PreferredWidth preferredWidth = GetPreferredWidth(widthDeclaration.Value);
            if (preferredWidth == null)
                return;

            if (preferredWidth.IsFixed && preferredWidth.IsPositive)
            {
                // WORDSNET-20289 HTML table width is based on preffered table width and HTML border width.
                // HTML border width may be adjusted during export (see CssBorderWriter.GetBrowserSafeWidth).
                // Also, the original border width may be exported along with CSS border in custom round-trip properties.
                double width = preferredWidth.Value;
                // WORDSNET-21609 Table direction should be taken into account while getting borders.
                bool isRtl = table.FirstRow.TablePr.FinalPr.Bidi;
                Border leftBorder = isRtl
                    ? table.FirstRow.FirstCell.CellFormat.Borders.Right
                    : table.FirstRow.FirstCell.CellFormat.Borders.Left;
                Border rightBorder = isRtl
                    ? table.FirstRow.LastCell.CellFormat.Borders.Left
                    : table.FirstRow.LastCell.CellFormat.Borders.Right;
                width -= GetBorderWidth(leftBorder, appliedLeftBorder) / 2;
                width -= GetBorderWidth(rightBorder, appliedRightBorder) / 2;

                // WORDSNET-9872 We should limit table's width to max allowed value.
                width = MathUtil.FitToRange(width, 0, MaxTableWidth);
                preferredWidth = PreferredWidth.FromPointsSafe(width);
            }
            table.PreferredWidth = preferredWidth;
        }

        /// <summary>
        /// Applies CSS width style to a model cell.
        /// </summary>
        internal static void ToCellFormat(
            CellFormat cellFormat,
            CssDeclarationCollection cellDeclarations,
            CssDeclarationCollection colDeclarations,
            CssDeclarationCollection[] spannedColDeclarations,
            CssBoxBorders appliedBorders,
            bool adjustPaddings,
            bool isBorderCollapsed,
            bool isCellEmpty)
        {
            // WORDSNET-23850 Previously, we added half-width of a border to all paddings. Currently, if borders
            // are collapsed then we add half-width of a border to the left and the right paddings only to mimic MS Word's
            // behavior.
            if (adjustPaddings && isBorderCollapsed)
            {
                SetupPaddings(cellDeclarations, appliedBorders, cellFormat);
            }

            PreferredWidth preferredWidth = GetPreferedCellWidth(
                cellDeclarations,
                colDeclarations,
                spannedColDeclarations,
                isCellEmpty);
            if (preferredWidth == null)
            {
                return;
            }
            if (preferredWidth.Type == PreferredWidthType.Percent)
            {
                cellFormat.PreferredWidth = preferredWidth;
                return;
            }

            double cellWidth = preferredWidth.Value;

            if (adjustPaddings)
            {
                // WORDSNET-9414 Left and right paddings and borders don't affect cell width in MS Word but they
                // increase total cell width in HTML. Here we adjust the HTML cell width accordingly.

                double paddingLeft = GetLength(cellDeclarations["padding-left"]);
                cellWidth += paddingLeft;
                cellWidth += GetBorderWidth(cellFormat.Borders.Left, appliedBorders.Left) / 2;

                double paddingRight = GetLength(cellDeclarations["padding-right"]);
                cellWidth += paddingRight;
                cellWidth += GetBorderWidth(cellFormat.Borders.Right, appliedBorders.Right) / 2;
            }

            if (cellWidth > 0)
            {
                // WORDSNET-9872 We should limit cell's width to max allowed value.
                cellWidth = System.Math.Min(cellWidth, MaxCellWidth);

                cellFormat.PreferredWidth = PreferredWidth.FromPointsSafe(cellWidth);
            }
        }

        internal static PreferredWidth GetPreferredWidth(CssPropertyValue propertyValue)
        {
            if (propertyValue.Count != 1)
                return null;

            PreferredWidth preferredWidth = null;
            CssValue cssValue = propertyValue.FirstValue;
            if (cssValue.ValueType == CssValueType.Percentage)
            {
                double widthPercentRaw = cssValue.DoubleValue;
                if (widthPercentRaw >= 0)
                {
                    widthPercentRaw = PreferredWidth.LimitPercentsToSafeRange(widthPercentRaw);
                    preferredWidth = PreferredWidth.FromPercentSafe(widthPercentRaw);
                }
            }
            else
            {
                double widthRaw = CssUtil.LengthToPoint(cssValue);
                if (widthRaw >= 0)
                {
                    widthRaw = PreferredWidth.LimitPointsToSafeRange(widthRaw);
                    preferredWidth = PreferredWidth.FromPointsSafe(widthRaw);
                }
            }
            return preferredWidth;
        }

        private static PreferredWidth GetPreferedCellWidth(
            CssDeclarationCollection cellDeclarations,
            CssDeclarationCollection colDeclarations,
            CssDeclarationCollection[] spannedColDeclarations,
            bool isCellEmpty)
        {
            // First, try to get width from the cell itself.
            PreferredWidth result = GetPreferredWidth(cellDeclarations, isCellEmpty);
            if (result != null)
            {
                return result;
            }

            // Second, try to get width from the corresponding COL element.
            result = GetPreferredWidth(colDeclarations, isCellEmpty);

            // If the cell spans more than one COL element, try to sum widths specified on all of them.
            if ((result != null) && (spannedColDeclarations != null))
            {
                Debug.Assert(result.Type != PreferredWidthType.Auto);

                int sumValueRaw = result.ValueRaw;
                foreach (CssDeclarationCollection singleSpannedColDeclaration in spannedColDeclarations)
                {
                    PreferredWidth additionalWidth = GetPreferredWidth(singleSpannedColDeclaration, isCellEmpty);
                    if ((additionalWidth == null) || (additionalWidth.Type != result.Type))
                    {
                        // We can sum only compatible widths that have the same type.
                        result = null;
                        break;
                    }
                    sumValueRaw += additionalWidth.ValueRaw;
                }

                // If all widths specified on all COL elements are compatible, return their sum. Otherwise, return no width.
                if (result != null)
                {
                    result = PreferredWidth.FromRawSafe(result.Type, sumValueRaw);
                }
            }

            return result;
        }

        private static PreferredWidth GetPreferredWidth(
            CssDeclarationCollection cssDeclarations,
            bool isCellEmpty)
        {
            PreferredWidth result = null;

            PreferredWidth minWidth = null;
            CssDeclaration minWidthDeclaration = cssDeclarations["min-width"];
            if (minWidthDeclaration != null)
            {
                minWidth = GetPreferredWidth(minWidthDeclaration.Value);
                // "min-width" doesn't work for table cells in browsers when its value is percentage.
                if (minWidth.Type == PreferredWidthType.Percent)
                {
                    minWidth = null;
                }
            }

            // WORDSNET-18622 If a width value is specified, it should be compared with a "min-width" value, 
            // and the result is the maximum value among them.
            // Otherwise, if the cell is empty, the result is the "min-width" value, 
            // and if the cell is not empty, the result is null that'll be converted to "auto" later.
            // This is a limited fix. We effectively ignore "min-width" on non-empty cells, because there is no 
            // counterpart for "min-width" in the document model and the actual width of the cell's contents will 
            // is unknown until the layout stage.
            CssDeclaration cellWidthDeclaration = cssDeclarations["width"];
            if (cellWidthDeclaration != null)
            {
                result = GetPreferredWidth(cellWidthDeclaration.Value);
                if ((result != null) &&
                    (minWidth != null) &&
                    (minWidth.Type == result.Type) &&
                    (result.Value < minWidth.Value))
                {
                    result = minWidth;
                }
            }
            else if (isCellEmpty)
            {
                result = minWidth;
            }

            return result;
        }

        private static void SetupPaddings(
            CssDeclarationCollection cellDeclarations,
            CssBoxBorders appliedBorders,
            CellFormat cellFormat)
        {
            // Table borders are already integrated to cell borders if they are "collapsed"
            // and it is wise to use them.

            // WORDSNET-21609 We should add half-width of a border only if the padding value is greater than 0,
            // because CssTableWriter.GetHtmlCellPadding subtracts border half-width from the padding value during export
            // to HTML only if the half-width is less than the original padding.
            // WORDSNET-23850 Only the left and the right paddings should be adjusted. Vertical padding of cells
            // in MS Word works in the same way as in browsers.

            if ((cellFormat.RightPadding > 0) &&
                (cellDeclarations[HtmlConstants.AsposePaddingRight] == null))
            {
                cellFormat.RightPadding += GetBorderWidth(cellFormat.Borders.Right, appliedBorders.Right) / 2;
            }

            if ((cellFormat.LeftPadding > 0) &&
                (cellDeclarations[HtmlConstants.AsposePaddingLeft] == null))
            {
                cellFormat.LeftPadding += GetBorderWidth(cellFormat.Borders.Left, appliedBorders.Left) / 2;
            }
        }

        private static double GetBorderWidth(Border border, CssBorder appliedBorder)
        {
            if ((border == null) || !border.IsVisible)
            {
                return 0;
            }
            return (appliedBorder != null)
                ? appliedBorder.CssBorderWidth
                : border.BorderWidth;
        }

        private static double GetLength(CssDeclaration paddingDeclaration)
        {
            if (paddingDeclaration == null)
                return 0;

            double length = CssUtil.LengthToPoint(paddingDeclaration.Value);
            if (MathUtil.AreEqual(length, double.MinValue))
                length = 0;
            return length;
        }

        /// <summary>
        /// MS Word allows cell's width to be manually set max to 22 inches
        /// but when importing from HTML it allows max 15 inches.
        /// </summary>
        private const double MaxCellWidth = 15 * 72;

        /// <summary>
        /// MS Word allows table's width to be manually set max to 22 inches
        /// but when importing from HTML it allows max 15 inches.
        /// </summary>
        private const double MaxTableWidth = 15 * 72;
    }
}
