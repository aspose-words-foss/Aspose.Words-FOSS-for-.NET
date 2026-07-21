// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/05/2020 by Mikhail Nepreteamov

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Link feature.
    /// </summary>
    public class TestLinkParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple Link.
        /// </summary>
        [Test]
        public void TestLinkA()
        {
            MarkdownDocument doc = Open("TestLinkA.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "/uri");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "text", "http://example.com");
        }

        /// <summary>
        /// Tests simple Link with a title.
        /// </summary>
        [Test]
        public void TestLinkB()
        {
            MarkdownDocument doc = Open("TestLinkB.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "/uri", "title");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "text", "http://example.com", "title");
        }

        /// <summary>
        /// Tests empty link text for Link.
        /// </summary>
        [Test]
        public void TestLinkC()
        {
            MarkdownDocument doc = Open("TestLinkC.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "", "http://example.com");
        }

        /// <summary>
        /// Tests empty link URI for Link.
        /// </summary>
        [Test]
        public void TestLinkD()
        {
            MarkdownDocument doc = Open("TestLinkD.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "text", "");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "link", "");
        }

        /// <summary>
        /// Tests Link titles with different quotes.
        /// </summary>
        [Test]
        public void TestLinkE()
        {
            MarkdownDocument doc = Open("TestLinkE.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "text", "http://example.com", "title");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "text", "http://example.com", "title");
        }

        /// <summary>
        /// Tests Link with angle brackets for link URI.
        /// </summary>
        [Test]
        public void TestLinkF()
        {
            MarkdownDocument doc = Open("TestLinkF.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "text", "http://example.com");
        }

        /// <summary>
        /// Tests Link titles with backslash-escaped quotes.
        /// </summary>
        [Test]
        public void TestLinkG()
        {
            MarkdownDocument doc = Open("TestLinkG.md");
            Assert.That(doc.Count, Is.EqualTo(13));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "text", "http://example.com", "ti\"tle");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "text", "http://example.com", "ti'tle");
            CheckLinkTextBlock(((ParagraphBlock)doc[2])[0], "text", "http://example.com", "ti)tle");
            CheckInlines(doc[3], new [] {new ExpectedInline(BlockType.Inline, "[text](http://example.com \"ti\"tle\")")});
            CheckInlines(doc[4], new [] {new ExpectedInline(BlockType.Inline, "[text](http://example.com 'ti'tle')")});
            CheckInlines(doc[5], new [] {new ExpectedInline(BlockType.Inline, "[text](http://example.com (ti)tle))")});
            CheckLinkTextBlock(((ParagraphBlock)doc[6])[0], "text", "http://example.com", "ti'tle");
            CheckLinkTextBlock(((ParagraphBlock)doc[7])[0], "text", "http://example.com", "ti\"tle");
            CheckLinkTextBlock(((ParagraphBlock)doc[8])[0], "text", "http://example.com", "ti\"tle");
            CheckLinkTextBlock(((ParagraphBlock)doc[9])[0], "text", "http://example.com", "ti)tle");
            CheckInlines(doc[10], new[] {new ExpectedInline(BlockType.Inline, "[link](/url \"title \"and\" title\")")});
            CheckLinkTextBlock(((ParagraphBlock)doc[11])[0], "link", "/url", "title \"and\" title");
            CheckInlines(doc[12], new[] {new ExpectedInline(BlockType.Inline, "[text](http://example.com (ti(tle))")});
        }

        /// <summary>
        /// Tests that the Link destination cannot contain spaces or line breaks, except spaces enclosed in pointy brackets.
        /// </summary>
        [Test]
        public void TestLinkH()
        {
            MarkdownDocument doc = Open("TestLinkH.md");
            Assert.That(doc.Count, Is.EqualTo(4));

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Inline, "[link](/my uri)") });
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "link", "/my%20uri");
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.Inline, "[link](foo"+SoftBreak+"bar)") });
            CheckInlines(doc[3], new[]
            {
                new ExpectedInline(BlockType.Inline, "[link]("),
                new ExpectedInline(BlockType.HtmlTag, "<foo"+SoftBreak+"bar>"),
                new ExpectedInline(BlockType.Inline, ")")
            });
        }

        /// <summary>
        /// Tests that parentheses inside the Link destination may be escaped.
        /// </summary>
        [Test]
        public void TestLinkI()
        {
            MarkdownDocument doc = Open("TestLinkI.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "(foo)");
        }

        /// <summary>
        /// Tests that any number of parentheses in the Link target are allowed without escaping,
        /// as long as they are balanced.
        /// </summary>
        [Test]
        public void TestLinkJ()
        {
            MarkdownDocument doc = Open("TestLinkJ.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "foo(and(bar))");
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.Inline, "[link](foo(and(bar))") });
        }

        /// <summary>
        /// Tests that if Link have unbalanced parentheses,
        /// they need to be escaped or used with the angle brackets.
        /// </summary>
        [Test]
        public void TestLinkK()
        {
            MarkdownDocument doc = Open("TestLinkK.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "foo(and(bar)");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "link", "foo(and(bar)");
        }

        /// <summary>
        /// Tests that parentheses and other symbols in the Link can also be escaped, as usual in Markdown.
        /// </summary>
        [Test]
        public void TestLinkL()
        {
            MarkdownDocument doc = Open("TestLinkL.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "foo):");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "link", "'.+-=*_%60~:!#$%25&@/?%5E%7C;\\%22%3C%3E%5B%5D()%7B%7D");
        }

        /// <summary>
        /// Tests that Links can contain fragment identifiers and queries.
        /// </summary>
        [Test]
        public void TestLinkM()
        {
            MarkdownDocument doc = Open("TestLinkM.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "#fragment");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "link", "http://example.com#fragment");
            CheckLinkTextBlock(((ParagraphBlock)doc[2])[0], "link", "http://example.com?foo=3#frag");
        }

        /// <summary>
        /// Tests that a backslash before a non-escapable character in the Link is just a backslash.
        /// </summary>
        [Test]
        public void TestLinkN()
        {
            MarkdownDocument doc = Open("TestLinkN.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "foo\\bar");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "li\\nk", "foobar");
            CheckLinkTextBlock(((ParagraphBlock)doc[2])[0], "link", "foo", "b\\ar");
        }

        /// <summary>
        /// Tests that URL-escaping should be left alone inside the Link destination,
        /// as all URL-escaped characters are also valid URL characters.
        /// </summary>
        [Test]
        public void TestLinkO()
        {
            MarkdownDocument doc = Open("TestLinkO.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "foo%20b&auml;");
        }

        /// <summary>
        /// Tests that, because titles can often be parsed as Link destinations,
        /// if you try to omit the destination and keep the title, you’ll get unexpected results.
        /// </summary>
        [Test]
        public void TestLinkP()
        {
            MarkdownDocument doc = Open("TestLinkP.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "%22title%22");
        }

        /// <summary>
        /// Tests that backslash escapes and entity and numeric character references may be used in Link titles.
        /// </summary>
        [Test]
        public void TestLinkQ()
        {
            MarkdownDocument doc = Open("TestLinkQ.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "/url", "title \"&quot;");
        }

        /// <summary>
        /// Tests that whitespace is allowed around the Link destination and title.
        /// </summary>
        /// <example>
        ///     [link](   /uri
        ///       "title"  )
        /// </example>
        [Test]
        public void TestLinkR()
        {
            MarkdownDocument doc = Open("TestLinkR.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "/uri", "title");
        }

        /// <summary>
        /// Tests that whitespace is not allowed between the Link text and the following parenthesis.
        /// </summary>
        [Test]
        public void TestLinkS()
        {
            MarkdownDocument doc = Open("TestLinkS.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "[link] (/uri)");
            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Inline, "[link] (/uri)") });
        }

        /// <summary>
        /// Tests that the Link text may contain balanced brackets, but not unbalanced ones, unless they are escaped.
        /// </summary>
        [Test]
        public void TestLinkT()
        {
            MarkdownDocument doc = Open("TestLinkT.md");
            Assert.That(doc.Count, Is.EqualTo(4));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link [foo [bar]]", "/uri");
            CheckInlines(doc[1], new[] {new ExpectedInline(BlockType.Inline, "[link] bar](/uri)")});
            CheckInlines(doc[2], new[] {
                new ExpectedInline(BlockType.Inline, "[link "),
                new ExpectedInline(BlockType.LinkText, "bar"),
                new ExpectedInline(BlockType.LinkDestination, "/uri")});
            CheckLinkTextBlock(((ParagraphBlock)doc[3])[0], "link [bar", "/uri");
        }

        /// <summary>
        /// Tests that the Link text may contain inline content.
        /// </summary>
        [Test]
        public void TestLinkU()
        {
            MarkdownDocument doc = Open("TestLinkU.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            LinkTextBlock linkTextBlock = (LinkTextBlock)((ParagraphBlock)doc[0])[0];
            CheckLinkTextBlock(linkTextBlock, "link *foo **bar** `#`*", "/uri");
            ItalicInlineBlock italicBlock = (ItalicInlineBlock)linkTextBlock[1];
            CheckInlines(italicBlock, new[]
                {
                    new ExpectedInline(BlockType.Inline, "foo "),
                    new ExpectedInline(BlockType.BoldInline, "**bar**"),
                    new ExpectedInline(BlockType.Inline, " "),
                    new ExpectedInline(BlockType.InlineCode, "`#`")
                });

            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.LinkText, "moonmoon.jpg"),
                new ExpectedInline(BlockType.LinkDestination, "/uri")});

            linkTextBlock = (LinkTextBlock)((ParagraphBlock)doc[1])[0];
            CheckImageBlock(linkTextBlock[0], "moon", "moon.jpg");
        }

        /// <summary>
        /// Tests that Links may not contain other Links, at any level of nesting.
        /// </summary>
        [Test]
        public void TestLinkV()
        {
            MarkdownDocument doc = Open("TestLinkV.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "[foo "),
                new ExpectedInline(BlockType.LinkText, "bar"),
                new ExpectedInline(BlockType.LinkDestination, "/uri"),
                new ExpectedInline(BlockType.Inline, "](/uri)") });

            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "[foo "),
                new ExpectedInline(BlockType.ItalicInline, "*[bar baz/uri](/uri)*"),
                new ExpectedInline(BlockType.Inline, "](/uri)") });
            ItalicInlineBlock italicBlock = (ItalicInlineBlock)((ParagraphBlock)doc[1])[1];
            CheckInlines(italicBlock, new[] {
                new ExpectedInline(BlockType.Inline, "[bar "),
                new ExpectedInline(BlockType.LinkText, "baz"),
                new ExpectedInline(BlockType.LinkDestination, "/uri"),
                new ExpectedInline(BlockType.Inline, "](/uri)") });

            CheckInlines(doc[2], new[] {
                new ExpectedInline(BlockType.ImageDescription, "[foo](uri2)"),
                new ExpectedInline(BlockType.LinkDestination, "uri3")
            });
        }

        /// <summary>
        /// Tests the precedence of Link text grouping over emphasis grouping.
        /// </summary>
        [Test]
        public void TestLinkW()
        {
            MarkdownDocument doc = Open("TestLinkW.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "*"),
                new ExpectedInline(BlockType.LinkText, "foo*"),
                new ExpectedInline(BlockType.LinkDestination, "/uri")});
            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.LinkText, "foo *bar"),
                new ExpectedInline(BlockType.LinkDestination, "baz*")});
        }

        /// <summary>
        /// Tests that brackets that aren’t part of Links do not take precedence.
        /// </summary>
        [Test]
        public void TestLinkX()
        {
            MarkdownDocument doc = Open("TestLinkX.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "*foo [bar* baz]");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.ItalicInline, "*foo [bar*"),
                new ExpectedInline(BlockType.Inline, " baz]") });
        }


        /// <summary>
        /// Tests that a Link destination consists of a sequence of characters between angle brackets that contains
        /// no unescaped angle brackets.
        /// </summary>
        [Test]
        public void TestLinkZ01()
        {
            MarkdownDocument doc = Open("TestLinkZ01.md");
            Assert.That(doc.Count, Is.EqualTo(8));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/uri");
            CheckInlines(doc[1], new[]
            {
                new ExpectedInline(BlockType.Inline, "[foo]("),
                new ExpectedInline(BlockType.HtmlTag, "</uri>"),
                new ExpectedInline(BlockType.Inline, ")")
            });
            CheckLinkTextBlock(((ParagraphBlock)doc[2])[0], "foo", "%3C/uri%3E");
            CheckLinkTextBlock(((ParagraphBlock)doc[3])[0], "foo", "%3C/uri%3E");
            CheckInlines(doc[4], new[]
            {
                new ExpectedInline(BlockType.Inline, "[foo]("),
                new ExpectedInline(BlockType.HtmlTag, "</u>"),
                new ExpectedInline(BlockType.Inline, "ri>)")
            });
            CheckLinkTextBlock(((ParagraphBlock)doc[5])[0], "foo", "/u%3Eri");
            CheckInlines(doc[6], new[]
            {
                new ExpectedInline(BlockType.Inline, "[foo]("),
                new ExpectedInline(BlockType.HtmlTag, "</u<ri>"),
                new ExpectedInline(BlockType.Inline, ")")
            });
            CheckLinkTextBlock(((ParagraphBlock)doc[7])[0], "foo", "/u%3Eri");
        }

        /// <summary>
        /// Tests that a Link titles may span multiple lines.
        /// </summary>
        [Test]
        public void TestLinkZ02()
        {
            MarkdownDocument doc = Open("TestLinkZ02.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.LinkText, "link"),
                new ExpectedInline(BlockType.LinkDestination, "/uri \"ti\vtle\"")});
        }

        /// <summary>
        /// Tests an empty Link title.
        /// </summary>
        [Test]
        public void TestLinkZ03()
        {
            MarkdownDocument doc = Open("TestLinkZ03.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "/uri");
        }

        /// <summary>
        /// Tests edge conditions during the Link parsing.
        /// </summary>
        [Test]
        public void TestLinkZ04()
        {
            MarkdownDocument doc = Open("TestLinkZ04.md");
            Assert.That(doc.Count, Is.EqualTo(6));

            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link", "/uri", "title");
            CheckInlines(doc[1], new[] { new ExpectedInline(BlockType.Inline, "[link](/uri \"title\" w)") });
            CheckInlines(doc[2], new[] { new ExpectedInline(BlockType.Inline, "[link](/uri \"title\" ww)") });
            CheckLinkTextBlock(((ParagraphBlock)doc[3])[0], "link", "");
            CheckLinkTextBlock(((ParagraphBlock)doc[4])[0], "link", "/uri");
            CheckLinkTextBlock(((ParagraphBlock)doc[5])[0], "link", "/uri");
        }

        /// <summary>
        /// Tests that backslash-escaped left square bracket is not valid for Link.
        /// </summary>
        [Test]
        public void TestLinkZ05()
        {
            MarkdownDocument doc = Open("TestLinkZ05.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "[foo](/uri)");

            CheckInlines(doc[0], new[] { new ExpectedInline(BlockType.Inline, "[foo](/uri)") });
        }


        /// <summary>
        /// Tests that AutoLink and InlineCode delimiters have a precedence over a InlineLink delimiter.
        /// </summary>
        [Test]
        public void TestLinkZ07()
        {
            MarkdownDocument doc = Open("TestLinkZ07.md");
            Assert.That(doc.Count, Is.EqualTo(4));

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "[n"),
                new ExpectedInline(BlockType.ItalicInline, "*o`t`a*"),
                new ExpectedInline(BlockType.BoldInline, "**li`nk](f`o**"),
                new ExpectedInline(BlockType.Inline, "o)")});

            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "[n"),
                new ExpectedInline(BlockType.ItalicInline, "*o`t`a*"),
                new ExpectedInline(BlockType.BoldInline, "**l<in:k](f>o**"),
                new ExpectedInline(BlockType.Inline, "o)")
            });

            BoldInlineBlock boldInlineBlock = (BoldInlineBlock)((ParagraphBlock)doc[1])[2];
            CheckInlines(boldInlineBlock, new[] {
                new ExpectedInline(BlockType.Inline, "l"),
                new ExpectedInline(BlockType.Autolink, "<in:k](f>"),
                new ExpectedInline(BlockType.Inline, "o")});

            LinkTextBlock linkTextBlock = (LinkTextBlock)((ParagraphBlock)doc[2])[0];
            CheckLinkTextBlock(linkTextBlock, "but*this`is`a***link", "f%60o**o");
            CheckInlines(linkTextBlock, new[] {
                new ExpectedInline(BlockType.Inline, "but"),
                new ExpectedInline(BlockType.ItalicInline, "*this`is`a*"),
                new ExpectedInline(BlockType.Inline, "**link")
            });

            CheckLinkTextBlock(((ParagraphBlock)doc[3])[0], "This`is](`link", "foo");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\Link\{0}", fileName));
        }
    }
}
