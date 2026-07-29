// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2004 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Common;
using Aspose.Ss;
using Aspose.Ss.Property;
using Aspose.Words.Notes;
using Aspose.Words.Properties;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for built-in and custom document properties.
    /// RK When reading RTF, MS Word seems to use current culture.
    /// Therefore we need to switch to the culture in which the document was created.
    /// </summary>
    [TestFixture]
    public class TestDocProperties : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDocProperties(LoadFormat lf, SaveFormat sf)
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();  // This is really need for RTF only, but okay lets keep it for all.

                Document doc = TestUtil.OpenSaveOpen(@"Model\Property\TestDocProperties", lf, sf);

                // This property is from SummaryInfo
                Assert.That((string)doc.BuiltInDocumentProperties["Author"].Value, Is.EqualTo("Test Author"));
                // This property is from DocSummaryInfo
                Assert.That((string)doc.BuiltInDocumentProperties["Company"].Value, Is.EqualTo("Test Company"));
                // This property is from the user defined properties.
                Assert.That((string)doc.BuiltInDocumentProperties["HyperlinkBase"].Value, Is.EqualTo("http://www.example.com/"));
                // Unicode chars
                Assert.That(doc.BuiltInDocumentProperties["Title"].ToString(), Is.EqualTo("КУКУ"));

                // Test various types of custom properties
                Assert.That(doc.CustomDocumentProperties.Count, Is.EqualTo(7));
                Assert.That(doc.CustomDocumentProperties["MyString"].ToString(), Is.EqualTo("Test String"));
                Assert.That(doc.CustomDocumentProperties["MyInt"].ToInt(), Is.EqualTo(123));
                Assert.That(doc.CustomDocumentProperties["MyDouble"].ToDouble(), Is.EqualTo(123.45));
                // RK The delta here is needed only for RTF and WordML. Strange, but MS files have a difference.
                Assert.That(doc.CustomDocumentProperties["MyDoubleSmall"].ToDouble(), Is.EqualTo(3.26954133990695E-302).Within(1.0E-315));
                Assert.That(doc.CustomDocumentProperties["MyBool"].ToBool(), Is.EqualTo(true));

                DocumentProperty prop = doc.CustomDocumentProperties["MyDate"];
                DateTime d = prop.ToDateTime();
                Assert.That(d, Is.EqualTo(new DateTime(2003, 1, 31, 12, 0, 0)));
                Assert.That(prop.LinkTarget, Is.EqualTo(""));

                prop = doc.CustomDocumentProperties["MyLinked"];
                Assert.That((string)prop.Value, Is.EqualTo("Hello"));
                Assert.That(prop.LinkTarget, Is.EqualTo("bmk1"));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// A customer complained that Comments property does not work.
        ///
        /// WORDSNET-3503 WordML import cannot deal with custom property names that contain Russian or Unicode characters.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDocPropertiesModify(LoadFormat lf, SaveFormat sf)
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();  // This is really need for RTF only, but okay lets keep it for all.

                Document doc = TestUtil.Open(@"Model\Property\TestDocProperties", lf);

                // Set some built int properties.
                Assert.That((string)doc.BuiltInDocumentProperties["Comments"].Value, Is.EqualTo("Test Comments"));
                doc.BuiltInDocumentProperties.TotalEditingTime = 12;
                doc.BuiltInDocumentProperties["Comments"].Value = "New Comments";
                doc.BuiltInDocumentProperties["Manager"].Value = "New Manager";
                doc.BuiltInDocumentProperties.Template = "AAA.dot";
                doc.BuiltInDocumentProperties.HyperlinkBase = "1234";

                // Try adding custom properties.
                doc.CustomDocumentProperties.Add("Лозунг 1_&.\x0001<\\>\r", "Слава\r\n КПСС \"\'&<\\>\x0002");
                doc.CustomDocumentProperties.Add("AddedProperty2", 321);

                // Try deleting a custom property.
                Assert.That(doc.CustomDocumentProperties.Contains("MyBool"), Is.EqualTo(true));
                doc.CustomDocumentProperties.RemoveAt(doc.CustomDocumentProperties.IndexOf("MyBool"));
                Assert.That(doc.CustomDocumentProperties.Contains("MyBool"), Is.EqualTo(false));

                doc = TestUtil.SaveOpen(doc, @"Model\Property\TestDocProperties Modified", lf, sf);

                // Check built in properties.
                Assert.That(doc.BuiltInDocumentProperties.TotalEditingTime, Is.EqualTo(12));
                Assert.That((string)doc.BuiltInDocumentProperties["Comments"].Value, Is.EqualTo("New Comments"));
                Assert.That(doc.BuiltInDocumentProperties["Manager"].ToString(), Is.EqualTo("New Manager"));
                Assert.That(doc.BuiltInDocumentProperties.HyperlinkBase, Is.EqualTo("1234"));


                // Template name is not stored in WordML and RTF.
                if ((sf != SaveFormat.WordML) && (sf != SaveFormat.Rtf))
                    Assert.That(doc.BuiltInDocumentProperties.Template, Is.EqualTo("AAA.dot"));

                // Check custom properties. 7 existing + 2 new properties now there - 1 deleted.
                Assert.That(doc.CustomDocumentProperties.Count, Is.EqualTo(8));
                Assert.That(doc.CustomDocumentProperties["AddedProperty2"].ToInt(), Is.EqualTo(321));
                switch (TestUtil.GetUnifiedScenario(lf, sf))
                {
                    case UnifiedScenario.Doc2Doc:
                    case UnifiedScenario.Doc2Rtf:
                    case UnifiedScenario.Rtf2RtfNoGold:
                        // All bytes are preserved.
                        Assert.That(doc.CustomDocumentProperties["Лозунг 1_&.\x0001<\\>\r"].ToString(), Is.EqualTo("Слава\r\n КПСС \"\'&<\\>\x0002"));
                        break;
                    case UnifiedScenario.Doc2Docx:
                    case UnifiedScenario.Docx2DocxNoGold:
                        // \x0001 etc chars are stripped out in value and in name because they are written as XML text.
                        Assert.That(doc.CustomDocumentProperties["Лозунг 1_&.<\\>\r"].ToString(), Is.EqualTo("Слава\n КПСС \"\'&<\\>_x0002_"));
                        break;
                    case UnifiedScenario.Doc2Wml:
                    case UnifiedScenario.Wml2WmlNoGold:
                        // \x0001 etc chars are stripped out in value only because it is written as XML text.
                        Assert.That(doc.CustomDocumentProperties["Лозунг 1_&.\x0001<\\>\r"].ToString(), Is.EqualTo("Слава\r\n КПСС \"\'&<\\>"));
                        break;
                    default:
                        throw new InvalidOperationException("Unknown file format.");
                }
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests that when document is cloned, the properties are cloned okay.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestClone(LoadFormat lf, SaveFormat sf)
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();  // This is really need for RTF only, but okay lets keep it for all.

                Document srcDoc = TestUtil.Open(@"Model\Property\TestDocProperties", lf);
                // Check the original property is there.
                DateTime d = srcDoc.CustomDocumentProperties["MyDate"].ToDateTime();
                Assert.That(d, Is.EqualTo(new DateTime(2003, 1, 31, 12, 0, 0)));

                Document dstDoc = srcDoc.Clone();
                // Check its copied.
                d = dstDoc.CustomDocumentProperties["MyDate"].ToDateTime();
                Assert.That(d, Is.EqualTo(new DateTime(2003, 1, 31, 12, 0, 0)));
                dstDoc.CustomDocumentProperties["MyDate"].FromDateTime(new DateTime(2004, 6, 18));

                // Check the original is still original.
                d = srcDoc.CustomDocumentProperties["MyDate"].ToDateTime();
                Assert.That(d, Is.EqualTo(new DateTime(2003, 1, 31, 12, 0, 0)));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests alternate property names. New name first
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlternateNamesNewFirst(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Property\TestDocProperties", lf);

            BuiltInDocumentProperties props = doc.BuiltInDocumentProperties;
            Assert.That(props["Application Name"], Is.EqualTo(props["NameOfApplication"]));
            Assert.That(props["Last Author"], Is.EqualTo(props["LastSavedBy"]));
            Assert.That(props["Creation Date"], Is.EqualTo(props["CreateTime"]));
            Assert.That(props["Last Print Date"], Is.EqualTo(props["LastPrinted"]));
            Assert.That(props["Last Save Time"], Is.EqualTo(props["LastSavedTime"]));
        }

        /// <summary>
        /// Tests alternate property names. Old name first
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlternateNamesOldFirst(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Property\TestDocProperties", lf);

            BuiltInDocumentProperties props = doc.BuiltInDocumentProperties;
            Assert.That(props["NameOfApplication"], Is.EqualTo(props["Application Name"]));
            Assert.That(props["LastSavedBy"], Is.EqualTo(props["Last Author"]));
            Assert.That(props["CreateTime"], Is.EqualTo(props["Creation Date"]));
            Assert.That(props["LastPrinted"], Is.EqualTo(props["Last Print Date"]));
            Assert.That(props["LastSavedTime"], Is.EqualTo(props["Last Save Time"]));
        }

        /// <summary>
        /// WORDSNET-395 Custom document properties are not saved.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomPropertyLost(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            doc.CustomDocumentProperties.Add("DocumentID", "12345");

            doc = TestUtil.SaveOpen(doc, @"Model\Property\TestCustomPropertyLost", lf, sf);
            Assert.That((string)doc.CustomDocumentProperties["DocumentID"].Value, Is.EqualTo("12345"));
        }

        /// <summary>
        /// RK It is very strange, but looks a bug in MS Word 2007 SP2. If you convert this DOC to
        /// DOCX or WML using MS Word 2007 SP2, the variables will be screwed! You can check that in Visual Basic window.
        /// But Word 2007 without SP2 seems to convert them okay. So I have these old ms files and added them to SVN.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDocVariables(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Property\TestDocVariables", lf, sf);
            Assert.That(doc.Variables.Count, Is.EqualTo(2));
            Assert.That(doc.Variables["V1"], Is.EqualTo("AAA"));
            Assert.That(doc.Variables["v1"], Is.EqualTo("AAA"));
            Assert.That(doc.Variables["V2"], Is.EqualTo("123"));
        }

        [Test]
        public void TestDefaults()
        {
            Document doc = new Document();
            CheckDefaults(doc);
            doc = TestUtil.SaveOpen(doc, @"Model\Property\TestDefaults.docx");
            CheckDefaults(doc);
        }

        private static void CheckDefaults(Document doc)
        {
            Assert.That(doc.BuiltInDocumentProperties.Author, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.Category, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.Characters, Is.EqualTo(0));
            Assert.That(doc.BuiltInDocumentProperties.CharactersWithSpaces, Is.EqualTo(0));
            Assert.That(doc.BuiltInDocumentProperties.Comments, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.Company, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.CreatedTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(doc.BuiltInDocumentProperties.HyperlinkBase, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.Keywords, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.LastPrinted, Is.EqualTo(DateTime.MinValue));
            Assert.That(doc.BuiltInDocumentProperties.LastSavedBy, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.LastSavedTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(doc.BuiltInDocumentProperties.Lines, Is.EqualTo(1));
            Assert.That(doc.BuiltInDocumentProperties.Manager, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.Pages, Is.EqualTo(1));
            Assert.That(doc.BuiltInDocumentProperties.Paragraphs, Is.EqualTo(1));
            Assert.That(doc.BuiltInDocumentProperties.RevisionNumber, Is.EqualTo(1));
            Assert.That(doc.BuiltInDocumentProperties.Security, Is.EqualTo(DocumentSecurity.None));
            Assert.That(doc.BuiltInDocumentProperties.Subject, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.Template, Is.EqualTo("Normal.dot"));
            Assert.That(doc.BuiltInDocumentProperties.Title, Is.EqualTo(""));
            Assert.That(doc.BuiltInDocumentProperties.TotalEditingTime, Is.EqualTo(0));
            Assert.That(doc.BuiltInDocumentProperties.Version, Is.EqualTo(786432));
            Assert.That(doc.BuiltInDocumentProperties.Words, Is.EqualTo(0));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestSetBuiltInToNull()
        {
            Document doc = new Document();
            Assert.That(doc.BuiltInDocumentProperties.Author, Is.EqualTo(""));
            doc.BuiltInDocumentProperties.Author = null;
        }

        /// <summary>
        /// Setting null string is not allowed.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestAddNullValue()
        {
            Document doc = new Document();
            doc.CustomDocumentProperties.Add("test", (string)null); // Need to cast to disambiguate for Java.
        }

        /// <summary>
        /// Setting empty string is allowed.
        /// </summary>
        [Test]
        public void TestAddEmptyValue()
        {
            Document doc = new Document();
            doc.CustomDocumentProperties.Add("test", "");
            Assert.That((string)doc.CustomDocumentProperties["TEST"].Value, Is.EqualTo(""));
        }

        /// <summary>
        /// Adding property with null name is not allowed.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestAddNullName()
        {
            Document doc = new Document();
            doc.CustomDocumentProperties.Add(null, "test");
        }

        /// <summary>
        /// Adding property with empty name is not allowed.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestAddEmptyName()
        {
            Document doc = new Document();
            doc.CustomDocumentProperties.Add("", "test");
        }




        private static void CheckHeadingPairs(Document doc, string[] goldTitlesOfParts, object[] goldHeadingPairs)
        {
            string[] titlesOfParts = doc.BuiltInDocumentProperties.TitlesOfParts;
            Assert.That(titlesOfParts.Length, Is.EqualTo(goldTitlesOfParts.Length));
            for (int i = 0; i < titlesOfParts.Length; i++)
                Assert.That(titlesOfParts[i], Is.EqualTo(goldTitlesOfParts[i]));

            object[] headingPairs = doc.BuiltInDocumentProperties.HeadingPairs;
            Assert.That(headingPairs.Length, Is.EqualTo(goldHeadingPairs.Length));
            for (int i = 0; i < headingPairs.Length; i++)
                Assert.That(headingPairs[i], Is.EqualTo(goldHeadingPairs[i]));
        }

        /// <summary>
        /// WORDSNET-6815 White spaces at the beginning and at the end are missed from DocumentProperty value.
        /// Avoided trimming of UserDefined properties.
        /// All other properties will be trimmed. this is needed to make all gold ExportImport tests work.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWhiteSpaces(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Property\TestWhiteSpaces", lf, sf);

            Assert.That((string)doc.CustomDocumentProperties["WhiteSpaceBefore"].Value, Is.EqualTo(" Test White Spaces"));
            Assert.That((string)doc.CustomDocumentProperties["WhiteSpaceAfter"].Value, Is.EqualTo("Test White Spaces "));
            Assert.That((string)doc.CustomDocumentProperties["WhiteSpaceBeforeAndAfter"].Value, Is.EqualTo(" Test White Spaces "));

            Assert.That(doc.BuiltInDocumentProperties.Author, Is.EqualTo("alexey"));
            Assert.That(doc.BuiltInDocumentProperties.Company, Is.EqualTo("Aspose"));
        }



        /// <summary>
        /// Test Jira 3721  Standard behavior of .NET collection is to throw exception on Add operation
        /// if value being added already exists.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestJira3721Regression()
        {
            Document doc = new Document();

            doc.CustomDocumentProperties.Add("Boobi", "InitialValue");
            Assert.That(doc.CustomDocumentProperties["Boobi"].ToString(), Is.EqualTo("InitialValue"));

            // ArgumentException is thrown here.
            doc.CustomDocumentProperties.Add("Boobi", "OtherValue");
        }

        /// <summary>
        /// WORDSNET-14499 Document endnote/footnote options were not saved into Docx and Wml
        /// if there is no endnotes/footnotes.
        /// </summary>
        [TestCase(UnifiedScenario.Docx2Docx)]
        public void TestFootnoteOptions(UnifiedScenario scenario)
        {
            Document doc = new Document();

            doc.EndnoteOptions.Position = EndnotePosition.EndOfSection;
            doc.EndnoteOptions.NumberStyle = NumberStyle.ChicagoManual;
            doc.EndnoteOptions.RestartRule = FootnoteNumberingRule.RestartSection;
            doc.EndnoteOptions.StartNumber = 2;

            doc.FootnoteOptions.Position = FootnotePosition.BeneathText;
            doc.FootnoteOptions.NumberStyle = NumberStyle.UppercaseRoman;
            doc.FootnoteOptions.RestartRule = FootnoteNumberingRule.RestartPage;
            doc.FootnoteOptions.StartNumber = 3;

            doc = TestUtil.SaveOpen(doc, @"ExportDocx\TestFootnoteOptions", scenario | UnifiedScenario.NoGold);

            Assert.That(doc.EndnoteOptions.Position, Is.EqualTo(EndnotePosition.EndOfSection));
            Assert.That(doc.EndnoteOptions.NumberStyle, Is.EqualTo(NumberStyle.ChicagoManual));
            Assert.That(doc.EndnoteOptions.RestartRule, Is.EqualTo(FootnoteNumberingRule.RestartSection));
            Assert.That(doc.EndnoteOptions.StartNumber, Is.EqualTo(2));

            Assert.That(doc.FootnoteOptions.Position, Is.EqualTo(FootnotePosition.BeneathText));
            Assert.That(doc.FootnoteOptions.NumberStyle, Is.EqualTo(NumberStyle.UppercaseRoman));
            Assert.That(doc.FootnoteOptions.RestartRule, Is.EqualTo(FootnoteNumberingRule.RestartPage));
            Assert.That(doc.FootnoteOptions.StartNumber, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-19103 Add feature to link CustomDocumentProperty to Bookmark.
        /// <see cref="CustomDocumentProperties.AddLinkToContent"/> was implemented to get
        /// ability to add such linked to content properties.
        /// <see cref="DocumentProperty.LinkSource"/> and <see cref="DocumentProperty.IsLinkToContent"/> were
        /// implemented as well.
        /// </summary>
        [Test]
        public void Test19103()
        {
            // Source document contains a bookmark.
            Document doc = TestUtil.Open(@"Model\Property\Test19103.docx");

            CustomDocumentProperties customProperties = doc.CustomDocumentProperties;

            // Add linked to content property
            DocumentProperty property = customProperties.AddLinkToContent("pr1", "B1");

            Assert.That(property.Name, Is.EqualTo("pr1"));
            Assert.That(property.LinkSource, Is.EqualTo("B1"));
            Assert.That(property.IsLinkToContent, Is.True);

            // Verify no text from field code or textbox appeared.
            Assert.That(property.Value, Is.EqualTo("dddd\rtxt8/28/2019\r"));

            // Try to create link for invalid bookmark.
            Assert.That(customProperties.Count, Is.EqualTo(1));
            DocumentProperty invalidProperty = customProperties.AddLinkToContent("pr2", "nonexistentBookmark");
            Assert.That(invalidProperty, Is.Null);
            Assert.That(customProperties.Count, Is.EqualTo(1));

            // Verify roundtrip.
            string fileName = TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(@"Model\Property\Test19103.docx"), "", SaveFormat.Docx);
            doc = TestUtil.SaveOpen(doc, fileName, null, false);

            customProperties = doc.CustomDocumentProperties;
            Assert.That(customProperties.Count, Is.EqualTo(1));
            property = customProperties[0];

            Assert.That(property.Name, Is.EqualTo("pr1"));
            Assert.That(property.LinkSource, Is.EqualTo("B1"));
            Assert.That(property.IsLinkToContent, Is.True);
            Assert.That(property.Value, Is.EqualTo("dddd\rtxt8/28/2019\r"));
        }

        /// <summary>
        /// WORDSNET-19776 PrintDate field is not updated after save to PDF
        /// <see cref="SaveOptions.UpdateLastPrintedProperty"/> was implemented to get
        /// ability to update that property during save to PDF and XPS formats.
        /// </summary>
        [Test]
        [TestCase(SaveFormat.Docx, false)]
        public void Test19776(SaveFormat saveFormat, bool shouldUpdateProperty)
        {
            Document doc = new Document();

            using (MemoryStream ms = new MemoryStream())
            {
                DateTime dateBeforeSave = doc.BuiltInDocumentProperties.LastPrinted;
                doc.Save(ms, saveFormat);
                DateTime dateAfterSave = doc.BuiltInDocumentProperties.LastPrinted;
                Assert.That(dateAfterSave.Equals(dateBeforeSave), IsNot.EqualTo(shouldUpdateProperty));
            }
        }

        /// <summary>
        /// WORDSNET-11848 Add a saveoptions either to mimics MS Word behavior or not for created, modified and printed dates
        /// A new <see cref="SaveOptions.UpdateCreatedTimeProperty"/> was implemented to get
        /// ability to update that property on save.
        /// </summary>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Test11848(bool shouldUpdateProperty)
        {
            Document doc = new Document();

            // For test mode we need date not equal to UnitTestingDateTime
            DateTime dateBeforeSave = DateTimeUtil.GetNow().AddDays(-1);
            doc.BuiltInDocumentProperties.CreatedTime = dateBeforeSave;

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            so.UpdateCreatedTimeProperty = shouldUpdateProperty;

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms, so);
                Assert.That(dateBeforeSave.Equals(doc.BuiltInDocumentProperties.CreatedTime), IsNot.EqualTo(shouldUpdateProperty));
            }
        }

        /// <summary>
        /// WORDSNET-22135 Line break is removed from Variable value.
        /// Fixed by encoding the correct writing of the Line Break character for the 'w:docVar' element.
        /// </summary>
        [Test]
        public void Test22135()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Property\Test22135", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Variables[0], Is.EqualTo("Value with\vline break"));
        }

        private static void AssertLinkedDocumentProperties(Document doc, SaveFormat sf)
        {
            Dictionary<string, string> nameToLinkTargetMap = new Dictionary<string, string>();
            nameToLinkTargetMap.Add("Document number", "Number");
            nameToLinkTargetMap.Add("RevisionInfo", "Version");
            nameToLinkTargetMap.Add("lifeCycleState", "State");
            nameToLinkTargetMap.Add("test", "testBM");
            nameToLinkTargetMap.Add("wtname", "Name");

            foreach (DocumentProperty property in doc.CustomDocumentProperties)
            {
                if (!property.IsLinkToContent)
                    continue;

                string propName = property.Name;
                Assert.That(property.LinkTarget, Is.EqualTo(nameToLinkTargetMap[propName]), "Linked target value is wrong for property: " + propName);

                if (property.Type != PropertyType.String)
                {
                    Assert.That(property.Type, Is.EqualTo(sf == SaveFormat.WordML ? PropertyType.Number : PropertyType.Double), "Property type mismatch: " + propName);
                }
            }
        }


        /// <summary>
        /// Tests reading properties values. All properties were set to false.
        /// </summary>
        /// <remarks>
        /// Seems only DOC/DOCX document formats support these properties.
        /// </remarks>
        [TestCase(LoadFormat.Docx)]
        public void Test27154A(LoadFormat lf)
        {
            Document doc = TestUtil.Open(@"Model\Property\Test27154A", lf);
            Assert.That(doc.BuiltInDocumentProperties.ScaleCrop, Is.False);
            Assert.That(doc.BuiltInDocumentProperties.SharedDocument, Is.False);
            Assert.That(doc.BuiltInDocumentProperties.HyperlinksChanged, Is.False);
        }

        private static void TestMetadata(string fileName)
        {
            FileSystem fs = new FileSystem(fileName);
            Stream srcStream = fs.Root["\x0005DocumentSummaryInformation"] as Stream;
            PropertySet ps = new PropertySet(srcStream);

            MemoryStream dstStream = new MemoryStream();
            ps.Save(dstStream);
            fs.Root["\x0005DocumentSummaryInformation"] = dstStream;
            PropertySetSection section = ps.Sections[0];

            Property scaleCrop = section.Properties.GetById(DocSummaryInfoProperty.ScaleCrop);
            Assert.That(scaleCrop, IsNot.Null());
            Assert.That((bool)scaleCrop.Value, Is.EqualTo(false));
        }

    }
}
