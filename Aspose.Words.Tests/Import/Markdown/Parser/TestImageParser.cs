// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2020 by Mikhail Nepreteamov

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Image feature.
    /// </summary>
    public class TestImageParser : TestMarkdownParserBase
    {

        /// <summary>
        /// Tests Image with a title.
        /// </summary>
        [Test]
        public void TestImageB()
        {
            MarkdownDocument doc = Open("TestImageB.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            ParagraphBlock paragraphBlock = (ParagraphBlock)doc[0];
            CheckParagraph(paragraphBlock, "alt text/uri \"title\"");
            CheckImageBlock(paragraphBlock[0], "alt text", "/uri", "title");

            paragraphBlock = (ParagraphBlock)doc[1];
            CheckParagraph(paragraphBlock, "My foo bar/path/to/train.jpg  \"title\"   ");
            Assert.That(paragraphBlock[0].Text, Is.EqualTo("My "));
            CheckImageBlock(paragraphBlock[1], "foo bar", "/path/to/train.jpg", "title");
        }

        /// <summary>
        /// Tests that it is recommended that in rendering, only the plain string content of the Image description
        /// be used, without formatting.
        /// </summary>
        [Test]
        public void TestImageC()
        {
            MarkdownDocument doc = Open("TestImageC.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            ParagraphBlock paragraphBlock = (ParagraphBlock)doc[0];
            CheckParagraph(paragraphBlock, "foo bar/url2");
            CheckImageBlock(paragraphBlock[0], "foo bar", "/url2");

            paragraphBlock = (ParagraphBlock)doc[1];
            CheckParagraph(paragraphBlock, "foo bar/url2");
            CheckImageBlock(paragraphBlock[0], "foo bar", "/url2");
        }


        /// <summary>
        /// Tests Image with empty description.
        /// </summary>
        [Test]
        public void TestImageE()
        {
            MarkdownDocument doc = Open("TestImageE.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            ParagraphBlock paragraphBlock = (ParagraphBlock)doc[0];
            CheckParagraph(paragraphBlock, "/url");
            CheckImageBlock(paragraphBlock[0], "", "/url");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\Image\{0}", fileName));
        }
    }
}
