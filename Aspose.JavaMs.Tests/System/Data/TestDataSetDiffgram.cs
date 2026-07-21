// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Anatoly Sidorenko

using System.Data;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// The main purpose of this class is to provide data for Java's tests.
    /// This class will be moved into new project Aspose.Ms.Tests when it gets created.
    /// https://technet.microsoft.com/en-us/library/aa257368(v=sql.80).aspx
    /// </summary>
    [TestFixture]
    public class TestDataSetDiffgram
    {
        [Test, Ignore("TestCreateDataSetSchema")]
        [JavaThrows(true)]
        public void TestCreateDataSetSchema()
        {
            // create ds
            DataSet ds = CreateDataSet();
#if !JAVA
            // write schema
            ds.WriteXmlSchema(TestDataSet.GetInTestDataPath(@"Data\diff\DiffGramSchema.xsd"));
#endif
        }





        private DataSet CreateDataSetFromSchemaFile()
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(@"Data\diff\DiffGramSchema.xsd"));

            PopulateDataSet(dataSet);

            return dataSet;
        }

        [JavaThrows(true)]
        public static void PopulateDataSet(DataSet dataSet)
        {
            DataTable customerTable = dataSet.Tables["Customer"];
            customerTable.Rows.Add("ALFKI", "Alfreds Futterkiste", "Maria Anders");
            customerTable.Rows.Add("ANATR", "Ana Trujillo Emparedados y helados", "Ana Trujillo");
            customerTable.Rows.Add("ANTON", "Antonio Moreno Taquería", "Antonio Moreno");

            DataTable ordersTable = dataSet.Tables["Order"];
            ordersTable.Rows.Add(1, "ALFKI");
            ordersTable.Rows.Add(2, "ANATR");
            ordersTable.Rows.Add(3, "ANTON");
        }

        [JavaThrows(true)]
        public static DataSet CreateDataSet()
        {
            DataSet ds = new DataSet("Customers_Data");

            // add customer table
            DataTable customerTable = new DataTable("Customer");

            DataColumn customerIdCol = new DataColumn("CustomerID", typeof(string));
            customerIdCol.AllowDBNull = false;

            customerTable.Columns.Add(customerIdCol);

            DataColumn companyNameCol = new DataColumn("CompanyName", typeof(string));
            customerTable.Columns.Add(companyNameCol);

            customerTable.Columns.Add(new DataColumn("ContactName", typeof(string)));
            customerTable.PrimaryKey = new DataColumn[] { customerIdCol };

            ds.Tables.Add(customerTable);

            // add orders table 
            DataTable orderTable = new DataTable("Order");

            DataColumn orderIdCol = new DataColumn("OrderID", typeof(int));
            orderIdCol.AllowDBNull = false;
            orderTable.Columns.Add(orderIdCol);
            orderTable.PrimaryKey = new DataColumn[] { orderIdCol };

            DataColumn customerOrderIdCol = new DataColumn("CustomerID", typeof(string));
            orderTable.Columns.Add(customerOrderIdCol);

            ds.Tables.Add(orderTable);

            // add relations and FK  DeleteRule == None by default
            DataRelation dataRelation = new DataRelation("Customer_Order", customerIdCol, customerOrderIdCol);

            ds.Relations.Add(dataRelation);

            return ds;
        }
    }
}
