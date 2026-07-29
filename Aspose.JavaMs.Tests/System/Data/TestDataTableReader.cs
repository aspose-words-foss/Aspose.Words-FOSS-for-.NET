// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2016 by Vyacheslav Durin

using System;
using System.Data;
using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// This class contains methods to test Java's implementation of DataTableReader.
    /// </summary>
    [TestFixture]
    public class TestDataTableReader
    {

        [Test]
        public void TestReadTable()
        {
            DataTable table = CreateDataTable();
            DataTableReader reader;
            using (reader = new DataTableReader(table))
            {
                Assert.That(reader.Depth, Is.EqualTo(0));
                Assert.That(reader.FieldCount, Is.EqualTo(table.Columns.Count));
                Assert.That(reader.HasRows, Is.True);
                Assert.That(reader.IsClosed, Is.False);
                Assert.That(reader.RecordsAffected, Is.EqualTo(0));

                int rowCount = 0;
                while (reader.Read()) ++rowCount;

                Assert.That(rowCount, Is.EqualTo(table.Rows.Count));
            }
            Assert.That(reader.IsClosed, Is.True);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReaderClosed()
        {
            DataTable table = CreateDataTable();
            DataTableReader reader;
            using (reader = new DataTableReader(table))
            {
                Assert.That(reader.Depth, Is.EqualTo(0));
            }
            Assert.That(reader.Depth, Is.EqualTo(0));
        }

        [Test]
        public void TestReadRow()
        {
            int rowPosition = 10;
            DataTable table = CreateDataTable();
            DataRow expectedRow = table.Rows[rowPosition];

            DataTableReader reader;
            using (reader = new DataTableReader(table))
            {
                MoveToPosition(reader, rowPosition);

                // test get(int)
                for (int i = 0; i < reader.FieldCount; i++)
                    Assert.That(reader[i], Is.EqualTo(expectedRow[i]));

                // test get(columnName)
                AssertDataReaderCurrentItem(expectedRow, reader);
            }
            Assert.That(reader.IsClosed, Is.True);
        }

        [Test]
        public void TestCreateDataTableSchema()
        {
            DataTable table = CreateDataTable();
            DataTable expectedSchemaTable = CreateExpectedSchemaDataTable(table);
            DataTableReader reader;
            using (reader = new DataTableReader(table))
            {
                AssertTables(expectedSchemaTable, reader.GetSchemaTable());
            }
            Assert.That(reader.IsClosed, Is.True);
        }

        [Test]
        public void TestReadDataSet()
        {
            DataSet dataSet = CreateDataSet();
            DataTable[] tables = new DataTable[dataSet.Tables.Count];
            for (int i = 0; i < tables.Length; i++)
                tables[i] = dataSet.Tables[i];

            using (DataTableReader reader = new DataTableReader(tables))
            {
                AssertTables(CreateExpectedSchemaDataTable(tables[0]), reader.GetSchemaTable());
                reader.NextResult();
                AssertTables(CreateExpectedSchemaDataTable(tables[1]), reader.GetSchemaTable());
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReaderReflectsColumnChanges()
        {
            DataTable table = CreateDataTable();
            int columnCount = table.Columns.Count;
            using (DataTableReader reader = new DataTableReader(table))
            {
                Assert.That(reader.FieldCount, Is.EqualTo(table.Columns.Count));
                reader.Read();
                table.Columns.Add("new column");
                Assert.That(table.Columns.Count, Is.EqualTo(columnCount + 1));

                //System.InvalidOperationException : Schema of current DataTable 'Risk' in DataTableReader has changed, DataTableReader is invalid.
                reader.Read();
                Assert.That(reader.FieldCount, Is.EqualTo(table.Columns.Count));
            }
        }

        [Test]
        public void TestAddRows()
        {
            DataTable table = CreateDataTable();
            int rowsCount = table.Rows.Count;
            Assert.That(table.Columns.Count, Is.EqualTo(8));

            using (DataTableReader reader = new DataTableReader(table))
            {
                Assert.That(reader.FieldCount, Is.EqualTo(table.Columns.Count));

                // take 3rd row
                MoveToPosition(reader, 3);
                AssertDataReaderCurrentItem(table.Rows[3], reader);

                // insert before the reader's current row which is 3rd
                table.Rows.InsertAt(CreateNewRiskRow(table.NewRow(), 21), 1);
                Assert.That(table.Rows.Count, Is.EqualTo(++rowsCount));

                // the reader's current row becomes 4th
                AssertDataReaderCurrentItem(table.Rows[4], reader);

                // insert at the position of the reader's current row which is 4th
                table.Rows.InsertAt(CreateNewRiskRow(table.NewRow(), 22), 4);
                Assert.That(table.Rows.Count, Is.EqualTo(++rowsCount));

                // the reader's current row becomes 5th
                AssertDataReaderCurrentItem(table.Rows[5], reader);

                // add after the reader's current row which is 5th now
                table.Rows.Add(CreateNewRiskRow(table.NewRow(), 23));
                Assert.That(table.Rows.Count, Is.EqualTo(++rowsCount));

                // nothing is changed
                AssertDataReaderCurrentItem(table.Rows[5], reader);
            }
        }

        [Test]
        public void TestDeleteRows()
        {
            DataTable table = CreateDataTable();
            int rowsCount = table.Rows.Count;
            Assert.That(table.Columns.Count, Is.EqualTo(8));

            using (DataTableReader reader = new DataTableReader(table))
            {
                Assert.That(reader.FieldCount, Is.EqualTo(table.Columns.Count));
                // take 3rd row
                MoveToPosition(reader, 3);
                AssertDataReaderCurrentItem(table.Rows[3], reader);

                // delete before the reader's current row
                table.Rows.RemoveAt(1);
                Assert.That(table.Rows.Count, Is.EqualTo(--rowsCount));
                AssertDataReaderCurrentItem(table.Rows[2], reader);

                // delete after the reader's current row
                table.Rows.RemoveAt(table.Rows.Count - 1);
                Assert.That(table.Rows.Count, Is.EqualTo(--rowsCount));
                AssertDataReaderCurrentItem(table.Rows[2], reader);

                // delete the reader's current row
                table.Rows.RemoveAt(2);
                Assert.That(table.Rows.Count, Is.EqualTo(--rowsCount));
                AssertDataReaderCurrentItem(table.Rows[1], reader); // move to previous 

                // delete the reader's current row
                table.Rows.RemoveAt(1);
                Assert.That(table.Rows.Count, Is.EqualTo(--rowsCount));
                AssertDataReaderCurrentItem(table.Rows[0], reader); // move to previous 

                // delete the reader's current row
                table.Rows.RemoveAt(0);
                Assert.That(table.Rows.Count, Is.EqualTo(--rowsCount));
                try
                {
                    AssertDataReaderCurrentItem(table.Rows[0], reader); // no previous 
                    Assert.Fail("Should have fail because the reader's current row is not set anymore.");
                }
                catch (InvalidOperationException)
                {
                    // intentionally left blank because it must happen
                }
            }
        }

        private static DataSet CreateDataSet()
        {
            DataSet dataSet = new DataSet("Generic Data Set");

            DataTable table1 = new DataTable("Table1");
            table1.Columns.Add(new DataColumn("ProductName"));
            table1.Columns.Add(new DataColumn("ProductPrice", typeof(float)));
            table1.Rows.Add("Shoes", 120.5f);
            table1.Rows.Add("Trousers", 190.51f);

            DataTable table2 = new DataTable("Table2");
            table2.Columns.Add(new DataColumn("Id", typeof(Int64)));
            table2.Columns.Add(new DataColumn("Store"));
            table2.Rows.Add(1, "North");
            table2.Rows.Add(2, "South");

            dataSet.Tables.Add(table1);
            dataSet.Tables.Add(table2);

            return dataSet;
        }

        private static DataTable CreateDataTable()
        {
            const string file = @"MailMerge\TestJira7133Data.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(TestDataSet.GetInTestDataPath(file));
            return dataSet.Tables["Risk"];
        }

        private static DataTable CreateExpectedSchemaDataTable(DataTable dataTable)
        {
            DataTable schema = new DataTable("SchemaTable");
            schema.Columns.Add("ColumnName");
            schema.Columns.Add("ColumnOrdinal", typeof(Int32));
            schema.Columns.Add("ColumnSize", typeof(Int32));
            schema.Columns.Add("NumericPrecision", typeof(Int16));
            schema.Columns.Add("NumericScale", typeof(Int16));
            schema.Columns.Add("DataType", typeof(Type));
            schema.Columns.Add("ProviderType", typeof(Int32));
            schema.Columns.Add("IsLong", typeof(bool));
            schema.Columns.Add("AllowDBNull", typeof(bool));
            schema.Columns.Add("IsReadOnly", typeof(bool));
            schema.Columns.Add("IsRowVersion", typeof(bool));
            schema.Columns.Add("IsUnique", typeof(bool));
            schema.Columns.Add("IsKey", typeof(bool));
            schema.Columns.Add("IsAutoIncrement", typeof(bool));
            schema.Columns.Add("BaseCatalogName");
            schema.Columns.Add("BaseSchemaName");
            schema.Columns.Add("BaseTableName");
            schema.Columns.Add("BaseColumnName");
            schema.Columns.Add("AutoIncrementSeed", typeof(Int64));
            schema.Columns.Add("AutoIncrementStep", typeof(Int64));
            schema.Columns.Add("DefaultValue", typeof(object));
            schema.Columns.Add("Expression");
            schema.Columns.Add("ColumnMapping", typeof(MappingType));
            schema.Columns.Add("BaseTableNamespace");
            schema.Columns.Add("BaseColumnNamespace");

            StringToObjDictionary<DataColumn> keyMap = new StringToObjDictionary<DataColumn>();
            DataColumn[] key = dataTable.PrimaryKey;
            if (key != null)
            {
                foreach (DataColumn keyColumn in key)
                    keyMap.Add(keyColumn.ColumnName, keyColumn);
            }

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn column = dataTable.Columns[i];

                schema.Rows.Add(column.ColumnName, i, -1, null, null, column.DataType,
                    null, false, column.AllowDBNull, column.ReadOnly, false,
                    column.Unique, keyMap.ContainsKey(column.ColumnName),
                    column.AutoIncrement, column.Table.DataSet != null ? column.Table.DataSet.DataSetName : null,
                    null, column.Table.TableName, column.ColumnName, column.AutoIncrementSeed,
                    column.AutoIncrementStep, column.DefaultValue,
                    string.IsNullOrEmpty(column.Expression) ? DBNull.Value : (object)column.Expression,
                    column.ColumnMapping,
                    column.Table.Namespace, column.Namespace);
            }
            return schema;
        }

        private void AssertTables(DataTable expected, DataTable actual)
        {
            Assert.That(expected, IsNot.Null());
            Assert.That(actual, IsNot.Null());
            Assert.That(actual.TableName, Is.EqualTo(expected.TableName));
            Assert.That(actual.Columns.Count, Is.EqualTo(expected.Columns.Count));
            Assert.That(actual.Rows.Count, Is.EqualTo(expected.Rows.Count));

            for (int i = 0; i < expected.Columns.Count; i++)
            {
                DataColumn expectedColumn = expected.Columns[i];
                DataColumn actualColumn = actual.Columns[i];
                Assert.That(actualColumn.ColumnName, Is.EqualTo(expectedColumn.ColumnName));
                Assert.That(actualColumn.DataType, Is.EqualTo(expectedColumn.DataType));
            }
            for (int i = 0; i < expected.Rows.Count; i++)
            {
                DataRow expectedRow = expected.Rows[i];
                DataRow actualRow = actual.Rows[i];
                for (int j = 0; j < expected.Columns.Count; j++)
                {
                    DataColumn expectedColumn = expected.Columns[j];
                    int expectedPosition = expected.Columns.IndexOf(expectedColumn);
                    object expectedValue = expectedRow[expectedPosition];

                    int actualPosition = actual.Columns.IndexOf(expectedColumn.ColumnName);
                    object actualValue = actualRow[actualPosition];
                    Assert.That(actualValue, Is.EqualTo(expectedValue), string.Format("Columns '{0}' have different values", expectedColumn.ColumnName));
                }
            }
        }

        private static void MoveToPosition(DataTableReader reader, int rowPosition)
        {
            int cursor = 0;
            reader.Read();
            while (cursor++ < rowPosition)
                reader.Read();
        }

        private static DataRow CreateNewRiskRow(DataRow emptyRow, int id)
        {
            emptyRow["active"] = "1";
            emptyRow["custom"] = "0";
            emptyRow["dateCreatedLocally"] = "17-08-2012";
            emptyRow["notes"] = null;
            emptyRow["riskName"] = "Welfare requirements_" + id;
            emptyRow["riskSubtitle"] = "location off site for consumption of food & breaks_" + id;
            emptyRow["ratingAsString"] = "Low";
            emptyRow["Risk_Id"] = id;
            return emptyRow;
        }

        private static void AssertDataReaderCurrentItem(DataRow expectedRow, DataTableReader reader)
        {
            Assert.That(expectedRow, IsNot.Null());
            Assert.That(reader, IsNot.Null());
            Assert.That(reader.IsClosed, Is.False);
            Assert.That(reader.HasRows, Is.True);

            DataTable table = expectedRow.Table;
            Assert.That(table, IsNot.Null());
            Assert.That(reader.FieldCount, Is.EqualTo(table.Columns.Count));

            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn column = table.Columns[i];
                Assert.That(reader[column.ColumnName], Is.EqualTo(expectedRow[column.ColumnName]));
            }
        }
    }
}
