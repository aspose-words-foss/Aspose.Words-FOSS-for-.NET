// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown ATX Heading feature.
    /// </summary>
    public class TestAtxHeadingParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple ATX headings.
        /// </summary>
        [Test]
        public void TestAtxHeadingA()
        {
            MarkdownDocument doc = Open("TestATXHeadingA.md");
            CheckAtxHeading(doc[0], 1, "Heading 1");
            CheckAtxHeading(doc[1], 2, "Heading 2");
            CheckAtxHeading(doc[2], 3, "Heading 3");
            CheckAtxHeading(doc[3], 4, "Heading 4");
            CheckAtxHeading(doc[4], 5, "Heading 5");
            CheckAtxHeading(doc[5], 6, "Heading 6");
        }

        /// <summary>
        /// Tests that more than six # characters is not a ATX heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingB()
        {
            MarkdownDocument doc = Open("TestATXHeadingB.md");
            CheckParagraph(doc[0], "####### foo");
        }

        /// <summary>
        /// Tests that at least one space is required between the # characters and the heading’s contents in ATX heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingC()
        {
            MarkdownDocument doc = Open("TestATXHeadingC.md");
            CheckParagraph(doc[0], "#5 bolt");
            CheckParagraph(doc[1], "#hashtag");
        }

        /// <summary>
        /// Tests ATX heading with leading and trailing blanks.
        /// </summary>
        [Test]
        public void TestAtxHeadingD()
        {
            MarkdownDocument doc = Open("TestATXHeadingD.md");
            CheckAtxHeading(doc[0], 1, "foo");
        }

        /// <summary>
        /// Tests that one to three spaces indentation are allowed in ATX headings.
        /// </summary>
        [Test]
        public void TestAtxHeadingE()
        {
            MarkdownDocument doc = Open("TestATXHeadingE.md");
            CheckAtxHeading(doc[0], 3, "foo");
            CheckAtxHeading(doc[1], 2, "bar");
            CheckAtxHeading(doc[2], 1, "baz");
        }

        /// <summary>
        /// Tests that four indentation spaces are too much for a ATX heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingF()
        {
            MarkdownDocument doc = Open("TestATXHeadingF.md");
            CheckIndentedCode(doc[0], "# foo");
        }

        /// <summary>
        /// Tests a closing sequence of # characters is optional in ATX headings.
        /// </summary>
        [Test]
        public void TestAtxHeadingG()
        {
            MarkdownDocument doc = Open("TestATXHeadingG.md");
            CheckAtxHeading(doc[0], 2, "foo");
            CheckAtxHeading(doc[1], 3, "bar");
        }

        /// <summary>
        /// Tests a closing sequence need not be the same length as the opening sequence in a ATX heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingH()
        {
            MarkdownDocument doc = Open("TestATXHeadingH.md");
            CheckAtxHeading(doc[0], 1, "foo");
            CheckAtxHeading(doc[1], 5, "bar");
        }

        /// <summary>
        /// Tests spaces are allowed after the closing sequence in a ATX heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingI()
        {
            MarkdownDocument doc = Open("TestATXHeadingI.md");
            CheckAtxHeading(doc[0], 3, "foo");
        }

        /// <summary>
        /// Tests that a sequence of # characters with anything but spaces following it is not a closing sequence,
        /// but counts as part of the contents of the heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingJ()
        {
            MarkdownDocument doc = Open("TestATXHeadingJ.md");
            CheckAtxHeading(doc[0], 3, "foo ### b");
        }

        /// <summary>
        /// Tests the closing sequence must be preceded by a space in a ATX heading.
        /// </summary>
        [Test]
        public void TestAtxHeadingK()
        {
            MarkdownDocument doc = Open("TestATXHeadingK.md");
            CheckAtxHeading(doc[0], 1, "foo#");
        }

        /// <summary>
        /// Tests ATX headings need not be separated from surrounding content by blank lines.
        /// </summary>
        [Test]
        public void TestAtxHeadingL()
        {
            MarkdownDocument doc = Open("TestATXHeadingL.md");
            CheckParagraph(doc[0], "Foo bar");
            CheckAtxHeading(doc[1], 1, "baz");
            CheckParagraph(doc[2], "Bar foo");
        }

        /// <summary>
        /// Tests that ATX headings can be empty.
        /// </summary>
        [Test]
        public void TestAtxHeadingM()
        {
            MarkdownDocument doc = Open("TestATXHeadingM.md");
            CheckAtxHeading(doc[0], 2, "");
            CheckAtxHeading(doc[1], 1, "");
            CheckAtxHeading(doc[2], 3, "");
        }
        
        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Headings\ATX\{0}", fileName));
        }
    }
}
