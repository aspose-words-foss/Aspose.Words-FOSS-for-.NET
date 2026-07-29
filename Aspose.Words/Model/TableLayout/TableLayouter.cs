// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2005 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Notes;
using Aspose.Words.Tables;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Calculates and updates table cell widths accordingly to the preferred widths.
    /// 
    /// This is DV's implementation of table layout that was done for old PDF and HTML export,
    /// before the layout engine. At the moment it has no relation to the layout engine.
    /// It should be deprecated some day, because it cannot calculate table widths exactly 
    /// like MS Word in some cases and this is by design. E.g. to calculate exactly like MS Word 
    /// we must perform proper table layout in the layout engine.
    /// </summary>
    internal class TableLayouter
    {
        /// <summary>
        /// Ctor.
        /// </summary> 
        internal TableLayouter(Table table)
        {
            if (table.ParentNode == null)
                throw new InvalidOperationException("The table must be in the document tree before it can be updated.");

            mTable = table;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="suppressNestedTablesUpdates">Controls whether to suppress layout update of nested tables at the end of layout process.</param>
        private TableLayouter(Table table, bool suppressNestedTablesUpdates)
            : this(table)
        {
            mSuppressNestedTablesUpdates = suppressNestedTablesUpdates;
        }

        /// <summary>
        /// Launches the layout process.
        /// </summary>
        internal LayoutWidth DoLayout()
        {
            // We should not update cell widths if the table AutoFit is disabled.
            // But we must do this, because it collects nested tables and nested tables might have AutoFit enabled and they need to be resized.
            CalculateLayoutWidths();

            // Do not resize (and do not have to perform further calculations) for tables that have AutoFit off.
            if (mTable.AllowAutoFit)
            {
                CalculateTableContainerWidth();
                CalculateTableSpecifiedWidth();
                CalculateResultingWidths();

                SetCellWidths();
            }

            // WORDSNET-6423 The problem occurs because there was a lot of useless layout updates for each nested table.
            // To fix this issue we suppress the useless layout updates for the nested tables.
            // There is one top-level cascade layout update for the nested tables now.
            if (!mSuppressNestedTablesUpdates)
            {
                // We must do this because nested tables might have AutoFit enabled and need to be resized.
                ProcessNestedTables();
            }

            return mTableLayoutWidth;
        }

        /// <summary>
        /// Calculates widths required for further processing.
        /// </summary>
        private void CalculateLayoutWidths()
        {
            if (!mAreLayoutWidthsDirty)
                return;

            InitializeLayout();
            CalculateColumnLayoutWidths();
            CalculateEffectiveWidth();
            // Dmatv: per WORDSNET-4794 I made an experiment to update this legacy table layouter logic to handle cell spacing.
            // Adding cell spacing (halfSpacing * 2) to EffectiveLayoutWidth.Min and .Max for all columns appears to work in most cases.
            // However, the logic was disabled in order to preserve the original behavior
            // for tables not handled by the new grid construction logic.
            // The reason is that those tables are often not handled correctly by this legacy table layouter either,
            // and replacing one incorrect layout (without spacing) with a different incorrect layout (with spacing)
            // can be perceived as breakage.
            // The correct way to handle spacing is to support a table via the new table grid calculation logic.
            CalculateTableLayoutWidth();

            mAreLayoutWidthsDirty = false;
        }

        /// <summary>
        /// Initializes the layout object.
        /// </summary>
        private void InitializeLayout()
        {
            mParagraphMeasurer = new ParagraphMeasurer();

            mSpanCells = new List<SpanCell>();
            mNestedTables = new List<Table>();

            mFixedContributor = null;
            mMaxContributor = null;

            mHasPercent = false;

            mTableLayoutWidth = new LayoutWidth();

            mAutoCount = 0;
            mFixedCount = 0;
            mPercentCount = 0;

            // Create an array of columns.
            mColumns = new Column[mTable.GetColumnCount()];
            // Initialize the array.
            for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                mColumns[columnIndex] = new Column();
        }

        /// <summary>
        /// Calculates layout and preferred widths for each column.
        /// </summary>
        private void CalculateColumnLayoutWidths()
        {
            for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
            {
                Column column = mColumns[columnIndex];

                for (int rowIndex = 0; rowIndex < mTable.Rows.Count; rowIndex++)
                {
                    Cell cell = GetCellUsingGridSpans(mTable, columnIndex, rowIndex);

                    if (cell == null)
                        continue;

                    CellFormat cellFormat = cell.CellFormat;

                    // Don't waste time.
                    if (cellFormat.VerticalMerge == CellMerge.Previous)
                        continue;

                    switch (cellFormat.HorizontalMerge)
                    {
                        case CellMerge.None:
                        case CellMerge.First:
                            column.EnsureColumnHasLayoutWidth();

                            if (Extensions.GetHorizontalMergeSpan(cell) > 1)
                                ContributeMergedCell(cell, columnIndex);
                            else
                                ContributeNonMergedCell(cell, column);

                            break;
                        case CellMerge.Previous:
                            // Skip these cells.
                            continue;
                        default:
                            throw new InvalidOperationException("Unknown cell merge.");
                    }
                }

                if (column.PreferredWidth.IsFixed && (column.LayoutWidth.Max > column.PreferredWidth.Value) && (mFixedContributor != mMaxContributor))
                {
                    column.PreferredWidth = PreferredWidth.Auto;
                    mFixedContributor = null;
                }

                column.LayoutWidth.Max = System.Math.Max(column.LayoutWidth.Min, column.LayoutWidth.Max);
            }
        }

        private void ContributeMergedCell(Cell cell, int columnIndex)
        {
            mSpanCells.Add(new SpanCell(cell, columnIndex));
        }

        private void ContributeNonMergedCell(Cell cell, Column column)
        {
            LayoutWidth cellLayoutWidth = CalculateCellLayoutWidth(cell);

            if (cellLayoutWidth.Max > column.LayoutWidth.Max)
                mMaxContributor = cell;

            column.LayoutWidth.Update(cellLayoutWidth);

            PreferredWidth cellPreferredWidth = cell.CellFormat.PreferredWidth;
            switch (cellPreferredWidth.Type)
            {
                case PreferredWidthType.Points:
                {
                    if ((cellPreferredWidth.IsPositive) && !column.PreferredWidth.IsPercent)
                    {
                        double totalCellWidth = cellPreferredWidth.Value + cell.CellFormat.LeftPadding + cell.CellFormat.RightPadding;

                        if (column.PreferredWidth.IsFixed)
                        {
                            if ((totalCellWidth > column.PreferredWidth.Value) || ((totalCellWidth > column.PreferredWidth.Value) && (cell == mMaxContributor)))
                            {
                                column.PreferredWidth = PreferredWidth.FromPointsSafe(totalCellWidth);
                                mFixedContributor = cell;
                            }
                        }
                        else
                        {
                            column.PreferredWidth = PreferredWidth.FromPointsSafe(totalCellWidth);
                            mFixedContributor = cell;
                        }
                    }

                    break;
                }
                case PreferredWidthType.Percent:
                {
                    mHasPercent = true;

                    if ((cellPreferredWidth.IsPositive) &&
                        (!column.PreferredWidth.IsPercent || (cellPreferredWidth.Value > column.PreferredWidth.Value)))
                    {
                        column.PreferredWidth = PreferredWidth.FromPercentSafe(cellPreferredWidth.Value);
                    }

                    break;
                }
                default:
                    break;
            }
        }

        private void CalculateEffectiveWidth()
        {
            PrepareToCalculateEffectiveWidth();

            for (int cellIndex = 0; cellIndex < mSpanCells.Count; cellIndex++)
            {
                SpanContext context = new SpanContext();

                SpanCell spanCell = mSpanCells[cellIndex];
                Cell cell = spanCell.Cell;

                context.Span = spanCell.Span;
                context.SpanIndex = spanCell.ColumnIndex;
                context.LastColumnIndex = context.SpanIndex;

                context.CellPreferredWidth = cell.CellFormat.PreferredWidth;

                if (!context.CellPreferredWidth.IsPositive)
                    context.CellPreferredWidth = PreferredWidth.Auto;

                context.CellLayoutWidth = CalculateCellLayoutWidth(cell);

                // These 2 should be disabled when spacing is handled by the Extensions method correctly:
                context.CellLayoutWidth.Min += mTable.CellSpacing;
                context.CellLayoutWidth.Max += mTable.CellSpacing;

                context.SpanLayoutWidth = new LayoutWidth();

                CollectSpanInformation(context);
                AdjustTableMaxWidth(context);
                SatisfySpanCellLayoutWidth(context);
                AdjustColumnMaxWidth(context);
            }
        }

        private void PrepareToCalculateEffectiveWidth()
        {
            mSpanMaxWidth = 0;

            mSpanCells.Sort(new SpanComparer());

            for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                mColumns[columnIndex].InitializeEffectiveWidth();
        }

        private void CollectSpanInformation(SpanContext context)
        {
            while ((context.LastColumnIndex < mColumns.Length) && (context.Span > 0))
            {
                Column lastColumn = mColumns[context.LastColumnIndex];

                switch (lastColumn.PreferredWidth.Type)
                {
                    case PreferredWidthType.Auto:
                    {
                        context.HasAuto = true;
                        context.AllColumnsAreFixed = false;
                        context.AllColumnsArePercent = false;

                        lastColumn.EffectivePreferredWidth = PreferredWidth.Auto;
                        break;
                    }
                    case PreferredWidthType.Points:
                    {
                        if (lastColumn.PreferredWidth.IsPositive)
                        {
                            context.TotalFixed += lastColumn.PreferredWidth.Value;
                            context.AllColumnsArePercent = false;
                        }
                        else
                        {
                            context.HasAuto = true;
                            context.AllColumnsAreFixed = false;
                            context.AllColumnsArePercent = false;

                            lastColumn.EffectivePreferredWidth = PreferredWidth.Auto;
                        }
                        break;
                    }
                    case PreferredWidthType.Percent:
                    {
                        context.TotalPercent += lastColumn.PreferredWidth.Value;
                        context.AllColumnsAreFixed = false;

                        break;
                    }
                    default:
                        throw new InvalidOperationException("Unknown length type.");
                }

                context.SpanLayoutWidth.Min += lastColumn.EffectiveLayoutWidth.Min;
                context.SpanLayoutWidth.Max += lastColumn.EffectiveLayoutWidth.Max;

                // These 2 should be disabled if Extensions class method returning the correct spacing are used :
                context.CellLayoutWidth.Min -= mTable.CellSpacing;
                context.CellLayoutWidth.Max -= mTable.CellSpacing;

                context.Span--;

                context.LastColumnIndex++;
            }
        }

        private void AdjustTableMaxWidth(SpanContext context)
        {
            if (context.CellPreferredWidth.IsPercent)
            {
                if ((context.TotalPercent > context.CellPreferredWidth.Value) || (context.AllColumnsArePercent))
                {
                    // Can't satisfy this condition, treat as auto.
                    context.CellPreferredWidth = PreferredWidth.Auto;
                }
                else
                {
                    double spanMaxWidth = System.Math.Max(context.SpanLayoutWidth.Max, context.CellLayoutWidth.Max);
                    mSpanMaxWidth = System.Math.Max(mSpanMaxWidth, spanMaxWidth * 100 / context.CellPreferredWidth.Value);

                    // All non percent columns in the span get percent values to sum up correctly.

                    double percentMissing = context.CellPreferredWidth.Value - context.TotalPercent;
                    double totalWidth = 0;

                    for (int columnIndex = context.SpanIndex; columnIndex < context.LastColumnIndex; columnIndex++)
                    {
                        Column column = mColumns[columnIndex];

                        if (!column.PreferredWidth.IsPercent)
                            totalWidth += column.EffectiveLayoutWidth.Max;
                    }

                    for (int columnIndex = context.SpanIndex; (columnIndex < context.LastColumnIndex) && (totalWidth > 0); columnIndex++)
                    {
                        Column column = mColumns[columnIndex];

                        if (!column.PreferredWidth.IsPercent)
                        {
                            int percent = (int)(percentMissing * column.EffectiveLayoutWidth.Max / totalWidth);

                            totalWidth -= column.EffectiveLayoutWidth.Max;
                            percentMissing -= percent;

                            if (percent > 0)
                                column.EffectivePreferredWidth = PreferredWidth.FromPercentSafe(percent);
                            else
                                column.EffectivePreferredWidth = PreferredWidth.Auto;
                        }
                    }
                }
            }
        }

        private void SatisfySpanCellLayoutWidth(SpanContext context)
        {
            if (context.CellLayoutWidth.Min > context.SpanLayoutWidth.Min)
            {
                if (context.AllColumnsAreFixed)
                {
                    for (int columnIndex = context.SpanIndex; (columnIndex < context.LastColumnIndex) && (context.TotalFixed > 0); columnIndex++)
                    {
                        Column column = mColumns[columnIndex];

                        double newWidth = System.Math.Max(
                            column.EffectiveLayoutWidth.Min,
                            context.CellLayoutWidth.Min * column.PreferredWidth.Value / context.TotalFixed);

                        context.TotalFixed -= column.PreferredWidth.Value;
                        context.CellLayoutWidth.Min -= newWidth;

                        column.EffectiveLayoutWidth.Min = newWidth;
                    }
                }
                else
                {
                    double minWidth = context.SpanLayoutWidth.Min;
                    double maxWidth = context.SpanLayoutWidth.Max;

                    for (
                        int columnIndex = context.SpanIndex;
                        (columnIndex < context.LastColumnIndex) && (maxWidth > 0);
                        columnIndex++)
                    {
                        Column column = mColumns[columnIndex];

                        if (column.PreferredWidth.IsFixed && context.HasAuto && (context.TotalFixed <= context.CellLayoutWidth.Min))
                        {
                            double newWidth = System.Math.Max(column.EffectiveLayoutWidth.Min, column.PreferredWidth.Value);

                            context.TotalFixed -= column.PreferredWidth.Value;
                            minWidth -= column.EffectiveLayoutWidth.Min;
                            maxWidth -= column.EffectiveLayoutWidth.Max;
                            context.CellLayoutWidth.Min -= newWidth;

                            column.EffectiveLayoutWidth.Min = newWidth;
                        }
                    }

                    for (
                        int columnIndex = context.SpanIndex;
                        (columnIndex < context.LastColumnIndex) && (minWidth < context.CellLayoutWidth.Min) && (maxWidth > 0);
                        columnIndex++)
                    {
                        Column column = mColumns[columnIndex];

                        if (!(column.PreferredWidth.IsFixed && context.HasAuto && (context.TotalFixed <= context.CellLayoutWidth.Min)))
                        {
                            double newWidth = System.Math.Max(
                                column.EffectiveLayoutWidth.Min,
                                context.CellLayoutWidth.Min * column.EffectiveLayoutWidth.Max / maxWidth);

                            newWidth = System.Math.Min(newWidth, column.EffectiveLayoutWidth.Min + (context.CellLayoutWidth.Min - minWidth));

                            minWidth -= column.EffectiveLayoutWidth.Min;
                            maxWidth -= column.EffectiveLayoutWidth.Max;
                            context.CellLayoutWidth.Min -= newWidth;

                            column.EffectiveLayoutWidth.Min = newWidth;
                        }
                    }
                }
            }
        }

        private void AdjustColumnMaxWidth(SpanContext context)
        {
            if (!context.CellPreferredWidth.IsPercent)
            {
                if (context.CellLayoutWidth.Max > context.SpanLayoutWidth.Max)
                {
                    for (int columnIndex = context.SpanIndex; (columnIndex < context.LastColumnIndex) && (context.SpanLayoutWidth.Max > 0); columnIndex++)
                    {
                        Column column = mColumns[columnIndex];

                        double newWidth = System.Math.Max(
                            column.EffectiveLayoutWidth.Max,
                            context.CellLayoutWidth.Max * column.EffectiveLayoutWidth.Max / context.SpanLayoutWidth.Max);

                        context.SpanLayoutWidth.Max -= column.EffectiveLayoutWidth.Max;
                        context.CellLayoutWidth.Max -= newWidth;

                        column.EffectiveLayoutWidth.Max = newWidth;
                    }
                }
            }
            else
            {
                for (int columnIndex = context.SpanIndex; columnIndex < context.LastColumnIndex; columnIndex++)
                {
                    Column column = mColumns[columnIndex];

                    column.LayoutWidth.Max = System.Math.Max(column.LayoutWidth.Min, column.LayoutWidth.Max);
                }
            }
        }

        /// <summary>
        /// Calculates table layout width.
        /// </summary>
        private void CalculateTableLayoutWidth()
        {
            double maxPercent = 0;
            double maxNonPercent = 0;

            double remainingPercent = 100;

            for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
            {
                Column column = mColumns[columnIndex];

                mTableLayoutWidth.Min += column.EffectiveLayoutWidth.Min;
                mTableLayoutWidth.Max += column.EffectiveLayoutWidth.Max;

                if (column.EffectivePreferredWidth.IsPercent)
                {
                    double percent = System.Math.Min(column.EffectivePreferredWidth.Value, remainingPercent);
                    double pw = column.EffectiveLayoutWidth.Max * 100 / System.Math.Max(percent, 1);

                    remainingPercent -= percent;
                    maxPercent = System.Math.Max(maxPercent, pw);
                }
                else
                {
                    maxNonPercent += column.EffectiveLayoutWidth.Max;
                }
            }

            if (ShouldScaleColumns())
            {
                maxNonPercent = (maxNonPercent * 100 + 50) / System.Math.Max(remainingPercent, 1);
                mTableLayoutWidth.Max = System.Math.Max(mTableLayoutWidth.Max, maxNonPercent);
                mTableLayoutWidth.Max = System.Math.Max(mTableLayoutWidth.Max, maxPercent);
            }

            mTableLayoutWidth.Max = System.Math.Max(mTableLayoutWidth.Max, mSpanMaxWidth);

            double bs = Extensions.GetTotalBorderAndSpacingWidth(mTable);
            mTableLayoutWidth.Min += bs;
            mTableLayoutWidth.Max += bs;

            if (mTable.PreferredWidth.IsFixed && mTable.PreferredWidth.IsPositive)
            {
                mTableLayoutWidth.Min = System.Math.Max(mTableLayoutWidth.Min, mTable.PreferredWidth.Value);
                mTableLayoutWidth.Max = mTableLayoutWidth.Min;
            }

            // Limit table width to the value allowed by Word.
            mTableLayoutWidth.Min = System.Math.Min(mTableLayoutWidth.Min, MaxTableWidth);
            mTableLayoutWidth.Max = System.Math.Min(mTableLayoutWidth.Max, MaxTableWidth);
        }

        private bool ShouldScaleColumns()
        {
            bool result = true;

            Table table = mTable;

            while (table != null)
            {
                PreferredWidth tableWidth = table.PreferredWidth;

                if (!tableWidth.IsFixed)
                {
                    Node container = mTable.ParentNode;

                    while ((container != null) && (container.NodeType != NodeType.Cell))
                        container = container.ParentNode;

                    table = null;

                    if ((container != null) && (container.NodeType == NodeType.Cell))
                    {
                        Cell cell = (Cell)container;
                        PreferredWidth cellWidth = cell.CellFormat.PreferredWidth;

                        if (!cellWidth.IsFixed)
                        {
                            if (tableWidth.IsPercent)
                            {
                                result = false;
                            }
                            else
                            {
                                if ((Extensions.GetHorizontalMergeSpan(cell) > 1) || cell.ParentTable.PreferredWidth.IsAuto)
                                    result = false;
                                else
                                    table = cell.ParentTable;
                            }
                        }
                    }
                }
                else
                {
                    table = null;
                }
            }

            return result;
        }

        internal static double GetContainerWidth(CompositeNode tableContainer)
        {
            double containerWidth;

            Section section = (Section)tableContainer.GetAncestor(NodeType.Section);

            switch (tableContainer.NodeType)
            {
                case NodeType.Body:
                {
                    containerWidth = GetMinimalColumnWidth(section);
                    break;
                }
                case NodeType.Cell:
                {
                    Cell cell = (Cell)tableContainer;
                    containerWidth = cell.CellFormat.Width;
                    break;
                }
                case NodeType.HeaderFooter:
                case NodeType.Comment:
                {
                    containerWidth = section.PageSetup.ContentWidth;
                    break;
                }
                case NodeType.Footnote:
                {
                    Footnote note = (Footnote)tableContainer;
                    bool useLastSectionWidth = (note.FootnoteType == FootnoteType.Endnote) &&
                        (section.PageSetup.EndnoteOptions.Position == EndnotePosition.EndOfDocument);
                    if (useLastSectionWidth)
                        section = section.FetchDocument().LastSection;

                    containerWidth = section.PageSetup.ContentWidth;
                    break;
                }
                case NodeType.Shape:
                {
                    Shape shape = (Shape)tableContainer;
                    ShapeAdaptor shapeAdaptor = new ShapeAdaptor(shape);
                    CompositeNode parentStory = null;
                    if (shapeAdaptor.WrapMode == TextBoxWrapMode.None)
                    {
                        // Word wrap OFF, the text box may grow to accommodate the contents.
                        // Use the container story width. This should be section body or header/footer.
                        // The other containers (footnotes, comments, text boxes) must not have text boxes inside.
                        parentStory = GetBodyOrHeaderAncestor(shape);
                        // It should not be null, but it is when fall-back shapes are validated (TestLayoutTableGridFixed2013.TestTblwPctInTextBoxNested).
                        // The code below will use the text box width instead.
                    }

                    if (parentStory != null)
                    {
                        containerWidth = GetContainerWidth(parentStory);
                    }
                    else
                    {
                        // Word wrap ON, the text box does not grow to accommodate the contents.
                        // Use the text box width.
                        containerWidth = shapeAdaptor.ContentWidth;
                    }

                    break;
                }
                case NodeType.System:
                    if (tableContainer is FootnoteSeparator)
                    {
                        // WORDSNET-13165 tables can be present in separators.

                        // Actually, the same separator may be used in different sections.
                        // But it seems MS Word always uses the first section width,
                        // even if there are no footnotes in the first section (TestJira13165TableInSeparator).
                        section = tableContainer.FetchDocument().FirstSection;
                        containerWidth = section.PageSetup.ContentWidth;
                        // The value is currently not used by the layout anyway as separators are not traversed by the validator,
                        // so the fixed grid is never built and because of that fall-back grid is always used by the layout.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected table container.");
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unexpected table container.");
            }

            return containerWidth;
        }

        private static CompositeNode GetBodyOrHeaderAncestor(Node node)
        {
            CompositeNode ancestor = node.ParentNode;
            while ((ancestor != null) && (ancestor.NodeType != NodeType.Body) && (ancestor.NodeType != NodeType.HeaderFooter))
            {
                ancestor = ancestor.ParentNode;
            }

            return ancestor;
        }

        /// <summary>
        /// Gets minimal column width for the given section, in points.
        /// </summary>
        private static double GetMinimalColumnWidth(Section section)
        {
            PageSetup pageSetup = section.PageSetup;

            Document doc = section.FetchDocumentOrGlossaryMain();
            bool hasSideGutter = PageMarginCalculator.GetPageHasSideGutter(section.SectPr, doc.DocPr, doc.FirstSection.SectPr.Orientation);

            // Width available for columns on the page.
            int availableWidthTwips = section.SectPr.GetContentWidthTwips(hasSideGutter);
            double minimalColumnWidth = ConvertUtilCore.TwipToPoint(availableWidthTwips);

            TextColumnCollection columns = pageSetup.TextColumns;
            // WORDSNET-22176 Even if there is only one column,
            // it may still have a width specified which is different from the available width between margins and gutter.
            if (columns == null || columns.Count < 1)
                return minimalColumnWidth;

            if (section.SectPr.ColumnsEvenlySpaced)
            {
                // Width available for columns on the page.
                availableWidthTwips = section.SectPr.GetTotalEvenlySpacedColumnSizeTwips(hasSideGutter);
                minimalColumnWidth = ConvertUtilCore.TwipToPoint(availableWidthTwips / columns.Count);
            }
            else
            {
                for (int i = 0; i < columns.Count; ++i)
                {
                    double currentColumnWidth = columns[i].Width;
                    if (currentColumnWidth < minimalColumnWidth)
                    {
                        minimalColumnWidth = currentColumnWidth;
                    }
                }
            }

            return minimalColumnWidth;
        }

        /// <summary>
        /// Calculates available content width of the table's container.
        /// </summary>
        private void CalculateTableContainerWidth()
        {
            mTableContainerWidth = GetContainerWidth(mTable.FirstNonMarkupParentNode) - mTable.LeftIndent;
            // Add magical 15 twips for tables with percent tblw in shapes.
            mTableContainerWidth += ConvertUtilCore.TwipToPoint(Extensions.GetShapeContainerWidthAdjustmentTwips(mTable));
            // It is no longer included in GetContainerWidth() as it does not apply to tables with auto width.

            mTableContainerWidth = System.Math.Min(mTableContainerWidth, ConvertUtilCore.TwipToPoint(Extensions.MaxTableWidthTwips));
        }

        /// <summary>
        /// Calculates the absolute width of the table in points.
        /// </summary>
        private void CalculateTableSpecifiedWidth()
        {
            PreferredWidth preferredWidth = mTable.PreferredWidth;

            switch (preferredWidth.Type)
            {
                case PreferredWidthType.Auto:
                {
                    mMarginsWidth = System.Math.Min(mTableContainerWidth, mTableLayoutWidth.Max);
                    break;
                }
                case PreferredWidthType.Points:
                {
                    if (preferredWidth.IsPositive)
                        mMarginsWidth = preferredWidth.Value;
                    else
                        mMarginsWidth = System.Math.Min(mTableContainerWidth, mTableLayoutWidth.Max);

                    break;
                }
                case PreferredWidthType.Percent:
                {
                    if (preferredWidth.IsPositive)
                        mMarginsWidth = mTableContainerWidth * preferredWidth.Value / 100;
                    else
                        mMarginsWidth = System.Math.Min(mTableContainerWidth, mTableLayoutWidth.Max);

                    break;
                }
                default:
                {
                    throw new InvalidOperationException("Unknown length type.");
                }
            }

            mTableSpecifiedWidth = System.Math.Max(mMarginsWidth, mTableLayoutWidth.Min);
        }

        /// <summary>
        /// The essential method which calculates the resulting column width which are then used
        /// for setting width of the cells.
        /// </summary>
        private void CalculateResultingWidths()
        {
            // Compute several widths required by further calculations.

            PrepareWidths();

            // Allocate the desired widths to the columns.

            AllocateToPercent();
            AllocateToFixed();
            AllocateToAuto();

            // If we have an extra space, distribute it among the columns.

            SpreadOverPercent();
            SpreadOverFixed();
            SpreadOverRemainder();

            // If we have over allocated, reduce every cell according to the difference between desired width
            // and min width.
            if (mAvailableWidth < 0)
            {
                ReduceAuto();
                ReduceFixed();
                ReducePercent();
            }

            // WORDSNET-7805 Round widths of columns to whole numbers of twips. Further processing of columns is done
            // in whole twips, and so refusing to round the widths leads to misaligned columns and unstable layout due to 
            // accumulation of fractional width parts.
            RoundWidthsToWholeTwips();
        }

        private void PrepareWidths()
        {
            mAvailableWidth = mTableSpecifiedWidth - Extensions.GetTotalBorderAndSpacingWidth(mTable);

            for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
            {
                Column column = mColumns[columnIndex];

                column.EffectiveLayoutWidth.Max = System.Math.Max(column.EffectiveLayoutWidth.Min, column.EffectiveLayoutWidth.Max);

                // Fill up every cell with its min width.
                column.CalculatedWidth = column.EffectiveLayoutWidth.Min;
                mAvailableWidth -= column.EffectiveLayoutWidth.Min;

                // Count widths necessary for further allocations.
                switch (column.EffectivePreferredWidth.Type)
                {
                    case PreferredWidthType.Auto:
                        mAutoCount++;
                        mTotalAutoMin += column.EffectiveLayoutWidth.Min;
                        mTotalAutoMax += column.EffectiveLayoutWidth.Max;
                        break;
                    case PreferredWidthType.Points:
                        mFixedCount++;
                        mTotalFixedMax += column.EffectiveLayoutWidth.Max;
                        break;
                    case PreferredWidthType.Percent:
                        mPercentCount++;
                        mTotalPercent += column.EffectivePreferredWidth.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        private void AllocateToPercent()
        {
            if ((mAvailableWidth) > 0 && (mPercentCount > 0))
            {
                double tableWidth = mTableSpecifiedWidth - Extensions.GetTotalBorderAndSpacingWidth(mTable);

                for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                {
                    Column column = mColumns[columnIndex];
                    PreferredWidth preferredWidth = column.EffectivePreferredWidth;

                    if (preferredWidth.IsPercent)
                    {
                        double newWidth = System.Math.Max(column.EffectiveLayoutWidth.Min, preferredWidth.Value * tableWidth / 100);
                        mAvailableWidth += column.CalculatedWidth - newWidth;
                        column.CalculatedWidth = newWidth;
                    }
                }
            }
        }

        private void AllocateToFixed()
        {
            if (mAvailableWidth > 0)
            {
                for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                {
                    Column column = mColumns[columnIndex];
                    PreferredWidth preferredWidth = column.EffectivePreferredWidth;

                    if ((preferredWidth.IsFixed) && (preferredWidth.Value > column.CalculatedWidth))
                    {
                        mAvailableWidth += column.CalculatedWidth - preferredWidth.Value;
                        column.CalculatedWidth = preferredWidth.Value;
                    }
                }
            }
        }

        private void AllocateToAuto()
        {
            if ((mAvailableWidth) > 0 && (mAutoCount > 0))
            {
                mAvailableWidth += mTotalAutoMin;

                for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                {
                    Column column = mColumns[columnIndex];
                    PreferredWidth preferredWidth = column.EffectivePreferredWidth;

                    if ((preferredWidth.IsAuto) && (mTotalAutoMax != 0))
                    {
                        double newWidth = System.Math.Max(
                            column.CalculatedWidth,
                            mAvailableWidth * column.EffectiveLayoutWidth.Max / mTotalAutoMax);

                        mAvailableWidth -= newWidth;
                        mTotalAutoMax -= column.EffectiveLayoutWidth.Max;
                        column.CalculatedWidth = newWidth;
                    }
                }
            }
        }

        private void SpreadOverPercent()
        {
            if ((mAvailableWidth > 0) && (mHasPercent) && (mTotalPercent < 100))
            {
                for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                {
                    Column column = mColumns[columnIndex];
                    PreferredWidth preferredWidth = column.EffectivePreferredWidth;

                    if (preferredWidth.IsPercent)
                    {
                        double newWidth = mAvailableWidth * preferredWidth.Value / mTotalPercent;

                        mAvailableWidth -= newWidth;
                        mTotalPercent -= preferredWidth.Value;
                        column.CalculatedWidth += newWidth;

                        if ((mAvailableWidth == 0) || (mTotalPercent == 0))
                            break;
                    }
                }
            }
        }

        private void SpreadOverFixed()
        {
            if ((mAvailableWidth > 0) && (mFixedCount > 0))
            {
                for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                {
                    Column column = mColumns[columnIndex];
                    PreferredWidth preferredWidth = column.EffectivePreferredWidth;

                    if (preferredWidth.IsFixed)
                    {
                        double newWidth = mAvailableWidth * column.EffectiveLayoutWidth.Max / mTotalFixedMax;

                        mAvailableWidth -= newWidth;
                        mTotalFixedMax -= column.EffectiveLayoutWidth.Max;
                        column.CalculatedWidth += newWidth;
                    }
                }
            }
        }

        private void SpreadOverRemainder()
        {
            if (mAvailableWidth > 0)
            {
                int columnsTotal = mColumns.Length;
                int columnIndex = columnsTotal;

                while (columnIndex-- > 0)
                {
                    double newWidth = mAvailableWidth / columnsTotal;
                    mAvailableWidth -= newWidth;
                    columnsTotal--;

                    mColumns[columnIndex].CalculatedWidth += newWidth;
                }
            }
        }

        private void ReduceAuto()
        {
            if (mAvailableWidth < 0)
            {
                double widthToReduce = 0;

                for (int columnIndex = mColumns.Length - 1; columnIndex >= 0; columnIndex--)
                {
                    Column column = mColumns[columnIndex];

                    if (column.EffectivePreferredWidth.IsAuto)
                        widthToReduce += column.CalculatedWidth - column.EffectiveLayoutWidth.Min;
                }

                for (int columnIndex = mColumns.Length - 1; columnIndex >= 0 && widthToReduce > 0; columnIndex--)
                {
                    Column column = mColumns[columnIndex];

                    if (column.EffectivePreferredWidth.IsAuto)
                    {
                        double minMaxDiff = column.CalculatedWidth - column.EffectiveLayoutWidth.Min;
                        double reduce = mAvailableWidth * minMaxDiff / widthToReduce;

                        column.CalculatedWidth += reduce;
                        mAvailableWidth -= reduce;
                        widthToReduce -= minMaxDiff;

                        if (mAvailableWidth >= 0)
                            break;
                    }
                }
            }
        }

        private void ReduceFixed()
        {
            if (mAvailableWidth < 0)
            {
                double widthToReduce = 0;

                for (int columnIndex = mColumns.Length - 1; columnIndex >= 0; columnIndex--)
                {
                    Column column = mColumns[columnIndex];

                    if (column.EffectivePreferredWidth.IsFixed)
                        widthToReduce += column.CalculatedWidth - column.EffectiveLayoutWidth.Min;
                }

                for (int columnIndex = mColumns.Length - 1; columnIndex >= 0 && widthToReduce > 0; columnIndex--)
                {
                    Column column = mColumns[columnIndex];

                    if (column.EffectivePreferredWidth.IsFixed)
                    {
                        double minMaxDiff = column.CalculatedWidth - column.EffectiveLayoutWidth.Min;
                        double reduce = mAvailableWidth * minMaxDiff / widthToReduce;

                        column.CalculatedWidth += reduce;
                        mAvailableWidth -= reduce;
                        widthToReduce -= minMaxDiff;

                        if (mAvailableWidth >= 0)
                            break;
                    }
                }
            }
        }

        private void ReducePercent()
        {
            if (mAvailableWidth < 0)
            {
                double widthToReduce = 0;

                for (int columnIndex = mColumns.Length - 1; columnIndex >= 0; columnIndex--)
                {
                    Column column = mColumns[columnIndex];

                    if (column.EffectivePreferredWidth.IsPercent)
                        widthToReduce += column.CalculatedWidth - column.EffectiveLayoutWidth.Min;
                }

                for (int columnIndex = mColumns.Length - 1; columnIndex >= 0 && widthToReduce > 0; columnIndex--)
                {
                    Column column = mColumns[columnIndex];

                    if (column.EffectivePreferredWidth.IsPercent)
                    {
                        double minMaxDiff = column.CalculatedWidth - column.EffectiveLayoutWidth.Min;
                        double reduce = mAvailableWidth * minMaxDiff / widthToReduce;

                        column.CalculatedWidth += reduce;
                        mAvailableWidth -= reduce;
                        widthToReduce -= minMaxDiff;

                        if (mAvailableWidth >= 0)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Rounds calculated column widths to whole numbers of twips.
        /// </summary>
        private void RoundWidthsToWholeTwips()
        {
            for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
            {
                Column column = mColumns[columnIndex];

                // Conversion from points to twips implies rounding.
                double roundedWidthInTwips = ConvertUtilCore.PointToTwip(column.CalculatedWidth);

                column.CalculatedWidth = ConvertUtilCore.TwipToPoint(roundedWidthInTwips);
            }
        }

        /// <summary>
        /// Calculates the cell layout width.
        /// </summary>
        private LayoutWidth CalculateCellLayoutWidth(Cell cell)
        {
            LayoutWidth layoutWidth = new LayoutWidth();

            foreach (Node childNode in cell.GetChildNodes(NodeType.Any, false))
                CalculateChildNode(childNode, layoutWidth);

            layoutWidth.Max = System.Math.Max(layoutWidth.Min, layoutWidth.Max);

            PreferredWidth cellPreferredWidth = cell.CellFormat.PreferredWidth;

            if (cellPreferredWidth.IsFixed && cellPreferredWidth.IsPositive)
                layoutWidth.Max = System.Math.Max(layoutWidth.Min, cellPreferredWidth.Value);

            layoutWidth.Add(Extensions.GetPaddingAndBorderWidth(cell));

            return layoutWidth;
        }

        private void CalculateChildNode(Node childNode, LayoutWidth layoutWidth)
        {
            switch (childNode.NodeType)
            {
                case NodeType.Paragraph:
                {
                    Paragraph paragraph = (Paragraph)childNode;
                    layoutWidth.Update(mParagraphMeasurer.GetLayoutWidth(paragraph));
                    break;
                }
                case NodeType.Table:
                {
                    Table table = (Table)childNode;
                    TableLayouter layouter = new TableLayouter(table, true);
                    LayoutWidth newLayoutWidth = layouter.DoLayout();
                    layoutWidth.Update(newLayoutWidth);
                    mNestedTables.Add(table);
                    break;
                }
                case NodeType.StructuredDocumentTag:
                {
                    CompositeNode sdt = (CompositeNode)childNode;
                    // NextNonMarkupNodeLimited may go out of parent node if the parent is SDT.
                    for (Node node = sdt.FirstNonMarkupDescendant; node != null && node.IsAncestorNode(sdt);
                            node = node.NextNonMarkupNodeLimited)
                        CalculateChildNode(node, layoutWidth);
                    break;
                }
                default:
                {
                    // Skip other types of nodes (should not actually exist).
                    break;
                }
            }
        }

        private void SetCellWidths()
        {
            for (int rowIndex = 0; rowIndex < mTable.Rows.Count; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < mColumns.Length; columnIndex++)
                {
                    Cell cell = GetCellUsingGridSpans(mTable, columnIndex, rowIndex);

                    // Skip grid columns in the middle of the cell.
                    if (cell == null)
                        continue;

                    double width = GetCellWidth(cell, columnIndex);
                    width = System.Math.Min(width, MaxTableWidth);

                    // Do not use CellFormat.Width, it sets the preferred width as well.
                    cell.CellPr.Width = ConvertUtilCore.PointToTwip(width);
                }
            }
        }

        private double GetCellWidth(Cell cell, int columnIndex)
        {
            switch (cell.CellFormat.HorizontalMerge)
            {
                case CellMerge.None:
                case CellMerge.First:
                {
                    int gridSpan = Extensions.GetHorizontalMergeSpan(cell);
                    return GetGridRangeWidth(columnIndex, gridSpan);
                }
                case CellMerge.Previous:
                {
                    return 0;
                }
                default:
                {
                    throw new InvalidOperationException("Unknown cell merge.");
                }
            }
        }

        /// <summary>
        /// Lays out the immediate nested tables.
        /// </summary>
        private void ProcessNestedTables()
        {
            for (int tableIndex = 0; tableIndex < mNestedTables.Count; tableIndex++)
            {
                Table table = mNestedTables[tableIndex];
                table.UpdateLayout();
            }
        }

        /// <summary>
        /// Gets the sum of grid column widths in the given range.
        /// </summary>
        private double GetGridRangeWidth(int firstColumn, int columnCount)
        {
            int rangeStart = System.Math.Max(firstColumn, 0);
            int rangeEnd = System.Math.Min(rangeStart + columnCount, mColumns.Length);

            double width = 0d;
            for (int column = rangeStart; column < rangeEnd; column++)
                width += mColumns[column].CalculatedWidth;

            return width;
        }

        /// <summary>
        /// Gets a cell node from the given table using the specified column and row indices.
        /// </summary>
        /// <remarks>
        /// Returns null if the table doesn't contain such a cell.
        /// Unlike <see cref="Table.GetCell"/>, the method takes grid spans into account.
        /// Grid span handling was introduced per WORDSNET-13177
        /// </remarks>
        [JavaConvertCheckedExceptions]
        private static Cell GetCellUsingGridSpans(Table table, int columnIndex, int rowIndex)
        {
            if ((rowIndex < 0) || (rowIndex >= table.Rows.Count))
                return null;

            if (columnIndex < 0)
                return null;

            CellCollection cells = table.Rows[rowIndex].Cells;

            // DM It seems, grid before is not taken into account. 
            // Let it be for now, the method is used by the "old layouter" only.
            int cellColumn = 0;
            Cell cellAtIndex = null;
            foreach (Cell cell in cells)
            {
                if (cellColumn == columnIndex)
                {
                    // Cell found.
                    cellAtIndex = cell;
                    break;
                }
                else if (cellColumn < columnIndex)
                {
                    // Next cell column:
                    cellColumn += cell.CellPr.GridSpan;
                }
                else
                {
                    // Column index does not match a cell start in the given row. 
                    // This is normal when cells spanning several columns are present, this method just returns null in this case.
                    break;
                }
            }

            return cellAtIndex;
        }

        private Cell mFixedContributor;
        private Cell mMaxContributor;
        private bool mHasPercent;
        private double mSpanMaxWidth;

        /// <summary>
        /// The table to lay out.
        /// </summary>
        private readonly Table mTable;
        /// <summary>
        /// An array of columns used to lay out the table.
        /// </summary>
        private Column[] mColumns;
        /// <summary>
        /// True if need to recalculate the layout widths.
        /// </summary>
        private bool mAreLayoutWidthsDirty = true;
        /// <summary>
        /// The width of the table container's client area.
        /// </summary>
        private double mTableContainerWidth;
        /// <summary>
        /// The absolute calculated width of the table.
        /// </summary>
        private double mTableSpecifiedWidth;
        /// <summary>
        /// The min and max possible widths of the table.
        /// </summary>
        private LayoutWidth mTableLayoutWidth;
        /// <summary>
        /// An object which performs the measurement of a paragraph contents.
        /// </summary>
        private ParagraphMeasurer mParagraphMeasurer;
        /// <summary>
        /// A list of cells whose span is > 1.
        /// </summary>
        private List<SpanCell> mSpanCells;
        /// <summary>
        /// This is used in CalculateResultingWidths to determine the extra or overallocated width.
        /// </summary>
        private double mAvailableWidth;
        /// <summary>
        /// The total number of "auto" (not specified) width columns.
        /// </summary>
        private int mAutoCount;
        /// <summary>
        /// The total number of fixed width columns.
        /// </summary>
        private int mFixedCount;
        /// <summary>
        /// The total number of percent width columns.
        /// </summary>
        private int mPercentCount;
        /// <summary>
        /// The total minimum width occupied by the contents of the "auto" width columns.
        /// </summary>
        private double mTotalAutoMin;
        /// <summary>
        /// The total maximum width occupied by the contents of the "auto" width columns.
        /// </summary>
        private double mTotalAutoMax;
        /// <summary>
        /// The total maximum width occupied by the contents of the fixed width columns.
        /// </summary>
        private double mTotalFixedMax;
        /// <summary>
        /// The total value of the percent columns.
        /// </summary>
        private double mTotalPercent;
        /// <summary>
        /// Width of table container. If GrowAutofit option is not set table should be scaled to this margins.
        /// </summary>
        private double mMarginsWidth;
        /// <summary>
        /// A list of immediate nested tables.
        /// </summary>
        private List<Table> mNestedTables;
        /// <summary>
        /// Controls whether to suppress layout update of nested tables at the end of layout process.
        /// </summary>
        private readonly bool mSuppressNestedTablesUpdates;
        /// <summary>
        /// This is the maximum table width allowed in Word.
        /// </summary>
        private const double MaxTableWidth = ConvertUtilCore.MaxSizePoint;  // 55.87 cm.
    }
}
