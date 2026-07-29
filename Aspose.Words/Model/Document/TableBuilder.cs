// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/11/2004 by Roman Korchagin
using System;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies state of processing a table.
    /// </summary>
    internal enum TableState
    {
        /// <summary>
        /// Not initialized.
        /// </summary>
        None,
        /// <summary>
        /// Inside Table
        /// </summary>
        InTable,
        /// <summary>
        /// Inside Row
        /// </summary>
        InRow,
        /// <summary>
        /// Inside Cell
        /// </summary>
        InCell
    }

    /// <summary>
    /// Works together with DocumentBuilder to build tables in a document.
    /// </summary>
    internal class TableBuilder
    {
        internal TableBuilder(DocumentBuilder builder, bool contextFormatting)
        {
            if (builder == null)
                throw new ArgumentOutOfRangeException("builder");

            mBuilder = builder;
            mContextFormatting = contextFormatting;
        }

        internal Table StartTable()
        {
            if (mTableState != TableState.None)
                throw new InvalidOperationException("Cannot start a table in this state.");

            // If you try to start a table in the middle of a paragraph, start it from a new paragraph.
            if (!mBuilder.IsAtStartOfParagraph)
            {
                // WORDSNET-27500 Ensure that cursor is moved to SDT end.
                mBuilder.EnsureAtStructuredDocumentTagEnd();
                mBuilder.InsertParagraph();
            }

            // Insert the table node.
            mCurTable = new Table(mBuilder.Document);
            mBuilder.InsertBeforeCurPara(mCurTable);

            mTableState = TableState.InTable;
            return mCurTable;
        }

        internal Table EndTable()
        {
            //Smart table building API method, end previous cell and row implicitly.
            if (mTableState == TableState.InCell)
                EndCell();

            if (mTableState == TableState.InRow)
                EndRow();

            if (mTableState != TableState.InTable)
                throw new InvalidOperationException("Cannot end a table in this state.");

            //Move the cursor outside of the table and this will detach table attributes from the model.
            Paragraph nextPara = (Paragraph)mCurTable.NextNonMarkupCompositeLimited;
            if (!mContextFormatting)
                nextPara.ParagraphBreakRunPr = mBuilder.GetRunPrCopy();

            mBuilder.MoveTo(nextPara, 0);

            // WORDSNET-21602 To avoid unwanted cell merging in the next table (if any) reset merge.
            mBuilder.CellFormat.VerticalMerge = CellMerge.None;
            mBuilder.CellFormat.HorizontalMerge = CellMerge.None;

            mTableState = TableState.None;
            Table result = mCurTable;
            mCurTable = null;
            return result;
        }

        /// <summary>
        /// Creates a new table row node and adds to the current table.
        /// </summary>
        internal Row StartRow()
        {
            if (mTableState != TableState.InTable)
                throw new InvalidOperationException("Cannot start a row in this state.");

            TablePr tablePr;

            // If we are adding rows to a table, I want all rows to have same attributes, hence clone from the previous row.
            Row prevRow = mCurTable.LastRow;
            if (prevRow != null)
            {
                tablePr = prevRow.TablePr.Clone();

                // Some formatting can be set before row is created, so copy them into newly created row.
                mBuilder.GetTablePrCopy().ExpandTo(tablePr);
            }
            else
            {
                tablePr = mBuilder.GetTablePrCopy();
            }

            // Create a table row and insert it into the model before the current paragraph.
            mCurRow = new Row(mBuilder.Document, tablePr);
            mCurTable.AppendChild(mCurRow);

            mTableState = TableState.InRow;
            return mCurRow;
        }

        internal Row EndRow()
        {
            // Smart table building API method, end previous cell implicitly.
            if (mTableState == TableState.InCell)
                EndCell();

            if (mTableState != TableState.InRow)
                throw new InvalidOperationException("Cannot end a row in this state.");

            mTableState = TableState.InTable;
            Row result = mCurRow;
            mCurRow = null;
            return result;
        }

        /// <summary>
        /// Creates a new table cell node with a cell mark, adds to the current table row
        /// and moves the cursor to it.
        /// </summary>
        internal Cell StartCell()
        {
            if (mTableState != TableState.InRow)
                throw new InvalidOperationException("Cannot start a cell in this state.");

            // Create a new cell and insert it into the current table row.
            Cell cell = new Cell(mBuilder.Document, mBuilder.GetCellPrCopy());
            mCurRow.AppendChild(cell);

            // Inside the cell, create a paragraph.
            Paragraph cellEndPara = new Paragraph(mBuilder.Document, mBuilder.GetParaPrCopy(), mBuilder.GetRunPrCopy());
            cell.AppendChild(cellEndPara);

            // Moving the cursor makes sure CellFormat and RowFormat get attached to the cell.
            mBuilder.MoveTo(cellEndPara, 0);

            mTableState = TableState.InCell;
            return cell;
        }

        internal void EndCell()
        {
            if (mTableState != TableState.InCell)
                throw new InvalidOperationException("Cannot end a cell in this state.");

            mTableState = TableState.InRow;

            // We want all cells have the same formatting. Hence we clone current cell formatting to the builder.
            // When new cell is started, we copy formatting from the builder to this new cell again.
            mBuilder.SaveCurCellFormatting();

            // Does not do anything special regarding attaching/detaching table formatting
            // so it is still possible to modify cell attributes after it was added and before
            // the new cell is started. Not very clean, but okay since EndCell is normally
            // followed by StartCell or EndRow.

            /*
             * TODO 3 I might try a different approach for attaching/detaching table formatting
             * that could be a little easier to understand:
             *
             * StartTable - DetachAll because no row and no cell yet and changing row and cell format now should not change previous table format.
             * StartRow - AttachRow so modifying it will change the current row.
             * StartCell - Move into the cell attaches both Cell and Row since the cursor is now inside a cell.
             * EndCell - DetachCell so modifying cell properties has no effect on the finished cell.
             * EndRow -    DetachRow so modifying row will not change the finished row.
             * EndTable - Moves the cursor out of the table so another complete detach is made (skipped because already detached).
             *
             */
        }

        internal TableState TableState
        {
            get { return mTableState; }
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentBuilder mBuilder;
        private readonly bool mContextFormatting;
        private TableState mTableState;
        private Table mCurTable;
        private Row mCurRow;
    }
}
