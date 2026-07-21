// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.Common;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestFieldBuilderSwitches : TestFieldBuilderBase
    {
        [Test]
        public void TestSimpleSwitches()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldListNum)
                .AddSwitch("l")
                .AddSwitch("s");

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldListNum)
                .Contains().Start().Text(" LISTNUM \\l \\s ").End().AndNoMore();
        }

        [Test]
        public void TestStringSwitches()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldBarcode)
                .AddSwitch("u", "value")
                .AddSwitch("f", "one more value")
                .AddSwitch("*", "MERGEFORMAT");

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldBarcode)
                .Contains().Start().Text(" BARCODE \\u value \\f \"one more value\" \\* MERGEFORMAT ").End().AndNoMore();
        }

        [Test]
        public void TestIntSwitches()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldNoteRef)
                .AddSwitch("h", -8)
                .AddSwitch("p", 44);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldNoteRef)
                .Contains().Start().Text(" NOTEREF \\h -8 \\p 44 ").Separator().End().AndNoMore();
        }

        [Test]
        public void TestDoubleSwitches()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldSymbol)
                .AddSwitch("j", 39.1)
                .AddSwitch("a", 15.15);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldSymbol)
                .Contains().Start().Text(" SYMBOL \\j 39.1 \\a 15.15 ").End().AndNoMore();
        }

        [Test]
        public void TestSwitchesWithBackslash()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldDate)
                .AddSwitch("\\h")
                .AddSwitch("\\l", "value")
                .AddSwitch("\\s", 42)
                .AddSwitch("\\u", 66.66)
                .AddSwitch("\\@", "MM/yy");

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldDate)
                .Contains().Start().Text(" DATE \\h \\l value \\s 42 \\u 66.66 \\@ MM/yy ").Separator().End().AndNoMore();
        }

        [Test]
        [TestCase("ru-RU", "82,37")]
        [TestCase("de-DE", "82,37")]
        [TestCase("en-GB", "82.37")]
        public void TestDoubleSwitchCulture(string culture, string expectedText)
        {
            SystemPal.SetCulture(culture);

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldAutoNum)
                .AddSwitch("s", 82.37);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldAutoNum)
                .Contains().Start().Text(" AUTONUM \\s ").Text(expectedText).Text(" ").End().AndNoMore();
        }
    }
}
