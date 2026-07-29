// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown InlineCode feature (code spans).
    /// </summary>
    public class TestInlineCodeParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple code span.
        /// </summary>
        [Test]
        public void TestInlineCodeA()
        {
            MarkdownDocument doc = Open("TestInlineCodeA.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`foo`")});
        }

        /// <summary>
        /// Tests that there should be an equal number of opening and closing backticks and
        /// single leading and trailing space are stripped.
        /// </summary>
        [Test]
        public void TestInlineCodeB()
        {
            MarkdownDocument doc = Open("TestInlineCodeB.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "``foo ` bar``")});
        }

        /// <summary>
        /// Tests that there should be an equal number of opening and closing backticks and
        /// single leading and trailing space are stripped.
        /// </summary>
        [Test]
        public void TestInlineCodeC()
        {
            MarkdownDocument doc = Open("TestInlineCodeC.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "````")});
        }

        /// <summary>
        /// Tests that only one space is stripped.
        /// </summary>
        [Test]
        public void TestInlineCodeD()
        {
            MarkdownDocument doc = Open("TestInlineCodeD.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "` `` `")});
        }

        /// <summary>
        /// Tests that stripping only happens if the space is on both sides of the string.
        /// </summary>
        [Test]
        public void TestInlineCodeE()
        {
            MarkdownDocument doc = Open("TestInlineCodeE.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "` a`")});
        }

        /// <summary>
        /// Tests that only spaces, and not Unicode whitespace in general, are stripped.
        /// </summary>
        [Test]
        public void TestInlineCodeF()
        {
            MarkdownDocument doc = Open("TestInlineCodeF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`\u00a0b\u00a0`")});
        }

        /// <summary>
        /// Tests that no stripping occurs if the code span contains only spaces.
        /// </summary>
        [Test]
        public void TestInlineCodeG()
        {
            MarkdownDocument doc = Open("TestInlineCodeG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {
                new ExpectedInline(BlockType.InlineCode, "` `"),
                new ExpectedInline(BlockType.Inline, SoftBreak),
                new ExpectedInline(BlockType.InlineCode, "`  `")});
        }

        /// <summary>
        /// Tests that line endings are treated like spaces.
        /// </summary>
        [Test]
        public void TestInlineCodeH()
        {
            MarkdownDocument doc = Open("TestInlineCodeH.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "``foo bar   baz``")});
        }

        /// <summary>
        /// Tests that line endings are treated like spaces.
        /// </summary>
        [Test]
        public void TestInlineCodeI()
        {
            MarkdownDocument doc = Open("TestInlineCodeI.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "``foo ``")});
        }

        /// <summary>
        /// Tests that interior spaces are not collapsed.
        /// </summary>
        [Test]
        public void TestInlineCodeJ()
        {
            MarkdownDocument doc = Open("TestInlineCodeJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`foo   bar  baz`")});
        }

        /// <summary>
        /// Tests that backslash escapes do not work in code spans. All backslashes are treated literally.
        /// </summary>
        [Test]
        public void TestInlineCodeK()
        {
            MarkdownDocument doc = Open("TestInlineCodeK.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`foo\\`"), new ExpectedInline(BlockType.Inline, "bar`")});
        }

        /// <summary>
        /// Tests that backslash escapes are never needed, because one can always
        /// choose a string of n backtick characters as delimiters.
        /// </summary>
        [Test]
        public void TestInlineCodeL()
        {
            MarkdownDocument doc = Open("TestInlineCodeL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "``foo`bar``")});
        }

        /// <summary>
        /// Tests that backslash escapes are never needed, because one can always
        /// choose a string of n backtick characters as delimiters.
        /// </summary>
        [Test]
        public void TestInlineCodeM()
        {
            MarkdownDocument doc = Open("TestInlineCodeM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`foo `` bar`")});
        }

        /// <summary>
        /// Tests that code span backticks have higher precedence than any other inline constructs
        /// except HTML tags and autolinks. Thus, for example, this is not parsed as emphasized text,
        /// since the second * is part of a code span.
        /// </summary>
        [Test]
        public void TestInlineCodeN()
        {
            MarkdownDocument doc = Open("TestInlineCodeN.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "*foo"), new ExpectedInline(BlockType.InlineCode, "`*`")});
        }

        /// <summary>
        /// Tests that code span backticks have higher precedence than any other inline constructs
        /// except HTML tags and autolinks. Thus, for example, this is not parsed as emphasized text,
        /// since the second * is part of a code span.
        /// </summary>
        [Test]
        public void TestInlineCodeO()
        {
            MarkdownDocument doc = Open("TestInlineCodeO.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new []
                {
                    new ExpectedInline(BlockType.Inline, "[not a "), new ExpectedInline(BlockType.InlineCode, "`link](/foo`"), new ExpectedInline(BlockType.Inline, ")")
                });
        }

        /// <summary>
        /// Tests that code spans, HTML tags, and autolinks have the same precedence. Thus, this is code.
        /// </summary>
        [Test]
        public void TestInlineCodeP()
        {
            MarkdownDocument doc = Open("TestInlineCodeP.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`<a href=\"`"), new ExpectedInline(BlockType.Inline, "\">`")});
        }


        /// <summary>
        /// Tests that code spans, HTML tags, and autolinks have the same precedence. Thus, this is code.
        /// </summary>
        [Test]
        public void TestInlineCodeR()
        {
            MarkdownDocument doc = Open("TestInlineCodeR.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "`<http://foo.bar.`"), new ExpectedInline(BlockType.Inline, "baz>`")});
        }

        /// <summary>
        /// Tests that code spans, HTML tags, and autolinks have the same precedence. Thus, this is an autolink.
        /// </summary>
        [Test]
        public void TestInlineCodeS()
        {
            MarkdownDocument doc = Open("TestInlineCodeS.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar.`baz>"),
                new ExpectedInline(BlockType.Inline, "`") });
        }

        /// <summary>
        /// Tests that when a backtick string is not closed by a matching backtick string, we just have literal backticks.
        /// </summary>
        [Test]
        public void TestInlineCodeT()
        {
            MarkdownDocument doc = Open("TestInlineCodeT.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "```foo``")});
        }

        /// <summary>
        /// Tests that when a backtick string is not closed by a matching backtick string, we just have literal backticks.
        /// </summary>
        [Test]
        public void TestInlineCodeU()
        {
            MarkdownDocument doc = Open("TestInlineCodeU.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "`foo")});
        }

        /// <summary>
        /// Tests that opening and closing backtick strings have to be equal in length.
        /// </summary>
        [Test]
        public void TestInlineCodeV()
        {
            MarkdownDocument doc = Open("TestInlineCodeV.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "`foo"), new ExpectedInline(BlockType.InlineCode, "``bar``")});
        }

        /// <summary>
        /// Tests a backtick-delimited string in a code span: `` `foo` ``
        /// </summary>
        [Test]
        public void TestInlineCodeW()
        {
            MarkdownDocument doc = Open("TestInlineCodeW.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.InlineCode, "```foo```") });
        }

        /// <summary>
        /// Returns Markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\InlineCode\{0}", fileName));
        }
    }
}
