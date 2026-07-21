// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2020 by Mikhail Nepreteamov

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Autolink feature.
    /// </summary>
    public class TestAutolinkParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple Autolink.
        /// </summary>
        [Test]
        public void TestAutolinkA()
        {
            MarkdownDocument doc = Open("TestAutolinkA.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Autolink, "<http://example.com>") });
            AutolinkInlineBlock autolinkBlock = (AutolinkInlineBlock)((ParagraphBlock)doc[0])[0];
            CheckInlines(autolinkBlock, new[] { new ExpectedInline(BlockType.Inline, "http://example.com") });
        }

        /// <summary>
        /// Tests simple Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkB()
        {
            MarkdownDocument doc = Open("TestAutolinkB.md");
            Assert.That(doc.Count, Is.EqualTo(6));

            CheckParagraph(doc[0], "text <http://example.com> text");
            CheckParagraph(doc[1], "text <http://example.com>");
            CheckParagraph(doc[2], "<http://example.com> text");
            CheckParagraph(doc[3], "text<http://example.com>text");
            CheckParagraph(doc[4], "text<http://example.com>");
            CheckParagraph(doc[5], "<http://example.com>text");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "text "),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, " text") });
            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "text "),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>") });
            CheckInlines(doc[2], new[] {
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, " text") });
            CheckInlines(doc[3], new[] {
                new ExpectedInline(BlockType.Inline, "text"),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, "text") });
            CheckInlines(doc[4], new[] {
                new ExpectedInline(BlockType.Inline, "text"),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>") });
            CheckInlines(doc[5], new[] {
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, "text") });
        }

        /// <summary>
        /// Tests simple Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkC()
        {
            MarkdownDocument doc = Open("TestAutolinkC.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "text <http://example.com> text <http://example.com> text");
            CheckParagraph(doc[1], "text <http://example.com><http://example.com> text");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "text "),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, " text "),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, " text") });
            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "text "),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Autolink, "<http://example.com>"),
                new ExpectedInline(BlockType.Inline, " text") });
        }

        /// <summary>
        /// Tests Autolinks with Whitespace characters.
        /// </summary>
        [Test]
        public void TestAutolinkD()
        {
            MarkdownDocument doc = Open("TestAutolinkD.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "<http://example"+SoftBreak+".com>");
            CheckParagraph(doc[1], "<http://example.com>");
            CheckParagraph(doc[2], "<http://example\t\t.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.HtmlTag, "<http://example"+SoftBreak+".com>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.HtmlTag, "<http://example.com>") });
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.Inline, "<http://example\t\t.com>") });
        }


        /// <summary>
        /// Tests Autolink without colon and commercial "At".
        /// </summary>
        [Test]
        public void TestAutolinkF()
        {
            MarkdownDocument doc = Open("TestAutolinkF.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "<http//example.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.HtmlTag, "<http//example.com>") });
        }

        /// <summary>
        /// Tests Autolink with colon and backslash not in the schema name.
        /// </summary>
        [Test]
        public void TestAutolinkG()
        {
            MarkdownDocument doc = Open("TestAutolinkG.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "<http://examp\\le.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Autolink, "<http://examp\\le.com>") });
        }

        /// <summary>
        /// Tests Autolink with commercial "At" and backslash.
        /// </summary>
        [Test]
        public void TestAutolinkH()
        {
            MarkdownDocument doc = Open("TestAutolinkH.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "<email@examp\\le.com>");
            CheckParagraph(doc[1], "<ema\\il@example.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.HtmlTag, "<email@examp\\le.com>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.HtmlTag, "<ema\\il@example.com>") });
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with incorrect length.
        /// </summary>
        [Test]
        public void TestAutolinkI()
        {
            MarkdownDocument doc = Open("TestAutolinkI.md");
            Assert.That(doc.Count, Is.EqualTo(5));

            CheckParagraph(doc[0], "<://example.com>");
            CheckParagraph(doc[1], "<h:example.com>");
            CheckParagraph(doc[2], "<ht://example.com>");
            CheckParagraph(doc[3], "<http5678901234567890123456789012://example.com>");
            CheckParagraph(doc[4], "<http56789012345678901234567890123:example.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Inline, "<://example.com>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.HtmlTag, "<h:example.com>") });
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.Autolink, "<ht://example.com>") });
            CheckInlines(doc[3], new[] {
                new ExpectedInline(BlockType.Autolink, "<http5678901234567890123456789012://example.com>") });
            CheckInlines(doc[4], new[] {
                new ExpectedInline(BlockType.HtmlTag, "<http56789012345678901234567890123:example.com>") });
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with wrong first symbol (must be [a-zA-Z]).
        /// </summary>
        [Test]
        public void TestAutolinkJ()
        {
            MarkdownDocument doc = Open("TestAutolinkJ.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "<1tr://example.com>");
            CheckParagraph(doc[1], "<ammdftfsg://example.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Inline, "<1tr://example.com>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.Autolink, "<ammdftfsg://example.com>") });
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with wrong not-first symbol (must be [a-zA-Z0-9.+-]).
        /// </summary>
        [Test]
        public void TestAutolinkK()
        {
            MarkdownDocument doc = Open("TestAutolinkK.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "<h.t0.+-89-r://example.com>");
            CheckParagraph(doc[1], "<a*mfsg:example.com>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Autolink, "<h.t0.+-89-r://example.com>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.HtmlTag, "<a*mfsg:example.com>") });
        }

        /// <summary>
        /// Tests Autolink with commercial "At" and incorrect symbols.
        /// </summary>
        /// <remarks>
        /// ^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@
        /// [a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$
        /// </remarks>
        [Test]
        public void TestAutolinkL()
        {
            MarkdownDocument doc = Open("TestAutolinkL.md");
            Assert.That(doc.Count, Is.EqualTo(10));

            CheckParagraph(doc[0], "<email@example.com>");
            CheckParagraph(doc[1], "<e@mail@example.com>");
            CheckParagraph(doc[2], "<email@e@xample.com>");
            CheckParagraph(doc[3], "<eM12.!#$%&'*+/=?^_`{|}~-@example.com>");
            CheckParagraph(doc[4], "<email@e-xample.com>");
            CheckParagraph(doc[5], "<email@-example.com>");
            CheckParagraph(doc[6], "<email@example-.com>");
            CheckParagraph(doc[7], "<email@example.c-om>");
            CheckParagraph(doc[8], "<email@example.-com>");
            CheckParagraph(doc[9], "<email@example.com->");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Autolink, "<email@example.com>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.HtmlTag, "<e@mail@example.com>") });
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.HtmlTag, "<email@e@xample.com>") });
            CheckInlines(doc[3], new[] { new ExpectedInline(BlockType.Autolink,
                "<eM12.!#$%&'*+/=?^_`{|}~-@example.com>") });
            CheckInlines(doc[4], new[] { new ExpectedInline(BlockType.Autolink, "<email@e-xample.com>") });
            CheckInlines(doc[5], new[] { new ExpectedInline(BlockType.HtmlTag, "<email@-example.com>") });
            CheckInlines(doc[6], new[] { new ExpectedInline(BlockType.HtmlTag, "<email@example-.com>") });
            CheckInlines(doc[7], new[] { new ExpectedInline(BlockType.Autolink, "<email@example.c-om>") });
            CheckInlines(doc[8], new[] { new ExpectedInline(BlockType.HtmlTag, "<email@example.-com>") });
            CheckInlines(doc[9], new[] { new ExpectedInline(BlockType.HtmlTag, "<email@example.com->") });
        }

        /// <summary>
        /// Tests Autolink with colon and any unexpected symbol after the schema name.
        /// </summary>
        [Test]
        public void TestAutolinkM()
        {
            MarkdownDocument doc = Open("TestAutolinkM.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "<http://examp@$%5^&#@$56&^%1!>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Autolink, "<http://examp@$%5^&#@$56&^%1!>") });
        }




        /// <summary>
        /// Tests non-Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkR()
        {
            MarkdownDocument doc = Open("TestAutolinkR.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "<>");
            CheckParagraph(doc[1], "e-mail@gmail.com");
            CheckParagraph(doc[2], "http://example.com");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Inline, "<>") });
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.Inline, "e-mail@gmail.com") });
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.Inline, "http://example.com") });
        }

        /// <summary>
        /// Tests Autolink with bookmark.
        /// </summary>
        [Test]
        public void TestAutolinkS()
        {
            MarkdownDocument doc = Open("TestAutolinkS.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "<https://spec.commonmark.org/0.28/#autolinks>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Autolink,
                "<https://spec.commonmark.org/0.28/#autolinks>") });
        }

        /// <summary>
        /// Tests that code spans and Autolinks have the same precedence.
        /// </summary>
        [Test]
        public void TestAutolinkU()
        {
            MarkdownDocument doc = Open("TestAutolinkU.md");
            Assert.That(doc.Count, Is.EqualTo(4));

            CheckParagraph(doc[0], "<http://foo.bar.`baz>`");
            CheckParagraph(doc[1], "`<http://foo.bar.`baz>");
            CheckParagraph(doc[2], "<http://foo.`bar.`baz>");
            CheckParagraph(doc[3], "`<http://foo.bar.baz>`");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar.`baz>"),
                new ExpectedInline(BlockType.Inline, "`") });
            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.InlineCode, "`<http://foo.bar.`"),
                new ExpectedInline(BlockType.Inline, "baz>") });
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.Autolink, "<http://foo.`bar.`baz>") });
            CheckInlines(doc[3], new[] { new ExpectedInline(BlockType.InlineCode, "`<http://foo.bar.baz>`") });
        }

        /// <summary>
        /// Tests Autolinks with closer angle brackets.
        /// </summary>
        [Test]
        public void TestAutolinkV()
        {
            MarkdownDocument doc = Open("TestAutolinkV.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "<http://foo.bar.baz>>");
            CheckParagraph(doc[1], "<<http://foo.bar.baz>");
            CheckParagraph(doc[2], "<<http://foo.bar.baz>>");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar.baz>"),
                new ExpectedInline(BlockType.Inline, ">") });
            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "<"),
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar.baz>") });
            CheckInlines(doc[2], new[] {
                new ExpectedInline(BlockType.Inline, "<"),
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar.baz>"),
                new ExpectedInline(BlockType.Inline, ">") });
        }

        /// <summary>
        /// Tests that backslash-escaped left angle bracket is not valid for Autolink.
        /// </summary>
        [Test]
        public void TestAutolinkW()
        {
            MarkdownDocument doc = Open("TestAutolinkW.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "<http://uri>");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.HtmlTag, "<http://uri>") });
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\Autolink\{0}", fileName));
        }
    }
}
