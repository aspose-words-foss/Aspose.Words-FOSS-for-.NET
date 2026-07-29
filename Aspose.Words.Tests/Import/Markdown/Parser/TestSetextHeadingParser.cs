// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown SeText Heading feature.
    /// </summary>
    public class TestSetextHeadingParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple setext heading.
        /// </summary>
        [Test]
        public void TestSetextHeadingA()
        {
            MarkdownDocument doc = Open("TestSetextHeadingA.md");
            CheckSetextHeading(doc[0], 1, "Foo *bar*");
            CheckSetextHeading(doc[1], 2, "Foo *baz*");
        }

        /// <summary>
        /// Tests that content of the header may span more than one line.
        /// </summary>
        [Test]
        public void TestSetextHeadingB()
        {
            MarkdownDocument doc = Open("TestSetextHeadingB.md");
            CheckSetextHeading(doc[0], 1, "Foo *bar"+SoftBreak+"baz*");
        }

        /// <summary>
        /// Tests that underlining can be any length.
        /// </summary>
        [Test]
        public void TestSetextHeadingC()
        {
            MarkdownDocument doc = Open("TestSetextHeadingC.md");
            CheckSetextHeading(doc[0], 2, "Foo");
            CheckSetextHeading(doc[1], 1, "Bar");
        }

        /// <summary>
        /// Tests that heading content can be indented up to three spaces, and need not line up with the underlining.
        /// </summary>
        [Test]
        public void TestSetextHeadingD()
        {
            MarkdownDocument doc = Open("TestSetextHeadingD.md");
            CheckSetextHeading(doc[0], 2, "Foo");
            CheckSetextHeading(doc[1], 2, "Bar");
            CheckSetextHeading(doc[2], 1, "baz");
        }

        /// <summary>
        /// Tests that four spaces indent is too much for setext heading.
        /// </summary>
        [Test]
        public void TestSetextHeadingE()
        {
            MarkdownDocument doc = Open("TestSetextHeadingE.md");
            CheckIndentedCode(doc[0], "Foo\v---\v\vbar");
            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that setext heading underline can be indented up to three spaces, and may have trailing spaces.
        /// </summary>
        [Test]
        public void TestSetextHeadingF()
        {
            MarkdownDocument doc = Open("TestSetextHeadingF.md");
            CheckSetextHeading(doc[0], 2, "Foo");
        }

        /// <summary>
        /// Tests that four opening indentation spaces is too much for setext heading.
        /// </summary>
        [Test]
        public void TestSetextHeadingG()
        {
            MarkdownDocument doc = Open("TestSetextHeadingG.md");
            CheckParagraph(doc[0], "Foo"+SoftBreak+"---");
        }

        /// <summary>
        /// Tests that setext heading underline cannot contain internal spaces.
        /// </summary>
        [Test]
        public void TestSetextHeadingH()
        {
            MarkdownDocument doc = Open("TestSetextHeadingH.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "Foo"+SoftBreak+"= =");
            CheckParagraph(doc[1], "Bar");
            CheckHorizontalRule(doc[2]);
        }

        /// <summary>
        /// Tests that trailing spaces in the content line do not cause a line break.
        /// </summary>
        [Test]
        public void TestSetextHeadingI()
        {
            MarkdownDocument doc = Open("TestSetextHeadingI.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckSetextHeading(doc[0], 2, "Foo");
        }

        /// <summary>
        /// Tests that trailing backslash in the content line do not cause a line break.
        /// </summary>
        [Test]
        public void TestSetextHeadingJ()
        {
            MarkdownDocument doc = Open("TestSetextHeadingJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckSetextHeading(doc[0], 2, @"Foo\");
        }

        /// <summary>
        /// Tests that setext heading indicators of block structure take precedence over indicators of inline structures.
        /// </summary>
        [Test]
        public void TestSetextHeadingK()
        {
            MarkdownDocument doc = Open("TestSetextHeadingK.md");

            Assert.That(doc.Count, Is.EqualTo(4));
            CheckSetextHeading(doc[0], 2, "`Foo");
            CheckParagraph(doc[1], "`");
            CheckSetextHeading(doc[2], 2, "<a title=\"a lot");
            CheckParagraph(doc[3], "of dashes\"/>");
        }

        /// <summary>
        /// Tests that setext heading underline cannot be a lazy continuation line in a block quote.
        /// </summary>
        [Test]
        public void TestSetextHeadingL()
        {
            MarkdownDocument doc = Open("TestSetextHeadingL.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckParagraph(quoteBlock[0], "Foo");

            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that setext heading underline cannot be a lazy continuation line in a multiline block quote.
        /// </summary>
        [Test]
        public void TestSetextHeadingM()
        {
            MarkdownDocument doc = Open("TestSetextHeadingM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            Assert.That(quoteBlock.Count, Is.EqualTo(1));
            CheckParagraph(quoteBlock[0], "foo"+SoftBreak+"bar"+SoftBreak+"===");
        }

        /// <summary>
        /// Tests that setext heading underline cannot be a lazy continuation line in a list.
        /// </summary>
        [Test]
        public void TestSetextHeadingN()
        {
            MarkdownDocument doc = Open("TestSetextHeadingN.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckBulletListItem(doc[0], ListMarker.Minus, "Foo");
            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that a blank line is needed between a paragraph and a following setext heading,
        /// since otherwise the paragraph becomes part of the heading’s content.
        /// </summary>
        [Test]
        public void TestSetextHeadingO()
        {
            MarkdownDocument doc = Open("TestSetextHeadingO.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckSetextHeading(doc[0], 2, "Foo"+SoftBreak+"Bar");
        }

        /// <summary>
        /// Tests that in general a blank line is not required before or after setext headings.
        /// A first line in this test is HorizontalRule.
        /// </summary>
        [Test]
        public void TestSetextHeadingP()
        {
            MarkdownDocument doc = Open("TestSetextHeadingP.md");

            Assert.That(doc.Count, Is.EqualTo(4));

            CheckHorizontalRule(doc[0]);
            CheckSetextHeading(doc[1], 2, "Foo");
            CheckSetextHeading(doc[2], 2, "Bar");
            CheckParagraph(doc[3], "Baz");
        }

        /// <summary>
        /// Tests that setext headings cannot be empty.
        /// </summary>
        [Test]
        public void TestSetextHeadingQ()
        {
            MarkdownDocument doc = Open("TestSetextHeadingQ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "====");
        }

        /// <summary>
        /// Tests that setext heading text lines must not be interpretable as block constructs other than paragraphs.
        /// So, the line of dashes gets interpreted as a HorizontalRule.
        /// </summary>
        [Test]
        public void TestSetextHeadingR()
        {
            MarkdownDocument doc = Open("TestSetextHeadingR.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckHorizontalRule(doc[0]);
            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that setext heading text lines must not be interpretable as block constructs other than paragraphs.
        /// So, the line with dash gets interpreted as a List.
        /// </summary>
        [Test]
        public void TestSetextHeadingS()
        {
            MarkdownDocument doc = Open("TestSetextHeadingS.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that setext heading text lines must not be interpretable as block constructs other than paragraphs.
        /// So, the line with dashes gets interpreted as a HorizontalRule.
        /// </summary>
        [Test]
        public void TestSetextHeadingT()
        {
            MarkdownDocument doc = Open("TestSetextHeadingT.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            Assert.That(quoteBlock.Count, Is.EqualTo(1));
            CheckParagraph(quoteBlock[0], "foo");
            CheckHorizontalRule(doc[1]);
        }

        /// <summary>
        /// Tests that blank line not allowed in text of multiline setext headings.
        /// </summary>
        [Test]
        public void TestSetextHeadingU()
        {
            MarkdownDocument doc = Open("TestSetextHeadingU.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "Foo");
            CheckSetextHeading(doc[1], 2, "bar");
            CheckParagraph(doc[2], "baz");
        }

        /// <summary>
        /// Tests that blank lines can be put around dashes to interpret them as a HorizontalRule.
        /// </summary>
        [Test]
        public void TestSetextHeadingV()
        {
            MarkdownDocument doc = Open("TestSetextHeadingV.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "Foo"+SoftBreak+"bar");
            CheckHorizontalRule(doc[1]);
            CheckParagraph(doc[2], "baz");
        }

        /// <summary>
        /// Tests that line "* * *" is not interpreted as a setext heading underline.
        /// </summary>
        [Test]
        public void TestSetextHeadingW()
        {
            MarkdownDocument doc = Open("TestSetextHeadingW.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "Foo"+SoftBreak+"bar");
            CheckHorizontalRule(doc[1]);
            CheckParagraph(doc[2], "baz");
        }

        /// <summary>
        /// Tests that line with backslash before dashes is not interpreted as a setext heading underline.
        /// </summary>
        [Test]
        public void TestSetextHeadingX()
        {
            MarkdownDocument doc = Open("TestSetextHeadingX.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "Foo"+SoftBreak+"bar"+SoftBreak+"---"+SoftBreak+"baz");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Headings\Setext\{0}", fileName));
        }
    }
}
