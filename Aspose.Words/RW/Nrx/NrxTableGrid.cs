// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2012 by Alexey Morozov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Tables;


namespace Aspose.Words.RW.Nrx
{
    /// <summary>
    /// Implements reading/writing of the tblGrid element.
    /// </summary>
    internal static class NrxTableGrid
    {
        /// <summary>
        /// Writes 'w:tblGrid' element for the current table and 'w:tblGridChange' element if table has formatting revision.
        /// The table grid is a definition of the set of grid columns which define all of the shared
        /// vertical edges of the table, as well as default widths for each of these grid columns.
        /// </summary>
        internal static void WriteTblGrid(NrxXmlBuilder builder, Table table)
        {
            TablePr tablePr = table.TablePr;

            IntList tableGrid = GetTableGrid(tablePr.RevisedPr);
            IntList tableGridChange = tablePr.HasFormatRevision ? GetTableGrid(tablePr) : null;

            builder.StartElement("w:tblGrid");
            for (int i = 0; i < tableGrid.Count; i++)
                builder.WriteElementWithAttributes("w:gridCol", "w:w", tableGrid[i]);

            if(tableGridChange != null)
            {
                builder.StartElement("w:tblGridChange");
                builder.StartElement("w:tblGrid");

                for (int i = 0; i < tableGridChange.Count; i++)
                    builder.WriteElementWithAttributes("w:gridCol", "w:w", tableGridChange[i]); 
                
                builder.EndElement();
                builder.EndElement();
            }

            builder.EndElement(); //w:tblGrid
        }

        /// <summary>
        /// Reads 'tblGrid' element which specifies table grid for current table.
        /// </summary>
        internal static void ReadTblGrid(NrxXmlReader partReader, TablePr tablePr, OoxmlComplianceInfo complianceInfo)
        {
            IntList tableGrid = new IntList();
            tablePr.SetAttr(TableAttr.Sys_TableGrid, tableGrid);

            while (partReader.ReadChild("tblGrid"))
            {
                switch (partReader.LocalName)
                {
                    case "gridCol":
                        // DD: this might be a good piece to make a function, but it is used only once so far. So leave as is.
                        int width = 0;
                        while (partReader.MoveToNextAttribute())
                        {
                            if (partReader.LocalName == "w")
                                width = partReader.GetValueAsTwips(complianceInfo);
                        }
                        tableGrid.Add(width);
                        break;

                    case "tblGridChange":
                        ReadTblGridChange(partReader, tablePr, complianceInfo);
                        break;

                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }

            // Save a copy of the grid to be used by the "new" table grid algorithm in FixedGridCalculator,
            // before an "old" algorithm tweaked it in any way.
            BackupTableGrid(tablePr);
        }

        /// <summary>
        /// Updates cell widths from table grid.
        /// </summary>
        internal static void TableGridToCellWidths(Row row, IList<Table> tablesWithMissedTableGrid)
        {
            Table table = row.ParentTable;
            
            // There is one file with TableGrid missed, build it from first row.
            if (!HasValidTableGrid(table.TablePr))
            {
                BuildTableGridFromFirstRow(table);
                tablesWithMissedTableGrid.Add(table);
            }

            if (table.TablePr.HasFormatRevision)
            {
                // In case table has table grid changed we have to additionally update before-changes cell widths.
                TableGridToCellWidths(row, RevisionsView.Original);
            }
            
            TableGridToCellWidths(row, RevisionsView.Final);
        }

        private static bool HasValidTableGrid(TablePr tablePr)
        {
            IntList tableGrid = (IntList)tablePr[TableAttr.Sys_TableGrid];

            if ((tableGrid == null) || (tableGrid.Count == 0))
                return false;

            int totalWidth = 0;
            for (int i = 0; i < tableGrid.Count; i++)
                totalWidth += tableGrid[i];

            return totalWidth > 0;
        }

        /// <summary>
        /// Updates either cell before-widths or after-widths from table grid depending on isRevised parameter.
        /// </summary>
        private static void TableGridToCellWidths(Row row, RevisionsView view)
        {
            IntList tableGrid = (IntList)row.ParentTable.TablePr.FetchAttr(TableAttr.Sys_TableGrid, view);

            if (row.IsFirstRow)
                ValidateTableGrid(tableGrid, row, view);

            int gridBefore = (int)row.TablePr.FetchAttr(TableAttr.Sys_GridBefore, view);
            UpdateWidthBefore(tableGrid, row, view);

            if (tableGrid.Count > 0)
            {
                int gridIndex = gridBefore;

                // RK Set fixed cell width in the model from the grid.
                foreach (Cell cell in row.Cells)
                {
                    CellPr cellPr = cell.CellPr;
                    int gridSpan = (int)cellPr.FetchAttr(CellAttr.Sys_CellSpan, view);
                    int width = 0;

                    int maxGridIndex = System.Math.Min(gridIndex + gridSpan, tableGrid.Count);
                    while (gridIndex < maxGridIndex)
                        width += tableGrid[gridIndex++];

                    // Here we have two options. Cell has format revision or not.
                    // If case of cell has no format revision cell widths should be updated from revised table grid otherwise
                    // cell before-widths should be updated from before-grid and after-widths from after-grid.
                    // Proper table grid is chosen depending isAccepted value.
                    //
                    // Note we process with accepted properties but it's just clone of real node attributes.
                    // So we must to update real node attributes.
                    if (view == RevisionsView.Final)
                    {
                        // After-grid is in process. Update after-width if cell has revision or cell width otherwise.
                        cellPr.RevisedPr.Width = width;
                    }
                    else
                    {
                        // Before-grid is in process. Update only before-width and only if cell has revision.
                        if (cellPr.HasFormatRevision)
                            cellPr.Width = width;
                    }
                }
            }
        }

        /// <summary>
        /// Validates table grid. 
        /// Currently checks that number of columns corresponds to number of table grid elements and 
        /// rebuild table grid if needed.
        /// </summary>
        private static void ValidateTableGrid(IntList tableGrid, Row row, RevisionsView view)
        {
            int columnCount = (int)row.TablePr.FetchAttr(TableAttr.Sys_GridBefore, view) + (int)row.TablePr.FetchAttr(TableAttr.Sys_GridAfter, view);
            foreach (Cell cell in row.Cells)
                columnCount += (int) cell.CellPr.FetchAttr(CellAttr.Sys_CellSpan, view);

            if (columnCount > tableGrid.Count)
            {
                // Table grid has insufficient count of elements. Rebuild it.
                if (row.TablePr.AllowAutoFit)
                {
                    int totalWidth = 0;
                    for (int i = 0; i < tableGrid.Count; i++)
                        totalWidth += tableGrid[i];

                    tableGrid.Clear();

                    for (int i = 0; i < columnCount; i++)
                        tableGrid.Add(totalWidth / columnCount);
                }
                // WORDSNET-11185 If table with insufficient grid cols has layout type 'fixed',
                // then MS Word adds an extra grid cols with fixed width.
                else
                {
                    while (columnCount > tableGrid.Count)
                        tableGrid.Add(DefaultGridColWidthTwips);
                }
            }
        }

        /// <summary>
        /// This is a part of an older logic implemented by TableGridToCellWidths.
        /// </summary>
        /// <remarks>
        /// The logic only works correctly in case the table has correct widths stored in tblGrid element.
        /// Even with the correct tblGrid, replacing widthBefore values stored in the document with values calculated from the grid
        /// may change the table layout.
        /// The logic is still needed to preserve AW behavior in scenarios not handled by the new table layout algorithm.
        /// </remarks>
        private static void UpdateWidthBefore(IntList tableGrid, Row row, RevisionsView view)
        {
            int gridBefore = (int)row.TablePr.FetchAttr(TableAttr.Sys_GridBefore, view);

            int widthBefore = 0;
            for (int i = 0; i < gridBefore; i++)
            {
                // andrnosk Resilience WORDSNET-8211 
                if (tableGrid.Count > i)
                    widthBefore += tableGrid[i];
            }

            // Update either original or final properties.
            TablePr rowPr = (view == RevisionsView.Final && row.TablePr.HasFormatRevision) 
                ? (TablePr)row.TablePr.FormatRevision.RevPr 
                : row.TablePr;

            // This will certainly overwrite width before specified in pct units, but that what AW always did.
            // If the new algorithm handles the table, the correct values will be restored from TableAttr.WidthBeforeOriginal.
            rowPr.WidthBeforeTwips = widthBefore;
        }
        
        /// <summary>
        /// Builds table grid from first row of table. Used if only table grid is missed.
        /// </summary>
        private static void BuildTableGridFromFirstRow(Table table)
        {
            IntList tableGrid = new IntList();
            table.TablePr.SetAttr(TableAttr.Sys_TableGrid, tableGrid);

            // RK The grid is not defined or empty. Let's build a grid using cells of (the first) row.
            // DM This is a simplified approach. It will be used as a fall-back if the new grid calculation encounters an unsupported case.
            foreach (Cell cell in table.FirstRow.Cells)
            {
                cell.CellPr.ValidateWidth();
                tableGrid.Add(cell.CellPr.Width);
            }
        }

        /// <summary>
        /// Reads 'tblGridChange' element which specifies original table grid in revision.
        /// </summary>
        private static void ReadTblGridChange(NrxXmlReader partReader, TablePr tablePr, OoxmlComplianceInfo complianceInfo)
        {
            while (partReader.ReadChild("tblGridChange"))
            {
                switch (partReader.LocalName)
                {
                    case "tblGrid":
                    {
                        // if we are reading 'Change' table grid it means that table grid that was read before is actually after-changes table grid and
                        // this one is before-changes table grid.
                        // 1. Move already read grid to format revision.
                        IntList tableGrid = (IntList)tablePr[TableAttr.Sys_TableGrid];
                        if (tablePr.FormatRevision == null)
                            tablePr.FormatRevision = new FormatRevision(new TablePr(), "", DateTime.MinValue);
                        tablePr.FormatRevision.RevPr.SetAttr(TableAttr.Sys_TableGrid, tableGrid);

                        // 2. Create and read new table grid.
                        ReadTblGrid(partReader, tablePr, complianceInfo);
                        break;
                    }

                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }

            BackupTableGrid(tablePr);
        }

        /// <summary>
        /// Helper method returns TableGrid from given table attrs.
        /// </summary>
        private static IntList GetTableGrid(WordAttrCollection tablePr)
        {
            return (IntList)tablePr.GetDirectAttr(TableAttr.Sys_TableGrid);
        }

        private static void BackupTableGrid(TablePr tablePr)
        {
            IntList grid = (IntList)tablePr[TableAttr.Sys_TableGrid];
            if (grid == null)
                return;

            IntList gridClone = new IntList(grid.Count);
            gridClone.AddRange(grid);
            tablePr.SetAttr(TableAttr.Sys_TableGridForNewAlgorithm, new TableGridColumnsAttr(gridClone));
        }

        /// <summary>
        /// Default grid col width 0,25" to be used in case when extra grid cols should be added.
        /// </summary>
        private const int DefaultGridColWidthTwips = 360;
    }
}
