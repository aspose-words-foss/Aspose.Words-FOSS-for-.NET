// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/04/2025 by Ilya Navrotskiy

using Aspose.Words.Loading;
using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown hard line break feature.
    /// </summary>
    public class TestHardLineBreakParser : TestMarkdownParserBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests simple hard line break with 2 spaces.
        /// </summary>
        [Test]
        public void TestHardLineBreakA()
        {
            MarkdownDocument doc = Open("TestHardLineBreakA.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo"+HardBreakSpaces+"baz");
        }

        /// <summary>
        /// Tests simple hard line break with backslash '\'.
        /// </summary>
        [Test]
        public void TestHardLineBreakB()
        {
            MarkdownDocument doc = Open("TestHardLineBreakB.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo"+HardBreakSlash+"baz");
        }


        /// <summary>
        /// Tests that more than two spaces can be used.
        /// </summary>
        [Test]
        public void TestHardLineBreakC()
        {
            MarkdownDocument doc = Open("TestHardLineBreakC.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo"+HardBreakSpaces+"baz");
        }

        /// <summary>
        /// Tests that leading spaces at the beginning of the next line are ignored in line break with two spaces.
        /// </summary>
        [Test]
        public void TestHardLineBreakD()
        {
            MarkdownDocument doc = Open("TestHardLineBreakD.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo"+ HardBreakSpaces +" bar");
        }


        /// <summary>
        /// Tests that leading spaces at the beginning of the next line are ignored in line break with backslash.
        /// </summary>
        [Test]
        public void TestHardLineBreakE()
        {
            MarkdownDocument doc = Open("TestHardLineBreakE.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo"+HardBreakSlash+" bar");
        }

        /// <summary>
        /// Tests that hard line breaks can occur inside emphasis, links, and other constructs that allow inline content.
        /// </summary>
        /// <remarks>Line break with two spaces.</remarks>
        [Test]
        public void TestHardLineBreakF()
        {
            MarkdownDocument doc = Open("TestHardLineBreakF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "*foo"+ HardBreakSpaces +"bar*");
            CheckItalicInline(((ParagraphBlock)doc[0])[0], "*foo"+HardBreakSpaces+"bar*");
        }

        /// <summary>
        /// Tests that hard line breaks can occur inside emphasis, links, and other constructs that allow inline content.
        /// </summary>
        /// <remarks>Line break with backslash.</remarks>
        [Test]
        public void TestHardLineBreakG()
        {
            MarkdownDocument doc = Open("TestHardLineBreakG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "*foo"+HardBreakSlash+"bar*");
            CheckItalicInline(((ParagraphBlock)doc[0])[0], "*foo"+HardBreakSlash+"bar*");
        }

        /// <summary>
        /// Tests that hard line breaks do not occur inside code spans.
        /// </summary>
        /// <remarks>Line break with two spaces.</remarks>
        [Test]
        public void TestHardLineBreakH()
        {
            MarkdownDocument doc = Open("TestHardLineBreakH.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "`code   span`");
        }

        /// <summary>
        /// Tests that hard line breaks do not occur inside code spans.
        /// </summary>
        /// <remarks>Line break with backslash.</remarks>
        [Test]
        public void TestHardLineBreakI()
        {
            MarkdownDocument doc = Open("TestHardLineBreakI.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "`code\\ span`");
        }


        /// <summary>
        /// Tests that hard line breaks do not occur inside HTML tags.
        /// </summary>
        /// <remarks>Line break with two spaces.</remarks>
        [Test]
        public void TestHardLineBreakJ()
        {
            MarkdownDocument doc = Open("TestHardLineBreakJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "<a href=\"foo"+HardBreakSpaces+"bar\">");
        }


        /// <summary>
        /// Tests that hard line breaks do not occur inside HTML tags.
        /// </summary>
        /// <remarks>Line break with backslash.</remarks>
        [Test]
        public void TestHardLineBreakK()
        {
            MarkdownDocument doc = Open("TestHardLineBreakK.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "<a href=\"foo"+HardBreakSlash+"bar\">");
        }

        /// <summary>
        /// Tests that neither syntax for hard line breaks works at the end
        /// of a paragraph or other block element.
        /// </summary>
        [Test]
        public void TestHardLineBreakL()
        {
            MarkdownDocument doc = Open("TestHardLineBreakL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo\\");
        }

        /// <summary>
        /// Tests that neither syntax for hard line breaks works at the end
        /// of a paragraph or other block element.
        /// </summary>
        [Test]
        public void TestHardLineBreakM()
        {
            MarkdownDocument doc = Open("TestHardLineBreakM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo");
        }

        /// <summary>
        /// Tests that neither syntax for hard line breaks works at the end
        /// of a paragraph or other block element.
        /// </summary>
        [Test]
        public void TestHardLineBreakN()
        {
            MarkdownDocument doc = Open("TestHardLineBreakN.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckAtxHeading(doc[0], 3, "foo\\");
        }

        /// <summary>
        /// Tests that neither syntax for hard line breaks works at the end
        /// of a paragraph or other block element.
        /// </summary>
        [Test]
        public void TestHardLineBreakO()
        {
            MarkdownDocument doc = Open("TestHardLineBreakO.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckAtxHeading(doc[0], 3, "foo");
        }

        /// <summary>
        /// Tests hard break with whitespace lines.
        /// </summary>
        [Test]
        public void TestHardLineBreak1()
        {
            MarkdownDocument doc = Open("TestHardLineBreak1.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckIndentedCode(doc[0], "\\");
            CheckParagraph(doc[1], HardBreakSlash+"\\");
        }

        /// <summary>
        /// Tests single backslash in file.
        /// </summary>
        [Test]
        public void TestHardLineBreak2()
        {
            MarkdownDocument doc = Open("TestHardLineBreak2.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "\\");
        }

        /// <summary>
        /// Tests two lines with only one backslash in each.
        /// </summary>
        [Test]
        public void TestHardLineBreak3()
        {
            MarkdownDocument doc = Open("TestHardLineBreak3.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], HardBreakSlash + "\\");
        }

        /// <summary>
        /// Tests escaped backslash.
        /// </summary>
        [Test]
        public void TestHardLineBreak4()
        {
            MarkdownDocument doc = Open("TestHardLineBreak4.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "abc\\" + SoftBreak + "def");
        }

        /// <summary>
        /// Tests two lines.
        /// </summary>
        [Test]
        public void TestHardLineBreak5()
        {
            MarkdownDocument doc = Open("TestHardLineBreak5.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "abc"+HardBreakSlash+"def");
        }

        /// <summary>
        /// Tests backslash hard line with backslash before it followed by space.
        /// </summary>
        [Test]
        public void TestHardLineBreak6()
        {
            MarkdownDocument doc = Open("TestHardLineBreak6.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "abc\\ "+HardBreakSlash+"def");
        }

        /// <summary>
        /// Returns Markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return Open(fileName, null);
        }

        /// <summary>
        /// Returns Markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName, MarkdownLoadOptions loadOptions)
        {
            return base.Open(string.Format(@"ImportMarkdown\HardLineBreak\{0}", fileName), loadOptions);
        }
    }
}
