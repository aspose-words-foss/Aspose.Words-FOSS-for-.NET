// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown regular paragraphs.
    /// </summary>
    public class TestParagraphParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests an empty input document.
        /// </summary>
        [Test]
        public void TestEmptyA()
        {
            MarkdownDocument doc = Open("TestEmptyA.md");
            Assert.That(doc.IsEmpty, Is.True);
        }

        /// <summary>
        /// Tests a document that contains only whitespace characters.
        /// </summary>
        [Test]
        public void TestEmptyB()
        {
            MarkdownDocument doc = Open("TestEmptyB.md");
            Assert.That(doc.IsEmpty, Is.True);
        }

        /// <summary>
        /// Tests paragraphs when there are no any whitespace characters or empty lines at the very start and end of a file.
        /// </summary>
        [Test]
        public void TestParagraphA()
        {
            MarkdownDocument doc = Open("TestParagraphA.md");

            CheckParagraph(doc[0], "Lorem ipsum"+SoftBreak+"dolor sit amet,"+SoftBreak+"consectetur adipiscing");
            CheckParagraph(doc[1], "elit, sed"+SoftBreak+"do eiusmod");
            CheckParagraph(doc[2], "tempor incididunt");
        }

        /// <summary>
        /// Tests paragraphs when there are blank lines at the start and end of a file.
        /// </summary>
        [Test]
        public void TestParagraphB()
        {
            MarkdownDocument doc = Open("TestParagraphB.md");

            CheckParagraph(doc[0], "Lorem ipsum");
            CheckParagraph(doc[1], "dolor sit amet,");
        }

        /// <summary>
        /// Tests paragraphs when input document has lines with spaces at the start.
        /// </summary>
        [Test]
        public void TestParagraphC()
        {
            MarkdownDocument doc = Open("TestParagraphC.md");
            CheckParagraph(doc[0], "aaa"+SoftBreak+"         bbb"+SoftBreak+"                         ccc");
        }

        /// <summary>
        /// Tests paragraphs when input document has lines with spaces at the end.
        /// </summary>
        [Test]
        public void TestParagraphD()
        {
            MarkdownDocument doc = Open("TestParagraphD.md");
            CheckParagraph(doc[0], "aaa"+HardBreakSpaces+"bbb");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Paragraphs\{0}", fileName));
        }
    }
}
