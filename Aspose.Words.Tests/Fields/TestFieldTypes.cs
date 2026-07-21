// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests how different field types are created and modified.
    /// </summary>
    [TestFixture]
    public class TestFieldTypes : TestFieldsBase
    {
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Field type 'FieldCannotParse' is invalid or not supported.")]
        public void TestCannotParse()
        {
            CreateField(FieldType.FieldCannotParse);
        }

        [Test]
        public void TestAddIn()
        {
            FieldAddIn field = (FieldAddIn)CreateField(FieldType.FieldAddin);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAddin));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDIN "));
        }

        [Test]
        public void TestAddressBlock()
        {
            FieldAddressBlock field = (FieldAddressBlock)CreateField(FieldType.FieldAddressBlock);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAddressBlock));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDRESSBLOCK "));

            Assert.That(field.IncludeCountryOrRegionName, Is.Null);
            Assert.That(field.FormatAddressOnCountryOrRegion, Is.False);
            Assert.That(field.ExcludedCountryOrRegionName, Is.Null);
            Assert.That(field.NameAndAddressFormat, Is.Null);
            Assert.That(field.LanguageId, Is.Null);

            field.IncludeCountryOrRegionName = "1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDRESSBLOCK  \\c 1"));

            field.FormatAddressOnCountryOrRegion = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDRESSBLOCK  \\c 1 \\d"));

            field.ExcludedCountryOrRegionName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDRESSBLOCK  \\c 1 \\d \\e Test2"));

            field.NameAndAddressFormat = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDRESSBLOCK  \\c 1 \\d \\e Test2 \\f Test3"));

            field.LanguageId = "Test 4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADDRESSBLOCK  \\c 1 \\d \\e Test2 \\f Test3 \\l \"Test 4\""));

            Assert.That(field.IncludeCountryOrRegionName, Is.EqualTo("1"));
            Assert.That(field.FormatAddressOnCountryOrRegion, Is.True);
            Assert.That(field.ExcludedCountryOrRegionName, Is.EqualTo("Test2"));
            Assert.That(field.NameAndAddressFormat, Is.EqualTo("Test3"));
            Assert.That(field.LanguageId, Is.EqualTo("Test 4"));
        }

        [Test]
        public void TestAdvance()
        {
            FieldAdvance field = (FieldAdvance)CreateField(FieldType.FieldAdvance);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAdvance));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE "));

            Assert.That(field.DownOffset, Is.Null);
            Assert.That(field.LeftOffset, Is.Null);
            Assert.That(field.RightOffset, Is.Null);
            Assert.That(field.UpOffset, Is.Null);
            Assert.That(field.HorizontalPosition, Is.Null);
            Assert.That(field.VerticalPosition, Is.Null);

            field.DownOffset = "10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE  \\d 10"));

            field.LeftOffset = "10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE  \\d 10 \\l 10"));

            field.RightOffset = "-3.3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE  \\d 10 \\l 10 \\r -3.3"));

            field.UpOffset = "0";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE  \\d 10 \\l 10 \\r -3.3 \\u 0"));

            field.HorizontalPosition = "100";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE  \\d 10 \\l 10 \\r -3.3 \\u 0 \\x 100"));

            field.VerticalPosition = "100";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ADVANCE  \\d 10 \\l 10 \\r -3.3 \\u 0 \\x 100 \\y 100"));

            Assert.That(field.DownOffset, Is.EqualTo("10"));
            Assert.That(field.LeftOffset, Is.EqualTo("10"));
            Assert.That(field.RightOffset, Is.EqualTo("-3.3"));
            Assert.That(field.UpOffset, Is.EqualTo("0"));
            Assert.That(field.HorizontalPosition, Is.EqualTo("100"));
            Assert.That(field.VerticalPosition, Is.EqualTo("100"));
        }

        [Test]
        public void TestAsk()
        {
            FieldAsk field = (FieldAsk)CreateField(FieldType.FieldAsk);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAsk));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ASK "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.PromptText, Is.Null);
            Assert.That(field.DefaultResponse, Is.Null);
            Assert.That(field.PromptOnceOnMailMerge, Is.False);

            field.BookmarkName = "Test 1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ASK  \"Test 1\""));

            field.PromptText = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ASK  \"Test 1\" Test2"));

            field.DefaultResponse = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ASK  \"Test 1\" Test2 \\d Test3"));

            field.PromptOnceOnMailMerge = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" ASK  \"Test 1\" Test2 \\d Test3 \\o"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test 1"));
            Assert.That(field.PromptText, Is.EqualTo("Test2"));
            Assert.That(field.DefaultResponse, Is.EqualTo("Test3"));
            Assert.That(field.PromptOnceOnMailMerge, Is.True);
        }

        [Test]
        public void TestAuthor()
        {
            FieldAuthor field = (FieldAuthor)CreateField(FieldType.FieldAuthor);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAuthor));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTHOR "));

            Assert.That(field.AuthorName, Is.Null);

            field.AuthorName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTHOR  Test1"));

            Assert.That(field.AuthorName, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestAutoNum()
        {
            FieldAutoNum field = (FieldAutoNum)CreateField(FieldType.FieldAutoNum);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAutoNum));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTONUM "));

            Assert.That(field.SeparatorCharacter, Is.Null);

            field.SeparatorCharacter = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTONUM  \\s Test1"));

            Assert.That(field.SeparatorCharacter, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestAutoNumLgl()
        {
            FieldAutoNumLgl field = (FieldAutoNumLgl)CreateField(FieldType.FieldAutoNumLegal);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAutoNumLegal));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTONUMLGL "));

            Assert.That(field.SeparatorCharacter, Is.Null);
            Assert.That(field.RemoveTrailingPeriod, Is.False);

            field.SeparatorCharacter = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTONUMLGL  \\s Test1"));

            field.RemoveTrailingPeriod = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTONUMLGL  \\s Test1 \\e"));

            Assert.That(field.SeparatorCharacter, Is.EqualTo("Test1"));
            Assert.That(field.RemoveTrailingPeriod, Is.True);
        }

        [Test]
        public void TestAutoNumOut()
        {
            FieldAutoNumOut field = (FieldAutoNumOut)CreateField(FieldType.FieldAutoNumOutline);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAutoNumOutline));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTONUMOUT "));
        }

        [Test]
        public void TestAutoText()
        {
            FieldAutoText field = (FieldAutoText)CreateField(FieldType.FieldAutoText);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAutoText));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTOTEXT "));

            Assert.That(field.EntryName, Is.Null);

            field.EntryName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTOTEXT  Test1"));

            Assert.That(field.EntryName, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestAutoTextList()
        {
            FieldAutoTextList field = (FieldAutoTextList)CreateField(FieldType.FieldAutoTextList);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldAutoTextList));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTOTEXTLIST "));

            Assert.That(field.EntryName, Is.Null);
            Assert.That(field.ListStyle, Is.Null);
            Assert.That(field.ScreenTip, Is.Null);

            field.EntryName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTOTEXTLIST  Test1"));

            field.ListStyle = "Test 2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTOTEXTLIST  Test1 \\s \"Test 2\""));

            field.ScreenTip = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" AUTOTEXTLIST  Test1 \\s \"Test 2\" \\t Test3"));

            Assert.That(field.EntryName, Is.EqualTo("Test1"));
            Assert.That(field.ListStyle, Is.EqualTo("Test 2"));
            Assert.That(field.ScreenTip, Is.EqualTo("Test3"));
        }

        [Test]
        public void TestBarcode()
        {
            FieldBarcode field = (FieldBarcode)CreateField(FieldType.FieldBarcode);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldBarcode));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BARCODE "));

            Assert.That(field.PostalAddress, Is.Null);
            Assert.That(field.IsBookmark, Is.False);
            Assert.That(field.FacingIdentificationMark, Is.Null);
            Assert.That(field.IsUSPostalAddress, Is.False);

            field.PostalAddress = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BARCODE  Test1"));

            field.IsBookmark = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BARCODE  Test1 \\b"));

            field.FacingIdentificationMark = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BARCODE  Test1 \\b \\f Test2"));

            field.IsUSPostalAddress = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BARCODE  Test1 \\b \\f Test2 \\u"));

            Assert.That(field.PostalAddress, Is.EqualTo("Test1"));
            Assert.That(field.IsBookmark, Is.True);
            Assert.That(field.FacingIdentificationMark, Is.EqualTo("Test2"));
            Assert.That(field.IsUSPostalAddress, Is.True);
        }

        [Test]
        public void TestBibliography()
        {
            FieldBibliography field = (FieldBibliography)CreateField(FieldType.FieldBibliography);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldBibliography));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BIBLIOGRAPHY "));

            Assert.That(field.FormatLanguageId, Is.Null);
            Assert.That(field.FilterLanguageId, Is.Null);
            Assert.That(field.SourceTag, Is.Null);

            field.FormatLanguageId = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BIBLIOGRAPHY  \\l Test1"));

            field.FilterLanguageId = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BIBLIOGRAPHY  \\l Test1 \\f Test2"));

            field.SourceTag = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BIBLIOGRAPHY  \\l Test1 \\f Test2 \\m Test3"));

            Assert.That(field.FormatLanguageId, Is.EqualTo("Test1"));
            Assert.That(field.FilterLanguageId, Is.EqualTo("Test2"));
            Assert.That(field.SourceTag, Is.EqualTo("Test3"));
        }

        [Test]
        public void TestBidiOutline()
        {
            FieldBidiOutline field = (FieldBidiOutline)CreateField(FieldType.FieldBidiOutline);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldBidiOutline));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" BIDIOUTLINE "));
        }

        [Test]
        public void TestCitation()
        {
            FieldCitation field = (FieldCitation)CreateField(FieldType.FieldCitation);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldCitation));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION "));

            Assert.That(field.SourceTag, Is.Null);
            Assert.That(field.FormatLanguageId, Is.Null);
            Assert.That(field.Prefix, Is.Null);
            Assert.That(field.Suffix, Is.Null);
            Assert.That(field.SuppressAuthor, Is.False);
            Assert.That(field.SuppressTitle, Is.False);
            Assert.That(field.SuppressYear, Is.False);
            Assert.That(field.PageNumber, Is.Null);
            Assert.That(field.VolumeNumber, Is.Null);
            Assert.That(field.AnotherSourceTag, Is.Null);

            field.SourceTag = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1"));

            field.FormatLanguageId = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2"));

            field.Prefix = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3"));

            field.Suffix = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4"));

            field.SuppressAuthor = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4 \\n"));

            field.SuppressTitle = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4 \\n \\t"));

            field.SuppressYear = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4 \\n \\t \\y"));

            field.PageNumber = "Test5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4 \\n \\t \\y \\p Test5"));

            field.VolumeNumber = "Test6";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4 \\n \\t \\y \\p Test5 \\v Test6"));

            field.AnotherSourceTag = "Test7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CITATION  Test1 \\l Test2 \\f Test3 \\s Test4 \\n \\t \\y \\p Test5 \\v Test6 \\m Test7"));

            Assert.That(field.SourceTag, Is.EqualTo("Test1"));
            Assert.That(field.FormatLanguageId, Is.EqualTo("Test2"));
            Assert.That(field.Prefix, Is.EqualTo("Test3"));
            Assert.That(field.Suffix, Is.EqualTo("Test4"));
            Assert.That(field.SuppressAuthor, Is.True);
            Assert.That(field.SuppressTitle, Is.True);
            Assert.That(field.SuppressYear, Is.True);
            Assert.That(field.PageNumber, Is.EqualTo("Test5"));
            Assert.That(field.VolumeNumber, Is.EqualTo("Test6"));
            Assert.That(field.AnotherSourceTag, Is.EqualTo("Test7"));
        }

        [Test]
        public void TestComments()
        {
            FieldComments field = (FieldComments)CreateField(FieldType.FieldComments);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldComments));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" COMMENTS "));

            Assert.That(field.Text, Is.Null);

            field.Text = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" COMMENTS  Test1"));

            Assert.That(field.Text, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestCompare()
        {
            FieldCompare field = (FieldCompare)CreateField(FieldType.FieldCompare);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldCompare));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" COMPARE "));

            Assert.That(field.LeftExpression, Is.Null);
            Assert.That(field.ComparisonOperator, Is.Null);
            Assert.That(field.RightExpression, Is.Null);

            field.LeftExpression = "123";
            field.ComparisonOperator = ">=";
            field.RightExpression = "a b c";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" COMPARE  123 >= \"a b c\""));

            Assert.That(field.LeftExpression, Is.EqualTo("123"));
            Assert.That(field.ComparisonOperator, Is.EqualTo(">="));
            Assert.That(field.RightExpression, Is.EqualTo("\"a b c\""));
        }

        [Test]
        public void TestCreateDate()
        {
            FieldCreateDate field = (FieldCreateDate)CreateField(FieldType.FieldCreateDate);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldCreateDate));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CREATEDATE "));

            Assert.That(field.UseLunarCalendar, Is.False);
            Assert.That(field.UseSakaEraCalendar, Is.False);
            Assert.That(field.UseUmAlQuraCalendar, Is.False);

            field.UseLunarCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CREATEDATE  \\h"));

            field.UseSakaEraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CREATEDATE  \\h \\s"));

            field.UseUmAlQuraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" CREATEDATE  \\h \\s \\u"));

            Assert.That(field.UseLunarCalendar, Is.True);
            Assert.That(field.UseSakaEraCalendar, Is.True);
            Assert.That(field.UseUmAlQuraCalendar, Is.True);
        }

        [Test]
        public void TestData()
        {
            FieldData field = (FieldData)CreateField(FieldType.FieldData);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldData));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATA "));
        }

        [Test]
        public void TestDatabase()
        {
            FieldDatabase field = (FieldDatabase)CreateField(FieldType.FieldDatabase);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldDatabase));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE "));

            Assert.That(field.FormatAttributes, Is.Null);
            Assert.That(field.Connection, Is.Null);
            Assert.That(field.FileName, Is.Null);
            Assert.That(field.FirstRecord, Is.Null);
            Assert.That(field.InsertHeadings, Is.False);
            Assert.That(field.TableFormat, Is.Null);
            Assert.That(field.InsertOnceOnMailMerge, Is.False);
            Assert.That(field.Query, Is.Null);
            Assert.That(field.LastRecord, Is.Null);

            field.FormatAttributes = "16";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16"));

            field.Connection = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1"));

            field.FileName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2"));

            field.FirstRecord = "0";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2 \\f 0"));

            field.InsertHeadings = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2 \\f 0 \\h"));

            field.TableFormat = "2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2 \\f 0 \\h \\l 2"));

            field.InsertOnceOnMailMerge = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2 \\f 0 \\h \\l 2 \\o"));

            field.Query = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2 \\f 0 \\h \\l 2 \\o \\s Test3"));

            field.LastRecord = "10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATABASE  \\b 16 \\c Test1 \\d Test2 \\f 0 \\h \\l 2 \\o \\s Test3 \\t 10"));

            Assert.That(field.FormatAttributes, Is.EqualTo("16"));
            Assert.That(field.Connection, Is.EqualTo("Test1"));
            Assert.That(field.FileName, Is.EqualTo("Test2"));
            Assert.That(field.FirstRecord, Is.EqualTo("0"));
            Assert.That(field.InsertHeadings, Is.True);
            Assert.That(field.TableFormat, Is.EqualTo("2"));
            Assert.That(field.InsertOnceOnMailMerge, Is.True);
            Assert.That(field.Query, Is.EqualTo("Test3"));
            Assert.That(field.LastRecord, Is.EqualTo("10"));
        }

        [Test]
        public void TestDate()
        {
            FieldDate field = (FieldDate)CreateField(FieldType.FieldDate);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldDate));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE "));

            Assert.That(field.UseLunarCalendar, Is.False);
            Assert.That(field.UseLastFormat, Is.False);
            Assert.That(field.UseSakaEraCalendar, Is.False);
            Assert.That(field.UseUmAlQuraCalendar, Is.False);

            field.UseLunarCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\h"));

            field.UseLastFormat = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\h \\l"));

            field.UseSakaEraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\h \\l \\s"));

            field.UseUmAlQuraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DATE  \\h \\l \\s \\u"));

            Assert.That(field.UseLunarCalendar, Is.True);
            Assert.That(field.UseLastFormat, Is.True);
            Assert.That(field.UseSakaEraCalendar, Is.True);
            Assert.That(field.UseUmAlQuraCalendar, Is.True);
        }

        [Test]
        public void TestDde()
        {
            FieldDde field = (FieldDde)CreateField(FieldType.FieldDDE);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldDDE));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE "));

            Assert.That(field.ProgId, Is.Null);
            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.SourceItem, Is.Null);
            Assert.That(field.AutoUpdate, Is.False);
            Assert.That(field.InsertAsBitmap, Is.False);
            Assert.That(field.IsLinked, Is.False);
            Assert.That(field.InsertAsHtml, Is.False);
            Assert.That(field.InsertAsPicture, Is.False);
            Assert.That(field.InsertAsRtf, Is.False);
            Assert.That(field.InsertAsText, Is.False);
            Assert.That(field.InsertAsUnicode, Is.False);

            field.ProgId = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1"));

            field.SourceFullName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2"));

            field.SourceItem = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3"));

            field.AutoUpdate = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a"));

            field.InsertAsBitmap = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b"));

            field.IsLinked = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b \\d"));

            field.InsertAsHtml = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b \\d \\h"));

            field.InsertAsPicture = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b \\d \\h \\p"));

            field.InsertAsRtf = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b \\d \\h \\p \\r"));

            field.InsertAsText = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b \\d \\h \\p \\r \\t"));

            field.InsertAsUnicode = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDE  Test1 Test2 Test3 \\a \\b \\d \\h \\p \\r \\t \\u"));

            Assert.That(field.ProgId, Is.EqualTo("Test1"));
            Assert.That(field.SourceFullName, Is.EqualTo("Test2"));
            Assert.That(field.SourceItem, Is.EqualTo("Test3"));
            Assert.That(field.AutoUpdate, Is.True);
            Assert.That(field.InsertAsBitmap, Is.True);
            Assert.That(field.IsLinked, Is.True);
            Assert.That(field.InsertAsHtml, Is.True);
            Assert.That(field.InsertAsPicture, Is.True);
            Assert.That(field.InsertAsRtf, Is.True);
            Assert.That(field.InsertAsText, Is.True);
            Assert.That(field.InsertAsUnicode, Is.True);
        }

        [Test]
        public void TestDdeAuto()
        {
            FieldDdeAuto field = (FieldDdeAuto)CreateField(FieldType.FieldDDEAuto);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldDDEAuto));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO "));

            Assert.That(field.ProgId, Is.Null);
            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.SourceItem, Is.Null);
            Assert.That(field.InsertAsBitmap, Is.False);
            Assert.That(field.IsLinked, Is.False);
            Assert.That(field.InsertAsHtml, Is.False);
            Assert.That(field.InsertAsPicture, Is.False);
            Assert.That(field.InsertAsRtf, Is.False);
            Assert.That(field.InsertAsText, Is.False);
            Assert.That(field.InsertAsUnicode, Is.False);

            field.ProgId = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1"));

            field.SourceFullName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2"));

            field.SourceItem = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3"));

            field.InsertAsBitmap = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b"));

            field.IsLinked = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b \\d"));

            field.InsertAsHtml = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b \\d \\h"));

            field.InsertAsPicture = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b \\d \\h \\p"));

            field.InsertAsRtf = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b \\d \\h \\p \\r"));

            field.InsertAsText = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b \\d \\h \\p \\r \\t"));

            field.InsertAsUnicode = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DDEAUTO  Test1 Test2 Test3 \\b \\d \\h \\p \\r \\t \\u"));

            Assert.That(field.ProgId, Is.EqualTo("Test1"));
            Assert.That(field.SourceFullName, Is.EqualTo("Test2"));
            Assert.That(field.SourceItem, Is.EqualTo("Test3"));
            Assert.That(field.InsertAsBitmap, Is.True);
            Assert.That(field.IsLinked, Is.True);
            Assert.That(field.InsertAsHtml, Is.True);
            Assert.That(field.InsertAsPicture, Is.True);
            Assert.That(field.InsertAsRtf, Is.True);
            Assert.That(field.InsertAsText, Is.True);
            Assert.That(field.InsertAsUnicode, Is.True);

        }

        [Test]
        public void TestDocProperty()
        {
            FieldDocProperty field = (FieldDocProperty)CreateField(FieldType.FieldDocProperty);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldDocProperty));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DOCPROPERTY "));

            Assert.That(field.PropertyName, Is.Null);

            field.PropertyName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DOCPROPERTY  Test1"));

            Assert.That(field.PropertyName, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestDocVariable()
        {
            FieldDocVariable field = (FieldDocVariable)CreateField(FieldType.FieldDocVariable);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldDocVariable));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DOCVARIABLE "));

            Assert.That(field.VariableName, Is.Null);

            field.VariableName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" DOCVARIABLE  Test1"));

            Assert.That(field.VariableName, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestEditTime()
        {
            FieldEditTime field = (FieldEditTime)CreateField(FieldType.FieldEditTime);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldEditTime));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" EDITTIME "));
        }

        [Test]
        public void TestEmbed()
        {
            FieldEmbed field = (FieldEmbed)CreateField(FieldType.FieldEmbed);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldEmbed));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" EMBED "));
        }

        [Test]
        public void TestEQ()
        {
            FieldEQ field = (FieldEQ)CreateField(FieldType.FieldEquation);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldEquation));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" EQ "));
        }

        [Test]
        public void TestFileName()
        {
            FieldFileName field = (FieldFileName)CreateField(FieldType.FieldFileName);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFileName));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILENAME "));

            Assert.That(field.IncludeFullPath, Is.False);

            field.IncludeFullPath = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILENAME  \\p"));

            Assert.That(field.IncludeFullPath, Is.True);
        }

        [Test]
        public void TestFileSize()
        {
            FieldFileSize field = (FieldFileSize)CreateField(FieldType.FieldFileSize);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFileSize));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILESIZE "));

            Assert.That(field.IsInKilobytes, Is.False);
            Assert.That(field.IsInMegabytes, Is.False);

            field.IsInKilobytes = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILESIZE  \\k"));

            field.IsInMegabytes = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILESIZE  \\k \\m"));

            Assert.That(field.IsInKilobytes, Is.True);
            Assert.That(field.IsInMegabytes, Is.True);
        }

        [Test]
        public void TestFillIn()
        {
            FieldFillIn field = (FieldFillIn)CreateField(FieldType.FieldFillIn);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFillIn));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILLIN "));

            Assert.That(field.PromptText, Is.Null);
            Assert.That(field.DefaultResponse, Is.Null);
            Assert.That(field.PromptOnceOnMailMerge, Is.False);

            field.PromptText = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILLIN  Test1"));

            field.DefaultResponse = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILLIN  Test1 \\d Test2"));

            field.PromptOnceOnMailMerge = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FILLIN  Test1 \\d Test2 \\o"));

            Assert.That(field.PromptText, Is.EqualTo("Test1"));
            Assert.That(field.DefaultResponse, Is.EqualTo("Test2"));
            Assert.That(field.PromptOnceOnMailMerge, Is.True);
        }

        [Test]
        public void TestFootnoteRef()
        {
            FieldFootnoteRef field = (FieldFootnoteRef)CreateField(FieldType.FieldFootnoteRef);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFootnoteRef));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FOOTNOTEREF "));
        }

        [Test]
        public void TestFormCheckBox()
        {
            FieldFormCheckBox field = (FieldFormCheckBox)CreateField(FieldType.FieldFormCheckBox);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFormCheckBox));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FORMCHECKBOX "));
        }

        [Test]
        public void TestFormDropDown()
        {
            FieldFormDropDown field = (FieldFormDropDown)CreateField(FieldType.FieldFormDropDown);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFormDropDown));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FORMDROPDOWN "));
        }

        [Test]
        public void TestFormText()
        {
            FieldFormText field = (FieldFormText)CreateField(FieldType.FieldFormTextInput);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFormTextInput));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" FORMTEXT "));
        }

        [Test]
        public void TestFormula()
        {
            FieldFormula field = (FieldFormula)CreateField(FieldType.FieldFormula);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldFormula));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" = "));
        }

        [Test]
        public void TestGlossary()
        {
            FieldGlossary field = (FieldGlossary)CreateField(FieldType.FieldGlossary);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldGlossary));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GLOSSARY "));

            Assert.That(field.EntryName, Is.Null);

            field.EntryName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GLOSSARY  Test1"));

            Assert.That(field.EntryName, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestGoToButton()
        {
            FieldGoToButton field = (FieldGoToButton)CreateField(FieldType.FieldGoToButton);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldGoToButton));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GOTOBUTTON "));

            Assert.That(field.Location, Is.Null);
            Assert.That(field.DisplayText, Is.Null);

            field.Location = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GOTOBUTTON  Test1"));

            field.DisplayText = "Test 2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GOTOBUTTON  Test1 Test 2"));

            Assert.That(field.Location, Is.EqualTo("Test1"));
            Assert.That(field.DisplayText, Is.EqualTo("Test 2"));
        }

        [Test]
        public void TestGreetingLine()
        {
            FieldGreetingLine field = (FieldGreetingLine)CreateField(FieldType.FieldGreetingLine);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldGreetingLine));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GREETINGLINE "));

            Assert.That(field.AlternateText, Is.Null);
            Assert.That(field.NameFormat, Is.Null);
            Assert.That(field.LanguageId, Is.Null);

            field.AlternateText = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GREETINGLINE  \\e Test1"));

            field.NameFormat = "Test 2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GREETINGLINE  \\e Test1 \\f \"Test 2\""));

            field.LanguageId = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" GREETINGLINE  \\e Test1 \\f \"Test 2\" \\l Test3"));

            Assert.That(field.AlternateText, Is.EqualTo("Test1"));
            Assert.That(field.NameFormat, Is.EqualTo("Test 2"));
            Assert.That(field.LanguageId, Is.EqualTo("Test3"));
        }

        [Test]
        public void TestHyperlink()
        {
            FieldHyperlink field = (FieldHyperlink)CreateField(FieldType.FieldHyperlink);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK "));

            Assert.That(field.Address, Is.Null);
            Assert.That(field.SubAddress, Is.Null);
            Assert.That(field.IsImageMap, Is.False);
            Assert.That(field.OpenInNewWindow, Is.False);
            Assert.That(field.ScreenTip, Is.Null);
            Assert.That(field.Target, Is.Null);
            Assert.That(field.DocLocation, Is.Null);
            Assert.That(field.NoHistory, Is.False);

            field.Address = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK  Test1"));

            field.SubAddress = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK  Test1 \\l Test2"));

            field.IsImageMap = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK  Test1 \\l Test2 \\m"));

            field.OpenInNewWindow = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK  Test1 \\l Test2 \\m \\n"));

            field.ScreenTip = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK  Test1 \\l Test2 \\m \\n \\o Test3"));

            field.Target = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" HYPERLINK  Test1 \\l Test2 \\m \\n \\o Test3 \\t Test4"));

            field.DocLocation = "Test5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(@" HYPERLINK  Test1 \l Test2 \m \n \o Test3 \t Test4 \s Test5"));

            field.NoHistory = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(@" HYPERLINK  Test1 \l Test2 \m \n \o Test3 \t Test4 \s Test5 \h"));

            Assert.That(field.Address, Is.EqualTo("Test1"));
            Assert.That(field.SubAddress, Is.EqualTo("Test2"));
            Assert.That(field.IsImageMap, Is.True);
            Assert.That(field.OpenInNewWindow, Is.True);
            Assert.That(field.ScreenTip, Is.EqualTo("Test3"));
            Assert.That(field.Target, Is.EqualTo("Test4"));
            Assert.That(field.DocLocation, Is.EqualTo("Test5"));
            Assert.That(field.NoHistory, Is.True);
        }

        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Field type 'FieldHtmlActiveX' is invalid or not supported.")]
        public void TestHtmlActiveX()
        {
            CreateField(FieldType.FieldHtmlActiveX);
        }

        [Test]
        public void TestIf()
        {
            FieldIf field = (FieldIf)CreateField(FieldType.FieldIf);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldIf));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" IF "));

            Assert.That(field.LeftExpression, Is.Null);
            Assert.That(field.ComparisonOperator, Is.Null);
            Assert.That(field.RightExpression, Is.Null);
            Assert.That(field.TrueText, Is.Null);
            Assert.That(field.FalseText, Is.Null);

            field.LeftExpression = "123";
            field.ComparisonOperator = ">=";
            field.RightExpression = "a b c";
            field.TrueText = "True";
            field.FalseText = "False";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" IF  123 >= \"a b c\" True False"));

            Assert.That(field.LeftExpression, Is.EqualTo("123"));
            Assert.That(field.ComparisonOperator, Is.EqualTo(">="));
            Assert.That(field.RightExpression, Is.EqualTo("\"a b c\""));
            Assert.That(field.TrueText, Is.EqualTo("True"));
            Assert.That(field.FalseText, Is.EqualTo("False"));
        }

        [Test]
        public void TestImport()
        {
            FieldImport field = (FieldImport)CreateField(FieldType.FieldImport);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldImport));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" IMPORT "));

            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.GraphicFilter, Is.Null);
            Assert.That(field.IsLinked, Is.False);

            field.SourceFullName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" IMPORT  Test1"));

            field.GraphicFilter = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" IMPORT  Test1 \\c Test2"));

            field.IsLinked = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" IMPORT  Test1 \\c Test2 \\d"));

            Assert.That(field.SourceFullName, Is.EqualTo("Test1"));
            Assert.That(field.GraphicFilter, Is.EqualTo("Test2"));
            Assert.That(field.IsLinked, Is.True);
        }

        [Test]
        public void TestIncludePicture()
        {
            FieldIncludePicture field = (FieldIncludePicture)CreateField(FieldType.FieldIncludePicture);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldIncludePicture));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDEPICTURE "));

            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.GraphicFilter, Is.Null);
            Assert.That(field.IsLinked, Is.False);
            Assert.That(field.ResizeHorizontally, Is.False);
            Assert.That(field.ResizeVertically, Is.False);

            field.SourceFullName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDEPICTURE  Test1"));

            field.GraphicFilter = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDEPICTURE  Test1 \\c Test2"));

            field.IsLinked = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDEPICTURE  Test1 \\c Test2 \\d"));

            field.ResizeHorizontally = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDEPICTURE  Test1 \\c Test2 \\d \\x"));

            field.ResizeVertically = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDEPICTURE  Test1 \\c Test2 \\d \\x \\y"));

            Assert.That(field.SourceFullName, Is.EqualTo("Test1"));
            Assert.That(field.GraphicFilter, Is.EqualTo("Test2"));
            Assert.That(field.IsLinked, Is.True);
            Assert.That(field.ResizeHorizontally, Is.True);
            Assert.That(field.ResizeVertically, Is.True);
        }

        [Test]
        public void TestInclude()
        {
            FieldInclude field = (FieldInclude)CreateField(FieldType.FieldInclude);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldInclude));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDE "));

            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.LockFields, Is.False);
            Assert.That(field.TextConverter, Is.Null);

            field.SourceFullName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDE  Test1"));

            field.BookmarkName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDE  Test1 Test2"));

            field.LockFields = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDE  Test1 Test2 \\!"));

            field.TextConverter = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDE  Test1 Test2 \\! \\c Test3"));

            Assert.That(field.SourceFullName, Is.EqualTo("Test1"));
            Assert.That(field.BookmarkName, Is.EqualTo("Test2"));
            Assert.That(field.LockFields, Is.True);
            Assert.That(field.TextConverter, Is.EqualTo("Test3"));
        }

        [Test]
        public void TestIncludeText()
        {
            FieldIncludeText field = (FieldIncludeText)CreateField(FieldType.FieldIncludeText);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldIncludeText));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT "));

            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.LockFields, Is.False);
            Assert.That(field.TextConverter, Is.Null);
            Assert.That(field.Encoding, Is.Null);
            Assert.That(field.MimeType, Is.Null);
            Assert.That(field.NamespaceMappings, Is.Null);
            Assert.That(field.XslTransformation, Is.Null);
            Assert.That(field.XPath, Is.Null);

            field.SourceFullName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1"));

            field.BookmarkName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2"));

            field.LockFields = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\!"));

            field.TextConverter = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\! \\c Test3"));

            field.Encoding = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\! \\c Test3 \\e Test4"));

            field.MimeType = "Test5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\! \\c Test3 \\e Test4 \\m Test5"));

            field.NamespaceMappings = "Test6";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\! \\c Test3 \\e Test4 \\m Test5 \\n Test6"));

            field.XslTransformation = "Test7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\! \\c Test3 \\e Test4 \\m Test5 \\n Test6 \\t Test7"));

            field.XPath = "Test8";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INCLUDETEXT  Test1 Test2 \\! \\c Test3 \\e Test4 \\m Test5 \\n Test6 \\t Test7 \\x Test8"));

            Assert.That(field.SourceFullName, Is.EqualTo("Test1"));
            Assert.That(field.BookmarkName, Is.EqualTo("Test2"));
            Assert.That(field.LockFields, Is.True);
            Assert.That(field.TextConverter, Is.EqualTo("Test3"));
            Assert.That(field.Encoding, Is.EqualTo("Test4"));
            Assert.That(field.MimeType, Is.EqualTo("Test5"));
            Assert.That(field.NamespaceMappings, Is.EqualTo("Test6"));
            Assert.That(field.XslTransformation, Is.EqualTo("Test7"));
            Assert.That(field.XPath, Is.EqualTo("Test8"));
        }

        [Test]
        public void TestIndex()
        {
            FieldIndex field = (FieldIndex)CreateField(FieldType.FieldIndex);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldIndex));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.NumberOfColumns, Is.Null);
            Assert.That(field.HasNumberOfColumnsSwitch, Is.False);
            Assert.That(field.SequenceSeparator, Is.Null);
            Assert.That(field.EntryType, Is.Null);
            Assert.That(field.PageRangeSeparator, Is.Null);
            Assert.That(field.Heading, Is.Null);
            Assert.That(field.CrossReferenceSeparator, Is.Null);
            Assert.That(field.PageNumberListSeparator, Is.Null);
            Assert.That(field.LetterRange, Is.Null);
            Assert.That(field.RunSubentriesOnSameLine, Is.False);
            Assert.That(field.SequenceName, Is.Null);
            Assert.That(field.UseYomi, Is.False);
            Assert.That(field.LanguageId, Is.Null);

            field.BookmarkName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1"));

            field.NumberOfColumns = "4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4"));

            field.SequenceSeparator = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2"));

            field.EntryType = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3"));

            field.PageRangeSeparator = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4"));

            field.Heading = "Test5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5"));

            field.CrossReferenceSeparator = "Test6";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6"));

            field.PageNumberListSeparator = "Test7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6 \\l Test7"));

            field.LetterRange = "Test8";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6 \\l Test7 \\p Test8"));

            field.RunSubentriesOnSameLine = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6 \\l Test7 \\p Test8 \\r"));

            field.SequenceName = "Test9";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6 \\l Test7 \\p Test8 \\r \\s Test9"));

            field.UseYomi = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6 \\l Test7 \\p Test8 \\r \\s Test9 \\y"));

            field.LanguageId = "Test10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INDEX  \\b Test1 \\c 4 \\d Test2 \\f Test3 \\g Test4 \\h Test5 \\k Test6 \\l Test7 \\p Test8 \\r \\s Test9 \\y \\z Test10"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test1"));
            Assert.That(field.NumberOfColumns, Is.EqualTo("4"));
            Assert.That(field.HasNumberOfColumnsSwitch, Is.True);
            Assert.That(field.NumberOfColumnsAsInt32.Value, Is.EqualTo(4));
            Assert.That(field.SequenceSeparator, Is.EqualTo("Test2"));
            Assert.That(field.EntryType, Is.EqualTo("Test3"));
            Assert.That(field.PageRangeSeparator, Is.EqualTo("Test4"));
            Assert.That(field.Heading, Is.EqualTo("Test5"));
            Assert.That(field.CrossReferenceSeparator, Is.EqualTo("Test6"));
            Assert.That(field.PageNumberListSeparator, Is.EqualTo("Test7"));
            Assert.That(field.LetterRange, Is.EqualTo("Test8"));
            Assert.That(field.RunSubentriesOnSameLine, Is.True);
            Assert.That(field.SequenceName, Is.EqualTo("Test9"));
            Assert.That(field.UseYomi, Is.True);
            Assert.That(field.LanguageId, Is.EqualTo("Test10"));
        }

        [Test]
        public void TestInfo()
        {
            FieldInfo field = (FieldInfo)CreateField(FieldType.FieldInfo);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldInfo));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INFO "));

            Assert.That(field.InfoType, Is.Null);
            Assert.That(field.NewValue, Is.Null);

            field.InfoType = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INFO  Test1"));

            field.NewValue = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" INFO  Test1 Test2"));

            Assert.That(field.InfoType, Is.EqualTo("Test1"));
            Assert.That(field.NewValue, Is.EqualTo("Test2"));
        }

        [Test]
        public void TestKeywords()
        {
            FieldKeywords field = (FieldKeywords)CreateField(FieldType.FieldKeyword);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldKeyword));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" KEYWORDS "));

            Assert.That(field.Text, Is.Null);

            field.Text = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" KEYWORDS  Test1"));

            Assert.That(field.Text, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestLastSavedBy()
        {
            FieldLastSavedBy field = (FieldLastSavedBy)CreateField(FieldType.FieldLastSavedBy);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldLastSavedBy));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LASTSAVEDBY "));
        }

        [Test]
        public void TestLink()
        {
            FieldLink field = (FieldLink)CreateField(FieldType.FieldLink);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldLink));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK "));

            Assert.That(field.ProgId, Is.Null);
            Assert.That(field.SourceFullName, Is.Null);
            Assert.That(field.SourceItem, Is.Null);
            Assert.That(field.AutoUpdate, Is.False);
            Assert.That(field.InsertAsBitmap, Is.False);
            Assert.That(field.IsLinked, Is.False);
            Assert.That(field.FormatUpdateType, Is.Null);
            Assert.That(field.InsertAsHtml, Is.False);
            Assert.That(field.InsertAsPicture, Is.False);
            Assert.That(field.InsertAsRtf, Is.False);
            Assert.That(field.InsertAsText, Is.False);
            Assert.That(field.InsertAsUnicode, Is.False);

            field.ProgId = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1"));

            field.SourceFullName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2"));

            field.SourceItem = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3"));

            field.AutoUpdate = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a"));

            field.InsertAsBitmap = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b"));

            field.IsLinked = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d"));

            field.FormatUpdateType = "1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d \\f 1"));

            field.InsertAsHtml = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d \\f 1 \\h"));

            field.InsertAsPicture = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d \\f 1 \\h \\p"));

            field.InsertAsRtf = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d \\f 1 \\h \\p \\r"));

            field.InsertAsText = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d \\f 1 \\h \\p \\r \\t"));

            field.InsertAsUnicode = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LINK  Test1 Test2 Test3 \\a \\b \\d \\f 1 \\h \\p \\r \\t \\u"));

            Assert.That(field.ProgId, Is.EqualTo("Test1"));
            Assert.That(field.SourceFullName, Is.EqualTo("Test2"));
            Assert.That(field.SourceItem, Is.EqualTo("Test3"));
            Assert.That(field.AutoUpdate, Is.True);
            Assert.That(field.InsertAsBitmap, Is.True);
            Assert.That(field.IsLinked, Is.True);
            Assert.That(field.FormatUpdateType, Is.EqualTo("1"));
            Assert.That(field.InsertAsHtml, Is.True);
            Assert.That(field.InsertAsPicture, Is.True);
            Assert.That(field.InsertAsRtf, Is.True);
            Assert.That(field.InsertAsText, Is.True);
            Assert.That(field.InsertAsUnicode, Is.True);
        }

        [Test]
        public void TestListNum()
        {
            FieldListNum field = (FieldListNum)CreateField(FieldType.FieldListNum);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldListNum));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LISTNUM "));

            Assert.That(field.ListName, Is.Null);
            Assert.That(field.ListLevel, Is.Null);
            Assert.That(field.ListLevelAsInt32, Is.EqualTo(-1));
            Assert.That(field.ListLevel, Is.Null);
            Assert.That(field.StartingNumberAsInt32, Is.EqualTo(-1));
            Assert.That(field.StartingNumber, Is.Null);

            field.ListName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LISTNUM  Test1"));

            field.ListLevel = "3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LISTNUM  Test1 \\l 3"));

            field.StartingNumber = "1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" LISTNUM  Test1 \\l 3 \\s 1"));

            Assert.That(field.ListName, Is.EqualTo("Test1"));
            Assert.That(field.ListLevel, Is.EqualTo("3"));
            Assert.That(field.ListLevelAsInt32, Is.EqualTo(3));
            Assert.That(field.StartingNumber, Is.EqualTo("1"));
            Assert.That(field.StartingNumberAsInt32, Is.EqualTo(1));
        }

        [Test]
        public void TestMacroButton()
        {
            FieldMacroButton field = (FieldMacroButton)CreateField(FieldType.FieldMacroButton);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldMacroButton));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MACROBUTTON "));

            Assert.That(field.MacroName, Is.Null);
            Assert.That(field.DisplayText, Is.Null);

            field.MacroName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MACROBUTTON  Test1"));

            field.DisplayText = "Test 2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MACROBUTTON  Test1 Test 2"));

            Assert.That(field.MacroName, Is.EqualTo("Test1"));
            Assert.That(field.DisplayText, Is.EqualTo("Test 2"));
        }

        [Test]
        public void TestMergeField()
        {
            FieldMergeField field = (FieldMergeField)CreateField(FieldType.FieldMergeField);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldMergeField));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEFIELD "));

            Assert.That(field.FieldName, Is.Null);
            Assert.That(field.TextBefore, Is.Null);
            Assert.That(field.TextAfter, Is.Null);
            Assert.That(field.IsMapped, Is.False);
            Assert.That(field.IsVerticalFormatting, Is.False);

            field.FieldName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEFIELD  Test1"));

            field.TextBefore = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEFIELD  Test1 \\b Test2"));

            field.TextAfter = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEFIELD  Test1 \\b Test2 \\f Test3"));

            field.IsMapped = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEFIELD  Test1 \\b Test2 \\f Test3 \\m"));

            field.IsVerticalFormatting = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEFIELD  Test1 \\b Test2 \\f Test3 \\m \\v"));

            Assert.That(field.FieldName, Is.EqualTo("Test1"));
            Assert.That(field.TextBefore, Is.EqualTo("Test2"));
            Assert.That(field.TextAfter, Is.EqualTo("Test3"));
            Assert.That(field.IsMapped, Is.True);
            Assert.That(field.IsVerticalFormatting, Is.True);
        }

        [Test]
        public void TestMergeRec()
        {
            FieldMergeRec field = (FieldMergeRec)CreateField(FieldType.FieldMergeRec);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldMergeRec));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGEREC "));
        }

        [Test]
        public void TestMergeSeq()
        {
            FieldMergeSeq field = (FieldMergeSeq)CreateField(FieldType.FieldMergeSeq);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldMergeSeq));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" MERGESEQ "));
        }

        [Test]
        public void TestNext()
        {
            FieldNext field = (FieldNext)CreateField(FieldType.FieldNext);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNext));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NEXT "));
        }

        [Test]
        public void TestNextIf()
        {
            FieldNextIf field = (FieldNextIf)CreateField(FieldType.FieldNextIf);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNextIf));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NEXTIF "));

            Assert.That(field.LeftExpression, Is.Null);
            Assert.That(field.ComparisonOperator, Is.Null);
            Assert.That(field.RightExpression, Is.Null);

            field.LeftExpression = "123";
            field.ComparisonOperator = ">=";
            field.RightExpression = "a b c";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NEXTIF  123 >= \"a b c\""));

            Assert.That(field.LeftExpression, Is.EqualTo("123"));
            Assert.That(field.ComparisonOperator, Is.EqualTo(">="));
            Assert.That(field.RightExpression, Is.EqualTo("\"a b c\""));
        }

        [Test]
        public void TestFieldNoneType()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field =  builder.InsertField(FieldType.FieldNone, false);
            Assert.That(field, Is.InstanceOf(typeof(FieldUnknown)));
            List<Node> range = new List<Node>(field.GetFieldRange());
            Assert.That(range[0].NodeType, Is.EqualTo(NodeType.FieldStart));
            Assert.That(range[1].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(range[1].GetText(), Is.EqualTo("  "));
            Assert.That(range[2].NodeType, Is.EqualTo(NodeType.FieldEnd));
        }

        [Test]
        public void TestFieldNoneCode()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Field field =  builder.InsertField(string.Empty);
            Assert.That(field, Is.InstanceOf(typeof(FieldUnknown)));
            List<Node> range = new List<Node>(field.GetFieldRange());
            Assert.That(range[0].NodeType, Is.EqualTo(NodeType.FieldStart));
            Assert.That(range[1].NodeType, Is.EqualTo(NodeType.FieldEnd));
        }

        [Test]
        public void TestNoteRef()
        {
            FieldNoteRef field = (FieldNoteRef)CreateField(FieldType.FieldNoteRef);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNoteRef));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NOTEREF "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.InsertReferenceMark, Is.False);
            Assert.That(field.InsertHyperlink, Is.False);
            Assert.That(field.InsertRelativePosition, Is.False);

            field.BookmarkName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NOTEREF  Test1"));

            field.InsertReferenceMark = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NOTEREF  Test1 \\f"));

            field.InsertHyperlink = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NOTEREF  Test1 \\f \\h"));

            field.InsertRelativePosition = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NOTEREF  Test1 \\f \\h \\p"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test1"));
            Assert.That(field.InsertReferenceMark, Is.True);
            Assert.That(field.InsertHyperlink, Is.True);
            Assert.That(field.InsertRelativePosition, Is.True);
        }

        [Test]
        public void TestNumChars()
        {
            FieldNumChars field = (FieldNumChars)CreateField(FieldType.FieldNumChars);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNumChars));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NUMCHARS "));
        }

        [Test]
        public void TestNumPages()
        {
            FieldNumPages field = (FieldNumPages)CreateField(FieldType.FieldNumPages);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNumPages));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NUMPAGES "));
        }

        [Test]
        public void TestNumWords()
        {
            FieldNumWords field = (FieldNumWords)CreateField(FieldType.FieldNumWords);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNumWords));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" NUMWORDS "));
        }

        [Test]
        public void TestOcx()
        {
            FieldOcx field = (FieldOcx)CreateField(FieldType.FieldOcx);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldOcx));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" OCX "));
        }

        [Test]
        public void TestPage()
        {
            FieldPage field = (FieldPage)CreateField(FieldType.FieldPage);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPage));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PAGE "));
        }

        [Test]
        public void TestPageRef()
        {
            FieldPageRef field = (FieldPageRef)CreateField(FieldType.FieldPageRef);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPageRef));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PAGEREF "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.InsertHyperlink, Is.False);
            Assert.That(field.InsertRelativePosition, Is.False);

            field.BookmarkName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PAGEREF  Test1"));

            field.InsertHyperlink = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PAGEREF  Test1 \\h"));

            field.InsertRelativePosition = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PAGEREF  Test1 \\h \\p"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test1"));
            Assert.That(field.InsertHyperlink, Is.True);
            Assert.That(field.InsertRelativePosition, Is.True);
        }

        [Test]
        public void TestPrint()
        {
            FieldPrint field = (FieldPrint)CreateField(FieldType.FieldPrint);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPrint));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINT "));

            Assert.That(field.PrinterInstructions, Is.Null);
            Assert.That(field.PostScriptGroup, Is.Null);

            field.PrinterInstructions = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINT  Test1"));

            field.PostScriptGroup = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINT  Test1 \\p Test2"));

            Assert.That(field.PrinterInstructions, Is.EqualTo("Test1"));
            Assert.That(field.PostScriptGroup, Is.EqualTo("Test2"));
        }

        [Test]
        public void TestPrintDate()
        {
            FieldPrintDate field = (FieldPrintDate)CreateField(FieldType.FieldPrintDate);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPrintDate));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINTDATE "));

            Assert.That(field.UseLunarCalendar, Is.False);
            Assert.That(field.UseSakaEraCalendar, Is.False);
            Assert.That(field.UseUmAlQuraCalendar, Is.False);

            field.UseLunarCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINTDATE  \\h"));

            field.UseSakaEraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINTDATE  \\h \\s"));

            field.UseUmAlQuraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRINTDATE  \\h \\s \\u"));

            Assert.That(field.UseLunarCalendar, Is.True);
            Assert.That(field.UseSakaEraCalendar, Is.True);
            Assert.That(field.UseUmAlQuraCalendar, Is.True);
        }

        [Test]
        public void TestPrivate()
        {
            FieldPrivate field = (FieldPrivate)CreateField(FieldType.FieldPrivate);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPrivate));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" PRIVATE "));
        }

        [Test]
        public void TestQuote()
        {
            FieldQuote field = (FieldQuote)CreateField(FieldType.FieldQuote);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldQuote));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" QUOTE "));

            Assert.That(field.Text, Is.Null);

            field.Text = "Test 1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" QUOTE  \"Test 1\""));

            Assert.That(field.Text, Is.EqualTo("Test 1"));
        }

        [Test]
        public void TestRD()
        {
            FieldRD field = (FieldRD)CreateField(FieldType.FieldRefDoc);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldRefDoc));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" RD "));

            Assert.That(field.FileName, Is.Null);
            Assert.That(field.IsPathRelative, Is.False);

            field.FileName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" RD  Test1"));

            field.IsPathRelative = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" RD  Test1 \\f"));

            Assert.That(field.FileName, Is.EqualTo("Test1"));
            Assert.That(field.IsPathRelative, Is.True);
        }

        [Test]
        public void TestRef()
        {
            FieldRef field = (FieldRef)CreateField(FieldType.FieldRef);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldRef));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.NumberSeparator, Is.Null);
            Assert.That(field.InsertHyperlink, Is.False);
            Assert.That(field.InsertParagraphNumber, Is.False);
            Assert.That(field.InsertRelativePosition, Is.False);
            Assert.That(field.InsertParagraphNumberInRelativeContext, Is.False);
            Assert.That(field.SuppressNonDelimiters, Is.False);
            Assert.That(field.InsertParagraphNumberInFullContext, Is.False);

            field.BookmarkName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1"));

            field.NumberSeparator = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2"));

            field.InsertHyperlink = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2 \\h"));

            field.InsertParagraphNumber = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2 \\h \\n"));

            field.InsertRelativePosition = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2 \\h \\n \\p"));

            field.InsertParagraphNumberInRelativeContext = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2 \\h \\n \\p \\r"));

            field.SuppressNonDelimiters = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2 \\h \\n \\p \\r \\t"));

            field.InsertParagraphNumberInFullContext = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REF  Test1 \\d Test2 \\h \\n \\p \\r \\t \\w"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test1"));
            Assert.That(field.NumberSeparator, Is.EqualTo("Test2"));
            Assert.That(field.InsertHyperlink, Is.True);
            Assert.That(field.InsertParagraphNumber, Is.True);
            Assert.That(field.InsertRelativePosition, Is.True);
            Assert.That(field.InsertParagraphNumberInRelativeContext, Is.True);
            Assert.That(field.SuppressNonDelimiters, Is.True);
            Assert.That(field.InsertParagraphNumberInFullContext, Is.True);
        }

        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Field type 'FieldRefNoKeyword' is invalid or not supported.")]
        public void TestRefNoKeyword()
        {
            CreateField(FieldType.FieldRefNoKeyword);
        }

        [Test]
        public void TestRevNum()
        {
            FieldRevNum field = (FieldRevNum)CreateField(FieldType.FieldRevisionNum);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldRevisionNum));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" REVNUM "));
        }

        [Test]
        public void TestSaveDate()
        {
            FieldSaveDate field = (FieldSaveDate)CreateField(FieldType.FieldSaveDate);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSaveDate));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SAVEDATE "));

            Assert.That(field.UseLunarCalendar, Is.False);
            Assert.That(field.UseSakaEraCalendar, Is.False);
            Assert.That(field.UseUmAlQuraCalendar, Is.False);

            field.UseLunarCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SAVEDATE  \\h"));

            field.UseSakaEraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SAVEDATE  \\h \\s"));

            field.UseUmAlQuraCalendar = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SAVEDATE  \\h \\s \\u"));

            Assert.That(field.UseLunarCalendar, Is.True);
            Assert.That(field.UseSakaEraCalendar, Is.True);
            Assert.That(field.UseUmAlQuraCalendar, Is.True);
        }

        [Test]
        public void TestSection()
        {
            FieldSection field = (FieldSection)CreateField(FieldType.FieldSection);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSection));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SECTION "));
        }

        [Test]
        public void TestSectionPages()
        {
            FieldSectionPages field = (FieldSectionPages)CreateField(FieldType.FieldSectionPages);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSectionPages));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SECTIONPAGES "));
        }

        [Test]
        public void TestSeq()
        {
            FieldSeq field = (FieldSeq)CreateField(FieldType.FieldSequence);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSequence));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ "));

            Assert.That(field.SequenceIdentifier, Is.Null);
            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.InsertClosestPrecedingNumber, Is.False);
            Assert.That(field.HideFieldResult, Is.False);
            Assert.That(field.InsertNextNumber, Is.False);
            Assert.That(field.ResetNumber, Is.Null);
            Assert.That(field.HasResetNumberSwitch, Is.False);
            Assert.That(field.ResetHeadingLevel, Is.Null);
            Assert.That(field.HasResetHeadingLevelSwitch, Is.False);

            field.SequenceIdentifier = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1"));

            field.BookmarkName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1 Test2"));

            field.InsertClosestPrecedingNumber = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1 Test2 \\c"));

            field.HideFieldResult = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1 Test2 \\c \\h"));

            field.InsertNextNumber = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1 Test2 \\c \\h \\n"));

            field.ResetNumber = "10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1 Test2 \\c \\h \\n \\r 10"));

            field.ResetHeadingLevel = "20";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SEQ  Test1 Test2 \\c \\h \\n \\r 10 \\s 20"));

            Assert.That(field.SequenceIdentifier, Is.EqualTo("Test1"));
            Assert.That(field.BookmarkName, Is.EqualTo("Test2"));
            Assert.That(field.InsertClosestPrecedingNumber, Is.True);
            Assert.That(field.HideFieldResult, Is.True);
            Assert.That(field.InsertNextNumber, Is.True);
            Assert.That(field.ResetNumber, Is.EqualTo("10"));
            Assert.That(field.HasResetNumberSwitch, Is.True);
            Assert.That(field.ResetNumberAsInt32.Value, Is.EqualTo(10));
            Assert.That(field.ResetHeadingLevel, Is.EqualTo("20"));
            Assert.That(field.HasResetHeadingLevelSwitch, Is.True);
            Assert.That(field.ResetHeadingLevelAsInt32.Value, Is.EqualTo(20));
        }

        [Test]
        public void TestSet()
        {
            FieldSet field = (FieldSet)CreateField(FieldType.FieldSet);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSet));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SET "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.BookmarkText, Is.Null);

            field.BookmarkName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SET  Test1"));

            field.BookmarkText = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SET  Test1 Test2"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test1"));
            Assert.That(field.BookmarkText, Is.EqualTo("Test2"));
        }

        [Test]
        public void TestShape()
        {
            FieldShape field = (FieldShape)CreateField(FieldType.FieldShape);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldShape));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SHAPE "));

            Assert.That(field.Text, Is.Null);

            field.Text = "Test 1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SHAPE  \"Test 1\""));

            Assert.That(field.Text, Is.EqualTo("Test 1"));
        }

        [Test]
        public void TestSkipIf()
        {
            FieldSkipIf field = (FieldSkipIf)CreateField(FieldType.FieldSkipIf);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSkipIf));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SKIPIF "));

            Assert.That(field.LeftExpression, Is.Null);
            Assert.That(field.ComparisonOperator, Is.Null);
            Assert.That(field.RightExpression, Is.Null);

            field.LeftExpression = "123";
            field.ComparisonOperator = ">=";
            field.RightExpression = "a b c";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SKIPIF  123 >= \"a b c\""));

            Assert.That(field.LeftExpression, Is.EqualTo("123"));
            Assert.That(field.ComparisonOperator, Is.EqualTo(">="));
            Assert.That(field.RightExpression, Is.EqualTo("\"a b c\""));
        }

        [Test]
        public void TestStyleRef()
        {
            FieldStyleRef field = (FieldStyleRef)CreateField(FieldType.FieldStyleRef);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldStyleRef));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF "));

            Assert.That(field.StyleName, Is.Null);
            Assert.That(field.SearchFromBottom, Is.False);
            Assert.That(field.InsertParagraphNumber, Is.False);
            Assert.That(field.InsertRelativePosition, Is.False);
            Assert.That(field.InsertParagraphNumberInRelativeContext, Is.False);
            Assert.That(field.SuppressNonDelimiters, Is.False);
            Assert.That(field.InsertParagraphNumberInFullContext, Is.False);

            field.StyleName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1"));

            field.SearchFromBottom = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1 \\l"));

            field.InsertParagraphNumber = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1 \\l \\n"));

            field.InsertRelativePosition = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1 \\l \\n \\p"));

            field.InsertParagraphNumberInRelativeContext = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1 \\l \\n \\p \\r"));

            field.SuppressNonDelimiters = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1 \\l \\n \\p \\r \\t"));

            field.InsertParagraphNumberInFullContext = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" STYLEREF  Test1 \\l \\n \\p \\r \\t \\w"));

            Assert.That(field.StyleName, Is.EqualTo("Test1"));
            Assert.That(field.SearchFromBottom, Is.True);
            Assert.That(field.InsertParagraphNumber, Is.True);
            Assert.That(field.InsertRelativePosition, Is.True);
            Assert.That(field.InsertParagraphNumberInRelativeContext, Is.True);
            Assert.That(field.SuppressNonDelimiters, Is.True);
            Assert.That(field.InsertParagraphNumberInFullContext, Is.True);
        }

        [Test]
        public void TestSubject()
        {
            FieldSubject field = (FieldSubject)CreateField(FieldType.FieldSubject);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSubject));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SUBJECT "));

            Assert.That(field.Text, Is.Null);

            field.Text = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SUBJECT  Test1"));

            Assert.That(field.Text, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestSymbol()
        {
            FieldSymbol field = (FieldSymbol)CreateField(FieldType.FieldSymbol);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldSymbol));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL "));

            Assert.That(field.CharacterCode, Is.Null);
            Assert.That(field.IsAnsi, Is.False);
            Assert.That(field.FontName, Is.Null);
            Assert.That(field.DontAffectsLineSpacing, Is.False);
            Assert.That(field.IsShiftJis, Is.False);
            Assert.That(field.FontSize, Is.Null);
            Assert.That(field.IsUnicode, Is.False);

            field.CharacterCode = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1"));

            field.IsAnsi = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1 \\a"));

            field.FontName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1 \\a \\f Test2"));

            field.DontAffectsLineSpacing = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1 \\a \\f Test2 \\h"));

            field.IsShiftJis = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1 \\a \\f Test2 \\h \\j"));

            field.FontSize = "10.5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1 \\a \\f Test2 \\h \\j \\s 10.5"));

            field.IsUnicode = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" SYMBOL  Test1 \\a \\f Test2 \\h \\j \\s 10.5 \\u"));

            Assert.That(field.CharacterCode, Is.EqualTo("Test1"));
            Assert.That(field.IsAnsi, Is.True);
            Assert.That(field.FontName, Is.EqualTo("Test2"));
            Assert.That(field.DontAffectsLineSpacing, Is.True);
            Assert.That(field.IsShiftJis, Is.True);
            Assert.That(field.FontSize, Is.EqualTo("10.5"));
            Assert.That(field.IsUnicode, Is.True);
        }

        [Test]
        public void TestTA()
        {
            FieldTA field = (FieldTA)CreateField(FieldType.FieldTOAEntry);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTOAEntry));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA "));

            Assert.That(field.IsBold, Is.False);
            Assert.That(field.EntryCategory, Is.Null);
            Assert.That(field.IsItalic, Is.False);
            Assert.That(field.LongCitation, Is.Null);
            Assert.That(field.PageRangeBookmarkName, Is.Null);
            Assert.That(field.ShortCitation, Is.Null);

            field.IsBold = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA  \\b"));

            field.EntryCategory = "7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA  \\b \\c 7"));

            field.IsItalic = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA  \\b \\c 7 \\i"));

            field.LongCitation = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA  \\b \\c 7 \\i \\l Test1"));

            field.PageRangeBookmarkName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA  \\b \\c 7 \\i \\l Test1 \\r Test2"));

            field.ShortCitation = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TA  \\b \\c 7 \\i \\l Test1 \\r Test2 \\s Test3"));

            Assert.That(field.IsBold, Is.True);
            Assert.That(field.EntryCategory, Is.EqualTo("7"));
            Assert.That(field.IsItalic, Is.True);
            Assert.That(field.LongCitation, Is.EqualTo("Test1"));
            Assert.That(field.PageRangeBookmarkName, Is.EqualTo("Test2"));
            Assert.That(field.ShortCitation, Is.EqualTo("Test3"));
        }

        [Test]
        public void TestTC()
        {
            FieldTC field = (FieldTC)CreateField(FieldType.FieldTOCEntry);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTOCEntry));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TC "));

            Assert.That(field.Text, Is.Null);
            Assert.That(field.TypeIdentifier, Is.Null);
            Assert.That(field.EntryLevel, Is.Null);
            Assert.That(field.HasEntryLevelSwitch, Is.False);
            Assert.That(field.OmitPageNumber, Is.False);

            field.Text = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TC  Test1"));

            field.TypeIdentifier = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TC  Test1 \\f Test2"));

            field.EntryLevel = "3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TC  Test1 \\f Test2 \\l 3"));

            field.OmitPageNumber = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TC  Test1 \\f Test2 \\l 3 \\n"));

            Assert.That(field.Text, Is.EqualTo("Test1"));
            Assert.That(field.TypeIdentifier, Is.EqualTo("Test2"));
            Assert.That(field.EntryLevel, Is.EqualTo("3"));
            Assert.That(field.HasEntryLevelSwitch, Is.True);
            Assert.That(field.EntryLevelAsInt32.Value, Is.EqualTo(3));
            Assert.That(field.OmitPageNumber, Is.True);
        }

        [Test]
        public void TestTemplate()
        {
            FieldTemplate field = (FieldTemplate)CreateField(FieldType.FieldTemplate);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTemplate));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TEMPLATE "));

            Assert.That(field.IncludeFullPath, Is.False);

            field.IncludeFullPath = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TEMPLATE  \\p"));

            Assert.That(field.IncludeFullPath, Is.True);
        }

        [Test]
        public void TestTime()
        {
            FieldTime field = (FieldTime)CreateField(FieldType.FieldTime);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTime));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TIME "));
        }

        [Test]
        public void TestTitle()
        {
            FieldTitle field = (FieldTitle)CreateField(FieldType.FieldTitle);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTitle));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TITLE "));

            Assert.That(field.Text, Is.Null);

            field.Text = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TITLE  Test1"));

            Assert.That(field.Text, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestToa()
        {
            FieldToa field = (FieldToa)CreateField(FieldType.FieldTOA);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTOA));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA "));

            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.EntryCategory, Is.Null);
            Assert.That(field.SequenceSeparator, Is.Null);
            Assert.That(field.EntrySeparator, Is.Null);
            Assert.That(field.RemoveEntryFormatting, Is.False);
            Assert.That(field.PageRangeSeparator, Is.Null);
            Assert.That(field.UseHeading, Is.False);
            Assert.That(field.PageNumberListSeparator, Is.Null);
            Assert.That(field.UsePassim, Is.False);
            Assert.That(field.SequenceName, Is.Null);

            field.BookmarkName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1"));

            field.EntryCategory = "7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7"));

            field.SequenceSeparator = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2"));

            field.EntrySeparator = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3"));

            field.RemoveEntryFormatting = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3 \\f"));

            field.PageRangeSeparator = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3 \\f \\g Test4"));

            field.UseHeading = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3 \\f \\g Test4 \\h"));

            field.PageNumberListSeparator = "Test6";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3 \\f \\g Test4 \\h \\l Test6"));

            field.UsePassim = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3 \\f \\g Test4 \\h \\l Test6 \\p"));

            field.SequenceName = "Test7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOA  \\b Test1 \\c 7 \\d Test2 \\e Test3 \\f \\g Test4 \\h \\l Test6 \\p \\s Test7"));

            Assert.That(field.BookmarkName, Is.EqualTo("Test1"));
            Assert.That(field.EntryCategory, Is.EqualTo("7"));
            Assert.That(field.SequenceSeparator, Is.EqualTo("Test2"));
            Assert.That(field.EntrySeparator, Is.EqualTo("Test3"));
            Assert.That(field.RemoveEntryFormatting, Is.True);
            Assert.That(field.PageRangeSeparator, Is.EqualTo("Test4"));
            Assert.That(field.UseHeading, Is.True);
            Assert.That(field.PageNumberListSeparator, Is.EqualTo("Test6"));
            Assert.That(field.UsePassim, Is.True);
            Assert.That(field.SequenceName, Is.EqualTo("Test7"));
        }

        [Test]
        public void TestToc()
        {
            FieldToc field = (FieldToc)CreateField(FieldType.FieldTOC);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldTOC));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC "));

            Assert.That(field.CaptionlessTableOfFiguresLabel, Is.Null);
            Assert.That(field.BookmarkName, Is.Null);
            Assert.That(field.TableOfFiguresLabel, Is.Null);
            Assert.That(field.SequenceSeparator, Is.Null);
            Assert.That(field.EntryIdentifier, Is.Null);
            Assert.That(field.InsertHyperlinks, Is.False);
            Assert.That(field.EntryLevelRange, Is.Null);
            Assert.That(field.PageNumberOmittingLevelRange, Is.Null);
            Assert.That(field.HeadingLevelRange, Is.Null);
            Assert.That(field.EntrySeparator, Is.Null);
            Assert.That(field.PrefixedSequenceIdentifier, Is.Null);
            Assert.That(field.CustomStyles, Is.Null);
            Assert.That(field.UseParagraphOutlineLevel, Is.False);
            Assert.That(field.PreserveTabs, Is.False);
            Assert.That(field.PreserveLineBreaks, Is.False);
            Assert.That(field.HideInWebLayout, Is.False);

            field.CaptionlessTableOfFiguresLabel = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1"));

            field.BookmarkName = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2"));

            field.TableOfFiguresLabel = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3"));

            field.SequenceSeparator = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4"));

            field.EntryIdentifier = "Test5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5"));

            field.InsertHyperlinks = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h"));

            field.EntryLevelRange = "Test6";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6"));

            field.PageNumberOmittingLevelRange = "Test7";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7"));

            field.HeadingLevelRange = "Test8";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8"));

            field.EntrySeparator = "Test9";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9"));

            field.PrefixedSequenceIdentifier = "Test10";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9 \\s Test10"));

            field.CustomStyles = "Test11";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9 \\s Test10 \\t Test11"));

            field.UseParagraphOutlineLevel = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9 \\s Test10 \\t Test11 \\u"));

            field.PreserveTabs = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9 \\s Test10 \\t Test11 \\u \\w"));

            field.PreserveLineBreaks = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9 \\s Test10 \\t Test11 \\u \\w \\x"));

            field.HideInWebLayout = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" TOC  \\a Test1 \\b Test2 \\c Test3 \\d Test4 \\f Test5 \\h \\l Test6 \\n Test7 \\o Test8 \\p Test9 \\s Test10 \\t Test11 \\u \\w \\x \\z"));

            Assert.That(field.CaptionlessTableOfFiguresLabel, Is.EqualTo("Test1"));
            Assert.That(field.BookmarkName, Is.EqualTo("Test2"));
            Assert.That(field.TableOfFiguresLabel, Is.EqualTo("Test3"));
            Assert.That(field.SequenceSeparator, Is.EqualTo("Test4"));
            Assert.That(field.EntryIdentifier, Is.EqualTo("Test5"));
            Assert.That(field.InsertHyperlinks, Is.True);
            Assert.That(field.EntryLevelRange, Is.EqualTo("Test6"));
            Assert.That(field.PageNumberOmittingLevelRange, Is.EqualTo("Test7"));
            Assert.That(field.HeadingLevelRange, Is.EqualTo("Test8"));
            Assert.That(field.EntrySeparator, Is.EqualTo("Test9"));
            Assert.That(field.PrefixedSequenceIdentifier, Is.EqualTo("Test10"));
            Assert.That(field.CustomStyles, Is.EqualTo("Test11"));
            Assert.That(field.UseParagraphOutlineLevel, Is.True);
            Assert.That(field.PreserveTabs, Is.True);
            Assert.That(field.PreserveLineBreaks, Is.True);
            Assert.That(field.HideInWebLayout, Is.True);
        }

        [Test]
        public void TestUserAddress()
        {
            FieldUserAddress field = (FieldUserAddress)CreateField(FieldType.FieldUserAddress);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldUserAddress));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" USERADDRESS "));

            Assert.That(field.UserAddress, Is.Null);

            field.UserAddress = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" USERADDRESS  Test1"));

            Assert.That(field.UserAddress, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestUserInitials()
        {
            FieldUserInitials field = (FieldUserInitials)CreateField(FieldType.FieldUserInitials);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldUserInitials));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" USERINITIALS "));

            Assert.That(field.UserInitials, Is.Null);

            field.UserInitials = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" USERINITIALS  Test1"));

            Assert.That(field.UserInitials, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestUserName()
        {
            FieldUserName field = (FieldUserName)CreateField(FieldType.FieldUserName);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldUserName));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" USERNAME "));

            Assert.That(field.UserName, Is.Null);

            field.UserName = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" USERNAME  Test1"));

            Assert.That(field.UserName, Is.EqualTo("Test1"));
        }

        [Test]
        public void TestXE()
        {
            FieldXE field = (FieldXE)CreateField(FieldType.FieldIndexEntry);

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldIndexEntry));
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE "));

            Assert.That(field.Text, Is.Null);
            Assert.That(field.IsBold, Is.False);
            Assert.That(field.EntryType, Is.Null);
            Assert.That(field.IsItalic, Is.False);
            Assert.That(field.PageRangeBookmarkName, Is.Null);
            Assert.That(field.PageNumberReplacement, Is.Null);
            Assert.That(field.Yomi, Is.Null);

            field.Text = "Test1";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1"));

            field.IsBold = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1 \\b"));

            field.EntryType = "Test2";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1 \\b \\f Test2"));

            field.IsItalic = true;
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1 \\b \\f Test2 \\i"));

            field.PageRangeBookmarkName = "Test3";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1 \\b \\f Test2 \\i \\r Test3"));

            field.PageNumberReplacement = "Test4";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1 \\b \\f Test2 \\i \\r Test3 \\t Test4"));

            field.Yomi = "Test5";
            Assert.That(field.GetFieldCode(), Is.EqualTo(" XE  Test1 \\b \\f Test2 \\i \\r Test3 \\t Test4 \\y Test5"));

            Assert.That(field.Text, Is.EqualTo("Test1"));
            Assert.That(field.IsBold, Is.True);
            Assert.That(field.EntryType, Is.EqualTo("Test2"));
            Assert.That(field.IsItalic, Is.True);
            Assert.That(field.PageRangeBookmarkName, Is.EqualTo("Test3"));
            Assert.That(field.PageNumberReplacement, Is.EqualTo("Test4"));
            Assert.That(field.Yomi, Is.EqualTo("Test5"));
        }


        /// <summary>
        /// WORDSNET-5113 In ASK field answer is stored in a bookmark. If used this bookmark in IF field, MS Word takes a value from ASK field..
        /// </summary>
        [Test]
        public void TestDefect5113()
        {
            Document doc = TestUtil.Open(@"Fields\FieldTypes\TestDefect5113.docx");
            doc.UpdateFields();
            TestUtil.SaveCheckGold(doc, @"Fields\FieldTypes\TestDefect5113.docx");
        }

        /// <summary>
        /// WORDSNET-5020 If the field code is enclosed in quotes, it should be parsed correctly
        /// </summary>
        [Test]
        public void TestDefect5020()
        {
            Document doc = TestUtil.Open(@"Fields\FieldTypes\TestDefect5020.docx");
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc, false);
            Assert.That(fields[0], Is.InstanceOf(typeof(FieldCreateDate)));
        }

        private Field CreateField(FieldType fieldType)
        {
            if (mDoc == null)
                mDoc = new Document();

            DocumentBuilder builder = new DocumentBuilder(mDoc);
            return builder.InsertField(fieldType, true);
        }

        /// <summary>
        /// Use this method to generate code inside FieldUtil.FieldTypeToString() switch.
        /// </summary>
        [Test, Ignore("Not a test."), JavaDelete("Not needed on java.")]
        public void GenerateFieldTypeToString()
        {
            List<string> list = new List<string>();
            foreach (FieldType value in Enum.GetValues(typeof(FieldType)))
                list.Add(value.ToString());

            list.Sort();
            foreach (string name in list)
            {
                Console.Out.WriteLine(
                    "                case FieldType.{0}:\r\n" +
                    "                    return \"{0}\";", name);
            }
        }

        /// <summary>
        /// WORDSNET-3602 Consider updating invalid IF fields.
        /// </summary>
        [Test]
        public void TestJira3602()
        {
            Document doc = TestUtil.OpenUpdateFields(@"Fields\FieldTypes\TestJira3602.docx");
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc, false);

            Assert.That(fields[0].Result, Is.EqualTo("True Text"));
            Assert.That(fields[1].Result, Is.EqualTo("1"));
            Assert.That(fields[2].Result, Is.EqualTo("0"));
            Assert.That(fields[3].Result, Is.EqualTo("-1"));
            Assert.That(fields[4].Result, Is.EqualTo("*1"));
            Assert.That(fields[5].Result, Is.EqualTo("aaa1"));
            Assert.That(fields[6].Result, Is.EqualTo("Error! Unknown op code for conditional."));
            Assert.That(fields[7].Result, Is.EqualTo("Error! Unknown op code for conditional."));
            Assert.That(fields[8].Result, Is.EqualTo("True Text"));
            Assert.That(fields[9].Result, Is.EqualTo("Error! Unknown op code for conditional."));
            Assert.That(fields[10].Result, Is.EqualTo("True Text"));
            Assert.That(fields[11].Result, Is.EqualTo("True Text"));
            Assert.That(fields[12].Result, Is.EqualTo("True Text"));
            Assert.That(fields[13].Result, Is.EqualTo("True Text"));
            Assert.That(fields[14].Result, Is.EqualTo("True Text"));
            Assert.That(fields[15].Result, Is.EqualTo("True Text"));
            Assert.That(fields[16].Result, Is.EqualTo("False Text"));
        }

        /// <summary>
        /// WORDSNET-4102 Portuguese style name in STYLEREF field was not translated.
        /// </summary>
        [Test]
        public void TestJira4102()
        {
            Document doc = TestUtil.OpenUpdateFields(@"Fields\FieldTypes\TestJira4102.docx");

            // No update page layout in FOSS.

            string asText;
            using (MemoryStream stream = TestUtil.Save(doc, SaveFormat.Text))
            using (StreamReader reader = new StreamReader(stream))
                asText = reader.ReadToEnd();

            if (asText.Contains(FieldStyleRef.NoTextOfStyleErrorMessage))
                Assert.Fail("Portuguese style name is not translated on updating STYLEREF field.");
        }

        /// <summary>
        /// Tests how the field types normalization works.
        /// </summary>
        [Test]
        public void TestNormalizeFieldTypes()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertField(" DATE ", null);

            Document document = builder.Document;
            Run run = (Run)document.GetNodeById("1.0.0.0");
            run.Text = @" PAGE \* roman ";

            document.NormalizeFieldTypes();

            Field field = FieldExtractor.ExtractToCollection(document)[0];
            Assert.That(field, Is.InstanceOf(typeof(FieldPage)));
            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPage));
            Assert.That(field.Start.FieldType, Is.EqualTo(FieldType.FieldPage));
            Assert.That(field.Separator.FieldType, Is.EqualTo(FieldType.FieldPage));
            Assert.That(field.End.FieldType, Is.EqualTo(FieldType.FieldPage));
        }

        /// <summary>
        /// WORDSNET-13246 Replacement of URL (UNC) is losing \ and breaking links (FieldHyperlink.Address).
        /// WORDSNET-13247 FieldHyperlink.Address ignores slashes ("\") while replacing hyperlinks.
        /// </summary>
        [Test]
        public void TestJira13246And13247()
        {
            DocumentBuilder builder = new DocumentBuilder();
            FieldHyperlink field = (FieldHyperlink)builder.InsertField(@" HYPERLINK \\\\host\\folder\\file.docx");

            field.Address = @"\\new host\new folder\new file.docx";

            Assert.That(field.Address, Is.EqualTo(@"\\new host\new folder\new file.docx"));
            Assert.That(field.GetFieldCode(), Is.EqualTo(@" HYPERLINK ""\\\\new host\\new folder\\new file.docx"""));
        }


        /// <summary>
        /// WORDSNET-10033 Field.Type return incorrect value for SaveDate field when it is hidden.
        /// </summary>
        [Test]
        public void Test10033()
        {
            Document document = TestUtil.Open(@"Fields\FieldTypes\Test10033.docx");

            Field field = document.Range.Fields[0];

            Assert.That(field.Type, Is.EqualTo(FieldType.FieldNone));
        }

        /// <summary>
        /// Required for C++.
        /// </summary>
        private Document mDoc;
    }
}
