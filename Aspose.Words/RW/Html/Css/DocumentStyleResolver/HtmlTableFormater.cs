// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2025 by Nikolay Sezganov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to table of the document model.
    /// </summary>
    internal static class HtmlTableFormater
    {
        /// <summary>
        /// Applies calculated CSS styles to a model table.
        /// </summary>
        /// <param name="table">A model table.</param>
        /// <param name="topCaptionCount">Number of caption rows at the top of the table.</param>
        /// <param name="bottomCaptionCount">Number of caption rows at the bottom of the table.</param>
        /// <param name="appliedLeftBorder">Left CSS border of the table.</param>
        /// <param name="appliedRightBorder">Right CSS border of the table.</param>
        /// <param name="cssStyleTracker"></param>
        /// <remarks>Table captions are imported as table rows. Top captions become leading table rows and bottom captions
        /// become trailing rows. To distinguish 'normal' table rows from caption rows we need to know how many top and bottom
        /// caption rows the table contains.</remarks>
        internal static void ToTable(Table table,
            int topCaptionCount,
            int bottomCaptionCount,
            CssBorder appliedLeftBorder,
            CssBorder appliedRightBorder,
            CssStyleTracker cssStyleTracker)
        {
            Debug.Assert(table != null);
            Debug.Assert(topCaptionCount >= 0);
            Debug.Assert(bottomCaptionCount >= 0);

            // Table direction affects table alignment so we apply direction before alignment.
            table.Bidi = cssStyleTracker.IsBlockRtl();

            // Apply the alignment declared by "align" attribute of the table's parent. This alignment can be overridden
            // by "align" attribute on the table itself, which is translated to table's CSS properties, so it is processed
            // before the CSS properties.
            ApplyAlignment(table, cssStyleTracker);
            ApplyBorderSpacing(table, cssStyleTracker.ElementDeclarations);

            ApplyMargin(cssStyleTracker.ElementDeclarations, table);
            foreach (CssDeclaration declaration in cssStyleTracker.ElementDeclarations)
                ((CssComputedDeclaration)declaration).ToTable(table);
            CssWidthStyleConverter.ToTable(
                cssStyleTracker.ElementDeclarations,
                table,
                appliedLeftBorder,
                appliedRightBorder);

            ApplyBackgroundColor(table, new CssBackgroundColor(cssStyleTracker.Background.GetParagraphBackgroundColor()));
        }

        private static void ApplyBackgroundColor(
            Table table,
            CssBackgroundColor backgroundColor)
        {
            Shading shading = new Shading();
            backgroundColor.ToShading(shading);
            if (shading.IsVisible)
            {
                // Word applies shading to every row and every cell.
                foreach (Row row in table.Rows)
                {
                    row.TablePr.Shading = shading.Clone();

                    foreach (Cell cell in row.Cells)
                    {
                        // Preserve cell shading processed before.
                        if (!cell.CellPr.Contains(CellAttr.Shading))
                            cell.CellPr.Shading = shading.Clone();
                    }
                }
            }
        }

        /// <summary>
        /// Applies CSS border-spacing style to table.
        /// </summary>
        private static void ApplyBorderSpacing(
            Table table,
            CssDeclarationCollection declarations)
        {
            double borderSpacingLength = declarations.GetLength("border-spacing");
            double cellSpacing = (!MathUtil.IsMinValue(borderSpacingLength)) ? borderSpacingLength : 0;

            CssDeclaration borderCollapseDeclaration = declarations["border-collapse"];
            bool isBorderSeparate = (borderCollapseDeclaration == null) ||
                                    borderCollapseDeclaration.Value.Equals(CssValue.Separate);

            // WORDSNET-1587 For HTML import, we should set cell spacing only if it is not zero, because
            // in MS Word when zero cell spacing is specified, the table appears differently
            // from the case where cell spacing is not specified at all. Looks like MS Word glitch.
            // WORDSNET-11389 We should set zero cell spacing when 'border-collapsed' value is 'separate'.
            if (!MathUtil.IsZero(cellSpacing) || isBorderSeparate)
            {
                // Divide HTML cell spacing by two since in MS Word a half of cell spacing is stored.
                table.CellSpacing = cellSpacing / 2.0;
            }
        }

        /// <summary>
        /// Applies 'text-align' CSS property of the parent element to a table.
        /// </summary>
        private static void ApplyAlignment(
            Table table,
            CssStyleTracker styleTracker)
        {
            // In HTML, containers with right-to-left direction (for example, <div dir=rtl>) align their child tables
            // to the right edge of the page.
            TableAlignment alignment = (styleTracker.CurrentElementInfo.ParentBlockElement.BlockLevelDirection == CssDirection.Rtl)
                ? TableAlignment.Right
                : TableAlignment.Left;

            CssDeclaration textAlignDeclaration = styleTracker.ParentTextAlign();

            if (textAlignDeclaration != null)
            {
                if (textAlignDeclaration.Value.Equals(CssValue.AwLeft))
                {
                    alignment = TableAlignment.Left;
                }
                else if (textAlignDeclaration.Value.Equals(CssValue.AwRight))
                {
                    alignment = TableAlignment.Right;
                }
                else if (textAlignDeclaration.Value.Equals(CssValue.AwCenter))
                {
                    alignment = TableAlignment.Center;
                }
            }

            alignment = HtmlToWordTableAlignment(alignment, table.Bidi);
            if (table.Alignment != alignment)
            {
                table.Alignment = alignment;
            }
        }

        /// <summary>
        /// Converts HTML table alignment to Word table alignment taking the table direction into account.
        /// </summary>
        /// <remarks>
        /// In the document model, table direction affects table alignment. For example, 'left' means 'the left edge
        /// of the page' for LTR tables but 'right edge of the page' for RTL tables. It is not the case in HTML documents
        /// and we need to adjust HTML table alignment during import taking table direction into account.
        /// </remarks>
        private static TableAlignment HtmlToWordTableAlignment(
            TableAlignment alignment,
            bool isRtlTable)
        {
            TableAlignment result = alignment;
            if (isRtlTable)
            {
                switch (alignment)
                {
                    case TableAlignment.Left:
                        result = TableAlignment.Right;
                        break;
                    case TableAlignment.Right:
                        result = TableAlignment.Left;
                        break;
                    default:
                        // Other alignment values have no opposites.
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Converts CSS property 'margin' or 'margin-xxx' into TableAlignment.
        /// This method goes out of our standard style processing. But that's okay since we
        /// only need alignment on tables.
        /// In the future we should think about any other combinations:
        /// 'auto' on one side can coexist with non-zero on another for instance.
        /// </summary>
        public static void ApplyMargin(
            CssDeclarationCollection declarations,
            Table table)
        {
            bool leftMarginIsAuto = declarations.ContainsIdentifier("margin-left", "auto");
            bool rightMarginIsAuto = declarations.ContainsIdentifier("margin-right", "auto");

            // If both left and right margins are not auto, we shouldn't change the table's alignment, because it is defined
            // by other attributes and properties.
            if (!leftMarginIsAuto && !rightMarginIsAuto)
            {
                return;
            }

            TableAlignment alignment = TableAlignment.Left;
            if (leftMarginIsAuto)
            {
                alignment = rightMarginIsAuto
                    ? TableAlignment.Center
                    : TableAlignment.Right;
            }
            table.Alignment = HtmlToWordTableAlignment(alignment, table.Bidi);
        }
    }
}
