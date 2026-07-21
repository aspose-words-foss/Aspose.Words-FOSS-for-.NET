// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2019 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Loading;
using Aspose.Words.Notes;
using Aspose.Words.RW.Markdown;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export;
using Aspose.Words.Tests.Import.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown
{
    /// <summary>
    /// Tests import from markdown format.
    /// </summary>
    [TestFixture]
    public class TestImportMarkdown : TestMarkdownBase
    {
        /// <summary>
        /// Tests reading an empty markdown document.
        /// </summary>
        [Test]
        public void TestEmptyA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Paragraphs\TestEmptyA.md");
            Assert.That(doc.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Tests reading a markdown document that contains only whitespace characters.
        /// </summary>
        [Test]
        public void TestEmptyB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Paragraphs\TestEmptyB.md");
            Assert.That(doc.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Tests reading a simple markdown document.
        /// </summary>
        [Test]
        public void TestParagraphA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Paragraphs\TestParagraphA.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckParagraph(paras[0], "Lorem ipsum dolor sit amet, consectetur adipiscing\r", "Normal");
            CheckParagraph(paras[1], "elit, sed do eiusmod\r", "Normal");
            CheckParagraph(paras[2], "tempor incididunt\f", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with empty lines at start and end.
        /// </summary>
        [Test]
        public void TestParagraphB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Paragraphs\TestParagraphB.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "Lorem ipsum\r", "Normal");
            CheckParagraph(paras[1], "dolor sit amet,\f", "Normal");

            Assert.That(paras.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests reading a markdown document with whitespace characters at the start and end of lines.
        /// </summary>
        [Test]
        public void TestParagraphC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Paragraphs\TestParagraphC.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "aaa bbb ccc\f", "Normal");
            Assert.That(paras.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests reading a markdown document with AtxHeadings.
        /// </summary>
        [Test]
        public void TestAtxHeading()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestAtxHeading.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "Lorem ipsum\r", "Heading 1");
            CheckParagraph(paras[1], "dolor sit amet,\r", "Normal");
            CheckParagraph(paras[2], "consectetur\r", "Heading 2");
            CheckHorizontalRule(paras[3], "Normal");
            CheckParagraph(paras[4], "----\r", "Heading 3");
            CheckParagraph(paras[5], "####adipiscing elit,\r", "Normal");
            CheckParagraph(paras[6], "incididunt ut\r", "Heading 4");
            CheckParagraph(paras[7], "eiusmod\r", "Heading 5");
            CheckParagraph(paras[8], "labore et# # # #\r", "Heading 6");
            CheckParagraph(paras[9], "####### sed do\r", "Normal");
            CheckParagraph(paras[10], "tempor##\f", "Heading 2");

            Assert.That(paras.Count, Is.EqualTo(11));
        }

        /// <summary>
        /// Tests reading a markdown document with SetextHeadings.
        /// </summary>
        [Test]
        public void TestSetextHeadingA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestSetextHeadingA.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "Lorem ipsum\r", "SetextHeading1", "Heading 1");
            CheckParagraph(paras[1], "dolor sit amet, consectetur\r", "SetextHeading2", "Heading 2");
            CheckParagraph(paras[2], "\f", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with SetextHeadings.
        /// </summary>
        [Test]
        public void TestSetextHeadingB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestSetextHeadingB.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "  Lorem ipsum\r", "IndentedCode");
            CheckParagraph(paras[1], "================ dolor sit amet, consectetur\r", "SetextHeading2", "Heading 2");
            CheckHorizontalRule(paras[2], "Normal");
            CheckParagraph(paras[3], "adipiscing elit,\f", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading a markdown document with Quotes.
        /// </summary>
        [Test]
        public void TestQuoteA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestQuoteA.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "Lorem ipsum\r", "Quote", "SetextHeading1", "Heading 1");
            CheckParagraph(paras[1], "dolor sit amet,\r", "Quote2", "Quote1", "Normal");
            CheckParagraph(paras[2], "consectetur\r", "Quote3", "Quote2");
            CheckHorizontalRule(paras[3], "Normal");
            CheckHorizontalRule(paras[4], "Quote2");

            Assert.That(paras.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests reading a markdown document with Quotes.
        /// </summary>
        [Test]
        public void TestQuoteB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestQuoteB.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "Lorem\r", "Quote", "Normal");
            CheckParagraph(paras[1], "ipsum\r", "Quote1", "Quote", "Normal");
            CheckParagraph(paras[2], "dolor\r", "Quote2", "Quote1", "Quote", "Normal");
            CheckParagraph(paras[3], "sit\r", new ExpectedStyleName[] {new ExpectedStyleName("Quote", 58),
                new ExpectedStyleName("Quote2", 1), new ExpectedStyleName("Quote1", 1), new ExpectedStyleName("Quote", 1), new ExpectedStyleName("Normal", 1)});
            CheckParagraph(paras[4], "amet,\r", "Quote61", "Heading 1");
            CheckParagraph(paras[5], "consectetur\r", "Quote63", "Quote62", "Heading 2");
            CheckParagraph(paras[6], "adipiscing\r", "Quote66", "Quote65", "Quote64", "Heading 3");
            CheckParagraph(paras[7], "\r", "Normal");
            CheckParagraph(paras[8], "elit\f", "Quote62", "Heading 2");

            Assert.That(paras.Count, Is.EqualTo(9));
        }

        /// <summary>
        /// Tests reading a markdown document with Quotes separated with blank lines.
        /// </summary>
        [Test]
        public void TestQuoteC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Quotes\TestQuoteV.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "baz\r", "Quote", "Normal");
            CheckParagraph(paras[1], "\r", "Normal");
            CheckParagraph(paras[2], "tempor incididunt\f", "Quote1", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with Quotes with Headings inside.
        /// </summary>
        [Test]
        public void TestQuoteD()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Quotes\TestQuoteW.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "foo\r", "Quote", "Heading 1");
            CheckParagraph(paras[1], "\r", "Normal");
            CheckParagraph(paras[2], "bar\r", "Quote1", "Normal");
            CheckParagraph(paras[3], "\r", "Normal");
            CheckParagraph(paras[4], "baz\r", "Quote", "Heading 1");
            CheckParagraph(paras[5], "bop\f", "Quote1", "Normal");

            Assert.That(paras.Count, Is.EqualTo(6));
        }


        /// <summary>
        /// Tests reading a markdown document with horizontal rules.
        /// </summary>
        [Test]
        public void TestHorizontalRuleA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestHorizontalRuleA.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            foreach (Paragraph para in paras)
                CheckHorizontalRule(para, "Normal");

            Assert.That(paras.Count, Is.EqualTo(9));
        }

        /// <summary>
        /// Tests reading a markdown document with horizontal rules.
        /// </summary>
        [Test]
        public void TestHorizontalRuleB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestHorizontalRuleB.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));
            CheckParagraph(paras[0], "--- Lorem ipsum\r", "SetextHeading2", "Heading 2");
            CheckParagraph(paras[1], "dolor sit amet, ------\r", "SetextHeading2", "Heading 2");
            CheckParagraph(paras[2], "consectetur\f", "SetextHeading2", "Heading 2");
        }

        /// <summary>
        /// Tests reading a markdown document with emphases.
        /// </summary>
        [Test]
        public void TestEmphasis()
        {
            // This is a small but very complex file with many emphases. It will be better to check gold here.
            Document doc = TestUtil.OpenSaveOpen(@"ImportMarkdown\TestEmphasis", UnifiedScenario.Md2Docx);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            RunCollection runs = paras[0].Runs;
            Assert.That(runs.Count, Is.EqualTo(21));
        }

        /// <summary>
        /// Tests reading a markdown document with IndentedCode.
        /// </summary>
        [Test]
        public void TestIndentedCode()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestIndentedCode.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(11));
            CheckParagraph(paras[0], "Lorem ipsum\r", "IndentedCode");
            CheckParagraph(paras[1], "dolor sit amet,       consectetur # adipiscing elit,\r", "Normal");
            CheckParagraph(paras[2], "sed do\r", "Heading 1");
            CheckParagraph(paras[3], "eiusmod\v\vtempor\vincididunt\vut labore\v> et dolore\r", "IndentedCode");
            CheckParagraph(paras[4], "magna aliqua.\r", "Quote", "Normal");
            CheckParagraph(paras[5], "Ut enim\r", "Quote1", "IndentedCode");
            CheckParagraph(paras[6], "ad minim\r", "IndentedCode");
            CheckParagraph(paras[7], "veniam\r", "Quote", "Normal");
            CheckParagraph(paras[8], "quis\r", "IndentedCode");
            CheckParagraph(paras[9], "======== nostrud\r", "SetextHeading1", "Heading 1");
            CheckParagraph(paras[10], "exercitation\f", "IndentedCode");
        }

        /// <summary>
        /// Tests reading a markdown document with FencedCode.
        /// </summary>
        [Test]
        public void TestFencedCode()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestFencedCode.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(8));
            CheckParagraph(paras[0], "Lorem ipsum\r", "FencedCode");
            CheckParagraph(paras[1], "dolor\r", "FencedCode.C#");
            CheckParagraph(paras[2], "sit\vamet,\r", "FencedCode.C++");
            CheckParagraph(paras[3], "adipiscing elit,\r", "Heading 3");
            CheckParagraph(paras[4], "# eiusmod\r", "FencedCode");
            CheckParagraph(paras[5], "   incididunt\r", "Quote", "FencedCode.Java");
            CheckParagraph(paras[6], "> ut labore\v> et dlr\r", "FencedCode");
            CheckParagraph(paras[7], "quis\f", "FencedCode");
        }

        /// <summary>
        /// Tests reading a markdown document with InlineCode.
        /// </summary>
        [Test]
        public void TestInlineCode()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestInlineCode.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paras[0], "``\v``` foo bar\r", "Normal");
            CheckParagraph(paras[1], "baz bop\r", "Heading 1");
            CheckParagraph(paras[2], "loremipsum dolor sitamet conse```ct`etur* adi*piscing elit, sed do\r", "Quote");
            CheckParagraph(paras[3], "eiusmod\f", "SetextHeading1", "Heading 1");
            Assert.That(paras.Count, Is.EqualTo(4));

            Paragraph para = paras[0];
            CheckRun(para.Runs[0], "``", "InlineCode");
            CheckRun(para.Runs[1], "\v``` ", "Default Paragraph Font");
            CheckRun(para.Runs[2], "foo ", "Default Paragraph Font");
            Assert.That(para.Runs[2].RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            CheckRun(para.Runs[3], "bar", "InlineCode");
            Assert.That(para.Runs[3].RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            para = paras[1];
            CheckRun(para.Runs[0], "baz", "InlineCode");
            CheckRun(para.Runs[1], " bop", "Default Paragraph Font");

            para = paras[2];
            CheckRun(para.Runs[0], "lorem", "InlineCode");
            CheckRun(para.Runs[1], "ipsum dolor ", "Default Paragraph Font");
            CheckRun(para.Runs[2], "sit", "InlineCode");
            CheckRun(para.Runs[3], "a", "Default Paragraph Font");
            CheckRun(para.Runs[4], "met ", "Default Paragraph Font");
            Assert.That(para.Runs[4].RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            CheckRun(para.Runs[5], "conse```ct`etur* adi*piscing", "InlineCode.2");
            CheckRun(para.Runs[6], " el", "Default Paragraph Font");
            Assert.That(para.Runs[6].RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            CheckRun(para.Runs[7], "it, se", "Default Paragraph Font");
            Assert.That(para.Runs[7].RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            CheckRun(para.Runs[8], "d do", "InlineCode.4");

            para = paras[3];
            CheckRun(para.Runs[0], "e", "Default Paragraph Font");
            CheckRun(para.Runs[1], "iusmo", "InlineCode");
            CheckRun(para.Runs[2], "d", "Default Paragraph Font");
        }

        /// <summary>
        /// Tests reading a markdown document with strikethrough.
        /// </summary>
        [Test]
        public void TestStrikethrough()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportMarkdown\TestStrikethrough", UnifiedScenario.Md2Docx);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Run run = GetRunWithText(paragraph, "Foo ~");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "boz");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "~~ bop~~~");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));

            run = GetRunWithText(paragraph, "ipsum");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "set");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "~~a");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));

            run = GetRunWithText(paragraph, "di~~piscin");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
        }

        /// <summary>
        /// Tests reading a markdown document with underline.
        /// </summary>
        [Test]
        public void TestUnderline()
        {
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { ImportUnderlineFormatting = true };
            Document doc = TestUtil.Open(@"ImportMarkdown\TestUnderline.md", loadOptions);
            doc = TestUtil.SaveOpen(doc, @"ImportMarkdown\TestUnderline.docx");
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Run run = GetRunWithText(paragraph, "Foo +");
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));

            run = GetRunWithText(paragraph, "boz");
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "++ bop+++");
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.None));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));

            run = GetRunWithText(paragraph, "ipsum");
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "set");
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "++a");
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.None));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));

            run = GetRunWithText(paragraph, "di++piscin");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
        }

        /// <summary>
        /// Tests reading a markdown document with simple bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListA.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Normal");
            CheckListItem(paras[1], "b\r", 1, "o", "Normal");
            CheckListItem(paras[2], "c\r", 1, "o", "Normal");
            CheckListItem(paras[3], "d\r", 1, "o", "Normal");
            CheckListItem(paras[4], "e\r", 1, "o", "Normal");
            CheckListItem(paras[5], "f\f", 2, "", "Normal");

            Assert.That(paras.Count, Is.EqualTo(6));
        }

        /// <summary>
        /// Tests reading a markdown document with nested in single line bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListB.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 1, "o", "List", "Normal");
            CheckListItem(paras[1], "b\r", 0, "-", "Normal");
            CheckListItem(paras[2], "c\r", 1, "o", "Normal");
            CheckListItem(paras[3], "d\r", 2, "", "List2", "List1", "Normal");
            CheckListItem(paras[4], "e\f", 2, "", "List3", "List", "Normal");

            Assert.That(paras.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests reading a markdown document with nested in single line bullet lists and quotes.
        /// </summary>
        [Test]
        public void TestBulletListC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListC.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Quote", "Normal");
            CheckListItem(paras[1], "b\r", 0, " ", "List", "Quote", "Normal");
            CheckListItem(paras[2], "c\r", 2, "", "Quote2", "List1", "Quote1", "List", "Quote", "Normal");
            CheckListItem(paras[3], "d\r", 5, "", "Quote5", "List6", "List5", "Quote4", "List4", "List3", "Quote3", "List2", "Normal");
            CheckListItem(paras[4], "e\f", 5, " ", "List11", "List10", "Quote7", "List9", "List8", "Quote6", "List7", "List", "Quote", "Normal");

            CheckStyleList(doc.Styles["List"], 0, 1, '-');
            CheckStyleList(doc.Styles["List11"], 0, 1, '-');

            Assert.That(paras.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests reading a markdown document with headings inside bullet lists and quotes.
        /// </summary>
        [Test]
        public void TestBulletListD()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListD.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Quote", "Normal");
            CheckListItem(paras[1], "b\r", 0, "-", "Quote1", "Heading 1", "Normal");
            CheckListItem(paras[2], "c\f", 0, "-", "Quote2", "SetextHeading2", "Heading 2", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with headings inside simple bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListE()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListE.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Heading 1", "Normal");
            CheckListItem(paras[1], "b\r", 1, "o", "Normal");
            CheckListItem(paras[2], "c\r", 1, "o", "Heading 2", "Normal");
            CheckListItem(paras[3], "d\f", 2, "", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading a very complex markdown document with bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListF()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListF.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "Lorem\r", 0, "-", "Normal");
            CheckListItem(paras[1], "ipsum\r", 0, "+", "Normal");
            CheckListItem(paras[2], "dolor\r", 0, "*", "Normal");
            CheckListItem(paras[3], "sit\r", 1, "o", "Normal");
            CheckListItem(paras[4], "amet\r", 1, "o", "Normal");
            CheckListItem(paras[5], "consectetur\r", 2, "", "Normal");
            CheckListItem(paras[6], "adipiscing elit\r", 3, "", "Normal");
            CheckListItem(paras[7], "sed\r", 3, " ", "Heading 1", "Normal");
            CheckListItem(paras[8], "do\r", 0, " ", "SetextHeading2", "Heading 2", "Normal");
            CheckHorizontalRule(paras[9], "Normal");
            CheckListItem(paras[9], "\r", 0, " ", "Normal");
            CheckListItem(paras[10], "tabs indentation\veiusmod\r", 0, " ", "IndentedCode");
            CheckListItem(paras[11], "tempor\r", 1, "o", "Quote", "Normal");
            CheckListItem(paras[12], "incididunt\r", 1, "o", "Quote2", "Quote1", "Quote", "Normal");
            CheckListItem(paras[13], "ut\r", 1, "o", "Quote1", "Quote", "Normal");
            CheckListItem(paras[14], "labore ** et\r", 0, "*", "Normal");
            CheckParagraph(paras[15], "-dolore\r", "Normal");
            CheckHorizontalRule(paras[16], "Quote", "Normal");
            CheckListItem(paras[16], "\r", 0, "+", "Quote");
            CheckListItem(paras[17], "aliqua.\r", 0, " ", "Quote");
            CheckListItem(paras[18], "Ut - enim\r", 1, "o", "Quote");
            CheckParagraph(paras[19], "+\tad\r", "IndentedCode");
            CheckListItem(paras[20], "minim\r", 0, "-", "Normal");
            CheckListItem(paras[21], "veniam\r", 2, "", "Quote4", "List1", "Quote3", "List", "Quote", "Normal");
            CheckListItem(paras[22], "quis\r", 0, "-", "Normal");
            CheckListItem(paras[23], "nostrud\r", 0, " ", "Normal");
            CheckListItem(paras[24], "exercitation -         foo\f", 1, "o", "Normal");

            Assert.That(paras.Count, Is.EqualTo(25));
        }

        /// <summary>
        /// Tests reading a markdown document with bullet lists combined with quotes.
        /// </summary>
        [Test]
        public void TestBulletListG()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListG.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Normal");
            CheckListItem(paras[1], "b\r", 1, "o", "Quote", "Normal");
            CheckListItem(paras[2], "c\f", 1, "o", "List", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with bullet lists combined with quotes.
        /// </summary>
        [Test]
        public void TestBulletListH()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListH.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Normal");
            CheckListItem(paras[1], "b\r", 0, "-", "Normal");
            CheckListItem(paras[2], "c\r", 1, "o", "Quote", "Normal");
            CheckListItem(paras[3], "d\r", 1, "o", "List", "Quote", "Normal");
            CheckListItem(paras[4], "e\r", 0, "-", "Quote", "Normal");
            CheckListItem(paras[5], "f\r", 1, "o", "Quote", "Normal");
            CheckListItem(paras[6], "g\f", 1, "o", "Quote1", "List1", "Normal");

            Assert.That(paras.Count, Is.EqualTo(7));
        }

        /// <summary>
        /// Tests reading a Markdown document with bullet lists combined with quotes.
        /// </summary>
        [Test]
        public void TestBulletListI()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListI.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[1], "A list item with a blockquote:\r", 0, "*", "Normal");
            CheckListItem(paras[2], "This is a blockquote inside a list item.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with bullet lists and indented code inside.
        /// </summary>
        [Test]
        public void TestBulletListJ()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListJ.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckParagraph(paras[0], "To put a code block within a list item\r", "Normal");
            CheckListItem(paras[1], "A list item with a code block:\r", 0, "*", "Normal");
            CheckListItem(paras[2], "<code goes here>\f", 0, " ", "IndentedCode");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with bullet lists and horizontal rule.
        /// </summary>
        [Test]
        public void TestBulletListK()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListK.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckHorizontalRule(paras[0], "Quote", "Normal");
            CheckListItem(paras[0], "\r", 0, "+", "Quote", "Normal");
            CheckListItem(paras[1], "aliqua.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests reading a markdown document with bullet lists, horizontal rule and indented code.
        /// </summary>
        [Test]
        public void TestBulletListL()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestBulletListL.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "dolor\r", 0, "*", "Normal");
            CheckListItem(paras[1], "sit\r", 1, "o", "Normal");

            CheckHorizontalRule(paras[2], "Normal");
            CheckListItem(paras[2], "\r", 0, " ", "Normal");

            CheckListItem(paras[3], "tabs indentation\f", 0, " ", "IndentedCode");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading markdown document with simple ordered lists with '.' as marker delimiter.
        /// </summary>
        [Test]
        public void TestOrderedListA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListA.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal" );
            CheckListItem(paras[1], "b\r", 1, "1.", "Normal" );
            CheckListItem(paras[2], "c\r", 1, "2.", "Normal" );
            CheckListItem(paras[3], "d\r", 0, "2.", "Normal" );
            CheckListItem(paras[4], "e\r", 1, "1.", "Normal" );
            CheckListItem(paras[5], "f\r", 2, "1.", "Normal" );
            CheckListItem(paras[6], "g\r", 3, "1.", "Normal" );
            CheckListItem(paras[7], "h\r", 2, "2.", "Normal" );
            CheckListItem(paras[8], "i\r", 1, "2.", "Normal" );
            CheckListItem(paras[9], "j\f", 0, "3.", "Normal" );

            Assert.That(paras.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests reading markdown document with simple ordered lists with ')' as marker delimiter.
        /// </summary>
        [Test]
        public void TestOrderedListB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListB.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1)", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1)", "Normal");
            CheckListItem(paras[2], "c\r", 1, "2)", "Normal");
            CheckListItem(paras[3], "d\r", 0, "2)", "Normal");
            CheckListItem(paras[4], "e\r", 1, "1)", "Normal");
            CheckListItem(paras[5], "f\r", 2, "1)", "Normal");
            CheckListItem(paras[6], "g\r", 3, "1)", "Normal");
            CheckListItem(paras[7], "h\r", 2, "2)", "Normal");
            CheckListItem(paras[8], "i\r", 1, "2)", "Normal");
            CheckListItem(paras[9], "j\f", 0, "3)", "Normal");

            Assert.That(paras.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests 'StartAt' is set properly upon reading ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListC.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1)", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "1)", "Normal");
            CheckListItem(paras[3], "d\f", 0, "1.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests 'StartAt' is set properly upon reading ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListD()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListD.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1)", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "5)", "Normal");
            CheckListItem(paras[3], "d\f", 0, "8.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests 'StartAt' is set properly upon reading ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListE()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListE.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "5.", "Normal");
            CheckListItem(paras[1], "b\r", 1, "3.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "100)", "Normal");
            CheckListItem(paras[3], "d\f", 0, "6.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading markdown document with ordered lists combined with bullet lists.
        /// </summary>
        [Test]
        public void TestOrderedListF()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListF.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckListItem(paras[1], "b\r", 1, "o", "Normal");
            CheckListItem(paras[2], "c\r", 1, "5.", "Normal");
            CheckListItem(paras[3], "d\r", 2, "", "Normal");
            CheckListItem(paras[4], "e\r", 1, "6.", "Normal");
            CheckListItem(paras[5], "f\f", 0, "2.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(6));
        }

        /// <summary>
        /// Tests reading markdown document with nested in a single line ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListG()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListG.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 1, "6.", "List", "Normal");
            CheckListItem(paras[1], "b\r", 0, "10.", "Normal");
            CheckListItem(paras[2], "c\r", 2, "5.", "List1", "Normal");
            CheckListItem(paras[3], "d\f", 2, "3.", "List3", "List2", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading markdown document with nested in a single line ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListH()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListH.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "3.", "Quote", "Normal");
            CheckListItem(paras[1], "b\r", 0, " ", "List", "Quote", "Normal");
            CheckListItem(paras[2], "c\r", 2, "7.", "Quote2", "List2", "Quote1", "List1", "Quote", "Normal");
            CheckListItem(paras[3], "d\r", 5, "44.", "Quote5", "List7", "List6", "Quote4", "List5", "List4", "Quote3", "List3", "Normal");
            CheckListItem(paras[4], "e\f", 5, " ", "List13", "List12", "Quote7", "List11", "List10", "Quote6", "List9", "List8", "Quote", "Normal");

            CheckStyleList(doc.Styles["List"], 0, 2, '.');
            CheckStyleList(doc.Styles["List5"], 2, 45, '.');
            CheckStyleList(doc.Styles["List13"], 0, 1, '.');

            Assert.That(paras.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests reading markdown document with ordered lists combined with headings.
        /// </summary>
        [Test]
        public void TestOrderedListI()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListI.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "7.", "Heading 1", "Normal");
            CheckListItem(paras[1], "b\r", 1, "2.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "3.", "Heading 2", "Normal");
            CheckListItem(paras[3], "d\f", 2, "2.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading markdown document with ordered lists nested in Quotes.
        /// </summary>
        [Test]
        public void TestOrderedListJ()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListJ.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Quote", "Normal");
            CheckListItem(paras[2], "c\f", 1, "1.", "List", "Quote", "Normal");

            CheckStyleList(doc.Styles["List"], 0, 1, '.');

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading markdown document with ordered lists nested in Quotes and vise versa.
        /// </summary>
        [Test]
        public void TestOrderedListK()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListK.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckListItem(paras[1], "b\r", 0, "2.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "1.", "Quote", "Normal");
            CheckListItem(paras[3], "d\r", 1, "1.", "List", "Quote", "Normal");
            CheckListItem(paras[4], "e\r", 0, "1.", "Quote", "Normal");
            CheckListItem(paras[5], "f\r", 1, "1.", "Quote", "Normal");
            CheckListItem(paras[6], "g\r", 1, "2.", "Quote", "Normal");
            CheckListItem(paras[7], "h\f", 1, "1.", "Quote1", "List1", "Normal");

            CheckStyleList(doc.Styles["List"], 0, 1, '.');
            CheckStyleList(doc.Styles["List1"], 0, 1, '.');

            Assert.That(paras.Count, Is.EqualTo(8));
        }

        /// <summary>
        /// Tests reading markdown document with Quote inside ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListL()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListL.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[1], "A list item with a blockquote:\r", 0, "5.", "Normal");
            CheckListItem(paras[2], "This is a blockquote inside a list item.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with ordered lists and indented code inside.
        /// </summary>
        [Test]
        public void TestOrderedListM()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListM.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckParagraph(paras[0], "To put a code block within a list item\r", "Normal");
            CheckListItem(paras[1], "A list item with a code block:\r", 0, "7.", "Normal");
            CheckListItem(paras[2], "<code goes here>\f", 0, " ", "IndentedCode");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading a markdown document with ordered lists and horizontal rule.
        /// </summary>
        [Test]
        public void TestOrderedListN()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListN.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckHorizontalRule(paras[0], "Quote", "Normal");
            CheckListItem(paras[0], "\r", 0, "4.", "Quote", "Normal");
            CheckListItem(paras[1], "aliqua.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests reading a very complex markdown document with ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListO()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListO.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "Lorem\r", 0, "2.", "Normal");
            CheckListItem(paras[1], "ipsum\r", 0, "4)", "Normal");
            CheckListItem(paras[2], "dolor\r", 0, "6.", "Normal");
            CheckListItem(paras[3], "sit\r", 1, "1.", "Normal");
            CheckListItem(paras[4], "amet\r", 1, "3)", "Normal");
            CheckListItem(paras[5], "consectetur\r", 2, "", "Normal");
            CheckListItem(paras[6], "adipiscing elit\r", 2, "25.", "Normal");
            CheckListItem(paras[7], "sed\r", 2, " ", "Heading 1", "Normal");
            CheckListItem(paras[8], "do\r", 0, " ", "SetextHeading2", "Heading 2", "Normal");
            CheckHorizontalRule(paras[9], "Normal");
            CheckListItem(paras[9], "\r", 0, " ", "Normal");
            CheckListItem(paras[10], "tabs indentation\veiusmod\r", 0, " ", "IndentedCode");
            CheckListItem(paras[11], "tempor\r", 1, "1.", "Quote", "Normal");
            CheckListItem(paras[12], "incididunt\r", 1, "1)", "Quote2", "Quote1", "Quote", "Normal");
            CheckListItem(paras[13], "ut\r", 1, "1)", "Quote1", "Quote", "Normal");
            CheckListItem(paras[14], "labore 1.et\r", 0, "7.", "Normal");
            CheckParagraph(paras[15], "1.dolore\r", "Normal");
            CheckHorizontalRule(paras[16], "Quote", "Normal");
            CheckListItem(paras[16], "\r", 0, "1.", "Quote");
            CheckListItem(paras[17], "aliqua.\r", 0, " ", "Quote");
            CheckListItem(paras[18], "Ut 1. enim\r", 1, "1.", "Quote");
            CheckParagraph(paras[19], "1.\tad\r", "IndentedCode");
            CheckListItem(paras[20], "minim\r", 0, "1.", "Normal");
            CheckListItem(paras[21], "veniam\r", 2, "1.", "Quote4", "List1", "Quote3", "List", "Quote", "Normal");
            CheckListItem(paras[22], "quis\r", 0, "1.", "Normal");
            CheckListItem(paras[23], "nostrud\r", 0, " ", "Normal");
            CheckListItem(paras[24], "exercitation 1.         foo\f", 1, "1.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(25));
        }

        /// <summary>
        /// Tests that new list is started when it was broken with a regular paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListP()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListP.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckParagraph(paras[1], "b\r", "Normal");
            CheckListItem(paras[2], "c\f", 0, "5.", "Normal");

            Assert.That(paras[0].ParaPr.ListId, Is.EqualTo(1));
            Assert.That(paras[2].ParaPr.ListId, Is.EqualTo(2));

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that paragraphs under the same list block don't break this list.
        /// </summary>
        [Test]
        public void TestOrderedListQ()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestOrderedListQ.md");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckListItem(paras[1], "b\r", 0, " ", "Normal");
            CheckListItem(paras[2], "c\f", 0, "2.", "Normal");

            Assert.That(paras[0].ParaPr.ListId, Is.EqualTo(1));
            Assert.That(paras[2].ParaPr.ListId, Is.EqualTo(1));

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading markdown document with table.
        /// </summary>
        [Test]
        public void TestTableA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestTableA.md");
            TableCollection tables = doc.FirstSection.Body.Tables;

            RowCollection rows = tables[0].Rows;
            Assert.That(rows.Count, Is.EqualTo(9));

            Row row = rows[0];
            CheckRow(row, new string[] {"NoAlign\a", "Center\a", "Left\a", "Right\a"});
            Assert.That(row.Cells[0].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(row.Cells[1].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(row.Cells[2].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Left));
            Assert.That(row.Cells[3].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Right));

            row = rows[1];
            CheckRow(row, new string[] {"First cell\a", "Second *cell\a", "Third cell*\a", "Fourth cell\a"});
            Run run = GetRunWithText(row.Cells[0].FirstParagraph, "First");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            run = GetRunWithText(row.Cells[1].FirstParagraph, "Second");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            run = GetRunWithText(row.Cells[2].FirstParagraph, "Third");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            run = GetRunWithText(row.Cells[3].FirstParagraph, "Fourth");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo(MarkdownUtil.InlineCodeStyleName));

            CheckRow(rows[2], new string[] {"Only first column\a", "\a", "\a", "\a"});
            CheckRow(rows[3], new string[] {"\a", "Only second column\a", "\a", "\a"});
            CheckRow(rows[4], new string[] {"The following are two completely empty rows:\a", "\a", "\a", "\a"});
            CheckRow(rows[5], new string[] {"\a", "\a", "\a", "\a"});
            CheckRow(rows[6], new string[] {"\a", "\a", "\a", "\a"});
            CheckRow(rows[7], new string[] {"1\a", "2\a", "3\a", "4\a"});

            row = rows[8];
            CheckRow(row, new string[] {"last row\a", "\a", "\a", "\a"});
            Assert.That(row.Cells[0].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(row.Cells[1].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(row.Cells[2].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Left));
            Assert.That(row.Cells[3].FirstParagraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Right));

            Paragraph paragraph = (Paragraph)tables[0].NextSibling;
            CheckParagraph(paragraph, "paragraph after table", "Normal");

            Assert.That(tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests reading markdown document with table inside a Quote.
        /// </summary>
        /// <remarks>
        /// For a moment this is not implemented and tables in Quotes are read into model without Quote style.
        /// </remarks>
        [Test]
        public void TestTableB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestTableB.md");

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            CheckParagraph(paragraph, "Foo", "Normal");

            Table table = (Table)paragraph.NextSibling;

            Assert.That(table.Rows.Count, Is.EqualTo(1));
            CheckRow(table.Rows[0], new string[] {"bar\a"});

            paragraph = (Paragraph)table.NextSibling;
            CheckParagraph(paragraph, "baz bop", "Quote", "Normal");
        }

        /// <summary>
        /// Tests sibling tables.
        /// </summary>
        [Test]
        public void TestTableC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\TestTableC.md");

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            CheckParagraph(paragraph, "Foo", "Normal");

            paragraph = doc.FirstSection.Body.LastParagraph;
            CheckParagraph(paragraph, "Bar", "Normal");

            TableCollection tables = doc.FirstSection.Body.Tables;

            Table table = tables[0];
            Assert.That(table.Rows.Count, Is.EqualTo(2));
            CheckRow(table.Rows[0], new string[] {"1\a", "2\a"});
            CheckRow(table.Rows[1], new string[] {"3\a", "4\a"});

            table = tables[1];
            Assert.That(table.Rows.Count, Is.EqualTo(2));
            CheckRow(table.Rows[0], new string[] {"1\a"});
            CheckRow(table.Rows[1], new string[] {"2\a"});

            table = tables[2];
            Assert.That(table.Rows.Count, Is.EqualTo(3));
            CheckRow(table.Rows[0], new string[] {"1\a", "2\a", "3\a"});
            CheckRow(table.Rows[1], new string[] {"2\a", "2\a", "\a"});
            CheckRow(table.Rows[2], new string[] {"3\a", "\a", "3\a"});

            Assert.That(tables.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests simple Autolink.
        /// </summary>
        [Test]
        public void TestAutolinkA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests simple Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkB.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(6));

            CheckParagraph(paras[0],
                "text \u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015 text\r", "Normal");
            CheckParagraph(paras[1],
                "text \u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015\r", "Normal");
            CheckParagraph(paras[2],
                "\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015 text\r", "Normal");
            CheckParagraph(paras[3],
                "text\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015text\r", "Normal");
            CheckParagraph(paras[4],
                "text\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015\r", "Normal");
            CheckParagraph(paras[5],
                "\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015text\f", "Normal");
        }

        /// <summary>
        /// Tests simple Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkC.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0],
                "text \u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015 text " +
                "\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015 text\r", "Normal");
            CheckParagraph(paras[1],
                "text \u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015" +
                "\u0013 HYPERLINK \"http://example.com\" \u0014http://example.com\u0015 text\f", "Normal");
        }

        /// <summary>
        /// Tests Autolinks with Whitespace characters.
        /// </summary>
        [Test]
        public void TestAutolinkD()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkD.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0], "<http://example .com>\r", "Normal");
            CheckParagraph(paras[1], "<http://example.com>\r", "Normal");
            CheckParagraph(paras[2], "<http://example\t\t.com>\f", "Normal");
        }


        /// <summary>
        /// Tests Autolink without colon and commercial "At".
        /// </summary>
        [Test]
        public void TestAutolinkF()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkF.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "<http//example.com>\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with colon and backslash not in the schema name.
        /// </summary>
        [Test]
        public void TestAutolinkG()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkG.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"http://examp\\\\le.com\" \u0014http://examp\\le.com\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with commercial "At" and backslash.
        /// </summary>
        [Test]
        public void TestAutolinkH()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkH.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "<email@examp\\le.com>\r", "Normal");
            CheckParagraph(paras[1], "<ema\\il@example.com>\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with incorrect length.
        /// </summary>
        [Test]
        public void TestAutolinkI()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkI.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(5));

            CheckParagraph(paras[0], "<://example.com>\r", "Normal");
            CheckParagraph(paras[1], "<h:example.com>\r", "Normal");
            CheckParagraph(paras[2],
                "\u0013 HYPERLINK \"ht://example.com\" \u0014ht://example.com\u0015\r", "Normal");
            CheckParagraph(paras[3],
                "\u0013 HYPERLINK \"http5678901234567890123456789012://example.com\" " +
                "\u0014http5678901234567890123456789012://example.com\u0015\r", "Normal");
            CheckParagraph(paras[4], "<http56789012345678901234567890123:example.com>\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with wrong first symbol (must be [a-zA-Z]).
        /// </summary>
        [Test]
        public void TestAutolinkJ()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkJ.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "<1tr://example.com>\r", "Normal");
            CheckParagraph(paras[1],
                "\u0013 HYPERLINK \"ammdftfsg://example.com\" \u0014ammdftfsg://example.com\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with wrong not-first symbol (must be [a-zA-Z0-9.+-]).
        /// </summary>
        [Test]
        public void TestAutolinkK()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkK.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"h.t0.+-89-r://example.com\" \u0014h.t0.+-89-r://example.com\u0015\r", "Normal");
            CheckParagraph(paras[1], "<a*mfsg:example.com>\f", "Normal");
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
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkL.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(10));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"mailto:email@example.com\" \u0014email@example.com\u0015\r", "Normal");
            CheckParagraph(paras[1], "<e@mail@example.com>\r", "Normal");
            CheckParagraph(paras[2], "<email@e@xample.com>\r", "Normal");
            CheckParagraph(paras[3],
                "\u0013 HYPERLINK \"mailto:eM12.!\" \\l \"$%&'*+/=?^_`{|}~-@example.com\" " +
                "\u0014eM12.!#$%&'*+/=?^_`{|}~-@example.com\u0015\r", "Normal");
            CheckParagraph(paras[4],
                "\u0013 HYPERLINK \"mailto:email@e-xample.com\" \u0014email@e-xample.com\u0015\r", "Normal");
            CheckParagraph(paras[5], "<email@-example.com>\r", "Normal");
            CheckParagraph(paras[6], "<email@example-.com>\r", "Normal");
            CheckParagraph(paras[7],
                "\u0013 HYPERLINK \"mailto:email@example.c-om\" \u0014email@example.c-om\u0015\r", "Normal");
            CheckParagraph(paras[8], "<email@example.-com>\r", "Normal");
            CheckParagraph(paras[9], "<email@example.com->\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with colon and any unexpected symbol after the schema name.
        /// </summary>
        [Test]
        public void TestAutolinkM()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkM.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"http://examp@$%5^&\" \\l \"@$56&^%1!\" " +
                "\u0014http://examp@$%5^&#@$56&^%1!\u0015\f", "Normal");
        }




        /// <summary>
        /// Tests non-Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkR()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkR.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0], "<>\r", "Normal");
            CheckParagraph(paras[1], "e-mail@gmail.com\r", "Normal");
            CheckParagraph(paras[2], "http://example.com\f", "Normal");
        }

        /// <summary>
        /// Tests Autolink with bookmark.
        /// </summary>
        [Test]
        public void TestAutolinkS()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkS.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"https://spec.commonmark.org/0.28/\" \\l \"autolinks\" " +
                "\u0014https://spec.commonmark.org/0.28/#autolinks\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that code spans and Autolinks have the same precedence.
        /// </summary>
        [Test]
        public void TestAutolinkU()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkU.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(4));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"http://foo.bar.`baz\" \u0014http://foo.bar.`baz\u0015`\r", "Normal");
            CheckParagraph(paras[1], "<http://foo.bar.baz>\r", "Normal");
            CheckParagraph(paras[2],
                "\u0013 HYPERLINK \"http://foo.`bar.`baz\" \u0014http://foo.`bar.`baz\u0015\r", "Normal");
            CheckParagraph(paras[3], "<http://foo.bar.baz>\f", "Normal");
        }

        /// <summary>
        /// Tests Autolinks with closer angle brackets.
        /// </summary>
        [Test]
        public void TestAutolinkV()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkV.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"http://foo.bar.baz\" \u0014http://foo.bar.baz\u0015>\r", "Normal");
            CheckParagraph(paras[1],
                "<\u0013 HYPERLINK \"http://foo.bar.baz\" \u0014http://foo.bar.baz\u0015\r", "Normal");
            CheckParagraph(paras[2],
                "<\u0013 HYPERLINK \"http://foo.bar.baz\" \u0014http://foo.bar.baz\u0015>\f", "Normal");
        }

        /// <summary>
        /// Tests that backslash-escaped left angle bracket is not valid for Autolink.
        /// </summary>
        [Test]
        public void TestAutolinkW()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Autolink\TestAutolinkW.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "<http://uri>\f", "Normal");
        }

        /// <summary>
        /// Tests simple Link.
        /// </summary>
        [Test]
        public void TestLinkA()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"http://example.com\" \u0014text\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests simple Link with a title.
        /// </summary>
        [Test]
        public void TestLinkB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkB.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \\o \"title\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1],
                "\u0013 HYPERLINK \"http://example.com\" \\o \"title\" \u0014text\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests empty link text for Link.
        /// </summary>
        /// <remarks>
        /// TODO: Spec allows 'linkText' with zero length, but for now it is not implemented.
        /// </remarks>
        [Test]
        public void TestLinkC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkC.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"http://example.com\" \u0014\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests empty link URI for Link.
        /// </summary>
        /// <remarks>
        /// TODO: Spec allows 'linkUri' with zero length, but for now it is not implemented.
        /// </remarks>
        [Test]
        public void TestLinkD()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkD.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \u0014text\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests Link titles with different quotes.
        /// </summary>
        [Test]
        public void TestLinkE()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkE.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"http://example.com\" \\o \"title\" \u0014text\u0015\r", "Normal");
            CheckParagraph(paras[1],
                "\u0013 HYPERLINK \"http://example.com\" \\o \"title\" \u0014text\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests Link with angle brackets for link URI.
        /// </summary>
        [Test]
        public void TestLinkF()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkF.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"http://example.com\" \u0014text\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests Link titles with backslash-escaped quotes.
        /// </summary>
        [Test]
        public void TestLinkG()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkG.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(13));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti\\\"tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti'tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[2], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti)tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[3], "[text](http://example.com \"ti\"tle\")\r", "Normal");
            CheckParagraph(paras[4], "[text](http://example.com 'ti'tle')\r", "Normal");
            CheckParagraph(paras[5], "[text](http://example.com (ti)tle))\r", "Normal");
            CheckParagraph(paras[6], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti'tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[7], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti\\\"tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[8], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti\\\"tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[9], "\u0013 HYPERLINK \"http://example.com\" \\o \"ti)tle\" \u0014text\u0015\r",
                "Normal");
            CheckParagraph(paras[10], "[link](/url \"title \"and\" title\")\r", "Normal");
            CheckParagraph(paras[11], "\u0013 HYPERLINK \"/url\" \\o \"title \\\"and\\\" title\" \u0014link\u0015\r",
                "Normal");
            CheckParagraph(paras[12], "[text](http://example.com (ti(tle))\f",
                "Normal");
        }

        /// <summary>
        /// Tests that the Link destination cannot contain spaces or line breaks, except spaces enclosed in pointy brackets.
        /// </summary>
        [Test]
        public void TestLinkH()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkH.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(4));

            CheckParagraph(paras[0], "[link](/my uri)\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"/my%20uri\" \u0014link\u0015", "Normal");
            CheckParagraph(paras[2], "[link](foo bar)\r", "Normal");
            CheckParagraph(paras[3], "[link](<foo bar>)\f", "Normal");
        }

        /// <summary>
        /// Tests that parentheses inside the Link destination may be escaped.
        /// </summary>
        [Test]
        public void TestLinkI()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkI.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"(foo)\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that any number of parentheses in the Link target are allowed without escaping,
        /// as long as they are balanced.
        /// </summary>
        [Test]
        public void TestLinkJ()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkJ.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"foo(and(bar))\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1], "[link](foo(and(bar))\f", "Normal");
        }

        /// <summary>
        /// Tests that if Link have unbalanced parentheses,
        /// they need to be escaped or used with the angle brackets.
        /// </summary>
        [Test]
        public void TestLinkK()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkK.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"foo(and(bar)\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"foo(and(bar)\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that parentheses and other symbols in the Link can also be escaped, as usual in Markdown.
        /// </summary>
        [Test]
        public void TestLinkL()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkL.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"foo):\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1],
                "\u0013 HYPERLINK \"'.+-=*_%60~:!\" \\l \"$%25&@/?%5E%7C;\\\\%22%3C%3E%5B%5D()%7B%7D\" \u0014link\u0015\f",
                "Normal");
        }

        /// <summary>
        /// Tests that Links can contain fragment identifiers and queries.
        /// </summary>
        [Test]
        public void TestLinkM()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkM.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \\l \"fragment\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"http://example.com\" \\l \"fragment\" \u0014link\u0015\r",
                "Normal");
            CheckParagraph(paras[2],
                "\u0013 HYPERLINK \"http://example.com?foo=3\" \\l \"frag\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that a backslash before a non-escapable character in the Link is just a backslash.
        /// </summary>
        [Test]
        public void TestLinkN()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkN.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"foo\\\\bar\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"foobar\" \u0014li\\nk\u0015\r", "Normal");
            CheckParagraph(paras[2], "\u0013 HYPERLINK \"foo\" \\o \"b\\\\ar\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that URL-escaping should be left alone inside the Link destination,
        /// as all URL-escaped characters are also valid URL characters.
        /// </summary>
        [Test]
        public void TestLinkO()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkO.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"foo%20b&auml;\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that, because titles can often be parsed as Link destinations,
        /// if you try to omit the destination and keep the title, you’ll get unexpected results.
        /// </summary>
        [Test]
        public void TestLinkP()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkP.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"%22title%22\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that backslash escapes and entity and numeric character references may be used in Link titles.
        /// </summary>
        [Test]
        public void TestLinkQ()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkQ.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "\u0013 HYPERLINK \"/url\" \\o \"title \\\"&quot;\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that whitespace is allowed around the Link destination and title.
        /// </summary>
        [Test]
        public void TestLinkR()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkR.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \\o \"title\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that whitespace is not allowed between the Link text and the following parenthesis.
        /// </summary>
        [Test]
        public void TestLinkS()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkS.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "[link] (/uri)\f", "Normal");
        }

        /// <summary>
        /// Tests that the Link text may contain balanced brackets, but not unbalanced ones, unless they are escaped.
        /// </summary>
        [Test]
        public void TestLinkT()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkT.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(4));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \u0014link [foo [bar]]\u0015\r", "Normal");
            CheckParagraph(paras[1], "[link] bar](/uri)\r", "Normal");
            CheckParagraph(paras[2], "[link \u0013 HYPERLINK \"/uri\" \u0014bar\u0015\r", "Normal");
            CheckParagraph(paras[3], "\u0013 HYPERLINK \"/uri\" \u0014link [bar\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that the Link text may contain inline content.
        /// </summary>
        [Test]
        public void TestLinkU()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkU.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \u0014link foo bar #\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"/uri\" \u0014\u0015\f", "Normal");
            Shape shape = (Shape)paras[1].GetChild(NodeType.Shape, 0, false);
            CheckShape(shape, "moon", "moon.jpg");
        }

        /// <summary>
        /// Tests that Links may not contain other Links, at any level of nesting.
        /// </summary>
        [Test]
        public void TestLinkV()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkV.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0], "[foo \u0013 HYPERLINK \"/uri\" \u0014bar\u0015](/uri)\r", "Normal");
            CheckParagraph(paras[1], "[foo [bar \u0013 HYPERLINK \"/uri\" \u0014baz\u0015](/uri)](/uri)\r", "Normal");
            CheckShape(paras[2].GetChildNodes(NodeType.Any, false)[0] as Shape, "[foo](uri2)", "uri3");
        }

        /// <summary>
        /// Tests the precedence of Link text grouping over emphasis grouping.
        /// </summary>
        [Test]
        public void TestLinkW()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkW.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "*\u0013 HYPERLINK \"/uri\" \u0014foo*\u0015\r", "Normal");
            CheckParagraph(paras[1], "\u0013 HYPERLINK \"baz*\" \u0014foo *bar\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that brackets that aren’t part of Links do not take precedence.
        /// </summary>
        [Test]
        public void TestLinkX()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkX.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "foo [bar baz]\f", "Normal");
        }


        /// <summary>
        /// Tests that a Link destination consists of a sequence of characters between angle brackets that contains
        /// no unescaped angle brackets.
        /// </summary>
        [Test]
        public void TestLinkZ01()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkZ01.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(8));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \u0014foo\u0015\r", "Normal");
            CheckParagraph(paras[1], "[foo](</uri>)\r", "Normal");
            CheckParagraph(paras[2], "\u0013 HYPERLINK \"%3C/uri%3E\" \u0014foo\u0015\r", "Normal");
            CheckParagraph(paras[3], "\u0013 HYPERLINK \"%3C/uri%3E\" \u0014foo\u0015\r",  "Normal");
            CheckParagraph(paras[4], "[foo](ri>)\r", "Normal");
            CheckParagraph(paras[5], "\u0013 HYPERLINK \"/u%3Eri\" \u0014foo\u0015\r", "Normal");
            CheckParagraph(paras[6], "[foo](</u<ri>)\r", "Normal");
            CheckParagraph(paras[7], "\u0013 HYPERLINK \"/u%3Eri\" \u0014foo\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that a Link titles may span multiple lines.
        /// </summary>
        [Test]
        public void TestLinkZ02()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkZ02.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \\o \"ti\vtle\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests an empty Link title.
        /// </summary>
        [Test]
        public void TestLinkZ03()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkZ03.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests edge conditions during the Link parsing.
        /// </summary>
        [Test]
        public void TestLinkZ04()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkZ04.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(6));

            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/uri\" \\o \"title\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[1], "[link](/uri \"title\" w)\r", "Normal");
            CheckParagraph(paras[2], "[link](/uri \"title\" ww)\r", "Normal");
            CheckParagraph(paras[3], "\u0013 HYPERLINK \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[4], "\u0013 HYPERLINK \"/uri\" \u0014link\u0015\r", "Normal");
            CheckParagraph(paras[5], "\u0013 HYPERLINK \"/uri\" \u0014link\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests that backslash-escaped left square bracket is not valid for Link.
        /// </summary>
        [Test]
        public void TestLinkZ05()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Link\TestLinkZ05.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "[foo](/uri)\f", "Normal");
        }


        /// <summary>
        /// Tests collapsed reference link import.
        /// </summary>
        [Test]
        public void TestReferenceLink_1()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceLinks\TestCRLinkA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckParagraph(paras[0], "This is a link \u0013 HYPERLINK \"http://example.com\" \\o \"title\" " +
                                     "\\s \"[here]\" \u0014here\u0015.\r", "Normal");
        }

        /// <summary>
        /// Tests full reference link import.
        /// </summary>
        [Test]
        public void TestReferenceLink_2()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceLinks\TestFRLinkB.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckParagraph(paras[0], "This is a link \u0013 HYPERLINK \"https://www.w3schools.com/\" \\o \"title\" \\s \"[ref]\" " +
                                     "\u0014text\u0015 and \u0013 HYPERLINK \"http://example.com\" \\o \"title1\" \\s \"[ref1]\" \u0014here\u0015 " +
                                     "and \u0013 HYPERLINK \"http://example.com\" \\o \"title1\" \\s \"[ref1]\" \u0014here1\u0015 and \u0013 " +
                                     "HYPERLINK \"http://example.com\" \\o \"title1\" \\s \"[ref1]\" \u0014here2\u0015.\r", "Normal");
        }

        /// <summary>
        /// Tests shortcut reference link import.
        /// </summary>
        [Test]
        public void TestReferenceLink_3()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceLinks\TestSRLinkA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckParagraph(paras[0], "This is a link \u0013 HYPERLINK \"http://example.com\" \\o \"title\" " +
                                     "\\s \"[here]\" \u0014here\u0015.\r", "Normal");
            }

        /// <summary>
        /// Tests importing the multiline reference link definition.
        /// </summary>
        [Test]
        public void TestReferenceLink_4()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceLinks\TestRDLinkJ.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));
            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/url\" \\o \"the title\" \\s \"[foo]\" \u0014foo\u0015\f", "Normal");
        }

        /// <summary>
        /// Tests importing the link definition with the URL in angle brackets.
        /// </summary>
        [Test]
        public void TestReferenceLink_5()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceLinks\TestRDLinkL.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));
            CheckParagraph(paras[0], "\u0013 HYPERLINK \"my%20url\" \\o \"title\" \\s \"[Foo bar]\" \u0014Foo bar\u0015\f",
                "Normal");
        }

        /// <summary>
        /// Tests importing the reference link definition with the multiline comment.
        /// </summary>
        [Test]
        public void TestReferenceLink_6()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceLinks\TestRDLinkM.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));
            CheckParagraph(paras[0], "\u0013 HYPERLINK \"/url\" \\o \"title\r\nline1\r\nline2\" \\s \"[foo]\" \u0014foo\u0015\f",
                "Normal");
        }


        /// <summary>
        /// Tests Image with a title.
        /// </summary>
        [Test]
        public void TestImageB()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Image\TestImageB.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckShape(paras[0].GetChild(NodeType.Any, 0, false) as Shape, "alt text", "/uri", "title");

            CheckParagraph(paras[1], "My \f", "Normal");
            CheckShape(paras[1].GetChild(NodeType.Any, 1, false) as Shape, "foo bar", "/path/to/train.jpg", "title");
        }

        /// <summary>
        /// Tests that it is recommended that in rendering, only the plain string content of the Image description
        /// be used, without formatting.
        /// </summary>
        [Test]
        public void TestImageC()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Image\TestImageC.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(2));

            CheckShape(paras[0].GetChild(NodeType.Any, 0, false) as Shape, "foo bar", "/url2");
            CheckShape(paras[1].GetChild(NodeType.Any, 0, false) as Shape, "foo bar", "/url2");
        }


        /// <summary>
        /// Tests Image with empty description.
        /// </summary>
        [Test]
        public void TestImageE()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Image\TestImageE.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(1));

            CheckShape(paras[0].GetChild(NodeType.Any, 0, false) as Shape, string.Empty, "/url");
        }



        /// <summary>
        /// Tests collapsed reference image import.
        /// </summary>
        [Test]
        public void TestReferenceImage_1()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceImages\TestCRImageA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckShape(paras[0].GetChild(NodeType.Any, 1, false) as Shape, "foo", "/uri", "title");
        }

        /// <summary>
        /// Tests full reference image import.
        /// </summary>
        [Test]
        public void TestReferenceImage_2()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceImages\TestFRImageA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckShape(paras[0].GetChild(NodeType.Any, 1, false) as Shape, "Alt text", "/uri", "title");
        }

        /// <summary>
        /// Tests shortcut reference image import.
        /// </summary>
        [Test]
        public void TestReferenceImage_3()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceImages\TestSRImageA.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckShape(paras[0].GetChild(NodeType.Any, 1, false) as Shape, "foo", "/uri", "title");

        }

        /// <summary>
        /// Tests importing the reference image in Base64.
        /// </summary>
        [Test]
        public void TestReferenceImage_4()
        {
            const string imageData = @"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGwAAAA4CAYAAAAYeR0sAAAAAXNSR0IArs4c" +
                                     "6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAGVSURBVHhe7ZXbTQNBDACvI3qhMWq" +
                                     "gUUCO7LBsnHCPfdh3M9L8Zm1PoixZeXv/+D6ifgy0wjty6deyfB7R+8xSHQNqvGOJ3pFH6s0k6tjXoT6Ad6" +
                                     "zI1vPrWuehXtA7QmbL3XTlnJw10CvTxSsH9ha6kuUt9DxxINJrw4Qj1DanhSPUMYdGI1Qb7UuvZ22PPeA9j" +
                                     "vvtEo1QfbUfg577GMQa5+FoxBrv7mjEmufmaMSa7+poxIrjv9GIFc+n0YgVU4Il9CEasWJLsITeoxErhwRL" +
                                     "JsGSSbCE3qIRLI8ESybBEnmLxX9YHu/BBKLFl2DJ/BNMIFpcH2IJBIurG0wgWjyfxhIIFsuXsQyixXBVLIN" +
                                     "o890UTCDaHOXum2MZRBvr7lAlVtx7ANvZJFYJ0frZPJZBtLbKPbvFMuwRbwBc55BQNYTb5/BQNYRbp91Jzz" +
                                     "YfG4h4v5Y30TPF5OrhUkTysMHPHq/cU9T1c1Mv5S2eyXIXXfHclAub3mFm680p6hrXxjuM6B2yh97bOhpsw" +
                                     "TtkD/U5ALgoy/IDBM5W/C1nowQAAAAASUVORK5CYII=";

            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceImages\TestFRImageH.md");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            CheckShape(paras[0].GetChild(NodeType.Any, 0, false) as Shape, "foo", imageData, "title");
            CheckShape(paras[0].GetChild(NodeType.Any, 2, false) as Shape, "foo1", imageData, "title");
            CheckShape(paras[0].GetChild(NodeType.Any, 4, false) as Shape, "foo2", imageData, "title");
        }

        /// <summary>
        /// Tests a simple Footnote.
        /// </summary>
        [Test]
        public void TestFootnote_01()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_01", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote \u0002 Some footnote text.\r and definition after two LineBreaks.", "Normal");
            CheckFootnotes(paras[0], "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// Tests Footnote Definition without Footnote reference is ignored.
        /// </summary>
        [Test]
        public void TestFootnote_02()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_02", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "\f", "Normal");
        }

        /// <summary>
        /// Tests Footnote Reference without Footnote Definition is not allowed.
        /// </summary>
        [Test]
        public void TestFootnote_03()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_03", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "Here's a footnote[^10] without definition (shown as simple text).", "Normal");
            CheckFootnotes(paras[0]);
        }

        /// <summary>
        /// Tests that blank line is not required before Footnote definition.
        /// </summary>
        [Test]
        public void TestFootnote_04()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_04", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "Here are footnote \u0002 Some footnote text.\r " +
                "and definition after one LineBreak (accepted).", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// Tests that multiple Footnote references with the same reference mark can refer to a single definition.
        /// </summary>
        [Test]
        public void TestFootnote_05()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_05", LoadFormat.Markdown);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote\u0002 Some footnote text.\r, footnote\u0002 Some footnote text.\r ", "Normal");
            CheckFootnotes(paras[0], "\u0002 Some footnote text.\r", "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// Tests that very first of Footnote definitions with the same reference mark will be taken
        /// and all the rest will be ignored.
        /// </summary>
        [Test]
        public void TestFootnote_06()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_06", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "Here are footnote\u0002 Some footnote text (line1).\r " +
                "and two definitions with same label (error, accepted line1).", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text (line1).\r");
        }

        /// <summary>
        /// Tests that multiline Footnote Definition is treated as a single line when lines are not separated with blank line.
        /// </summary>
        [Test]
        public void TestFootnote_07()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_07", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote \u0002 Some footnote text (line1). Some footnote text (line2). " +
                "Some footnote text (line3). Some footnote text (line4).\r " +
                "and multiline definition with one LineBreak (accepted as one line).", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text (line1). " +
                                     "Some footnote text (line2). " +
                                     "Some footnote text (line3). " +
                                     "Some footnote text (line4).\r");
        }

        /// <summary>
        /// Tests that in order to have a multiple paragraphs in a Footnote Definition,
        /// each line must be indented with at least four space characters and prepended with a blank line.
        /// </summary>
        [Test]
        public void TestFootnote_08()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_08", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote " +
                "\u0002 Some footnote text (line1).\r" +
                "Some footnote text (line2).\r" +
                "Some footnote text (line3).\r" +
                " and multiline definition with two LineBreaks and indent (accepted).",
                "Normal");

            CheckFootnotes(paras[0],
                "\u0002 Some footnote text (line1).\rSome footnote text (line2).\rSome footnote text (line3).\r");
        }

        /// <summary>
        /// Tests that when lines in Footnote definition are not indented with at least four space characters,
        /// they will not treated as a part of this definition.
        /// </summary>
        [Test]
        public void TestFootnote_09()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_09", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "Here are footnote \u0002 Some footnote text (line1).\r " +
                "and multiline definition with two LineBreaks and without indent (accepted line1).", "Normal");
            CheckParagraph(paras[1], "Some footnote text (line2).", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text (line1).\r");
        }

        /// <summary>
        /// Tests that Footnote Definition cannot have a space character in the reference mark.
        /// </summary>
        [Test]
        public void TestFootnote_10()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_10", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(6));

            CheckParagraph(paras[0], "Here are wrong footnote[^ e] and wrong definition (simple text).", "Normal");
            CheckParagraph(paras[1], "[^ e]: Some footnote text (simple text).", "Normal");
            CheckParagraph(paras[2], "Here are wrong footnote[^e f] and wrong definition (simple text).", "Normal");
            CheckParagraph(paras[3], "[^e f]: Some footnote text (simple text).", "Normal");
            CheckParagraph(paras[4], "Here are wrong footnote[^e ] and wrong definition (simple text).", "Normal");
            CheckParagraph(paras[5], "[^e ]: Some footnote text (simple text).", "Normal");
        }

        /// <summary>
        /// Tests a Footnote Reference with a Footnote Definition with a valid symbols in the label.
        /// </summary>
        [Test]
        public void TestFootnote_11()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_11", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote\u0002 Some footnote text.\r and definition (accepted).", "Normal");
            CheckFootnotes(paras[0], "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// Tests that InlineCode delimiters have a precedence over a FootnoteReference delimiter.
        /// </summary>
        [Test]
        public void TestFootnote_12()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_12", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0],
                "Here are wrong footnote[^t] and wrong definition (simple text and code).", "Normal");
            CheckParagraph(paras[1], "[^t]: Some footnote text (simple text and code).", "Normal");
        }


        /// <summary>
        /// Tests that InlineLink title has a precedence over a FootnoteReference delimiter.
        /// </summary>
        [Test]
        public void TestFootnote_14()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_14", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are \u0013 HYPERLINK \"/uri\" \\o \"titl[^pe\" \u0014link with title\u0015], " +
                "footnote\u0002 Some footnote text.\r and definition (accepted).", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// Tests how the simple text is parsed after the multiline definition.
        /// </summary>
        [Test]
        public void TestFootnote_15()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_15", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0],
                "Here are footnote\u0002 Some footnote text (para1).\rSome footnote text (para2).\r" +
                ", multiline definition and simple text.", "Normal");
            CheckParagraph(paras[1], "Simple text.", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text (para1).\rSome footnote text (para2).\r");
        }

        /// <summary>
        /// Tests two Footnote Definitions in a row.
        /// </summary>
        [Test]
        public void TestFootnote_16()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_16", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote\u0002 Footnote def1.\r, footnote\u0002 Footnote def2.\r" +
                " and two definitions in a row (accepted).", "Normal");

            CheckFootnotes(paras[0], "\u0002 Footnote def1.\r", "\u0002 Footnote def2.\r");
        }


        /// <summary>
        /// Tests IndentedCode in Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_18()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_18", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "Here is footnote" +
                                     "\u0002 Some footnote text (para1).\r" +
                                     "    Some indented footnote text (para2).\r" +
                                     " with IndentedCode in definition.\f", "Normal");

            NodeCollection footnotes = CheckFootnotes(paras[0],
                "\u0002 Some footnote text (para1).\r    Some indented footnote text (para2).\r");

            CheckParagraph(((Footnote)footnotes[0]).Paragraphs[0], "\u0002 Some footnote text (para1).\r", "Footnote Text");
            CheckParagraph(((Footnote)footnotes[0]).Paragraphs[1], "    Some indented footnote text (para2).\r",
                "Footnote Text1", "IndentedCode");
        }

        /// <summary>
        /// Tests an empty Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_19()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_19", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "Here are footnote\u0002\r and empty definition.", "Normal");
            CheckFootnotes(paras[0], "\u0002\r");
        }

        /// <summary>
        /// Tests an empty Footnote Definition and followed simple text.
        /// </summary>
        [Test]
        public void TestFootnote_20()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_20", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));

            CheckParagraph(paras[0], "Here are footnote\u0002\r, empty definition and simple text.", "Normal");
            CheckParagraph(paras[1], "Simple text.", "Normal");

            CheckFootnotes(paras[0], "\u0002\r");
        }

        /// <summary>
        /// Tests a Footnote Reference in italics.
        /// </summary>
        [Test]
        public void TestFootnote_21()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_21", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            Paragraph para = paras[0];

            CheckParagraph(para, "Here are footnote in italics \u0002 Some footnote text.\r and definition.", "Normal");

            CheckRunAttr(GetRunWithText(para, "Here are footnote in italics"), false, false);
            CheckRunAttr(GetRunWithText(para, "and definition"), false, false);

            NodeCollection footnotes = CheckFootnotes(para, "\u0002 Some footnote text.\r");
            Assert.That(((Footnote)footnotes[0]).Font.Italic, Is.True);
        }

        /// <summary>
        /// Tests a Footnote Reference and a simple text in italics.
        /// </summary>
        [Test]
        public void TestFootnote_22()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_22", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            Paragraph para = paras[0];

            CheckParagraph(para, "Here are footnote\u0002 Some footnote text.\r and simple text in italics.", "Normal");

            CheckRunAttr(GetRunWithText(para, "Here are"), false, false);
            CheckRunAttr(GetRunWithText(para, "footnote"), false, true);
            CheckRunAttr(GetRunWithText(para, "and"), false, true);
            CheckRunAttr(GetRunWithText(para, "simple text"), false, false);

            NodeCollection footnotes = CheckFootnotes(para, "\u0002 Some footnote text.\r");
            Assert.That(((Footnote)footnotes[0]).Font.Italic, Is.True);
        }

        /// <summary>
        /// Tests a Footnote Reference and a simple text in italics (special cases).
        /// </summary>
        [Test]
        public void TestFootnote_23()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_23", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            Paragraph para = paras[0];

            CheckParagraph(para,
                "Here are footnote \u0002 Some footnote text.\r, footnote\u0002 Some footnote text.\r " +
                "and simple text in italics.", "Normal");

            CheckRunAttr(GetRunWithText(para, "Here are footnote"), false, false);
            Run run = GetRunWithText(para, ",");
            CheckRunAttr(run, false, true);
            // This is non-italic run with space character between two footnotes in italics.
            CheckRun((Run)run.NextSibling, " ", false, false);
            CheckRunAttr(GetRunWithText(para, " and"), false, false);

            NodeCollection footnotes = CheckFootnotes(para, "\u0002 Some footnote text.\r", "\u0002 Some footnote text.\r");
            Assert.That(((Footnote)footnotes[0]).Font.Italic, Is.True);
            Assert.That(((Footnote)footnotes[1]).Font.Italic, Is.True);
        }

        /// <summary>
        /// Tests a Footnote Reference with an indented Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_24()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_24", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote\u0002 Some footnote text.\r and indented definition.", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text.\r");
        }


        /// <summary>
        /// Tests a Footnote Reference with a Footnote Definition containing list.
        /// </summary>
        /// <remarks>
        /// TODO: Footnote Definition containing list is not implemented.
        /// </remarks>
        [Test]
        public void TestFootnote_26()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_26", LoadFormat.Markdown);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0],
                "Here are footnote\u0002 Some footnote text. + list item 1. + list item 2.\r" +
                " and definition containing list.", "Normal");

            CheckFootnotes(paras[0], "\u0002 Some footnote text. + list item 1. + list item 2.\r");
        }

        /// <summary>
        /// Tests that InlineLink text has a precedence over a FootnoteReference delimiter.
        /// </summary>
        [Test]
        public void TestFootnote_27()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_27", LoadFormat.Markdown);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            CheckParagraph(paras[0], "Here is link \u0013 HYPERLINK \"link_dest\" \u0014^1\u0015.", "Normal");
            CheckFootnotes(paras[0]);
        }

        /// <summary>
        /// Tests emphases in Footnote.
        /// </summary>
        [Test]
        public void TestFootnote_28()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Footnotes\TestFootnote_28", LoadFormat.Markdown);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            // Check paragraph with footnote references.
            Paragraph paragraph = paras[0];
            CheckRunAttr(GetRunWithText(paragraph, "Here is footnote in "), false, false);
            CheckRunAttr(GetRunWithText(paragraph, "bold"), true, false);
            CheckRunAttr(GetRunWithText(paragraph, "and footnote in"), false, false);
            CheckRunAttr(GetRunWithText(paragraph, "italics"), false, true);

            NodeCollection footnotes = CheckFootnotes(paragraph,
                "\u0002 Some bold footnote text.\r",
                "\u0002 Some italic footnote\r");

            // Check first footnote.
            Footnote footnote = (Footnote)footnotes[0];
            Assert.That(footnote.Font.Bold, Is.True);
            paragraph = footnote.Paragraphs[0];
            CheckRunAttr(GetRunWithText(paragraph, "Some"), false, false);
            CheckRunAttr(GetRunWithText(paragraph, "bold footnote"), true, false);
            CheckRunAttr(GetRunWithText(paragraph, "text."), false, false);

            // Check second footnote.
            footnote = (Footnote)footnotes[1];
            Assert.That(footnote.Font.Italic, Is.False);
            paragraph = footnote.Paragraphs[0];
            CheckRunAttr(GetRunWithText(paragraph, "Some"), false, false);
            CheckRunAttr(GetRunWithText(paragraph, "italic footnote"), false, true);
        }

        /// <summary>
        /// WORDSNET-21680 Missed footnotes during conversion between DOCX and Markdown.
        /// Fixed via implementing the Markdown footnotes.
        /// </summary>
        [Test]
        public void Test21680()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test21680", LoadFormat.Markdown);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(3));

            CheckParagraph(paras[0], "header\r", "Heading 1");

            CheckParagraph(paras[1], "Here's a footnote \u0002 Some footnote text.\r.", "Normal");
            CheckFootnotes(paras[1], "\u0002 Some footnote text.\r");

            CheckParagraph(paras[2], "Here's a other footnote \u0002 Some footnote text.\r.", "Normal");
            CheckFootnotes(paras[2], "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// WORDSNET-19240 Improve markdown emphases parsing.
        /// There was incorrect calculation of the next character of run delimiter
        /// that caused underscore delimiter to be invalid intraword emphasis.
        /// </summary>
        [Test]
        public void Test19240()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test19240.md");

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            CheckParagraph(paragraph, "1234\f", "Normal");

            RunCollection runs = paragraph.Runs;

            CheckRun(runs[0], "1", true, false);
            CheckRun(runs[1], "2", true, true);
            CheckRun(runs[2], "3", false, true);
            CheckRun(runs[3], "4", false, false);
        }


        /// <summary>
        /// WORDSNET-20703 XML (or DOCX) Document hangs upon loading.
        /// There are a lot of nested AutoLinks in the file that takes a lot of time to parse them. As there cannot be
        /// nested AutoLinks by spec, we can significantly improve performance if will stop to parse such AutoLinks.
        /// </summary>
        [Test]
        public void Test20703()
        {
            Test20703ThreadWorker worker = new Test20703ThreadWorker();
            TestUtil.IsHanging(worker, 15);
        }




        /// <summary>
        /// WORDSNET-22787 Introduce ExportImagesAsBase64 option for MD format.
        /// Tests reading the inline base64 image.
        /// </summary>
        [Test]
        public void Test22787()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Test22787.md");
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Assert.That(shape.ImageData.HasImage, Is.True);
            Assert.That(shape.ImageData.IsLinkOnly, Is.False);
            Assert.That(shape.ImageData.ImageSize.HeightPoints, Is.EqualTo(42).Within(0.1));
            Assert.That(shape.ImageData.ImageSize.HeightPixels, Is.EqualTo(56));
            Assert.That(shape.ImageData.ImageSize.WidthPoints, Is.EqualTo(81).Within(0.1));
            Assert.That(shape.ImageData.ImageSize.WidthPixels, Is.EqualTo(108));
        }



        /// <summary>
        /// WORDSNET-23746 Markdown document with a SVG image embedded as a data URL.
        /// FOSS Storing an SVG requires rasterizing a PNG fallback (removed rendering subsystem),
        /// so the Markdown reader skips SVG images instead of throwing. They are not added to the model.
        /// </summary>
        [Test]
        public void Test23746()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Inlines\Image\Test23746.md");

            Assert.That(doc.FirstSection.Body.Shapes.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-26214 Code block gets corrupted during loading Markdown.
        /// Improved appending markdown blocks in <see cref="FencedCodeBlock.TryAppend"/>.
        /// </summary>
        [Test]
        public void Test26214()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test26214.md");
            Paragraph para = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.GetText().StartsWith("*       *\v"), Is.True);
            Assert.That(para.ParagraphStyle.Name, Is.EqualTo("FencedCode"));
        }

        /// <summary>
        /// WORDSNET-26064 Empty lines are lost after importing MD document.
        /// Implemented new option <see cref="MarkdownLoadOptions.PreserveEmptyLines"/>.
        /// </summary>
        [TestCase(true, 3)]
        [TestCase(false, 0)]
        public void Test26064(bool preserveEmptyLines, int expectedEmptyParagraphsCount)
        {
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { PreserveEmptyLines = preserveEmptyLines };
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test26064.md", loadOptions);

            // Check before save.
            Paragraph para = TestUtil.GetParagraphWithText(doc, "1 empty line\r");
            Assert.That(GetEmptyParagraphsCount(para, false), Is.EqualTo(expectedEmptyParagraphsCount));

            // Check after save-open.
            doc = TestUtil.SaveOpen(doc, string.Format(@"ImportMarkdown\Customers\Test26064_{0}", preserveEmptyLines),
                UnifiedScenario.Md2Md, loadOptions);
            para = TestUtil.GetParagraphWithText(doc, "1 empty line\r");
            Assert.That(GetEmptyParagraphsCount(para, false), Is.EqualTo(expectedEmptyParagraphsCount));
        }

        /// <summary>
        /// Relates to WORDSNET-26064.
        /// Tests complex case with simple paragraph, tables, lists and quotes.
        /// </summary>
        [Test]
        public void Test26064Complex()
        {
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { PreserveEmptyLines = true };
            Document doc = TestUtil.OpenSaveOpen(@"ImportMarkdown\Customers\Test26064Complex",
                UnifiedScenario.Md2Md, loadOptions);

            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(GetEmptyParagraphsCount(table, true), Is.EqualTo(3));
            Assert.That(GetEmptyParagraphsCount(table, false), Is.EqualTo(3));

            Paragraph para = TestUtil.GetParagraphWithText(doc, "lorem ipsum\r");
            Assert.That(GetEmptyParagraphsCount(para, true), Is.EqualTo(3));
            Assert.That(GetEmptyParagraphsCount(para, false), Is.EqualTo(3));

            para = TestUtil.GetParagraphWithText(doc, "item1\r");
            Assert.That(GetEmptyParagraphsCount(para, true), Is.EqualTo(3));
            Assert.That(GetEmptyParagraphsCount(para, false), Is.EqualTo(3));

            para = TestUtil.GetParagraphWithText(doc, "quote3\r");
            Assert.That(GetEmptyParagraphsCount(para, true), Is.EqualTo(3));
        }

        /// <summary>
        /// Relates to WORDSNET-26064.
        /// Checks empty lines at very start and end of input file.
        /// </summary>
        [TestCase(true, "\rLorem\r\r\rIpsum\r\f")]
        [TestCase(false, "Lorem\rIpsum\f")]
        public void Test26064EmptyLines(bool preserveEmptyLines, string expectedText)
        {
            const string inputText = "\nLorem\n\n\nIpsum\n\n";
            using (MemoryStream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(inputText.ToCharArray())))
            {
                MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { PreserveEmptyLines = preserveEmptyLines };
                Document doc = new Document(inputStream, loadOptions);
                Assert.That(doc.FirstSection.Body.GetText(), Is.EqualTo(expectedText));
            }
        }

        /// <summary>
        /// Relates to WORDSNET-26064.
        /// Checks, that <see cref="MarkdownLoadOptions"/> is created with <see cref="LoadFormat.Markdown"/> by default.
        /// </summary>
        [Test]
        public void Test26064LoadFormat()
        {
            MarkdownLoadOptions markdownLoadOptions = new MarkdownLoadOptions();
            Assert.That(markdownLoadOptions.LoadFormat, Is.EqualTo(LoadFormat.Markdown));
        }

        /// <summary>
        /// WORDSNET-26368 Add resilience by ignoring Spaces at start/end of each Row during Markdown Table import.
        /// Spaces before and after the table have been taken into account.
        /// </summary>
        [Test]
        public void Test26368()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test26368.md");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// WORDSNET-26451 Paragraph in the table has heading style after importing MD.
        /// Should reset paragraph style to 'Normal' before writing Table block into model
        /// in <see cref="Aspose.Words.RW.Markdown.Reader.MarkdownReaderContext.Open"/>.
        /// </summary>
        [Test]
        public void Test26451()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test26451.md");
            Table table = doc.FirstSection.Body.Tables[0];
            Paragraph para = table.FirstRow.FirstCell.FirstParagraph;

            Assert.That(para.ParagraphStyle.Name, Is.EqualTo("Normal"));
        }

        /// <summary>
        /// Tests loading of HTML elements nested to other inline blocks.
        /// </summary>
        [Test]
        public void TestHtmlNestedToInline()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlNestedToInline.md");
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(2));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("qw"));
            Assert.That(run.Font.Bold, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("erty"));
            Assert.That(run.Font.Bold, Is.True);

            paragraph = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(2));
            run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("gh"));
            Assert.That(run.Font.Bold, Is.True);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("jk"));
            Assert.That(run.Font.Bold, Is.False);
        }

        /// <summary>
        /// Tests loading of nested HTML elements.
        /// </summary>
        [Test]
        public void TestHtmlNested()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlNested.md");
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("q"));
            Assert.That(run.Font.Bold, Is.False);
            Assert.That(run.Font.Italic, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("we"));
            Assert.That(run.Font.Bold, Is.True);
            Assert.That(run.Font.Italic, Is.False);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("rty"));
            Assert.That(run.Font.Bold, Is.True);
            Assert.That(run.Font.Italic, Is.True);

            paragraph = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("g"));
            Assert.That(run.Font.Bold, Is.True);
            Assert.That(run.Font.Italic, Is.True);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("h"));
            Assert.That(run.Font.Bold, Is.False);
            Assert.That(run.Font.Italic, Is.True);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("jk"));
            Assert.That(run.Font.Bold, Is.False);
            Assert.That(run.Font.Italic, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML 'br' element.
        /// </summary>
        [Test]
        public void TestHtmlBr()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlBr.md");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));
            Assert.That(paras[0].GetText(), Is.EqualTo("qwerty\r"));
            Assert.That(paras[1].GetText(), Is.EqualTo("ghjk\f"));
        }

        /// <summary>
        /// Tests loading of HTML 'b' element.
        /// </summary>
        [Test]
        public void TestHtmlBold()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlBold.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Bold, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Bold, Is.True);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Bold, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML 'i' element.
        /// </summary>
        [Test]
        public void TestHtmlItalic()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlItalic.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Italic, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Italic, Is.True);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Italic, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML 'u' element.
        /// </summary>
        [Test]
        public void TestHtmlUnderline()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlUnderline.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Underline, Is.EqualTo(Underline.None));
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Underline, Is.EqualTo(Underline.Single));
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Underline, Is.EqualTo(Underline.None));
        }

        /// <summary>
        /// Tests loading of HTML 'strong' element.
        /// </summary>
        [Test]
        public void TestHtmlStrong()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlStrong.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Bold, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Bold, Is.True);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Bold, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML 'sup' element.
        /// </summary>
        [Test]
        public void TestHtmlSup()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlSup.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Superscript, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Superscript, Is.True);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Superscript, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML 'sub' element.
        /// </summary>
        [Test]
        public void TestHtmlSub()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlSub.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Subscript, Is.False);
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Subscript, Is.True);
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Subscript, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML 'code' element.
        /// </summary>
        [Test]
        public void TestHtmlCode()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlCode.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(3));
            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.Style.Name, Is.EqualTo("Default Paragraph Font"));
            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("4567"));
            Assert.That(run.Font.Style.Name, Is.EqualTo("InlineCode"));
            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("890"));
            Assert.That(run.Font.Style.Name, Is.EqualTo("Default Paragraph Font"));
        }

        /// <summary>
        /// Tests loading of HTML 's', 'strike', 'del' elements.
        /// </summary>
        [Test]
        public void TestHtmlStrike()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlStrike.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(7));

            Run run = paragraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123"));
            Assert.That(run.Font.StrikeThrough, Is.False);

            run = paragraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("abcd"));
            Assert.That(run.Font.StrikeThrough, Is.True);

            run = paragraph.Runs[2];
            Assert.That(run.Text, Is.EqualTo("56"));
            Assert.That(run.Font.StrikeThrough, Is.False);

            run = paragraph.Runs[3];
            Assert.That(run.Text, Is.EqualTo("zxcvb"));
            Assert.That(run.Font.StrikeThrough, Is.True);

            run = paragraph.Runs[4];
            Assert.That(run.Text, Is.EqualTo("8"));
            Assert.That(run.Font.StrikeThrough, Is.False);

            run = paragraph.Runs[5];
            Assert.That(run.Text, Is.EqualTo("ghjkl"));
            Assert.That(run.Font.StrikeThrough, Is.True);

            run = paragraph.Runs[6];
            Assert.That(run.Text, Is.EqualTo("0"));
            Assert.That(run.Font.StrikeThrough, Is.False);
        }

        /// <summary>
        /// Tests loading of HTML unknown element 'z'.
        /// </summary>
        [Test]
        public void TestHtmlUnknown()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestHtmlUnknown.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.Runs.Count, Is.EqualTo(5));
            Assert.That(paragraph.Runs[0].Text, Is.EqualTo("123"));
            Assert.That(paragraph.Runs[1].Text, Is.EqualTo("<z>"));
            Assert.That(paragraph.Runs[2].Text, Is.EqualTo("4567"));
            Assert.That(paragraph.Runs[3].Text, Is.EqualTo("</z>"));
            Assert.That(paragraph.Runs[4].Text, Is.EqualTo("890"));
        }

        /// <summary>
        /// Tests loading of HTML inline element inside Quote.
        /// </summary>
        [Test]
        public void TestHtmlInsideQuote()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestInsideQuote.md");

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.GetText(), Is.EqualTo("aaa\r"));
            Style style = paragraph.ParagraphStyle;
            Assert.That(style.Name, Is.EqualTo("Quote"));
            Assert.That(style.BaseStyleName, Is.EqualTo("Heading 1"));

            paragraph = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(paragraph.GetText(), Is.EqualTo("bbb\f"));
            style = paragraph.ParagraphStyle;
            Assert.That(style.Name, Is.EqualTo("Quote1"));
            Assert.That(style.BaseStyleName, Is.EqualTo("Heading 6"));
        }

        /// <summary>
        /// Tests loading of HTML inline element inside List.
        /// </summary>
        [Test]
        public void TestHtmlInsideList()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestInsideList.md");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.GetText(), Is.EqualTo("aaa\f"));
            Style style = paragraph.ParagraphStyle;
            Assert.That(style.Name, Is.EqualTo("Heading 1"));
            Assert.That(style.BaseStyleName, Is.EqualTo("Normal"));

            doc.UpdateListLabels();
            Assert.That(paragraph.ListLabel.LabelString, Is.EqualTo("1."));
        }

        /// <summary>
        /// Tests loading of HTML inline element inside InlineCode.
        /// </summary>
        [Test]
        public void TestHtmlInsideInlineCode()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\InlineHtml\TestInsideInlineCode.md");

            RunCollection runs = doc.FirstSection.Body.Paragraphs[0].Runs;
            Assert.That(runs.Count, Is.EqualTo(5));

            Assert.That(runs[0].Text, Is.EqualTo("<b>aaa</b>"));
            Assert.That(runs[0].Font.Style.Name, Is.EqualTo("InlineCode"));

            Assert.That(runs[1].Text, Is.EqualTo(", "));
            Assert.That(runs[1].Font.Style.Name, Is.EqualTo("Default Paragraph Font"));

            Assert.That(runs[2].Text, Is.EqualTo("<br>"));
            Assert.That(runs[2].Font.Style.Name, Is.EqualTo("InlineCode"));

            Assert.That(runs[3].Text, Is.EqualTo(", "));
            Assert.That(runs[3].Font.Style.Name, Is.EqualTo("Default Paragraph Font"));

            Assert.That(runs[4].Text, Is.EqualTo("<i>bbb</i>"));
            Assert.That(runs[4].Font.Style.Name, Is.EqualTo("InlineCode"));
        }

        /// <summary>
        /// WORDSNET-27290 HTML is not interpreted while loading Markdown.
        /// Implemented loading of raw HTML into markdown.
        /// </summary>
        [Test]
        public void Test27290()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test27290.md");
            Table table = doc.FirstSection.Body.Tables[0];
            Cell cell = table.FirstRow.FirstCell;

            Assert.That(cell.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(cell.Paragraphs[0].GetText(), Is.EqualTo("你是谁\r"));
            Assert.That(cell.Paragraphs[1].GetText(), Is.EqualTo("我是你\a"));
        }


        /// <summary>
        /// WORDSNET-27318 Investigate whether text between ++ should be underlined.
        /// Implemented a new public option <see cref="MarkdownLoadOptions.ImportUnderlineFormatting"/>.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test27318(bool isImportUnderlineFormatting)
        {
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions();
            loadOptions.ImportUnderlineFormatting = isImportUnderlineFormatting;
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test27318.md", loadOptions);

            Run run = TestUtil.GetRunWithText(doc.FirstSection.Body.LastParagraph, "...12...");
            Assert.That(run.Font.Underline, Is.EqualTo(isImportUnderlineFormatting ? Underline.Single : Underline.None));
        }

        /// <summary>
        /// Relates to WORDSNET-27318.
        /// Tests <see cref="MarkdownLoadOptions.ImportUnderlineFormatting"/> option default value.
        /// </summary>
        [Test]
        public void Test27318Default()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test27318Default.md");

            Run run = TestUtil.GetRunWithText(doc.FirstSection.Body.LastParagraph, "...12...");
            Assert.That(run.Font.Underline, Is.EqualTo(Underline.None));
        }

        /// <summary>
        /// WORDSNET-28225 Soft line break is improperly read from markdown document.
        /// Added a new option <see cref="MarkdownLoadOptions.SoftLineBreakCharacter"/> to control the behavior.
        /// </summary>
        [TestCase(' ')]
        [TestCase('\v')]
        [TestCase('\n')]
        [TestCase('X')]
        public void Test28225(char softLineBreakChar)
        {
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions();
            loadOptions.SoftLineBreakCharacter = softLineBreakChar;
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test28255.txt", loadOptions);

            // This is last word of the problematic line of text.
            const string lastWord = "#NULL";
            Run run = TestUtil.GetRunWithText(doc.FirstSection.Body, string.Format("...{0}...", lastWord));
            if (softLineBreakChar < ' ')
            {
                Assert.That(run.IsLastChild);
            }
            else
            {
                int nextCharIndex = run.Text.IndexOf(lastWord, StringComparison.Ordinal) + lastWord.Length;
                Assert.That(run.Text[nextCharIndex], Is.EqualTo(softLineBreakChar));
            }

            TestUtil.SaveCheckGold(doc, string.Format(@"ImportMarkdown\Customers\Test28255_{0}.docx",
                FormatterPal.IntToStrX(softLineBreakChar)));
        }

        /// <summary>
        /// Relates to WORDSNET-28225.
        /// Tests a new option with default value.
        /// </summary>
        [Test]
        public void Test28225Default()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\Customers\Test28255Default.txt");

            // This is last word of the problematic line of text immediately followed by a 'SPACE' character.
            const string lastWord = "#NULL ";
            Run run = TestUtil.GetRunWithText(doc.FirstSection.Body, string.Format("...{0}...", lastWord));
            Assert.That(run, IsNot.Null());

            TestUtil.SaveCheckGold(doc, @"ImportMarkdown\Customers\Test28255Default.docx");
        }


        /// <summary>
        /// Relates to WORDSNET-28605.
        /// Tests import simple HTML table.
        /// </summary>
        [Test]
        public void TestHtmlTableSimple()
        {
            Document doc = OpenSaveOpen(@"ImportMarkdown\InlineHtml\TestHtmlTableSimple.md");

            Table table = doc.FirstSection.Body.Tables[0];
            Cell cell = table.FirstRow.FirstCell;

            Assert.That(cell.GetText(), Is.EqualTo("RMP Version number: \a"));
        }



        /// <summary>
        /// Relates to WORDSNET-28605.
        /// Tests when there are two consequent HTML tables.
        /// </summary>
        [Test]
        public void TestTwoHtmlTables()
        {
            Document doc = OpenSaveOpen(@"ImportMarkdown\InlineHtml\TestTwoHtmlTables.md");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(2));

            Assert.That(tables[0].GetText(), Is.EqualTo("1st table \a1 \a\a"));
            Assert.That(tables[1].GetText(), Is.EqualTo("2nd table\a2 \a\a"));
        }



        /// <summary>
        /// Relates to WORDSNET-28605.
        /// Tests HTML table with unclosed tags ('table', 'td').
        /// </summary>
        [Test]
        public void TestHtmlTableWithUnclosedTagsA()
        {
            Document doc = OpenSaveOpen(@"ImportMarkdown\InlineHtml\TestHtmlTableWithUnclosedTagsA.md");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Relates to WORDSNET-28605.
        /// Tests HTML table with unclosed tags (two consequent 'td').
        /// </summary>
        [Test]
        public void TestHtmlTableWithUnclosedTagsB()
        {
            Document doc = OpenSaveOpen(@"ImportMarkdown\InlineHtml\TestHtmlTableWithUnclosedTagsB.md");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Relates to WORDSNET-28605.
        /// Tests nested HTML tables.
        /// </summary>
        [Test]
        public void TestNestedHtmlTables()
        {
            Document doc = OpenSaveOpen(@"ImportMarkdown\InlineHtml\TestNestedHtmlTables.md");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(1));

            // Check nested table.
            tables = tables[0].FirstRow.LastCell.Tables;
            Assert.That(tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Relates to WORDSNET-28605.
        /// Test MarkdownList placed in HTML table, but outside 'td'.
        /// </summary>
        [Test]
        public void TestHtmlTableWithMisplacedMarkdownList()
        {
            Document doc = OpenSaveOpen(@"ImportMarkdown\InlineHtml\TestHtmlTableWithMisplacedMarkdownList.md");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(1));

            Paragraph para = TestUtil.GetParagraphWithText(doc, "...item1...");
            Assert.That(para.IsListItem);
        }

        /// <summary>
        /// Returns count of empty paragraphs before or after a specified Node.
        /// </summary>
        /// <param name="node">The node before or after which to start count.</param>
        /// <param name="isBefore">If true, then count before a specified node. Otherwise, count after it.</param>
        internal static int GetEmptyParagraphsCount(Node node, bool isBefore)
        {
            int count = 0;
            Node siblingNode = isBefore ? node.PreviousSibling : node.NextSibling;
            while ((siblingNode != null) && (siblingNode.NodeType == NodeType.Paragraph) &&
                   !((Paragraph)siblingNode).HasChildNodes)
            {
                count++;
                siblingNode = isBefore ? siblingNode.PreviousSibling : siblingNode.NextSibling;
            }

            return count;
        }

        /// <summary>
        /// A worker thread used in the Test20703.
        /// </summary>
        private class Test20703ThreadWorker : ThreadPal
        {
            protected override void DoWork()
            {
                LoadOptions loadOptions = new LoadOptions(LoadFormat.Markdown, null, null);
                TestUtil.Open(@"ImportMarkdown\Customers\Test20703.xml", loadOptions);
            }
        }

        /// <summary>
        /// The resource loading callback used in Test21685A.
        /// </summary>
        private class Test21685ResourceLoadingCallback : IResourceLoadingCallback
        {
            /// <summary>
            /// Called when Aspose.Words loads any external resource.
            /// </summary>
            public ResourceLoadingAction ResourceLoading(ResourceLoadingArgs args)
            {
                Count++;
                switch (Path.GetFileName(args.OriginalUri))
                {
                    case "serilog_180px.png":
                        return ResourceLoadingAction.Skip;
                    case "gfm_640px.png":
                    {
                        using (Stream srcImageStream = ResourceUtil.FetchResourceStream("Aspose.Words.Resources.3dBubble.jpg"))
                        {
                            byte[] newImage = StreamUtil.CopyStreamToByteArray(srcImageStream);
                            args.SetData(newImage);
                        }
                        return ResourceLoadingAction.UserProvided;
                    }
                    default:
                        return ResourceLoadingAction.Default;
                }
            }

            /// <summary>
            /// Gets number of times the resource loading callback is invoked.
            /// </summary>
            internal int Count { get; private set; }
        }

        /// <summary>
        /// Opens document specified in the path as Markdown.
        /// Then performs <see cref="TestUtil.SaveOpen(Aspose.Words.Document,string)"/>.
        /// Checks gold.
        /// </summary>
        private static Document OpenSaveOpen(string path, SaveFormat sf = SaveFormat.Docx)
        {
            // Set format explicitly. Otherwise, it can be wrong detected as, for example, HTML in some cases.
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Markdown;
            Document doc = TestUtil.Open(path, loadOptions);

            string saveExt = FileFormatUtil.SaveFormatToExtension(sf);
            string savePath = Path.ChangeExtension(path, saveExt);

            return TestUtil.SaveOpen(doc, savePath);
        }
    }
}
