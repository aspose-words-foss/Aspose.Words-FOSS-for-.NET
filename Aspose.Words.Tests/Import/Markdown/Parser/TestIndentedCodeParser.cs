// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown IndentedCode feature.
    /// </summary>
    public class TestIndentedCodeParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple indented code block.
        /// </summary>
        [Test]
        public void TestIndentedCodeA()
        {
            MarkdownDocument doc = Open("TestIndentedCodeA.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "a simple\v  indented code block");
        }

        /// <summary>
        /// Tests the list item interpretation takes precedence.
        /// </summary>
        [Test]
        public void TestIndentedCodeB()
        {
            MarkdownDocument doc = Open("TestIndentedCodeB.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckParagraph(bulletListItemBlock[0], "foo");
            CheckParagraph(bulletListItemBlock[1], "bar");
        }

        /// <summary>
        /// Tests the list item interpretation takes precedence.
        /// </summary>
        [Test]
        public void TestIndentedCodeC()
        {
            MarkdownDocument doc = Open("TestIndentedCodeC.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedListItemBlock[0], "foo");
            CheckBulletListItem(orderedListItemBlock[1], ListMarker.Minus, "bar");
        }

        /// <summary>
        /// Tests that the contents of a code block are literal text, and do not get parsed as Markdown.
        /// </summary>
        [Test]
        public void TestIndentedCodeD()
        {
            MarkdownDocument doc = Open("TestIndentedCodeD.md");

            CheckIndentedCode(doc[0], "<a/>\v*hi*\v\v- one");
        }


        /// <summary>
        /// Tests that any initial spaces beyond four will be included in the content, even in interior blank lines.
        /// </summary>
        [Test]
        public void TestIndentedCodeF()
        {
            MarkdownDocument doc = Open("TestIndentedCodeF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "chunk1\v  \v  chunk2");
        }

        /// <summary>
        /// Tests that an indented code block cannot interrupt a paragraph.
        /// </summary>
        [Test]
        public void TestIndentedCodeG()
        {
            MarkdownDocument doc = Open("TestIndentedCodeG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "Foo"+SoftBreak+"bar");
        }

        /// <summary>
        /// Tests that any non-blank line with fewer than four leading spaces ends the code block immediately.
        /// So a paragraph may occur immediately after indented code.
        /// </summary>
        [Test]
        public void TestIndentedCodeH()
        {
            MarkdownDocument doc = Open("TestIndentedCodeH.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckIndentedCode(doc[0], "foo");
            CheckParagraph(doc[1], "bar");
        }

        /// <summary>
        /// Tests that indented code can occur immediately before and after other kinds of blocks.
        /// </summary>
        [Test]
        public void TestIndentedCodeI()
        {
            MarkdownDocument doc = Open("TestIndentedCodeI.md");

            Assert.That(doc.Count, Is.EqualTo(5));
            CheckAtxHeading(doc[0], 1, "Heading");
            CheckIndentedCode(doc[1], "foo");
            CheckSetextHeading(doc[2], 2, "Heading");
            CheckIndentedCode(doc[3], "foo");
            CheckHorizontalRule(doc[4]);
        }

        /// <summary>
        /// Tests that the first line can be indented more than four spaces.
        /// </summary>
        [Test]
        public void TestIndentedCodeJ()
        {
            MarkdownDocument doc = Open("TestIndentedCodeJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "    foo\vbar");
        }

        /// <summary>
        /// Tests that blank lines preceding or following an indented code block are not included in it.
        /// </summary>
        [Test]
        public void TestIndentedCodeK()
        {
            MarkdownDocument doc = Open("TestIndentedCodeK.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "foo");
        }

        /// <summary>
        /// Tests that trailing spaces are included in the code block’s content.
        /// </summary>
        [Test]
        public void TestIndentedCodeL()
        {
            MarkdownDocument doc = Open("TestIndentedCodeL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "foo  ");
        }

        /// <summary>
        /// Tests paragraph with only whitespace characters inside indented code block.
        /// </summary>
        [Test]
        public void TestIndentedCodeM()
        {
            MarkdownDocument doc = Open("TestIndentedCodeM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "foo\v\vbar");
        }

        /// <summary>
        /// Tests indented code block inside block quote.
        /// </summary>
        [Test]
        public void TestIndentedCodeN()
        {
            MarkdownDocument doc = Open("TestIndentedCodeN.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckIndentedCode(quoteBlock[0], "foo");
            CheckParagraph(doc[1], "bar");
        }

        /// <summary>
        /// Tests indented code block inside block quote with multiple lines.
        /// </summary>
        [Test]
        public void TestIndentedCodeO()
        {
            MarkdownDocument doc = Open("TestIndentedCodeO.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckIndentedCode(quoteBlock[0], "foo");
            CheckParagraph(quoteBlock[1], "bar");
        }

        /// <summary>
        /// Tests indented code block inside block quote with multiple lines.
        /// </summary>
        [Test]
        public void TestIndentedCodeP()
        {
            MarkdownDocument doc = Open("TestIndentedCodeP.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckIndentedCode(quoteBlock[0], "foo");

            quoteBlock = (QuoteBlock)doc[1];
            CheckParagraph(quoteBlock[0], "bar");
        }

        /// <summary>
        /// Tests indented code block inside block quote with multiple lines.
        /// </summary>
        [Test]
        public void TestIndentedCodeQ()
        {
            MarkdownDocument doc = Open("TestIndentedCodeQ.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckIndentedCode(quoteBlock[0], "foo");

            quoteBlock = (QuoteBlock)doc[1];
            CheckIndentedCode(quoteBlock[0], "bar");
        }

        /// <summary>
        /// Tests indented code block inside block quote with multiple lines.
        /// </summary>
        [Test]
        public void TestIndentedCodeR()
        {
            MarkdownDocument doc = Open("TestIndentedCodeR.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckIndentedCode(quoteBlock[0], "foo\v\vbar");
        }

        /// <summary>
        /// Tests indented code block inside block quote with multiple lines.
        /// </summary>
        [Test]
        public void TestIndentedCodeS()
        {
            MarkdownDocument doc = Open("TestIndentedCodeS.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckIndentedCode(quoteBlock[0], "foo");

            CheckAtxHeading(doc[1], 1, "Heading");

            quoteBlock = (QuoteBlock)doc[2];
            CheckIndentedCode(quoteBlock[0], "bar");
        }

        /// <summary>
        /// Tests indented code block inside various block quotes.
        /// </summary>
        [Test]
        public void TestIndentedCodeT()
        {
            MarkdownDocument doc = Open("TestIndentedCodeT.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "magna"+SoftBreak+"aliqua.");
            QuoteBlock nestedQuoteBlock = (QuoteBlock)quoteBlock[1];
            Assert.That(nestedQuoteBlock.Count, Is.EqualTo(0));
            CheckIndentedCode(quoteBlock[2], "Ut enim");
        }

        /// <summary>
        /// Tests indented code block inside block quote with line continuation.
        /// </summary>
        [Test]
        public void TestIndentedCodeU()
        {
            MarkdownDocument doc = Open("TestIndentedCodeU.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "magna"+SoftBreak+"aliqua.");
            CheckIndentedCode(quoteBlock[1], "Ut enim");

            CheckIndentedCode(doc[1], "ad minim");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Code\IndentedCode\{0}", fileName));
        }
    }
}
