// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/09/2011 by Roman Korchagin

using System;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.TableLayout;
using Aspose.Words.Tables;
using Aspose.Words.Themes;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// Validates tables, rows and cells during document save.
    /// </summary>
    internal class TableValidator
    {
        internal TableValidator(DocumentValidatorActions saveActions, SaveInfo saveInfo)
        {
            mWordTableLimits = (saveActions & DocumentValidatorActions.WordTableLimits) != 0;

            mIsNormalizeHorizontalMerge = (saveActions & DocumentValidatorActions.NormalizeHorizontalMerge) != 0;
            mExplicitAutoPreferredWidth = (saveActions & DocumentValidatorActions.ExplicitAutoPreferredWidth) != 0;
            mSaveInfo = saveInfo;
        }

        internal VisitorAction VisitTableStart(Table table)
        {
            if (table.HasCells)
            {
                // RK This is for rendering at the moment. Ideally, should do for all formats as it will allow to simplify writers.
                if (mIsNormalizeHorizontalMerge)
                {
                    bool anyModified = false;
                    foreach (Row row in table.Rows)
                        foreach (Cell cell in row.Cells)
                            anyModified |= cell.CellPr.ValidateWidth();

                    if(anyModified)
                        UpdateTableLayoutOnce(table);

                    table.NormalizeHorizontalMerge();
                }
                else
                {
                    TableGridFromCellWidth tableGrid = new TableGridFromCellWidth(table);
                    if (tableGrid.ColumnWidths.Length == 0)
                        UpdateTableLayoutOnce(table);
                }

                table.NormalizeVerticalMerge();

                ValidateTableWidth(table);
                ValidateTablePr(table);
            }

            return VisitorAction.Continue;
        }

        private bool UpdateTableLayoutOnce(Table table)
        {
            if (mUpdatedTables.Contains(table))
                return false;

            table.UpdateLayout();
            mUpdatedTables.Add(table);
                        return true;
        }

        private static void ValidateTablePr(Table table)
        {
            if (table.FirstRow == null)
                return;

            // Dmatv: WORDSNET-25085: The logic below seems to match MS Word behavior at the moment of 25085 fix.
            // Conflicting table x/y and alignment attributes are resolved by removing tblpX, tblpY.
            // Note: for frames in case of similar conflicts, MS Word behaves differently.
            // The logic below supersedes the earlier fixes:
            //      andrnosk: WORDSNET-7084 Table vertically positioned relatively to a paragraph, and in the same time vertically centered.
            //      17370 forcing negative table position for "outside" alignment.

            TablePr tablePr = table.FirstRow.TablePr;
            HorizontalAlignment ha = tablePr.HorizontalAlignment;
            bool isHorizontalAlignmentSpecified = (ha != HorizontalAlignment.None);
            if (isHorizontalAlignmentSpecified)
                table.RemoveAttrFromAllRows(TableAttr.FrameLeft);

            VerticalAlignment va = tablePr.VerticalAlignment;
            bool isVerticalAlignmentSpecified = (va != VerticalAlignment.None) && (va != VerticalAlignment.Inline);
            if (isVerticalAlignmentSpecified)
                table.RemoveAttrFromAllRows(TableAttr.FrameTop);

            // No model changes are needed for an invalid combination of paragraph-relative vertical position and vertical alignment.
            // MS Word does not change the model on re-saving in this case.
            // The invalid combination is handled by the layout code.

            // The above logic was not tested for cases when conflicting attributes are defined via styles.
            // Removing direct table attributes will not work if conflicting x or y came from a style.
        }

        internal VisitorAction VisitRowStart(Row row)
        {
            if (mWordTableLimits)
                if (row.Cells.Count > Table.MaxColumns)
                    throw new InvalidOperationException("More than 63 cells per row is not supported for this file format.");

            if (row.TablePr.Height > ConvertUtilCore.MaxSizeTwip)
            {
                row.TablePr.Height = ConvertUtilCore.MaxSizeTwip;
                WarningUtil.WarnUnexpected(WarningCallback, WarningStrings.RowHeightExceedLimit);
            }

            if (row.Cells.Count >= Table.MaxColumns)
                WarningUtil.WarnUnexpected(WarningCallback, WarningStrings.TableValidatorWordCellLimitExceed);

            ThemeColorUpdater.Update(row);

            RemoveInvalidAttributes(row);
            row.ValidateAlignedIndent();

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Checks the values of some attributes and removes the if they are not valid.
        /// </summary>
        /// <remarks>
        /// Some docs have negative gridAfter (TestJira8211).
        /// This method is called from DocumentPostLoader as well, so that grid calculation deals with correct attributes.
        /// </remarks>
        internal static void RemoveInvalidAttributes(Row row)
        {
            RemoveNegativeIntAttribute(row.TablePr, TableAttr.Sys_GridBefore);
            RemoveNegativeIntAttribute(row.TablePr, TableAttr.Sys_GridAfter);
        }

        private static void RemoveNegativeIntAttribute(TablePr rowPr, int attrKey)
        {
            object attrValue = rowPr.GetDirectAttr(attrKey);

            if ((attrValue != null) && ((int)attrValue < 0))
            {
                Debug.WriteLine(string.Format("Invalid value {0} of attribute {1} removed from a row.", attrValue, attrKey));
                rowPr.Remove(attrKey);
            }
        }

        internal VisitorAction VisitTableEnd(Table table)
        {
            // Resiliency: Don't write empty table. In fact, delete a completely empty table.
            // RK We do this in the table end handler because the table could become empty
            // during validation if all rows are empty and deleted from it.
            //
            // After this no further checks should be made because we deleted the table.
            if (table.FirstRow == null)
            {
                Warn(WarningType.MinorFormattingLoss, WarningStrings.TableValidatorEmptyTable);
                table.Remove();
            }

            return VisitorAction.Continue;
        }

        internal VisitorAction VisitRowEnd(Row row)
        {
            if (row.FirstCell == null)
            {
                // Resiliency: Don't write empty rows. In fact, delete a completely empty row.
                Warn(WarningType.MinorFormattingLoss, WarningStrings.TableValidatorEmptyRow);
                row.Remove();
                return VisitorAction.Continue;
            }

            // WORDSNET-905 MS Word 2000 crashes if the first cell is merged to previous.
            Cell firstCell = row.FirstCell;
            if (firstCell.CellPr.HorizontalMerge == CellMerge.Previous)
                firstCell.CellPr.HorizontalMerge = CellMerge.First;

            // Last cell should not be first merged.
            Cell lastCell = row.LastCell;

            if (lastCell.CellPr.HorizontalMerge == CellMerge.First)
                lastCell.CellPr.HorizontalMerge = CellMerge.None;

            ValidateEditRevision(row);

            // Dmatv: originally the logic was called for docx/wml only from the readers, but I moved it
            // to avoid applying to intermediate TablePr state before tblPrEx etc are processed.
            // Also it seems that to emulate MS Word the logic should be applied to other formats as well.
            RemoveDisregardedProperties(row.TablePr);

            if (ExpandHtmlBlockAttrs(row))
                mSaveInfo.HasHtmlBlockReferences = true;

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Reverts attributes modified by expanding procedures.
        /// </summary>
        internal void Revert()
        {
            if (mHtmlBlockRows.Count == 0)
                return;

            foreach(Row row in mHtmlBlockRows)
                row.Document.HtmlBlockCollection.RemoveFormatting(row.TablePr);
        }

        /// <summary>
        /// Expands <see cref="HtmlBlock"/> attributes to the table row properties.
        /// </summary>
        /// <remarks>
        /// These attributes will be reverted after saving. See <see cref="Revert"/>
        /// </remarks>
        private bool ExpandHtmlBlockAttrs(Row row)
        {
            if (!row.TablePr.Contains(TableAttr.HtmlBlockId))
                return false;

            int htmlBlockId = (int)row.TablePr[TableAttr.HtmlBlockId];
            // Do not place to the formatting expander because a document may be without table styles.
            row.Document.HtmlBlockCollection
                .Expand(row.TablePr, htmlBlockId, !(row.ParentTable.GetAncestor(NodeType.Table) is Table));

            mHtmlBlockRows.Add(row);

            return true;
        }

        /// <summary>
        /// Removes table properties related to floating position and text wrapping if they should be disregarded.
        /// </summary>
        /// <remarks>
        /// It seems that MS Word does not take into account and removes the following table properties from the documents:
        /// - tblpXSpec = left;
        /// - vertAnchor = page, when the table is not floating;
        /// - all "distance from text" attributes when the table is not floating.
        /// <see cref="TablePr.IsFloating"/> for the logic that determines if the table is floating.
        /// </remarks>
        private static void RemoveDisregardedProperties(TablePr tablePr)
        {
            // Remove tblpXSpec attribute if its value is Left.
            if (tablePr.HorizontalAlignment == HorizontalAlignment.Left)
                tablePr.Remove(TableAttr.HorizontalAlignment);
            // Dmatv it even works correctly with Page alignment
            // because of a special condition in layout positioning logic that substitutes Left as needed.
            // Leave it as is as MS Word does remove Left alignment on re-saving.

            if (!tablePr.IsFloating)
            {
                // Remove relative vertical position Page if other floating-position-related properties are default (checked by IsFloating).
                tablePr.Remove(TableAttr.RelativeVerticalPosition);

                // Remove FrameDistanceFrom* attributes if the table is not floating.
                tablePr.Remove(TableAttr.FrameDistanceFromLeft);
                tablePr.Remove(TableAttr.FrameDistanceFromTop);
                tablePr.Remove(TableAttr.FrameDistanceFromRight);
                tablePr.Remove(TableAttr.FrameDistanceFromBottom);
            }
        }

        /// <summary>
        /// If all paragraph in row have InsertRevision/DeleteRevisions we can guess that entire row was inserted/deleted.
        /// This is how insert/delete revision for rows works in DOC format.
        /// In DOCX we read insert and delete revisions directly from document.
        /// </summary>
        private static void ValidateEditRevision(Row row)
        {
            bool isInsertedRow = true;
            bool isDeletedRow = true;
            foreach (Cell cell in row.Cells)
            {
                isInsertedRow &= cell.LastParagraph.ParagraphBreakRunPr.HasInsertRevision;
                isDeletedRow &= cell.LastParagraph.ParagraphBreakRunPr.HasDeleteRevision;
            }

            if (isInsertedRow && !row.TablePr.HasInsertRevision)
                row.TablePr.InsertRevision = new EditRevision(EditRevisionType.Insertion, row.FirstCell.LastParagraph.ParagraphBreakRunPr.InsertRevision.Author,
                                                              row.FirstCell.LastParagraph.ParagraphBreakRunPr.InsertRevision.DateTime);

            if (isDeletedRow && !row.TablePr.HasDeleteRevision)
                row.TablePr.DeleteRevision = new EditRevision(EditRevisionType.Deletion, row.FirstCell.LastParagraph.ParagraphBreakRunPr.DeleteRevision.Author,
                                                              row.FirstCell.LastParagraph.ParagraphBreakRunPr.DeleteRevision.DateTime);
        }

        internal VisitorAction VisitCellStart(Cell cell)
        {
            if (mExplicitAutoPreferredWidth)
                if(!cell.CellPr.Contains(CellAttr.PreferredWidth))
                    cell.CellPr.PreferredWidth = PreferredWidth.Auto;

            // WORDSNET-956 A cell needs at least one paragraph to be valid.
            cell.EnsureMinimum();

            // WORDSNET-5164 RK At first we want to validate width to make sure we don't write zero width to the file.
            // But also, if the width had to be corrected because it was zero it is a good update to recalculate the table to update width.
            // Recalculation of the table will produce better results because validation performs only a basic correction.
            if (cell.CellPr.ValidateWidth())
            {
                Warn(WarningType.MinorFormattingLoss, WarningStrings.TableValidatorInvalidCellWidth);
                // WORDSJAVA-898 Table.setAllowAutoFit(false) takes much time as compared to Table.setAllowAutoFit(true)
                // If the table contains 1000 cells (e.g. 1000 rows and 1 column) then UpdateLayout() is invoked 1000 times.
                // UpdateLayout() calculates width of each cell by invoking TableLayouter.CalculateColumnLayoutWidths().
                // Thus the inner loop in TableLayouter.CalculateColumnLayoutWidths() may be executed up to 1000 * 1000 times.
                // We can update layout only once instead of doing it for each cell.
                if (UpdateTableLayoutOnce(cell.ParentTable))
                {
                    // Once we update layout we need to validate table width again.
                    ValidateTableWidth(cell.ParentTable);
                }
            }

            // WORDSNET-17118 Cells padding values have to be casted to the two bytes integers, according to the Word behavior.
            ValidateCellPadding(cell);

            ThemeColorUpdater.Update(cell);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Validates padding attributes of the cell.
        /// </summary>
        private static void ValidateCellPadding(Cell cell)
        {
            CellPr cellPr = cell.CellPr;

            ValidatePaddingAttr(cellPr, CellAttr.LeftPadding);
            ValidatePaddingAttr(cellPr, CellAttr.RightPadding);
            ValidatePaddingAttr(cellPr, CellAttr.TopPadding);
            ValidatePaddingAttr(cellPr, CellAttr.BottomPadding);
        }

        /// <summary>
        /// Validates passed padding attribute in the specified collection of cell properties.
        /// </summary>
        private static void ValidatePaddingAttr(CellPr cellPr, int key)
        {
            if (!cellPr.Contains(key))
                return;

            cellPr[key] = MathUtil.CastIntToShort((int)cellPr[key]);
        }

        /// <summary>
        /// Indicates that table width is affected by container width.
        /// </summary>
        private static bool IsContainerAffected(Table table)
        {
            // Skip empty tables.
            if (table.Rows.Count == 0)
                return false;

            // Floating tables are not affected by container width.
            if (table.IsFloating)
                return false;

            if (table.IsNested)
            {
                // Nested table is affected by container if it has percent preferred width.
                return table.PreferredWidth.IsPercent || table.PreferredWidth.IsAuto;
            }
            else
            {
                if (table.AllowAutoFit)
                {
                    // Auto fit table is affected by container if not GrowAutoFit compatibility options set.
                    return !table.Document.DocPr.CompatibilityOptions.GrowAutofit;
                }
                else
                {
                    // Top-level table is affected by container if it has percent preferred width.
                    return table.PreferredWidth.IsPercent;
                }
            }
        }

        /// <summary>
        /// Validates that table grid is valid and shrinks table to fit into container margins is needed.
        /// </summary>
        /// <remarks>
        /// AM. Actually Word doesn't update cells width at all and
        /// only cares that total cells width not exceed 31680 (widely used Word maximum).
        ///
        /// But layout engine completely relies on cell width so we have to update them and
        /// mimic Word rendering behavior. So below comments refer to Word renderer behavior
        /// rather than Word export to file algorithm.
        /// </remarks>
        private void ValidateTableWidth(Table table)
        {
            // Total cell width must not exceed 31680 otherwise Word is not able to render table properly.
            int cellsWidth = table.GetCellsWidth();

            Section parentSection = (Section)table.GetAncestor(NodeType.Section);
            int parentSectionWidth = (parentSection != null) ? parentSection.SectPr.PageWidth : 0;

            // WORDSNET-12477 Allow table goes beyond MaxSizeTwip if target is not MS Word native document.
            // After some discussion we decided to allow table goes beyond MaxSizeTwip only if parent page is wider than MaxSizeTwip
            // to avoid possible regression.
            if ((mWordTableLimits || (parentSectionWidth <= ConvertUtilCore.MaxSizeTwip)) && (cellsWidth > ConvertUtilCore.MaxSizeTwip))
            {
                ShrinkCells(table, ConvertUtilCore.MaxSizeTwip);
                // Get updated table cell width.
                cellsWidth = table.GetCellsWidth();
            }

            if (!IsContainerAffected(table))
                return;

            int newWidth = cellsWidth;
            // Do not try resizing fixed layout table to fit container, unless the table width is in percents.
            if (table.AllowAutoFit || table.PreferredWidth.Type == PreferredWidthType.Percent)
                newWidth = Extensions.GetMaxAllowedWidthResolveTblwPct(table);

            // Word processes fixed width table is quite strange way.
            if (table.PreferredWidth.IsFixed)
            {
                // If table preferred width is greater than container Word renders table as is (cells are not shrink).
                if (table.PreferredWidth.ValueTwips >= newWidth)
                {
                    // Verify that total cells width is less than Word maximum value.
                    newWidth = (cellsWidth < ConvertUtilCore.MaxSizeTwip) ? cellsWidth : ConvertUtilCore.MaxSizeTwip;
                }
            }

            // AM. Accept 1mm extent as valid, trying to update as few tables as possible.
            if (cellsWidth > (newWidth + gMaxAllowedExtent))
                ShrinkCells(table, newWidth);
        }

        /// <summary>
        /// Shrink table cells to get total table cells width to be equal to given width.
        /// </summary>
        private void ShrinkCells(Table table, int newWidth)
        {
            int cellsWidth = table.GetCellsWidth();

            double scale = (double)(newWidth) / cellsWidth;
            if (scale < 1)
            {
                // AM. Last check. I put it here in order to do not check table revisions often.
                // Insert cell revision is poorly implemented in Word
                // (See UnifiedTestInsertCellRevision remarks for example) and moreover
                // it seems that table has inconsistent attribute values when in format revision
                // so I decided t exclude such table from validation at all till users complain.
                if (table.HasRevision)
                    return;

                Warn(WarningType.MinorFormattingLoss, WarningStrings.TableValidatorTableResized);

                // Scale factor less than 1 if margin width is less than cells width. Scale table in this case.
                foreach (Row row in table.Rows)
                {
                    int totalWidthInRow = 0;

                    foreach (Cell cell in row.Cells)
                    {
                        cell.CellPr.Width = (int)System.Math.Round(scale * cell.CellPr.Width);
                        totalWidthInRow += cell.CellPr.Width;
                    }

                    // Adjust last cell if needed to get total cell width the same for all rows.
                    int lastCellWidth = row.LastCell.CellPr.Width + (newWidth - totalWidthInRow);

                    if (lastCellWidth >= 0)
                        row.LastCell.CellPr.Width = lastCellWidth;
                }
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, string description)
        {
            if (WarningCallback != null)
                WarningCallback.Warning(new WarningInfo(warningType, WarningSource.Validator, description));
        }

        private readonly HashSetGeneric<Table> mUpdatedTables = new HashSetGeneric<Table>();
        private readonly HashSetGeneric<Row> mHtmlBlockRows = new HashSetGeneric<Row>();

        private readonly bool mIsNormalizeHorizontalMerge;

        /// <summary>
        /// True if auto width should be explicitly set.
        /// </summary>
        private readonly bool mExplicitAutoPreferredWidth;

        /// <summary>
        /// Validator will throw an exception if cell count is exceed allowed limit for current save format and
        /// will shrink table width to maximum allowed size.
        /// </summary>
        private readonly bool mWordTableLimits;

        /// <summary>
        /// Maximum allowed table extent into container margins. 1mm in twips.
        /// </summary>
        private static readonly int gMaxAllowedExtent = ConvertUtilCore.PointToTwip(ConvertUtil.MillimeterToPoint(1.0));

        private IWarningCallback WarningCallback { get { return mSaveInfo.Document.WarningCallback; } }
        private readonly SaveInfo mSaveInfo;
    }
}
