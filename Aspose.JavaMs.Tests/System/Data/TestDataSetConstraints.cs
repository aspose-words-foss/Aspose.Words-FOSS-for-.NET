// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Anatoly Sidorenko

using System;
using System.Data;
using System.Diagnostics;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// The main purpose of this class is to provide data for Java's tests.
    /// </summary>
    [TestFixture]
    public class TestDataSetConstraints
    {
        [Test]
        public void TestConstraintExists()
        {
            DataSet ds = GetDataSet();

            // Customer
            // CustomerID can't be null
            // CustomerID is unique and it is PrimaryKey

            //Order
            // OrderID can't be null
            // OrderID is unique and is PrimaryKey
            // CustomerID is foreign Key

            DataTable customerTable = ds.Tables["Customer"];
            DataTable orderTable = ds.Tables["Order"];

            Assert.That(1, Is.EqualTo(customerTable.Constraints.Count));
            Assert.That(2, Is.EqualTo(orderTable.Constraints.Count));

            // Customer
            CheckUniqueConstraint((UniqueConstraint)customerTable.Constraints[0],
                "Constraint1", true, "Customer", new DataColumn[] { new DataColumn("CustomerID"), });

            // Order
            CheckUniqueConstraint((UniqueConstraint)orderTable.Constraints[0],
                "Constraint1", true, "Order", new DataColumn[] { new DataColumn("OrderID"), });

            CheckForeignKeyConstraint((ForeignKeyConstraint)orderTable.Constraints[1], "Customer_Order",
                "Customer", "Order",
                new DataColumn[] { new DataColumn("CustomerID") }, new DataColumn[] { new DataColumn("CustomerID"), },
                Rule.Cascade, Rule.Cascade); // default rules
        }

        [Test, ExpectedException(typeof(ConstraintException))]
        public void TestUniqueConstraintViolated()
        {
            DataSet ds = GetDataSet();
            DataTable customerTable = ds.Tables["Customer"];
            customerTable.Rows.Add("ALFKI", "Duplicate Customer ID", "Duplicate Customer ID");
            // System.Data.ConstraintException : Column 'CustomerID' is constrained to be unique.  Value 'ALFKI' is already present.
        }

        [Test, ExpectedException(typeof(InvalidConstraintException))]
        public void TestForeignKeyConstraintViolated()
        {
            DataSet ds = GetDataSet();
            DataTable orderTable = ds.Tables["Order"];

            orderTable.Rows.Add(4, "ALFKI NOT EXIST");
            //InvalidConstraintException : ForeignKeyConstraint Customer_Order requires the child key values (ALFKI NOT EXIST) to exist in the parent table.
        }

        [Test]
        public void TestDeleteRuleCascade()
        {
            DataSet ds = GetDataSet();

            ds.Tables["Customer"].Rows[2].Delete();

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();
            DataTable customerTable = new DataTable("Customer");
            expectedTables.AddTable(customerTable, 2);
            customerTable.Columns.Add("CustomerID");
            customerTable.Columns.Add("CompanyName");
            customerTable.Columns.Add("ContactName");
            customerTable.Rows.Add("ALFKI", "Alfreds Futterkiste", "Maria Anders");
            customerTable.Rows.Add("ANATR", "Ana Trujillo Emparedados y helados", "Ana Trujillo");

            DataTable orderTable = new DataTable("Order");
            expectedTables.AddTable(orderTable, 2);
            orderTable.Columns.Add("OrderID", typeof(int));
            orderTable.Columns.Add("CustomerID");
            orderTable.Rows.Add("1", "ALFKI");
            orderTable.Rows.Add("2", "ANATR");

            expectedTables.AddRelation(customerTable, orderTable, new string[] { "CustomerID" }, new string[] { "CustomerID" });

            TestDataSet.AssertDataSet(ds, expectedTables);
        }

        [Test]
        public void TestDeleteRuleCascadeFromChildTable()
        {
            DataSet ds = GetDataSet();

            ds.Tables["Order"].Rows[1].Delete();

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();
            DataTable customerTable = new DataTable("Customer");
            expectedTables.AddTable(customerTable, 3);
            customerTable.Columns.Add("CustomerID");
            customerTable.Columns.Add("CompanyName");
            customerTable.Columns.Add("ContactName");
            customerTable.Rows.Add("ALFKI", "Alfreds Futterkiste", "Maria Anders");
            customerTable.Rows.Add("ANATR", "Ana Trujillo Emparedados y helados", "Ana Trujillo");
            customerTable.Rows.Add("ANTON", "Antonio Moreno Taquería", "Antonio Moreno");

            DataTable orderTable = new DataTable("Order");
            expectedTables.AddTable(orderTable, 2);
            orderTable.Columns.Add("OrderID", typeof(int));
            orderTable.Columns.Add("CustomerID");
            orderTable.Rows.Add("1", "ALFKI");
            orderTable.Rows.Add("3", "ANTON");

            expectedTables.AddRelation(customerTable, orderTable, new string[] { "CustomerID" }, new string[] { "CustomerID" });

            TestDataSet.AssertDataSet(ds, expectedTables);
        }

        [Test]
        [AndroidDelete("Performance issues")]
        public void TestFindRow()
        {
            DataSet ds = GetDataSet();

            DataTable dataTable = ds.Tables["Customer"];
            DataRowCollection dataRowCollection = dataTable.Rows;
            for (int i = 0; i < 10000; i++)
                dataRowCollection.Add("CUSTOMER" + i, "Name" + i, "Contact" + i); // adding is too slow in Java version

            Stopwatch sw = Stopwatch.StartNew();
            DataRow found = dataTable.Rows.Find("CUSTOMER999");
            sw.Stop();

            Debug.WriteLine(string.Format("first elapsed time: {0}", sw.ElapsedMilliseconds));
            Assert.That(found[0], Is.EqualTo("CUSTOMER999"));
            Assert.That(found[1], Is.EqualTo("Name999"));
            Assert.That(found[2], Is.EqualTo("Contact999"));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestFindRowBySeveralParamsWithOnePkColumn()
        {
            DataSet ds = GetDataSet();
            DataTable dataTable = ds.Tables["Customer"];
            dataTable.Rows.Find(new object[] { "CUSTOMER900", "Name900", "Contact900" });
        }

        [Test]
        [JavaThrows(true)]
        public void TestFindRowBySeveralParams()
        {
            DataSet ds = new DataSet();

            DataTable table = new DataTable("Students");
            table.Columns.Add("First Name");
            table.Columns.Add("Last Name");
            table.Columns.Add("DOB");
            table.PrimaryKey = new DataColumn[] { table.Columns["First Name"], table.Columns["Last Name"], table.Columns["DOB"], };
            ds.Tables.Add(table);

            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
                table.Rows.Add("Student" + i, "LastName" + i, rnd.Next(1, 30) + "/" + rnd.Next(1, 12) + "/" + rnd.Next(1990, 2000));

            DataRow rowToBeFound = table.NewRow();
            rowToBeFound[0] = "Student1000";
            rowToBeFound[1] = "LastName1000";
            rowToBeFound[2] = "04/07/1986";
            table.Rows.InsertAt(rowToBeFound, 789);

            Stopwatch sw = Stopwatch.StartNew();
            DataRow found = table.Rows.Find(new object[] { "Student1000", "LastName1000", "04/07/1986" });
            sw.Stop();

            Debug.WriteLine(string.Format("first elapsed time: {0}", sw.ElapsedMilliseconds));
            Assert.That(found[0], Is.EqualTo("Student1000"));
            Assert.That(found[1], Is.EqualTo("LastName1000"));
            Assert.That(found[2], Is.EqualTo("04/07/1986"));
        }

        private void CheckUniqueConstraint(UniqueConstraint constraint, string cName, bool isPk, string dtName, DataColumn[] cols)
        {
            Assert.That(constraint.ConstraintName, Is.EqualTo(cName));
            Assert.That(constraint.IsPrimaryKey, Is.EqualTo(isPk));
            Assert.That(constraint.Table.TableName, Is.EqualTo(dtName));
            Assert.That(constraint.Columns.Length, Is.EqualTo(cols.Length));

            for (int i = 0; i < cols.Length; i++)
                Assert.That(constraint.Columns[i].ColumnName, Is.EqualTo(cols[i].ColumnName));
        }

        private void CheckForeignKeyConstraint(ForeignKeyConstraint constraint, string constraintName,
                        string parentTableName, string childTableName,
                        DataColumn[] parentCols, DataColumn[] childCols,
                        Rule expUpdateRule, Rule expDeleteRule)
        {

            Assert.That(constraint.ConstraintName, Is.EqualTo(constraintName));
            Assert.That(constraint.RelatedTable.TableName, Is.EqualTo(parentTableName));
            Assert.That(constraint.Table.TableName, Is.EqualTo(childTableName));

            Assert.That(constraint.UpdateRule, Is.EqualTo(expUpdateRule));
            Assert.That(constraint.DeleteRule, Is.EqualTo(expDeleteRule));

            // parent columns
            Assert.That(constraint.RelatedColumns.Length, Is.EqualTo(parentCols.Length));
            for (int i = 0; i < parentCols.Length; i++)
                Assert.That(constraint.RelatedColumns[i].ColumnName, Is.EqualTo(parentCols[i].ColumnName));

            // child columns
            Assert.That(constraint.Columns.Length, Is.EqualTo(childCols.Length));
            for (int i = 0; i < childCols.Length; i++)
                Assert.That(constraint.Columns[i].ColumnName, Is.EqualTo(childCols[i].ColumnName));
        }

        private static DataSet GetDataSet()
        {
            DataSet ds = TestDataSetDiffgram.CreateDataSet();
            TestDataSetDiffgram.PopulateDataSet(ds);

            // Customer
            // CustomerID can't be null
            // CustomerID is unique and it is PrimaryKey

            //Order
            // OrderID can't be null
            // OrderID is unique and is PrimaryKey
            // CustomerID is foreign Key

            return ds;
        }
    }
}
