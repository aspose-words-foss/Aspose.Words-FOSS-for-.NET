// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Quote feature.
    /// </summary>
    public class TestQuoteParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple quote.
        /// </summary>
        [Test]
        public void TestQuoteA()
        {
            MarkdownDocument doc = Open(@"TestQuoteA.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 1, "Foo");
            CheckParagraph(quoteBlock[1], "bar"+SoftBreak+"baz");
        }

        /// <summary>
        /// Tests that spaces after the > characters can be omitted in a quote.
        /// </summary>
        [Test]
        public void TestQuoteB()
        {
            MarkdownDocument doc = Open("TestQuoteB.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 1, "Foo");
            CheckParagraph(quoteBlock[1], "bar"+SoftBreak+"baz");
        }

        /// <summary>
        /// Tests that the > characters can be indented 1-3 spaces in a quote.
        /// </summary>
        [Test]
        public void TestQuoteC()
        {
            MarkdownDocument doc = Open("TestQuoteC.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 1, "Foo");
            CheckParagraph(quoteBlock[1], "bar"+SoftBreak+"baz");
        }

        /// <summary>
        /// Tests that four indentation spaces are too much for a quote.
        /// </summary>
        [Test]
        public void TestQuoteD()
        {
            MarkdownDocument doc = Open("TestQuoteD.md");
            CheckIndentedCode(doc[0], "> # Foo\v> bar\v> baz");
        }

        /// <summary>
        /// Tests that we can omit the > before paragraph continuation text in a quote.
        /// </summary>
        [Test]
        public void TestQuoteE()
        {
            MarkdownDocument doc = Open("TestQuoteE.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 1, "Foo");
            CheckParagraph(quoteBlock[1], "bar"+SoftBreak+"baz");
        }

        /// <summary>
        /// Tests that a quote can contain some lazy and some non-lazy continuation lines.
        /// </summary>
        [Test]
        public void TestQuoteF()
        {
            MarkdownDocument doc = Open("TestQuoteF.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "bar"+SoftBreak+"baz"+SoftBreak+"foo");
        }

        /// <summary>
        /// Tests that sequence of --- characters will be parsed as a horizontal rule outside of quote.
        /// </summary>
        [Test]
        public void TestQuoteG()
        {
            MarkdownDocument doc = Open("TestQuoteG.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "foo");
            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that sequence of >--- characters will be parsed as a setext heading inside a quote.
        /// </summary>
        [Test]
        public void TestQuoteH()
        {
            MarkdownDocument doc = Open("TestQuoteH.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckSetextHeading(quoteBlock[0], 2, "foo");
        }

        /// <summary>
        /// Tests that sequence of '>-' characters will be parsed as a list item inside a quote.
        /// </summary>
        [Test]
        public void TestQuoteI()
        {
            MarkdownDocument doc = Open("TestQuoteI.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckBulletListItem(quoteBlock[0], ListMarker.Minus, "foo");
            CheckBulletListItem(quoteBlock[1], ListMarker.Minus, "bar");
        }

        /// <summary>
        /// Tests that - without preceding > is a list item outside of a quote.
        /// </summary>
        [Test]
        public void TestQuoteJ()
        {
            MarkdownDocument doc = Open("TestQuoteJ.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckBulletListItem(quoteBlock[0], ListMarker.Minus, "foo");

            CheckBulletListItem(doc[1], ListMarker.Minus, "bar");
        }

        /// <summary>
        /// Tests lazy continuation line in a quote.
        /// </summary>
        [Test]
        public void TestQuoteK()
        {
            MarkdownDocument doc = Open("TestQuoteK.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "foo"+SoftBreak+"- bar");
        }

        /// <summary>
        /// Tests a block quote can be empty.
        /// </summary>
        [Test]
        public void TestQuoteL()
        {
            MarkdownDocument doc = Open("TestQuoteL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            Assert.That(quoteBlock.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests empty or whitespace lines can be inside a block quote.
        /// </summary>
        [Test]
        public void TestQuoteM()
        {
            MarkdownDocument doc = Open("TestQuoteM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            Assert.That(quoteBlock.Count, Is.EqualTo(0));
        }


        /// <summary>
        /// Tests that blank line always separates block quotes.
        /// </summary>
        [Test]
        public void TestQuoteO()
        {
            MarkdownDocument doc = Open("TestQuoteO.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "foo");

            quoteBlock = (QuoteBlock)doc[1];
            CheckParagraph(quoteBlock[0], "bar");
        }

        /// <summary>
        /// Tests that if we put block quotes together, we get a single block quote.
        /// </summary>
        [Test]
        public void TestQuoteP()
        {
            MarkdownDocument doc = Open("TestQuoteP.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "foo"+SoftBreak+"bar");
        }

        /// <summary>
        /// Tests that if we put block quotes together, we get a single block quote.
        /// </summary>
        [Test]
        public void TestQuoteQ()
        {
            MarkdownDocument doc = Open("TestQuoteQ.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "foo");
            CheckParagraph(quoteBlock[1], "bar");
        }

        /// <summary>
        /// Tests paragraph is not appended to a quote that is ended with a heading.
        /// </summary>
        [Test]
        public void TestQuoteR()
        {
            MarkdownDocument doc = Open("TestQuoteR.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 1, "foo");
            CheckParagraph(doc[1], "bar");
        }

        /// <summary>
        /// Tests quote level when empty nested quote has equal or less level than parent quote.
        /// </summary>
        [Test]
        public void TestQuoteS()
        {
            MarkdownDocument doc = Open("TestQuoteS.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteLvl1 = (QuoteBlock)doc[0];
            CheckParagraph(quoteLvl1[0], "This is the first level of quoting.");
            QuoteBlock quoteLvl2 = (QuoteBlock)quoteLvl1[1];
            CheckParagraph(quoteLvl2[0], "This is nested blockquote.");
            CheckParagraph(quoteLvl1[2], "Back to the first level.");
        }

        /// <summary>
        /// Tests quote level when empty nested quote has equal or greater level than parent quote.
        /// </summary>
        [Test]
        public void TestQuoteT()
        {
            MarkdownDocument doc = Open("TestQuoteT.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteLvl1 = (QuoteBlock)doc[0];
            CheckParagraph(quoteLvl1[0], "lvl1");

            QuoteBlock quoteLvl2 = (QuoteBlock)quoteLvl1[1];
            QuoteBlock quoteLvl3 = (QuoteBlock)quoteLvl2[0];
            QuoteBlock quoteLvl4 = (QuoteBlock)quoteLvl3[0];
            QuoteBlock quoteLvl5 = (QuoteBlock)quoteLvl4[0];
            QuoteBlock quoteLvl6 = (QuoteBlock)quoteLvl5[0];
            Assert.That(quoteLvl6.Count, Is.EqualTo(0));


            CheckParagraph(quoteLvl4[1], "lvl4_s1"+SoftBreak+"lvl4_s2");

            QuoteBlock quoteLvl45 = (QuoteBlock)quoteLvl4[2];
            QuoteBlock quoteLvl46 = (QuoteBlock)quoteLvl45[0];
            CheckParagraph(quoteLvl46[0], "lvl6");
        }

        /// <summary>
        /// Tests quote separated with blank quotes.
        /// </summary>
        [Test]
        public void TestQuoteU()
        {
            MarkdownDocument doc = Open("TestQuoteU.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 2, "This is a header.");
            CheckParagraph(quoteBlock[1], "q1"+SoftBreak+"q2");

            CheckParagraph(doc[1], "text outside of quote");
        }

        /// <summary>
        ///  Tests quote separated with blank.
        /// </summary>
        [Test]
        public void TestQuoteV()
        {
            MarkdownDocument doc = Open("TestQuoteV.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "baz");

            quoteBlock = (QuoteBlock)((QuoteBlock)doc[1])[0];
            CheckParagraph(quoteBlock[0], "tempor incididunt");
        }

        /// <summary>
        /// Tests quotes with Heading separated with blank lines.
        /// </summary>
        [Test]
        public void TestQuoteW()
        {
            MarkdownDocument doc = Open("TestQuoteW.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckAtxHeading(quoteBlock[0], 1, "foo");

            quoteBlock = (QuoteBlock)doc[1];
            CheckParagraph(quoteBlock[0], "bar");

            quoteBlock = (QuoteBlock)doc[2];
            CheckAtxHeading(quoteBlock[0], 1, "baz");
            CheckParagraph(quoteBlock[1], "bop");
        }

        /// <summary>
        /// Tests AtxHeading is inserted into an appropriate nesting quote level,
        /// when it is in a quote with the level less than a previous paragraph.
        /// </summary>
        [Test]
        public void TestQuoteHeadingLess()
        {
            MarkdownDocument doc = Open(@"TestQuoteHeadingLess.md");

            // There should be one root Quote inside a document.
            Assert.That(doc.Count, Is.EqualTo(1));

            // This is a root Quote. The root Quote has two nested Quotes.
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            // The first nested Quote has child AtxHeading and another one nested Quote.
            quoteBlock = (QuoteBlock)quoteBlock[0];
            CheckAtxHeading(quoteBlock[1], 1, "bar");

            // The second nested quote has child paragraph.
            quoteBlock = (QuoteBlock)quoteBlock[0];
            CheckParagraph(quoteBlock[0], "Foo");
        }

        /// <summary>
        /// Tests AtxHeading is inserted into an appropriate nesting quote level,
        /// when it is in a quote with the level greater than a previous paragraph.
        /// </summary>
        [Test]
        public void TestQuoteHeadingGreater()
        {
            MarkdownDocument doc = Open(@"TestQuoteHeadingGreater.md");

            // There should be one root Quote inside a document.
            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "Foo");

            quoteBlock = (QuoteBlock)quoteBlock[1];
            CheckAtxHeading(quoteBlock[0], 1, "bar");
        }

        /// <summary>
        /// Tests AtxHeading is NOT inserted into a quote when it is just after
        /// a paragraph inside a quote, but not in a quote itself.
        /// </summary>
        [Test]
        public void TestQuoteHeadingOutside()
        {
            MarkdownDocument doc = Open(@"TestQuoteHeadingOutside.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            // The first child of the document is a Quote that has two nested Quotes.
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            quoteBlock = (QuoteBlock)quoteBlock[0];
            quoteBlock = (QuoteBlock)quoteBlock[0];

            // Last quote contains a paragraph.
            Assert.That(quoteBlock.Count, Is.EqualTo(1));
            CheckParagraph(quoteBlock[0], "Foo");

            // AtxHeading is outside of Quotes and it is the second child of the document.
            CheckAtxHeading(doc[1], 1, "bar");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Quotes\{0}", fileName));
        }
    }
}
