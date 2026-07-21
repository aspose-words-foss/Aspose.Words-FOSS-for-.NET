// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown FencedCode feature.
    /// </summary>
    public class TestFencedCodeParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple fenced code with backticks.
        /// </summary>
        [Test]
        public void TestFencedCodeA()
        {
            MarkdownDocument doc = Open("TestFencedCodeA.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "<\v >");
        }

        /// <summary>
        /// Tests simple fenced code with tildes.
        /// </summary>
        [Test]
        public void TestFencedCodeB()
        {
            MarkdownDocument doc = Open("TestFencedCodeB.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "<\v >");
        }

        /// <summary>
        /// Tests that fewer than three backticks is not enough.
        /// </summary>
        [Test]
        public void TestFencedCodeC()
        {
            MarkdownDocument doc = Open("TestFencedCodeC.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "``foo``");
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.InlineCode, "``foo``")});
        }

        /// <summary>
        /// Tests that the closing code fence uses the same character as the opening fence.
        /// </summary>
        [Test]
        public void TestFencedCodeD()
        {
            MarkdownDocument doc = Open("TestFencedCodeD.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v~~~");
        }

        /// <summary>
        /// Tests that the closing code fence uses the same character as the opening fence.
        /// </summary>
        [Test]
        public void TestFencedCodeE()
        {
            MarkdownDocument doc = Open("TestFencedCodeE.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v```");
        }

        /// <summary>
        /// Tests that the closing code fence must be at least as long as the opening fence.
        /// </summary>
        [Test]
        public void TestFencedCodeF()
        {
            MarkdownDocument doc = Open("TestFencedCodeF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v```");
        }

        /// <summary>
        /// Tests that the closing code fence must be at least as long as the opening fence.
        /// </summary>
        [Test]
        public void TestFencedCodeG()
        {
            MarkdownDocument doc = Open("TestFencedCodeG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v~~~");
        }


        /// <summary>
        /// Tests that unclosed code blocks are closed by the end of the document.
        /// </summary>
        [Test]
        public void TestFencedCodeI()
        {
            MarkdownDocument doc = Open("TestFencedCodeI.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "\v```\vaaa");
        }

        /// <summary>
        /// Tests that unclosed code blocks are closed by the enclosing block quote.
        /// </summary>
        [Test]
        public void TestFencedCodeJ()
        {
            MarkdownDocument doc = Open("TestFencedCodeJ.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckFencedCode(quoteBlock[0], "aaa");
            CheckParagraph(doc[1], "bbb");
        }

        /// <summary>
        /// Tests that a code block can have all empty lines as its content.
        /// </summary>
        [Test]
        public void TestFencedCodeK()
        {
            MarkdownDocument doc = Open("TestFencedCodeK.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "\v  ");
        }


        /// <summary>
        /// Tests that fences can be indented. If the opening fence is indented, content lines will have
        /// equivalent opening indentation removed, if present.
        /// </summary>
        [Test]
        public void TestFencedCodeM()
        {
            MarkdownDocument doc = Open("TestFencedCodeM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\vaaa");
        }

        /// <summary>
        /// Tests that fences can be indented. If the opening fence is indented, content lines will have
        /// equivalent opening indentation removed, if present.
        /// </summary>
        [Test]
        public void TestFencedCodeN()
        {
            MarkdownDocument doc = Open("TestFencedCodeN.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\vaaa\vaaa");
        }

        /// <summary>
        /// Tests that fences can be indented. If the opening fence is indented, content lines will have
        /// equivalent opening indentation removed, if present.
        /// </summary>
        [Test]
        public void TestFencedCodeO()
        {
            MarkdownDocument doc = Open("TestFencedCodeO.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v aaa\vaaa");
        }

        /// <summary>
        /// Tests that four spaces indentation produces an indented code block.
        /// </summary>
        [Test]
        public void TestFencedCodeP()
        {
            MarkdownDocument doc = Open("TestFencedCodeP.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "```\vaaa\v```");
        }

        /// <summary>
        /// Tests that closing fences may be indented by 0-3 spaces,
        /// and their indentation need not match that of the opening fence.
        /// </summary>
        [Test]
        public void TestFencedCodeQ()
        {
            MarkdownDocument doc = Open("TestFencedCodeQ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa");
        }

        /// <summary>
        /// Tests that closing fences may be indented by 0-3 spaces,
        /// and their indentation need not match that of the opening fence.
        /// </summary>
        [Test]
        public void TestFencedCodeR()
        {
            MarkdownDocument doc = Open("TestFencedCodeR.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa");
        }

        /// <summary>
        /// Tests that this is not a closing fence, because it is indented 4 spaces.
        /// </summary>
        [Test]
        public void TestFencedCodeS()
        {
            MarkdownDocument doc = Open("TestFencedCodeS.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v    ```");
        }

        /// <summary>
        /// Tests that opening fence cannot contain internal spaces.
        /// </summary>
        [Test]
        public void TestFencedCodeT()
        {
            MarkdownDocument doc = Open("TestFencedCodeT.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            ParagraphBlock paragraphBlock = (ParagraphBlock)doc[0];
            CheckParagraph(paragraphBlock, "``` ```"+SoftBreak+"aaa");
            CheckInlines(doc[0], new [] {
                new ExpectedInline(BlockType.InlineCode, "``` ```"),
                new ExpectedInline(BlockType.Inline, SoftBreak+"aaa")});
        }

        /// <summary>
        /// Tests that closing fence cannot contain internal spaces.
        /// </summary>
        [Test]
        public void TestFencedCodeU()
        {
            MarkdownDocument doc = Open("TestFencedCodeU.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "aaa\v~~~ ~~");
        }

        /// <summary>
        /// Tests that fenced code blocks can interrupt paragraphs, and can be followed directly by paragraphs,
        /// without a blank line between.
        /// </summary>
        [Test]
        public void TestFencedCodeV()
        {
            MarkdownDocument doc = Open("TestFencedCodeV.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "foo");
            CheckFencedCode(doc[1], "bar");
            CheckParagraph(doc[2], "baz");
        }

        /// <summary>
        /// Tests that other blocks can also occur before and after fenced code blocks without an intervening blank line.
        /// </summary>
        [Test]
        public void TestFencedCodeW()
        {
            MarkdownDocument doc = Open("TestFencedCodeW.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckSetextHeading(doc[0], 2, "foo");
            CheckFencedCode(doc[1], "bar");
            CheckAtxHeading(doc[2], 1, "baz");
        }

        /// <summary>
        /// Tests that an info string can be provided after the opening code fence.
        /// </summary>
        [Test]
        public void TestFencedCodeX()
        {
            MarkdownDocument doc = Open("TestFencedCodeX.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "def foo(x)\v  return 3\vend", "ruby");
        }

        /// <summary>
        /// Tests that an info string is truncated to the first occurred whitespace character.
        /// </summary>
        [Test]
        public void TestFencedCodeY()
        {
            MarkdownDocument doc = Open("TestFencedCodeY.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "def foo(x)\v  return 3\vend", "ruby");
        }

        /// <summary>
        /// Tests that the content of fenced code block can be empty when info string exists.
        /// </summary>
        [Test]
        public void TestFencedCodeZ()
        {
            MarkdownDocument doc = Open("TestFencedCodeZ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "", ";");
        }

        /// <summary>
        /// Tests that info strings for backtick code blocks cannot contain backticks.
        /// </summary>
        [Test]
        public void TestFencedCodeZ1()
        {
            MarkdownDocument doc = Open("TestFencedCodeZ1.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "```aa```"+SoftBreak+"foo");
            CheckInlines(doc[0], new [] {
                new ExpectedInline(BlockType.InlineCode, "```aa```"),
                new ExpectedInline(BlockType.Inline, SoftBreak+"foo")});
        }

        /// <summary>
        /// Tests that info strings for tilde code blocks can contain backticks and tildes.
        /// </summary>
        [Test]
        public void TestFencedCodeZ2()
        {
            MarkdownDocument doc = Open("TestFencedCodeZ2.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "foo", "aa");
        }

        /// <summary>
        /// Tests that closing code fences cannot have info strings.
        /// </summary>
        [Test]
        public void TestFencedCodeZ3()
        {
            MarkdownDocument doc = Open("TestFencedCodeZ3.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckFencedCode(doc[0], "``` aaa");
        }

        /// <summary>
        /// Tests parsing of a complex markdown document with FencedCode.
        /// </summary>
        [Test]
        public void TestFencedCode()
        {
            MarkdownDocument doc = base.Open(@"ImportMarkdown\TestFencedCode.md");

            CheckFencedCode(doc[0], "Lorem ipsum");
            CheckFencedCode(doc[1], "dolor", "C#");
            CheckFencedCode(doc[2], "sit\vamet,", "C++");
            CheckAtxHeading(doc[3], 3, "adipiscing elit,");
            CheckFencedCode(doc[4], "# eiusmod");

            QuoteBlock quoteBlock = (QuoteBlock)doc[5];
            CheckFencedCode(quoteBlock[0], "   incididunt", "Java");

            CheckFencedCode(doc[6], "> ut labore\v> et dlr");
            CheckFencedCode(doc[7], "quis");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Code\FencedCode\{0}", fileName));
        }
    }
}
