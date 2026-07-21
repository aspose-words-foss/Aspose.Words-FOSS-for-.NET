// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using System;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.Html.Reader.CommonBorder;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Responsible for importing HTML table.
    /// </summary>
    internal class HtmlTableReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="builder">Current builder</param>
        /// <param name="cssStyleTracker">CSS style tracker.</param>
        /// <param name="tableFormatter">Table formatter.</param>
        /// <param name="commonBorderResolver">Common border resolver</param>
        /// <param name="nodeProcessor">Callback interface for node processing</param>
        /// <param name="applyFormattingAsMsWord">
        /// Controls whether the reader should mimic MS Word when formatting imported tables.
        /// </param>
        /// <param name="htmlBlockReader">
        /// Creates <see cref="HtmlBlock"/> from 'div', 'body' and 'blockquote' HTML elements.
        /// </param>
        /// <param name="hyperlinkResolver">
        /// Adds bookmarks for imported HTML elements with 'id' attributes.
        /// </param>
        internal HtmlTableReader(
            DocumentBuilder builder,
            CssStyleTracker cssStyleTracker,
            ITableFormatter tableFormatter,
            HtmlCommonBorderResolver commonBorderResolver,
            IHtmlNodeProcessor nodeProcessor,
            bool applyFormattingAsMsWord,
            HtmlBlockReader htmlBlockReader,
            HtmlHyperlinkResolver hyperlinkResolver)
        {
            Debug.Assert(builder != null);
            Debug.Assert(cssStyleTracker != null);
            Debug.Assert(tableFormatter != null);
            Debug.Assert(commonBorderResolver != null);
            Debug.Assert(nodeProcessor != null);
            Debug.Assert(htmlBlockReader != null);

            mBuilder = builder;
            mCssStyleTracker = cssStyleTracker;
            mTableFormatter = tableFormatter;
            mCommonBorderResolver = commonBorderResolver;
            mNodeProcessor = nodeProcessor;
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
            mHtmlBlockReader = htmlBlockReader;
            mHyperlinkResolver = hyperlinkResolver;
        }

        /// <summary>
        /// Imports an HTML table into the document.
        /// </summary>
        /// <param name="htmlTable">HTML table.</param>
        /// <param name="isTopmostTable">
        /// Indicates whether the table is topmost (is not nested in another table) in this HTML reading session.
        /// </param>
        /// <param name="tableNode">HTML table node.</param>
        internal Table ProcessTable(
            HtmlTable htmlTable,
            bool isTopmostTable,
            HtmlElementNode tableNode)
        {
            Debug.Assert(htmlTable != null);

            mHtmlTable = htmlTable;
            if ((mHtmlTable.RowCount == 0) || (mHtmlTable.ColumnCount == 0))
                return null;

            Table table = mBuilder.StartTable();

            if (mHyperlinkResolver != null)
            {
                mHyperlinkResolver.AddPendingBookmarkForTableIfNeeded(tableNode, table);
            }

            // In HtmlReader nested tables are handled first so collect tables here in HtmlTableReader.
            mCommonBorderResolver.CollectTable(table, mCssStyleTracker.ElementDeclarations);
            mIsRtlTable = mCssStyleTracker.IsBlockRtl();

            // WORDSNET-954 InsertHtml sometimes does not stop marking paragraphs as list members.
            // Table always starts with a new paragraph in MS Word, reset paragraph and list formatting.
            mBuilder.ParagraphFormat.ClearFormatting();

            // Top captions.
            int topCaptionCount = 0;
            for (int i = 0; i < mHtmlTable.CaptionCount; i++)
            {
                HtmlTableCaption tableCaption = mHtmlTable.GetCaption(i);
                if (tableCaption.IsTopCaption)
                {
                    topCaptionCount++;
                    ProcessCaption(tableCaption);
                }
            }

            // WORDSNET-21609 Creates inside horizontal and vertical borders from a table CSS declaration.
            CssDeclarationCollection declarations = mCssStyleTracker.ElementDeclarations;
            mInsideHorizontalCssBorder = CssBorder.CreateInsideBorder(declarations, BorderType.Horizontal);
            mInsideVerticalCssBorder = CssBorder.CreateInsideBorder(declarations, BorderType.Vertical);
            // Table rows.
            mCurrentRowGroup = null;
            mCurrentHtmlRowCanBeTableHeader = true;
            for (int rowIdx = 0; rowIdx < mHtmlTable.RowCount; rowIdx++)
            {
                ProcessRow(rowIdx);
                // WORDSNET-21609 Applies inside horizontal and vertical borders to row TablePr attributes.
                Row curRow = mBuilder.CurrentParagraph.ParentRow;
                if (curRow != null)
                {
                    SetInsideBorders(curRow);
                }
            }
            if (mCurrentRowGroup != null)
            {
                Debug.Assert(mCssStyleTracker.CurrentElement == mCurrentRowGroup.Node);
                mCssStyleTracker.PopElement();
                mCurrentRowGroup = null;
            }

            // Bottom captions.
            int bottomCaptionCount = 0;
            for (int i = 0; i < mHtmlTable.CaptionCount; i++)
            {
                HtmlTableCaption tableCaption = mHtmlTable.GetCaption(i);
                if (!tableCaption.IsTopCaption)
                {
                    bottomCaptionCount++;
                    ProcessCaption(tableCaption);
                }
            }
            // Set table attributes after all rows have been processed because they will update all rows.
            // Word turns this option on when importing.
            table.AllowAutoFit = true;

            mTableFormatter.ToTable(
                table,
                topCaptionCount,
                bottomCaptionCount,
                mHtmlTable.FirstRowFirstCellLeftBorder,
                mHtmlTable.FirstRowLastCellRightBorder);
            ApplyTableBorders(table, topCaptionCount, bottomCaptionCount);
            ApplyIndents(table);

            mHtmlBlockReader.UpdateTable(table);

            // WORDSNET-14898 Table borders were applied to table and possibly to some cells.
            // Now we should remove table borders from cells, if applying them only to table is enough.
            if (mHtmlTable.IsBorderCollapsed)
            {
                RemoveCellBordersDuplicatingTableBorders(topCaptionCount == 0, bottomCaptionCount == 0);
            }

            mBuilder.EndTable();

            // WORDSNET-996 Invalid table width when inserting it into a table cell.
            // WORDSNET-19685 Update widths for the nested table if it was inserted into another table using InsertHtml.
            // We should update layout for topmost tables only to reduce performance impact. UpdateLayout method updates
            // the current table and all nested tables.
            if (!table.IsNested || isTopmostTable)
                table.UpdateLayout();

            return table;
        }

        private void SetInsideBorders(Row row)
        {
            Debug.Assert(row != null);
            TablePr rowTablePr = row.TablePr;
            if ((mInsideHorizontalCssBorder != null) &&
                (mInsideHorizontalCssBorder.IsVisible || mInsideHorizontalCssBorder.IsAwBorder))
            {
                rowTablePr.SetAttr(TableAttr.BorderHorizontal, new Border());
                mInsideHorizontalCssBorder.ToModelBorder(rowTablePr.BorderHorizontal);
            }
            if ((mInsideVerticalCssBorder != null) &&
                (mInsideVerticalCssBorder.IsVisible || mInsideVerticalCssBorder.IsAwBorder))
            {
                rowTablePr.SetAttr(TableAttr.BorderVertical, new Border());
                mInsideVerticalCssBorder.ToModelBorder(rowTablePr.BorderVertical);
            }
        }

        private void ProcessRow(int rowIndex)
        {
            mCurrentHtmlRow = mHtmlTable[rowIndex];

            // We push the row group only when it is changed to ensure each row group is pushed only once. Repeated pushing
            // of elements is blocked because it affects CSS counter values.
            if (mCurrentHtmlRow.RowGroup != mCurrentRowGroup)
            {
                if (mCurrentRowGroup != null)
                {
                    mCssStyleTracker.PopElement();
                }
                mCurrentRowGroup = mCurrentHtmlRow.RowGroup;
                Debug.Assert(mCurrentRowGroup != null);
                mCssStyleTracker.PushElement(mCurrentRowGroup.Node, true);
            }

            mCssStyleTracker.PushElement(mCurrentHtmlRow.Node, true);

            if (mCssStyleTracker.ElementDisplayState == HtmlElementDisplayState.Visible)
            {
                // Note that the row formatting will be cleared when the first cell of the row is created.
                for (int colIdx = 0; colIdx < mCurrentHtmlRow.CellCount; colIdx++)
                    ProcessCell(mCurrentHtmlRow[colIdx], rowIndex, colIdx);

                // Set row properties. Some of them in HTML are table properties,
                // but in Aspose.Words table properties are stored per row at the moment.
                Row row = mBuilder.CurrentParagraph.ParentRow;
                CssStylesToRow(row);
                if (!MathUtil.IsMinValue(mCurrentHtmlRow.MaxCellHeightInRow) && (mCurrentHtmlRow.MaxCellHeightInRow > row.RowFormat.Height))
                {
                    row.RowFormat.Height = mCurrentHtmlRow.MaxCellHeightInRow;
                    row.RowFormat.HeightRule = HeightRule.AtLeast;
                }

                // WORDSNET-14812 Read row's HeightRule.Exactly value.
                string heightRuleValue = mCssStyleTracker.ElementDeclarations.GetIdentifier(HtmlConstants.HeightRule);
                if (heightRuleValue == HtmlConstants.HeightRuleExactly)
                    row.RowFormat.HeightRule = HeightRule.Exactly;

                // WORDSNET-22050 Read `AllowBreakAcrossPages` row property value.
                string allowBreakAcrossPagesValue = mCssStyleTracker.ElementDeclarations.GetIdentifier("page-break-inside");
                if (string.Equals(allowBreakAcrossPagesValue, "avoid", StringComparison.OrdinalIgnoreCase))
                    row.RowFormat.AllowBreakAcrossPages = false;

                // WORDSNET-10504 Enable "Repeat as the header row at the top of each page" option when importing HTML thead rows.
                if (mCurrentHtmlRowCanBeTableHeader)
                {
                    if (mCurrentHtmlRow.RowGroup.IsThead)
                        row.RowFormat.HeadingFormat = true;
                    else
                        mCurrentHtmlRowCanBeTableHeader = false;
                }

                if (mHyperlinkResolver != null)
                {
                    mHyperlinkResolver.EndInlineBookmarkIfNeeded(mCurrentHtmlRow.Node);
                }
                mBuilder.EndRow();
            }

            mCssStyleTracker.PopElement();
            mCurrentHtmlRow = null;
        }

        /// <summary>
        /// Processes HTML cell.
        /// </summary>
        /// <param name="htmlCell">HTML cell for processing.</param>
        /// <param name="rowIndex">Table row index of the processed cell. Should be <c>-1</c> for caption rows.</param>
        /// <param name="colIndex">Table column index of the processed cell. Should be <c>-1</c> for caption rows.</param>
        private void ProcessCell(HtmlCell htmlCell, int rowIndex, int colIndex)
        {
            mCurrentHtmlCell = htmlCell;

            if (htmlCell.Node != null)
                mCssStyleTracker.PushElement(htmlCell.Node, true);

            // WORDSNET-19889 There is a limit on the number of table columns that MS Word supports. If the number of
            // columns in the source table exceeds that limit, all content from extra columns is imported into the last column.
            Cell modelCell;
            if (colIndex < Table.MaxColumns)
            {
                modelCell = mBuilder.InsertCell();

                // WORDSNET-28462 If the cell that we've just inserted starts a new table row, we need to clear its formatting.
                // Otherwise, it will inherit default table row formatting specified in the document builder, which is not what
                // we want. Instead, we want to load the formatting completely from HTML.
                if (modelCell.IsFirstCell)
                {
                    modelCell.ParentRow.TablePr.Clear();
                }

                htmlCell.ModelCell = modelCell;

                // WORDSNET-12113 Incorrect image size after importing html.
                // AW downsizes the image to fit the cell (this behavior was implemented in WORDSNET-10174),
                // because the table has AllowAutoFit=false style at the moment when the image is being inserted.
                // But it is not allowed to set AllowAutoFit=true immediately after creating the table because
                // it does not contain any rows.
                if (!modelCell.ParentTable.AllowAutoFit)
                    modelCell.ParentTable.AllowAutoFit = true;

                // Need to clear formatting first because it might be changed by a previous cell.
                mBuilder.CellFormat.ClearAllFormatting();

                // WORDSNET-16021 By default, MS Word sets HideMark to true for all cells.
                // Note that this flag may be cleared later, after content of the cell is fully loaded.
                modelCell.CellPr.HideMark = true;

                // Html table cell has padding equal 1px by default, but element that has been
                // turned into table cells via 'display:table-cell' has 0px padding.
                // In case a table is defined using the 'display: table' or 'display:inline-table'
                // property the padding is not set and should be set to 0px.
                if (mHtmlTable.IsDisplayTable)
                {
                    mBuilder.CellFormat.LeftPadding = 0;
                    mBuilder.CellFormat.RightPadding = 0;
                }

                mBuilder.CellFormat.HorizontalMerge = htmlCell.HorizontalMerge;
                mBuilder.CellFormat.VerticalMerge = htmlCell.VerticalMerge;
                mBuilder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;

                if (htmlCell.Node != null)
                {
                    bool isCellEmpty = !htmlCell.Node.ContainsText();
                    CssStylesToCell(
                        modelCell,
                        rowIndex,
                        htmlCell.ColSpan,
                        htmlCell.UnrestrictedColSpan,
                        htmlCell.RowSpan,
                        isCellEmpty);
                }

                // Some attributes should be also taken from the first nodes in corresponding chains.
                HtmlCell mergeFirstHtmlCell = htmlCell.HorizontalMergeFirstCell;
                if (mergeFirstHtmlCell == null)
                    mergeFirstHtmlCell = htmlCell.VerticalMergeFirstCell;
                if (mergeFirstHtmlCell != null)
                {
                    Cell mergeFirstCell = mergeFirstHtmlCell.ModelCell;
                    Debug.Assert(mergeFirstCell != null);

                    if (mergeFirstHtmlCell.VerticalMerge != CellMerge.None)
                    {
                        // WORDSNET-21609 CopyBorders method should take into account bottom and top border
                        // properties declared in a declaration block for the TD element selector declared in a <style>
                        // element, if the block exists. This is from MS Word behavior. If there is a declaration block
                        // for the TD element selector that contains border properties then these properties are applied
                        // to top and bottom borders of merged cells during import from HTML, or otherwise if there is
                        // no the TD selector with top and border border properties then the inner borders of vertical
                        // merged cells are not set.
                        // To get border properties from TD element selector the next block of code adds a TD element to
                        // the style tracker stack and obtains the default element declaration for the processed table
                        // that contains border properties from the TD element selector declaration block if they exist
                        // there.
                        mCssStyleTracker.PushElement(new HtmlElementNode("td"), false);
                        Border defaultTopBorder = new Border();
                        CssBorder.CreateBorder(mCssStyleTracker.ElementDeclarations, BorderType.Top, true)
                            .ToModelBorder(defaultTopBorder);
                        Border defaultBottomBorder = new Border();
                        CssBorder.CreateBorder(mCssStyleTracker.ElementDeclarations, BorderType.Bottom, true)
                            .ToModelBorder(defaultBottomBorder);
                        mCssStyleTracker.PopElement();

                        CopyBorders(
                            mergeFirstCell,
                            modelCell,
                            mergeFirstHtmlCell.RowSpan,
                            defaultTopBorder,
                            defaultBottomBorder,
                            mInsideHorizontalCssBorder.IsVisible);
                    }
                    else
                    {
                        CopyBorders(mergeFirstCell, modelCell);
                    }
                }
            }
            else
            {
                modelCell = mCurrentHtmlRow[Table.MaxColumns - 1].ModelCell;
            }

            // Start of bookmark for an empty row.
            if ((colIndex == 0) && (mHyperlinkResolver != null))
            {
                mHyperlinkResolver.StartInlineBookmarkIfNeeded(mCurrentHtmlRow.Node);
            }

            if (htmlCell.Node != null)
            {
                if (mHyperlinkResolver != null)
                {
                    mHyperlinkResolver.StartInlineBookmarkIfNeeded(htmlCell.Node);
                }
                // Delegate to the node processor.
                mNodeProcessor.ProcessCell(htmlCell.Node);
                if (mHyperlinkResolver != null)
                {
                    mHyperlinkResolver.EndInlineBookmarkIfNeeded(htmlCell.Node);
                }
                mCssStyleTracker.PopElement();
            }
            else if (colIndex < Table.MaxColumns)
            {
                // WORDSNET-9232 Clear paragraph formatting for empty cells.
                // WORDSNET-19889 It shouldn't clear the formatting when empty cells from ignored columns are
                // processed.
                mBuilder.ParagraphFormat.ClearFormatting();
            }

            // WORDSNET-11248 and 11618 - Bookmark position is changed after conversion from Doc to Html.
            // In DocumentBuilder paragraph containing only bookmarks is considered empty.
            // In DocumentBuilder when current paragraph is empty new table is inserted before this paragraph.
            // So when bookmarks are located just before a table in HTML document they appear after the table in the model.
            // Here we use MS Word approach and import such bookmarks at the begining of the paragraph of the first table's cell.
            if ((rowIndex == 0) && (colIndex == 0) && (modelCell.ParentTable.NextSibling is Paragraph))
            {
                Paragraph nextPara = (Paragraph)modelCell.ParentTable.NextSibling;
                while (nextPara.LastChild != null)
                {
                    if ((nextPara.LastChild.NodeType != NodeType.BookmarkStart) &&
                        (nextPara.LastChild.NodeType != NodeType.BookmarkEnd))
                    {
                        break;
                    }

                    string bookmarkName;
                    switch (nextPara.LastChild.NodeType)
                    {
                        case NodeType.BookmarkStart:
                        {
                            BookmarkStart bookmarkStart = (BookmarkStart)nextPara.LastChild;
                            bookmarkName = bookmarkStart.Name;
                            break;
                        }
                        case NodeType.BookmarkEnd:
                        {
                            BookmarkEnd bookmarkEnd = (BookmarkEnd)nextPara.LastChild;
                            bookmarkName = bookmarkEnd.Name;
                            break;
                        }
                        default:
                        {
                            bookmarkName = string.Empty;
                            break;
                        }
                    }

                    if ((mHyperlinkResolver != null) && !mHyperlinkResolver.ContainsStartedAnchorBookmark(bookmarkName))
                    {
                        break;
                    }

                    Node bookmarkNode = nextPara.LastChild;
                    bookmarkNode.Remove();
                    modelCell.FirstParagraph.Insert(bookmarkNode, null, true);
                }
            }
        }

        private void ProcessCaption(HtmlTableCaption tableCaption)
        {
            HtmlCell firstCell = new HtmlCell(tableCaption.Node, null);
            if (mHtmlTable.ColumnCount > 1)
            {
                firstCell.HorizontalMerge = CellMerge.First;
            }
            ProcessCell(firstCell, -1, -1);

            // First cell has just been processed.
            for (int i = 1; i < mHtmlTable.ColumnCount; i++)
            {
                HtmlCell mergedCell = new HtmlCell(null);
                mergedCell.HorizontalMerge = CellMerge.Previous;
                mergedCell.HorizontalMergeFirstCell = firstCell;
                ProcessCell(mergedCell, -1, -1);
            }

            mBuilder.EndRow();
        }

        private static void CopyBorders(Cell source, Cell destination)
        {
            foreach (int borderKey in CellPr.PossibleBorderKeys.Values)
            {
                destination.CellPr.Remove(borderKey);
                Border sourceBorder = source.CellPr[borderKey] as Border;
                if ((sourceBorder != null) && (!sourceBorder.IsInherited))
                {
                    destination.CellPr.SetAttr(borderKey, sourceBorder.Clone());
                }
            }
        }

        private static void CopyBorders(
            Cell source,
            Cell destination,
            int rowSpan,
            Border defaultTopBorder,
            Border defaultBottomBorder,
            bool setNilForLast)
        {
            bool isLast = (destination.RowIndex - source.RowIndex + 1) == rowSpan;
            foreach (BorderType borderType in CellPr.PossibleBorderKeys.Keys)
            {
                int borderKey = CellPr.PossibleBorderKeys[borderType];
                destination.CellPr.Remove(borderKey);

                Border sourceBorder = source.CellPr[borderKey] as Border;

                // WORDSNET-28238 - 'borders: none' is not always processed correctly in table cells,
                // so we should skip empty borders here.
                if (sourceBorder == null)
                    continue;

                if (!sourceBorder.IsInherited &&
                    sourceBorder.IsVisible &&
                    (borderType != BorderType.Top) &&
                    ((borderType != BorderType.Bottom) || isLast))
                {
                    destination.CellPr.SetAttr(borderKey, sourceBorder.Clone());
                }
                else if ((borderType == BorderType.Top) || (borderType == BorderType.Bottom))
                {
                    Border defaultBorder = (borderType == BorderType.Top)
                        ? defaultTopBorder
                        : defaultBottomBorder;
                    if (defaultBorder.IsVisible && !defaultBorder.IsNil)
                    {
                        Border newBorder = new Border(
                            defaultBorder.LineStyle,
                            defaultBorder.RawLineWidth,
                            defaultBorder.ColorInternal);
                        destination.CellPr.SetAttr(borderKey, newBorder);
                    }
                }
            }
            // The bottom border of the top vertical merged cell should be replaced by a nil border if there are inner
            // horizontal borders in the table, or otherwise it should be an empty border.
            if (isLast)
            {
                int borderKey = CellPr.PossibleBorderKeys[BorderType.Bottom];
                Border oldBorder = source.CellPr[borderKey] as Border;
                if (oldBorder != null)
                {
                    Border newBorder = setNilForLast
                        ? Border.CreateNilBorder()
                        : new Border();
                    source.CellPr.SetAttr(borderKey, newBorder);
                }
            }
        }

        /// <summary>
        /// Applies CSS styles to a model row.
        /// </summary>
        private void CssStylesToRow(Row row)
        {
            Debug.Assert(row != null);
            foreach (CssDeclaration declaration in mCssStyleTracker.ElementDeclarations)
                ((CssComputedDeclaration)declaration).ToRow(row);
        }

        /// <summary>
        /// Applies CSS styles to a model cell.
        /// </summary>
        private void CssStylesToCell(
            Cell cell,
            int rowIndex,
            int colSpan,
            int unrestrictedColSpan,
            int rowSpan,
            bool isCellEmpty)
        {
            Debug.Assert(cell != null);

            CssBoxBorders appliedBorders = mHtmlTable.IsBorderCollapsed
                ? ApplyCollapsedBordersToCell(cell, cell.ColumnIndex, rowIndex, colSpan, rowSpan)
                : ApplySeparateBordersToCell(cell.CellFormat.Borders);

            foreach (CssDeclaration declaration in mCssStyleTracker.ElementDeclarations)
            {
                ((CssComputedDeclaration)declaration).ToCellFormat(cell.CellFormat);
            }

            ApplyWidthStyleToCell(cell, unrestrictedColSpan, isCellEmpty, appliedBorders);
            ApplyRowAndColumnBackgroundColorToCell(cell);

            // WORDSNET-19526 CSS propery "writing-mode" doesn't support text
            // rotated 90 degrees counterclockwise (upward). We use the "transform" property to emulate this.
            ApplyRotateTransformation(cell);
        }

        private void ApplyRotateTransformation(Cell cell)
        {
            // The "writing-mode" CSS property with value 'tb-rl' always parses as TextOrientation.VerticalFarEast.
            if (cell.CellFormat.Orientation == TextOrientation.VerticalFarEast)
            {
                CssDeclaration transformDeclaration = mCssStyleTracker.ElementDeclarations["transform"];

                if ((transformDeclaration == null) ||
                    (transformDeclaration.Value.FirstValue.ValueType != CssValueType.Function))
                {
                    return;
                }

                CssFunctionValue functionValue = (CssFunctionValue)transformDeclaration.Value.FirstValue;
                if ((functionValue.Name == "rotate") &&
                    (functionValue.Arguments.Count == 1) &&
                    (functionValue.Arguments[0].ValueType == CssValueType.Degree) &&
                    MathUtil.AreEqual(180d, functionValue.Arguments[0].DoubleValue))
                {
                    cell.CellFormat.Orientation = TextOrientation.Upward;
                }
            }
        }

        private void ApplyRowAndColumnBackgroundColorToCell(Cell cell)
        {
            // This method applies only background color that is specified indirectly: on table columns and rows.
            // Background color specified directly on table cells is applied somewhere else.
            if (mCssStyleTracker.ElementDeclarations["background-color"] != null)
                return;

            if (mCurrentHtmlRow != null)
            {
                CssComputedDeclaration rowBackgroundDeclaration =
                    (CssComputedDeclaration)mCurrentHtmlRow.Declarations["background-color"];
                if (rowBackgroundDeclaration != null)
                {
                    rowBackgroundDeclaration.ToCellFormat(cell.CellFormat);
                    return;
                }

                if (mCurrentHtmlRow.RowGroup != null)
                {
                    CssComputedDeclaration rowGroupBackgroundDeclaration =
                        (CssComputedDeclaration)mCurrentHtmlRow.RowGroup.Declarations["background-color"];
                    if (rowGroupBackgroundDeclaration != null)
                    {
                        rowGroupBackgroundDeclaration.ToCellFormat(cell.CellFormat);
                        return;
                    }
                }
            }

            if (mHtmlTable.ColElementCount > cell.ColumnIndex)
            {
                HtmlCol col = mHtmlTable.GetCol(cell.ColumnIndex);
                CssDeclarationCollection colDeclarations = col.Declarations;

                CssComputedDeclaration colBackgroundDeclaration = (CssComputedDeclaration)colDeclarations["background-color"];
                if (colBackgroundDeclaration != null)
                {
                    colBackgroundDeclaration.ToCellFormat(cell.CellFormat);
                    return;
                }

                if (col.ColGroup != null)
                {
                    CssComputedDeclaration groupBackgroundDeclaration =
                        (CssComputedDeclaration)col.ColGroup.Declarations["background-color"];
                    if (groupBackgroundDeclaration != null)
                        groupBackgroundDeclaration.ToCellFormat(cell.CellFormat);
                }
            }
        }

        private void ApplyWidthStyleToCell(Cell cell, int colSpan, bool isCellEmpty, CssBoxBorders appliedBorders)
        {
            CssDeclarationCollection colDeclarations = CssDeclarationCollection.Empty;
            CssDeclarationCollection[] spannedColDeclarations = null;
            if (cell.ColumnIndex < mHtmlTable.ColElementCount)
            {
                HtmlCol col = mHtmlTable.GetCol(cell.ColumnIndex);
                colDeclarations = col.Declarations;

                // If the cell spans more than one column, get declarations of additional COL elements.
                // Note that we create the corresponding array only when needed, because it's a rare case where a table
                // contains both COL elements and cells that span multiple columns.
                if (colSpan >= 2)
                {
                    int lastColumnIndex = System.Math.Min(mHtmlTable.ColElementCount, cell.ColumnIndex + colSpan) - 1;
                    spannedColDeclarations = new CssDeclarationCollection[lastColumnIndex - cell.ColumnIndex];
                    for (int colIndex = cell.ColumnIndex + 1; colIndex <= lastColumnIndex; colIndex++)
                    {
                        HtmlCol spannedCol = mHtmlTable.GetCol(colIndex);
                        spannedColDeclarations[colIndex - cell.ColumnIndex - 1] = spannedCol.Declarations;
                    }
                }
            }

            CssWidthStyleConverter.ToCellFormat(
                cell.CellFormat,
                mCssStyleTracker.ElementDeclarations,
                colDeclarations,
                spannedColDeclarations,
                appliedBorders,
                !mApplyFormattingAsMsWord,
                mHtmlTable.IsBorderCollapsed,
                isCellEmpty);

            if (cell.IsFirstCell && cell.ParentRow.IsFirstRow)
                mHtmlTable.FirstRowFirstCellLeftBorder = appliedBorders.Left;

            if (cell.IsLastCell && cell.ParentRow.IsFirstRow)
                mHtmlTable.FirstRowLastCellRightBorder = appliedBorders.Right;
        }

        /// <summary>
        /// Applies CSS separate border styles to model cell borders.
        /// </summary>
        private CssBoxBorders ApplySeparateBordersToCell(BorderCollection cellBorders)
        {
            CssBoxBorders cssBorders = (mCurrentHtmlCell.CssBorders != null)
                ? mCurrentHtmlCell.CssBorders
                : CssBoxBorders.CreateBorders(mCssStyleTracker.ElementDeclarations, true);

            if (cssBorders.Top.IsVisible || cssBorders.Top.IsAwBorder)
                cssBorders.Top.ToModelBorder(cellBorders.Top);
            if (cssBorders.Bottom.IsVisible || cssBorders.Bottom.IsAwBorder)
                cssBorders.Bottom.ToModelBorder(cellBorders.Bottom);

            // When importing Rtl direction table into the AW model the left and the right
            // cell borders are switched.
            if (cssBorders.Left.IsVisible || cssBorders.Left.IsAwBorder)
            {
                cssBorders.Left.ToModelBorder(mIsRtlTable ? cellBorders.Right : cellBorders.Left);
            }

            if (cssBorders.Right.IsVisible || cssBorders.Right.IsAwBorder)
            {
                cssBorders.Right.ToModelBorder(mIsRtlTable ? cellBorders.Left : cellBorders.Right);
            }

            return cssBorders;
        }

        /// <summary>
        /// Applies CSS collapsed borders to a model cell.
        /// </summary>
        private CssBoxBorders ApplyCollapsedBordersToCell(Cell cell, int cellIndex, int rowIndex, int colSpan, int rowSpan)
        {
            CssBoxBorders cellBorders = (mCurrentHtmlCell.CssBorders != null)
                ? mCurrentHtmlCell.CssBorders
                : CssBoxBorders.CreateBorders(mCssStyleTracker.ElementDeclarations, true);

            // WORDSNET-10395 Cells with negative row indexes are caption cells
            // and they should not participate in the border conflict resolution.
            if (rowIndex < 0)
            {
                cellBorders.ToModelBorders(cell.CellFormat.Borders);
            }
            else
            {
                BorderType[] borderTypes = new BorderType[]
                {
                    BorderType.Top,
                    BorderType.Right,
                    BorderType.Bottom,
                    BorderType.Left
                };
                CssBorder[] appliedBorders = new CssBorder[4];
                int appliedBorderIndex = 0;
                foreach (BorderType borderType in borderTypes)
                {
                    CssBorder winnerBorder = GetWinnerBorder(borderType, cellIndex, rowIndex, colSpan, rowSpan);

                    // When importing Rtl direction table into the AW model the left and the right
                    // cell borders are switched.
                    BorderType rtlAwareBorderType = borderType;
                    if (mIsRtlTable)
                    {
                        if (borderType == BorderType.Left)
                        {
                            rtlAwareBorderType = BorderType.Right;
                        }
                        else if (borderType == BorderType.Right)
                        {
                            rtlAwareBorderType = BorderType.Left;
                        }
                    }

                    Border modelBorder = cell.CellFormat.Borders[rtlAwareBorderType];

                    if (winnerBorder.IsVisible || winnerBorder.IsAwBorder)
                    {
                        winnerBorder.ToModelBorder(modelBorder);
                        appliedBorders[appliedBorderIndex] = winnerBorder;
                    }
                    else
                    {
                        // WORDSNET-21609 Adds nil-borders instead of empty ones for inner table borders.
                        if (((((rtlAwareBorderType == BorderType.Left) || (rtlAwareBorderType == BorderType.Right)) && mInsideVerticalCssBorder.IsVisible) ||
                             (((rtlAwareBorderType == BorderType.Top) || (rtlAwareBorderType == BorderType.Bottom)) && mInsideHorizontalCssBorder.IsVisible)) &&
                                ((cellIndex != 0) || (rtlAwareBorderType != BorderType.Left)) &&
                                ((cellIndex != (mCurrentHtmlRow.CellCount - 1)) || (rtlAwareBorderType != BorderType.Right)) &&
                                ((rowIndex != 0) || (rtlAwareBorderType != BorderType.Top)) &&
                                ((rowIndex != (mHtmlTable.RowCount - 1)) || (rtlAwareBorderType != BorderType.Bottom)))
                        {

                            winnerBorder.ToModelBorder(modelBorder);
                            modelBorder.IsNil = true;
                        }
                    }

                    ++appliedBorderIndex;
                }
                cellBorders = new CssBoxBorders(
                    appliedBorders[0],
                    appliedBorders[1],
                    appliedBorders[2],
                    appliedBorders[3]);
            }
            return cellBorders;
        }

        /// <summary>
        /// Returns cell border specified directly on cell.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        private CssBorder GetCellBorder(BorderType borderType)
        {
            if ((mCurrentHtmlCell == null) || (mCurrentHtmlCell.CssBorders == null))
                return CssBorder.Empty;

            return mCurrentHtmlCell.CssBorders[borderType];
        }

        /// <summary>
        /// Returns cell border inherited from a row element.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        /// <param name="cellIndex">Cell index in the row.</param>
        /// <param name="colSpan">The number of cols the cell is to span.</param>
        private CssBorder GetRowBorder(BorderType borderType, int cellIndex, int colSpan)
        {
            if ((mCurrentHtmlRow == null) ||
                ((borderType == BorderType.Left) && (cellIndex != 0)) ||
                ((borderType == BorderType.Right) && ((cellIndex + colSpan) != mCurrentHtmlRow.CellCount)))
            {
                return CssBorder.Empty;
            }

            return mCurrentHtmlRow.CssBorders[borderType];
        }

        /// <summary>
        /// Returns cell border inherited from a row group element.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        /// <param name="cellIndex">Cell index in the row.</param>
        /// <param name="rowIndex">The current row index.</param>
        /// <param name="colSpan">The number of cols the cell is to span.</param>
        private CssBorder GetRowGroupBorder(BorderType borderType, int cellIndex, int rowIndex, int colSpan)
        {
            if ((mHtmlTable == null) || (mCurrentHtmlRow == null) || (mCurrentHtmlRow.RowGroup == null))
                return CssBorder.Empty;

            bool isEdgeCell;
            switch (borderType)
            {
                case BorderType.Top:
                    isEdgeCell = (rowIndex == 0) ||
                        (mHtmlTable[rowIndex - 1].RowGroup != mCurrentHtmlRow.RowGroup);
                    break;
                case BorderType.Right:
                    isEdgeCell = (cellIndex + colSpan) == mCurrentHtmlRow.CellCount;
                    break;
                case BorderType.Bottom:
                    isEdgeCell = (rowIndex == (mHtmlTable.RowCount - 1)) ||
                        (mHtmlTable[rowIndex + 1].RowGroup != mCurrentHtmlRow.RowGroup);
                    break;
                case BorderType.Left:
                    isEdgeCell = cellIndex == 0;
                    break;
                default:
                    throw new InvalidOperationException("Unknown border type");
            }

            return isEdgeCell
                ? mCurrentHtmlRow.RowGroup.CssBorders[borderType]
                : CssBorder.Empty;
        }

        /// <summary>
        /// Returns cell border inherited from a col element.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        /// <param name="cellIndex">Cell index in the row.</param>
        /// <param name="rowIndex">The current row index.</param>
        private CssBorder GetColBorder(BorderType borderType, int cellIndex, int rowIndex)
        {
            if ((mHtmlTable == null) ||
                (cellIndex >= mHtmlTable.ColElementCount) ||
                ((borderType == BorderType.Top) && (rowIndex != 0)) ||
                (borderType == BorderType.Bottom) && (rowIndex != (mHtmlTable.RowCount - 1)))
            {
                return CssBorder.Empty;
            }

            return mHtmlTable.GetCol(cellIndex).CssBorders[borderType];
        }

        /// <summary>
        /// Returns cell border inherited from a colgroup element.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        /// <param name="cellIndex">Cell index in the row.</param>
        /// <param name="rowIndex">The current row index.</param>
        private CssBorder GetColGroupBorder(BorderType borderType, int cellIndex, int rowIndex)
        {
            if ((mHtmlTable == null) || (mHtmlTable.ColElementCount <= cellIndex))
            {
                return CssBorder.Empty;
            }

            switch (borderType)
            {
                case BorderType.Top:
                    return (rowIndex == 0)
                        ? mHtmlTable.GetCol(cellIndex).ColGroup.CssBorders.Top
                        : CssBorder.Empty;
                case BorderType.Right:
                    return GetColGroupRightBorder(cellIndex);
                case BorderType.Bottom:
                    return (rowIndex == mHtmlTable.RowCount - 1)
                        ? mHtmlTable.GetCol(cellIndex).ColGroup.CssBorders.Bottom
                        : CssBorder.Empty;
                case BorderType.Left:
                    return GetColGroupLeftBorder(cellIndex);
                default:
                    throw new InvalidOperationException("Unknown border type");
            }
        }

        private CssBorder GetColGroupRightBorder(int cellIndex)
        {
            HtmlColGroup colGroup = mHtmlTable.GetCol(cellIndex).ColGroup;
            int nextCellIndex = cellIndex + 1;
            HtmlColGroup nextColGroup = ((nextCellIndex < mCurrentHtmlRow.CellCount) && (nextCellIndex < mHtmlTable.ColElementCount))
                ? mHtmlTable.GetCol(nextCellIndex).ColGroup
                : null;
            return (nextColGroup != colGroup)
                ? colGroup.CssBorders.Right
                : CssBorder.Empty;
        }

        private CssBorder GetColGroupLeftBorder(int cellIndex)
        {
            HtmlColGroup colGroup = mHtmlTable.GetCol(cellIndex).ColGroup;
            HtmlColGroup prevColGroup = (cellIndex > 0)
                ? mHtmlTable.GetCol(cellIndex - 1).ColGroup
                : null;
            return (prevColGroup != colGroup)
                ? colGroup.CssBorders.Left
                : CssBorder.Empty;
        }

        /// <summary>
        /// Returns cell border inherited from table element.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        /// <param name="cellIndex">Cell index in the row.</param>
        /// <param name="rowIndex">The current row index.</param>
        /// <param name="colSpan">The number of cols the cell is to span.</param>
        /// <param name="rowSpan">The number of rows the cell is to span.</param>
        private CssBorder GetTableBorder(BorderType borderType, int cellIndex, int rowIndex, int colSpan, int rowSpan)
        {
            if (mHtmlTable == null)
                return CssBorder.Empty;

            bool isEdgeCell;
            switch (borderType)
            {
                case BorderType.Top:
                    isEdgeCell = rowIndex == 0;
                    break;
                case BorderType.Right:
                    isEdgeCell = (cellIndex + colSpan) == mCurrentHtmlRow.CellCount;
                    break;
                case BorderType.Bottom:
                    isEdgeCell = (rowIndex + rowSpan) == mHtmlTable.RowCount;
                    break;
                case BorderType.Left:
                    isEdgeCell = cellIndex == 0;
                    break;
                default:
                    throw new InvalidOperationException("Unknown border type");
            }

            return isEdgeCell
                ? mHtmlTable.CssBorders[borderType]
                : CssBorder.Empty;
        }

        /// <summary>
        /// Applies CSS separate border styles to model table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="topCaptionCount">Number of caption rows at the top of the table.</param>
        /// <param name="bottomCaptionCount">Number of caption rows at the bottom of the table.</param>
        /// <remarks>Table captions are imported as table rows. Top captions become leading table rows and bottom captions
        /// become trailing rows. To distinguish 'normal' table rows from caption rows we need to know how many top and bottom
        /// caption rows the table contains.</remarks>
        private void ApplyTableBorders(Table table, int topCaptionCount, int bottomCaptionCount)
        {
            // Table borders are applied at row level, but only to 'normal' table rows, not to captions.
            int firstTableRowIndex = topCaptionCount;
            int lastTableRowIndex = table.Rows.Count - bottomCaptionCount - 1;
            for (int i = firstTableRowIndex; i <= lastTableRowIndex; ++i)
            {
                BorderCollection rowBorders = table.Rows[i].RowFormat.Borders;
                CssBoxBorders htmlTableBorders = mHtmlTable.CssBorders;
                if (htmlTableBorders.Top.IsVisible || htmlTableBorders.Top.IsAwBorder)
                    htmlTableBorders.Top.ToModelBorder(rowBorders.Top);
                if (htmlTableBorders.Right.IsVisible || htmlTableBorders.Right.IsAwBorder)
                    htmlTableBorders.Right.ToModelBorder(rowBorders.Right);
                if (htmlTableBorders.Bottom.IsVisible || htmlTableBorders.Bottom.IsAwBorder)
                    htmlTableBorders.Bottom.ToModelBorder(rowBorders.Bottom);
                if (htmlTableBorders.Left.IsVisible || htmlTableBorders.Left.IsAwBorder)
                    htmlTableBorders.Left.ToModelBorder(rowBorders.Left);
            }
        }

        /// <summary>
        /// Removes borders edge cell borders in case they are equal to table border. Needed to remove duplication in case
        /// table borders are collapsed.
        /// </summary>
        private void RemoveCellBordersDuplicatingTableBorders(bool removeTopBorder, bool removeBottomBorder)
        {
            if (removeTopBorder)
            {
                RemoveHorizontalBorderDuplicatingTableBorder(BorderType.Top, CellAttr.BorderTop, 0);
            }
            if (removeBottomBorder)
            {
                RemoveHorizontalBorderDuplicatingTableBorder(BorderType.Bottom, CellAttr.BorderBottom, mHtmlTable.RowCount - 1);
            }
            for (int rowIndex = 0; rowIndex < mHtmlTable.RowCount; rowIndex++)
            {
                RemoveVerticalBorderDuplicatingTableBorder(BorderType.Left, CellAttr.BorderLeft, rowIndex, 0);
                RemoveVerticalBorderDuplicatingTableBorder(BorderType.Right, CellAttr.BorderRight, rowIndex,
                    mHtmlTable[rowIndex].ModelCellCount - 1);
            }
        }

        private void RemoveVerticalBorderDuplicatingTableBorder(
            BorderType borderType,
            int borderCellAttr,
            int rowIndex,
            int cellIndex)
        {
            mCurrentHtmlRow = mHtmlTable[rowIndex];
            mCurrentHtmlCell = mCurrentHtmlRow[cellIndex];

            CssBorder winnerBorder = GetWinnerBorder(
                borderType,
                cellIndex,
                rowIndex,
                mCurrentHtmlCell.ColSpan,
                mCurrentHtmlCell.RowSpan);

            if (winnerBorder == mHtmlTable.CssBorders[borderType])
            {
                Cell modelCell = mCurrentHtmlCell.ModelCell;
                modelCell.CellPr.Remove(borderCellAttr);

                // In case the current cell is merged with other cells, we should remove the border from them as well.
                int mergedCellIndex;
                // Find the first merged cell.
                for (mergedCellIndex = cellIndex; mergedCellIndex >= 0; mergedCellIndex--)
                {
                    modelCell = mCurrentHtmlRow[mergedCellIndex].ModelCell;
                    if (modelCell.CellPr.HorizontalMerge != CellMerge.Previous)
                    {
                        break;
                    }
                }
                // Remove the border from all cells that are merged with the first one.
                for (; mergedCellIndex < mCurrentHtmlRow.ModelCellCount; mergedCellIndex++)
                {
                    modelCell = mCurrentHtmlRow[mergedCellIndex].ModelCell;
                    if (modelCell.CellPr.HorizontalMerge == CellMerge.None)
                    {
                        break;
                    }
                    modelCell.CellPr.Remove(borderCellAttr);
                }
            }

            mCurrentHtmlRow = null;
            mCurrentHtmlCell = null;
        }

        private void RemoveHorizontalBorderDuplicatingTableBorder(
            BorderType borderType,
            int borderCellAttr,
            int rowIndex)
        {
            mCurrentHtmlRow = mHtmlTable[rowIndex];
            for (int cellIndex = 0; cellIndex < mCurrentHtmlRow.ModelCellCount; cellIndex++)
            {
                mCurrentHtmlCell = mCurrentHtmlRow[cellIndex];

                CssBorder winnerBorder = GetWinnerBorder(
                    borderType,
                    cellIndex,
                    rowIndex,
                    mCurrentHtmlCell.ColSpan,
                    mCurrentHtmlCell.RowSpan);

                if (winnerBorder == mHtmlTable.CssBorders[borderType])
                {
                    Cell modelCell = mCurrentHtmlCell.ModelCell;
                    modelCell.CellPr.Remove(borderCellAttr);

                    // In case the current cell is merged with other cells, we should remove the border from them as well.
                    int mergedRowIndex;
                    // Find the first merged cell.
                    for (mergedRowIndex = rowIndex; mergedRowIndex >= 0; mergedRowIndex--)
                    {
                        modelCell = mHtmlTable[mergedRowIndex][cellIndex].ModelCell;
                        if (modelCell.CellPr.VerticalMerge != CellMerge.Previous)
                        {
                            break;
                        }
                    }
                    // Remove the border from all cells that are merged with the first one.
                    for (; mergedRowIndex < mHtmlTable.RowCount; mergedRowIndex++)
                    {
                        modelCell = mHtmlTable[mergedRowIndex][cellIndex].ModelCell;
                        if (modelCell.CellPr.VerticalMerge == CellMerge.None)
                        {
                            break;
                        }
                        modelCell.CellPr.Remove(borderCellAttr);
                    }
                }
            }
        }

        /// <summary>
        /// In the collapsing border model returns winner border according to Border Conflict Resolution specification
        /// (http://www.w3.org/TR/CSS21/tables.html#border-conflict-resolution).
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        /// <param name="cellIndex">Cell index in the row.</param>
        /// <param name="rowIndex">The current row index.</param>
        /// <param name="colSpan">The number of cols the cell is to span.</param>
        /// <param name="rowSpan">The number of rows the cell is to span.</param>
        private CssBorder GetWinnerBorder(BorderType borderType, int cellIndex, int rowIndex, int colSpan, int rowSpan)
        {
            // WORDSNET-28188 Border with 'nil' style should not participate in border conflict resolution.
            CssBorder cellBorder = GetCellBorder(borderType);
            if (cellBorder.IsAwBorder)
                return cellBorder;

            // Rule 4: If border styles differ only in color, then a style set on a cell wins over one on a row, which wins
            // over a row group, column, column group and, lastly, table.
            // Note #1: Elements in this array are ordered as defined in rule 4 of Border Conflict Resolution specification.
            // Note #2: The specification says: When two elements of the same type conflict, then the one further to the left
            // (if the table's 'direction' is 'ltr'; right, if it is 'rtl') and further to the top wins. We ignore this rule
            // in our algorithm because MS Word uses a very similar behavior. We can implement this rule later.
            CssBorder[] elementsBorders = new CssBorder[]
            {
                GetRowBorder(borderType, cellIndex, colSpan),
                GetRowGroupBorder(borderType, cellIndex, rowIndex, colSpan),
                GetColBorder(borderType, cellIndex, rowIndex),
                GetColGroupBorder(borderType, cellIndex, rowIndex),
                GetTableBorder(borderType, cellIndex, rowIndex, colSpan, rowSpan)
            };
            CssBorder winnerBorder = cellBorder;
            for (int i = 0; i < elementsBorders.Length; i++)
            {
                if (winnerBorder.Wins(elementsBorders[i]) < 0)
                    winnerBorder = elementsBorders[i];
            }

            return winnerBorder;
        }

        /// <summary>
        /// Applies indents (distances from text) from the CSS box model to a table.
        /// </summary>
        /// <remarks>
        /// The <see cref="Table.Bidi"/> property must be set before calling this method.
        /// </remarks>
        private void ApplyIndents(Table table)
        {
            // Margins of floating tables are translated into tables' distances from text.
            if (table.IsFloating)
            {
                if (table.FirstRow != null)
                {
                    TablePr tablePr = table.FirstRow.TablePr;
                    // Our CSS box model can return negative horizontal margins, but distances from text
                    // are not allowed to be negative.
                    int left = System.Math.Max(ConvertUtilCore.PointToTwip(mCssStyleTracker.BoxModel.Left.Value), 0);
                    tablePr.SetAttr(TableAttr.FrameDistanceFromLeft, left);

                    int right = System.Math.Max(ConvertUtilCore.PointToTwip(mCssStyleTracker.BoxModel.Right.Value), 0);
                    tablePr.SetAttr(TableAttr.FrameDistanceFromRight, right);

                    // Our CSS box model guarantee that vertical margins are always non-negative.
                    int top = ConvertUtilCore.PointToTwip(mCssStyleTracker.BoxModel.Top.Value);
                    Debug.Assert(top >= 0);
                    tablePr.SetAttr(TableAttr.FrameDistanceFromTop, top);

                    int bottom = ConvertUtilCore.PointToTwip(mCssStyleTracker.BoxModel.Bottom.Value);
                    Debug.Assert(bottom >= 0);
                    tablePr.SetAttr(TableAttr.FrameDistanceFromBottom, bottom);
                }
            }
            else
            {
                double leftIndent;
                // Now, we use HtmlBlock nodes during import altChunk elements to store borders and margins of block-level HTML elements,
                // we don't need the box model and we apply only the current element margins directly to the current table indent.
                if (mApplyFormattingAsMsWord)
                {
                    leftIndent = table.Bidi
                        ? GetMargin(mCssStyleTracker.ElementDeclarations, "margin-right")
                        : GetMargin(mCssStyleTracker.ElementDeclarations, "margin-left");
                }
                else
                {
                    leftIndent = table.Bidi
                        ? mCssStyleTracker.BoxModel.Right.Value
                        : mCssStyleTracker.BoxModel.Left.Value;
                }

                // WORDSNET-20364 Adjust table left margin in documents intended for older versions of MS Word.
                // Older MS Word versions used a table aligment algorithm that is different from the one used in HTML, and
                // we need to adjust table margin in order to compensate this difference.
                // Please note that this adjustment is not performed by MS Word when it loads HTML documents.
                // WORDSNET-7392 for an inner HTML table MS Word converts its 'margin-left' CSS property value
                // directly to LeftIndent value, so no additional correction is needed for LeftIndent of inner tables.
                if ((mBuilder.Document.CompatibilityOptions.MswVersion < MsWordVersionCore.Word2013) &&
                    !table.IsNested &&
                    (table.FirstRow != null) &&
                    !mApplyFormattingAsMsWord)
                {
                    CellPr firstCellPr = table.FirstRow.FirstCell.CellPr;
                    leftIndent += ConvertUtilCore.TwipToPoint(firstCellPr.LeftPadding);
                    if ((firstCellPr.BorderLeft != null) && (firstCellPr.BorderLeft.LineStyle != LineStyle.None))
                    {
                        leftIndent += firstCellPr.BorderLeft.LineWidth / 2;
                    }
                }

                if (!MathUtil.AreEqual(table.LeftIndent, leftIndent))
                {
                    table.LeftIndent = leftIndent;
                }
            }
        }

        private static double GetMargin(
            CssDeclarationCollection declarations,
            string marginPropertyName)
        {
            CssValue cssValue = GetSingleValue(declarations, marginPropertyName);
            if (cssValue == null)
                return 0;

            return CssUtil.LengthToPoint(cssValue);
        }

        protected static CssValue GetSingleValue(CssDeclarationCollection declarations, string propertyName)
        {
            CssDeclaration declaration = declarations[propertyName];
            if (declaration == null)
            {
                return null;
            }

            Debug.Assert(declaration.Value.Count == 1);
            return declaration.Value.FirstValue;
        }

        private readonly DocumentBuilder mBuilder;
        private readonly CssStyleTracker mCssStyleTracker;
        private readonly ITableFormatter mTableFormatter;
        private readonly HtmlCommonBorderResolver mCommonBorderResolver;
        private readonly IHtmlNodeProcessor mNodeProcessor;
        private HtmlTable mHtmlTable;

        /// <summary>
        /// An HTML row group that is currently being processed.
        /// </summary>
        private HtmlRowGroup mCurrentRowGroup;

        /// <summary>
        /// HTML table row that is currently being processed.
        /// </summary>
        private HtmlRow mCurrentHtmlRow;

        /// <summary>
        /// HTML table cell that is currently being processed.
        /// </summary>
        private HtmlCell mCurrentHtmlCell;

        /// <summary>
        /// Indicate whether the currently processed row can be repeated as the header row at the top of each page.
        /// </summary>
        private bool mCurrentHtmlRowCanBeTableHeader;

        private bool mIsRtlTable;

        /// <summary>
        /// Instructs the table reader to stick to MS Word's behavior when applying certain formatting.
        /// </summary>
        private readonly bool mApplyFormattingAsMsWord;

        private CssBorder mInsideHorizontalCssBorder;
        private CssBorder mInsideVerticalCssBorder;

        private readonly HtmlBlockReader mHtmlBlockReader;
        private readonly HtmlHyperlinkResolver mHyperlinkResolver;
    }
}
