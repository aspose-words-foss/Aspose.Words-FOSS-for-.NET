// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestFieldBuilderSimpleArguments : TestFieldBuilderBase
    {
        [Test]
        public void TestSimpleStringArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldRef)
                .AddArgument("StringArgument")
                .AddArgument("NextStringArgument");

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldRef)
                .Contains().Start().Text(" REF StringArgument NextStringArgument ").Separator().End().AndNoMore();
        }

        [Test]
        public void TestComplexStringArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldStyleRef)
                .AddArgument(" string argument with whitespaces ")
                .AddArgument("string argument \"with\" qoutes")
                .AddArgument("string argument \\with\\ slashes")
                .AddArgument("\\\\share name\\folder name\\file name.txt");

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldStyleRef)
                .Contains().Start()
                .Text(" STYLEREF ")
                .Text("\" string argument with whitespaces \" ")
                .Text("\"string argument \\\"with\\\" qoutes\" ")
                .Text("\"string argument \\\\with\\\\ slashes\" ")
                .Text("\"\\\\\\\\share name\\\\folder name\\\\file name.txt\" ")
                .Separator().End().AndNoMore();
        }

        [Test]
        public void TestIntArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldIncludeText)
                .AddArgument(77)
                .AddArgument(0)
                .AddArgument(-321);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldIncludeText)
                .Contains().Start().Text(" INCLUDETEXT 77 0 -321 ").Separator().End().AndNoMore();
        }

        [Test]
        public void TestDoubleArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldTime)
                .AddArgument(35)
                .AddArgument(0.1)
                .AddArgument(-17.9);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldTime)
                .Contains().Start().Text(" TIME 35 0.1 -17.9 ").Separator().End().AndNoMore();
        }

        [Test]
        public void TestMultipleArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldFileName)
                .AddArgument("one")
                .AddArgument(2)
                .AddArgument("one two three")
                .AddArgument(4.5);

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldFileName)
                .Contains().Start().Text(" FILENAME one 2 \"one two three\" 4.5 ").Separator().End().AndNoMore();
        }
    }
}