// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Anatoly Sidorenko

using System;
using System.Collections.Generic;
using System.Data;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// Helps to print DataSet or DataTable to console.
    /// Used by tests or for debug.
    /// </summary>
    public static class DataSetWriter
    {
        public static void PrintDataSetSchema(DataSet dataSet)
        {
            PrintDataSet(dataSet, false);
        }

        public static void PrintDataSet(DataSet dataSet)
        {
            PrintDataSet(dataSet, true);
        }

        public static void PrintDataTableSchema(DataTable table)
        {
            PrintDataTable(table, false);
        }

        public static void PrintDataTable(DataTable table)
        {
            PrintDataTable(table, true);
        }

        public static void PrintRelations(DataSet dataSet)
        {
            if (dataSet == null)
                throw new InvalidOperationException("DataSet cannot be null");

            int offset = 5;
            int[] widths = new int[5];
            foreach (DataRelation dr in dataSet.Relations)
            {
                widths[0] = Max(("Relation: '" + dr.RelationName + "'").Length, widths[0]);
                widths[1] = Max(("Parent: '" + dr.ParentTable.TableName + "'").Length, widths[1]);
                widths[2] = Max(("Child: '" + dr.ChildTable.TableName + "'").Length, widths[2]);
                widths[3] = Max(("Parent Column: '" + ToColumnNames(dr.ParentColumns) + "'").Length, widths[3]);
                widths[4] = Max(("Child Column: '" + ToColumnNames(dr.ChildColumns) + "'").Length, widths[4]);
            }
            foreach (DataRelation dr in dataSet.Relations)
            {
                Write(("Relation: '" + dr.RelationName + "'"), -(widths[0] + offset));
                Write(("Parent: '" + dr.ParentTable.TableName + "'"), -(widths[1] + offset));
                Write(("Child: '" + dr.ChildTable.TableName + "'"), -(widths[2] + offset));
                Write(("Parent Column: '" + ToColumnNames(dr.ParentColumns) + "'"), -(widths[3] + offset));
                Write(("Child Column: '" + ToColumnNames(dr.ChildColumns) + "'"), -(widths[4] + offset));
                NextLine();
            }
        }

        private static void PrintDataSet(DataSet dataSet, bool printContent)
        {
            if (dataSet == null)
                throw new InvalidOperationException("DataSet cannot be null");

            foreach (DataTable table in dataSet.Tables)
            {
                if (table == null)
                    continue;

                PrintDataTable(table, printContent);
                NextLine();
            }
            PrintRelations(dataSet);
        }

        private static void PrintDataTable(DataTable table, bool printContent)
        {
            if (table == null)
                throw new InvalidOperationException("DataTable cannot be null");

            int[] columnWidths = GetColumnWidths(table);
            int maxTableWidth = GetMaxTableWidth(columnWidths);

            PrintTableName(table, maxTableWidth);
            PrintHorizontalBorder(maxTableWidth);
            PrintTableColumns(table, columnWidths);
            PrintHorizontalBorder(maxTableWidth);
            if (printContent)
                PrintTableContent(table, columnWidths, maxTableWidth);
        }

        #region helpers

        private static void PrintTableColumns(DataTable table, int[] columnWidths)
        {
            List<string> pkColNames = ToPkString(table);

            for (int columnIdx = 0; columnIdx < table.Columns.Count; columnIdx++)
            {
                DataColumn column = table.Columns[columnIdx];
                string columnName = column.ColumnName;
                string type = column.DataType != typeof(string) ? ("(" + column.DataType + ")") : "";
                String pkMark = pkColNames.Contains(columnName) ? "(PK)" : "";
                Write("|");
                Write(columnName + type + pkMark, columnWidths[columnIdx]);
            }
            Write("|");
            NextLine();
        }

        private static void PrintTableContent(DataTable table, int[] columnWidths, int maxTableLength)
        {
            bool rowsExists = false;
            for (int rowIdx = 0; rowIdx < table.Rows.Count; rowIdx++)
            {
                DataRow row = table.Rows[rowIdx];
                if (row.RowState == DataRowState.Deleted)
                    continue;

                rowsExists = true;

                for (int colIdx = 0; colIdx < columnWidths.Length; colIdx++)
                {
                    Write("|");
                    Write(GetColumnValue(row[colIdx]), columnWidths[colIdx]);
                }
                Write("|");
                NextLine();
            }

            if (!rowsExists)
                PrintInTheMiddle("EMPTY", maxTableLength, false);

            PrintHorizontalBorder(maxTableLength);
        }

        private static string GetColumnValue(object obj)
        {
            if (obj == null)
                return "null";

            return obj.ToString().Replace("\n", "&#10;").Replace("\r", "&#13;");
        }

        private static int GetMaxTableWidth(int[] columnWidths)
        {
            int maxWidth = 0;
            foreach (int w in columnWidths)
                maxWidth += w;

            maxWidth += columnWidths.Length - 1; // for |
            return maxWidth;
        }

        private static int[] GetColumnWidths(DataTable table)
        {
            if (table == null)
                throw new InvalidOperationException("DataTable cannot be null");

            List<string> pkColNames = ToPkString(table);

            DataColumnCollection columns = table.Columns;
            int[] widths = new int[columns.Count];
            for (int columnIdx = 0; columnIdx < columns.Count; columnIdx++)
            {
                String columnName = columns[columnIdx].ColumnName;
                int columnNameWidth = columnName.Length;
                int typeLength = table.Columns[columnIdx].DataType != typeof(string) ? ("(" + table.Columns[columnIdx].DataType + ")").Length : 0;
                int maxRowValueLength = GetMaxRowValueLength(table, columnIdx);
                int pkMarkLength = pkColNames.Contains(columnName) ? "(PK)".Length : 0;
                widths[columnIdx] = Max(columnNameWidth + typeLength + pkMarkLength, maxRowValueLength) + 2;
            }
            return widths;
        }

        private static int GetMaxRowValueLength(DataTable table, int columnIdx)
        {
            int maxLength = 0;
            foreach (DataRow row in table.Rows)
            {
                int length = row[columnIdx] != null ? row[columnIdx].ToString().Length : 0;
                maxLength = Max(maxLength, length);
            }
            return maxLength;
        }

        private static void PrintTableName(DataTable table, int maxTableWidth)
        {
            PrintInTheMiddle(table.TableName, maxTableWidth, true);
        }

        private static void PrintInTheMiddle(string message, int maxTableWidth, bool drawTopBorder)
        {
            int halfTableName = message.Length / 2;
            int halfMaxWidth = maxTableWidth / 2;
            int emptySpaceBeforeName = halfMaxWidth - halfTableName;
            int emptySpaceAfterName = maxTableWidth - emptySpaceBeforeName - message.Length;

            if (drawTopBorder)
            {
                Write("+");
                WriteRepeadetly("-", maxTableWidth);
                Write("+");
                NextLine();
            }
            Write("|");
            WriteRepeadetly(" ", emptySpaceBeforeName);
            Write(message);
            WriteRepeadetly(" ", emptySpaceAfterName);
            Write("|");
            NextLine();
        }

        private static void PrintHorizontalBorder(int maxTableWidth)
        {
            Write("+");
            WriteRepeadetly("-", maxTableWidth);
            Write("+");
            NextLine();
        }

        private static void Write(string msg)
        {
            Console.Write(msg);
        }

        private static void Write(string msg, int width)
        {
            Console.Write(string.Format("{0," + width + "}", msg));
        }

        private static void WriteRepeadetly(string msg, int count)
        {
            for (int i = 0; i < count; i++)
                Console.Write(msg);
        }

        private static void NextLine()
        {
            Console.WriteLine();
        }

        private static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        private static List<string> ToPkString(DataTable table)
        {
            DataColumn[] pk = table.PrimaryKey;
            List<string> pkColNames = new List<string>(pk.Length);
            foreach (DataColumn dataColumn in pk)
                pkColNames.Add(dataColumn.ColumnName);

            return pkColNames;
        }

        private static string ToColumnNames(DataColumn[] columns)
        {
            if (columns == null || columns.Length == 0)
                return string.Empty;

            string[] names = new string[columns.Length];
            for (int i = 0; i < columns.Length; i++)
                names[i] = columns[i].ColumnName;

            return string.Join(", ", names);
        }

        #endregion helpers
    }
}
