// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2016 by Dmitry Matveenko

using Aspose.Collections;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Implements methods for getting table column width information from cell widths specified in the document.
    /// </summary>
    /// <remarks>
    /// It seems binary doc and rtf formats do not have tblGrid analogue stored in the document.
    /// Instead, cell widths are stored for each table row.
    /// The older TableGrid class generates the grid by intersecting all the stored cell widths.
    /// The result is used similar to tblGrid in docx file.
    /// It is used as a preferred width for cells with auto width in fixed table layout.
    /// And the actual cell width may be different if a table is resized in order to meet table width,
    /// and for other reasons (just like the width in tblGrid in docx is not final).
    /// </remarks>
    internal static class DocRtfGridHandler
    {
        /// <summary>
        /// Initializes grid and cell spans taking binary doc format specifics into account.
        /// </summary>
        /// <remarks>
        /// The method does nothing if load format is not doc or rtf.
        /// The method is to be used after loading the document before any model changes.
        /// Changing the model can make the widths stored in the model inaccurate.
        /// </remarks>
        internal static void GetGridAndSpansFromCellWidths(Table table)
        {
            if (IsGridStoredInCellWidths(table) && table.FirstRow!= null)
            {
                TableGridFromCellWidth gridFromDoc = TableGridFromCellWidth.GetDocRtfGridFromAttrs(table);

                // Treat it the same way as the original tblGrid is treated for docx files.
                IntList grid = gridFromDoc.ToIntList();

                TableGridColumnsAttr gridColumns = new TableGridColumnsAttr(grid);
                table.TablePr.SetAttr(TableAttr.Sys_TableGridForNewAlgorithm, gridColumns);

                // Update the grid spans in the table so that they match the cell widths and the generated grid.
                UpdateGridSpans(table, gridFromDoc);
                // Cells having zero width can be removed in the process,
                // but it requires updating some tests that check the model built with zero-width cells present.
                // I think it is better to do it as a separate change, there is a large delta for the table merge already.
            }
        }

        /// <summary>
        /// Gets a boolean value indicating if the Document of the given table node
        /// was loaded from doc or rtf formats which requires grid span calculation from the cell widths in the document.
        /// </summary>
        internal static bool IsGridStoredInCellWidths(Node table)
        {
            Document doc = table.Document as Document;

            // Return false for documents other than main document.
            if (doc == null)
                return false;

            switch (doc.OriginalLoadFormat)
            {
                case LoadFormat.Rtf:
                case LoadFormat.Doc:
                case LoadFormat.DocPreWord60:
                case LoadFormat.Dot:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Updates grid spans for the table cells so that they match cell width and the constructed grid.
        /// </summary>
        private static void UpdateGridSpans(Table table, TableGridFromCellWidth gridFromDoc)
        {
            // Go through cells and check if gridSpan needs updating.
            // In doc files, grid span may not be stored explicitly, just derived from cell widths.
            foreach (Row row in table.Rows)
            {
                gridFromDoc.NextRow(row);

                // Row grid before calculated from the grid may be different from the attribute value in some cases.
                // Do not overwrite width before in percent units.
                if (!row.TablePr.WidthBefore.IsPercent && gridFromDoc.Before != row.TablePr.WidthBeforeTwips)
                {
                    if (gridFromDoc.Before != 0)
                        row.TablePr.WidthBeforeTwips = gridFromDoc.Before;
                    else
                        row.TablePr.Remove(TableAttr.WidthBefore);
                }

                if (gridFromDoc.GridBefore != row.TablePr.GridBefore)
                {
                    // It seems, in rtf files gridBefore can be missing when widthBefore is specified.
                    Debug.Assert(row.TablePr.GridBefore == 0);

                    // Set it to the calculated value.
                    row.TablePr.SetAttr(TableAttr.Sys_GridBefore, gridFromDoc.GridBefore);
                }

                foreach (Cell cell in row.Cells)
                {
                    Debug.Assert(!gridFromDoc.IsLastCell());
                    int spanCountFromGrid = gridFromDoc.NextCell();

                    if (spanCountFromGrid == 0)
                    {
                        int cellWidth = cell.CellPr.Width;
                        if ( cellWidth > TableGridFromCellWidth.WidthTolerance)
                        {
                            // This should not happen because of the way TableGrid works.
                            Debug.Fail("Unexpected span count value for a grid generated from cell widths in doc/rtf document.");
                        }
                        else
                        {
                            TableGridDebugLogger.DebugWriteLine(
                                string.Format("### Zero grid span is assigned to a cell with width {0} when constructing spans for doc/rtf.", cellWidth));
                        }
                    }
                    cell.CellPr.GridSpan = spanCountFromGrid;
                }

                SetRowAfter(row, gridFromDoc.After, gridFromDoc.GridAfter);
            }
        }

        /// <summary>
        /// Updates row grid/width after with the given values (calculated from cell widths in the document).
        /// </summary>
        private static void SetRowAfter(Row row, int widthAfterFromDoc, int gridAfterFromDoc)
        {
            TablePr rowPr = row.TablePr;
            if (widthAfterFromDoc > 0)
            {
                Debug.Assert(gridAfterFromDoc > 0);
                int gridAfter = System.Math.Max(1, gridAfterFromDoc);
                rowPr.SetAttr(TableAttr.Sys_GridAfter, gridAfter);

                if (rowPr.WidthAfterTwips != widthAfterFromDoc)
                {
                    Debug.WriteLine("WidthAfter calculated from cell widths does not match the value read from the document. Document value is used.");
                    // This happens with a document from TestJira6793.
                    // The original document is inconsistent: similar rows have different widthAfter/gridAfter.
                    // Also the document has revisions. But even if the source docx is re-saved via MS Word with revisions accepted,
                    // widthAfter in the re-saved document does not match the stored grid.
                    // So even when the document is saved to .doc and re-opened, preserved widthAfter values
                    // may not match the actual column widths. I removed the assertion that used to be here.
                    // Also happens with TestJira9653FormatRevision, but the difference is minor there.
                }

                // It should not happen, but do not overwrite widthAfter in pct units, just in case.
                if (rowPr.WidthAfter.Type != PreferredWidthType.Percent)
                    rowPr.WidthAfterTwips = widthAfterFromDoc;
                else
                    Debug.Fail("Unexpected: trying to overwrite with after in pct units with twips.");
            }
            else
            {
                // Zero grid after should be in the model.
                if (rowPr.GridAfter != 0)
                {
                    rowPr.SetAttr(TableAttr.Sys_GridAfter, 0);
                    Debug.WriteLine("GridAfter calculated from cell widths is zero, but a not zero value is specified in the model. Replaced with 0.");
                }
                if (rowPr.WidthAfterTwips != 0)
                {
                    rowPr.WidthAfterTwips = 0;
                    Debug.WriteLine("WidthAfter calculated from cell widths is zero, but a not zero value is specified in the model. Replaced with 0.");
                    // The issue happens with TestJira9656, TestJira9653FormatRevision (both with revisions).
                    // TODO investigate merging of tables with revisions.
                }
            }
        }
    }
}
