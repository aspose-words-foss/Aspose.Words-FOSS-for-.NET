// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/01/2016 by Edward Voronov

using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    [TestFixture]
    public class TestFieldBuilderPositions : TestFieldBuilderBase
    {
        [Test]
        public void TestInsertIntoParagraph()
        {
            Document document = BuildTestDocument();
            Paragraph inside = (Paragraph) document.GetNodeById("1.0.0");
            Run after = (Run) document.GetNodeById("4.1.0.0");

            Field field = new FieldBuilder(FieldType.FieldPage)
                .BuildAndInsert(document.FirstSection.Body.Paragraphs[1]);

            FieldAssert.Field(field)
                .Inside(inside)
                .Between(after, null);
        }

        [Test]
        public void TestInsertIntoEmptyParagraph()
        {
            Document document = BuildTestDocument();
            Paragraph inside = (Paragraph)document.GetNodeById("2.0.0");

            Field field = new FieldBuilder(FieldType.FieldPage)
                .BuildAndInsert(document.FirstSection.Body.Paragraphs[2]);

            FieldAssert.Field(field)
                .Inside(inside)
                .Between(null, null);
        }

        [Test]
        public void TestInsertBeforeRun()
        {
            Document document = BuildTestDocument();
            Paragraph inside = (Paragraph)document.GetNodeById("1.0.0");
            Run after = (Run)document.GetNodeById("1.1.0.0");
            Run before = (Run)document.GetNodeById("2.1.0.0");

            Field field = new FieldBuilder(FieldType.FieldPage)
                .BuildAndInsert(document.FirstSection.Body.Paragraphs[1].Runs[2]);

            FieldAssert.Field(field)
                .Inside(inside)
                .Between(after, before);
        }

        private static Document BuildTestDocument()
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.Write("I. ");
            builder.Write("This ");
            builder.Write("is ");
            builder.Write("first ");
            builder.Write("paragraph.");
            builder.Writeln();

            builder.Write("II. ");
            builder.Write("This ");
            builder.Write("is ");
            builder.Write("second ");
            builder.Write("paragraph.");
            builder.Writeln();

            return builder.Document;
        }
    }
}
