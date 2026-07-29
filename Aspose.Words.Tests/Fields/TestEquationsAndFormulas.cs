// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Aspose.Common;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests how the equations and formulas fields work.
    /// </summary>
    [TestFixture]
    public class TestEquationsAndFormulas : TestFieldsBase
    {
        /// <summary>
        /// Test how the = (formula) fields are updated. Also tests the numeric formatting.
        /// </summary>
        [Test]
        public void TestFormula()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestFormula.docx");
            doc = TestUtil.SaveOpen(doc, @"Fields\EquationsAndFormulas\TestFormula Modified.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[0].Result, Is.EqualTo("56"));
            Assert.That(fields[1].Result, Is.EqualTo("2.654"));
            Assert.That(fields[2].Result, Is.EqualTo("0.004"));
            Assert.That(fields[3].Result, Is.EqualTo("4"));
            Assert.That(fields[4].Result, Is.EqualTo("249.4"));
            Assert.That(fields[5].Result, Is.EqualTo("(225.55)"));
            Assert.That(fields[6].Result, Is.EqualTo("776,458"));
            Assert.That(fields[7].Result, Is.EqualTo("  38.00"));
            Assert.That(fields[8].Result, Is.EqualTo("$   0.00"));
            Assert.That(fields[9].Result, Is.EqualTo("13"));
            Assert.That(fields[10].Result, Is.EqualTo("6%"));
            Assert.That(fields[11].Result, Is.EqualTo("555.00"));
            Assert.That(fields[12].Result, Is.EqualTo("-1.00%"));
            Assert.That(fields[13].Result, Is.EqualTo("6.89"));
        }

        /// <summary>
        /// Test how the = (formula) fields containing functions are updated. Also tests the numeric formatting.
        /// </summary>
        [Test]
        public void TestFunctions()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestFunctions.docx");
            doc = TestUtil.SaveOpen(doc, @"Fields\EquationsAndFormulas\TestFunctions Modified.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // ABS

            Assert.That(fields[0].Result, Is.EqualTo("15"));
            Assert.That(fields[1].Result, Is.EqualTo("1.829"));

            // AND

            Assert.That(fields[2].Result, Is.EqualTo("0"));
            Assert.That(fields[3].Result, Is.EqualTo("0"));
            Assert.That(fields[4].Result, Is.EqualTo("1"));

            // AVERAGE

            Assert.That(fields[5].Result, Is.EqualTo("1"));
            Assert.That(fields[6].Result, Is.EqualTo("21.129"));

            // COUNT

            Assert.That(fields[7].Result, Is.EqualTo("4"));

            // DEFINED

            Assert.That(fields[8].Result, Is.EqualTo("1"));
            Assert.That(fields[9].Result, Is.EqualTo("0"));

            // FALSE

            Assert.That(fields[10].Result, Is.EqualTo("0"));

            // IF

            Assert.That(fields[11].Result, Is.EqualTo("6"));
            Assert.That(fields[12].Result, Is.EqualTo("32"));

            // INT

            Assert.That(fields[13].Result, Is.EqualTo("-64"));

            // MAX

            Assert.That(fields[14].Result, Is.EqualTo("6"));

            // MIN

            Assert.That(fields[15].Result, Is.EqualTo("-0.534"));

            // MOD

            Assert.That(fields[16].Result, Is.EqualTo("1"));
            Assert.That(fields[17].Result, Is.EqualTo("-0.6547"));

            // NOT

            Assert.That(fields[18].Result, Is.EqualTo("0"));
            Assert.That(fields[19].Result, Is.EqualTo("1"));

            // OR

            Assert.That(fields[20].Result, Is.EqualTo("0"));
            Assert.That(fields[21].Result, Is.EqualTo("1"));
            Assert.That(fields[22].Result, Is.EqualTo("1"));

            // PRODUCT

            Assert.That(fields[23].Result, Is.EqualTo("102,237,900.8"));

            // ROUND

            Assert.That(fields[24].Result, Is.EqualTo("4"));
            Assert.That(fields[25].Result, Is.EqualTo("0"));

            // SIGN

            Assert.That(fields[26].Result, Is.EqualTo("-1"));
            Assert.That(fields[27].Result, Is.EqualTo("0"));
            Assert.That(fields[28].Result, Is.EqualTo("1"));

            // SUM

            Assert.That(fields[29].Result, Is.EqualTo("-5254.5"));

            // TRUE

            Assert.That(fields[30].Result, Is.EqualTo("1"));
        }

        /// <summary>
        /// WORDSNET-13542 Test how bookmark references work.
        /// </summary>
        [Test]
        public void TestBookmarkReference()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestBookmarkReference.docx");
            doc = TestUtil.SaveOpen(doc, @"Fields\EquationsAndFormulas\TestBookmarkReference Modified.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[0].Result, Is.EqualTo("4"));
            Assert.That(fields[1].Result, Is.EqualTo("28"));
            Assert.That(fields[2].Result, Is.EqualTo("14"));
            Assert.That(fields[3].Result, Is.EqualTo("0"));
            Assert.That(fields[4].Result, Is.EqualTo("48.7"));
        }

        /// <summary>
        /// Test how cell references work.
        /// </summary>
        [Test]
        public void TestCellReference()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestCellReference.docx");
            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestCellReference Modified.docx");
        }



        /// <summary>
        /// WORDSNET-11066 Support cell reference, like ABOVE upon updating fields.
        /// </summary>
        [Test]
        public void TestDefect11066()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestDefect11066.docx");
            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestDefect11066 Modified.docx");
        }


        /// <summary>
        /// WORDSNET-711 Allow format strings in any culture.
        /// Specific culture's separators are now replaced with invariant culture's '.' and ',' as .NET requires.
        /// </summary>
        [Test]
        public void TestDefect711()
        {
            SystemPal.SetCulture("ru-RU");

            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestDefect711.docx");
            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestDefect711 Modified.docx");
        }

        /// <summary>
        /// WORDSNET-16189 SUM(ABOVE) is calculated improperly if there is merged cells.
        /// The value was taken from "previous" merged cells. Now I have implemented searching for the first merged cell.
        /// </summary>
        [Test]
        public void TestDefect16189()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\EquationsAndFormulas\TestDefect16189.docx");
            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestDefect16189 Modified.docx");
        }

        private static string RelativePath(string fileName)
        {
            return @"Fields\EquationsAndFormulas\" + fileName;
        }

        private static Document OpenTestDocument(string fileName)
        {
            return TestUtil.Open(RelativePath(fileName));
        }

        /// <summary>
        /// WORDSNET-24122 SUM(ABOVE) is calculated improperly if there are merged(simply wide) cells.
        /// Cell width should be taken into account.
        /// </summary>
        [Test]
        [TestCase("TestDefect24122.docx")]
        [TestCase("TestDefect24122Flipped.docx")]
        public void TestDefect24122(string file)
        {
            TestUpdatedFields(file, "33", "34", "35");
        }

        /// <summary>
        /// Tests SUM(ABOVE)/BELOW for a jagged table.
        /// </summary>
        [Test]
        [TestCase("TestJaggedTableWithSums.docx")]
        [TestCase("TestJaggedTableWithSumsFlipped.docx")]
        public void TestJaggedTableWithSums(string file)
        {
            TestUpdatedFields(file, "67", "56", "33", "34", "0");
        }

        /// <summary>
        /// WORDSNET-25839 NullReferenceException when updating a field with SUM(BELOW).
        /// Table boundaries shall be checked when skipping merged cells.
        /// </summary>
        [Test]
        public void TestDefect25839MergedCellAtLastRow()
        {
            TestUpdatedFields("TestDefect25839MergedCellAtLastRow.docx", "21");
        }

        /// <summary>
        /// WORDSNET-25839  NullReferenceException when updating a field with SUM(BELOW).
        /// Table boundaries shall be checked when skipping merged cells.
        /// This test checks a similar problem with SUM(RIGHT).
        /// </summary>
        [Test]
        public void TestDefect25839MergedCellAtLastColumn()
        {
            TestUpdatedFields("TestDefect25839MergedCellAtLastColumn.docx", "12");
        }

        /// <summary>
        /// WORDSNET-25839 NullReferenceException when updating a field with SUM(BELOW).
        /// Checks that merged cells are skipped.
        /// </summary>
        [Test]
        public void TestDefect25839IgnoreMergedCells()
        {
            TestUpdatedFields("TestDefect25839IgnoreMergedCells.docx", "26", "24", "31", "14", "15");
        }

        private static void TestUpdatedFields(string testFileName, params string[] expectedValues)
        {
            // Do the test.
            Document doc = OpenTestDocument(testFileName);
            doc.UpdateFields();

            // Check the calculated field values.
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            string[] actualValues = new string[System.Math.Min(fields.Count, expectedValues.Length)];
            for (int i = 0; i < actualValues.Length; i++)
                actualValues[i] = fields[i].Result;

            Assert.That(actualValues, Is.EqualTo(expectedValues));
        }








        /// <summary>
        /// WORDSNET-5563 SET field inside IF field is not updated properly.
        /// </summary>
        [Test]
        public void TestJira5563()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira5563.docx");
            doc.UpdateFields();

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // Check bookmark values.
            Assert.That(fields[4].Result, Is.EqualTo("equal"), fields[4].GetFieldCode());
            Assert.That(fields[8].Result, Is.EqualTo("equal"), fields[8].GetFieldCode());
            Assert.That(fields[12].Result, Is.EqualTo("equal"), fields[12].GetFieldCode());
            Assert.That(fields[23].Result, Is.EqualTo("equal-1"), fields[23].GetFieldCode());

            // Check IF field results except last one.
            // They must be empty, as SET field result must not be included to IF field result.
            int index = 0;
            int lastIndex = fields.Count - 1;
            foreach (Field field in fields)
            {
                if ((index++ != lastIndex) && (field.Type == FieldType.FieldIf))
                    Assert.That(field.Result, Is.EqualTo(string.Empty), field.GetFieldCode());
            }

            // Check last IF field result.
            Assert.That(fields[lastIndex].Result, Is.EqualTo("Clause [-5--]"), fields[lastIndex].GetFieldCode());

            // Perform GOLD check just to ensure that simple SET field result contains nothing but bookmark.
            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestJira5563 Modified.docx");
        }

        /// <summary>
        /// WORDSNET-5671 a + b formula is calculated incorrectly in case b is negative.
        /// </summary>
        [Test]
        public void TestJira5671()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira5671.docx");
            doc.UpdateFields();

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            string[] fieldResults = { "10", "10", "20", "20", "250", "-10", "100", "-1", "80" };
            int fieldIndex = 2;
            foreach (string fieldResult in fieldResults)
            {
                Assert.That(fields[fieldIndex].Result, Is.EqualTo(fieldResult), fields[fieldIndex].GetFieldCode());
                fieldIndex++;
            }
        }


        /// <summary>
        /// WORDSNET-9201 Mimic MS Word for rounding decimal numbers to correct precision
        /// </summary>
        [Test]
        public void TestDefect9201()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestDefect9201.docx");
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestDefect9201.docx");
        }

        /// <summary>
        /// WORDSNET-10008 System.InvalidOperationException is thrown during calculation INT function with incorrect parameters.
        /// </summary>
        [Test]
        public void TestDefect10008()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertField("= INT (trash)", "");

            doc.UpdateFields();
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            Assert.That(fields[0].Result, Is.EqualTo("!Undefined Bookmark, TRASH"));
        }

        /// <summary>
        /// WORDSNET-10309 multiple numbers in cell are not calculated by SUM(ABOVE)
        /// </summary>
        [Test]
        public void TestDefect10309()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestDefect10309.docx");
            doc.UpdateFields();
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[0].Result, Is.EqualTo("6"));
            Assert.That(fields[1].Result, Is.EqualTo("10"));
            Assert.That(fields[2].Result, Is.EqualTo("19"));
        }

        /// <summary>
        /// WORDSNET-10899 Document.UpdateFields does not update the formula field
        /// </summary>
        [Test]
        public void TestDefect10899()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestDefect10899.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestDefect10899.docx");
        }


        /// <summary>
        /// WORDSNET-10943 Document.UpdateFields does not update the formula field
        /// </summary>
        [Test]
        [TestCase("text", "0")]
        [TestCase("text text", "0")]
        [TestCase("\"text text\"", "0")]
        [TestCase("=", "0")]
        [TestCase("0", "0")]
        [TestCase("=0", "0")]
        [TestCase("= 0", "0")]
        [TestCase("1", "1")]
        [TestCase("=1", "1")]
        [TestCase("= 1", "0")]
        [TestCase("=2=2", "4")]
        [TestCase("=2", "2")]
        [TestCase("2=2", "4")]
        [TestCase("2=", "2")]
        [TestCase("2+2*2", "6")]
        public void TestJira10943(string code, string expectedFormulaFieldResult)
        {
            DocumentBuilder db = new DocumentBuilder();
            db.InsertField("SET book " + code, null);
            Field formulaField = db.InsertField("=book", null);

            db.Document.UpdateFields();

            Assert.That(formulaField.Result, Is.EqualTo(expectedFormulaFieldResult));
        }


        /// <summary>
        /// Test COMPARE field logic with expressions contains "*".
        /// </summary>
        [Test]
        public void TestCompareWildcards()
        {
            Document doc = TestUtil.OpenUpdateFields(@"Fields\EquationsAndFormulas\TestCompareWildcards.docx");
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc, true);
            Assert.That(fields[3].Result, Is.EqualTo("1"));
            Assert.That(fields[4].Result, Is.EqualTo("1"));
            Assert.That(fields[5].Result, Is.EqualTo("1"));
            Assert.That(fields[6].Result, Is.EqualTo("1"));
            Assert.That(fields[7].Result, Is.EqualTo("1"));
            Assert.That(fields[8].Result, Is.EqualTo("1"));
            Assert.That(fields[9].Result, Is.EqualTo("1"));
            Assert.That(fields[10].Result, Is.EqualTo("1"));
            Assert.That(fields[11].Result, Is.EqualTo("1"));
            Assert.That(fields[12].Result, Is.EqualTo("1"));
            Assert.That(fields[13].Result, Is.EqualTo("0"));
            Assert.That(fields[14].Result, Is.EqualTo("0"));
            Assert.That(fields[15].Result, Is.EqualTo("1"));
            Assert.That(fields[16].Result, Is.EqualTo("1"));
            Assert.That(fields[17].Result, Is.EqualTo("1"));
            Assert.That(fields[18].Result, Is.EqualTo("0"));
            Assert.That(fields[19].Result, Is.EqualTo("1"));
            Assert.That(fields[20].Result, Is.EqualTo("0"));
            Assert.That(fields[21].Result, Is.EqualTo("1"));
            Assert.That(fields[22].Result, Is.EqualTo("1"));
            Assert.That(fields[23].Result, Is.EqualTo("1"));
            Assert.That(fields[24].Result, Is.EqualTo("1"));
            Assert.That(fields[25].Result, Is.EqualTo("0"));
            Assert.That(fields[26].Result, Is.EqualTo("0"));
            Assert.That(fields[27].Result, Is.EqualTo("1")); //p = p*
            Assert.That(fields[28].Result, Is.EqualTo("1"));
            Assert.That(fields[29].Result, Is.EqualTo("1"));
            Assert.That(fields[30].Result, Is.EqualTo("0"));
            Assert.That(fields[31].Result, Is.EqualTo("1"));
            Assert.That(fields[32].Result, Is.EqualTo("1"));
            Assert.That(fields[33].Result, Is.EqualTo("1"));
            Assert.That(fields[34].Result, Is.EqualTo("0"));
            Assert.That(fields[35].Result, Is.EqualTo("1"));
            Assert.That(fields[36].Result, Is.EqualTo("0"));
            Assert.That(fields[37].Result, Is.EqualTo("1"));
            Assert.That(fields[38].Result, Is.EqualTo("0"));
            Assert.That(fields[39].Result, Is.EqualTo("1"));
            Assert.That(fields[40].Result, Is.EqualTo("0"));
            Assert.That(fields[41].Result, Is.EqualTo("1"));
            Assert.That(fields[42].Result, Is.EqualTo("0"));
            Assert.That(fields[43].Result, Is.EqualTo("1"));
            Assert.That(fields[44].Result, Is.EqualTo("0"));
            Assert.That(fields[45].Result, Is.EqualTo("1"));
            Assert.That(fields[46].Result, Is.EqualTo("0"));
            Assert.That(fields[47].Result, Is.EqualTo("0"));
            Assert.That(fields[48].Result, Is.EqualTo("1"));
            Assert.That(fields[49].Result, Is.EqualTo("0"));
            Assert.That(fields[50].Result, Is.EqualTo("0"));
            Assert.That(fields[51].Result, Is.EqualTo("1"));
            Assert.That(fields[52].Result, Is.EqualTo("0"));
            Assert.That(fields[53].Result, Is.EqualTo("0"));
            Assert.That(fields[54].Result, Is.EqualTo("1"));
            Assert.That(fields[55].Result, Is.EqualTo("0"));
            Assert.That(fields[56].Result, Is.EqualTo("1"));
            Assert.That(fields[57].Result, Is.EqualTo("1"));
            Assert.That(fields[58].Result, Is.EqualTo("1"));
            Assert.That(fields[59].Result, Is.EqualTo("1"));
            Assert.That(fields[60].Result, Is.EqualTo("1"));
            Assert.That(fields[61].Result, Is.EqualTo("0"));
            Assert.That(fields[62].Result, Is.EqualTo("0"));
            Assert.That(fields[63].Result, Is.EqualTo("0"));
            Assert.That(fields[64].Result, Is.EqualTo("1"));
            Assert.That(fields[65].Result, Is.EqualTo("1"));
            Assert.That(fields[66].Result, Is.EqualTo("1"));
        }


        /// <summary>
        /// WORDSNET-11470 StackOverflowException encountered during mail merge process
        /// </summary>
        [Test]
        public void TestJira11470()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira11470.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestJira11470.docx");
        }


        /// <summary>
        /// Test how spacing between digits processed.
        /// </summary>
        [Test]
        public void TestExpressionWithSpacingBetweenDigits()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestExpressionWithSpacingBetweenDigits.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestExpressionWithSpacingBetweenDigits.docx");
        }

        /// <summary>
        /// Test how current culture currency symbol is used by formula field.
        /// </summary>
        [Test]
        [TestCase("nl-NL")]
        [TestCase("en-US")]
        [TestCase("en-GB")]
        public void TestFormulaFieldWithCurrencySymbol(string culture)
        {
            SystemPal.SetCulture(culture);

            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestFormulaFieldWithCurrencySymbol.docx");
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestFormulaFieldWithCurrencySymbol " + culture + ".docx");
        }

        /// <summary>
        /// WORDSNET-11647 CARDTEXT swtich does not work for Dutch language
        /// </summary>
        [Test]
        public void TestJira11647()
        {
            SystemPal.SetCulture("nl-NL");

            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira11647.docx");
            doc.UpdateFields();
            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestJira11647.docx");
        }

        /// <summary>
        /// WORDSNET-9670 Document.UpdateFields does not update the IF field.
        /// </summary>
        [Test]
        public void TestJira9670()
        {
            Document doc = TestUtil.OpenUpdateFields(@"Fields\EquationsAndFormulas\TestJira9670.docx");
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc, true);

            Assert.That(fields[0].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[1].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[2].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[3].Result, Is.EqualTo("FALSE"));
            Assert.That(fields[4].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[5].Result, Is.EqualTo("FALSE"));
            Assert.That(fields[6].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[7].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[8].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[9].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[10].Result, Is.EqualTo("FALSE"));
            Assert.That(fields[11].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[12].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[13].Result, Is.EqualTo("FALSE"));
            Assert.That(fields[14].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[15].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[16].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[17].Result, Is.EqualTo("FALSE"));
            Assert.That(fields[18].Result, Is.EqualTo("TRUE"));
            Assert.That(fields[19].Result, Is.EqualTo("FALSE"));
            Assert.That(fields[20].Result, Is.EqualTo("TRUE"));
        }

        /// <summary>
        /// WORDSNET-11907 Document.UpdateFields does not update SUM(LEFT) and SUM(ABOVE) fields.
        /// </summary>
        [Test]
        public void TestJira11907()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira11907.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\EquationsAndFormulas\TestJira11907.docx");
        }



        /// <summary>
        /// WORDSNET-12361 If field does not evaluate correctly when one part of expression is result of another field and it contains only whitespaces.
        /// </summary>
        [Test]
        public void TestJira12361()
        {
            Document doc = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira12361.docx");
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Field fieldIf = fields[2];

            doc.UpdateFields();

            Assert.That(fieldIf.Result, Is.EqualTo("false"));
        }

        /// <summary>
        /// WORDSNET-12522 ExecuteWithRegions throws System.ArgumentOutOfRangeException when NumberGroupSeparator is used as ",".
        /// </summary>
        [Test]
        [TestCase("de-CH", ",", ";", "!Syntax Error")]
        [TestCase("de-CH", ",", ",", "3")]
        [TestCase("de-CH", ";", ";", "3")]
        [TestCase("de-CH", ";", ",", "!Syntax Error")]
        [TestCase("en-US", ",", ";", "!Syntax Error")]
        [TestCase("en-US", ",", ",", "3")]
        [TestCase("en-US", ";", ";", "3")]
        [TestCase("en-US", ";", ",", "!Syntax Error")]
        public void TestJira12522(string cultureName, string fieldSeparator, string cultureSeparator, string expectedResult)
        {
            SystemPal.SetCulture(cultureName);
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            culture.TextInfo.ListSeparator = cultureSeparator;

            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(string.Format("= MOD(23{0}5)", fieldSeparator), null);

            field.Update();

            Assert.That(field.Result, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// WORDSNET-13114 Nested IF statements cause error when merge field contains trailing backslash.
        /// </summary>
        [Test]
        public void TestJira13114()
        {
            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira13114.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\EquationsAndFormulas\TestJira13114.docx");
        }

        /// <summary>
        /// AW ignores the sign of the currency when calculating the sum of the range of table cells but Word saves one and shows it in the result field.
        /// </summary>
        [Test]
        [TestCase("en-US")]
        [TestCase("de-DE")]
        public void TestJira6470(string culture)
        {
            SystemPal.SetCulture(culture);

            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira6470.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\EquationsAndFormulas\TestJira6470 " + culture + ".docx");
        }

        /// <summary>
        /// WORDSNET-13697 UpdateFields throws "Index was outside the bounds of the array" exception.
        /// </summary>
        [Test]
        public void TestJira13697()
        {
            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira13697.docx");

            document.UpdateFields();

            Assert.That(document.Range.Fields[0].Result, Is.EqualTo("0"));
            Assert.That(document.Range.Fields[1].Result, Is.EqualTo("0"));
        }

        /// <summary>
        /// WORDSNET-13693, WORDSNET-13694 Document.UpdateFields incorrectly calculates SUM fields value.
        /// </summary>
        [Test]
        public void TestJira13693()
        {
            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\TestJira13693.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\EquationsAndFormulas\TestJira13693.docx");
        }

        [Test]
        [TestCase(new string[]{"10", "1000%", null}, 0)]
        [TestCase(new string[]{"1", "100%", null}, 0)]
        [TestCase(new string[]{"0.1", "10%", null}, 0)]
        [TestCase(new string[]{"0.01", "1%", null}, 0)]
        [TestCase(new string[]{"0.001", "0.1%", null}, 0)]
        [TestCase(new string[]{"10", "1000 %", null}, 0)]
        [TestCase(new string[]{"1", "100 %", null}, 0)]
        [TestCase(new string[]{"0.1", "10 %", null}, 0)]
        [TestCase(new string[]{"0.01", "1 %", null}, 0)]
        [TestCase(new string[]{"0.001", "0.1 %", null}, 0)]
        [TestCase(new string[]{"10.0001", "1000.01%", null}, 0)]
        [TestCase(new string[]{"1.0001", "100.01%", null}, 0)]
        [TestCase(new string[]{"0.1001", "10.01%", null}, 0)]
        [TestCase(new string[]{"0.0101", "1.01%", null}, 0)]
        [TestCase(new string[]{"0.00101", "0.101%", null}, 0)]
        [TestCase(new string[]{"10", "1000.00%", null}, 0)]
        [TestCase(new string[]{"1", "100.00%", null}, 0)]
        [TestCase(new string[]{"0.1", "10.00%", null}, 0)]
        [TestCase(new string[]{"0.01", "1.00%", null}, 0)]
        [TestCase(new string[]{"0.001", "0.100%", null}, 0)]
        [TestCase(new string[]{"0.0", "0%", null}, 0)]
        [TestCase(new string[]{"4.02", "1", "2%", "3"}, 0)]
        [TestCase(new string[]{"1", "1", "2% 3", "4"}, 0)]
        [TestCase(new string[]{"7.03", "1 2", "3%", "4"}, 0)]
        [TestCase(new string[]{"3.03", "1 2", "3%", "4 5"}, 0)]
        [TestCase(new string[]{"12.03", "1 2", "3% 4", "5"}, 0)]
        [TestCase(new string[]{"18.03", "1 2", "3% 4", "5 6"}, 0)]
        [TestCase(new string[]{"11.04", "1 2", "3 4%", "5"}, 0)]
        [TestCase(new string[]{"1", "1", "2 %", "3"}, 0)]
        [TestCase(new string[]{"1", "1", "2 % 3", "4"}, 0)]
        [TestCase(new string[]{"7.03", "1 2", "3 %", "4"}, 0)]
        [TestCase(new string[]{"12.03", "1 2", "3 %", "4 5"}, 0)]
        [TestCase(new string[]{"12.03", "1 2", "3 % 4", "5"}, 0)]
        [TestCase(new string[]{"18.03", "1 2", "3 % 4", "5 6"}, 0)]
        [TestCase(new string[]{"11.04", "1 2", "3 4 %", "5"}, 0)]
        public void Test13733(string[] cells, int notused)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartTable();

            builder.InsertCell();
            Field field = builder.InsertField("=SUM(RIGHT)", null);

            string result = cells[0];
            for (int i = 1; i < cells.Length; i++)
            {
                if (cells[i] == null)
                    break;

                builder.InsertCell();
                builder.Write(cells[i]);
            }

            field.Update();

            Assert.That(field.Result, Is.EqualTo(result));
        }

        /// <summary>
        /// WORDSNET-8651 SUM(ABOVE) not correct working with values in brackets.
        /// </summary>
        [Test]
        [TestCase(new string[]{"10", "1", "2", "3", "4"}, 0)]
        [TestCase(new string[]{"9", "(1)", "2", "3", "4"}, 0)]
        [TestCase(new string[]{"7", "(1)", "(2)", "3", "4"}, 0)]
        [TestCase(new string[]{"4", "(1)", "(2)", "(3)", "4"}, 0)]
        [TestCase(new string[]{"(10)", "(1)", "(2)", "(3)", "(4)"}, 0)]
        [TestCase(new string[]{"(8)", "1", "(2)", "(3)", "(4)"}, 0)]
        [TestCase(new string[]{"(4)", "1", "2", "(3)", "(4)"}, 0)]
        [TestCase(new string[]{"2", "1", "2", "3", "(4)"}, 0)]
        [TestCase(new string[]{"8", "-1", "2", "3", "4"}, 0)]
        [TestCase(new string[]{"4", "-1", "-2", "3", "4"}, 0)]
        [TestCase(new string[]{"-2", "-1", "-2", "-3", "4"}, 0)]
        [TestCase(new string[]{"5", "(1)", "-2", "3", "4"}, 0)]
        [TestCase(new string[]{"-7", "(1)", "-(2)", "-3", "-4"}, 0)]
        [TestCase(new string[]{"-4", "(1)", "(2)", "(3)", "-4"}, 0)]
        [TestCase(new string[]{"(2)", "(1)", "(2)", "(3)", "-(4)"}, 0)]
        [TestCase(new string[]{"2", "1", "-(2)", "-(3)", "(4)"}, 0)]
        [TestCase(new string[]{"2", "1", "2", "-(3)", "(4)"}, 0)]
        [TestCase(new string[]{"(6)", "-1", "2", "-3", "(4)"}, 0)]
        [TestCase(new string[]{"1", "-1", "(2)", "5", "(4)"}, 0)]
        [TestCase(new string[]{"5", "1", "1", "1", "1", "1"}, 0)]
        [TestCase(new string[]{"1", "1", "1", "1", "(12222)", "1"}, 0)]
        [TestCase(new string[]{"3", "1", "(1)", "1", "1", "1"}, 0)]
        [TestCase(new string[]{"1", "(1)", "1", "1", "(1)", "1"}, 0)]
        [TestCase(new string[]{"1", "1", "1", "1", "(1)", "(1)"}, 0)]
        [TestCase(new string[]{"(4)", "1", "1", "(1)", "(4)", "(1)"}, 0)]
        [TestCase(new string[]{"(1)", "100000", "(19999)", "1", "(1)", "(1)"}, 0)]
        [TestCase(new string[]{"102", "100000000", "(345667)", "100", "1", "1", "1", "(1)"}, 0)]
        [TestCase(new string[]{"-10", "100000000", "(345667)", "-10", "1", "-1", "-1", "1"}, 0)]
        [TestCase(new string[]{"10", "100000000", "(345667)", "10", "1", "-1", "-1", "1"}, 0)]
        [TestCase(new string[]{"-12", "100000000", "(345667)", "-10", "1", "-1", "-1", "(-1)"}, 0)]
        [TestCase(new string[]{"(12)", "100000000", "(345667)", "-10", "1", "-1", "-1", "(1)"}, 0)]
        public void TestJira8651SumAbove(string[] cells, int notused)
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.StartTable();

            string result = cells[0];
            for (int i = 1; i < cells.Length; i++)
            {
                builder.InsertCell();
                builder.Write(cells[i]);
                builder.EndRow();
            }

            builder.InsertCell();
            Field field = builder.InsertField("=SUM(ABOVE)", null);
            builder.EndRow();

            field.Update();

            Assert.That(field.Result, Is.EqualTo(result));
        }

        /// <summary>
        /// WORDSNET-8651 SUM(ABOVE) not correct working with values in brackets.
        /// </summary>
        [Test]
        [TestCase("(1)", false, "(1)")]
        [TestCase("(1)", true, "-1")]
        [TestCase("(-1)", false, "-1")]
        [TestCase("(-1)", true, "-1")]
        [TestCase("(1-2)", false, "-1")]
        [TestCase("(1-2)", true, "-1")]
        [TestCase("3 * (2)", false, "(6)")]
        [TestCase("3 * (2)", true, "-6")]
        [TestCase("(3) * 2", false, "(6)")]
        [TestCase("(3) * 2", true, "-6")]
        [TestCase("MOD((5), 2)", false, "(1)")]
        [TestCase("MOD((5), 2)", true, "-1")]
        [TestCase("SIGN((5))", false, "-1")]
        [TestCase("SIGN((5))", true, "-1")]
        [TestCase("(100)%", false, "(1)")]
        [TestCase("(100)%", true, "-1")]
        [TestCase("(5)+2$", false, "($3.00)")]
        [TestCase("(5)+2$", true, "-3")]
        public void TestJira8651Formula(string formula, bool useFormat, string result)
        {
            DocumentBuilder builder = new DocumentBuilder();
            string format = useFormat ? "\\# \"#\"" : null;
            Field field = builder.InsertField(string.Format(" = {0} {1}", formula, format), null);

            field.Update();

            Assert.That(field.Result, Is.EqualTo(result));
        }


        /// <summary>
        /// WORDSNET-15783 Document.UpdateFields does not update the formula field
        /// </summary>
        [Test]
        [TestCase(@"=4*1525,07 \# “#.##0,00”")]
        [TestCase(@"=4*1525,07\# “#.##0,00”")]
        public void TestJira15783(string fieldCode)
        {
            SystemPal.SetCulture("nl-NL");

            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(fieldCode, null);

            field.Update();

            Assert.That(field.Format.NumericFormat, Is.EqualTo("#.##0,00"));
            Assert.That(field.Result, Is.EqualTo("6.100,28"));
        }

        /// <summary>
        /// WORDSNET-19209 UpdateFields does not calculate SUM inside FORMTEXT.
        /// </summary>
        [Test]
        public void Test19209()
        {
            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\Test19209.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\EquationsAndFormulas\Test19209.docx");
        }


        [Test]
        [TestCase(@"=ABS(100)", "100")]
        [TestCase(@"=ABS(100,000.00)", "100,000")]
        [TestCase(@"=ABS(12,000+1000 - 1,000)", "12,000")]
        [TestCase(@"=ABS(12,000+1000.00 -ABS(-1,000))", "12,000")]
        [TestCase(@"=NOT(1,000 >=1.0)", "0")]
        public void TestFunctionParametersWhenListDelimiterEqualsToGroupDelimiter(string fieldCode, string result)
        {
            // Culture with list delimiter == group separator.
            SystemPal.SetCulture("en-UK");

            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(fieldCode, null);

            field.Update();

            Assert.That(field.Result, Is.EqualTo(result));
        }

        /// <summary>
        /// WORDSNET-19482 Document.UpdateFields does not update the formula fields.
        /// </summary>
        [Test]
        public void Test19482()
        {
            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\Test19482.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\EquationsAndFormulas\Test19482.docx");
        }



        /// <summary>
        /// WORDSNET-25941 FORMULA field dies not recognize table cell coordinates references.
        /// </summary>
        [Test]
        [TestCase("R2C2", "26")]
        [TestCase("R1C3 + R3C1", "114")]
        [TestCase("SUM(R1C1, R2C2, R3C3)", "360")]
        [TestCase("R5C5", "!E5 Is Not In Table")]
        [TestCase("R0C0", "!Undefined Bookmark, R0C0")]
        [TestCase("SUM(R1C1:R3C3)", "!Syntax Error")]
        public void Test25941(string formula, string result)
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.StartTable();
            AddTableRow(builder, "1", "2", "3", "4");
            AddTableRow(builder, "15", "26", "37", "48");
            AddTableRow(builder, "111", "222", "333", "444");
            builder.InsertCell();
            Field field = builder.InsertField(string.Format(" = {0}", formula), null);
            builder.EndTable();

            field.Update();

            Assert.That(field.Result, Is.EqualTo(result));
        }

        private static void AddTableRow(DocumentBuilder builder, params string[] values)
        {
            foreach (string value in values)
            {
                builder.InsertCell();
                builder.Write(value);
            }

            builder.EndRow();
        }


        /// <summary>
        /// WORDSNET-28506 Formula fields are updated improperly.
        /// </summary>
        [Test]
        public void Test28506()
        {
            Document document = TestUtil.Open(@"Fields\EquationsAndFormulas\Test28506.docx");

            document.UpdateFields();

            FieldCollection fields = document.Range.Fields;
            Assert.That(fields[0].Result, Is.EqualTo("6"));
            Assert.That(fields[1].Result, Is.EqualTo("1"));
            Assert.That(fields[2].Result, Is.EqualTo("1"));
            Assert.That(fields[3].Result, Is.EqualTo("1"));
            Assert.That(fields[4].Result, Is.EqualTo("5"));
        }
    }
}
