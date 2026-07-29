// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests how differently composed field code is parsed and how the fields are updated.
    /// </summary>
    [TestFixture]
    public class TestFieldCode : TestFieldsBase
    {
        /// <summary>
        /// Tests how various field code features are parsed (no spaces, escape symbols etc).
        /// </summary>
        [Test]
        public void TestParse()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Fields\FieldCode\TestFieldCode.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            FieldCode fieldCode = new FieldCode(fields[0]);
            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("FieldName"));
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\b"), Is.EqualTo("TextBefore"));
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("Text after"));

            fieldCode = new FieldCode(fields[1]);
            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("Field name with \" and \\ and *"));
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\b"), Is.EqualTo("TextBefore"));
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("Text after"));

            fieldCode = new FieldCode(fields[2]);
            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("FieldName"));
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\b"), Is.EqualTo("\"TextBefore\""));
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo(""));
            Assert.That(fieldCode.HasSwitch("\\m"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\m"), Is.Null);
            Assert.That(fieldCode.HasSwitch("\\v"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\v"), Is.Null);
            Assert.That(fieldCode.HasSwitch("\\s"), Is.False);

            doc.Range.UpdateFields();
            TestUtil.SaveCheckGold(doc, @"Fields\FieldCode\TestFieldCode Modified.docx");

            Assert.That(fields[0].Result, Is.EqualTo("TextBefore«FieldName»Text after"));
            Assert.That(fields[1].Result, Is.EqualTo("TextBefore«Field name with \" and \\ and *»Text after"));
            Assert.That(fields[2].Result, Is.EqualTo("\"TextBefore\"«FieldName»"));
        }

        /// <summary>
        /// Test how nested fields are updated.
        /// </summary>
        [Test]
        public void TestNested()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\FieldCode\TestNested.docx");
            doc = TestUtil.SaveOpen(doc, @"Fields\FieldCode\TestNested Modified.docx");

            NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);

            IList<Field> fields = FieldExtractor.ExtractToCollection(paragraphs[0]);
            Assert.That(fields[fields.Count - 1].Result, Is.EqualTo("not"));

            fields = FieldExtractor.ExtractToCollection(paragraphs[1]);
            Assert.That(fields[fields.Count - 1].Result, Is.EqualTo(""));

            fields = FieldExtractor.ExtractToCollection(paragraphs[2]);
            Assert.That(fields[fields.Count - 1].Result, Is.EqualTo("Credit not acceptable"));

            fields = FieldExtractor.ExtractToCollection(paragraphs[3]);
            Assert.That(fields[fields.Count - 1].Result, Is.EqualTo("-26.2"));
        }

        /// <summary>
        /// Test how fields containing paragraph breaks are updated.
        /// </summary>
        [Test]
        public void TestParagraphBreaks()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\FieldCode\TestParagraphBreaks.docx");
            doc = TestUtil.SaveOpen(doc, @"Fields\FieldCode\TestParagraphBreaks Modified.docx");


            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields[fields.Count - 1].Result, Is.EqualTo("-26.2%"));
        }

        /// <summary>
        /// Tests how fields are updated when a field code contains nested fields without extra
        /// leading and trailing whitespaces for the corresponding field chars.
        /// </summary>
        [Test]
        public void TestNoExtraWhitespaces()
        {
            Document doc = TestUtil.Open(@"Fields\FieldCode\TestNoExtraWhitespaces.docx");
            doc.UpdateFields();

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            string[] results = { "0", "1", "", "3", "3", "0", "1", "2", "", "2" };
            Assert.That(fields.Count, Is.EqualTo(results.Length));

            for (int i = 0; i < results.Length; i++)
                Assert.That(fields[i].Result, Is.EqualTo(results[i]), fields[i].GetFieldCode());
        }

        /// <summary>
        /// Tests how field arguments are modified.
        /// </summary>
        [Test]
        public void TestSetArguments()
        {
            Document doc = TestUtil.Open(@"Fields\FieldCode\TestFieldCodeSet.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Field field = fields[0];
            FieldCode fieldCode = field.FieldCodeCache;

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("0"));
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);

            // Change the first argument. Set it to an empty string. It should be enclosed in double quotes.
            fieldCode.SetArgument(0, "");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  """" \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo(""));

            // Change the first argument. Set it to a string without whitespaces. No double quotes this time.
            fieldCode.SetArgument(0, "New_argument");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  New_argument \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("New_argument"));

            // Change the first argument. Set it to a string with a whitespace. It should be enclosed in double quotes
            // again.
            fieldCode.SetArgument(0, "New argument");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  ""New argument"" \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("New argument"));

            // Add the third argument. The second one should be added as an empty string.
            fieldCode.SetArgument(2, "New argument");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  ""New argument"" """" ""New argument"" \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(3));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("New argument"));
            Assert.That(fieldCode.GetArgumentAsString(1), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(2), Is.EqualTo("New argument"));

            // Attempt to set the first argument to null, which causes setting it to an empty string to avoid shifting.
            fieldCode.SetArgument(0, null);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  """" """" ""New argument"" \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(3));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(1), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(2), Is.EqualTo("New argument"));

            // Set the third argument to null, which works safely.
            fieldCode.SetArgument(2, null);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  """" """"  \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(2));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(1), Is.EqualTo(""));

            // Attempt to set the third argument to null now makes no sense.
            fieldCode.SetArgument(2, null);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  """" """"  \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(2));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(1), Is.EqualTo(""));

            // Remove all arguments.
            fieldCode.SetArgument(1, null);
            fieldCode.SetArgument(0, null);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK     \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(0));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            // Add the third argument. The first one and second one should be added as an empty string.
            fieldCode.SetArgument(2, "New argument");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK     """" """" ""New argument"" \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(3));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(1), Is.EqualTo(""));
            Assert.That(fieldCode.GetArgumentAsString(2), Is.EqualTo("New argument"));
        }

        /// <summary>
        /// Tests how field switches are set.
        /// </summary>
        [Test]
        public void TestSetSwitches()
        {
            Document doc = TestUtil.Open(@"Fields\FieldCode\TestFieldCodeSet.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Field field = fields[0];
            FieldCode fieldCode = field.FieldCodeCache;

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("0"));
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);

            // Setting existing switch changes nothing.
            fieldCode.SetSwitch("\\a", true);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a \f 0 \p "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("0"));
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);

            // Setting new switch appends it to the code.
            fieldCode.SetSwitch("\\d", true);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a \f 0 \p  \d"));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(4));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("0"));
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);

            // Setting a switch with an argument replaces an existing one.
            fieldCode.SetSwitch("\\f", "100500");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a \f 100500 \p  \d"));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(4));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("100500"));
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);

            // Setting a switch with a null argument removes an existing one.
            fieldCode.SetSwitch("\\f", null);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a  \p  \d"));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(3));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.False);
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);

            // Setting a switch with an argument appends it to the code. Also double quotes are added if needed.
            fieldCode.SetSwitch("\\f", "100 500");

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a  \p  \d \f ""100 500"""));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(4));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("100 500"));

            // Setting a switch with an argument and index appends it to the code.
            fieldCode.SetSwitch("\\*", "MERGEFORMAT", 0);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a  \p  \d \f ""100 500"" \* MERGEFORMAT"));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(5));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("100 500"));
            Assert.That(fieldCode.HasSwitch("\\*"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\*"), Is.EqualTo("MERGEFORMAT"));

            // Setting a switch with an argument and existing index replaces it.
            fieldCode.SetSwitch("\\*", "CHARFORMAT", 0);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a  \p  \d \f ""100 500"" \* CHARFORMAT"));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(5));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("100 500"));
            Assert.That(fieldCode.HasSwitch("\\*"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\*"), Is.EqualTo("CHARFORMAT"));

            // Setting a switch with a null argument and existing index removes it.
            fieldCode.SetSwitch("\\*", null, 0);

            Assert.That(field.GetFieldCode(), Is.EqualTo(@" LINK  AcroExch.acrobatsecuritysettings.1 \a  \p  \d \f ""100 500"" "));

            Assert.That(fieldCode.Arguments.Count, Is.EqualTo(1));
            Assert.That(fieldCode.Switches.Count, Is.EqualTo(4));

            Assert.That(fieldCode.GetArgumentAsString(0), Is.EqualTo("AcroExch.acrobatsecuritysettings.1"));
            Assert.That(fieldCode.HasSwitch("\\a"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\p"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\d"), Is.True);
            Assert.That(fieldCode.HasSwitch("\\f"), Is.True);
            Assert.That(fieldCode.GetSwitchArgumentAsString("\\f"), Is.EqualTo("100 500"));
        }

        /// <summary>
        /// Tests how field format is parsed and changed.
        /// </summary>
        [Test]
        public void TestFieldFormat()
        {
            Document doc = TestUtil.Open(@"Fields\FieldCode\TestFieldFormat.docx");

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Field field = fields[0];
            FieldFormat format = field.Format;

            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\@ \"dddd, MMMM dd, yyyy\" \\* Upper \\* Lower \\* FirstCap \\* MERGEFORMAT "));

            Assert.That(format.NumericFormat, Is.Null);
            Assert.That(format.DateTimeFormat, Is.EqualTo("dddd, MMMM dd, yyyy"));
            Assert.That(format.GeneralFormats.Count, Is.EqualTo(4));
            Assert.That(format.GeneralFormats[0], Is.EqualTo(GeneralFormat.Upper));
            Assert.That(format.GeneralFormats[1], Is.EqualTo(GeneralFormat.Lower));
            Assert.That(format.GeneralFormats[2], Is.EqualTo(GeneralFormat.FirstCap));
            Assert.That(format.GeneralFormats[3], Is.EqualTo(GeneralFormat.MergeFormat));

            format.NumericFormat = "0.#";
            format.GeneralFormats.Remove(GeneralFormat.MergeFormat);

            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\# 0.# \\@ \"dddd, MMMM dd, yyyy\" \\* Upper \\* Lower \\* FirstCap  "));

            Assert.That(format.NumericFormat, Is.EqualTo("0.#"));
            Assert.That(format.DateTimeFormat, Is.EqualTo("dddd, MMMM dd, yyyy"));
            Assert.That(format.GeneralFormats.Count, Is.EqualTo(3));
            Assert.That(format.GeneralFormats[0], Is.EqualTo(GeneralFormat.Upper));
            Assert.That(format.GeneralFormats[1], Is.EqualTo(GeneralFormat.Lower));
            Assert.That(format.GeneralFormats[2], Is.EqualTo(GeneralFormat.FirstCap));

            format.GeneralFormats.RemoveAt(1);
            format.DateTimeFormat = null;

            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\# 0.#  \\* Upper  \\* FirstCap  "));

            Assert.That(format.NumericFormat, Is.EqualTo("0.#"));
            Assert.That(format.DateTimeFormat, Is.Null);
            Assert.That(format.GeneralFormats.Count, Is.EqualTo(2));
            Assert.That(format.GeneralFormats[0], Is.EqualTo(GeneralFormat.Upper));
            Assert.That(format.GeneralFormats[1], Is.EqualTo(GeneralFormat.FirstCap));

            format.GeneralFormats.Add(GeneralFormat.CharFormat);

            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\# 0.#  \\* Upper  \\* FirstCap   \\* CHARFORMAT"));

            Assert.That(format.NumericFormat, Is.EqualTo("0.#"));
            Assert.That(format.DateTimeFormat, Is.Null);
            Assert.That(format.GeneralFormats.Count, Is.EqualTo(3));
            Assert.That(format.GeneralFormats[0], Is.EqualTo(GeneralFormat.Upper));
            Assert.That(format.GeneralFormats[1], Is.EqualTo(GeneralFormat.FirstCap));
            Assert.That(format.GeneralFormats[2], Is.EqualTo(GeneralFormat.CharFormat));
        }

        /// <summary>
        /// Tests how the field API works with existing fields.
        /// </summary>
        [Test]
        public void TestExistingField()
        {
            DocumentBuilder builder = new DocumentBuilder();

            // Insert a field.
            FieldSeq field = (FieldSeq)builder.InsertField(FieldType.FieldSequence, true);
            field.BookmarkName = "Bookmark1";
            field.ResetHeadingLevel = "5";
            field.HideFieldResult = true;
            field.Format.GeneralFormats.Add(GeneralFormat.MergeFormat);

            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"\" Bookmark1 \\s 5 \\h \\* MERGEFORMAT"));

            // We changed our mind.
            field.ResetHeadingLevel = "0";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"\" Bookmark1 \\s 0 \\h \\* MERGEFORMAT"));

            // Find the existing field.
            field = (FieldSeq)((FieldEnd)builder.CurrentStory.FirstParagraph.LastChild).GetField();
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"\" Bookmark1 \\s 0 \\h \\* MERGEFORMAT"));

            field.SequenceIdentifier = "Sequence 1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"Sequence 1\" Bookmark1 \\s 0 \\h \\* MERGEFORMAT"));

            field.ResetNumber = "10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"Sequence 1\" Bookmark1 \\s 0 \\h \\* MERGEFORMAT \\r 10"));

            field.HideFieldResult = false;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"Sequence 1\" Bookmark1 \\s 0  \\* MERGEFORMAT \\r 10"));

            field.SequenceIdentifier = null;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"\" Bookmark1 \\s 0  \\* MERGEFORMAT \\r 10"));

            field.BookmarkName = null;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  \"\"  \\s 0  \\* MERGEFORMAT \\r 10"));

            field.SequenceIdentifier = null;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ    \\s 0  \\* MERGEFORMAT \\r 10"));
        }


        /// <summary>
        /// WORDSNET-11844 Content (Fields) position is changed after conversion from Doc to Pdf
        /// </summary>
        [Test]
        [TestCase("IF 1 = 2 \"true\" \"false\"")]
        [TestCase("IF 1 = 2 „true“ „false“")]
        public void TestJira11844(string fieldCode)
        {
            DocumentBuilder builder = new DocumentBuilder();
            FieldIf field = (FieldIf) builder.InsertField(fieldCode);

            Assert.That(field.LeftExpression, Is.EqualTo("1"));
            Assert.That(field.ComparisonOperator, Is.EqualTo("="));
            Assert.That(field.RightExpression, Is.EqualTo("2"));
            Assert.That(field.TrueText, Is.EqualTo("true"));
            Assert.That(field.FalseText, Is.EqualTo("false"));
        }

        /// <summary>
        /// Tests how field type is parsed from field code.
        /// </summary>
        [Test]
        [TestCase("=A", FieldType.FieldFormula, TestName = "TestFieldTypeParseA")]
        [TestCase("SAVEDATE", FieldType.FieldSaveDate, TestName = "TestFieldTypeParseB")]
        [TestCase("QUOT", FieldType.FieldNone, TestName = "TestFieldTypeParseC")]
        [TestCase("QUOTE%", FieldType.FieldNone, TestName = "TestFieldTypeParseD")]
        [TestCase("QUOTES", FieldType.FieldNone, TestName = "TestFieldTypeParseE")]
        [TestCase("NUMPAGES-1", FieldType.FieldNone, TestName = "TestFieldTypeParseF")]
        [TestCase("NUMPAGES-", FieldType.FieldNone, TestName = "TestFieldTypeParseG")]
        [TestCase("NUMPAGES!", FieldType.FieldNone, TestName = "TestFieldTypeParseH")]
        [TestCase("NUMPAGESSSSS", FieldType.FieldNone, TestName = "TestFieldTypeParseI")]
        [TestCase("NUMPAGES", FieldType.FieldNumPages, TestName = "TestFieldTypeParseJ")]
        [TestCase("\"NUMPAGES\"", FieldType.FieldNumPages, TestName = "TestFieldTypeParseK")]
        [TestCase(" \"NUMPAGES\" blah blah", FieldType.FieldNumPages, TestName = "TestFieldTypeParseL")]
        [TestCase(" NUMPAGES blah blah", FieldType.FieldNumPages, TestName = "TestFieldTypeParseO")]
        [TestCase("\"NUMPAGES!\"", FieldType.FieldNone, TestName = "TestFieldTypeParseP")]
        [TestCase("QUOTE”", FieldType.FieldQuote, TestName = "TestFieldTypeParseQ")]
        [TestCase(" IF\x0013 ", FieldType.FieldIf, TestName = "TestFieldTypeParseR")]
        [TestCase("\\PAGE", FieldType.FieldPage, TestName = "TestFieldTypeParseS")]
        [TestCase("\\\\PAGE", FieldType.FieldNone, TestName = "TestFieldTypeParseT")]
        [TestCase("\"\\PAGE\"", FieldType.FieldPage, TestName = "TestFieldTypeParseU")]
        [TestCase("\\\"PAGE\"", FieldType.FieldNone, TestName = "TestFieldTypeParseV")]
        public void TestFieldTypeParse(string fieldCode, FieldType expectedType)
        {
            FieldType actualType = FieldUtil.GetFieldType(fieldCode);
            Assert.That(actualType, Is.EqualTo(expectedType));
        }

        private static void VerifyTestJira11905Fields(Document doc)
        {
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            Assert.That(fields.Count, Is.EqualTo(4));

            // These values of field codes are the same, as they displayed in MSW.
            // In RTF code of the source document the values ":"  and "\\:" are inverted.
            Assert.That(fields[0].GetFieldCode(), Is.EqualTo(" XE \"A\\:B\\:C\" "));
            Assert.That(fields[1].GetFieldCode(), Is.EqualTo(" XE \"A:B:C\" "));
            Assert.That(fields[2].GetFieldCode(), Is.EqualTo(" XE \"A\\:B:C\" "));
        }

        /// <summary>
        /// Tests how fields with hidden text in code are updated.
        /// </summary>
        [Test]
        public void TestFieldCodeWithHiddenText()
        {
            Document document = TestUtil.Open(@"Fields\FieldCode\TestFieldCodeWithHiddenText.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\FieldCode\TestFieldCodeWithHiddenText.docx");
        }

        [Test]
        public void TestSetSwitchShouldNotAffectArguments()
        {
            DocumentBuilder builder = new DocumentBuilder();
            FieldIncludePicture field = (FieldIncludePicture) builder.InsertField(@"INCLUDEPICTURE C:\\Temp\\Picuture.png", null);

            field.Format.GeneralFormats.AddOrRemove(GeneralFormat.MergeFormat, true);

            Assert.That(field.SourceFullName, Is.EqualTo(@"C:\Temp\Picuture.png"));
        }

        [Test]
        public void TestSetArgumentsSeparately()
        {
            RunPr boldPr = new RunPr();
            RunPr italicPr = new RunPr();

            boldPr.Bold = AttrBoolEx.True;
            italicPr.Italic = AttrBoolEx.True;

            Document document = new Document();

            FieldIf field = (FieldIf)new FieldBuilder(FieldType.FieldIf)
                .AddArgument("left")
                .AddArgument("=")
                .AddArgument("right")
                .AddArgument(new FieldArgumentBuilder().AddNode(new Run(document, "true text (bold)", boldPr)))
                .AddArgument(new FieldArgumentBuilder().AddNode(new Run(document, "false text (italic)", italicPr)))
                .BuildAndInsert(document.FirstSection.Body.FirstParagraph);


            field.ComparisonOperator = "<>";

            Run trueText = (Run)document.GetNodeById("11.0.0.0");
            Run falseText = (Run)document.GetNodeById("15.0.0.0");

            Assert.That(trueText.Text, Is.EqualTo("true text (bold)"));
            Assert.That(trueText.Font.Bold, Is.True);
            Assert.That(falseText.Text, Is.EqualTo("false text (italic)"));
            Assert.That(falseText.Font.Italic, Is.True);
        }

        [Test]
        public void TestRemoveDuplicatedSwitch()
        {
            DocumentBuilder builder = new DocumentBuilder();
            FieldFileName field = (FieldFileName)builder.InsertField(@" FILENAME \p \p \t \p ", null);

            field.IncludeFullPath = false;

            Assert.That(field.IncludeFullPath, Is.False);
            Assert.That(field.GetFieldCode(), Is.EqualTo(@" FILENAME   \t  "));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestIsolateArguments(int count)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(@" REF ""arg 1"" ""arg 2"" ""arg 3"" ", null);
            FieldCode fieldCode = field.FieldCodeCache;

            for (int i = 0; i < count; i++)
                fieldCode.IsolateElements();


            string[] arguments = {"\"arg 1\"", "\"arg 2\"", "\"arg 3\""};
            AssertArguments(fieldCode, arguments);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestIsolateSwitches(int count)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(@" REF \d \f \h \p ", null);
            FieldCode fieldCode = field.FieldCodeCache;

            for (int i = 0; i < count; i++)
                fieldCode.IsolateElements();

            string[] switches = {@"\d", @"\f", @"\h", @"\p"};
            AssertSwitches(fieldCode, switches, null);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestIsolateElements(int count)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(@" REF ""arg 1"" \d ""switch arg"" ""arg 2"" \f ""arg 3"" \h \p \* Upper \* MERGEFORMAT ", null);
            FieldCode fieldCode = field.FieldCodeCache;

            for (int i = 0; i < count; i++)
                fieldCode.IsolateElements();

            string[] switches = { @"\d", @"\f", @"\h", @"\p", @"\*", @"\*" };
            string[] switchArguments = { "\"switch arg\"", null, null, null, "Upper", "MERGEFORMAT" };
            AssertSwitches(fieldCode, switches, switchArguments);

            string[] arguments = { "\"arg 1\"", "\"arg 2\"", "\"arg 3\"" };
            AssertArguments(fieldCode, arguments);
        }

        private static void AssertArguments(FieldCode fieldCode, string[] arguments)
        {
            int i = 0;
            foreach (FieldArgument argument in fieldCode.Arguments)
            {
                AssertRange(argument.Range, arguments[i]);
                Assert.That(argument.Text, Is.EqualTo(arguments[i]));
                i++;
            }
        }

        private static void AssertSwitches(FieldCode fieldCode, string[] switches, string[] switchArguments)
        {
            if (switchArguments == null)
                switchArguments = new string[switches.Length];

            int i = 0;
            foreach (FieldSwitch fieldSwitch in fieldCode.Switches)
            {
                AssertRange(fieldSwitch.Range, switches[i]);
                Assert.That(fieldSwitch.Name, Is.EqualTo(switches[i]));

                if (switchArguments[i] == null)
                    Assert.That(fieldSwitch.HasArgument, Is.False);
                else
                {
                    AssertRange(fieldSwitch.Argument.Range, switchArguments[i]);
                    Assert.That(fieldSwitch.Argument.Text, Is.EqualTo(switchArguments[i]));
                }

                i++;
            }
        }

        private static void AssertRange(NodeRange range, string expectedText)
        {
            string text = NodeTextCollector.GetText(range);
            Assert.That(text, Is.EqualTo(expectedText));
            Assert.That(range.Start.IsStart, Is.True);
            Assert.That(range.End.IsEnd, Is.True);
        }

        [Test]
        public void TestSetArgumentWithNestedField()
        {
            Document document = new Document();

            FieldIf field = (FieldIf)new FieldBuilder(FieldType.FieldIf)
                .AddArgument(new FieldArgumentBuilder().AddText("! ").AddField(new FieldBuilder(FieldType.FieldMergeField).AddArgument("field1")).AddText(" !"))
                .AddArgument("=")
                .AddArgument(new FieldBuilder(FieldType.FieldMergeField).AddArgument("field2"))
                .AddArgument("true text")
                .AddArgument("false text")
                .BuildAndInsert(document.FirstSection.Body.FirstParagraph);

            field.LeftExpression = "left";
            field.ComparisonOperator = "<>";
            field.RightExpression = "right";

            Assert.That(field.LeftExpression, Is.EqualTo("left"));
            Assert.That(field.ComparisonOperator, Is.EqualTo("<>"));
            Assert.That(field.RightExpression, Is.EqualTo("right"));

            Assert.That(FieldExtractor.ExtractToCollection(document, true, FieldType.FieldMergeField).Count, Is.EqualTo(0));
        }



        /// <summary>
        /// WORDSNET-13583 GetFieldCode on a IF field returning Empty.
        /// </summary>
        [Test]
        [TestCase(true, " IF \x0013 MERGEFIELD Left \x0014\x00ABLeft\x00BB\x0015 = \x0013 MERGEFIELD Right \x0014\x00ABRight\x00BB\x0015 \"true text\" \"false text\" ", TestName = "TestJira13583A")]
        [TestCase(false, " IF \x00ABLeft\x00BB = \x00ABRight\x00BB \"true text\" \"false text\" ", TestName = "TestJira13583B")]
        public void TestJira13583(bool includeChildFieldCodes, string expectedFieldCode)
        {
            Document document = new Document();
            Field field = new FieldBuilder(FieldType.FieldIf)
                .AddArgument(new FieldBuilder(FieldType.FieldMergeField).AddArgument("Left"))
                .AddArgument("=")
                .AddArgument(new FieldBuilder(FieldType.FieldMergeField).AddArgument("Right"))
                .AddArgument("true text")
                .AddArgument("false text")
                .BuildAndInsert(document.FirstSection.Body.FirstParagraph);
            field.Update();

            string fieldCode = field.GetFieldCode(includeChildFieldCodes);

            Assert.That(fieldCode, Is.EqualTo(expectedFieldCode));
        }

        /// <summary>
        /// WORDSNET-13773 System.NullReferenceException occurs during converting DOC to PDF.
        /// </summary>
        [Test]
        public void TestJira13773()
        {
            DocumentBuilder builder = new DocumentBuilder();

            FieldSet field = (FieldSet)builder.InsertField(@" SET Bookmark\Name Bookmark\Text\* MERGEFORMAT");

            Assert.That(field.BookmarkName, Is.EqualTo("BookmarkName"));
            NodeRange nameRange = field.FieldCodeCache.GetArgument(0).Range;
            Assert.That(nameRange.IsVoid, Is.False);
            Assert.That(nameRange.Start.Offset, Is.EqualTo(5));
            Assert.That(nameRange.End.Offset, Is.EqualTo(18));

            Assert.That(field.BookmarkText, Is.EqualTo("BookmarkText"));
            NodeRange textRange = field.FieldCodeCache.GetArgument(1).Range;
            Assert.That(textRange.IsVoid, Is.False);
            Assert.That(textRange.Start.Offset, Is.EqualTo(19));
            Assert.That(textRange.End.Offset, Is.EqualTo(32));
        }

        private const int NullLanguageId = int.MaxValue;

        [Test]
        [TestCaseSource("TestFieldLocaleIdSource")]
        public void TestFieldLocaleId(int initialLocaleId, int initialLocaleIdBidi, bool initialBidi, int newLocaleId)
        {
            DocumentBuilder builder = new DocumentBuilder();

            if (initialLocaleId != NullLanguageId)
                builder.Font.LocaleId = initialLocaleId;
            if (initialLocaleIdBidi != NullLanguageId)
                builder.Font.LocaleIdBi = initialLocaleIdBidi;
            builder.Font.Bidi = initialBidi;
            Field field = builder.InsertField("=1", null);

            AssertLocaleId(
                field,
                initialBidi
                    ? (initialLocaleIdBidi != NullLanguageId ? initialLocaleIdBidi : RunPr.ProcessOrUserDefaultLanguageId)
                    : (initialLocaleId != NullLanguageId ? initialLocaleId : RunPr.ProcessOrUserDefaultLanguageId));

            field.LocaleId = newLocaleId;
            AssertLocaleId(field, newLocaleId);
        }

        private static void AssertLocaleId(Field field, int expectedLocaleId)
        {
            Assert.That(field.LocaleId, Is.EqualTo(expectedLocaleId));
            field = field.Document.Range.Fields[0];
            Assert.That(field.LocaleId, Is.EqualTo(expectedLocaleId));
        }

        public static List<TestCaseData> TestFieldLocaleIdSource()
        {
            List<TestCaseData> result = new List<TestCaseData>();

            int[] languages = {NullLanguageId, (int)Language.RussianRussia, (int)Language.EnglishUS};
            int[] languageBis = { NullLanguageId, (int)Language.HebrewIsrael, (int)Language.ArabicSaudiArabia };
            int[] newLanguages = {(int)Language.RussianRussia, (int)Language.EnglishUS, (int)Language.HebrewIsrael, (int)Language.ArabicSaudiArabia};
            bool[] bidis = {true, false};

            foreach (int language in languages)
                foreach (int languageBi in languageBis)
                    foreach (bool bidi in bidis)
                        foreach (int newLanguage in newLanguages)
                            result.Add(new TestCaseData(language, languageBi, bidi, newLanguage));

            return result;
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9999)]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Not a valid LCID:", MatchType = MessageMatch.StartsWith)]
        public void TestInvalidLocaleId(int localeId)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField("=1", null);

            field.LocaleId = localeId;
        }

        /// <summary>
        /// WORDSNET-17472 DOCX to PDF conversion issue with SAVEDATE rendering
        /// </summary>
        [Test]
        public void  TestJira17472()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field = builder.InsertField(" DATE \\@ «yyyy\"", null);

            field.Update();

            Assert.That(field.Result, Is.EqualTo("2006"));
        }

        /// <summary>
        /// WORDSNET-22027 System.InvalidOperationException occurs upon updating fields.
        /// </summary>
        [Test]
        public void Test22027()
        {
            Document document = TestUtil.Open(@"Fields\FieldCode\Test22027.docx");
            Field field = FieldExtractor.ExtractToCollection(document, true, FieldType.FieldEquation)[0];
            FieldArgument argument = field.FieldCodeCache.GetArgument(0);

            string argumentText = FieldCodeParser.GetArgumentTextByRange(argument);

            Assert.That(argumentText, Is.EqualTo(@"\x (1)"));
        }

        /// <summary>
        /// Tests how quotes with symbolic fonts are parsed.
        /// </summary>
        [Test]
        public void TestSymbolicQuotes()
        {
            Document document = TestUtil.Open(@"Fields\FieldCode\TestSymbolicQuotes.docx");

            FieldCollection fields = document.Range.Fields;

            AssertIndexEntryFieldArgument((FieldXE)fields[0], true, "\"A“\"", "A“");
            AssertIndexEntryFieldArgument((FieldXE)fields[1], false, "B“", "B“");
            AssertIndexEntryFieldArgument((FieldXE)fields[2], false, "“C", "“C");
            AssertIndexEntryFieldArgument((FieldXE)fields[3], false, "“", "“");
            AssertIndexEntryFieldArgument((FieldXE)fields[4], false, "“““", "“““");
        }

        private static void AssertIndexEntryFieldArgument(FieldXE field, bool isInDoubleQuotes, string text, string normalizedText)
        {
            FieldArgument argument = field.FieldCodeCache.GetArgument(0);

            Assert.That(argument.IsInDoubleQuotes, Is.EqualTo(isInDoubleQuotes));
            Assert.That(argument.GetNormalizedText(), Is.EqualTo(normalizedText));
            Assert.That(argument.Text, Is.EqualTo(text));
            ((IFieldArgument)argument).InvalidateText();
            Assert.That(argument.Text, Is.EqualTo(text));
        }

        /// <summary>
        /// WORDSNET-26596 Hyperlink address is truncated f contains Unicode characters.
        /// </summary>
        [Test]
        public void Test26596()
        {
            const string address = @"C:\Temp\tmp\⺀⺁⺂⺃¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹ÊËÌÍΑβγδεζηθικדהוזח؉؊ٓ٧٩\Parent File.docx";

            DocumentBuilder builder = new DocumentBuilder();
            FieldHyperlink field = (FieldHyperlink)builder.InsertField(FieldType.FieldHyperlink, false);

            field.Address = address;

            Assert.That(field.Address, Is.EqualTo(address));
        }

        [Test]
        public void TestEncodeDoubleQuotesInFieldArgument()
        {
            string encoded = FieldCodeUpdater.EncodeArgument(@"""«“„»”""", false);

            Assert.That(encoded, Is.EqualTo(@"\""\«\“\„\»\”\"""));
        }
    }
}
