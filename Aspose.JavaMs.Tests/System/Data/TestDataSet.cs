// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Anatoly Sidorenko

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// The main purpose of this class is to provide data for Java's tests.
    /// This class will be moved into new project Aspose.Ms.Tests when it gets created.
    /// </summary>
    [TestFixture]
    public class TestDataSet
    {

        [Test]
        public void TestJava2194()
        {
            string xml = "<_5f10ae8a2fbe413d997289e1fac04c7d>" +
                        "<name>John</name>" +
                        "<age>30</age>" +
                        "<cars>Ford</cars>" +
                        "<cars>BMW</cars>" +
                        "<cars>Fiat</cars>" +
                    "</_5f10ae8a2fbe413d997289e1fac04c7d>";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            DataSet ds = new DataSet();
            ds.ReadXml(stream);
            DataSetWriter.PrintDataSet(ds);

            Assert.That(ds.Tables.Count, Is.EqualTo(2));
            Assert.That(ds.Tables[0].Columns.Count, Is.EqualTo(3));
            Assert.That(ds.Tables[1].Columns.Count, Is.EqualTo(2));

            Assert.That(ds.Tables[0].Rows.Count, Is.EqualTo(1));
            Assert.That(ds.Tables[1].Rows.Count, Is.EqualTo(3));

            Assert.That(ds.Relations.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestJava2178()
        {
            string XML =
               "<?xml version=\"1.0\" ?>" +
                       "<Persons>" +
                       "<Person>John Doe</Person>" +
                       "<Person>Jane Doe</Person>" +
                       "</Persons>";

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XML));
            DataSet ds = new DataSet();
            ds.ReadXml(stream);

            DataSetWriter.PrintDataSet(ds);

            Assert.That(ds.Tables.Count, Is.EqualTo(1));
            Assert.That(ds.Tables[0].Columns.Count, Is.EqualTo(1));
            Assert.That(ds.Tables[0].Columns[0].ColumnName, Is.EqualTo("Person_Text"));
            Assert.That(ds.Tables[0].Rows.Count, Is.EqualTo(2));
            Assert.That(ds.Tables[0].Rows[0].ItemArray[0], Is.EqualTo("John Doe"));
            Assert.That(ds.Tables[0].Rows[1].ItemArray[0], Is.EqualTo("Jane Doe"));
        }

        /// <summary>
        /// WORDSJAVA-1521 Mergefields outside the region are ignored.
        /// </summary>
        [Test]
        public void TestJira1521()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                         "<root>\n" +
                         "  <titolo>Elenco Lavoratori</titolo>\n" +
                         "  <test>\n" +
                         "    <ragione_sociale_sede>SISTEMA DI GESTIONE DIMOSTRATIVO</ragione_sociale_sede>\n" +
                         "  </test>\n" +
                         "</root>";

            DataSet ds = new DataSet();
            ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            // Table 1
            DataTable table = ds.Tables[0];
            Assert.That(table.TableName, Is.EqualTo("root"));
            Assert.That(table.Columns[0].Caption, Is.EqualTo("titolo"));
            Assert.That(table.Columns[1].Caption, Is.EqualTo("root_Id"));

            Assert.That(table.Rows[0].ItemArray[0].ToString(), Is.EqualTo("Elenco Lavoratori"));
            Assert.That(table.Rows[0].ItemArray[1].ToString(), Is.EqualTo("0"));

            // Table 2
            table = ds.Tables[1];
            Assert.That(table.TableName, Is.EqualTo("test"));
            Assert.That(table.Columns[0].Caption, Is.EqualTo("ragione_sociale_sede"));
            Assert.That(table.Columns[1].Caption, Is.EqualTo("root_Id"));

            Assert.That(table.Rows[0].ItemArray[0].ToString(), Is.EqualTo("SISTEMA DI GESTIONE DIMOSTRATIVO"));
            Assert.That(table.Rows[0].ItemArray[1].ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void TestColumnIndex()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                         "<root>\n" +
                         "  <titolo1>Elenco Lavoratori 1</titolo1>\n" +
                         "  <titolo2>Elenco Lavoratori 2</titolo2>\n" +
                         "  <test>\n" +
                         "    <ragione_sociale_sede1>SISTEMA DI GESTIONE DIMOSTRATIVO 1</ragione_sociale_sede1>\n" +
                         "    <ragione_sociale_sede2>SISTEMA DI GESTIONE DIMOSTRATIVO 2</ragione_sociale_sede2>\n" +
                         "  </test>\n" +
                         "</root>";

            DataSet ds = new DataSet();
            ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            // Table 1
            DataTable table = ds.Tables[0];
            Assert.That(table.TableName, Is.EqualTo("root"));
            Assert.That(table.Columns[0].Caption, Is.EqualTo("titolo1"));
            Assert.That(table.Columns[1].Caption, Is.EqualTo("titolo2"));
            Assert.That(table.Columns[2].Caption, Is.EqualTo("root_Id"));

            Assert.That(table.Rows[0].ItemArray[0].ToString(), Is.EqualTo("Elenco Lavoratori 1"));
            Assert.That(table.Rows[0].ItemArray[1].ToString(), Is.EqualTo("Elenco Lavoratori 2"));
            Assert.That(table.Rows[0].ItemArray[2].ToString(), Is.EqualTo("0"));

            // Table 2
            table = ds.Tables[1];
            Assert.That(table.TableName, Is.EqualTo("test"));
            Assert.That(table.Columns[0].Caption, Is.EqualTo("ragione_sociale_sede1"));
            Assert.That(table.Columns[1].Caption, Is.EqualTo("ragione_sociale_sede2"));
            Assert.That(table.Columns[2].Caption, Is.EqualTo("root_Id"));

            Assert.That(table.Rows[0].ItemArray[0].ToString(), Is.EqualTo("SISTEMA DI GESTIONE DIMOSTRATIVO 1"));
            Assert.That(table.Rows[0].ItemArray[1].ToString(), Is.EqualTo("SISTEMA DI GESTIONE DIMOSTRATIVO 2"));
            Assert.That(table.Rows[0].ItemArray[2].ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void TestXmlWithNoSchema()
        {
            const string file = @"MailMerge\TestJira7133Data.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();

            // tables
            DataTable riskTable = new DataTable("Risk");
            riskTable.Columns.Add("active");
            riskTable.Columns.Add("custom");
            riskTable.Columns.Add("dateCreatedLocally");
            riskTable.Columns.Add("notes");
            riskTable.Columns.Add("riskName");
            riskTable.Columns.Add("riskSubtitle");
            riskTable.Columns.Add("ratingAsString");
            riskTable.Columns.Add("Risk_Id", typeof(int));

            riskTable.Rows.Add("1", "0", "17-08-2012", null, "Asbestos", null, "High", "0");
            expectedTables.AddTable(riskTable, 21);

            DataTable refWorseCaseOutcome = new DataTable("RefWorseCaseOutcome");
            refWorseCaseOutcome.Columns.Add("dateUpdatedRemotely");
            refWorseCaseOutcome.Columns.Add("displayTitle");
            refWorseCaseOutcome.Columns.Add("isDisplayed");
            refWorseCaseOutcome.Columns.Add("referenceId");
            refWorseCaseOutcome.Columns.Add("sortOrder");
            refWorseCaseOutcome.Columns.Add("value");
            refWorseCaseOutcome.Columns.Add("Risk_Id", typeof(int));

            refWorseCaseOutcome.Rows.Add(null, "10 Fatality", "1", "0", "0", "10", "0");
            expectedTables.AddTable(refWorseCaseOutcome, 21);

            DataTable refLikelihood = new DataTable("RefLikelihood");
            refLikelihood.Columns.Add("dateUpdatedRemotely");
            refLikelihood.Columns.Add("displayTitle");
            refLikelihood.Columns.Add("isDisplayed");
            refLikelihood.Columns.Add("referenceId");
            refLikelihood.Columns.Add("sortOrder");
            refLikelihood.Columns.Add("value");
            refLikelihood.Columns.Add("Risk_Id", typeof(int));

            refLikelihood.Rows.Add(null, "10 Certain / Imminent Death", "1", "0", "0", "10", "0");
            expectedTables.AddTable(refLikelihood, 21);

            expectedTables.AddRelation(riskTable, refLikelihood, new string[] { "Risk_Id" }, new string[] { "Risk_Id" });
            expectedTables.AddRelation(riskTable, refWorseCaseOutcome, new string[] { "Risk_Id" }, new string[] { "Risk_Id" });

            AssertDataSet(dataSet, expectedTables);
        }



        [Test]
        public void TestXml7068()
        {
            const string file = @"MailMerge\TestJira7068.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();
            DataTable item = new DataTable("Item");
            expectedTables.AddTable(item, 2);
            item.Columns.Add("Caption");
            item.Columns.Add("Data");
            item.Rows.Add("Caption1", "Data portion 1");
            item.Rows.Add("Caption2", "Data portion 2");

            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestXml7068LoadDataIntoAlreadyCreatedStructure()
        {
            const string file = "MailMerge\\TestJira7068.xml";
            DataSet dataSet = new DataSet();

            // create original structure
            DataTable origItemTable = new DataTable("Item");
            origItemTable.Columns.Add("Caption");
            origItemTable.Columns.Add("Data");
            dataSet.Tables.Add(origItemTable);

            // load data into already exist DataSet structure
            dataSet.ReadXml(GetInTestDataPath(file), XmlReadMode.IgnoreSchema);

            // compare
            AssertTableInfo expectedTables = new AssertTableInfo();
            DataTable item = new DataTable("Item");
            expectedTables.AddTable(item, 2);
            item.Columns.Add("Caption");
            item.Columns.Add("Data");
            item.Rows.Add("Caption1", "Data portion 1");
            item.Rows.Add("Caption2", "Data portion 2");

            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXml1()
        {
            const string file = @"Data\region.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();
            DataTable item = new DataTable("Region");
            expectedTables.AddTable(item, 2);
            item.Columns.Add("RegionID");
            item.Columns.Add("RegionDescription");
            item.Rows.Add("1", "Eastern\n   ");
            item.Rows.Add("2", "Western\n   ");

            DataSetWriter.PrintDataSet(dataSet);
            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXml2()
        {
            const string file = @"Data\TestMerge1.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable item = new DataTable("Main");
            expectedTables.AddTable(item, 3);
            item.Columns.Add("ID", typeof(int));
            item.Columns.Add("Data");
            item.Rows.Add("1", "One");
            item.Rows.Add("2", "Two");
            item.Rows.Add("3", "Three");

            DataTable item2 = new DataTable("Child");
            expectedTables.AddTable(item2, 3);
            item2.Columns.Add("ID", typeof(int));
            item2.Columns.Add("PID", typeof(int));
            item2.Columns.Add("ChildData");
            item2.Rows.Add("1", "1", "Parent1Child1");
            item2.Rows.Add("2", "1", "Parent1Child2");
            item2.Rows.Add("3", "2", "Parent2Child3");

            expectedTables.AddRelation("MainToChild", item, item2, new string[] { "ID" }, new string[] { "PID" });

            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXml3()
        {
            const string file = @"Data\TestReadXml1.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable item = new DataTable("DESC");
            expectedTables.AddTable(item, 11);
            item.Columns.Add("ColumnName");
            item.Columns.Add("DataType");
            item.Columns.Add("IsAutoIncrement");
            item.Columns.Add("AllowDBNull");
            item.Columns.Add("IsReadOnly");
            item.Columns.Add("IsKey");
            item.Columns.Add("IsUnique");
            item.Columns.Add("ColumnSize");

            item.Rows.Add("ColumnName", "System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("DataType", "System.Type, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("IsAutoIncrement", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("AllowDBNull", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("IsReadOnly", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("IsKey", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("IsUnique", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("ColumnSize", "System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("ColumnNumber", "System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("Summary", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");
            item.Rows.Add("Print", "System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "false", "true", "true", "false", "false", "0");

            AssertDataSet(dataSet, expectedTables);
        }

        #region check DataColumns types

        /// <summary>
        /// TODO: add check of types of columns
        /// </summary>
        [Test]
        public void TestReadXmlTypedDataSet()
        {
            const string file = @"Data\TypedDataSet.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable item = new DataTable("Order Details");
            expectedTables.AddTable(item, 4);
            item.Columns.Add("OrderID");
            item.Columns.Add("ProductID");
            item.Columns.Add("UnitPrice");
            item.Columns.Add("Quantity");
            item.Columns.Add("Discount");

            item.Rows.Add("10250", "41", "7.7000", "10", "5.0");
            item.Rows.Add("10250", "51", "42.4000", "35", "5.0");
            item.Rows.Add("10250", "65", "16.8000", "15", "5.0");
            item.Rows.Add("10251", "22", "16.8000", "6", "5.0");

            DataTable item2 = new DataTable("Orders");
            expectedTables.AddTable(item2, 3);
            item2.Columns.Add("OrderID");
            item2.Columns.Add("CustomerID");
            item2.Columns.Add("EmployeeID");
            item2.Columns.Add("OrderDate");
            item2.Columns.Add("RequiredDate");
            item2.Columns.Add("ShippedDate");

            item2.Rows.Add("10250", "HANAR", "4", "1996-07-08T00:00:00.0000000+03:00", "1996-08-05T00:00:00.0000000+03:00", "1996-07-12T00:00:00.0000000+03:00");
            item2.Rows.Add("10251", "VICTE", "3", "1996-07-08T00:00:00.0000000+03:00", "1996-08-05T00:00:00.0000000+03:00", "1996-07-15T00:00:00.0000000+03:00");
            item2.Rows.Add("10252", "SUPRD", "4", "1996-07-09T00:00:00.0000000+03:00", "1996-08-06T00:00:00.0000000+03:00", "1996-07-11T00:00:00.0000000+03:00");

            AssertDataSet(dataSet, expectedTables);
        }

        #endregion

        [Test]
        public void TestReadXmlSchema1()
        {
            const string file = @"Data\TestReadXmlSchema1.xml";
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(file));

            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable item = new DataTable("DESC");
            expectedTables.AddTable(item, 0);
            item.Columns.Add("ColumnName");
            item.Columns.Add("DataType", typeof(Type));
            item.Columns.Add("IsAutoIncrement", typeof(bool));
            item.Columns.Add("AllowDBNull", typeof(bool));
            item.Columns.Add("IsReadOnly", typeof(bool));
            item.Columns.Add("IsKey", typeof(bool));
            item.Columns.Add("IsUnique", typeof(bool));
            item.Columns.Add("ColumnSize", typeof(int));
            item.Columns.Add("ColumnNumber", typeof(int));
            item.Columns.Add("Summary", typeof(bool));
            item.Columns.Add("Print", typeof(bool));

            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlDocs1()
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(GetInTestDataPath(@"Data\xsd\496192.xsd"));
            dataSet.ReadXml(GetInTestDataPath(@"Data\496192.xml"));

            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable personTable = new DataTable("Person");
            expectedTables.AddTable(personTable, 1);
            personTable.Columns.Add("Firstname");
            personTable.Columns.Add("Middlename");
            personTable.Columns.Add("Lastname");
            personTable.Columns.Add("Person_Id", typeof(int));
            personTable.Rows.Add("Daniel", null, "Stutz", "0");

            DataTable adressesTable = new DataTable("Addresses");
            expectedTables.AddTable(adressesTable, 1);
            adressesTable.Columns.Add("Addresses_Id", typeof(int));
            adressesTable.Columns.Add("Person_Id", typeof(int));
            adressesTable.Rows.Add("0", "0");

            DataTable adressTable = new DataTable("Address");
            expectedTables.AddTable(adressTable, 2);
            adressTable.Columns.Add("Street");
            adressTable.Columns.Add("ZIP");
            adressTable.Columns.Add("City");
            adressTable.Columns.Add("Country");
            adressTable.Columns.Add("Addresses_Id", typeof(int));
            adressTable.Rows.Add("Teststreet", "8000", "Zich", null, "0");
            adressTable.Rows.Add(null, null, "Angaga", "Maldives", "0");

            expectedTables.AddRelation(adressesTable, adressTable, new string[] { "Addresses_Id" }, new string[] { "Addresses_Id" });
            expectedTables.AddRelation(personTable, adressesTable, new string[] { "Person_Id" }, new string[] { "Person_Id" });

            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlDocs2()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                DataSet dataSet = new DataSet();
                dataSet.ReadXmlSchema(GetInTestDataPath(@"Data\xsd\676993.xsd"));
                dataSet.ReadXml(GetInTestDataPath(@"Data\676993.xml"));

                AssertTableInfo expectedTables = new AssertTableInfo();

                DataTable espPump = new DataTable("ESPPump");
                expectedTables.AddTable(espPump, 1);
                espPump.Columns.Add("id", typeof(uint));
                espPump.Columns.Add("ESPPump_Id", typeof(int));
                espPump.Rows.Add("1", "0");

                DataTable performanceTable = new DataTable("PerformanceTable");
                expectedTables.AddTable(performanceTable, 1);
                performanceTable.Columns.Add("t1", typeof(double));
                performanceTable.Columns.Add("t2", typeof(double));
                performanceTable.Columns.Add("t3", typeof(double));
                performanceTable.Columns.Add("t4", typeof(double));
                performanceTable.Columns.Add("t5", typeof(Decimal));
                performanceTable.Columns.Add("t6", typeof(Decimal));
                performanceTable.Columns.Add("t7", typeof(double));
                performanceTable.Columns.Add("t8", typeof(double));
                performanceTable.Columns.Add("t9", typeof(double));
                performanceTable.Columns.Add("t10", typeof(double));
                performanceTable.Columns.Add("PerformanceTable_Id", typeof(int));
                performanceTable.Columns.Add("ESPPump_Id", typeof(int));
                performanceTable.Rows.Add("1.14", "1.15", "1.16", "1.17", "1.18", "1.19", "1.201", "1.21", "1.22", "1.23", "0", "0");

                DataTable performanceValueTable = new DataTable("PerformanceValue");
                expectedTables.AddTable(performanceValueTable, 1);
                performanceValueTable.Columns.Add("a1", typeof(double));
                performanceValueTable.Columns.Add("a2", typeof(double));
                performanceValueTable.Columns.Add("a3", typeof(Decimal));
                performanceValueTable.Columns.Add("a4", typeof(double));
                performanceValueTable.Columns.Add("PerformanceTable_Id", typeof(int));
                performanceValueTable.Rows.Add("1.24", "1.25", "1.26", "1.27", "0");

                expectedTables.AddRelation(performanceTable, performanceValueTable, new string[] { "PerformanceTable_Id" }, new string[] { "PerformanceTable_Id" });
                expectedTables.AddRelation(espPump, performanceTable, new string[] { "ESPPump_Id" }, new string[] { "ESPPump_Id" });

                AssertDataSet(dataSet, expectedTables);
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }



        [Test]
        public void TestReadXmlJ1157()
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(GetInTestDataPath(@"Data\TestJira1157.xml"));

            DataSetWriter.PrintDataSet(dataSet);

            AssertTableInfo expectedTables = new AssertTableInfo();
            expectedTables.DataSetName = "NewDataSet";

            DataTable iscrizionePdf = new DataTable("iscrizionePdf");
            expectedTables.AddTable(iscrizionePdf, 1);
            iscrizionePdf.Columns.Add("test", typeof(string));
            iscrizionePdf.Columns.Add("iscrizionePdf_Id", typeof(int));
            iscrizionePdf.Rows.Add("ABC", 0);

            DataTable identity = new DataTable("identity");
            expectedTables.AddTable(identity, 1);
            identity.Columns.Add("name", typeof(string));
            identity.Columns.Add("id", typeof(string));
            identity.Columns.Add("birthDate", typeof(string));
            identity.Columns.Add("iscrizionePdf_Id", typeof(int));
            identity.Rows.Add("Simone", "123", "1950-04-01T00:00:00+01:00", 0);

            DataTable issue = new DataTable("issue");
            expectedTables.AddTable(issue, 1);
            issue.Columns.Add("title", typeof(string));
            issue.Columns.Add("iscrizionePdf_Id", typeof(int));
            issue.Rows.Add("xxxx", 0);

            expectedTables.AddRelation(iscrizionePdf, identity, new string[] { "iscrizionePdf_Id" }, new string[] { "iscrizionePdf_Id" });
            expectedTables.AddRelation(iscrizionePdf, issue, new string[] { "iscrizionePdf_Id" }, new string[] { "iscrizionePdf_Id" });

            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadDateTimeColumns()
        {
            DataSet dataSet = new DataSet();

            dataSet.ReadXml(GetInTestDataPath(@"Data\TestTableWithDates.xml"));

            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable root = new DataTable("root");
            expectedTables.AddTable(root, 1);
            root.Columns.Add("zipCode", typeof(string));
            root.Columns.Add("cellulare", typeof(string));
            root.Columns.Add("date1", TestDataSetReadSchema.GetDateTimeType());
            root.Columns.Add("date2", TestDataSetReadSchema.GetDateTimeType());
            root.Columns.Add("date3", TestDataSetReadSchema.GetDateTimeType());
            root.Columns.Add("date4", TestDataSetReadSchema.GetDateTimeType());

            root.Rows.Add("11122233", null, DateTime.Parse("1950-04-01T00:00:00+01:00"),
                DateTime.Parse("2002-01-01T00:00:00+01:00"), DateTime.Parse("2015-06-22T00:00:00+02:00"),
                DateTime.Parse("2015-06-26T00:00:00+02:00"));
            AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestJava2282()
        {
            DataSet dataSet = new DataSet();

            DataTable parentTable = new DataTable("PARENT");
            parentTable.Columns.Add("ID", typeof(int));
            dataSet.Tables.Add(parentTable);

            DataTable childTable = new DataTable("CHILD");
            childTable.Columns.Add("PARENT_ID", typeof(int));
            dataSet.Tables.Add(childTable);

            dataSet.Relations.Add("PARENT_CHILD", parentTable.Columns["ID"], childTable.Columns["PARENT_ID"], false);

            parentTable.Rows.Add(1);
            parentTable.Rows.Add(2);
            parentTable.Rows.Add(1);

            childTable.Rows.Add(1);

            DataRow[] parentRows = childTable.Rows[0].GetParentRows(dataSet.Relations[0]);
            foreach (DataRow r in parentRows)
                Console.WriteLine("row=" + r[0]);
            Assert.That(parentRows.Length, Is.EqualTo(2));
        }

        [JavaThrows(true)]
        private static AssertTableInfo AssertJ1158(DataSet dataSet)
        {
            AssertTableInfo expectedTables = new AssertTableInfo();

            DataTable civicoStrada = new DataTable("civicoStrada");
            expectedTables.AddTable(civicoStrada, 2);
            civicoStrada.Columns.Add("codice", typeof(string));
            civicoStrada.Columns.Add("numero", typeof(string));
            civicoStrada.Columns.Add("civicoStrada_Id", typeof(int));
            civicoStrada.Columns.Add("domicilio_Id", typeof(int));
            civicoStrada.Columns.Add("residenza_Id", typeof(int));

            civicoStrada.Rows.Add("7850", "16", "0", "0", null);
            civicoStrada.Rows.Add("11445", "96", "1", null, "0");

            DataTable strada = new DataTable("strada");
            expectedTables.AddTable(strada, 3);
            strada.Columns.Add("areaIt", typeof(string));
            strada.Columns.Add("codice", typeof(string));
            strada.Columns.Add("descrizioneDe", typeof(string));
            strada.Columns.Add("descrizioneIt", typeof(string));
            strada.Columns.Add("civicoStrada_Id", typeof(int));
            strada.Columns.Add("civico_Id", typeof(int));

            strada.Rows.Add("VIA", "2160", "DELAI-STRASSE", "DELAI", "0", null);
            strada.Rows.Add("VIA", "1710", "CLAUDIA-AUGUSTA-STRASSE", "CLAUDIA AUGUSTA", null, "0");
            strada.Rows.Add("VIA", "6960", "ST.-VIGIL-STRASSE", "SAN VIGILIO", "1", null);

            DataTable comune = new DataTable("comune");
            expectedTables.AddTable(comune, 2);
            comune.Columns.Add("capoluogo", typeof(string));
            comune.Columns.Add("codiceIstat", typeof(string));
            comune.Columns.Add("descrizioneDe", typeof(string));
            comune.Columns.Add("descrizioneIt", typeof(string));
            comune.Columns.Add("comune_Id", typeof(int));
            comune.Columns.Add("domicilio_Id", typeof(int));
            comune.Columns.Add("residenza_Id", typeof(int));

            comune.Rows.Add("true", "021008", "BOZEN", "BOLZANO", "0", "0", null);
            comune.Rows.Add("true", "021008", "BOZEN", "BOLZANO", "1", null, "0");

            DataTable provincia = new DataTable("provincia");
            expectedTables.AddTable(provincia, 2);
            provincia.Columns.Add("codiceIstat", typeof(string));
            provincia.Columns.Add("descrizione", typeof(string));
            provincia.Columns.Add("siglaAuto", typeof(string));
            provincia.Columns.Add("comune_Id", typeof(int));

            provincia.Rows.Add("021", "BOLZANO/BOZEN", "BZ", "0");
            provincia.Rows.Add("021", "BOLZANO/BOZEN", "BZ", "1");

            DataTable iscrizionePdf = new DataTable("iscrizionePdf");
            expectedTables.AddTable(iscrizionePdf, 1);
            iscrizionePdf.Columns.Add("domicilioCivico", typeof(string));
            iscrizionePdf.Columns.Add("domicilioViaDe", typeof(string));
            iscrizionePdf.Columns.Add("domicilioViaIt", typeof(string));
            iscrizionePdf.Columns.Add("residenzaCivico", typeof(string));
            iscrizionePdf.Columns.Add("residenzaViaDe", typeof(string));
            iscrizionePdf.Columns.Add("residenzaViaIt", typeof(string));
            iscrizionePdf.Columns.Add("iscrizionePdf_Id", typeof(int));

            iscrizionePdf.Rows.Add("16", "DELAI-STRASSE", "VIA DELAI", "96", "ST.-VIGIL-STRASSE", "VIA SAN VIGILIO", "0");

            DataTable identita = new DataTable("identita");
            expectedTables.AddTable(identita, 1);
            identita.Columns.Add("cap", typeof(string));
            identita.Columns.Add("cellulare", typeof(string));
            identita.Columns.Add("codiceFiscale", typeof(string));
            identita.Columns.Add("cognome", typeof(string));
            identita.Columns.Add("dataDiNascita", typeof(string));
            identita.Columns.Add("email", typeof(string));
            identita.Columns.Add("linguaPreferita", typeof(string));
            identita.Columns.Add("localitaEstera", typeof(string));
            identita.Columns.Add("localitaEsteraDiNascita", typeof(string));
            identita.Columns.Add("nome", typeof(string));
            identita.Columns.Add("numeroCivico", typeof(string));
            identita.Columns.Add("sesso", typeof(string));
            identita.Columns.Add("statoDe", typeof(string));
            identita.Columns.Add("statoIt", typeof(string));
            identita.Columns.Add("strada", typeof(string));
            identita.Columns.Add("telefono", typeof(string));
            identita.Columns.Add("tipoPersona", typeof(string));
            identita.Columns.Add("utenteCollegatoCodiceFiscale", typeof(string));
            identita.Columns.Add("utenteCollegatoCognome", typeof(string));
            identita.Columns.Add("utenteCollegatoNome", typeof(string));
            identita.Columns.Add("iscrizionePdf_Id", typeof(int));

            identita.Rows.Add("zipCode", "", "SNTSMN80H15A952N", "Represented", "1950-04-01T00:00:00+01:00", "testuser@siag.it",
                "de", "locationName", "?", "Test", "streetNumber", "M", "nameDE", "nameIT", "street", "", "PersonaFisica",
                "SNTSMN80H15A952N", "Testuser", "Testuser", 0);

            DataTable istanza = new DataTable("istanza");
            expectedTables.AddTable(istanza, 1);
            istanza.Columns.Add("anno", typeof(string));
            istanza.Columns.Add("cellulare", typeof(string));
            istanza.Columns.Add("dichiarazioniMendaci", typeof(string));
            istanza.Columns.Add("email", typeof(string));
            istanza.Columns.Add("familyPlus", typeof(string));
            istanza.Columns.Add("importo", typeof(string));
            istanza.Columns.Add("informativa", typeof(string));
            istanza.Columns.Add("minoriResidentiBolzano", typeof(string));
            istanza.Columns.Add("ricevutaPagamento", typeof(string));
            istanza.Columns.Add("telefono", typeof(string));
            istanza.Columns.Add("istanza_Id", typeof(int));
            istanza.Columns.Add("iscrizionePdf_Id", typeof(int));

            istanza.Rows.Add("2015", "222", "true", "testuser@siag.it", "false", "54", "true", "true", "true", "222", 0, 0);

            DataTable domicilio = new DataTable("domicilio");
            expectedTables.AddTable(domicilio, 1);
            domicilio.Columns.Add("cap", typeof(string));
            domicilio.Columns.Add("domicilio_Id", typeof(int));
            domicilio.Columns.Add("istanza_Id", typeof(int));

            domicilio.Rows.Add("39100", "0", "0");

            DataTable minori = new DataTable("minori");
            expectedTables.AddTable(minori, 1);
            minori.Columns.Add("assistenzaPersonale", typeof(string));
            minori.Columns.Add("certificatoMedico", typeof(string));
            minori.Columns.Add("classe", typeof(string));
            minori.Columns.Add("codiceFiscale", typeof(string));
            minori.Columns.Add("cognome", typeof(string));
            minori.Columns.Add("dataNascita", typeof(string));
            minori.Columns.Add("nome", typeof(string));
            minori.Columns.Add("oreAssistenza", typeof(string));
            minori.Columns.Add("minori_Id", typeof(int));
            minori.Columns.Add("istanza_Id", typeof(int));

            minori.Rows.Add("true", "1", "1", "PDVSMN71L22A952V", "4562222X", "2002-01-01T00:00:00+01:00", "2^12", 0, 0, 0);

            DataTable dieta = new DataTable("dieta");
            expectedTables.AddTable(dieta, 1);
            dieta.Columns.Add("codice", typeof(string));
            dieta.Columns.Add("descrizioneDe", typeof(string));
            dieta.Columns.Add("descrizioneIt", typeof(string));
            dieta.Columns.Add("minori_Id", typeof(int));

            dieta.Rows.Add("NOCARNE", "Menu ohne jede Art von Fleisch", "Menu senza alcun tipo di carne", 0);

            DataTable malattie = new DataTable("malattie");
            expectedTables.AddTable(malattie, 1);
            malattie.Columns.Add("codice", typeof(string));
            malattie.Columns.Add("descrizioneDe", typeof(string));
            malattie.Columns.Add("descrizioneIt", typeof(string));
            malattie.Columns.Add("minori_Id", typeof(int));

            malattie.Rows.Add("ALLERGIE", "Allergien/Unvertraglichkeiten", "Allergie/intolleranze varie", 0);

            DataTable scuola = new DataTable("scuola");
            expectedTables.AddTable(scuola, 1);
            scuola.Columns.Add("codice", typeof(string));
            scuola.Columns.Add("descrizioneDe", typeof(string));
            scuola.Columns.Add("descrizioneIt", typeof(string));
            scuola.Columns.Add("email", typeof(string));
            scuola.Columns.Add("lingua", typeof(string));
            scuola.Columns.Add("telefono", typeof(string));
            scuola.Columns.Add("tipo", typeof(string));
            scuola.Columns.Add("scuola_Id", typeof(int));
            scuola.Columns.Add("minori_Id", typeof(int));

            scuola.Rows.Add("662", "DANTE ALIGHIERI (KINDERGARTEN)", "DANTE ALIGHIERI (MATERNA)", "scuolamaternadante@tin.it",
                "IT", "0471 28 25 98", "Materna", 0, 0);

            DataTable civico = new DataTable("civico");
            expectedTables.AddTable(civico, 1);
            civico.Columns.Add("codice", typeof(string));
            civico.Columns.Add("numero", typeof(string));
            civico.Columns.Add("civico_Id", typeof(int));
            civico.Columns.Add("scuola_Id", typeof(int));

            civico.Rows.Add("7427", "38", 0, 0);

            DataTable turni = new DataTable("turni");
            expectedTables.AddTable(turni, 1);
            turni.Columns.Add("codice", typeof(string));
            turni.Columns.Add("daData", typeof(string));
            turni.Columns.Add("descrizioneDe", typeof(string));
            turni.Columns.Add("descrizioneIt", typeof(string));
            turni.Columns.Add("aData", typeof(string));
            turni.Columns.Add("minori_Id", typeof(int));

            turni.Rows.Add("1", "2015-06-22T00:00:00+02:00", "1 turno", "1 turno", "2015-06-26T00:00:00+02:00", 0);

            DataTable potestaGenitoriale = new DataTable("potestaGenitoriale");
            expectedTables.AddTable(potestaGenitoriale, 1);
            potestaGenitoriale.Columns.Add("codice", typeof(string));
            potestaGenitoriale.Columns.Add("descrizioneDe", typeof(string));
            potestaGenitoriale.Columns.Add("descrizioneIt", typeof(string));
            potestaGenitoriale.Columns.Add("istanza_Id", typeof(int));

            potestaGenitoriale.Rows.Add("GEN", "Elternteil", "Esercente responsabilitÃ¡ genitoriale", 0);

            DataTable residenza = new DataTable("residenza");
            expectedTables.AddTable(residenza, 1);
            residenza.Columns.Add("cap", typeof(string));
            residenza.Columns.Add("residenza_Id", typeof(int));
            residenza.Columns.Add("istanza_Id", typeof(int));

            residenza.Rows.Add("39100", 0, 0);

            expectedTables.AddRelation(civicoStrada, strada, new string[] { "civicoStrada_Id" }, new string[] { "civicoStrada_Id" });
            expectedTables.AddRelation(comune, provincia, new string[] { "comune_Id" }, new string[] { "comune_Id" });
            expectedTables.AddRelation(domicilio, civicoStrada, new string[] { "domicilio_Id" }, new string[] { "domicilio_Id" });
            expectedTables.AddRelation(domicilio, comune, new string[] { "domicilio_Id" }, new string[] { "domicilio_Id" });
            expectedTables.AddRelation(civico, strada, new string[] { "civico_Id" }, new string[] { "civico_Id" });
            expectedTables.AddRelation(scuola, civico, new string[] { "scuola_Id" }, new string[] { "scuola_Id" });
            expectedTables.AddRelation(minori, dieta, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation(minori, malattie, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation(minori, scuola, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation(minori, turni, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation(residenza, civicoStrada, new string[] { "residenza_Id" }, new string[] { "residenza_Id" });
            expectedTables.AddRelation(residenza, comune, new string[] { "residenza_Id" }, new string[] { "residenza_Id" });
            expectedTables.AddRelation(istanza, domicilio, new string[] { "istanza_Id" }, new string[] { "istanza_Id" });
            expectedTables.AddRelation(istanza, minori, new string[] { "istanza_Id" }, new string[] { "istanza_Id" });
            expectedTables.AddRelation(istanza, potestaGenitoriale, new string[] { "istanza_Id" }, new string[] { "istanza_Id" });
            expectedTables.AddRelation(istanza, residenza, new string[] { "istanza_Id" }, new string[] { "istanza_Id" });
            expectedTables.AddRelation(iscrizionePdf, identita, new string[] { "iscrizionePdf_Id" },
                new string[] { "iscrizionePdf_Id" });
            expectedTables.AddRelation(iscrizionePdf, istanza, new string[] { "iscrizionePdf_Id" },
                new string[] { "iscrizionePdf_Id" });

            AssertDataSet(dataSet, expectedTables);

            return expectedTables;
        }

        internal static void AssertDataSet(DataSet dataSet, AssertTableInfo info)
        {
            if (info.DataSetName != null)
                Assert.That(dataSet.DataSetName, Is.EqualTo(info.DataSetName));

            if (dataSet.Tables.Count != info.GetTables().Count)
                Assert.Fail("Expected number of tables is different from the actual DataSet tables. \nMissed following tables: " + GetMissedTables(dataSet, info));

            if (dataSet.Relations.Count != info.GetRelations().Count)
                Assert.Fail("Expected number of relations is different from the actual DataSet' relations. \nMissed following relationships: " + GetMissedRelations(dataSet, info));

            foreach (DataTable expectedTable in info.GetTables())
            {
                DataTable actualTable = dataSet.Tables[expectedTable.TableName];

                Assert.That(actualTable, IsNot.Null(), "Table '" + expectedTable.TableName + "' does not present in actual DataSet");
                Assert.That(expectedTable.TableName, Is.EqualTo(actualTable.TableName), "Table names are different");
                Assert.That(expectedTable.Columns.Count, Is.EqualTo(actualTable.Columns.Count), "Table '" + actualTable.TableName + "' columns count is different.");

                AssertRowsCount(actualTable, info.GetRowsCount(expectedTable));

                // check only if AssertTableInfo contains cols and rows to be tested.
                for (int c = 0; c < expectedTable.Columns.Count; c++)
                {
                    DataColumn expectedDataColumn = expectedTable.Columns[c];
                    String expectedColumnName = expectedDataColumn.ColumnName;

                    // position of columns could be different than in .NET
                    DataColumn actualDataColumn = actualTable.Columns[expectedColumnName];

                    Assert.That(actualDataColumn, IsNot.Null(), "Data Column: '" + expectedColumnName + "' was not found in DataSet");
                    Assert.That(expectedColumnName, Is.EqualTo(actualDataColumn.ColumnName));

                    if (expectedDataColumn.DataType != typeof(object))
                        Assert.That(actualDataColumn.DataType, Is.EqualTo(expectedDataColumn.DataType), "DataTable: " + actualDataColumn.Table.TableName + " DataColumn: " + expectedColumnName);
                }

                int expCounter = 0;
                int actCounter = 0;
                while (expCounter < expectedTable.Rows.Count)
                {
                    while (actCounter < actualTable.Rows.Count && actualTable.Rows[actCounter].RowState == DataRowState.Deleted)
                        actCounter++;

                    for (int rc = 0; rc < actualTable.Columns.Count; rc++)
                    {
                        string expectedColumnName = expectedTable.Columns[rc].ColumnName;
                        string actualColumnName = actualTable.Columns[expectedColumnName].ColumnName;
                        object actualRowValueAtColumn = actualTable.Rows[actCounter][expectedColumnName];
                        object expectedRowValueAtColumn = expectedTable.Rows[expCounter][expectedColumnName];
                        Assert.That((expectedRowValueAtColumn != null)
                                ? expectedRowValueAtColumn.ToString()
                                : "", Is.EqualTo((actualRowValueAtColumn != null)
                            ? actualRowValueAtColumn.ToString()
                            : ""), "Table '" + actualTable.TableName + "' Column='" + actualColumnName +
                            "' actual=" + actualRowValueAtColumn + " expected=" + expectedRowValueAtColumn);
                    }

                    expCounter++;
                    actCounter++;
                }
            }

            // dataRelations
            Dictionary<string, DataRelation> actualDataRelationMap = new Dictionary<string, DataRelation>();
            foreach (DataRelation dr in dataSet.Relations)
                actualDataRelationMap.Add(dr.RelationName, dr);

            foreach (DataRelationStub expectedDRelation in info.GetRelations())
            {
                DataRelation actualDRelation = actualDataRelationMap.GetValueOrNull(expectedDRelation.RelationName);
                Assert.That(actualDRelation, IsNot.Null(), "DataRelation '" + expectedDRelation.RelationName + "' is not found.");
                Assert.That(expectedDRelation.RelationName, Is.EqualTo(actualDRelation.RelationName));
                Assert.That(expectedDRelation.ParentTableName, Is.EqualTo(actualDRelation.ParentTable.TableName));
                Assert.That(expectedDRelation.ChildTableName, Is.EqualTo(actualDRelation.ChildTable.TableName));
                Assert.That(expectedDRelation.ParentColumns.Length, Is.EqualTo(actualDRelation.ParentColumns.Length));
                Assert.That(expectedDRelation.ChildColumns.Length, Is.EqualTo(actualDRelation.ChildColumns.Length));

                // check only if AssertTableInfo contains these names.
                for (int p = 0; p < expectedDRelation.ParentColumns.Length; p++)
                    Assert.That(expectedDRelation.ParentColumns[p], Is.EqualTo(actualDRelation.ParentColumns[p].ColumnName));

                for (int p = 0; p < expectedDRelation.ChildColumns.Length; p++)
                    Assert.That(expectedDRelation.ChildColumns[p], Is.EqualTo(actualDRelation.ChildColumns[p].ColumnName));
            }
        }

        internal static string GetInTestDataPath(string relativePath)
        {
            return Aspose.TestFx.TestFxUtil.CorrectPath(Path.Combine(@"X:\awnet\TestData", relativePath));
        }

        private static void AssertRowsCount(DataTable actual, int expected)
        {
            int actualNonDeletedRowCount = 0;
            foreach (DataRow dataRow in actual.Rows)
            {
                if (dataRow.RowState != DataRowState.Deleted)
                    actualNonDeletedRowCount++;
            }

            Assert.That(actualNonDeletedRowCount, Is.EqualTo(expected), "Row's count is different for " + actual.TableName);
        }

        private static string GetMissedTables(DataSet dataSet, AssertTableInfo info)
        {
            StringBuilder str = new StringBuilder();
            foreach (DataTable expectedTable in info.GetTables())
            {
                DataTable actualTable = dataSet.Tables[expectedTable.TableName];
                if (actualTable == null)
                    str.Append(expectedTable.TableName).Append(", ");
            }
            return str.ToString();
        }


        private static string GetMissedRelations(DataSet dataSet, AssertTableInfo info)
        {
            List<string> actualRelationNames = new List<string>(dataSet.Relations.Count);
            foreach (DataRelation rel in dataSet.Relations)
                actualRelationNames.Add(rel.RelationName);


            StringBuilder str = new StringBuilder();
            foreach (object relation in info.GetRelations())
            {
                DataRelationStub expectedRelation = relation as DataRelationStub;
                if (!actualRelationNames.Contains(expectedRelation.RelationName))
                    str.Append(expectedRelation.RelationName).Append(", ");
            }

            foreach (string relation in actualRelationNames)
            {
                if (!ContainsRelationship(info, relation))
                    str.Append(relation).Append(" first child column: " + dataSet.Relations[relation].ChildColumns[0].ColumnName).Append(" and it is not being expected.").Append("\n");
            }

            return str.ToString();
        }

        private static bool ContainsRelationship(AssertTableInfo dataSet, string relation)
        {
            foreach (object actual in dataSet.GetRelations())
                if (((DataRelationStub)actual).RelationName.Equals(relation))
                    return true;

            return false;
        }

        public class AssertTableInfo
        {
            public ICollection<DataTable> GetTables()
            {
                return mTables.Values;
            }

            public void AddTable(DataTable dt, int rows)
            {
                mTables.Add(dt.TableName, dt);
                mRowsCount.Add(dt.TableName, rows);
            }

            public IList<DataRelationStub> GetRelations()
            {
                return mRelations;
            }

            public void AddRelation(string relationName, DataTable parentTable, DataTable childTable, string[] parentColumnNames, string[] childColumnNames)
            {
                DataRelationStub relation = new DataRelationStub();
                relation.RelationName = relationName;
                relation.ParentTableName = parentTable.TableName;
                relation.ChildTableName = childTable.TableName;
                relation.ParentColumns = parentColumnNames;
                relation.ChildColumns = childColumnNames;

                if (!mRelations.Contains(relation))
                    mRelations.Add(relation);
            }

            public void AddRelation(DataTable parentTable, DataTable childTable, string[] parentColumnNames, string[] childColumnNames)
            {
                string relationName = parentTable.TableName + "_" + childTable.TableName;
                AddRelation(relationName, parentTable, childTable, parentColumnNames, childColumnNames);
            }

            public void SetRowsCount(DataTable dataTable, int rowsCount)
            {
                mRowsCount.Remove(dataTable.TableName);
                mRowsCount.Add(dataTable.TableName, rowsCount);
            }

            public int GetRowsCount(DataTable dataTable)
            {
                return mRowsCount[dataTable.TableName];
            }

            public DataTable GetTable(string tableName)
            {
                return mTables.GetValueOrNull(tableName);
            }

            private readonly Dictionary<string, DataTable> mTables = new Dictionary<string, DataTable>();
            private readonly List<DataRelationStub> mRelations = new List<DataRelationStub>();
            private readonly Dictionary<string, int> mRowsCount = new Dictionary<string, int>();
            public string DataSetName { get; set; }
        }

        public class DataRelationStub
        {
            public string RelationName { get; set; }
            public string ParentTableName { get; set; }
            public string ChildTableName { get; set; }
            public string[] ParentColumns { get; set; }
            public string[] ChildColumns { get; set; }
        }
    }
}
