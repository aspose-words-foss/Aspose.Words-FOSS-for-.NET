// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2020 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Table feature.
    /// </summary>
    public class TestTableParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple table without leading and trailing separator.
        /// </summary>
        [Test]
        public void TestTableA()
        {
            MarkdownDocument doc = Open("TestTableA.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(table.Count, Is.EqualTo(1));

            RowBlock row = (RowBlock)table[0];
            Assert.That(row.Count, Is.EqualTo(2));
            Assert.That(row.Text, Is.EqualTo("|Foo|bar|"));
        }

        /// <summary>
        /// Tests simple table consisting only from the two delimiter rows.
        /// </summary>
        [Test]
        public void TestTableB()
        {
            MarkdownDocument doc = Open("TestTableB.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(table.Count, Is.EqualTo(1));

            RowBlock row = (RowBlock)table[0];
            Assert.That(row.Count, Is.EqualTo(2));
            Assert.That(row.Text, Is.EqualTo("|-|-|"));
        }

        /// <summary>
        /// Tests table with header row that has no terminal separators, but with data row that has them.
        /// </summary>
        [Test]
        public void TestTableC()
        {
            MarkdownDocument doc = Open("TestTableC.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(table.Count, Is.EqualTo(3));

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|a|b|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|c|d|"));
            Assert.That(((RowBlock)table[2]).Text, Is.EqualTo("|-|-|"));
        }

        /// <summary>
        /// Tests table with many columns.
        /// </summary>
        [Test]
        public void TestTableD()
        {
            MarkdownDocument doc = Open("TestTableD.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(table.Count, Is.EqualTo(2));

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|1|2|3|4|5|6|7|8|9|10|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|11|12|13|14|15|16|17|18|19|20|"));
        }

        /// <summary>
        /// Tests number of columns in delimiter and header rows must be equal.
        /// </summary>
        [Test]
        public void TestTableE()
        {
            MarkdownDocument doc = Open("TestTableE.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "1|2|3|"+SoftBreak+"|-|-|");
            CheckParagraph(doc[1], "1|2"+SoftBreak+"|-|-|-");
        }

        /// <summary>
        /// Tests cells with indentation and alignment.
        /// </summary>
        [Test]
        public void TestTableF()
        {
            MarkdownDocument doc = Open("TestTableF.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(table.Count, Is.EqualTo(2));

            RowBlock row = (RowBlock)table[0];
            Assert.That(row.Text, Is.EqualTo("|1|2|3|4|"));
            Assert.That(((CellBlock)row[0]).Alignment, Is.EqualTo(ParagraphAlignment.Left));
            Assert.That(((CellBlock)row[1]).Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(((CellBlock)row[2]).Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(((CellBlock)row[3]).Alignment, Is.EqualTo(ParagraphAlignment.Center));

            row = (RowBlock)table[1];
            Assert.That(row.Text, Is.EqualTo("|foo|bar|bop|baz|"));
            Assert.That(((CellBlock)row[0]).Alignment, Is.EqualTo(ParagraphAlignment.Left));
            Assert.That(((CellBlock)row[1]).Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(((CellBlock)row[2]).Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(((CellBlock)row[3]).Alignment, Is.EqualTo(ParagraphAlignment.Center));
        }

        /// <summary>
        /// Tests how table is created from the paragraph with multiple lines.
        /// </summary>
        [Test]
        public void TestTableG()
        {
            MarkdownDocument doc = Open("TestTableG.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "1  |  2  |  3  |");

            TableBlock table = (TableBlock)doc[1];
            Assert.That(table.Count, Is.EqualTo(2));

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|4|5|6|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|foo|bar|baz|"));
        }

        /// <summary>
        /// Tests that delimiter row can be indented with more than 3 characters.
        /// </summary>
        [Test]
        public void TestTableH()
        {
            MarkdownDocument doc = Open("TestTableH.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(table.Count, Is.EqualTo(2));

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|1|2|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|3|4|"));
        }

        /// <summary>
        /// Tests table cannot be created from the indented code block.
        /// </summary>
        [Test]
        public void TestTableJ()
        {
            MarkdownDocument doc = Open("TestTableJ.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckIndentedCode(doc[0], "1  |  2 ");
            CheckParagraph(doc[1], "--|--"+SoftBreak+"3  |  4");
        }

        /// <summary>
        /// Tests that delimiter row must have at least one cell separator to be valid.
        /// </summary>
        [Test]
        public void TestTableK()
        {
            MarkdownDocument doc = Open("TestTableK.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckSetextHeading(doc[0], 2, "1|2");
        }

        /// <summary>
        /// Tests recognizing table with cell separator inside inline code block.
        /// </summary>
        [Ignore("WORDSNET-20302")]
        [Test]
        public void TestTableL()
        {
            MarkdownDocument doc = Open("TestTableL.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "1`|`2\v-|-");
        }

        /// <summary>
        /// Tests that two consequent alignment characters are not allowed in content of cell.
        /// </summary>
        [Test]
        public void TestTableM()
        {
            MarkdownDocument doc = Open("TestTableM.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "1|2 "+SoftBreak+"-::|-");
        }

        /// <summary>
        /// Tests table is allowed inside Quote.
        /// </summary>
        [Test]
        public void TestTableN()
        {
            MarkdownDocument doc = Open("TestTableN.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quote = (QuoteBlock)doc[0];
            TableBlock table = (TableBlock)quote[0];

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|1|2|"));
        }

        /// <summary>
        /// Tests that paragraph must be at the same Quote level as the table to became the Row of the table.
        /// </summary>
        [Test]
        public void TestTableO()
        {
            MarkdownDocument doc = Open("TestTableO.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quote = (QuoteBlock)doc[0];
            QuoteBlock nestedQuote = (QuoteBlock)quote[0];

            TableBlock table = (TableBlock)nestedQuote[0];
            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|1|2|"));

            CheckParagraph(quote[1], "3|4");
        }

        /// <summary>
        /// Tests blank line breaks table.
        /// </summary>
        [Test]
        public void TestTableP()
        {
            MarkdownDocument doc = Open("TestTableP.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            TableBlock table = (TableBlock)doc[0];
            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|1|2|"));

            table = (TableBlock)doc[1];
            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|3|4|"));
        }

        /// <summary>
        /// Tests table cannot be inside a list.
        /// </summary>
        [Test]
        public void TestTableQ()
        {
            MarkdownDocument doc = Open("TestTableQ.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "1|2");
            CheckOrderedListItem(doc[1], 1, ListMarker.Dot, "-|-");
        }

        /// <summary>
        /// Tests various formatting of inlines in table cells.
        /// </summary>
        [Test]
        public void TestTableR()
        {
            MarkdownDocument doc = Open("TestTableR.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];

            RowBlock row = (RowBlock)table[0];
            Assert.That(row.Text, Is.EqualTo("|f**o**o|b*a*r|b~~a~~z|"));
            CheckInlines(row[0], new[] {
                new ExpectedInline(BlockType.Inline, "f"),
                new ExpectedInline(BlockType.BoldInline, "**o**"),
                new ExpectedInline(BlockType.Inline, "o")});

            CheckInlines(row[1],
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "b"),
                    new ExpectedInline(BlockType.ItalicInline, "*a*"),
                    new ExpectedInline(BlockType.Inline, "r")
                });

            CheckInlines(row[2],
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "b"),
                    new ExpectedInline(BlockType.Strikethrough, "~~a~~"),
                    new ExpectedInline(BlockType.Inline, "z")
                });
        }

        /// <summary>
        /// Tests the table is broken at the beginning of another block-level structure.
        /// </summary>
        [Test]
        public void TestTableS()
        {
            MarkdownDocument doc = Open("TestTableS.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            TableBlock table = (TableBlock)doc[0];
            QuoteBlock quote = (QuoteBlock)doc[1];

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|abc|def|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|bar|baz|"));

            CheckParagraph(quote[0], "bop");
        }

        /// <summary>
        /// Tests the table is broken at the beginning of another block-level structure.
        /// </summary>
        [Test]
        public void TestTableT()
        {
            MarkdownDocument doc = Open("TestTableT.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            TableBlock table = (TableBlock)doc[0];

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|abc|def|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|foo|bar|"));

            CheckParagraph(doc[1], "baz");
            CheckParagraph(doc[2], "bop");
        }

        /// <summary>
        /// Tests that data rows may vary in the number of cells.
        /// </summary>
        [Test]
        public void TestTableU()
        {
            MarkdownDocument doc = Open("TestTableU.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|abc|def|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|foo|"));
            Assert.That(((RowBlock)table[2]).Text, Is.EqualTo("|bar|baz|boo|"));

            Assert.That(table.DelimiterRow.Count, Is.EqualTo(2));
        }


        /// <summary>
        /// Tests table with inline html. This allows to construct a multi-paragraph and multiline cells.
        /// </summary>
        /// <remarks>
        /// Until inline html is implemented, this will be just single-line cells.
        /// </remarks>
        [Test]
        public void TestTableW()
        {
            MarkdownDocument doc = Open("TestTableW.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            TableBlock table = (TableBlock)doc[0];

            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|Multiline<br>cell|"));
            Assert.That(((RowBlock)table[1]).Text, Is.EqualTo("|<p>Multiparagraph</p><p>cell</p>|"));
            Assert.That(((RowBlock)table[2]).Text, Is.EqualTo("|<p>Multi<br>paragraph<br>and<br>line</p><p>cell</p>|"));
        }

        /// <summary>
        /// Tests table combined with Quotes.
        /// </summary>
        [Test]
        public void TestTableX()
        {
            MarkdownDocument doc = Open("TestTableX.md");

            QuoteBlock quote = (QuoteBlock)doc[0];
            CheckParagraph(quote[0], "Foo"+SoftBreak+"|bar|"+SoftBreak+"|---|");

            quote = (QuoteBlock)doc[1];
            CheckParagraph(quote[0], "a");

            CheckParagraph(doc[2], "|b|");

            quote = (QuoteBlock)doc[3];
            CheckParagraph(quote[0], "|-|");

            quote = (QuoteBlock)doc[4];
            TableBlock table = (TableBlock)quote[0];
            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|baz|"));

            CheckParagraph(doc[5], "bop");

            quote = (QuoteBlock)doc[6];
            table = (TableBlock)quote[0];
            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|bem|"));

            Assert.That(doc.Count, Is.EqualTo(7));
        }

        /// <summary>
        /// Tests table surrounded with List.
        /// </summary>
        [Test]
        public void TestTableY()
        {
            MarkdownDocument doc = Open("TestTableY.md");

            OrderedListItemBlock orderedList = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedList[0], "Top level");
            CheckOrderedListItem(orderedList[1], 1, ListMarker.Dot, "Sub level");

            TableBlock table = (TableBlock)doc[1];
            Assert.That(((RowBlock)table[0]).Text, Is.EqualTo("|Cell A|Cell B|"));

            CheckOrderedListItem(doc[2], 1, ListMarker.Dot, "ItemA");
            CheckOrderedListItem(doc[3], 1, ListMarker.Dot, "ItemB");

            Assert.That(doc.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests table and FencedCode.
        /// </summary>
        [Test]
        public void TestTableZ()
        {
            MarkdownDocument doc = Open("TestTableZ.md");

            CheckFencedCode(doc[0], "|---|\vbaz", "|");
            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests table and IndentedCode.
        /// </summary>
        [Test]
        public void TestTableZ1()
        {
            MarkdownDocument doc = Open("TestTableZ1.md");

            CheckIndentedCode(doc[0], "|Foo|");
            CheckParagraph(doc[1], "|---|");
            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests table and SetextHeading.
        /// </summary>
        [Test]
        public void TestTableZ2()
        {
            MarkdownDocument doc = Open("TestTableZ2.md");

            CheckSetextHeading(doc[0], 2, "|Foo|");
            CheckParagraph(doc[1], "|---|");
            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests table and AtxHeading.
        /// </summary>
        [Test]
        public void TestTableZ3()
        {
            MarkdownDocument doc = Open("TestTableZ3.md");

            CheckAtxHeading(doc[0], 1, "|Foo|");
            CheckParagraph(doc[1], "|---|");
            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Tables\{0}", fileName));
        }
    }
}
