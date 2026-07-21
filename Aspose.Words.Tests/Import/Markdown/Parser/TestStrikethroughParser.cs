// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Strikethrough feature.
    /// </summary>
    public class TestStrikethroughParser : TestMarkdownParserBase
    {

        /// <summary>
        /// Tests simple strikethrough.
        /// </summary>
        [Test]
        public void TestStrikethroughA()
        {
            MarkdownDocument doc = Open("TestStrikethroughA.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Strikethrough, "~~Hi~~"), new ExpectedInline(BlockType.Inline, " Hello, world!")});
        }

        /// <summary>
        /// Tests simple strikethrough.
        /// </summary>
        [Test]
        public void TestStrikethroughB()
        {
            MarkdownDocument doc = Open("TestStrikethroughB.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckParagraph(doc[0], "This ~~has a");
            CheckParagraph(doc[1], "new paragraph~~.");

            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "This ~~has a")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.Inline, "new paragraph~~.")});
        }

        /// <summary>
        /// Tests complex strikethrough with nested emphases.
        /// </summary>
        [Test]
        public void TestStrikethroughC()
        {
            MarkdownDocument doc = Open("TestStrikethroughC.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "~~Not "), new ExpectedInline(BlockType.Strikethrough, "~~*~~a~~ **code** block*~~")});

            StrikethroughInlineBlock strikethroughBlock = (StrikethroughInlineBlock)((ParagraphBlock)(doc[0]))[1];
            CheckInlines(strikethroughBlock, new [] {new ExpectedInline(BlockType.ItalicInline, "*~~a~~ **code** block*")});
        }

        /// <summary>
        /// Tests strikethrough inside Quote.
        /// </summary>
        [Test]
        public void TestStrikethroughD()
        {
            MarkdownDocument doc = Open("TestStrikethroughD.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            ParagraphBlock paragraphBlock = (ParagraphBlock)quoteBlock[0];

            CheckInlines(paragraphBlock, new [] {new ExpectedInline(BlockType.Strikethrough, "~~Foo"+SoftBreak+"bar~~")});
        }

        /// <summary>
        /// Tests strikethrough inside a Quote separated with blank line.
        /// </summary>
        [Test]
        public void TestStrikethroughE()
        {
            MarkdownDocument doc = Open("TestStrikethroughE.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "~~Foo");

            quoteBlock = (QuoteBlock)doc[1];
            CheckParagraph(quoteBlock[0], "bar~~");
        }

        /// <summary>
        /// Tests strikethrough inside ATX Heading.
        /// </summary>
        [Test]
        public void TestStrikethroughF()
        {
            MarkdownDocument doc = Open("TestStrikethroughF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckAtxHeading(doc[0], 1, "~~Foo~~");

            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Strikethrough, "~~Foo~~")});
        }

        /// <summary>
        /// Tests strikethrough inside Setext Heading.
        /// </summary>
        [Test]
        public void TestStrikethroughG()
        {
            MarkdownDocument doc = Open("TestStrikethroughG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckSetextHeading(doc[0], 1, "~~Foo~~");

            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Strikethrough, "~~Foo~~")});
        }

        /// <summary>
        /// Tests strikethrough inside InlineCode.
        /// </summary>
        [Test]
        public void TestStrikethroughH()
        {
            MarkdownDocument doc = Open("TestStrikethroughH.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`~~Foo~~`")});

            InlineCodeBlock inlineCodeBlock = (InlineCodeBlock)((ParagraphBlock)doc[0])[0];
            CheckInlines(inlineCodeBlock, new [] {new ExpectedInline(BlockType.Inline, "~~Foo~~")});
        }

        /// <summary>
        /// Tests strikethrough with InlineCode inside.
        /// </summary>
        [Test]
        public void TestStrikethroughI()
        {
            MarkdownDocument doc = Open("TestStrikethroughI.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Strikethrough, "~~`Foo`~~")});

            StrikethroughInlineBlock strikethroughBlock = (StrikethroughInlineBlock)((ParagraphBlock)doc[0])[0];
            CheckInlines(strikethroughBlock, new [] {new ExpectedInline(BlockType.InlineCode, "`Foo`")});
        }

        /// <summary>
        /// Tests that one tilde is not enough to be a strikethrough.
        /// </summary>
        [Test]
        public void TestStrikethroughJ()
        {
            MarkdownDocument doc = Open("TestStrikethroughJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "~Foo~")});
        }

        /// <summary>
        /// Tests that closing tildes can be length greater than two.
        /// </summary>
        [Test]
        public void TestStrikethroughK()
        {
            MarkdownDocument doc = Open("TestStrikethroughK.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Strikethrough, "~~Foo~~"), new ExpectedInline(BlockType.Inline, "~")});
        }

        /// <summary>
        /// Tests that there should be at least two not broken closing tildes.
        /// </summary>
        [Test]
        public void TestStrikethroughL()
        {
            MarkdownDocument doc = Open("TestStrikethroughL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "~~Foo~ ~~")});
        }

        /// <summary>
        /// Tests that new line acts as whitespace so that tildes at the beginning of the line can not be a closing sequence.
        /// </summary>
        [Test]
        public void TestStrikethroughM()
        {
            MarkdownDocument doc = Open("TestStrikethroughM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "~~Foo"+SoftBreak+"~~")});
        }

        /// <summary>
        /// Tests that strikethrough has less precedence than inline code.
        /// </summary>
        [Test]
        public void TestStrikethroughN()
        {
            MarkdownDocument doc = Open("TestStrikethroughN.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "~~"), new ExpectedInline(BlockType.InlineCode, "`Foo~~`")});
        }

        /// <summary>
        /// Tests that tildes are allowed intraword.
        /// </summary>
        [Test]
        public void TestStrikethroughO()
        {
            MarkdownDocument doc = Open("TestStrikethroughO.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "F"), new ExpectedInline(BlockType.Strikethrough, "~~o~~"), new ExpectedInline(BlockType.Inline, "o")});
        }

        /// <summary>
        /// Tests strikethrough with different length of opening and closing sequence of tildes.
        /// </summary>
        [Test]
        public void TestStrikethroughP()
        {
            MarkdownDocument doc = Open("TestStrikethroughP.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new [] {new ExpectedInline(BlockType.Inline, "F~~"), new ExpectedInline(BlockType.Strikethrough, "~~o~~"), new ExpectedInline(BlockType.Inline, "~o")});
        }

        /// <summary>
        /// Tests that when tildes are of different lengths, they are cut and remainder can be merged with neighbour tildes.
        /// </summary>
        [Test]
        public void TestStrikethroughQ()
        {
            MarkdownDocument doc = Open("TestStrikethroughQ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new [] {new ExpectedInline(BlockType.Inline, "F"), new ExpectedInline(BlockType.Strikethrough, "~~~~o~~~o~~")});
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\Strikethrough\{0}", fileName));
        }
    }
}
