// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown HorizontalRule feature.
    /// </summary>
    public class TestHorizontalRuleParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple horizontal rules.
        /// </summary>
        [Test]
        public void TestHorizontalRuleA()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleA.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckHorizontalRule(doc[0]);
            CheckHorizontalRule(doc[1]);
            CheckHorizontalRule(doc[2]);
        }

        /// <summary>
        /// Tests wrong characters sequence +++ does not constitute a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleB()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleB.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "+++");
        }

        /// <summary>
        /// Tests wrong characters sequence === does not constitute a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleC()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleC.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "===");
        }

        /// <summary>
        /// Tests that a sequence of opening characters MUST have a minimum length of 3 to constitute a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleD()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleD.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "--"+SoftBreak+"**"+SoftBreak+"__");
        }

        /// <summary>
        /// Tests that one to three spaces indent are allowed in a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleE()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleE.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckHorizontalRule(doc[0]);
            CheckHorizontalRule(doc[1]);
            CheckHorizontalRule(doc[2]);
        }

        /// <summary>
        /// Tests that four indentation spaces are too many for a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleF()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "***");
        }

        /// <summary>
        /// Tests that four indentation spaces are too many for a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleG()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "Foo"+SoftBreak+"***");
        }

        /// <summary>
        /// Tests that more than three characters may be used in a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleH()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleH.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckHorizontalRule(doc[0]);
        }

        /// <summary>
        /// Tests that spaces are allowed between the characters in a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleI()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleI.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            Assert.That(doc[0].Type, Is.EqualTo(BlockType.HorizontalRule));
            Assert.That(doc[1].Type, Is.EqualTo(BlockType.HorizontalRule));
            Assert.That(doc[2].Type, Is.EqualTo(BlockType.HorizontalRule));
        }

        /// <summary>
        /// Tests that spaces are allowed at the end in a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleJ()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            Assert.That(doc[0].Type, Is.EqualTo(BlockType.HorizontalRule));
        }

        /// <summary>
        /// Tests that no other characters may occur in the line in a horizontal rule except of allowed Openings.
        /// </summary>
        [Test]
        public void TestHorizontalRuleK()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleK.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "_ _ _ _ a");
            CheckParagraph(doc[1], "a------");
            CheckParagraph(doc[2], "---a---");
        }

        /// <summary>
        /// Tests that all of the non-whitespace characters MUST be the same.
        /// So, in this test it is not a horizontal rule.
        /// </summary>
        [Test]
        public void TestHorizontalRuleL()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "*-*");
        }

        /// <summary>
        /// Tests that horizontal rules do not need blank lines before or after.
        /// </summary>
        [Test]
        public void TestHorizontalRuleM()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleM.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckHorizontalRule(doc[1]);
            CheckBulletListItem(doc[2], ListMarker.Minus, "bar");
        }

        /// <summary>
        /// Tests that horizontal rules can interrupt a paragraph.
        /// </summary>
        [Test]
        public void TestHorizontalRuleN()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleN.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckParagraph(doc[0], "Foo");
            CheckHorizontalRule(doc[1]);
            CheckParagraph(doc[2], "bar");
        }

        /// <summary>
        /// Tests that if a line of dashes that can be interpreted as either a horizontal rule or the underline of a setext
        /// heading, the interpretation as a setext heading takes precedence.
        /// </summary>
        [Test]
        public void TestHorizontalRuleO()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleO.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckSetextHeading(doc[0], 2, "Foo");
            CheckParagraph(doc[1], "bar");
        }

        /// <summary>
        /// Tests that when both a horizontal rule and a list item are possible interpretations of a line,
        /// the horizontal rule takes precedence.
        /// </summary>
        [Test]
        public void TestHorizontalRuleP()
        {
            MarkdownDocument doc = Open("TestHorizontalRuleP.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckBulletListItem(doc[0], ListMarker.Asterisk, "Foo");
            CheckHorizontalRule(doc[1]);
            CheckBulletListItem(doc[2], ListMarker.Asterisk, "Bar");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\HorizontalRules\{0}", fileName));
        }
    }
}
