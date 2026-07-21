// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestFieldBuilderCompositeArguments : TestFieldBuilderBase
    {
        [Test]
        public void TestFieldBuilderArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldFormula)
                .AddArgument(new FieldBuilder(FieldType.FieldStyleRef).AddArgument("Heading 1"))
                .AddArgument("+")
                .AddArgument(new FieldBuilder(FieldType.FieldFormula).AddArgument(3));

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldFormula)
                .Contains()
                    .Start().Text(" = ")
                    .Start().Text(" STYLEREF \"Heading 1\" ").Separator().End()
                    .Text(" + ")
                    .Start().Text(" = 3 ").Separator().End()
                    .Text(" ").Separator().End().AndNoMore();
        }

        [Test]
        public void TestFieldArgumentBuilderWithRunArguments()
        {
            Document document = new Document();

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldIf)
                .AddArgument("left expression")
                .AddArgument("=")
                .AddArgument("right expression")
                .AddArgument(new FieldArgumentBuilder().AddText("true part ").AddNode(new Run(document, "expressions are equal")))
                .AddArgument(new FieldArgumentBuilder().AddText("false part ").AddNode(new Run(document, "expressions are not equal")));

            Field field = BuildAndInsert(fieldBuilder, document);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldIf)
                .Contains()
                    .Start()
                    .Text(" IF \"left expression\" = \"right expression\" \"true part expressions are equal\" \"false part expressions are not equal\" ")
                    .Separator()
                    .End()
                    .AndNoMore();
        }

        [Test]
        public void TestFieldArgumentBuilderWithParagraphBreakArguments()
        {
            DocumentBuilder documentBuilder = new DocumentBuilder();
            documentBuilder.Write("Run 1");
            documentBuilder.Write("Run 2");
            documentBuilder.Write("Run 3");

            Document document = documentBuilder.Document;
            Run after = (Run)document.GetNodeById("1.0.0.0");
            Run before = (Run)document.GetNodeById("2.0.0.0");

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldIf)
                .AddArgument("left expression")
                .AddArgument("=")
                .AddArgument("right expression")
                .AddArgument(new FieldArgumentBuilder().AddText("\rexpressions are equal"))
                .AddArgument(new FieldArgumentBuilder().AddText("\rexpressions are not equal"));

            Field field = fieldBuilder.BuildAndInsert(document.FirstSection.Body.FirstParagraph.Runs[2]);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldIf)
                .Inside((Paragraph)document.GetNodeById("0.0.0"), (Paragraph)document.GetNodeById("2.0.0"))
                .Between(after, before)
                .Contains()
                    .Start().Text(" IF \"left expression\" = \"right expression\" \"").ParagraphBreak()
                    .Text("expressions are equal\" \"").ParagraphBreak()
                    .Text("expressions are not equal\" ").Separator().End().AndNoMore();
        }

        [Test]
        public void TestFieldArgumentBuilderWithFieldBuilderArguments()
        {
            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldIf)
                .AddArgument("left expression")
                .AddArgument("=")
                .AddArgument("right expression")
                .AddArgument(
                    new FieldArgumentBuilder()
                        .AddText("Firstname: ")
                        .AddField(new FieldBuilder(FieldType.FieldMergeField).AddArgument("firstname")))
                .AddArgument(
                    new FieldArgumentBuilder()
                        .AddText("Secondname: ")
                        .AddField(new FieldBuilder(FieldType.FieldMergeField).AddArgument("secondname")));

            Field field = BuildAndInsert(fieldBuilder);

            FieldAssert.Field(field)
                .OfType(FieldType.FieldIf)
                .Contains()
                    .Start().Text(" IF \"left expression\" = \"right expression\" \"Firstname: ")
                    .Start().Text(" MERGEFIELD firstname ").Separator().End()
                    .Text("\" \"Secondname: ")
                    .Start().Text(" MERGEFIELD secondname ").Separator().End()
                    .Text("\" ").Separator().End().AndNoMore();
        }
    }
}
