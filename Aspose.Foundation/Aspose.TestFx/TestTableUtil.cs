// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/09/2015 by Vyacheslav Durin

using System;
using System.Data;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx.Pal;

namespace Aspose.TestFx
{
    /// <summary>
    /// This class contains handy methods to work with ADO.NET classes.
    /// PalTable was replaced with this autoportable class.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Manual implementation.")]
    public static class TestTableUtil
    {
        // fabrics should be used instead of ctors:
        public static DataTable GetTestTable()
        {
            return new DataTable(DefaultTestTableName);
        }

        public static DataTable GetTestTable(string tableName)
        {
            if (!StringUtil.HasChars(tableName))
                tableName = DefaultTestTableName;

            return new DataTable(tableName);
        }

        public static DataTable GetTestTable(string tableName, string[] columnNames)
        {
            DataTable dt = GetTestTable(tableName);
            AddColumns(dt, columnNames);
            return dt;
        }

        public static DataTable GetTestTable(string[] columnNames)
        {
            DataTable dt = GetTestTable();
            AddColumns(dt, columnNames);
            return dt;
        }

        public static DataTable GetTestTable(string dataTableName, string[] columnNames, bool addTestRow)
        {
            DataTable dt = GetTestTable(dataTableName);
            AddColumns(dt, columnNames);
            if (addTestRow)
                AddTestRow(dt);
            return dt;
        }

        public static DataTable GetTestTable(string dataTableName, string columnName, bool addTestRow)
        {
            return GetTestTable(dataTableName, new string[] { columnName }, addTestRow);
        }

        public static DataTable GetTestTable(string dataTableName, string[] columnNames, object[] oneRowValues)
        {
            DataTable dt = GetTestTable(dataTableName);
            AddColumns(dt, columnNames);
            AddRow(dt, oneRowValues);
            return dt;
        }

        public static DataTable GetTestTable(string dataTableName, string[] columnNames, object[][] manyRowValues)
        {
            DataTable dt = GetTestTable(dataTableName);
            AddColumns(dt, columnNames);
            AddRows(dt, manyRowValues);
            return dt;
        }

        public static DataTable GetTestTable(string[] columnNames, object[] oneRowValues)
        {
            DataTable dt = GetTestTable();
            AddColumns(dt, columnNames);
            AddRow(dt, oneRowValues);
            return dt;
        }

        public static DataTable GetTestTable(string[] columnNames, object[][] manyRowValues)
        {
            DataTable dt = GetTestTable();
            AddColumns(dt, columnNames);
            AddRows(dt, manyRowValues);
            return dt;
        }

        public static DataTable GetTestTable(string dataTableName, string columnName, string columnValue)
        {
            DataTable dt = GetTestTable(dataTableName);
            dt.Columns.Add(columnName);
            AddRow(dt, columnValue);
            return dt;
        }

        /// <summary>
        /// Creates a dummy table using the specified column names.
        /// The table contains one row with values that are column names.
        /// Can contain duplicate column names. The duplicates will be filtered out.
        /// </summary>
        public static DataTable GetOneRowTestTable(params string[] columnNames)
        {
            DataTable testTable = GetTestTable();
            foreach (string columnName in columnNames)
                AddColumn(testTable, columnName);

            AddTestRow(testTable);
            return testTable;
        }

        public static DataTable AddColumn(DataTable table, string columnName)
        {
            if (!table.Columns.Contains(columnName) && !string.IsNullOrEmpty(columnName))
                table.Columns.Add(columnName);

            return table;
        }

        /// <summary>
        /// Given column names can contain duplicates because some tests pass here just merge field names
        /// that can be highly duplicated. The duplicates will be filtered out.
        /// </summary>
        public static DataTable AddColumns(DataTable table, params string[] columnNames)
        {
            foreach (string columnName in columnNames)
                AddColumn(table, columnName);

            return table;
        }

        [JavaThrows(true)]
        public static DataTable AddRow(DataTable table, params object[] values)
        {
            table.Rows.Add(values);
            return table;
        }

        [JavaThrows(true)]
        public static DataTable AddRows(DataTable table, params object[][] valueRows)
        {
            foreach (object[] row in valueRows)
                AddRow(table, row);

            return table;
        }

        /// <summary>
        /// Adds a row with values that are column names.
        /// </summary>
        [JavaThrows(true)]
        public static DataTable AddTestRow(DataTable table)
        {
            object[] values = new object[table.Columns.Count];
            for (int i = 0; i < values.Length; i++)
                values[i] = table.Columns[i].ColumnName;

            AddRow(table, values);
            return table;
        }

        public static DataTable LoadTableFromCsv(string fileName)
        {
            return LoadTableFromCsvFile(BuildTestFileName(fileName));
        }

        [JavaThrows(true)]
        public static DataTable CreateDbNull()
        {
            // Create a typed data set row with DBNull values.
            DBNullDataSet ds = new DBNullDataSet();
            ds.Customer.Rows.Add(ds.Customer.NewCustomerRow());
            return ds.Customer;
        }

        public static void AddRelation(DataSet ds, DataTable parent, string parentColumnName, DataTable child, string childColumnName)
        {
            ds.Relations.Add(null, parent.Columns[parentColumnName], child.Columns[childColumnName], false);
        }

        /// <summary>
        /// Loads a CSV file into a DataTable according to https://auckland.dynabic.com/wiki/display/org/How+MailMerge+is+Autoported+to+Java.
        /// To automatically build in test file path from relative one it is better to use TestTableUtil.LoadTableFromCsv(string fileName).
        /// </summary>
        private static DataTable LoadTableFromCsvFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string tableName = reader.ReadLine();
                if (StringUtil.Contains(tableName, ",", false))
                    throw new InvalidOperationException("The first line of the file should be a table name and must not contain commas.");

                DataTable table = new DataTable(tableName);

                string header = reader.ReadLine();
                if (!StringUtil.HasChars(header))
                    return table;

                string[] columnNames = header.Split(',');
                AddColumns(table, columnNames);

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!StringUtil.HasChars(line))
                        break;

                    string[] values = line.Split(',');

                    // Perform the required replacements.
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = values[i].Replace("\\r", "\r");
                        values[i] = values[i].Replace("\\n", "\n");

                        if (values[i] == "NULL")
                            values[i] = null;
                    }

                    table.Rows.Add(values);
                    line = reader.ReadLine();
                }

                return table;
            }
        }

        private static string BuildTestFileName(string fileName)
        {
            if (Path.IsPathRooted(fileName))
                return fileName;

            return TestFxUtil.BuildRootTestFileName(fileName);
        }

        private const string DefaultTestTableName = "TestTable";
    }
}
