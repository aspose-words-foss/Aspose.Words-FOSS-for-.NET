// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Aspose.Common;
using Aspose.TestFx;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests how the document information fields work.
    /// </summary>
    [TestFixture]
    public class TestDocumentInformation : TestFieldsBase
    {
        /// <summary>
        /// Test how the DOCPROPERTY field is updated.
        /// </summary>
        [Test]
        public void TestDocProperty()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Fields\DocumentInformation\TestDocProperty.docx");

            doc.CustomDocumentProperties["propS"].FromString("aaa");
            doc.CustomDocumentProperties["propInt"].FromInt(999);
            doc.CustomDocumentProperties["prop Bool"].FromBool(false);
            doc.CustomDocumentProperties["propDate"].FromDateTime(new DateTime(2005, 6, 1));
            doc.BuiltInDocumentProperties["Author"].FromString("New Author");
            doc.BuiltInDocumentProperties["Title"].FromString("New Title");

            doc.UpdateFields();
            doc = TestUtil.SaveOpen(doc, @"Fields\DocumentInformation\TestDocProperty Modified.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[0].Result, Is.EqualTo("aaa"));
            Assert.That(fields[1].Result, Is.EqualTo("999"));
            Assert.That(fields[2].Result, Is.EqualTo("N"));
            Assert.That(fields[3].Result, Is.EqualTo("1/06/2005"));
            Assert.That(fields[4].Result, Is.EqualTo("New Author"));
            Assert.That(fields[5].Result, Is.EqualTo("New Title"));
        }

        /// <summary>
        /// WORDSNET-602 Throws when setting result of a DOCPROPERTY field that does not have a separator.
        /// </summary>
        [Test]
        public void TestDocPropertyNoSeparator()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Fields\DocumentInformation\TestDocPropertyNoSeparator.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[0].Separator, IsNot.Null());
            Assert.That(fields[0].Result, Is.EqualTo(""));
            fields[0].Result = "test";
            Assert.That(fields[0].Separator, IsNot.Null());
            Assert.That(fields[0].Result, Is.EqualTo("test"));
        }



        /// <summary>
        /// WORDSNET-4290 DOCPROPERTY fields related to DateTime are not updated correctly.
        /// </summary>
        /// <remarks>
        /// It is unable to test weather value of the date/time document property is converted from UTC to
        /// local time value during forming of field result, since test mode timezone is always UTC + 0.
        /// So we rely on DateTimeUtil.ToLocalTime implementation out of test mode.
        /// </remarks>
        [Test, Category(SingleThreadTestGroup.SingleThread)]
        public void TestJira4290()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestJira4290.docx");

            try
            {
                // Common date format.
                // Common time format.
                TestJira4290Case(
                    "d/MM/yyyy",
                    "H:mm:ss",
                    ":",
                    doc,
                    new string[] { "8/06/2012 16:51", "08/06/2012 16:51:00", "17/06/2010", "17/06/2010 00:00:00" });

                // Date format with "yy" year format. It must be converted to "yyyy".
                // Time format with short hour. PM values are not distinguished from AM.
                TestJira4290Case(
                    "d/MM/yy",
                    "h:mm:ss",
                    ":",
                    doc,
                    new string[] { "8/06/2012 4:51", "08/06/2012 04:51:00", "17/06/2010", "17/06/2010 00:00:00" });

                // Date format with year designator enclosed by single quotes. Enclosed year designator must not be converted.
                // Time format with hour designator enclosed by single quotes. Not enclosed hour designator must be used.
                string[] expectedResults = { "8/06/2012y. 16:51", "08/06/2012 16:51:00", "17/06/2010y.", "17/06/2010 00:00:00" };
                TestJira4290Case(
                    "d/MM/yyyy'y.'",
                    "'h: 'HH' m: 'mm",
                    " m: ",
                    doc,
                    expectedResults);

                // Date format with "y" year format. It must be converted to "yy".
                // Time format without hour designator. "H" must be used by default.
                TestJira4290Case(
                    "d/MM/y",
                    "mm:ss",
                    ":",
                    doc,
                    new string[] { "8/06/12 16:51", "08/06/2012 16:51:00", "17/06/10", "17/06/2010 00:00:00" });

                // Date format with inversed date part order.
                // Time format with one character separator other than ":". Specified separator must be used.
                TestJira4290Case(
                    "yyyy-MM-dd",
                    "hh.mm.ss",
                    ".",
                    doc,
                    new string[] { "2012-06-08 04.51", "08/06/2012 04:51:00", "2010-06-17", "17/06/2010 00:00:00" });

                // Date format with day and month replaced by each other.
                // Time format with more than one character separator. ":" must be used by default.
                TestJira4290Case(
                    "M/dd/yyyy",
                    "hh--mm--ss",
                    "--",
                    doc,
                    new string[] { "6/08/2012 04:51", "08/06/2012 04:51:00", "6/17/2010", "17/06/2010 00:00:00" });

                // Common date format.
                // Time format without time separator. ":" must be used by default.
                TestJira4290Case(
                    "d/MM/yyyy",
                    "hhmmss",
                    "",
                    doc,
                    new string[] { "8/06/2012 04:51", "08/06/2012 04:51:00", "17/06/2010", "17/06/2010 00:00:00" });

                // Common date format.
                // Time format with short minute. "mm" must be used anyway.
                TestJira4290Case(
                    "d/MM/yyyy",
                    "h:m:s",
                    ":",
                    doc,
                    new string[] { "8/06/2012 4:51", "08/06/2012 04:51:00", "17/06/2010", "17/06/2010 00:00:00" });

                // Common date format.
                // Time format without minute designator. "mm" must be used anyway.
                TestJira4290Case(
                    "d/MM/yyyy",
                    "h:s",
                    ":",
                    doc,
                    new string[] { "8/06/2012 4:51", "08/06/2012 04:51:00", "17/06/2010", "17/06/2010 00:00:00" });
            }
            finally
            {
                FormatterPal.SetInitialCurrentDateTimeFormatInfo();
            }
        }

        /// <summary>
        /// Performs Jira 4290 single case testing.
        /// </summary>
        /// <param name="shortDatePattern">case short date pattern</param>
        /// <param name="longTimePattern">case long time pattern</param>
        /// <param name="timeSeparator">case time separator</param>
        /// <param name="doc">document object</param>
        /// <param name="expectedResults">four-item array of expected field results</param>
        private static void TestJira4290Case(
            string shortDatePattern,
            string longTimePattern,
            string timeSeparator,
            Document doc,
            string[] expectedResults)
        {
            try
            {
                FormatterPal.SetCurrentNativeShortDatePattern(shortDatePattern);
                FormatterPal.SetCurrentNativeLongTimePattern(longTimePattern);
                FormatterPal.SetCurrentTimeSeparator(timeSeparator);

                doc.UpdateFields();

                IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
                int index = 0;

                foreach (Field field in fields)
                {
                    string message = string.Format(
                        "Date format: {0}; time format: {1}; field code: {2}.",
                        shortDatePattern,
                        longTimePattern,
                        field.GetFieldCode());

                    Assert.That(field.Result, Is.EqualTo(expectedResults[index++]), message);
                }
            }
            finally
            {
                FormatterPal.SetInitialCurrentDateTimeFormatInfo();
            }
        }

        /// <summary>
        /// Test how the AUTHOR field is updated.
        /// </summary>
        [Test]
        public void TestAuthor()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestAuthor.docx");
            Assert.That(doc.BuiltInDocumentProperties.Author, Is.EqualTo("Dmitry"));

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("Dmitry"));
            Assert.That(doc.BuiltInDocumentProperties.Author, Is.EqualTo("Dmitry"));

            // The second field updates the value and displays it.
            fields[1].Update();
            Assert.That(fields[1].Result, Is.EqualTo("New Author"));
            Assert.That(doc.BuiltInDocumentProperties.Author, Is.EqualTo("New Author"));

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("New Author"));
        }

        /// <summary>
        /// Test how the COMMENTS field is updated.
        /// </summary>
        [Test]
        public void TestComments()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestComments.docx");
            Assert.That(doc.BuiltInDocumentProperties.Comments, Is.EqualTo("My comments"));

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("My comments"));
            Assert.That(doc.BuiltInDocumentProperties.Comments, Is.EqualTo("My comments"));

            // The second field updates the value and displays it.
            fields[1].Update();
            Assert.That(fields[1].Result, Is.EqualTo("New comments"));
            Assert.That(doc.BuiltInDocumentProperties.Comments, Is.EqualTo("New comments"));

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("New comments"));
        }

        /// <summary>
        /// Test how the KEYWORDS field is updated.
        /// </summary>
        [Test]
        public void TestKeywords()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestKeywords.docx");
            Assert.That(doc.BuiltInDocumentProperties.Keywords, Is.EqualTo("My keywords"));

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("My keywords"));
            Assert.That(doc.BuiltInDocumentProperties.Keywords, Is.EqualTo("My keywords"));

            // The second field updates the value and displays it.
            fields[1].Update();
            Assert.That(fields[1].Result, Is.EqualTo("New keywords"));
            Assert.That(doc.BuiltInDocumentProperties.Keywords, Is.EqualTo("New keywords"));

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("New keywords"));
        }

        /// <summary>
        /// Test how the SUBJECT field is updated.
        /// </summary>
        [Test]
        public void TestSubject()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestSubject.docx");
            Assert.That(doc.BuiltInDocumentProperties.Subject, Is.EqualTo("My subject"));

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("My subject"));
            Assert.That(doc.BuiltInDocumentProperties.Subject, Is.EqualTo("My subject"));

            // The second field updates the value and displays it.
            fields[1].Update();
            Assert.That(fields[1].Result, Is.EqualTo("New subject"));
            Assert.That(doc.BuiltInDocumentProperties.Subject, Is.EqualTo("New subject"));

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("New subject"));
        }

        /// <summary>
        /// Test how the TITLE field is updated.
        /// </summary>
        [Test]
        public void TestTitle()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestTitle.docx");
            Assert.That(doc.BuiltInDocumentProperties.Title, Is.EqualTo("My Title"));

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("My Title"));
            Assert.That(doc.BuiltInDocumentProperties.Title, Is.EqualTo("My Title"));

            // The second field updates the value and displays it.
            fields[1].Update();
            Assert.That(fields[1].Result, Is.EqualTo("New Title"));
            Assert.That(doc.BuiltInDocumentProperties.Title, Is.EqualTo("New Title"));

            // The first field displays current property value.
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("New Title"));
        }

        /// <summary>
        /// Test how the LASTSAVEDBY field is updated.
        /// </summary>
        [Test]
        public void TestLastSavedBy()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestLastSavedBy.docx");
            Assert.That(doc.BuiltInDocumentProperties.LastSavedBy, Is.EqualTo("Dmitry"));

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("Dmitry"));

            doc.BuiltInDocumentProperties.LastSavedBy = "Someone else";
            fields[0].Update();
            Assert.That(fields[0].Result, Is.EqualTo("Someone else"));
        }




        /// <summary>
        /// Test how the FILENAME field is updated.
        /// </summary>
        [Test]
        public void TestFileName()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestFileName.docx");
            Assert.That(doc.OriginalFileName, Is.EqualTo(TestEnvironment.GetTestData("Fields\\DocumentInformation\\TestFileName.docx")));

            doc.UpdateFields();

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[0].Result, Is.EqualTo("TestFileName.docx"));
            Assert.That(fields[1].Result, Is.EqualTo(TestEnvironment.GetTestData("Fields\\DocumentInformation\\TestFileName.docx")));
        }

        /// <summary>
        /// Test how the FILENAME field is updated when the document is created blank.
        /// </summary>
        [Test]
        public void TestFileNameMissing()
        {
            Document doc = new Document();
            Assert.That(doc.OriginalFileName, Is.Null);

            DocumentBuilder builder = new DocumentBuilder(doc);
            FieldFileName field = (FieldFileName)builder.InsertField("FILENAME");
            Assert.That(field.Result, Is.Empty);
        }

        /// <summary>
        /// Test how the FILESIZE field is updated.
        /// </summary>
        [Test]
        public void TestFileSize()
        {
            Document doc = TestUtil.Open(@"Fields\DocumentInformation\TestFileSize.docx");
            doc.UpdateFields();

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            // File size in bytes.
            Assert.That(fields[0].Result, Is.EqualTo("27392"));
            // File size in kilobytes.
            Assert.That(fields[1].Result, Is.EqualTo("27"));
            // File size in megabytes.
            Assert.That(fields[2].Result, Is.EqualTo("0"));
        }

        /// <summary>
        /// Test how the FILESIZE field is updated when the document is created blank.
        /// </summary>
        [Test]
        public void TestFileSizeMissing()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            FieldFileSize field = (FieldFileSize)builder.InsertField("FILESIZE");
            Assert.That(field.Result, Is.EqualTo("0"));
        }


        /// <summary>
        /// When you update field "AUTHOR" value should be taken from the option "DefaultDocumentAuthor"
        /// </summary>
        [Test]
        public void WhenFieldUpdateAuthorNameShouldBeTakenFromOptionTest()
        {
            const string authorName = "John Smith";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Field authorField = builder.InsertField("AUTHOR \\* MERGEFORMAT");

            doc.FieldOptions.DefaultDocumentAuthor = authorName;
            doc.UpdateFields();

            Assert.That(authorField.Result, Is.EqualTo(authorName));
        }

        /// <summary>
        /// When you update field "AUTHOR" value should not be taken from the option "DefaultDocumentAuthor", if builtin document property "Author" exist
        /// </summary>
        [Test]
        public void WhenFieldUpdateAuthorNameShouldNotBeTakenFromOptionIfExistDocumentPropertyTest()
        {
            const string authorNameFromOption = "John Smith";
            const string authorNameFromBuiltInProperty = "authorNameFromBuiltInProperty";

            Document doc = new Document();
            doc.BuiltInDocumentProperties.Author = authorNameFromBuiltInProperty;
            DocumentBuilder builder = new DocumentBuilder(doc);
            Field authorField = builder.InsertField("AUTHOR \\* MERGEFORMAT");

            doc.FieldOptions.DefaultDocumentAuthor = authorNameFromOption;
            doc.UpdateFields();

            Assert.That(authorField.Result, Is.EqualTo(authorNameFromBuiltInProperty));
        }


#if NETSTANDARD
        private const string EnCaAm = "a.m.";
        private const string EnCaPm = "p.m.";
        private const string EnGbAm = "am";
        private const string EnGbPm = "pm";
#else
        private const string EnCaAm = "AM";
        private const string EnCaPm = "PM";
        private const string EnGbAm = "AM";
        private const string EnGbPm = "PM";
#endif



        /// <summary>
        /// WORDSNET-14678 When DOCX is saved as PDF by using a FileStream or MemoryStream, then the output document is missing some elements.
        /// </summary>
        [Test]
        public void TestJira14678()
        {
            Document document;
            using (Stream stream = File.OpenRead(TestUtil.BuildTestFileName(@"Fields\DocumentInformation\TestFileName.docx")))
                document = new Document(stream);

            const string fileName = @"TestJira14678.docx";
            string fullPath = TestEnvironment.GetUserHome() + fileName;
            document.FieldOptions.FileName = fullPath;

            document.UpdateFields();

            IList<Field> fields = FieldExtractor.ExtractToCollection(document, true);
            Assert.That(fields[0].Result, Is.EqualTo(fileName));
            Assert.That(fields[1].Result, Is.EqualTo(fullPath));
        }

        /// <summary>
        /// Tests the priority order of <see cref="Document.OriginalFileName"/> and <see cref="FieldOptions.FileName"/>.
        /// </summary>
        [Test]
        public void TestFileNameFieldOptionPriority()
        {
            Document document = TestUtil.Open(@"Fields\DocumentInformation\TestFileName.docx");

            const string fileName = @"Blah blah blah.docx";
            string fullPath = TestEnvironment.GetUserHome() + fileName;
            document.FieldOptions.FileName = fullPath;

            document.UpdateFields();
            IList<Field> fields = FieldExtractor.ExtractToCollection(document, true);
            Assert.That(fields[0].Result, Is.EqualTo(fileName));
            Assert.That(fields[1].Result, Is.EqualTo(fullPath));
        }

        /// <summary>
        /// WORDSNET-15318 Document.UpdateFields changes decimal symbol for Finnish culture.
        /// </summary>
        [Test]
        [TestCase("en-US", "0.00", "123.40")]
        [TestCase("fi-FI", "0,00", "123,40")]
        public void TestJira15318(string cultureName, string format, string expectedResult)
        {
            SystemPal.SetCulture(cultureName);

            DocumentBuilder builder = new DocumentBuilder();
            builder.Document.CustomDocumentProperties.Add("TestProperty", 123.4);
            Field field = builder.InsertField(string.Format("DOCPROPERTY TestProperty \\# {0}", format), null);

            field.Update();

            Assert.That(field.Result, Is.EqualTo(expectedResult));
        }
    }
}
