// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/01/2010 by Dmitry Vorobyev

using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Designates a position of a cell in a table.
    /// </summary>
    internal class CellPosition
    {
        internal CellPosition(Cell cell)
            : this(cell.ParentTable, cell.ColumnIndex, cell.RowIndex)
        {
        }

        private CellPosition(Table table, int columnIndex, int rowIndex)
        {
            mTable = table;
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        /// <summary>
        /// Parse the specified text to a cell position.
        /// </summary>
        internal static CellPosition TryParse(Table table, string name, int cellIndex)
        {
            return TryParseA1Notation(table, name, cellIndex) ??
                   TryParseR1C1Notation(table, name);
        }

        private static CellPosition TryParseA1Notation(Table table, string name, int cellIndex)
        {
            Match match = gCellPositionA1NotationRegex.Match(name);

            if (!match.Success)
                return null;

            Group startGroup = match.Groups[1];
            Group endGroup = match.Groups[2];

            Debug.Assert(startGroup.Success || endGroup.Success);

            if (startGroup.Success && endGroup.Success)
            {
                // One cell.
                // WORDSNET-9786 The name matches to regex pattern, but can no be converted into cell position, because contains row index more than max int.
                // Return null, if row index can not be parsed from the name.
                NullableInt32 rowIndex = FormatterPal.ParseNullableInt(endGroup.Value);
                return rowIndex.HasValue
                    ? new CellPosition(
                        table,
                        NumberConverter.ParseExcelColumnName(startGroup.Value),
                        rowIndex.Value - 1)
                    : null;
            }
            else if (startGroup.Success)
            {
                // One column.
                return new CellPosition(table, NumberConverter.ParseExcelColumnName(startGroup.Value), cellIndex);
            }
            else
            {
                // One row.
                NullableInt32 rowIndex = FormatterPal.ParseNullableInt(endGroup.Value);
                return rowIndex.HasValue
                    ? new CellPosition(table, cellIndex, rowIndex.Value - 1)
                    : null;
            }
        }

        private static CellPosition TryParseR1C1Notation(Table table, string name)
        {
            Match match = gCellPositionR1C1NotationRegex.Match(name);

            if (!match.Success)
                return null;

            Group rowGroup = match.Groups[1];
            Group columnGroup = match.Groups[2];

            NullableInt32 rowIndex = FormatterPal.ParseNullableInt(rowGroup.Value);
            NullableInt32 columnIndex = FormatterPal.ParseNullableInt(columnGroup.Value);

            if (!rowIndex.HasValue || !columnIndex.HasValue)
                return null;

            if ((rowIndex.Value == 0) || (columnIndex.Value == 0))
                return null;

            return new CellPosition(table, columnIndex.Value - 1, rowIndex.Value - 1);
        }

        internal static bool AreSame(CellPosition position1, CellPosition position2)
        {
            return (position1.ColumnIndex == position2.ColumnIndex) && (position1.RowIndex == position2.RowIndex);
        }

        internal CellPosition Clone()
        {
            return (CellPosition)MemberwiseClone();
        }

        public override string ToString()
        {
            string columnName = NumberConverter.ToExcelColumnName(ColumnIndex) ?? InvalidColumnName;

            return string.Format("{0}{1}", columnName, RowIndex + 1);
        }

        /// <summary>
        /// Moves the current position right or left.
        /// </summary>
        /// <param name="step">The number of columns to move. A negative value moves left.</param>
        /// <returns>A boolean value indicating if the new position is valid (true) or not (false)</returns>
        [JavaConvertCheckedExceptions]
        internal bool MoveRight(int step)
        {
            // WORDSNET-6456 Because mColumnIndex might get a negative value in the constructor
            // (after parsing an invalid column name), we need to check it before using.
            if (ColumnIndex < 0)
                return false;

            int newColumnIndex = ColumnIndex + step;
            bool isNewIndexValid = IsColumnValid(RowIndex, newColumnIndex);
            if(isNewIndexValid)
                ColumnIndex = newColumnIndex;
            return isNewIndexValid;
        }

        /// <summary>
        /// Moves the current position up or down.
        /// </summary>
        /// <param name="step">The number of rows to move. A negative value moves up.</param>
        /// <returns>A boolean value indicating if the new position is valid (true) or not (false)</returns>
        internal bool MoveDown(int step)
        {
            // WORDSNET-6456 Since mColumnIndex might get a negative value in the constructor
            // (after parsing an invalid column name), we have to prohibit any operation with invalid cell position.
            if (ColumnIndex < 0)
                return false;

            int newRowIndex = RowIndex + step;
            bool isNewIndexValid = IsRowValid(newRowIndex);
            if (isNewIndexValid)
                RowIndex = newRowIndex;
            return isNewIndexValid;
        }

        /// <summary>
        /// Gets a cell located at the position.
        /// </summary>
        internal Cell Cell
        {
            get { return mTable.GetCell(ColumnIndex, RowIndex); }
        }

        internal bool IsValid
        {
            get { return IsColumnValid(RowIndex, ColumnIndex); }
        }

        private bool IsRowValid(int rowIndex)
        {
            return (0 <= rowIndex) && (rowIndex < mTable.Rows.Count);
        }

        private bool IsColumnValid(int rowIndex, int columnIndex)
        {
            return IsRowValid(rowIndex) &&
                   (0 <= columnIndex) &&
                   (columnIndex < mTable.Rows[rowIndex].Cells.Count);
        }

        internal bool IsAtFirstColumn
        {
            get { return ColumnIndex == 0; }
        }

        internal bool IsAtFirstRow
        {
            get { return RowIndex == 0; }
        }

        internal bool IsAtLastColumn
        {
            get { return ColumnIndex == mTable.Rows[RowIndex].Cells.Count - 1; }
        }

        internal bool IsAtLastRow
        {
            get { return RowIndex == mTable.Rows.Count - 1; }
        }

        /// <summary>
        /// Gets or sets the index of the cell's column.
        /// </summary>
        internal int ColumnIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the cell's row.
        /// </summary>
        internal int RowIndex { get; set; }

        private readonly Table mTable;

        private static readonly Regex gCellPositionA1NotationRegex = new Regex(
            @"^([A-Z]{1,2})?(\d{1,5})?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex gCellPositionR1C1NotationRegex = new Regex(
            @"^R(\d{1,5})C(\d{1,5})$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string InvalidColumnName = "!Invalid column name.";
    }
}
