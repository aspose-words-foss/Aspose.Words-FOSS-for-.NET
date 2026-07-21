// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2016 by Anatoly Sidorenko

using System;
using System.Data;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Data
{
    /// <summary>
    /// The main purpose of this class is to provide data for Java's tests.
    /// This class will be moved into new project Aspose.Ms.Tests when it gets created.
    /// </summary>
    [TestFixture]
    public class TestDataSetReadSchema
    {

        [Test]
        public void TestReadXmlSchema2()
        {
            const string file = @"Data\xsd\own_schema.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item = new DataTable("test_table");
            expectedTables.AddTable(item, 0);
            item.Columns.Add("first");
            item.Columns.Add("second", typeof(object)); // do not check this type because it is not ported

            DataTable item2 = new DataTable("second_test_table");
            expectedTables.AddTable(item2, 0);
            item2.Columns.Add("second_first");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlSchema3()
        {
            const string file = @"Data\xsd\own_schema1.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item = new DataTable("test_table");
            expectedTables.AddTable(item, 0);
            item.Columns.Add("first");
            item.Columns.Add("second", typeof(object)); // do not check this type because it is not ported

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlSchema4()
        {
            const string file = @"Data\xsd\own_schema2.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item = new DataTable("second_test_table");
            expectedTables.AddTable(item, 0);
            item.Columns.Add("second_first");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlSchema5()
        {
            const string file = @"Data\xsd\store.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item = new DataTable("bookstore");
            expectedTables.AddTable(item, 0);
            item.Columns.Add("bookstore_Id", typeof(int));

            DataTable item2 = new DataTable("book");
            expectedTables.AddTable(item2, 0);
            item2.Columns.Add("genre");
            item2.Columns.Add("title");
            item2.Columns.Add("price", typeof(Decimal));
            item2.Columns.Add("book_Id", typeof(int));
            item2.Columns.Add("bookstore_Id", typeof(int));


            DataTable item3 = new DataTable("author");
            expectedTables.AddTable(item3, 0);
            item3.Columns.Add("first-name");
            item3.Columns.Add("last-name");
            item3.Columns.Add("book_Id", typeof(int));

            expectedTables.AddRelation("book_author", item2, item3, new string[] { "book_Id" }, new string[] { "book_Id" });
            expectedTables.AddRelation("bookstore_book", item, item2, new string[] { "bookstore_Id" }, new string[] { "bookstore_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlSchema6()
        {
            const string file = @"Data\xsd\store2.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item2 = new DataTable("book");
            expectedTables.AddTable(item2, 0);
            item2.Columns.Add("genre");
            item2.Columns.Add("title");
            item2.Columns.Add("price", typeof(Decimal));
            item2.Columns.Add("book_Id", typeof(int));

            DataTable item3 = new DataTable("author");
            expectedTables.AddTable(item3, 0);
            item3.Columns.Add("first-name");
            item3.Columns.Add("last-name");
            item3.Columns.Add("book_Id", typeof(int));

            expectedTables.AddRelation("book_author", item2, item3, new string[] { "book_Id" }, new string[] { "book_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlSchema7()
        {
            const string file = @"Data\xsd\store3.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item = new DataTable("bookstore");
            expectedTables.AddTable(item, 0);
            item.Columns.Add("bookstore_Id", typeof(int));

            DataTable item2 = new DataTable("book");
            expectedTables.AddTable(item2, 0);
            item2.Columns.Add("genre");
            item2.Columns.Add("title");
            item2.Columns.Add("price", typeof(Decimal));
            item2.Columns.Add("book_Id", typeof(int));
            item2.Columns.Add("bookstore_Id", typeof(int));


            DataTable item3 = new DataTable("author");
            expectedTables.AddTable(item3, 0);
            item3.Columns.Add("first-name");
            item3.Columns.Add("last-name");
            item3.Columns.Add("book_Id", typeof(int));

            expectedTables.AddRelation("book_author", item2, item3, new string[] { "book_Id" }, new string[] { "book_Id" });
            expectedTables.AddRelation("bookstore_book", item, item2, new string[] { "bookstore_Id" }, new string[] { "bookstore_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlSchema8()
        {
            const string file = @"Data\xsd\store4.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable item2 = new DataTable("book");
            expectedTables.AddTable(item2, 0);
            item2.Columns.Add("genre");
            item2.Columns.Add("title");
            item2.Columns.Add("price", typeof(Decimal));
            item2.Columns.Add("book_Id", typeof(int));

            DataTable item3 = new DataTable("author");
            expectedTables.AddTable(item3, 0);
            item3.Columns.Add("first-name");
            item3.Columns.Add("last-name");
            item3.Columns.Add("book_Id", typeof(int));

            expectedTables.AddRelation("book_author", item2, item3, new string[] { "book_Id" }, new string[] { "book_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        [ExpectedException(typeof(DataException))]
        public void TestReadXmlXsd1()
        {
            const string file = @"Data\xsd\1.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));
        }

        [Test]
        public void TestReadXmlXsdIncludeOtherSchema1()
        {
            const string file = @"Data\xsd\3.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));
        }

        [Test]
        public void TestReadXmlXsd5()
        {
            const string file = @"Data\xsd\5.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable foo = new DataTable("Foo");
            expectedTables.AddTable(foo, 0);
            foo.Columns.Add("Bar");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd6()
        {
            const string file = @"Data\xsd\6.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable foo = new DataTable("Foo");
            expectedTables.AddTable(foo, 0);
            foo.Columns.Add("Bar");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlDatatypes1()
        {
            const string file = @"Data\xsd\datatypesTest.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable foo = new DataTable("e00");
            expectedTables.AddTable(foo, 0);

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd8()
        {
            const string file = @"Data\xsd\x-net-config-storage-factory-provider-schema.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable conStringTable = new DataTable("connection-string");
            expectedTables.AddTable(conStringTable, 0);
            conStringTable.Columns.Add("text");
            conStringTable.Columns.Add("connection-string_Id", typeof(int));

            DataTable secretTable = new DataTable("secret");
            expectedTables.AddTable(secretTable, 0);
            secretTable.Columns.Add("n");
            secretTable.Columns.Add("secret_text");
            secretTable.Columns.Add("connection-string_Id", typeof(int));

            DataTable paramsTable = new DataTable("params");
            expectedTables.AddTable(paramsTable, 0);
            paramsTable.Columns.Add("params_Id", typeof(int));

            DataTable paramTable = new DataTable("param");
            expectedTables.AddTable(paramTable, 0);
            paramTable.Columns.Add("n");
            paramTable.Columns.Add("param_text");
            paramTable.Columns.Add("params_Id", typeof(int));

            expectedTables.AddRelation("connection-string_secret", conStringTable, secretTable, new string[] { "connection-string_Id" }, new string[] { "connection-string_Id" });
            expectedTables.AddRelation("params_param", paramsTable, paramTable, new string[] { "params_Id" }, new string[] { "params_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd9()
        {
            const string file = @"Data\xsd\77687.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable table = new DataTable("text");
            expectedTables.AddTable(table, 0);
            table.Columns.Add("foo");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd10()
        {
            const string file = @"Data\xsd\78985.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable scoreGroupTable = new DataTable("scoreGroup");
            expectedTables.AddTable(scoreGroupTable, 0);
            scoreGroupTable.Columns.Add("key");
            scoreGroupTable.Columns.Add("scoreGroup_Id", typeof(int));

            DataTable scoreTable = new DataTable("score");
            expectedTables.AddTable(scoreTable, 0);
            scoreTable.Columns.Add("key");
            scoreTable.Columns.Add("score_text");
            scoreTable.Columns.Add("scoreGroup_Id", typeof(int));

            expectedTables.AddRelation("scoreGroup_score", scoreGroupTable, scoreTable, new string[] { "scoreGroup_Id" }, new string[] { "scoreGroup_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd11()
        {
            const string file = @"Data\xsd\79650.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable table = new DataTable("test");
            expectedTables.AddTable(table, 0);
            table.Columns.Add("mode");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd12()
        {
            const string file = @"Data\xsd\82010.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable documentTable = new DataTable("Document");
            expectedTables.AddTable(documentTable, 0);
            documentTable.Columns.Add("Document_Id", typeof(int));

            DataTable simpleCodeListTable = new DataTable("SimpleCodeList");
            expectedTables.AddTable(simpleCodeListTable, 0);
            simpleCodeListTable.Columns.Add("SimpleCodeList_Id", typeof(int));
            simpleCodeListTable.Columns.Add("Document_Id", typeof(int));

            DataTable rowsimpleCodeListTable = new DataTable("Row");
            expectedTables.AddTable(rowsimpleCodeListTable, 0);
            rowsimpleCodeListTable.Columns.Add("SimpleCodeList_Id", typeof(int));

            expectedTables.AddRelation("SimpleCodeList_Row", simpleCodeListTable, rowsimpleCodeListTable, new string[] { "SimpleCodeList_Id" }, new string[] { "SimpleCodeList_Id" });
            expectedTables.AddRelation("Document_SimpleCodeList", documentTable, simpleCodeListTable, new string[] { "Document_Id" }, new string[] { "Document_Id" });

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd13()
        {
            const string file = @"Data\xsd\422581.xsd";
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(file));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable table = new DataTable("root");
            expectedTables.AddTable(table, 0);
            table.Columns.Add("foo", typeof(Int64));

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd14()
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(@"Data\xsd\595947.xsd"));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable table1 = new DataTable("MyTest1");
            expectedTables.AddTable(table1, 0);

            DataTable table2 = new DataTable("MyTest2");
            expectedTables.AddTable(table2, 0);
            table2.Columns.Add("Id");

            DataTable table3 = new DataTable("MyTest3");
            expectedTables.AddTable(table3, 0);
            table3.Columns.Add("Id");

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd15()
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath(@"Data\xsd\376395.xsd"));
            dataSet.ReadXml(TestDataSet.GetInTestDataPath(@"Data\376395.xml"));

            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable table1 = new DataTable("B");
            expectedTables.AddTable(table1, 1);
            table1.Columns.Add("C");
            table1.Columns.Add("D");
            table1.Columns.Add("E");
            table1.Rows.Add(null, null, null);

            TestDataSet.AssertDataSet(dataSet, expectedTables);
        }

        [Test]
        public void TestReadXmlXsd1181()
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath("Data\\xsd\\TestJira1181.xsd"));
            TestDataSet.AssertDataSet(dataSet, Assert1181(dataSet, false));
        }

        [Test]
#if !JAVA
        [ExpectedException(typeof(DataException), ExpectedMessage = "DataSet doesn't allow the circular reference in the ComplexType named 'scuola'.")]
#endif
        public void TestReadXmlXsdCyclicDependency1181JavaOnly()
        {
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            try
            {
                SystemPal.SetStandardCulture();
                SystemPal.SetStandardUICulture();

                DataSet dataSet = new DataSet();
                dataSet.ReadXmlSchema(TestDataSet.GetInTestDataPath("Data\\xsd\\TestJira1181_Java.xsd"));

                TestDataSet.AssertDataSet(dataSet, Assert1181(dataSet, true));
            }
            finally
            {
                SystemPal.RestoreCulture();
                SystemPal.RestoreUICulture();
            }
        }

        public static TestDataSet.AssertTableInfo Assert1181(DataSet dataSet, bool addSelfRelation)
        {
            TestDataSet.AssertTableInfo expectedTables = new TestDataSet.AssertTableInfo();

            DataTable indirizzo = new DataTable("indirizzo");
            expectedTables.AddTable(indirizzo, 0);
            indirizzo.Columns.Add("cap");
            indirizzo.Columns.Add("civico");
            indirizzo.Columns.Add("via");
            indirizzo.Columns.Add("indirizzo_Id", typeof(int));

            DataTable civicoStrada = new DataTable("civicoStrada");
            expectedTables.AddTable(civicoStrada, 0);
            civicoStrada.Columns.Add("barrato");
            civicoStrada.Columns.Add("codice");
            civicoStrada.Columns.Add("numero", typeof(int));
            civicoStrada.Columns.Add("scala");
            civicoStrada.Columns.Add("civicoStrada_Id", typeof(int));
            civicoStrada.Columns.Add("indirizzo_Id", typeof(int));
            civicoStrada.Columns.Add("domicilio_Id", typeof(int));
            civicoStrada.Columns.Add("residenza_Id", typeof(int));

            DataTable strada = new DataTable("strada");
            expectedTables.AddTable(strada, 0);
            strada.Columns.Add("areaDe");
            strada.Columns.Add("areaIt");
            strada.Columns.Add("codice");
            strada.Columns.Add("descrizioneDe");
            strada.Columns.Add("descrizioneIt");
            strada.Columns.Add("civicoStrada_Id", typeof(int));
            strada.Columns.Add("civico_Id", typeof(int));

            DataTable comune = new DataTable("comune");
            expectedTables.AddTable(comune, 0);
            comune.Columns.Add("capoluogo", typeof(bool));
            comune.Columns.Add("codiceIstat");
            comune.Columns.Add("dataFine", GetDateTimeType());
            comune.Columns.Add("descrizioneDe");
            comune.Columns.Add("descrizioneIt");
            comune.Columns.Add("comune_Id", typeof(int));
            comune.Columns.Add("indirizzo_Id", typeof(int));
            comune.Columns.Add("domicilio_Id", typeof(int));
            comune.Columns.Add("residenza_Id", typeof(int));

            DataTable provincia = new DataTable("provincia");
            expectedTables.AddTable(provincia, 0);
            provincia.Columns.Add("codiceIstat");
            provincia.Columns.Add("dataFine", GetDateTimeType());
            provincia.Columns.Add("descrizione");
            provincia.Columns.Add("siglaAuto");
            provincia.Columns.Add("comune_Id", typeof(int));

            DataTable iscrizione = new DataTable("iscrizione");
            expectedTables.AddTable(iscrizione, 0);
            iscrizione.Columns.Add("anno", typeof(int));
            iscrizione.Columns.Add("cellulare");
            iscrizione.Columns.Add("dichiarazioniMendaci", typeof(bool));
            iscrizione.Columns.Add("email");
            iscrizione.Columns.Add("familyPlus", typeof(bool));
            iscrizione.Columns.Add("importo", typeof(Decimal));
            iscrizione.Columns.Add("informativa", typeof(bool));
            iscrizione.Columns.Add("minoriResidentiBolzano", typeof(bool));
            iscrizione.Columns.Add("ricevutaPagamento", typeof(bool));
            iscrizione.Columns.Add("telefono");
            iscrizione.Columns.Add("iscrizione_Id", typeof(int));

            DataTable domicilio = new DataTable("domicilio");
            expectedTables.AddTable(domicilio, 0);
            domicilio.Columns.Add("cap");
            domicilio.Columns.Add("civico");
            domicilio.Columns.Add("via");
            domicilio.Columns.Add("domicilio_Id", typeof(int));
            domicilio.Columns.Add("iscrizione_Id", typeof(int));

            DataTable minori = new DataTable("minori");
            expectedTables.AddTable(minori, 0);
            minori.Columns.Add("assistenzaPersonale", typeof(bool));
            minori.Columns.Add("certificatoMedico", typeof(Int16));
            minori.Columns.Add("classe");
            minori.Columns.Add("codiceFiscale");
            minori.Columns.Add("cognome");
            minori.Columns.Add("dataNascita", GetDateTimeType());
            minori.Columns.Add("nome");
            minori.Columns.Add("oreAssistenza", typeof(int));
            minori.Columns.Add("minori_Id", typeof(int));
            minori.Columns.Add("iscrizione_Id", typeof(int));

            DataTable dieta = new DataTable("dieta");
            expectedTables.AddTable(dieta, 0);
            dieta.Columns.Add("altro");
            dieta.Columns.Add("codice");
            dieta.Columns.Add("dataFine", GetDateTimeType());
            dieta.Columns.Add("descrizioneDe");
            dieta.Columns.Add("descrizioneIt");
            dieta.Columns.Add("numero", typeof(Decimal));
            dieta.Columns.Add("minori_Id", typeof(int));
            dieta.Columns.Add("minore_Id", typeof(int));

            DataTable malattie = new DataTable("malattie");
            expectedTables.AddTable(malattie, 0);
            malattie.Columns.Add("altro");
            malattie.Columns.Add("codice");
            malattie.Columns.Add("dataFine", GetDateTimeType());
            malattie.Columns.Add("descrizioneDe");
            malattie.Columns.Add("descrizioneIt");
            malattie.Columns.Add("numero", typeof(Decimal));
            malattie.Columns.Add("minori_Id", typeof(int));
            malattie.Columns.Add("minore_Id", typeof(int));

            DataTable scuola = new DataTable("scuola");
            expectedTables.AddTable(scuola, 0);
            scuola.Columns.Add("codice");
            scuola.Columns.Add("dataFine", GetDateTimeType());
            scuola.Columns.Add("descrizioneDe");
            scuola.Columns.Add("descrizioneIt");
            scuola.Columns.Add("email");
            scuola.Columns.Add("fax");
            scuola.Columns.Add("lingua");
            scuola.Columns.Add("telefono");
            scuola.Columns.Add("tipo");
            scuola.Columns.Add("scuola_Id", typeof(int));
            scuola.Columns.Add("minori_Id", typeof(int));
            scuola.Columns.Add("minore_Id", typeof(int));

            DataTable civico = new DataTable("civico");
            expectedTables.AddTable(civico, 0);
            civico.Columns.Add("barrato");
            civico.Columns.Add("codice");
            civico.Columns.Add("numero", typeof(int));
            civico.Columns.Add("scala");
            civico.Columns.Add("civico_Id", typeof(int));
            civico.Columns.Add("istitutoComprensivo_Id", typeof(int));
            civico.Columns.Add("scuola_Id", typeof(int));

            DataTable istitutoComprensivo = new DataTable("istitutoComprensivo");
            expectedTables.AddTable(istitutoComprensivo, 0);
            istitutoComprensivo.Columns.Add("codice");
            istitutoComprensivo.Columns.Add("dataFine", GetDateTimeType());
            istitutoComprensivo.Columns.Add("descrizioneDe");
            istitutoComprensivo.Columns.Add("descrizioneIt");
            istitutoComprensivo.Columns.Add("email");
            istitutoComprensivo.Columns.Add("fax");
            istitutoComprensivo.Columns.Add("lingua");
            istitutoComprensivo.Columns.Add("telefono");
            istitutoComprensivo.Columns.Add("tipo");
            istitutoComprensivo.Columns.Add("istitutoComprensivo_Id", typeof(int));
            istitutoComprensivo.Columns.Add("scuola_Id", typeof(int));

            DataTable turni = new DataTable("turni");
            expectedTables.AddTable(turni, 0);
            turni.Columns.Add("codice");
            turni.Columns.Add("daData", GetDateTimeType());
            turni.Columns.Add("descrizioneDe");
            turni.Columns.Add("descrizioneIt");
            turni.Columns.Add("aData", GetDateTimeType());
            turni.Columns.Add("minori_Id", typeof(int));
            turni.Columns.Add("minore_Id", typeof(int));

            DataTable potestaGenitoriale = new DataTable("potestaGenitoriale");
            expectedTables.AddTable(potestaGenitoriale, 0);
            potestaGenitoriale.Columns.Add("altro");
            potestaGenitoriale.Columns.Add("codice");
            potestaGenitoriale.Columns.Add("dataFine", GetDateTimeType());
            potestaGenitoriale.Columns.Add("descrizioneDe");
            potestaGenitoriale.Columns.Add("descrizioneIt");
            potestaGenitoriale.Columns.Add("numero", typeof(Decimal));
            potestaGenitoriale.Columns.Add("iscrizione_Id", typeof(int));

            DataTable residenza = new DataTable("residenza");
            expectedTables.AddTable(residenza, 0);
            residenza.Columns.Add("cap");
            residenza.Columns.Add("civico");
            residenza.Columns.Add("via");
            residenza.Columns.Add("residenza_Id", typeof(int));
            residenza.Columns.Add("iscrizione_Id", typeof(int));

            DataTable iscrizionePdf = new DataTable("iscrizionePdf");
            expectedTables.AddTable(iscrizionePdf, 0);
            iscrizionePdf.Columns.Add("domicilioCivico");
            iscrizionePdf.Columns.Add("domicilioViaDe");
            iscrizionePdf.Columns.Add("domicilioViaIt");
            iscrizionePdf.Columns.Add("residenzaCivico");
            iscrizionePdf.Columns.Add("residenzaViaDe");
            iscrizionePdf.Columns.Add("residenzaViaIt");
            iscrizionePdf.Columns.Add("iscrizionePdf_Id", typeof(int));

            DataTable identita = new DataTable("identita");
            expectedTables.AddTable(identita, 0);
            identita.Columns.Add("cap");
            identita.Columns.Add("cellulare");
            identita.Columns.Add("codiceFiscale");
            identita.Columns.Add("codiceIstatComune");
            identita.Columns.Add("codiceIstatComuneDiNascita");
            identita.Columns.Add("cognome");
            identita.Columns.Add("comuneDe");
            identita.Columns.Add("comuneDiNascitaDe");
            identita.Columns.Add("comuneDiNascitaIt");
            identita.Columns.Add("comuneIt");
            identita.Columns.Add("dataDiNascita", GetDateTimeType());
            identita.Columns.Add("email");
            identita.Columns.Add("linguaPreferita");
            identita.Columns.Add("localitaEstera");
            identita.Columns.Add("localitaEsteraDiNascita");
            identita.Columns.Add("nome");
            identita.Columns.Add("numeroCivico");
            identita.Columns.Add("partitaIva");
            identita.Columns.Add("provinciaDe");
            identita.Columns.Add("provinciaIt");
            identita.Columns.Add("ragioneSocialeDe");
            identita.Columns.Add("ragioneSocialeIt");
            identita.Columns.Add("sesso");
            identita.Columns.Add("statoDe");
            identita.Columns.Add("statoIt");
            identita.Columns.Add("strada");
            identita.Columns.Add("telefono");
            identita.Columns.Add("tipoPersona");
            identita.Columns.Add("utenteCollegatoCodiceFiscale");
            identita.Columns.Add("utenteCollegatoCognome");
            identita.Columns.Add("utenteCollegatoNome");
            identita.Columns.Add("iscrizionePdf_Id", typeof(int));
            identita.Columns.Add("istanzaPdf_Id", typeof(int));

            DataTable istanza = new DataTable("istanza");
            expectedTables.AddTable(istanza, 0);
            istanza.Columns.Add("iscrizionePdf_Id", typeof(int));
            istanza.Columns.Add("istanzaPdf_Id", typeof(int));

            DataTable istanzaBase = new DataTable("istanzaBase");
            expectedTables.AddTable(istanzaBase, 0);

            DataTable istanzaPdf = new DataTable("istanzaPdf");
            expectedTables.AddTable(istanzaPdf, 0);
            istanzaPdf.Columns.Add("istanzaPdf_Id", typeof(int));

            DataTable minore = new DataTable("minore");
            expectedTables.AddTable(minore, 0);
            minore.Columns.Add("assistenzaPersonale", typeof(bool));
            minore.Columns.Add("certificatoMedico", typeof(Int16));
            minore.Columns.Add("classe");
            minore.Columns.Add("codiceFiscale");
            minore.Columns.Add("cognome");
            minore.Columns.Add("dataNascita", GetDateTimeType());
            minore.Columns.Add("nome");
            minore.Columns.Add("oreAssistenza", typeof(int));
            minore.Columns.Add("minore_Id", typeof(int));

            DataTable turno = new DataTable("turno");
            expectedTables.AddTable(turno, 0);
            turno.Columns.Add("codice");
            turno.Columns.Add("daData", GetDateTimeType());
            turno.Columns.Add("descrizioneDe");
            turno.Columns.Add("descrizioneIt");
            turno.Columns.Add("aData", GetDateTimeType());

            expectedTables.AddRelation("civicoStrada_strada", civicoStrada, strada, new string[] { "civicoStrada_Id" },
                new string[] { "civicoStrada_Id" });
            expectedTables.AddRelation("comune_provincia", comune, provincia, new string[] { "comune_Id" },
                new string[] { "comune_Id" });
            expectedTables.AddRelation("indirizzo_civicoStrada", indirizzo, civicoStrada, new string[] { "indirizzo_Id" },
                new string[] { "indirizzo_Id" });
            expectedTables.AddRelation("indirizzo_comune", indirizzo, comune, new string[] { "indirizzo_Id" },
                new string[] { "indirizzo_Id" });
            expectedTables.AddRelation("domicilio_civicoStrada", domicilio, civicoStrada, new string[] { "domicilio_Id" },
                new string[] { "domicilio_Id" });
            expectedTables.AddRelation("domicilio_comune", domicilio, comune, new string[] { "domicilio_Id" },
                new string[] { "domicilio_Id" });
            expectedTables.AddRelation("civico_strada", civico, strada, new string[] { "civico_Id" }, new string[] { "civico_Id" });
            expectedTables.AddRelation("istitutoComprensivo_civico", istitutoComprensivo, civico,
                new string[] { "istitutoComprensivo_Id" }, new string[] { "istitutoComprensivo_Id" });
            expectedTables.AddRelation("scuola_civico", scuola, civico, new string[] { "scuola_Id" }, new string[] { "scuola_Id" });
            expectedTables.AddRelation("scuola_istitutoComprensivo", scuola, istitutoComprensivo, new string[] { "scuola_Id" },
                new string[] { "scuola_Id" });
            expectedTables.AddRelation("minori_dieta", minori, dieta, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation("minori_malattie", minori, malattie, new string[] { "minori_Id" },
                new string[] { "minori_Id" });
            expectedTables.AddRelation("minori_scuola", minori, scuola, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation("minori_turni", minori, turni, new string[] { "minori_Id" }, new string[] { "minori_Id" });
            expectedTables.AddRelation("residenza_civicoStrada", residenza, civicoStrada, new string[] { "residenza_Id" },
                new string[] { "residenza_Id" });
            expectedTables.AddRelation("residenza_comune", residenza, comune, new string[] { "residenza_Id" },
                new string[] { "residenza_Id" });
            expectedTables.AddRelation("iscrizione_domicilio", iscrizione, domicilio, new string[] { "iscrizione_Id" },
                new string[] { "iscrizione_Id" });
            expectedTables.AddRelation("iscrizione_minori", iscrizione, minori, new string[] { "iscrizione_Id" },
                new string[] { "iscrizione_Id" });
            expectedTables.AddRelation("iscrizione_potestaGenitoriale", iscrizione, potestaGenitoriale,
                new string[] { "iscrizione_Id" }, new string[] { "iscrizione_Id" });
            expectedTables.AddRelation("iscrizione_residenza", iscrizione, residenza, new string[] { "iscrizione_Id" },
                new string[] { "iscrizione_Id" });
            expectedTables.AddRelation("iscrizionePdf_identita", iscrizionePdf, identita, new string[] { "iscrizionePdf_Id" },
                new string[] { "iscrizionePdf_Id" });
            expectedTables.AddRelation("iscrizionePdf_istanza", iscrizionePdf, istanza, new string[] { "iscrizionePdf_Id" },
                new string[] { "iscrizionePdf_Id" });
            expectedTables.AddRelation("istanzaPdf_identita", istanzaPdf, identita, new string[] { "istanzaPdf_Id" },
                new string[] { "istanzaPdf_Id" });
            expectedTables.AddRelation("istanzaPdf_istanza", istanzaPdf, istanza, new string[] { "istanzaPdf_Id" },
                new string[] { "istanzaPdf_Id" });
            expectedTables.AddRelation("minore_dieta", minore, dieta, new string[] { "minore_Id" }, new string[] { "minore_Id" });
            expectedTables.AddRelation("minore_malattie", minore, malattie, new string[] { "minore_Id" },
                new string[] { "minore_Id" });
            expectedTables.AddRelation("minore_scuola", minore, scuola, new string[] { "minore_Id" }, new string[] { "minore_Id" });
            expectedTables.AddRelation("minore_turni", minore, turni, new string[] { "minore_Id" }, new string[] { "minore_Id" });

            if (addSelfRelation)
            {
                expectedTables.AddRelation("istitutoComprensivo_istitutoComprensivo", istitutoComprensivo,
                    istitutoComprensivo, new string[] {"istitutoComprensivo_Id"}, new string[] {"istitutoComprensivo_Id"});
            }

            return expectedTables;
        }

        public static Type GetDateTimeType()
        {
#if PLAIN_JAVA
            return java.util.Date.class;
#else
            return typeof(DateTime);
#endif
        }
    }
}
