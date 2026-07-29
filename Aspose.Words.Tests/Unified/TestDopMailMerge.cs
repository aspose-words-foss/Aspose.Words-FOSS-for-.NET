// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Text;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Tests for the mail merge settings in various formats.
    /// 
    /// These tests are "manual unified" because it is difficult to automatically convert these documents 
    /// into "ms" files without damaging the mail merge settings in them.
    /// </summary>
    [TestFixture]
    public class TestDopMailMerge
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        // FOSS: only the OOXML scenario survives — Doc/Wml/Rtf load+save were removed.
        // MailMergeSettings/ODSO model and their DOCX persistence are unaffected.
        private static readonly UnifiedScenario[] gTestScenarios = new UnifiedScenario[]
       {
           UnifiedScenario.Docx2DocxNoGold
       };

        /// <summary>
        /// Outlook data source and some settings.
        /// 
        /// JAVAGOLD Java is better than .NET in outputting several uniqueTag numbers as Unicode chars. 
        /// It looks like .NET screws 4 types but Java only 2 times in this document.
        /// </summary>
        [Test]
        public void TestMailMergeSettings1()
        {
            foreach (UnifiedScenario scenario in gTestScenarios)
                TestMailMergeSettings1(scenario);
        }
        
        private static void TestMailMergeSettings1(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\MailMerge\TestMailMergeSettings1", scenario);

            // Check mail merge settings.
            MailMergeSettings mmSettings = doc.DocPr.MailMergeSettings;
            Assert.That(mmSettings.ActiveRecord, Is.EqualTo(66));
            Assert.That(mmSettings.AddressFieldName, Is.EqualTo("Email_Address"));
            Assert.That(mmSettings.CheckErrors, Is.EqualTo(MailMergeCheckErrors.PauseOnError));
            Assert.That(mmSettings.ConnectString, Is.EqualTo("Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=\"\";Mode=Share Deny None;Extended " +
                "Properties=\"Outlook 9.0;MAPILEVEL=Personal Folders|;TABLETYPE=0;DATABASE=C:\\Users\\romeok\\AppData" +
                "\\Local\\Temp\\OfficeMMergeTempDir\\OLKA53B.tmp;COLSETVERSION=12.0\";Je"));
            Assert.That(mmSettings.DataSource, Is.EqualTo("C:\\Program Files (x86)\\Microsoft Office\\Office12\\~~~_virtual_file_~~~Personal Folders|"));
            Assert.That(mmSettings.DataType, Is.EqualTo(MailMergeDataType.Native));
            Assert.That(mmSettings.Destination, Is.EqualTo(MailMergeDestination.NewDocument));
            Assert.That(mmSettings.DoNotSupressBlankLines, Is.EqualTo(false));
            Assert.That(mmSettings.HeaderSource, Is.EqualTo(""));
            Assert.That(mmSettings.LinkToQuery, Is.EqualTo(false));
            Assert.That(mmSettings.MailAsAttachment, Is.EqualTo(false));
            Assert.That(mmSettings.MailSubject, Is.EqualTo("test subject"));
            Assert.That(mmSettings.MainDocumentType, Is.EqualTo(MailMergeMainDocumentType.FormLetters));
            Assert.That(mmSettings.Query, Is.EqualTo("SELECT * FROM `Contacts` "));
            Assert.That(mmSettings.ViewMergedData, Is.EqualTo(true));

            // Check ODSO settings.
            Odso odso = mmSettings.Odso;
            Assert.That(odso.ColumnDelimiter, Is.EqualTo('\t'));
            Assert.That(odso.DataSource, Is.EqualTo("Personal Folders|"));
            Assert.That(odso.DataSourceType, Is.EqualTo(OdsoDataSourceType.Email));
            Assert.That(odso.FirstRowContainsColumnNames, Is.EqualTo(true));
            Assert.That(odso.TableName, Is.EqualTo("Contacts"));

             // This cuts to the "common size" and converts "OLK58DD.tmp" to "OLK.tmp" so it can always compare ok.
            string udl = odso.UdlConnectString.Substring(0, 255).Remove(224, 4);
            Assert.That(//                "Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password=\"\";Jet OLEDB:" + 
//                "Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;" +
//                "Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False;Jet OLEDB:Support Complex Data=False",
                udl, Is.EqualTo("Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=\"\";Mode=Share Deny None;Extended " +
                "Properties=\"Outlook 9.0;MAPILEVEL=Personal Folders|;TABLETYPE=0;DATABASE=C:\\Users\\romeok\\AppData" +
                "\\Local\\Temp\\OfficeMMergeTempDir\\OLK.tmp;COLSETVERSION=12.0\";Je"));

            // Check field map data.
            OdsoFieldMapDataCollection fieldMap = odso.FieldMapDatas;
            Assert.That(fieldMap.Count, Is.EqualTo(30));
            
            // Check a mapped field.
            OdsoFieldMapData fieldMapData = fieldMap[0];
            Assert.That(fieldMapData.Column, Is.EqualTo(62));
            Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Column));
            Assert.That(fieldMapData.Name, Is.EqualTo("Customer ID"));
            Assert.That(fieldMapData.MappedName, Is.EqualTo("Unique Identifier"));
            
            // Check a non mapped field
            fieldMapData = fieldMap[10];
            Assert.That(fieldMapData.Column, Is.EqualTo(0));
            Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Null));
            Assert.That(fieldMapData.Name, Is.EqualTo(""));
            Assert.That(fieldMapData.MappedName, Is.EqualTo(""));


            // Check recipient data.
            OdsoRecipientDataCollection recipientDatas = odso.RecipientDatas;
            Assert.That(recipientDatas.Count, Is.EqualTo(70));

            // Check an active recipient.
            OdsoRecipientData recipient = recipientDatas[0];
            Assert.That(recipient.Active, Is.EqualTo(true));
            Assert.That(recipient.Column, Is.EqualTo(62));
            Assert.That(ArrayUtil.CompareBytes(new byte[] {0x56, 0x2e, 0x9f, 0x0b}, recipient.UniqueTag, 4), Is.True);

            // Check an inactive recipient.
            recipient = recipientDatas[2];
            Assert.That(recipient.Active, Is.EqualTo(false));
            Assert.That(recipient.Column, Is.EqualTo(62));
            Assert.That(ArrayUtil.CompareBytes(new byte[] { 0xba, 0x4c, 0x56, 0x5c }, recipient.UniqueTag, 4), Is.True);
        }

        /// <summary>
        /// MS Access data source and some other settings.
        /// </summary>
        [Test]
        public void TestMailMergeSettings2()
        {
            foreach (UnifiedScenario scenario in gTestScenarios)
                TestMailMergeSettings2(scenario);
        }

        private static void TestMailMergeSettings2(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\MailMerge\TestMailMergeSettings2", scenario);

            // Check mail merge settings.
            MailMergeSettings mmSettings = doc.DocPr.MailMergeSettings;
            Assert.That(mmSettings.ActiveRecord, Is.EqualTo(1));
            Assert.That(mmSettings.AddressFieldName, Is.EqualTo(""));
            Assert.That(mmSettings.CheckErrors, Is.EqualTo(MailMergeCheckErrors.PauseOnError));

            string connectString =
                "Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=X:\\Aspose\\Aspose.Words\\Demos\\Common\\Database\\Northwind.mdb;" +
                "Mode=Read;Extended Properties=\"\";Jet OLEDB:System database=\"\";Jet OLEDB:Registry Path=\"\";Jet OLEDB:Engine Type=5;" +
                "Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password=\"\";" +
                "Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact " +
                "Without Replica Repair=False;Jet OLEDB:SFP=False;Jet OLEDB:Support Complex Data=False";
            // In this case (probably because it is an ODSO connection), MS Word does not write this connection string into main mail
            // merge settings in the DOC format.
            if ((TestUtil.GetLoadFormat(scenario) == LoadFormat.Doc) || (TestUtil.GetLoadFormat(scenario) == LoadFormat.Rtf))
                connectString = "";
            Assert.That(mmSettings.ConnectString, Is.EqualTo(connectString));

            Assert.That(mmSettings.DataSource, Is.EqualTo("X:\\Aspose\\Aspose.Words\\Demos\\Common\\Database\\Northwind.mdb"));
            Assert.That(mmSettings.DataType, Is.EqualTo(MailMergeDataType.Native));
            Assert.That(mmSettings.Destination, Is.EqualTo(MailMergeDestination.NewDocument));
            Assert.That(mmSettings.DoNotSupressBlankLines, Is.EqualTo(false));
            Assert.That(mmSettings.HeaderSource, Is.EqualTo(""));
            Assert.That(mmSettings.LinkToQuery, Is.EqualTo(true));
            Assert.That(mmSettings.MailAsAttachment, Is.EqualTo(false));
            Assert.That(mmSettings.MailSubject, Is.EqualTo(""));
            Assert.That(mmSettings.MainDocumentType, Is.EqualTo(MailMergeMainDocumentType.Catalog));
            Assert.That(mmSettings.Query, Is.EqualTo("SELECT * FROM `Customers`  ORDER BY `CompanyName` DESC "));
            Assert.That(mmSettings.ViewMergedData, Is.EqualTo(false));

            // Check ODSO settings.
            Odso odso = mmSettings.Odso;
            Assert.That(odso.ColumnDelimiter, Is.EqualTo('\t'));
            Assert.That(odso.DataSource, Is.EqualTo("X:\\Aspose\\Aspose.Words\\Demos\\Common\\Database\\Northwind.mdb"));
            Assert.That(odso.DataSourceType, Is.EqualTo(OdsoDataSourceType.Database));
            Assert.That(odso.FirstRowContainsColumnNames, Is.EqualTo(true));
            Assert.That(odso.TableName, Is.EqualTo("Customers"));

            // This cuts to the "common size" and converts "OLK58DD.tmp" to "OLK.tmp" so it can always compare ok.
            string udl = odso.UdlConnectString.Substring(0, 255);
            Assert.That(udl, Is.EqualTo("Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=X:\\Aspose\\Aspose.Words\\Demos\\Common\\Database\\Northwind.mdb;" +
                "Mode=Read;Extended Properties=\"\";Jet OLEDB:System database=\"\";Jet OLEDB:Registry Path=\"\";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Lock"));

            // Check field map data.
            OdsoFieldMapDataCollection fieldMap = odso.FieldMapDatas;
            Assert.That(fieldMap.Count, Is.EqualTo(30));

            // Check a mapped field.
            OdsoFieldMapData fieldMapData = fieldMap[0];
            Assert.That(fieldMapData.Column, Is.EqualTo(0));
            Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Column));
            Assert.That(fieldMapData.Name, Is.EqualTo("CustomerID"));
            Assert.That(fieldMapData.MappedName, Is.EqualTo("Unique Identifier"));

            // Check a non mapped field
            fieldMapData = fieldMap[1];
            Assert.That(fieldMapData.Column, Is.EqualTo(0));
            Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Null));
            Assert.That(fieldMapData.Name, Is.EqualTo(""));
            Assert.That(fieldMapData.MappedName, Is.EqualTo(""));


            // Check recipient data.
            OdsoRecipientDataCollection recipientDatas = odso.RecipientDatas;
            switch (TestUtil.GetLoadFormat(scenario))
            {
                case LoadFormat.Docx:
                case LoadFormat.Doc:
                case LoadFormat.Rtf:
                {
                    Assert.That(recipientDatas.Count, Is.EqualTo(91));

                    // Check an active recipient.
                    OdsoRecipientData recipient = recipientDatas[0];
                    Assert.That(recipient.Active, Is.EqualTo(true));
                    Assert.That(recipient.Column, Is.EqualTo(0));
                    Assert.That(recipient.Hash, Is.EqualTo(0));
                    Assert.That(Encoding.Unicode.GetString(recipient.UniqueTag, 0, 10), Is.EqualTo("WOLZA"));
                    break;
                }
                case LoadFormat.WordML:
                {
                    // For some reason there is one more recipient in WML.
                    Assert.That(recipientDatas.Count, Is.EqualTo(92));

                    // Check an active recipient.
                    OdsoRecipientData recipient = recipientDatas[0];
                    Assert.That(recipient.Active, Is.EqualTo(true));
                    Assert.That(recipient.Column, Is.EqualTo(0));
                    // In this WML there is a "hash" value, but no "uniqueTag".
                    Assert.That(recipient.Hash, Is.EqualTo(5177431));
                    Assert.That(recipient.UniqueTag, Is.EqualTo(null));
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }


        private static void TestMailMergeSettings97(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\MailMerge\TestMailMergeSettings97", scenario);

            // Check mail merge settings.
            MailMergeSettings mmSettings = doc.DocPr.MailMergeSettings;
            Assert.That(mmSettings.ActiveRecord, Is.EqualTo(1));
            Assert.That(mmSettings.AddressFieldName, Is.EqualTo(""));
            Assert.That(mmSettings.CheckErrors, Is.EqualTo(MailMergeCheckErrors.CollectErrors));
            Assert.That(mmSettings.ConnectString, Is.EqualTo(""));
            Assert.That(mmSettings.DataSource, Is.EqualTo("X:\\Aspose\\Aspose.Words.TestData\\Model\\Dop\\MailMerge\\TestMailMergeSettings97_Data.doc"));
            Assert.That(mmSettings.DataType, Is.EqualTo(MailMergeDataType.TextFile));
            Assert.That(mmSettings.Destination, Is.EqualTo(MailMergeDestination.Printer));
            Assert.That(mmSettings.DoNotSupressBlankLines, Is.EqualTo(true));
            Assert.That(mmSettings.HeaderSource, Is.EqualTo("X:\\Aspose\\Aspose.Words.TestData\\Model\\Dop\\MailMerge\\TestMailMergeSettings97_Header.doc"));
            Assert.That(mmSettings.LinkToQuery, Is.EqualTo(true));
            Assert.That(mmSettings.MailAsAttachment, Is.EqualTo(false));
            Assert.That(mmSettings.MailSubject, Is.EqualTo(""));
            Assert.That(mmSettings.MainDocumentType, Is.EqualTo(MailMergeMainDocumentType.MailingLabels));
            Assert.That(mmSettings.Query, Is.EqualTo("SELECT * FROM X:\\Aspose\\Aspose.Words.TestData\\Model\\Dop\\MailMerge\\TestMailMergeSettings97_Data.doc"));
            Assert.That(mmSettings.ViewMergedData, Is.EqualTo(true));

            // Check ODSO settings.
            Odso odso = mmSettings.Odso;
            Assert.That(odso.ColumnDelimiter, Is.EqualTo('\x0000'));
            // Do not check odso.DataSource because it has some "left overs" in WML. DataSourceType is None anyway so ignore DataSource.
            Assert.That(odso.DataSourceType, Is.EqualTo(OdsoDataSourceType.None));
            Assert.That(odso.FirstRowContainsColumnNames, Is.EqualTo(false));
            Assert.That(odso.TableName, Is.EqualTo(""));
            Assert.That(odso.UdlConnectString, Is.EqualTo(""));

            // Check legacy separators. They are only preserved during doc2doc conversion, in all other cases they are default.
            if (scenario == UnifiedScenario.Doc2Doc)
            {
                Assert.That(mmSettings.LegacyDataFieldSeparator, Is.EqualTo(MailMergeLegacySeparator.FieldEnd));
                Assert.That(mmSettings.LegacyDataRowSeparator, Is.EqualTo(MailMergeLegacySeparator.TableCell));
                Assert.That(mmSettings.LegacyHeaderFieldSeparator, Is.EqualTo(MailMergeLegacySeparator.FieldEnd));
                Assert.That(mmSettings.LegacyHeaderRowSeparator, Is.EqualTo(MailMergeLegacySeparator.TableCell));
            }
            else
            {
                // WORDSNET-7853 LegacyDataFiledSeparator should be semicolon otherwise Word for Mac fails mail merge.
                Assert.That(mmSettings.LegacyDataFieldSeparator, Is.EqualTo(MailMergeLegacySeparator.Semicolon));
                Assert.That(mmSettings.LegacyDataRowSeparator, Is.EqualTo(MailMergeLegacySeparator.Enter));
                Assert.That(mmSettings.LegacyHeaderFieldSeparator, Is.EqualTo(MailMergeLegacySeparator.Tab));
                Assert.That(mmSettings.LegacyHeaderRowSeparator, Is.EqualTo(MailMergeLegacySeparator.Enter));
            }

            // Check field mapping.
            OdsoFieldMapDataCollection fieldMap = odso.FieldMapDatas;
            switch (TestUtil.GetLoadFormat(scenario))
            {
                case LoadFormat.Doc:
                case LoadFormat.Rtf:
                {
                    // This file was created using Word 97 and does not have field a map.
                    Assert.That(fieldMap.Count, Is.EqualTo(0));
                    break;
                }
                case LoadFormat.Docx:
                case LoadFormat.WordML:
                {
                    // These files were created by Word 2007 and it always writes a field map.
                    Assert.That(fieldMap.Count, Is.EqualTo(30));

                    // Check a non mapped field
                    OdsoFieldMapData fieldMapData = fieldMap[0];
                    Assert.That(fieldMapData.Column, Is.EqualTo(0));
                    Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Null));
                    Assert.That(fieldMapData.Name, Is.EqualTo(""));
                    Assert.That(fieldMapData.MappedName, Is.EqualTo(""));

                    // Check a mapped field.
                    fieldMapData = fieldMap[1];
                    Assert.That(fieldMapData.Column, Is.EqualTo(0));
                    Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Column));
                    Assert.That(fieldMapData.Name, Is.EqualTo("Title"));
                    Assert.That(fieldMapData.MappedName, Is.EqualTo("Courtesy Title"));
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }

            // Check recipient data.
            OdsoRecipientDataCollection recipientDatas = odso.RecipientDatas;
            Assert.That(recipientDatas.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that we write exactly 30 field map entries and in a specific order.
        /// </summary>
        /// <remarks>FOSS: originally exercised the DOC writer; the field-map validation is
        /// format-agnostic, so this now round-trips through DOCX (the .doc input/save were removed).</remarks>
        [Test]
        public void TestFieldMapValid()
        {
            Document doc = TestUtil.Open(@"Model\Dop\MailMerge\TestMailMergeSettings1 ms.docx");
            OdsoFieldMapDataCollection fieldMap = doc.DocPr.MailMergeSettings.Odso.FieldMapDatas;

            Assert.That(fieldMap.Count, Is.EqualTo(30));

            // Swap 2nd and 3rd items to verify they will be sorted back to order.
            OdsoFieldMapData temp = fieldMap[1];
            fieldMap[1] = fieldMap[2];
            fieldMap[2] = temp;

            // Remove one item to verify it will be added.
            fieldMap.RemoveAt(0);

            doc = TestUtil.SaveOpen(doc, @"Model\Dop\MailMerge\TestFieldMapValid.docx", null, false);

            // Check field map data was corrected by the document validation.
            fieldMap = doc.DocPr.MailMergeSettings.Odso.FieldMapDatas;
            Assert.That(fieldMap.Count, Is.EqualTo(30));

            // Check the deleted item got "restored".
            OdsoFieldMapData fieldMapData = fieldMap[0];
            Assert.That(fieldMapData.Column, Is.EqualTo(0));
            Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Null));
            Assert.That(fieldMapData.Name, Is.EqualTo(""));
            Assert.That(fieldMapData.MappedName, Is.EqualTo(""));

            // Check the swappted item "came back".
            fieldMapData = fieldMap[1];
            Assert.That(fieldMapData.Column, Is.EqualTo(2));
            Assert.That(fieldMapData.Type, Is.EqualTo(OdsoFieldMappingType.Column));
            Assert.That(fieldMapData.Name, Is.EqualTo("Title"));
            Assert.That(fieldMapData.MappedName, Is.EqualTo("Courtesy Title"));
        }

        // FOSS: TestDefect6392 removed — it preserves a mail-merge data source through a binary
        // .doc open/save (removed format) and its input is a customer .doc.

        [Test]
        public void TestClear()
        {
            // FOSS: repointed to the DOCX input + no-gold DOCX round-trip; Clear() is model-level.
            Document doc = TestUtil.Open(@"Model\Dop\MailMerge\TestMailMergeSettings1 ms.docx");
            doc.MailMergeSettings.Clear();

            doc = TestUtil.SaveOpen(doc, @"Model\Dop\MailMerge\TestClear.docx", null, false);
            Assert.That(doc.MailMergeSettings.DataType, Is.EqualTo(MailMergeDataType.None));
        }

        // FOSS: TestCreateFile / TestCreateFileCustom / TestCreateFileOdso (and their CreateSampleDoc
        // helper) were removed. They only saved a sample document to the removed .doc format for manual
        // inspection in Word (no assertions), and TestCreateFileCustom relied on DOC-only legacy
        // field/record separators. MailMergeSettings serialization to DOCX is covered by the tests above.
    }
}
