// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Aspose.Collections;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Celler;
using Aspose.Words.TableLayout;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents a table in a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Table"/> is a block-level node and can be a child of classes derived from <see cref="Story"/> or
    /// <see cref="InlineStory"/>.</p>
    /// <p><see cref="Table"/> can contain one or more <see cref="Row"/> nodes.</p>
    /// <p>A minimal valid table needs to have at least one <see cref="Row"/>.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Row>")]
    public class Table : CompositeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Table"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Table"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the story where you want the table inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Table(DocumentBase doc) : base(doc)
        {
        }

        /// <summary>
        /// Converts cells horizontally merged by width to cells merged by <see cref="CellFormat.HorizontalMerge" />.
        /// </summary>
        /// <remarks>
        /// <p>Table cells can be horizontally merged either using merge flags <see cref="CellFormat.HorizontalMerge" /> or using cell width <see cref="CellFormat.Width" />.</p>
        /// <p>When table cell is merged by width property <see cref="CellFormat.HorizontalMerge" /> is meaningless but sometimes having merge flags is more convenient way.</p>
        /// <p>Use this method to transforms table cells horizontally merged by width to cells merged by merge flags.</p>
        /// </remarks>
        public void ConvertToHorizontallyMergedCells()
        {
            // Use CellerTable to build table grid.
            CellerTable celler = new CellerTable(this, CellerTableOptions.GotoMergedCells);
            if (celler.ColumnCount == 0)
                return;

            celler.GotoFirstRow();

            TablePr.Remove(TableAttr.Sys_CalculatedTableGrid);
            TablePr.Remove(TableAttr.Sys_TableGridForNewAlgorithm);

            foreach(Row row in Rows)
            {
                List<int>spans = new List<int>();
                List<Cell>cells = new List<Cell>();

                // Collect spans to avoid side effect when table is modified.
                foreach (Cell cell in row.Cells)
                {
                    int colSpan = 1;

                    celler.GotoFirstCell();
                    for (int col = 0; col < celler.ColumnCount; col++)
                    {
                        if (ReferenceEquals(cell, celler.CurCell.CellNode))
                        {
                            colSpan = celler.CurCell.ColSpan;
                            break;
                        }

                        celler.GotoNextCell();
                    }

                    cells.Add(cell);
                    spans.Add(colSpan);
                }

                // Now we have spans for each cells, transform table.
                for (int i = 0; i < cells.Count; i++)
                {
                    Cell cell = cells[i];
                    int span = spans[i];

                    cell.CellPr.Remove(CellAttr.Sys_CellSpan);

                    if (span == 1)
                        continue;

                    // First try next cell as it might be already merged by flag.
                    // Naive check. We should go deeper and ensure that whole merge chain already exist. Postpone for a while.
                    if ((cell.NextCell != null) && ((CellMerge)cell.NextCell.CellPr.FetchAttr(CellAttr.HorizontalMerge) == CellMerge.Previous))
                        continue;

                    cell.CellPr.SetAttr(CellAttr.HorizontalMerge, CellMerge.First);
                    for (int j = 0; j < span - 1; j++)
                    {
                        Cell mergedCell = (Cell)cell.Clone(false);
                        mergedCell.CellPr.SetAttr(CellAttr.Width, 0);
                        mergedCell.CellPr.SetAttr(CellAttr.PreferredWidth, PreferredWidth.FromPoints(0));
                        mergedCell.CellPr.SetAttr(CellAttr.HorizontalMerge, CellMerge.Previous);
                        mergedCell.AppendChild(new Paragraph(mergedCell.Document));

                        cell.InsertNext(mergedCell);
                    }
                }

                celler.GotoNextRow();
            }
        }

        /// <summary>
        /// Returns <see cref="NodeType.Table"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Table; }
        }

        /// <summary>
        /// Returns the first <see cref="Row"/> node in the table.
        /// </summary>
        public Row FirstRow
        {
            // SPEED for RTF export. While the table is allowed to contain only rows this is a fast method
            // for getting first row. However if we later allow bookmarks between rows, this has to be improved.
            // Same for LastRow.
            get { return (Row)FirstNonMarkupCompositeDescendant; }
        }

        /// <summary>
        /// Returns the last <see cref="Row"/> node in the table.
        /// </summary>
        public Row LastRow
        {
            get { return (Row)LastNonMarkupCompositeDescendant; }
        }

        /// <summary>
        /// Provides typed access to the rows of the table.
        /// </summary>
        public RowCollection Rows
        {
            get
            {
                if (mRowsCache == null)
                    mRowsCache = new RowCollection(this);
                return mRowsCache;
            }
        }

        /// <summary>
        /// Returns true if table has any cell in it.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "cell",
            Justification = "False positive.")]
        internal bool HasCells
        {
            get
            {
                foreach (Row row in Rows)
                    foreach (Cell cell in row.Cells)
                        return true;

                return false;
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Table lhs = (Table)base.Clone(isCloneChildren, cloningListener);
            if (mTablePr != null)
            {
                lhs.mTablePr = mTablePr.Clone();
            }
            lhs.mRowsCache = null;
            return lhs;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitTableStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the section
        /// and calls <see cref="DocumentVisitor.VisitTableEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the table.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitTableStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the table.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitTableEnd(this);
        }

        /// <summary>
        /// If the table has no rows, creates and appends one <see cref="Row"/>.
        /// </summary>
        public void EnsureMinimum()
        {
            Row row = FirstRow;
            if (row == null)
                row = (Row)AppendChild(new Row(Document));

            row.EnsureMinimum();
        }

        /// <summary>
        /// Gets a cell node using the specified column and row indices.
        /// Does NOT take merged cells into account, just works with nodes.
        /// Returns null if the table doesn't contain such a cell.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal Cell GetCell(int columnIndex, int rowIndex)
        {
            if ((rowIndex < 0) || (rowIndex >= Rows.Count))
                return null;

            CellCollection cells = Rows[rowIndex].Cells;

            if ((columnIndex < 0) || (columnIndex >= cells.Count))
                return null;

            return cells[columnIndex];
        }

        /// <summary>
        /// Returns the maximum number of columns spanned by the cells in a table row.
        /// </summary>
        internal int GetColumnCount()
        {
            int columnCount = 0;

            for (Row row = FirstRow; row != null; row = row.NextRow)
                columnCount = System.Math.Max(columnCount, GetColumnCount(row));

            return columnCount;
        }

        private static int GetColumnCount(Row row)
        {
            int columnCount = 0;
            for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                columnCount += cell.CellPr.GridSpan;

            return columnCount;
        }

        /// <summary>
        /// Updates the widths of the cells in accordance with preferred widths.
        ///
        /// This is DV's implementation of table layout that was done for old PDF and HTML export,
        /// before the layout engine. At the moment it has no relation to the layout engine.
        /// </summary>
        internal LayoutWidth UpdateLayout()
        {
            TableLayouter layouter = new TableLayouter(this);
            return layouter.DoLayout();
        }

        /// <summary>
        /// Converts horizontally merged cells into normal cells.
        ///
        /// It is perfectly valid to represent horizontally merged cells using the CellFormat.HorizontalMerge.
        /// But MS Word usually represents horizontally merged cells using normal cells.
        /// </summary>
        internal void NormalizeHorizontalMerge()
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                Cell cell = row.FirstCell;
                Cell firstMergedCell = null;
                while (cell != null)
                {
                    switch (cell.CellPr.HorizontalMerge)
                    {
                        case CellMerge.First:
                            // The cell that is first in a series of merged cells, becomes a normal cell
                            // and it will now take on the width of all merged cells.
                            firstMergedCell = cell;
                            firstMergedCell.CellPr.HorizontalMerge = CellMerge.None;
                            break;
                        case CellMerge.Previous:
                            if (firstMergedCell != null)
                            {
                                firstMergedCell.CellPr.Width = firstMergedCell.CellPr.Width + cell.CellPr.Width;
                                firstMergedCell.CellPr.GridSpan += cell.CellPr.GridSpan;

                                // WORDSNET-7089 Update fixed preferred width.
                                // AM Maybe we should try to add up cell preferred width for percents as well?
                                // andrnosk: WORDSNET-7654 Do not update preferred width if current cell width equals zero.
                                // This case is possible upon HTML import only.
                                // DMatv: In fact, after WORDSNET-14978 preferred width for the merged cells is ignored by docx/wml reader.
                                if (cell.CellPr.Width != 0 && firstMergedCell.CellPr.PreferredWidth.IsFixed &&
                                    cell.CellPr.PreferredWidth.IsFixed)
                                {
                                    int twips = firstMergedCell.CellPr.PreferredWidth.ValueTwips +
                                                cell.CellPr.PreferredWidth.ValueTwips;
                                    firstMergedCell.CellPr.PreferredWidth = PreferredWidth.FromTwipsSafe(twips);
                                    if (cell.CellPr.PreferredWidth.ValueTwips > 0)
                                    {
                                        // Dmatv: the above logic prevents correct grid calculation for Test4500().
                                        // Set the grid state to prevent applying the calculated grid to the table.
                                        if (TablePr.CalculatedGrid == null || TablePr.CalculatedGrid.State != GridState.CalculationFailed)
                                        {
                                            FixedGridCalculator.SetGridCalculationFailed(this);
                                            TableGridDebugLogger.DebugWriteLine(string.Format("### Grid state set to CalculationFailed in NormalizeHorizontalMerges() for table {0}", this));
                                        }
                                    }
                                }

                                // WORDSNET-7644 Move right border of the last cell in a merge to the result merged cell.
                                if ((cell.NextCell == null) ||
                                    (cell.NextCell.CellPr.HorizontalMerge != CellMerge.Previous))
                                {
                                    object border = cell.CellPr.GetDirectAttr(CellAttr.BorderRight);
                                    if (border != null)
                                    {
                                        // Clone is not needed this cell will be removed.
                                        firstMergedCell.CellPr.SetAttr(CellAttr.BorderRight, border);
                                    }
                                    else
                                    {
                                        firstMergedCell.CellPr.Remove(CellAttr.BorderRight);
                                    }
                                }

                                cell.Remove();
                                cell = firstMergedCell;
                            }
                            else
                            {
                                // Invalid case. A cell merged to previous while there was no first.
                                // Let's just reset for resiliency.
                                cell.CellPr.HorizontalMerge = CellMerge.None;
                            }
                            break;
                        case CellMerge.None:
                            firstMergedCell = null;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown cell merge type.");
                    }
                    cell = cell.NextCell;
                }
            }
        }

        /// <summary>
        /// Check if vertically merged cells are merged properly, i.e. merged cells span the same number of grid columns.
        /// If cells are merged improperly, VerticalMerge will be reset.
        ///
        /// Call this method only after normalizing horizontally merged cells because this can change table grid.
        /// </summary>
        internal void NormalizeVerticalMerge()
        {
            NormalizeVerticalMerge(RevisionsView.Original);
            if (HasRevision)
                NormalizeVerticalMerge(RevisionsView.Final);
        }

        private void NormalizeVerticalMerge(RevisionsView view)
        {
            // Check if there are some rows in this table.
            // If there are no rows, this method has nothing to do.
            if(FirstRow == null)
                return;

            TableGridFromCellWidth tableGrid = new TableGridFromCellWidth(this, (view == RevisionsView.Final));
            // Avoid to normalize tables with invalid table grid.
            if (tableGrid.ColumnWidths.Length == 0)
                return;

            // Due to OOXML specification, if a vertically merged group of cells do not span the same set of grid columns,
            // then this vertical merge is invalid. Such invalid merged cells can corrupt a document or cause problems upon rendering.

            // This hashtable is used to store information about vertically merged cells.
            // This is needed to avoid invalid vertical merge. Key is number of grid columns before vertically merged cell,
            // value is number of grid columns spanned by the cell.
            IntToIntDictionary previousRowMergedRegions = new IntToIntDictionary();

            // In this hashtable we will store cells, which are vertically merged.
            // Key is number of grid columns before vertically merged region, value is actually Cell.
            // This is needed to reset VerticalMerge, if it is necessary.
            IntToObjDictionary<Cell> previousRowMergedCells = new IntToObjDictionary<Cell>();

            bool isVerticalMergeUpdated = false;
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                tableGrid.NextRow(row);

                IntToIntDictionary currentRowMergedRegions = new IntToIntDictionary();
                IntToObjDictionary<Cell> currentRowMergedCells = new IntToObjDictionary<Cell>();

                // This value represents current grid position, it is used as a key in MergedRegions and MergedCells tables.
                // Since rows can be indented differently, currentGridPosition not always starts from zero.
                int currentGridPosition = tableGrid.GridBefore;
                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                {
                    int gridSpan = tableGrid.NextCell();

                    // For further processing we are interested only in vertically merged cells.
                    if (GetVerticalMerge(cell, view) != CellMerge.None)
                    {
                        int previousGridSpan = previousRowMergedRegions[currentGridPosition];

                        if (IntToIntDictionary.IsNullSubstitute(previousGridSpan))
                            previousGridSpan = cell.CellPr.GridSpan;

                        // WORDSNET-19247 Use grid span of previous merged cell to compare with current cell "GridSpan" property.
                        int prevMergedGridSpan = previousRowMergedCells.ContainsKey(currentGridPosition) ?
                            previousRowMergedCells[currentGridPosition].CellPr.GridSpan : previousGridSpan;

                        // Merged regions should start at the same grid position and span the same number of grid columns.
                        // Otherwise consider the cell as the first in a vertically merged region.
                        // WORDSNET-16216 gridSpan can be equal 0 if this cell is merged in previous cell,
                        // in this case compare with cells GridSpan property, it has right value.
                        // WORDSNET-23156 The Word allows vertical merge when the grid span of an upper cell greater than current one.
                        if ((previousGridSpan < gridSpan) && (prevMergedGridSpan < cell.CellPr.GridSpan))
                        {
                            isVerticalMergeUpdated = true;
                            cell.CellPr.SetAttrRevisionAware(CellAttr.VerticalMerge, CellMerge.First, view);
                        }

                        // WORDSNET-8818 If it is a one-column row without RowHeightRule, we should specify RowHeightRule.
                        // WORDSNET-25744 The explicitly set HeightRule.Auto shall also be processed.
                        bool heightRuleNeedsSpecifying = (row.TablePr.HeightRule == HeightRule.Auto) && (row.Cells.Count == 1);
                        if (heightRuleNeedsSpecifying)
                            SpecifyRowHeightRule(row);

                        currentRowMergedRegions[currentGridPosition] = gridSpan;
                        currentRowMergedCells[currentGridPosition] = cell;
                    }
                    // There can be not normalized horizontal merge (for example if we save to DOC format).
                    // So consider merge flag when advance grid position.
                    currentGridPosition += cell.CellPr.GridSpan;
                }

                // There could be cells which are vertically merged to previous but there are no cells merged in previous row.
                // Make them first in merge.
                IntToObjDictionary<Cell>.Enumerator enumerator = currentRowMergedCells.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Cell currentCell = enumerator.CurrentValue;
                    if ((GetVerticalMerge(currentCell, view) == CellMerge.Previous) && (previousRowMergedCells[enumerator.CurrentKey] == null))
                    {
                        isVerticalMergeUpdated = true;
                        currentCell.CellPr.SetAttrRevisionAware(CellAttr.VerticalMerge, CellMerge.First, view);
                    }
                }

                // There could be cells, which are the first cells in the vertically merged region,
                // but there are no cells merged with them in the next row. In such case we can reset VerticalMerge.
                enumerator = previousRowMergedCells.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Cell previousCell = enumerator.CurrentValue;
                    if ((GetVerticalMerge(previousCell, view) == CellMerge.First) && (currentRowMergedCells[enumerator.CurrentKey] == null))
                    {
                        isVerticalMergeUpdated = true;
                        previousCell.CellPr.SetAttrRevisionAware(CellAttr.VerticalMerge, CellMerge.None, view);
                    }
                }

                previousRowMergedRegions = currentRowMergedRegions;
                previousRowMergedCells = currentRowMergedCells;
            }

            if (isVerticalMergeUpdated)
            {
                // Dmatv: the above logic prevents correct grid calculation for Test11439().
                // Originally I intended to set the grid state to prevent applying the calculated grid to the table here.
                // However, it turned out there are tests with this condition where the new grid calculation still works correctly.
                // For example, setting grid state to CalculationFailed here fails TestJira16899().
                TableGridDebugLogger.DebugWriteLine(string.Format(
                    "### Grid calculation may not be reliable after correcting vertical merges in NormalizeVerticalMerges() for table {0}", this));
            }

            // Resiliency: Cells in the last row should not be first vertically merged.
            for (Cell cell = LastRow.FirstCell; cell != null; cell = cell.NextCell)
            {
                if (GetVerticalMerge(cell, view) == CellMerge.First)
                    cell.CellPr.SetAttrRevisionAware(CellAttr.VerticalMerge, CellMerge.None, view);
            }
        }

        /// <summary>
        /// Fixes nested table layout.
        /// </summary>
        internal void FixNestedTableLayout()
        {
            // Nowhere to set the attribute if the first row is null.
            if (!IsNested || (FirstRow == null))
                return;

            FixNestedTableAutoFit();

            // WORDSNET-19655 Nested tables cannot have negative indents.
            FixNestedTableNegativeLefIndent();
        }

        /// <summary>
        /// Sets Autofit value of the nested table to the value of the containing table.
        /// </summary>
        /// <remarks>
        /// MS Word does not seem to allow fixed table layout in autofit tables and vice versa.
        /// </remarks>
        private void FixNestedTableAutoFit()
        {
            Table containingTable = (Table)GetAncestor(NodeType.Table);
            bool parentAutoFit = containingTable.AllowAutoFit;
            if (parentAutoFit != AllowAutoFit)
            {
                // Dmatv: An UpdateLayout() call that was added per 13636 but caused 19798 was removed when working on WORDSNET-23611
                AllowAutoFit = parentAutoFit;
            }
        }

        /// <summary>
        /// Removes the LefIndent value of the nested table if negative.
        /// </summary>
        /// <remarks>
        /// MS Word overwrites negative nested indents on re-saving.
        /// </remarks>
        private void FixNestedTableNegativeLefIndent()
        {
            if (LeftIndent >= 0)
                return;

            TablePr.Remove(TableAttr.LeftIndent);

            foreach (Row row in Rows)
                row.TablePr.Remove(TableAttr.LeftIndent);
        }

        /// <summary>
        /// Gets vertical merge value depending on revision view.
        /// </summary>
        private static CellMerge GetVerticalMerge(Cell cell, RevisionsView view)
        {
            return (CellMerge)cell.CellPr.FetchAttr(CellAttr.VerticalMerge, view);
        }

        /// <summary>
        /// Specifies RowHeightRule and RowHeight, according to the last ParagraphBreakFont.Size.
        /// </summary>
        /// <remarks>
        /// Mimic MSW. Used for vertically merged one-column rows.
        /// </remarks>
        private static void SpecifyRowHeightRule(Row row)
        {
            // MSW uses this value as a RowHeight when cells have no child nodes.
            int rowHeight = 276;

            if (row.FirstCell.LastParagraph != null)
            {
                Paragraph para = row.FirstCell.LastParagraph;

                ParaPr expParaPr = para.GetExpandedParaPr(ParaPrExpandFlags.Layout);
                RunPr expParaBreakRunPr = para.GetExpandedParagraphBreakRunPr(RunPrExpandFlags.Layout);

                int lineSpacing = expParaPr.LineSpacing;
                int fontSize = expParaBreakRunPr.Size;

                // AM. This is the best approximation I've got while test this issue.
                //
                // Magic value actually reflects some TTF metric, according to tests it slightly differs
                // depending on font name.
                //
                // Courier New     ~ 21.1
                // Times New Roman ~ 20.8
                //
                // Lets take some average.
                const double magic = 21;

                rowHeight = (int)(System.Math.Round(lineSpacing / magic, 1) * fontSize);
            }

            row.TablePr.SetAttr(TableAttr.RowHeight, new Height(HeightRule.AtLeast, rowHeight));
        }

        /// <summary>
        /// Gets the number of rows at the top of the table that must be repeated on each page as header rows.
        /// </summary>
        internal int GetHeaderRowCount()
        {
            int result = 0;
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                if (row.TablePr.HeadingFormat)
                    result++;
                else
                    break;
            }
            return result;
        }

        /// <summary>
        /// Gets the left side of an inline table according to the MS Word algorithm.
        /// </summary>
        internal int GetTableLeft()
        {
            Row row = FirstRow;
            if (row == null)
                return 0;

            CellPr firstCellPr = row.FirstCell.CellPr;
            return row.TablePr.GetTableLeft(firstCellPr, IsNested);
        }

        /// <summary>
        /// Allows to insert row-level elements only.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.IsRowLevelNode(newChild);
        }

        /// <summary>
        /// Returns true if this table is contained inside a table cell and therefore a nested table.
        /// </summary>
        internal bool IsNested
        {
            get
            {
                // WORDSNET-9691 If this table was created by node cloning, then it's parent is null
                // and access to its parent is not allowed.
                // Also, if table has no parent, then we should treat it as not nested.
                if (FirstNonMarkupParentNode == null)
                    return false;

                return (FirstNonMarkupParentNode.NodeType == NodeType.Cell);
            }
        }

        /// <summary>
        /// Finds row with different positioning or style.
        /// </summary>
        internal Row FindDifferentPositioningOrStyle()
        {
            Debug.Assert(ParentNode != null, "The table must already be added to the document.");

            // WORDSNET-12534 Word ignores floating inside textbox.
            if (GetAncestor(NodeType.Shape) != null)
                return null;

            Row prevRow = null;
            Row curRow = FirstRow;
            while (curRow != null)
            {
                if (prevRow != null)
                    if ((prevRow.TablePr.Istd != curRow.TablePr.Istd) || !prevRow.IsSameRowFloatingPositioning(curRow))
                        return curRow;

                prevRow = curRow;
                curRow = curRow.NextRow;
            }

            return null;
        }

        /// <summary>
        /// Makes the table floating if it is not already so.
        /// </summary>
        private void MakeTableFloatingWhenSettingDistance()
        {
            if (IsFloating)
                return;

            // MS Word sets these properties when setting the DistanceXXX properties in VBA if a table is not floating yet.
            // This makes the table floating.
            VerticalAnchor = RelativeVerticalPosition.Paragraph;
            AbsoluteVerticalDistance = 0.0;
        }

        /// <summary>
        /// Specifies how an inline table is aligned in the document.
        /// </summary>
        /// <remarks>
        /// <para>The default value is <see cref="TableAlignment.Left"/>.</para>
        /// </remarks>
        public TableAlignment Alignment
        {
            get { return (TableAlignment)FetchFirstRowAttr(TableAttr.Alignment); }
            set { SetAttrOnAllRows(TableAttr.Alignment, value); }
        }

        /// <summary>
        /// Allows Microsoft Word and Aspose.Words to automatically resize cells in a table to fit their contents.
        /// </summary>
        /// <remarks>
        /// <para>The default value is <c>true</c>.</para>
        ///
        /// <seealso cref="AutoFit"/>
        /// </remarks>
        public bool AllowAutoFit
        {
            get { return (bool)FetchFirstRowAttr(TableAttr.AllowAutoFit); }
            set
            {
                if (value != AllowAutoFit)
                {
                    // WORDSNET-16899 Reset the calculated grid on changing table layout.
                    RemoveCalculatedGrid();
                }

                SetAttrOnAllRows(TableAttr.AllowAutoFit, value);
            }
        }

        /// <summary>
        /// Indicates if the table is inside a shape and shape size determines the available width.
        /// </summary>
        /// <remarks>
        /// This currently includes all tables nested in a topmost table resized to text box container width.
        ///
        /// Text box content area size is supposed to be precisely computed by Rendering.
        /// It might be needed to compute nested table width, even for fixed layout tables.
        /// Shape size is available in layout,
        /// but not in the document model where fixed table grids are normally computed.
        /// </remarks>
        internal bool NeedsShapeContainerWidth
        {
            get
            {
                Table topmostTable = this;
                CompositeNode container;
                // Move up the nested table tree.
                for (container = topmostTable.FirstNonMarkupParentNode;
                    container.NodeType == NodeType.Cell;
                    container = topmostTable.FirstNonMarkupParentNode)
                {
                    topmostTable = ((Cell)container).ParentTable;
                }

                if (container.NodeType != NodeType.Shape)
                    return false;
                // There might be other cases when container width is not available at the document model level.
                // A table in an inherited header/footer in a section with different page properties is an example.
                // Not handled for now.

                // Check if the shape grows to fit contents.
                Shape shape = (Shape)container;
                ShapeAdaptor shapeAdaptor = new ShapeAdaptor(shape);
                if (shapeAdaptor.WrapMode == TextBoxWrapMode.None)
                    return false;

                // For fixed layout only percent tblw requires container width.
                // "Auto" tblw for fixed layout tables does not depend on contents on container metrics,
                // only on cell preferred widths, and auto cell widths are assigned from the grid for fixed layout.
                return topmostTable.PreferredWidth.IsPercent ||
                    (AllowAutoFit && topmostTable.PreferredWidth.IsAuto);
            }
        }

        /// <summary>
        /// Gets or sets the table preferred width.
        /// </summary>
        /// <remarks>
        /// <para>The default value is <see cref="Aspose.Words.Tables.PreferredWidth.Auto"/>.</para>
        /// </remarks>
        public PreferredWidth PreferredWidth
        {
            get { return (PreferredWidth)FetchFirstRowAttr(TableAttr.PreferredWidth); }
            set { SetAttrOnAllRows(TableAttr.PreferredWidth, value); }
        }

        /// <summary>
        /// Gets or sets whether this is a right-to-left table.
        /// </summary>
        /// <remarks>
        /// <p>When <c>true</c>, the cells in this row are laid out right to left.</p>
        /// <para>The default value is <c>false</c>.</para>
        /// </remarks>
        public bool Bidi
        {
            get { return (bool)FetchFirstRowAttr(TableAttr.Bidi); }
            set { SetAttrOnAllRows(TableAttr.Bidi, value); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add to the left of the contents of cells.
        /// </summary>
        public double LeftPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.LeftPadding)); }
            set { SetAttrOnAllRows(TableAttr.LeftPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add to the right of the contents of cells.
        /// </summary>
        public double RightPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.RightPadding)); }
            set { SetAttrOnAllRows(TableAttr.RightPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add above the contents of cells.
        /// </summary>
        public double TopPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.TopPadding)); }
            set { SetAttrOnAllRows(TableAttr.TopPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add below the contents of cells.
        /// </summary>
        public double BottomPadding
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.BottomPadding)); }
            set { SetAttrOnAllRows(TableAttr.BottomPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) between the cells.
        /// </summary>
        public double CellSpacing
        {
            // TODO it is actually not correct to define this at a table level
            // as different rows may have different spacing settings.
            get { return ((PreferredWidth)FetchFirstRowAttr(TableAttr.CellSpacing)).Value; }
            set { SetAttrOnAllRows(TableAttr.CellSpacing, PreferredWidth.FromPointsSafe(value)); }
        }

        /// <summary>
        /// Gets or sets the "Allow spacing between cells" option.
        /// </summary>
        public bool AllowCellSpacing
        {
            get
            {
                // TODO it is actually not correct to define this at a table level
                // as different rows may have different spacing settings.
                return FirstRow.HasCellSpacing;
            }
            set
            {
                if (value)
                {
                    // Mimic MSW. Assign initial value if CellSpacing was not defined.
                    if (!AllowCellSpacing)
                        SetAttrOnAllRows(TableAttr.CellSpacing,
                            PreferredWidth.FromTwipsSafe(InitialCellSpacing));
                }
                else
                {
                    RemoveAttrFromAllRows(TableAttr.CellSpacing);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value that represents the left indent of the table.
        /// </summary>
        public double LeftIndent
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.LeftIndent)); }
            set
            {
                // WORDSNET-19655 Nested tables cannot have negative indents.
                if (IsNested && (value < 0))
                    return;

                SetAttrOnAllRows(TableAttr.LeftIndent, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets bit flags that specify how a table style is applied to this table.
        /// </summary>
        public TableStyleOptions StyleOptions
        {
            get { return (TableStyleOptions)FetchFirstRowAttr(TableAttr.StyleOptions); }
            set { SetAttrOnAllRows(TableAttr.StyleOptions, value); }
        }

        /// <summary>
        /// Gets or sets the table style applied to this table.
        /// </summary>
        public Style Style
        {
            get { return Document.Styles.FetchByIstd(Istd, StyleIndex.TableNormal); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Document != this.Document)
                    throw new ArgumentException("This style belongs to a different document.");

                if (value.Type != StyleType.Table)
                    throw new ArgumentException("This style is not a table style.");

                Istd = value.Istd;
            }
        }

        /// <summary>
        /// Gets or sets the name of the table style applied to this table.
        /// </summary>
        public string StyleName
        {
            get { return Style.Name; }
            set { Style = Document.Styles.FetchByName(value); }
        }

        /// <summary>
        /// Gets or sets the locale independent style identifier of the table style applied to this table.
        /// </summary>
        public StyleIdentifier StyleIdentifier
        {
            get { return Style.StyleIdentifier; }
            set { Style = Document.Styles.FetchBySti(value); }
        }

        /// <summary>
        /// Gets or sets <see cref="TextWrapping" /> for table.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get
            {
                // AM. I think it enough to just check whether table floating or not.
                return IsFloating ? TextWrapping.Around : TextWrapping.None;
            }

            set
            {
                if (value == TextWrapping.Around)
                    SetTextWrapping();
                else
                    RemoveTextWrapping();
            }
        }

        /// <summary>
        /// Gets or sets title of this table.
        /// It provides an alternative text representation of the information contained in the table.
        /// </summary>
        /// <remarks>
        /// <para>The default value is an empty string.</para>
        /// <para>This property is meaningful for ISO/IEC 29500 compliant DOCX documents
        /// (<see cref="Saving.OoxmlCompliance"/>).
        /// When saved to pre-ISO/IEC 29500 formats, the property is ignored.</para>
        /// </remarks>
        public string Title
        {
            get { return (string)FetchFirstRowAttr(TableAttr.Title); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                SetAttrOnAllRows(TableAttr.Title, value);
            }
        }

        /// <summary>
        /// Gets or sets description of this table.
        /// It provides an alternative text representation of the information contained in the table.
        /// </summary>
        /// <remarks>
        /// <para>The default value is an empty string.</para>
        /// <para>This property is meaningful for ISO/IEC 29500 compliant DOCX documents
        /// (<see cref="Saving.OoxmlCompliance"/>).
        /// When saved to pre-ISO/IEC 29500 formats, the property is ignored.</para>
        /// </remarks>
        public string Description
        {
            get { return (string)FetchFirstRowAttr(TableAttr.Description); }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                SetAttrOnAllRows(TableAttr.Description, value);
            }
        }

        /// <summary>
        /// Gets or sets distance between table left and the surrounding text, in points.
        /// </summary>
        public double DistanceLeft
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.FrameDistanceFromLeft)); }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "value");

                SetAttrOnAllRows(TableAttr.FrameDistanceFromLeft, ConvertUtilCore.PointToTwip(value));

                MakeTableFloatingWhenSettingDistance();
            }
        }

        /// <summary>
        /// Gets or sets distance between table right and the surrounding text, in points.
        /// </summary>
        public double DistanceRight
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.FrameDistanceFromRight)); }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "value");

                SetAttrOnAllRows(TableAttr.FrameDistanceFromRight, ConvertUtilCore.PointToTwip(value));

                MakeTableFloatingWhenSettingDistance();
            }
        }

        /// <summary>
        /// Gets or sets distance between table top and the surrounding text, in points.
        /// </summary>
        public double DistanceTop
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.FrameDistanceFromTop)); }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "value");

                SetAttrOnAllRows(TableAttr.FrameDistanceFromTop, ConvertUtilCore.PointToTwip(value));

                MakeTableFloatingWhenSettingDistance();
            }
        }

        /// <summary>
        /// Gets or sets distance between table bottom and the surrounding text, in points.
        /// </summary>
        public double DistanceBottom
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.FrameDistanceFromBottom)); }
            set
            {
                ArgumentUtil.CheckNonNegative(value, "value");

                SetAttrOnAllRows(TableAttr.FrameDistanceFromBottom, ConvertUtilCore.PointToTwip(value));

                MakeTableFloatingWhenSettingDistance();
            }
        }

        /// <summary>
        /// Gets or sets floating table relative horizontal alignment.
        /// </summary>
        public HorizontalAlignment RelativeHorizontalAlignment
        {
            get { return (HorizontalAlignment)FetchFirstRowAttr(TableAttr.HorizontalAlignment); }
            set
            {
                RemoveAttrFromAllRows(TableAttr.FrameLeft);
                SetAttrOnAllRows(TableAttr.HorizontalAlignment, value);
            }
        }

        /// <summary>
        /// Gets or sets floating table relative vertical alignment.
        /// </summary>
        public VerticalAlignment RelativeVerticalAlignment
        {
            get { return (VerticalAlignment)FetchFirstRowAttr(TableAttr.VerticalAlignment); }
            set
            {
                RemoveAttrFromAllRows(TableAttr.FrameTop);
                SetAttrOnAllRows(TableAttr.VerticalAlignment, value);
            }
        }

        /// <summary>
        /// Gets the base object from which the horizontal positioning of floating table should be calculated.
        /// Default value is <see cref="RelativeHorizontalPosition.Column"/>.
        /// </summary>
        public RelativeHorizontalPosition HorizontalAnchor
        {
            get { return (RelativeHorizontalPosition)FetchFirstRowAttr(TableAttr.RelativeHorizontalPosition); }
            set
            {
                switch (value)
                {
                    case RelativeHorizontalPosition.Margin:
                    case RelativeHorizontalPosition.Page:
                    case RelativeHorizontalPosition.Column:
                        // Allowed values.
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("HorizontalAnchor", "Parameter value outside of acceptable range.");
                }

                SetAttrOnAllRows(TableAttr.RelativeHorizontalPosition, value);
                // It is interesting that Word sets vertical position to force floating when horizontal anchor set.
                SetAttrOnAllRowsNoOverride(TableAttr.FrameTop, 0);
                SetAttrOnAllRowsNoOverride(TableAttr.RelativeVerticalPosition, RelativeVerticalPosition.Paragraph);
                SetAttrOnAllRowsNoOverride(TableAttr.FrameDistanceFromLeft, 180);
                SetAttrOnAllRowsNoOverride(TableAttr.FrameDistanceFromRight, 180);
            }
        }

        /// <summary>
        /// Gets the base object from which the vertical positioning of floating table should be calculated.
        /// Default value is <see cref="RelativeVerticalPosition.Margin"/>.
        /// </summary>
        public RelativeVerticalPosition VerticalAnchor
        {
            get { return (RelativeVerticalPosition)FetchFirstRowAttr(TableAttr.RelativeVerticalPosition); }
            set
            {
                switch (value)
                {
                    case RelativeVerticalPosition.Margin:
                    case RelativeVerticalPosition.Page:
                    case RelativeVerticalPosition.Paragraph:
                        // Allowed values.
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("VerticalAnchor", "Parameter value outside of acceptable range.");
                }

                SetAttrOnAllRows(TableAttr.RelativeVerticalPosition, value);
                SetAttrOnAllRowsNoOverride(TableAttr.FrameTop, 0);
                SetAttrOnAllRowsNoOverride(TableAttr.FrameDistanceFromLeft, 180);
                SetAttrOnAllRowsNoOverride(TableAttr.FrameDistanceFromRight, 180);
            }
        }

        /// <summary>
        /// Gets or sets absolute horizontal floating table position specified by the table properties, in points.
        /// Default value is 0.
        /// </summary>
        public double AbsoluteHorizontalDistance
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.FrameLeft)); }
            set
            {
                RemoveAttrFromAllRows(TableAttr.HorizontalAlignment);
                SetAttrOnAllRows(TableAttr.FrameLeft, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets absolute vertical floating table position specified by the table properties, in points.
        /// Default value is 0.
        /// </summary>
        public double AbsoluteVerticalDistance
        {
            get { return ConvertUtilCore.TwipToPoint((int)FetchFirstRowAttr(TableAttr.FrameTop)); }
            set
            {
                RemoveAttrFromAllRows(TableAttr.VerticalAlignment);
                SetAttrOnAllRows(TableAttr.FrameTop, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets whether a floating table shall allow other floating objects in the document
        /// to overlap its extents when displayed.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool AllowOverlap
        {
            get { return (bool)FetchFirstRowAttr(TableAttr.AllowOverlap); }
        }

        /// <summary>
        /// Performs no checks on set.
        /// </summary>
        internal int Istd
        {
            get { return (int)FetchFirstRowAttr(TableAttr.Istd); }
            set { SetAttrOnAllRows(TableAttr.Istd, value); }
        }

        /// <summary>
        /// Indicates that the table is floating table.
        /// </summary>
        internal bool IsFloating
        {
            get { return (Rows.Count > 0) && Rows[0].TablePr.IsFloating; }
        }

        /// <summary>
        /// Indicates if <see cref="TablePr.CalculatedGrid"/> is applied to the table.
        /// </summary>
        internal bool IsCalculatedGridApplied
        {
            get { return TableGridAttr.IsApplied(TablePr.CalculatedGrid); }
        }

        /// <summary>
        /// Holds table related properties. Currently used only to hold table grid.
        /// </summary>
        /// <remarks>
        /// See ImportDocx\TestJira6362() for design notes.
        /// </remarks>
        internal TablePr TablePr
        {
            get
            {
                // Instantiate at first access.
                if(mTablePr == null)
                    mTablePr = new TablePr();

                return mTablePr;
            }
        }

        /// <summary>
        /// Sets all table borders to the specified line style, width and color.
        /// </summary>
        /// <param name="lineStyle">The line style to apply.</param>
        /// <param name="lineWidth">The line width to set (in points).</param>
        /// <param name="color">The color to use for the border.</param>
        public void SetBorders(LineStyle lineStyle, double lineWidth, Color color)
        {
            foreach (BorderType borderType in TablePr.PossibleBorderKeys.Keys)
                SetBorder(borderType, lineStyle, lineWidth, color, true);
        }

        /// <summary>
        /// Sets the specified table border to the specified line style, width and color.
        /// </summary>
        /// <param name="borderType">The table border to change.</param>
        /// <param name="lineStyle">The line style to apply.</param>
        /// <param name="lineWidth">The line width to set (in points).</param>
        /// <param name="color">The color to use for the border.</param>
        /// <param name="isOverrideCellBorders">When <c>true</c>, causes all existing explicit cell borders to be removed.</param>
        public void SetBorder(BorderType borderType, LineStyle lineStyle, double lineWidth, Color color, bool isOverrideCellBorders)
        {
            // Clear cell borders that are specified on cells explicitly because we want this method to "override" any cell borders.
            if (isOverrideCellBorders)
                RemoveCellBorders(borderType);

            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                // Now set the table border (we do this on the row level).
                Border border = row.RowFormat.Borders[borderType];
                border.LineStyle = lineStyle;
                border.LineWidth = lineWidth;
                border.Color = color;
            }
        }

        /// <summary>
        /// Removes all table and cell borders on this table.
        /// </summary>
        public void ClearBorders()
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                row.TablePr.RemoveBorders();

                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                    cell.CellPr.RemoveBorders();
            }
        }

        private void RemoveTextWrapping()
        {
            TablePr firstRowPr = FirstRow.TablePr;

            // Convert HorizontalAlignment to TableAlignment.
            HorizontalAlignment alignment = (HorizontalAlignment)firstRowPr.FetchAttr(TableAttr.HorizontalAlignment);

            if (alignment == HorizontalAlignment.Right)
                SetAttrOnAllRows(TableAttr.Alignment, TableAlignment.Right);
            else if (alignment == HorizontalAlignment.Center)
                SetAttrOnAllRows(TableAttr.Alignment, TableAlignment.Center);

            // FrameLeft doesn't go to LeftIndent.

            RemoveAttrFromAllRows(TableAttr.FrameLeft);
            RemoveAttrFromAllRows(TableAttr.RelativeVerticalPosition);
            RemoveAttrFromAllRows(TableAttr.RelativeHorizontalPosition);
            RemoveAttrFromAllRows(TableAttr.FrameTop);
            RemoveAttrFromAllRows(TableAttr.FrameDistanceFromLeft);
            RemoveAttrFromAllRows(TableAttr.FrameDistanceFromRight);
            RemoveAttrFromAllRows(TableAttr.AllowOverlap);
            RemoveAttrFromAllRows(TableAttr.HorizontalAlignment);
            RemoveAttrFromAllRows(TableAttr.VerticalAlignment);
        }

        private void SetTextWrapping()
        {
            TablePr firstRowPr = FirstRow.TablePr;

            // It seems Word always set below floating attributes.
            SetAttrOnAllRows(TableAttr.RelativeVerticalPosition, RelativeVerticalPosition.TextFrameDefault);
            SetAttrOnAllRows(TableAttr.FrameTop, 0);
            SetAttrOnAllRows(TableAttr.FrameDistanceFromLeft, 180);
            SetAttrOnAllRows(TableAttr.FrameDistanceFromRight, 180);
            SetAttrOnAllRows(TableAttr.AllowOverlap, false);

            // Convert TableAligment to HorizontalAlignment.
            TableAlignment alignment = (TableAlignment)firstRowPr.FetchAttr(TableAttr.Alignment);

            if (alignment == TableAlignment.Right)
                SetAttrOnAllRows(TableAttr.HorizontalAlignment, HorizontalAlignment.Right);
            else if (alignment == TableAlignment.Center)
                SetAttrOnAllRows(TableAttr.HorizontalAlignment, HorizontalAlignment.Center);
            RemoveAttrFromAllRows(TableAttr.Alignment);

            // Convert LeftIndent to FrameLeft.
            if (firstRowPr.Contains(TableAttr.LeftIndent))
            {
                int leftIndent = (int)firstRowPr.GetDirectAttr(TableAttr.LeftIndent);
                SetAttrOnAllRows(TableAttr.FrameLeft, leftIndent - 1); // +1 hack.
            }
            RemoveAttrFromAllRows(TableAttr.LeftIndent);
        }

        private void RemoveCellBorders(BorderType borderType)
        {
            switch (borderType)
            {
                case BorderType.Top:
                {
                    // Remove top borders from the cells in the first row.
                    Row row = FirstRow;
                    if (row != null)
                    {
                        for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                            cell.CellPr.Remove(CellAttr.BorderTop);
                    }
                    break;
                }
                case BorderType.Bottom:
                {
                    // Remove bottom borders from the cells in the last row.
                    Row row = LastRow;
                    if (row != null)
                    {
                        for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                            cell.CellPr.Remove(CellAttr.BorderBottom);
                    }
                    break;
                }
                case BorderType.Left:
                {
                    // Remove left border from the first cell in each row.
                    for (Row row = FirstRow; row != null; row = row.NextRow)
                    {
                        Cell cell = row.FirstCell;
                        if (cell != null)
                            cell.CellPr.Remove(CellAttr.BorderLeft);
                    }
                    break;
                }
                case BorderType.Right:
                {
                    // Remove right border from the last cell in each row.
                    for (Row row = FirstRow; row != null; row = row.NextRow)
                    {
                        Cell cell = row.LastCell;
                        if (cell != null)
                            cell.CellPr.Remove(CellAttr.BorderRight);
                    }
                    break;
                }
                case BorderType.Horizontal:
                {
                    // Removing horizontal table border means we need to remove all top and bottom cell borders except cases for the first and last row.
                    for (Row row = FirstRow; row != null; row = row.NextRow)
                    {
                        bool isRemoveTop = !row.IsFirstRow;
                        bool isRemoveBottom = !row.IsLastRow;

                        for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                        {
                            if (isRemoveTop)
                                cell.CellPr.Remove(CellAttr.BorderTop);

                            if (isRemoveBottom)
                                cell.CellPr.Remove(CellAttr.BorderBottom);
                        }
                    }
                    break;
                }
                case BorderType.Vertical:
                {
                    // Removing vertical table border means we need to remove all left and right cell borders except cases for the first and last cell.
                    for (Row row = FirstRow; row != null; row = row.NextRow)
                    {
                        for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                        {
                            bool isRemoveLeft = !cell.IsFirstCell;
                            bool isRemoveRight = !cell.IsLastCell;

                            if (isRemoveLeft)
                                cell.CellPr.Remove(CellAttr.BorderLeft);

                            if (isRemoveRight)
                                cell.CellPr.Remove(CellAttr.BorderRight);
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentException("Unexpected border type.");
            }
        }

        /// <summary>
        /// Sets shading to the specified values on whole table.
        /// </summary>
        /// <param name="texture">The texture to apply.</param>
        /// <param name="foregroundColor">The color of the texture.</param>
        /// <param name="backgroundColor">The color of the background fill.</param>
        public void SetShading(TextureIndex texture, Color foregroundColor, Color backgroundColor)
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                Shading rowShading = row.TablePr.Shading;
                if (rowShading == null)
                {
                    rowShading = new Shading();
                    row.TablePr.Shading = rowShading;
                }

                rowShading.Texture = texture;
                rowShading.ForegroundPatternColor = foregroundColor;
                rowShading.BackgroundPatternColor = backgroundColor;

                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                {
                    Shading shading = cell.CellFormat.Shading;
                    shading.Texture = texture;
                    shading.ForegroundPatternColor = foregroundColor;
                    shading.BackgroundPatternColor = backgroundColor;
                }
            }
        }

        /// <summary>
        /// Removes all shading on the table.
        /// </summary>
        public void ClearShading()
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                row.TablePr.RemoveShading();

                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                    cell.CellPr.RemoveShading();
            }
        }

        /// <summary>
        /// Resizes the table and cells according to the specified auto fit behavior.
        /// </summary>
        /// <remarks>
        /// <para>This method mimics the commands available in the Auto Fit menu for a table in Microsoft Word.
        /// The commands available are "Auto Fit to Contents", "Auto Fit to Window" and "Fixed Column Width". In Microsoft Word
        /// these commands set relevant table properties and then update the table layout and Aspose.Words does the same for you.</para>
        /// </remarks>
        /// <param name="behavior">Specifies how to auto fit the table.</param>
        public void AutoFit(AutoFitBehavior behavior)
        {
            switch (behavior)
            {
                case AutoFitBehavior.AutoFitToContents:
                    AutoFitToContents();
                    break;
                case AutoFitBehavior.AutoFitToWindow:
                    AutoFitToWindow();
                    break;
                case AutoFitBehavior.FixedColumnWidths:
                    AutoFitFixedColumnWidths();
                    break;
                default:
                    throw new ArgumentException("Unknown auto fit behavior.");
            }
        }

        /// <summary>
        /// Calculates sum of cells width.
        /// </summary>
        internal int GetCellsWidth()
        {
            if (Rows.Count == 0)
                return 0;

            int maxWidth = 0;
            foreach (Row row in Rows)
            {
                int width = 0;
                foreach (Cell cell in row.Cells)
                    width += cell.CellPr.Width;

                maxWidth = System.Math.Max(maxWidth, width);
            }

            return maxWidth;
        }

        internal bool HasRevision
        {
            get
            {
                foreach (Row row in Rows)
                {
                    if (row.TablePr.HasRevisions)
                        return true;

                    foreach(Cell cell in row.Cells)
                        if (cell.CellPr.HasRevisions)
                            return true;
                }

                return false;
            }
        }

        internal void RemoveAttrFromAllRows(int key)
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
                row.TablePr.Remove(key);
        }

        internal void SetAttrOnAllRows(int key, object value)
        {
            if (FirstRow == null)
                throw new InvalidOperationException("Formatting cannot be applied because the table is empty. Add at least one row to the table first.");

            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                if (key == TableAttr.Istd)
                    PrepareRowBeforeSetIstd(row);

                row.TablePr.SetAttr(key, value);
            }
        }

        internal void SetAttrOnAllRowsNoOverride(int key, object value)
        {
            if (FirstRow == null)
                throw new InvalidOperationException("Formatting cannot be applied because the table is empty. Add at least one row to the table first.");

            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                if (key == TableAttr.Istd)
                    PrepareRowBeforeSetIstd(row);

                if(!row.TablePr.ContainsKey(key))
                    row.TablePr.SetAttr(key, value);
            }
        }

        private void AutoFitToContents()
        {
            AllowAutoFit = true;
            RemoveAttrFromAllRows(TableAttr.PreferredWidth);
            RemoveAttrFromAllCells(CellAttr.PreferredWidth);
            UpdateLayout();
        }

        private void AutoFitToWindow()
        {
            AllowAutoFit = true;

            // WORDSNET-27499 Don't remove preffered width value from the cells, just convert them to percentages.
            UpdateCellPreferredWidthToPercentage();
            SetAttrOnAllRows(TableAttr.PreferredWidth, PreferredWidth.FromPercent(100));
            // WORDSNET-18407 Remove left indent attribute when table fits to window.
            RemoveAttrFromAllRows(TableAttr.LeftIndent);

            UpdateLayout();
        }

        /// <summary>
        /// Updates the preferred width of each cell in the table to a percentage of the table's width.
        /// </summary>
        /// <remarks>
        /// If the cell's preferred width is set to auto and the document is not compatible with Word 2013 or later,
        /// the method will return without making any changes. This is because calculating percentages for old documents
        /// with auto preferred width is complex.
        /// </remarks>
        private void UpdateCellPreferredWidthToPercentage()
        {
            PreferredWidth firstCellPreferedWidth = FirstRow.FirstCell.CellPr.PreferredWidth;
            if ((firstCellPreferedWidth.IsAuto) && !(this.Document.DocPr.CompatibilityOptions.IsWord2013OrLaterCompatible))
                return;

            // If the preferred width is fixed, use its value; otherwise, calculate the table width.
            double tableWidth = PreferredWidth.IsFixed && (PreferredWidth.Value > 0)
                ? PreferredWidth.Value
                : CalculateTableWidth();

            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                bool isRecalculationNeeded = PreferredWidthRecalculationNeeded(row);

                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                {
                    PreferredWidth cellPreferredWidth = cell.CellPr.PreferredWidth;
                    // Set the preferred width of the cell to a percentage value.
                    // If the current preferred width is already a percentage, use its value.
                    // Otherwise, calculate the percentage based on the cell and table width.
                    cell.CellPr.PreferredWidth = PreferredWidth.FromPercent(!isRecalculationNeeded
                        ? cellPreferredWidth.Value
                        : CalculateCellWidthPercent(cell, tableWidth));
                }
            }
        }

        /// <summary>
        /// Returns True if PreferredWidth for the cells in the specified row should be recalculated.
        /// </summary>
        private bool PreferredWidthRecalculationNeeded(Row row)
        {
            double totalInPct = 0;

            foreach (Cell cell in row.Cells)
            {
                if (!cell.CellPr.PreferredWidth.IsPercent)
                    return true;

                totalInPct += cell.CellPr.PreferredWidth.Value;
            }

            return System.Math.Round(totalInPct, 2) > 100.1;
        }

        /// <summary>
        /// Convert cell preferred width to a percentage of the table's width.
        /// </summary>
        /// <returns>The width of the table cell in percent.</returns>
        private double CalculateCellWidthPercent(Cell cell, double tableWidth)
        {
            // Define a constant value used for truncating the percentage to three decimal places.
            int trancateValue = 1000;

            double cellWidth = ConvertUtilCore.TwipToPoint(cell.CellPr.Width);
            double percent = (cellWidth / tableWidth) * 100;
            // We need to pass the 3 digits after the dots to MS Word to get the correct result.
            percent = System.Math.Truncate(percent * trancateValue) / trancateValue;

            return percent;
        }

        /// <summary>
        /// Calculates the total width of a table base on cell widths in the first row.
        /// </summary>
        /// <returns>The total width of the table in points.</returns>
        private double CalculateTableWidth()
        {
            double tableWidth = 0;
            for (Cell cell = FirstRow.FirstCell; cell != null; cell = cell.NextCell)
                tableWidth += ConvertUtilCore.TwipToPoint(cell.CellPr.Width);

            return tableWidth;
        }

        private void RemoveAttrFromAllCells(int key)
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                    cell.CellPr.Remove(key);
            }
        }

        private void AutoFitFixedColumnWidths()
        {
            // Set fixed table layout.
            AllowAutoFit = false;

            // In fixed layout, auto table width is determined by cell widths.
            PreferredWidth = PreferredWidth.Auto;

            // Remove the grid calculated earlier.
            RemoveCalculatedGrid();

            // In fixed layout table cells should have preferred width.
            SetCellPreferredToCurrentWidth();

            // Calculate grid spans from cell widths.
            CellWidthsToTableGrid();
        }

        /// <summary>
        /// Builds table grid, revised tabled grid if needed and populates table with table grid specific attributes.
        /// </summary>
        internal void CellWidthsToTableGrid()
        {
            // If the grid re-calculated from table properties is available, use it.
            TableGridAttr calculatedGrid = TablePr.CalculatedGrid;

            if (TableGridAttr.IsApplied(calculatedGrid) &&
                TablePr.GetDirectAttr(TableAttr.Sys_TableGrid) != null)
            {
                // Currently, the new algorithm does not support tables with revisions,
                // so do not bother with the revisions logic for now.
                // Just use the calculated grid instead of the grid in the document.

                // Do not use the "old" grid calculation logic implemented by TableGrid class if the "new" grid is available.
                return;
            }

            // Build two table grids: original is before-changes, final is after-changes.
            TableGridFromCellWidth tableGridOriginal = new TableGridFromCellWidth(this, false);
            TableGridFromCellWidth tableGridFinal = new TableGridFromCellWidth(this, true);

            TablePr.SetAttr(TableAttr.Sys_TableGrid, new IntList(tableGridOriginal.ColumnWidths));

            // Update before-rows.
            foreach (Row row in Rows)
            {
                tableGridOriginal.NextRow(row);
                UpdateRowGrid(row.TablePr, tableGridOriginal);

                // Update cell span.
                foreach (Cell cell in row.Cells)
                    cell.CellPr.SetAttr(CellAttr.Sys_CellSpan, tableGridOriginal.NextCell());
            }

            // If table grid is unchanged do not write changed table grid.
            if (ArrayUtil.IsArrayEqual(tableGridOriginal.ColumnWidths, tableGridFinal.ColumnWidths))
            {
                // It could be that table had format revision before but now it has not. Remove formatting revision then.
                TablePr.RejectFormatRevision();
                return;
            }

            // Table grid changes saved into DOCX without revision author/date so this only internally used revision.
            if (TablePr.FormatRevision == null)
                TablePr.FormatRevision = new FormatRevision(new TablePr(), "", DateTime.MinValue);

            // Store revised table grid.
            TablePr.RevisedPr.SetAttr(TableAttr.Sys_TableGrid, new IntList(tableGridFinal.ColumnWidths));

            foreach (Row row in Rows)
            {
                // Regardless of table's revision row can has or has not revision independently.
                TablePr rowPr = row.TablePr.RevisedPr;

                tableGridFinal.NextRow(row);
                UpdateRowGrid(rowPr, tableGridFinal);

                foreach (Cell cell in row.Cells)
                    cell.CellPr.RevisedPr.SetAttr(CellAttr.Sys_CellSpan, tableGridFinal.NextCell());
            }
        }

        /// <summary>
        /// Updates table grid row specific properties from table grid.
        /// </summary>
        /// <remarks>
        /// The method may overwrite width before/after specified in percent units.
        /// It has always worked this way.
        /// The method is only used for tables without a grid calculated by the "new" algorithm.
        /// </remarks>
        private static void UpdateRowGrid(TablePr rowPr, TableGridFromCellWidth tableGrid)
        {
            rowPr.SetAttr(TableAttr.Sys_GridBefore, tableGrid.GridBefore);
            rowPr.SetAttr(TableAttr.Sys_GridAfter, tableGrid.GridAfter);
            if (tableGrid.Before > 0)
                rowPr.WidthBeforeTwips = tableGrid.Before;
            else
                rowPr.Remove(TableAttr.WidthBefore);

            if (tableGrid.After > 0)
                rowPr.WidthAfterTwips = tableGrid.After;
            else
                rowPr.Remove(TableAttr.WidthAfter);
        }

        private void RemoveCalculatedGrid()
        {
            if (TablePr.CalculatedGrid != null)
                TablePr.Remove(TableAttr.Sys_CalculatedTableGrid);
        }

        private void SetCellPreferredToCurrentWidth()
        {
            for (Row row = FirstRow; row != null; row = row.NextRow)
            {
                Cell firstMergedCell = null;
                for (Cell cell = row.FirstCell; cell != null; cell = cell.NextCell)
                {
                    object width = cell.CellPr.GetDirectAttr(CellAttr.Width);
                    int cellWidth = (width == null) ? 0 : (int) width;

                    if (cellWidth <= 0)
                    {
                        // Try using preferred width then.
                        PreferredWidth cellPreferred = cell.CellFormat.PreferredWidth;
                        // I do not see a sensible way to use percent or auto width.
                        // The absence of computed Width in the model means that table grid was never calculated with those values.
                        // As they are being replaced with fixed preferred width, it does not make sense to use those preferred width values.
                        if (cellPreferred.IsFixed)
                        {
                            cellWidth = cellPreferred.ValueTwips;
                            cell.CellPr.Width = cellWidth;
                        }
                    }

                    if (cellWidth <= 0)
                    {
                        // Use the same default MS Word uses when loading a document having cell width not specified in a fixed layout table.
                        cellWidth = TableGridMetrics.DefaultFixedColumnWidthTwips * cell.CellPr.GridSpan;
                        cell.CellPr.Width = cellWidth;
                    }

                    switch (cell.CellPr.HorizontalMerge)
                    {
                        case CellMerge.First:
                            // The cell that is first in a series of merged cells, becomes a normal cell
                            // and it will now take on the width of all merged cells.
                            firstMergedCell = cell;
                            cell.CellPr.PreferredWidth = PreferredWidth.FromTwipsSafe(cellWidth);
                            break;
                        case CellMerge.Previous:
                            if (firstMergedCell != null)
                            {
                                int mergedWidth = firstMergedCell.CellPr.PreferredWidth.ValueTwips + cellWidth;
                                firstMergedCell.CellPr.PreferredWidth = PreferredWidth.FromTwipsSafe(mergedWidth);
                                // Merged cell preferred is normally ignored.
                                cell.CellPr.PreferredWidth = PreferredWidth.Auto;
                            }
                            else
                            {
                                // Invalid case. A cell merged to previous while there was no first.
                                // Treat it as a regular cell.
                                cell.CellPr.PreferredWidth = PreferredWidth.FromTwipsSafe(cellWidth);
                                firstMergedCell = null;
                            }
                            break;
                        case CellMerge.None:
                            cell.CellPr.PreferredWidth = PreferredWidth.FromTwipsSafe(cellWidth);
                            firstMergedCell = null;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown cell merge type.");
                    }

                    // This may have unexpected results if the current cell width in the model
                    // is actually less than minimal possible cell width determined by cell margins.
                    // But it is difficult to interpret what the expected result should be in that case,
                    // so the code is still using the widths in the model.
                }
            }
        }

        /// <summary>
        /// andrnosk: WORDSNET-7926 Mimic MS Word behavior.
        /// Clear all row attributes except Height, HeightRule, PreferredWidth and AllowAutoFit.
        /// </summary>
        private void PrepareRowBeforeSetIstd(Row row)
        {
            object savedHeight = row.TablePr[TableAttr.RowHeight];
            object savedAllowAutoFit = row.TablePr[TableAttr.AllowAutoFit];
            object savedPreferredWidth = row.TablePr[TableAttr.PreferredWidth];

            row.RowFormat.ClearFormatting();

            // MS Word does not preserve cell VerticalAlignment upon style applying.
            RemoveAttrFromAllCells(CellAttr.VerticalAlignment);

            if (savedHeight != null)
                row.TablePr.SetAttr(TableAttr.RowHeight, savedHeight);
            if (savedAllowAutoFit != null)
                row.TablePr.SetAttr(TableAttr.AllowAutoFit, savedAllowAutoFit);
            if (savedPreferredWidth != null)
                row.TablePr.SetAttr(TableAttr.PreferredWidth, savedPreferredWidth);
        }

        /// <summary>
        /// Gets the attribute from the first row. If there is no row, gets the default value.
        /// </summary>
        private object FetchFirstRowAttr(int key)
        {
            Row row = FirstRow;

            if (row == null)
                return TablePr.FetchDefaultAttr(key);

            // AM. Since FetchRowAttr needs Istd to get base table style
            // we must not use FetchRowAttr to get Istd value to prevent infinite loop.
            // In fact Istd attribute is not inheritable attribute so we really don't need to resolve it.
            if (key == TableAttr.Istd)
            {
                // AM. There can be case when row.TablePr is null.
                // For example when we resolve BiDi table alignment row properties is not ready.
                return (row.TablePr != null) ? row.TablePr.Istd : TablePr.FetchDefaultAttr(TableAttr.Istd);
            }

            return ((IRowAttrSource)row).FetchRowAttr(key);
        }

        /// <summary>
        /// Facades, created on demand.
        /// </summary>
        private RowCollection mRowsCache;

        private TablePr mTablePr;

        /// <summary>
        /// That's how many columns in a table MS Word allows maximum.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxColumns = 63;

        /// <summary>
        /// MSW adds this cell spacing when "Allow spacing between cells" checkbox
        /// changes its state from unchecked to checked.
        /// </summary>
        /// <remarks>
        /// The value of this constant depends on operating system property "Measurement system".
        /// (Region -> Formats -> Additional settings -> Numbers -> Measurement system)
        /// It is 7 twips for U.S. measurement system, and it is 11 twips for Metric measurement system.
        /// Here we relies on U.S. measurement system.
        /// In case of customer request, we should take measurement system into consideration
        /// in order to receive correct value.
        /// </remarks>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int InitialCellSpacing = 7;
    }
}
