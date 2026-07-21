// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2023 by Ilya Navrotskiy

using Aspose.Words.Loading;
using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Underline feature.
    /// </summary>
    public class TestUnderlineParser : TestMarkdownParserBase
    {

        /// <summary>
        /// Tests simple underline.
        /// </summary>
        [Test]
        public void TestUnderlineA()
        {
            MarkdownDocument doc = Open("TestUnderlineA.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Underline, "++Hello++"), new ExpectedInline(BlockType.Inline, " world!")});
        }

        /// <summary>
        /// Tests simple underline.
        /// </summary>
        [Test]
        public void TestUnderlineB()
        {
            MarkdownDocument doc = Open("TestUnderlineB.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckParagraph(doc[0], "This ++has a");
            CheckParagraph(doc[1], "new paragraph++.");

            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "This ++has a")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.Inline, "new paragraph++.")});
        }

        /// <summary>
        /// Tests complex underline with nested emphases.
        /// </summary>
        [Test]
        public void TestUnderlineC()
        {
            MarkdownDocument doc = Open("TestUnderlineC.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "++Not "), new ExpectedInline(BlockType.Underline, "++*++a++ **code** block*++")});

            UnderlineInlineBlock underlineBlock = (UnderlineInlineBlock)((ParagraphBlock)(doc[0]))[1];
            CheckInlines(underlineBlock, new [] {new ExpectedInline(BlockType.ItalicInline, "*++a++ **code** block*")});
        }

        /// <summary>
        /// Tests underline inside Quote.
        /// </summary>
        [Test]
        public void TestUnderlineD()
        {
            MarkdownDocument doc = Open("TestUnderlineD.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            ParagraphBlock paragraphBlock = (ParagraphBlock)quoteBlock[0];

            CheckInlines(paragraphBlock, new [] {new ExpectedInline(BlockType.Underline, "++Foo"+SoftBreak+"bar++")});
        }

        /// <summary>
        /// Tests underline inside a Quote separated with blank line.
        /// </summary>
        [Test]
        public void TestUnderlineE()
        {
            MarkdownDocument doc = Open("TestUnderlineE.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "++Foo");

            quoteBlock = (QuoteBlock)doc[1];
            CheckParagraph(quoteBlock[0], "bar++");
        }

        /// <summary>
        /// Tests underline inside ATX Heading.
        /// </summary>
        [Test]
        public void TestUnderlineF()
        {
            MarkdownDocument doc = Open("TestUnderlineF.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckAtxHeading(doc[0], 1, "++Foo++");

            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Underline, "++Foo++")});
        }

        /// <summary>
        /// Tests underline inside Setext Heading.
        /// </summary>
        [Test]
        public void TestUnderlineG()
        {
            MarkdownDocument doc = Open("TestUnderlineG.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckSetextHeading(doc[0], 1, "++Foo++");

            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Underline, "++Foo++")});
        }

        /// <summary>
        /// Tests underline inside InlineCode.
        /// </summary>
        [Test]
        public void TestUnderlineH()
        {
            MarkdownDocument doc = Open("TestUnderlineH.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`++Foo++`")});

            InlineCodeBlock inlineCodeBlock = (InlineCodeBlock)((ParagraphBlock)doc[0])[0];
            CheckInlines(inlineCodeBlock, new [] {new ExpectedInline(BlockType.Inline, "++Foo++")});
        }

        /// <summary>
        /// Tests underline with InlineCode inside.
        /// </summary>
        [Test]
        public void TestUnderlineI()
        {
            MarkdownDocument doc = Open("TestUnderlineI.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Underline, "++`Foo`++")});

            UnderlineInlineBlock underlineBlock = (UnderlineInlineBlock)((ParagraphBlock)doc[0])[0];
            CheckInlines(underlineBlock, new [] {new ExpectedInline(BlockType.InlineCode, "`Foo`")});
        }

        /// <summary>
        /// Tests that one plus is not enough to be an underline.
        /// </summary>
        [Test]
        public void TestUnderlineJ()
        {
            MarkdownDocument doc = Open("TestUnderlineJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "+Foo+")});
        }

        /// <summary>
        /// Tests that closing pluses can be length greater than two.
        /// </summary>
        [Test]
        public void TestUnderlineK()
        {
            MarkdownDocument doc = Open("TestUnderlineK.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Underline, "++Foo++"), new ExpectedInline(BlockType.Inline, "+")});
        }

        /// <summary>
        /// Tests that there should be at least two not broken closing pluses.
        /// </summary>
        [Test]
        public void TestUnderlineL()
        {
            MarkdownDocument doc = Open("TestUnderlineL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "++Foo+ ++")});
        }

        /// <summary>
        /// Tests that new line acts as whitespace so that pluses at the beginning of the line can not be a closing sequence.
        /// </summary>
        [Test]
        public void TestUnderlineM()
        {
            MarkdownDocument doc = Open("TestUnderlineM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "++Foo"+SoftBreak+"++")});
        }

        /// <summary>
        /// Tests that underline has less precedence than inline code.
        /// </summary>
        [Test]
        public void TestUnderlineN()
        {
            MarkdownDocument doc = Open("TestUnderlineN.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "++"), new ExpectedInline(BlockType.InlineCode, "`Foo++`")});
        }

        /// <summary>
        /// Tests that pluses are allowed intraword.
        /// </summary>
        [Test]
        public void TestUnderlineO()
        {
            MarkdownDocument doc = Open("TestUnderlineO.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "F"), new ExpectedInline(BlockType.Underline, "++o++"), new ExpectedInline(BlockType.Inline, "o")});
        }

        /// <summary>
        /// Tests underline with different length of opening and closing sequence of pluses.
        /// </summary>
        [Test]
        public void TestUnderlineP()
        {
            MarkdownDocument doc = Open("TestUnderlineP.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new [] {new ExpectedInline(BlockType.Inline, "F++"), new ExpectedInline(BlockType.Underline, "++o++"), new ExpectedInline(BlockType.Inline, "+o")});
        }

        /// <summary>
        /// Tests that when pluses are of different lengths, they are cut and remainder can be merged with neighbour pluses.
        /// </summary>
        [Test]
        public void TestUnderlineQ()
        {
            MarkdownDocument doc = Open("TestUnderlineQ.md", loadOptions);

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new [] {new ExpectedInline(BlockType.Inline, "F"), new ExpectedInline(BlockType.Underline, "++++o+++o++")});
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return Open(fileName, null);
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName, MarkdownLoadOptions loadOptions)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\Underline\{0}", fileName), loadOptions);
        }

        MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { ImportUnderlineFormatting = true };
    }
}
