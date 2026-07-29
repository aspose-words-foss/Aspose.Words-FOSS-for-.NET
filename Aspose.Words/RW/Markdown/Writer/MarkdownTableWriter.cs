// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2020 by Ilya Navrotskiy

using System.Collections.Generic;
using System.Text;
using Aspose.Collections;
using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Tables into markdown.
    /// </summary>
    internal class MarkdownTableWriter
    {
        /// <summary>
        /// Creates instance for a specified table.
        /// </summary>
        internal MarkdownTableWriter(Table table, MarkdownWriter writer)
        {
            Debug.Assert(writer != null);
            Debug.Assert(table != null);

            mWriter = writer;
            mTable = table;
        }

        /// <summary>
        /// Processes start of the table.
        /// </summary>
        internal void OnTableStart()
        {
            // WORDSNET-27382 Handle a new implemented option.
            if (IsExportAsHtml)
            {
                string tableAsRawHtml = mTable.ToString(mWriter.RawHtmlSaveOptions);
                Builder.Append(tableAsRawHtml);
                Builder.AppendLine();
                return;
            }

            // Add one blank line before the table.
            if (!mTable.IsFirstChild)
            {
                // WORDSNET-26064 We have allowed blank paragraphs in Document,
                // and they are natural separators in Markdown. So, no need to
                // add another one blank line if it has been existed there already.
                Paragraph prevPara = mTable.PreviousSibling as Paragraph;
                if ((prevPara != null) && prevPara.HasChildNodes)
                    Builder.Append(ParagraphBreak);
            }

            if (UseHtmlSyntax)
                Builder.Append("<table>");

            // WORDSNET-26757 Allow table to fall under a previous list item,
            // if it has the same left indent as this list item and thus make
            // appropriate left indent for the table.
            Paragraph listItemSource = MarkdownParagraphWriter.GetListItemSource(mTable);
            MarkdownParagraphWriter listItemSourceWriter = mWriter.GetParagraphWriterByNode(listItemSource);
            if (listItemSourceWriter != null)
            {
                int indentLength = 0;
                foreach (string listLabel in listItemSourceWriter.ListLabels)
                {
                    if (listLabel != null)
                        indentLength += listLabel.Length;
                }
                mIndent = new string(' ', indentLength);
            }
        }

        /// <summary>
        /// Processes end of the table.
        /// </summary>
        internal void OnTableEnd()
        {
            // When this option is enabled, whole table is processed in 'OnTableStart()', so just exit.
            if (IsExportAsHtml)
                return;

            if (UseHtmlSyntax)
            {
                Builder.Append("</table>");
                Builder.Append(ParagraphBreak);
            }
        }

        /// <summary>
        /// Starts nested table.
        /// </summary>
        internal MarkdownTableWriter StartNestedTable(Table table)
        {
            // When this option is enabled, nested tables are processed in 'OnTableStart()' of parent table.
            if (!IsExportAsHtml)
            {
                // Write collected paragraphs into current cell.
                FlushParagraphs();

                // Fill with empty cells to the end and finish split row.
                // WORDSNET-25339 Resilience when number of columns in table cannot be determined properly.
                Builder.Append(CellBlock.SeparatorChar, System.Math.Max(0, ColsCount - CurCell.ColumnIndex));
                Builder.Append(ParagraphBreak);

                // Write delimiter row for split part of the table, if it is not written yet.
                if (CurCell.ParentRow == FirstRow)
                {
                    WriteDelimiterRow(FirstRow);
                    Builder.Append(ParagraphBreak);
                }

                // Add one blank line before nested table.
                Builder.Append(ParagraphBreak);

                // Remember cell where nested table is inserted.
                mSplitCell = CurCell;
            }

            return new MarkdownTableWriter(table, mWriter);
        }

        /// <summary>
        /// Processes end of a nested table.
        /// </summary>
        internal void OnNestedTableEnd()
        {
            // When this option is enabled, whole table is processed in 'OnTableStart()', so just exit.
            if (IsExportAsHtml)
                return;

            // Add one blank line after nested table.
            Builder.Append(ParagraphBreak);

            // Fill current row with empty cells to the split cell.
            int cellsCountBefore = mSplitCell.ColumnIndex + 1;
            Builder.Append(CellBlock.SeparatorChar, cellsCountBefore);

            // Write collected paragraphs into the current cell.
            FlushParagraphs();
        }

        /// <summary>
        /// Processes end of a row.
        /// </summary>
        internal void OnRowStart(Row row)
        {
            // When this option is enabled, whole table is processed in 'OnTableStart()', so just exit.
            if (IsExportAsHtml)
                return;

            // WORDSNET-26757 Add left indent to the table if it falls into a list.
            Builder.Append(mIndent);

            if (UseHtmlSyntax)
                Builder.Append("<tr>");
        }

        /// <summary>
        /// Processes end of a row.
        /// </summary>
        internal void OnRowEnd(Row row)
        {
            // When this option is enabled, whole table is processed in 'OnTableStart()', so just exit.
            if (IsExportAsHtml)
                return;

            if (UseHtmlSyntax)
            {
                Builder.Append("</tr>");
                Builder.Append(ParagraphBreak);
            }
            else
            {
                // Close last cell.
                // WORDSNET-25339 Resilience when number of columns in table cannot be determined properly.
                Builder.Append(CellBlock.SeparatorChar, System.Math.Max(1, ColsCount - CurCell.ColumnIndex));
                Builder.Append(ParagraphBreak);

                if (row == FirstRow)
                {
                    WriteDelimiterRow(row);
                    Builder.Append(ParagraphBreak);
                }
            }
        }

        /// <summary>
        /// Processes start of a cell.
        /// </summary>
        internal void OnCellStart(Cell cell)
        {
            // When this option is enabled, whole table is processed in 'OnTableStart()', so just exit.
            if (IsExportAsHtml)
                return;

            mCurCell = cell;

            if (UseHtmlSyntax)
            {
                if (cell.CellPr.IsMergedToPrevious)
                    return;

                string cellTag = (cell.ParentRow == FirstRow) ? "th" : "td";
                string rowSpan = string.Empty;
                string colSpan = string.Empty;

                if ((cell.CellPr.HorizontalMerge == CellMerge.First) || (cell.CellPr[CellAttr.Sys_CellSpan] != null))
                    colSpan = string.Format(@" colspan=""{0}""", GetHorizontalMergedCellCount(cell));
                if (cell.CellPr.VerticalMerge == CellMerge.First)
                    rowSpan = string.Format(@" rowspan=""{0}""", GetVerticalMergedCellCount(cell));

                Builder.Append(string.Format("<{0}{1}{2}{3}>", cellTag, colSpan, rowSpan, GetVerticalAlign()));
            }
            else
                Builder.Append(CellBlock.SeparatorChar);
        }

        /// <summary>
        /// Processes end of a cell.
        /// </summary>
        internal void OnCellEnd(Cell cell)
        {
            // When this option is enabled, whole table is processed in 'OnTableStart()', so just exit.
            if (IsExportAsHtml)
                return;

            FlushParagraphs();
            if (UseHtmlSyntax && !cell.CellPr.IsMergedToPrevious)
            {
                string cellTag = (cell.ParentRow == FirstRow) ? "th" : "td";
                Builder.AppendFormat("</{0}>", cellTag);
            }
        }

        /// <summary>
        /// Adds a specified paragraph to the list of paragraphs pending for writing into current cell.
        /// </summary>
        /// <remarks>
        /// We don`t write paragraph inside cell immediately as there can be multiple paragraphs in the cell.
        /// In such case they should be wrapped into HTML elements &lt;p&gt; and &lt;/p&gt;.
        /// </remarks>
        internal void AddParagraph(MarkdownParagraphWriter paragraphWriter)
        {
            mParagraphs.Add(paragraphWriter);
        }

        /// <summary>
        /// Flushes paragraphs to the writer.
        /// </summary>
        private void FlushParagraphs()
        {
            foreach (MarkdownParagraphWriter paragraph in mParagraphs)
            {
                if ((mWriter.SaveOptions.EmptyParagraphExportMode == MarkdownEmptyParagraphExportMode.None) && paragraph.IsEmpty)
                    continue;

                // Write HTML paragraph opening element, if this is multi-paragraph cell.
                if (mParagraphs.Count > 1)
                    Builder.Append(HtmlParaOpen);

                paragraph.Write();

                // Write HTML paragraph closing element, if this is multi-paragraph cell.
                if (mParagraphs.Count > 1)
                    Builder.Append(HtmlParaClose);
            }

            mParagraphs.Clear();
        }

        /// <summary>
        /// Writes delimiter row based on a specified row.
        /// </summary>
        private void WriteDelimiterRow(Row row)
        {
            // WORDSNET-26757 Add left indent to the table if it falls into a list.
            Builder.Append(mIndent);

            Builder.Append(CellBlock.SeparatorChar);

            // WORDSNET-25339 Loop through the all columns of table
            // instead of all cells of the current row.
            for (int i = 0; i < ColsCount; i++)
            {
                // It is not necessary, but lets add one indentation space for better looking.
                Builder.Append(' ');

                // WORDSNET-20425 New option is introduced to control over alignment of the tables content.
                ParagraphAlignment alignment = ParagraphAlignment.Left;
                if ((i < row.Cells.Count) && row.Cells[i].FirstParagraph != null)
                    alignment = GetParagraphAlignment(row.Cells[i].FirstParagraph);

                if ((alignment == ParagraphAlignment.Left) || (alignment == ParagraphAlignment.Center))
                    Builder.Append(CellBlock.AlignmentChar);

                Builder.Append(CellBlock.ContentChar);

                if ((alignment == ParagraphAlignment.Right) || (alignment == ParagraphAlignment.Center))
                    Builder.Append(CellBlock.AlignmentChar);

                // It is not necessary, but lets add one indentation space for better looking.
                Builder.Append(' ');
                Builder.Append(CellBlock.SeparatorChar);
            }
        }

        /// <summary>
        /// Returns paragraph alignment for a specified paragraph in respect of
        /// <see cref="MarkdownSaveOptions.TableContentAlignment"/> saving option.
        /// </summary>
        private ParagraphAlignment GetParagraphAlignment(Paragraph paragraph)
        {
            switch (mWriter.SaveOptions.TableContentAlignment)
            {
                case TableContentAlignment.Left:
                    return ParagraphAlignment.Left;
                case TableContentAlignment.Center:
                    return ParagraphAlignment.Center;
                case TableContentAlignment.Right:
                    return ParagraphAlignment.Right;
                default:
                    return paragraph.ParaPr.Alignment;
            }
        }

        /// <summary>
        /// Gets the format string for the vertical alignment of a table cell.
        /// If the vertical alignment value is equal to the default value, an empty string is returned.
        /// </summary>
        private string GetVerticalAlign()
        {
            if (mCurCell.CellPr.VerticalAlignment == CellVerticalAlignment.Center)
                return string.Empty;

            return string.Format(@" valign=""{0}""", (mCurCell.CellPr.VerticalAlignment == CellVerticalAlignment.Top)
                        ? "top"
                        : "bottom");
        }

        /// <summary>
        /// Gets the number of horizontally merged cells adjacent to the specified cell.
        /// </summary>
        private static int GetHorizontalMergedCellCount(Cell cell)
        {
            int result = 1;

            // If the Sys_CellSpan attribute is set, we prefer to use it.
            if (cell.CellPr[CellAttr.Sys_CellSpan] != null)
                return (int)cell.CellPr[CellAttr.Sys_CellSpan];

            Cell curCell = cell.NextCell;
            while ((curCell != null) && (curCell.CellPr.HorizontalMerge == CellMerge.Previous))
            {
                result++;
                curCell = curCell.NextCell;
            }

            return result;
        }

        /// <summary>
        /// Gets the number of vertically merged cells adjacent to the specified cell.
        /// </summary>
        private static int GetVerticalMergedCellCount(Cell cell)
        {
            int result = 1;
            int colIndex = cell.ColumnIndex;

            Row row = cell.ParentRow.NextRow;
            // WORDSNET-25335 Get rid of row.NextRow in clause to avoid null reference exception.
            while ((row != null) && (row.Cells[colIndex] != null) && (row.Cells[colIndex].CellPr.VerticalMerge == CellMerge.Previous))
            {
                result++;
                row = row.NextRow;
            }

            return result;
        }

        /// <summary>
        /// Returns True if the current table has merged cells.
        /// </summary>
        private bool HasMergedCells
        {
            get
            {
                foreach (Row row in mTable.Rows)
                foreach (Cell cell in row.Cells)
                    if (cell.CellPr.IsMergedToPrevious)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Gets the number of columns in the current table.
        /// </summary>
        private int ColsCount
        {
            get
            {
                if (mColsCount == -1)
                {
                    // First try to get from TableGrid.
                    IntList colsWidths = (IntList)mTable.TablePr[TableAttr.Sys_TableGrid];
                    if (colsWidths != null)
                    {
                        mColsCount = colsWidths.Count;
                    }
                    // Then try to get from First row cells.
                    else if (FirstRow != null)
                    {
                        mColsCount = FirstRow.Cells.Count;
                    }

                    if (mColsCount < 0)
                        mColsCount = 0;
                }

                return mColsCount;
            }
        }

        /// <summary>
        /// Returns True if Html syntax shall be used to correctly write the attributes of the current table.
        /// </summary>
        internal bool HtmlSyntaxRequired
        {
            get
            {
                // WORDSNET-28289 Added a new flag into MarkdownExportAsHtml.
                if (mWriter.SaveOptions != null &&
                    (mWriter.SaveOptions.ExportAsHtml & MarkdownExportAsHtml.NonCompatibleTables) == 0)
                    return false;

                // Markdown does not support nested table in Html syntax.
                bool hasNestedTable = mTable.GetChild(NodeType.Table, 0, true) != null;

                return HasMergedCells && !hasNestedTable;
            }
        }

        /// <summary>
        /// Returns true, if the current writer is in Html syntax mode.
        /// </summary>
        private bool UseHtmlSyntax
        {
            get { return mWriter.UseHtmlSyntax; }
        }

        /// <summary>
        /// The first logical row.
        /// </summary>
        /// <remarks>
        /// Note, this can be different from the actual first row of the table due to nested tables,
        /// which split the table and moves first logical row to the place where it is inserted.
        /// </remarks>
        private Row FirstRow
        {
            get { return (mSplitCell == null) ? mTable.FirstRow : mSplitCell.ParentRow; }
        }

        /// <summary>
        /// The current cell.
        /// </summary>
        private Cell CurCell
        {
            get { return (mCurCell == null) ? FirstRow.FirstCell : mCurCell; }
        }

        /// <summary>
        /// Gets current builder.
        /// </summary>
        private StringBuilder Builder
        {
            get { return mWriter.Builder; }
        }

        /// <summary>
        /// Gets string representing current paragraph break value to use in the writer.
        /// </summary>
        private string ParagraphBreak
        {
            get { return mWriter.SaveOptions.ParagraphBreak; }
        }

        /// <summary>
        /// Gets a boolean value indicating either table should be exported as raw HTML
        /// in accordance with specified <see cref="MarkdownSaveOptions.ExportAsHtml"/> option.
        /// </summary>
        private bool IsExportAsHtml
        {
            get
            {
                return (mWriter.SaveOptions != null) && ((mWriter.SaveOptions.ExportAsHtml & MarkdownExportAsHtml.Tables) != 0);
            }
        }

        /// <summary>
        /// The table being written.
        /// </summary>
        private readonly Table mTable;

        /// <summary>
        /// The underlying writer.
        /// </summary>
        private readonly MarkdownWriter mWriter;

        /// <summary>
        /// The list of paragraphs pending for writing.
        /// </summary>
        /// <remarks>
        /// We don't write paragraphs immediately inside a cell, because we need to know
        /// either that cell has multiple paragraphs or not (as this should be written using inline HTML blocks).
        /// </remarks>
        private readonly List<MarkdownParagraphWriter> mParagraphs = new List<MarkdownParagraphWriter>();

        /// <summary>
        /// The currently processing cell.
        /// </summary>
        private Cell mCurCell;

        /// <summary>
        /// The cell where nested table is inserted, if any.
        /// </summary>
        private Cell mSplitCell;

        private int mColsCount = -1;

        private const string HtmlParaOpen = "<p>";
        private const string HtmlParaClose = "</p>";

        /// <summary>
        /// Specifies an indent of the table.
        /// </summary>
        private string mIndent = "";
    }
}
