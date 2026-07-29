// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2019 by Ilya Navrotskiy

using System;
using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Loading;
using Aspose.Words.Notes;
using Aspose.Words.RW.Markdown;
using Aspose.Words.RW.Markdown.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Txt;
using Aspose.Words.Tests.Import.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Tests export to markdown format.
    /// </summary>
    [TestFixture]
    public class TestExportMarkdown : TestMarkdownBase
    {
        /// <summary>
        /// Tests writing an empty document.
        /// </summary>
        [Test]
        public void TestEmpty()
        {
            Document doc = new Document();
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\TestEmpty.md", new MarkdownSaveOptions(), false);
            Assert.That(doc.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Tests writing a document with Emphases.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestEmphasis(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestEmphasis", scenario);

            Paragraph paragraph = GetParagraphWithText(doc, "Lorem ipsum dolor sit amet, consectetur _adipiscing_ ___elit***,");

            Run run = GetRunWithText(paragraph, "Lorem");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));

            run = GetRunWithText(paragraph, "ipsum");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "dolor");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "sit");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            run = GetRunWithText(paragraph, "amet");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Tests writing a document with Quotes.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestQuoteA(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestQuoteA", scenario);

            Paragraph paragraph = GetParagraphWithText(doc, "sit");
            CheckStyles(paragraph, "Quote62");

            paragraph = GetParagraphWithText(doc, "adipiscing");
            CheckStyles(paragraph, "Quote67", "Quote66", "Quote65", "Heading 3");

            paragraph = GetParagraphWithText(doc, "elit");
            CheckStyles(paragraph, "Quote63", "Heading 2");

            paragraph = GetParagraphWithText(doc, "Lorem ipsum");
            CheckStyles(paragraph, "Quote68", "SetextHeading1", "Heading 1", "Normal");

            paragraph = GetParagraphWithText(doc, "dolor si amet,");
            CheckStyles(paragraph, "Quote3", "Quote1");
        }

        /// <summary>
        /// Tests writing a document with multi line quotes and multi line regular paragraphs.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestQuoteB(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestQuoteB", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem\vipsum\r", "Normal");
            CheckParagraph(paragraphs[1], "foo\r", "Quote");
            CheckParagraph(paragraphs[2], "bar\r", "Quote1");
            CheckParagraph(paragraphs[3], "dolor sit amet,\vconsectetur adipiscing\velit,\r", "Normal");
            CheckParagraph(paragraphs[4], "baz\vbop\vsed do\veiusmod\r", "Quote");
            CheckParagraph(paragraphs[5], "\r", "Normal");
            CheckParagraph(paragraphs[6], "tempor incididunt\r", "Quote1");
            CheckParagraph(paragraphs[7], "ut labore\f", "Normal");
        }

        /// <summary>
        /// Tests writing a document with AtxHeadings.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestAtxHeadingA(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAtxHeadingA", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem ipsum\r", "Heading 1");
            CheckParagraph(paragraphs[1], "dolor sit amet,\r", "Heading 2");
            CheckParagraph(paragraphs[2], "consectetur\r", "Heading 3");
            CheckParagraph(paragraphs[3], "adipiscing\r", "Heading 6");
            CheckParagraph(paragraphs[4], "####### elit,\v#sed\v##do\v###eiusmod###\v####tempor ####\r", "Normal");
            CheckParagraph(paragraphs[5], "incididunt\r", "Heading 1");
            CheckParagraph(paragraphs[6], "ut # # #\f", "Heading 1");
        }

        /// <summary>
        /// Tests writing a document with AtxHeadings combined with regular paragraphs.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestAtxHeadingB(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAtxHeadingB", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem ipsum\r", "Normal");
            CheckParagraph(paragraphs[1], "dolor sit amet,\r", "Heading 1");
            CheckParagraph(paragraphs[2], "consectetur\r", "Heading 1");
            CheckParagraph(paragraphs[3], "adipiscing\f", "Normal");
        }

        /// <summary>
        /// Tests writing a document with SetextHeadings.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestSetextHeadingA(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestSetextHeadingA", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem ipsum\r", "SetextHeading1", "Heading 1", "Normal");
            CheckParagraph(paragraphs[1], "dolor sit amet,\f", "SetextHeading2", "Heading 2", "Normal");
        }

        /// <summary>
        /// Tests writing a document with multi line SetextHeadings.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestSetextHeadingB(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestSetextHeadingB", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem\vipsum\r", "SetextHeading1", "Heading 1", "Normal");
            CheckParagraph(paragraphs[1], "dolor\vsit\vamet,\r", "SetextHeading2", "Heading 2", "Normal");
            CheckParagraph(paragraphs[2], "consectetur\vadipiscing\velit\f", "SetextHeading2", "Heading 2", "Normal");
        }

        /// <summary>
        /// Tests writing a document with HorizontalRules.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestHorizontalRuleA(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestHorizontalRuleA", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckHorizontalRule(paragraphs[0], "Normal");
            CheckHorizontalRule(paragraphs[1], "Normal");
            CheckHorizontalRule(paragraphs[2], "Normal");
            CheckHorizontalRule(paragraphs[3], "Normal");

            CheckParagraph(paragraphs[4], "-_*\v-*-\r", "Normal");

            CheckHorizontalRule(paragraphs[5], "Normal");
            CheckHorizontalRule(paragraphs[6], "Normal");

            Assert.That(paragraphs.Count, Is.EqualTo(7));
        }

        /// <summary>
        /// Tests writing a document with HorizontalRules combined with heading and regular paragraph.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestHorizontalRuleB(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestHorizontalRuleB", scenario);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem\r", "SetextHeading2", "Heading 2", "Normal");
            CheckParagraph(paragraphs[1], "--\vipsum\r", "Normal");
            CheckHorizontalRule(paragraphs[2], "Normal");

            Assert.That(paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests writing a multi section document with HeadersFooters.
        /// </summary>
        [Test]
        public void TestHeadersFooters()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestHeadersFooters.docx");

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.ExportHeadersFootersMode = TxtExportHeadersFootersMode.AllAtEnd;

            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\TestHeadersFooters.md", so);

            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo("1Lorem ipsum\r"));
            Assert.That(doc.FirstSection.Body.LastParagraph.GetText(), Is.EqualTo("Section3 FIRST footer line3\f"));
        }

        /// <summary>
        /// Tests writing document with IndentedCodes.
        /// </summary>
        [Test]
        public void TestIndentedCode()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestIndentedCode", UnifiedScenario.Docx2Md);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem ipsum\r", "IndentedCode");
            CheckParagraph(paragraphs[1], "dolor\vsit\vamet,       consectetur\v# adipiscing elit,\r", "Normal");
            CheckParagraph(paragraphs[2], "sed do\r", "Heading 1");
            CheckParagraph(paragraphs[3], "eiusmod\v\vtempor\vincididunt\vut labore\v> et dolore\r", "IndentedCode");
            CheckParagraph(paragraphs[4], "magna\valiqua.\r", "Quote");
            CheckParagraph(paragraphs[5], "Ut enim\r", "Quote1", "IndentedCode");
            CheckParagraph(paragraphs[6], "ad minim\r", "IndentedCode");
            CheckParagraph(paragraphs[7], "veniam\r", "Quote");
            CheckParagraph(paragraphs[8], "quis\r", "IndentedCode");
            CheckParagraph(paragraphs[9], "========\vnostrud\r", "SetextHeading1", "Heading 1", "Normal");
            CheckParagraph(paragraphs[10], "exercitation\f", "IndentedCode");
        }

        /// <summary>
        /// Tests writing document with FencedCodes.
        /// </summary>
        [Test]
        public void TestFencedCode()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFencedCode", UnifiedScenario.Docx2Md);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            CheckParagraph(paragraphs[0], "Lorem ipsum\r", "FencedCode");
            CheckParagraph(paragraphs[1], "dolor\r", "FencedCode.C#");
            CheckParagraph(paragraphs[2], "sit\vamet,\r", "FencedCode.C++");
            CheckParagraph(paragraphs[3], "adipiscing elit,\r", "Heading 3");
            CheckParagraph(paragraphs[4], "# eiusmod\r", "FencedCode");
            CheckParagraph(paragraphs[5], "   incididunt\r", "Quote", "FencedCode.Java");
            CheckParagraph(paragraphs[6], "> ut labore\v> et dlr\r", "FencedCode");
            CheckParagraph(paragraphs[7], "quis\f", "FencedCode");
        }

        /// <summary>
        /// Tests writing document with InlineCodes.
        /// </summary>
        [Test]
        public void TestInlineCode()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestInlineCode", UnifiedScenario.Docx2Md);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // Check Normal paragraph with combinations of regular, InlineCode and emphasis text.
            CheckStyles(paras[0], "Normal");
            // A regular text.
            Run run = GetRunWithText(paras[0], "```");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            // An emphasized text.
            run = GetRunWithText(paras[0], "foo");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            // An emphasized InlineCode text.
            run = GetRunWithText(paras[0], "bar");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            // The same as above, but inside a Heading.
            CheckStyles(paras[1], "Heading 1");
            run = GetRunWithText(paras[1], "baz");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            run = GetRunWithText(paras[1], "bop");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));

            // The same as above, but inside a Quote.
            CheckStyles(paras[2], "Quote");
            run = GetRunWithText(paras[2], "lorem");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            run = GetRunWithText(paras[2], "ipsum");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            // Check InlineCode with two back-ticks inside a Quote.
            run = GetRunWithText(paras[2], "conse```ct`etur* adi*piscing");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode.2"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            // Check InlineCode with four back-ticks inside a Quote.
            run = GetRunWithText(paras[2], "d do");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode.4"));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
        }

        /// <summary>
        /// Tests writing document with Strikethrough.
        /// </summary>
        [Test]
        public void TestStrikethrough()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestStrikethrough", UnifiedScenario.Docx2Md);

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            CheckStyles(para, "Normal");

            // A strikethrough text.
            Run run = GetRunWithText(para, "Foo ~bar ");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            // A strikethrough + bold text.
            run = GetRunWithText(para, "boz");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            // A regular text.
            run = GetRunWithText(para, "~~ bop~~~\v~lorem");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));
            // A strikethrough + bold + italic text.
            run = GetRunWithText(para, "set");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            // An InlineCode text.
            run = GetRunWithText(para, "di~~piscin");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
        }

        /// <summary>
        /// Tests writing document with Underline.
        /// </summary>
        [Test]
        public void TestUnderline()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestUnderline.docx");

            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions() { ExportUnderlineFormatting = true };
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { ImportUnderlineFormatting = true };
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\TestUnderline", UnifiedScenario.Md2Md, saveOptions, loadOptions);

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            CheckStyles(para, "Normal");

            // An underline text.
            Run run = GetRunWithText(para, "Foo +bar ");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
            // An underline + bold text.
            run = GetRunWithText(para, "boz");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            // A regular text.
            run = GetRunWithText(para, "++ bop+++\v+lorem");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.None));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));
            // A underline + bold + italic text.
            run = GetRunWithText(para, "set");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            // An InlineCode text.
            run = GetRunWithText(para, "di++piscin");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("InlineCode"));
        }

        /// <summary>
        /// Tests writing document with simple bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListA", UnifiedScenario.Docx2Md);
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
        /// Tests writing document with nested in single line bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListB", UnifiedScenario.Docx2Md);
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
        /// Tests writing document with nested in single line bullet lists and quotes.
        /// </summary>
        [Test]
        public void TestBulletListC()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListC", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Quote", "Normal");
            CheckListItem(paras[1], "b\r", 0, " ", "List", "Quote", "Normal");
            CheckListItem(paras[2], "c\r", 2, "", "Quote2", "List1", "Quote1", "List", "Quote", "Normal");
            CheckListItem(paras[3], "d\r", 5, "", "Quote5", "List6", "List5", "Quote4", "List4", "List3", "Quote3",
                "List2", "Normal");
            CheckListItem(paras[4], "e\f", 5, " ", "List11", "List10", "Quote7", "List9", "List8", "Quote6", "List7",
                "List", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests writing document with headings inside bullet lists and quotes.
        /// </summary>
        [Test]
        public void TestBulletListD()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListD", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Quote", "Normal");
            CheckListItem(paras[1], "b\r", 0, "-", "Quote1", "Heading 1", "Normal");
            CheckListItem(paras[2], "c\f", 0, "-", "Quote2", "SetextHeading2", "Heading 2", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests writing document with headings inside simple bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListE()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListE", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Heading 1", "Normal");
            CheckListItem(paras[1], "b\r", 1, "o", "Normal");
            CheckListItem(paras[2], "c\r", 1, "o", "Heading 2", "Normal");
            CheckListItem(paras[3], "d\f", 2, "", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests writing very complex document with bullet lists.
        /// </summary>
        [Test]
        public void TestBulletListF()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListF", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "Lorem\r", 0, "-", "Normal");
            CheckListItem(paras[1], "ipsum\r", 0, "+", "Normal");
            CheckListItem(paras[2], "dolor\r", 0, "*", "Normal");
            CheckListItem(paras[3], "sit\r", 1, "o", "Normal");
            CheckListItem(paras[4], "amet\r", 1, "o", "Normal");
            CheckListItem(paras[5], "consectetur\r", 2, "", "Normal");
            CheckListItem(paras[6], "adipiscing\velit\r", 3, "", "Normal");
            CheckListItem(paras[7], "sed\r", 3, " ", "Heading 1", "Normal");
            CheckListItem(paras[8], "do\r", 0, " ", "SetextHeading2", "Heading 2", "Normal");
            CheckHorizontalRule(paras[9], "Normal");
            CheckListItem(paras[9], "\r", 0, " ", "Normal");
            CheckListItem(paras[10], "tabs indentation\veiusmod\r", 0, " ", "IndentedCode");
            CheckListItem(paras[11], "tempor\r", 1, "o", "Quote", "Normal");
            CheckListItem(paras[12], "incididunt\r", 1, "o", "Quote2", "Quote1", "Quote", "Normal");
            CheckListItem(paras[13], "ut\r", 1, "o", "Quote1", "Quote", "Normal");
            CheckListItem(paras[14], "labore\v** et\r", 0, "*", "Normal");
            CheckParagraph(paras[15], "-dolore\r", "Normal");
            CheckHorizontalRule(paras[16], "Quote", "Normal");
            CheckListItem(paras[16], "\r", 0, "+", "Quote");
            CheckListItem(paras[17], "aliqua.\r", 0, " ", "Quote");
            CheckListItem(paras[18], "Ut\v- enim\r", 1, "o", "Quote");
            CheckParagraph(paras[19], "+\tad\r", "IndentedCode");
            CheckListItem(paras[20], "minim\r", 0, "-", "Normal");
            CheckListItem(paras[21], "veniam\r", 2, "", "Quote4", "List1", "Quote3", "List", "Quote", "Normal");
            CheckListItem(paras[22], "quis\r", 0, "-", "Normal");
            CheckListItem(paras[23], "nostrud\r", 0, " ", "Normal");
            CheckListItem(paras[24], "exercitation\v-         foo\f", 1, "o", "Normal");

            Assert.That(paras.Count, Is.EqualTo(25));
        }

        /// <summary>
        /// Tests writing document with bullet lists combined with quotes.
        /// </summary>
        [Test]
        public void TestBulletListG()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListG", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "-", "Normal");
            CheckListItem(paras[1], "b\r", 1, "o", "Quote", "Normal");
            CheckListItem(paras[2], "c\f", 1, "o", "List", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests writing document with bullet lists combined with quotes.
        /// </summary>
        [Test]
        public void TestBulletListH()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListH", UnifiedScenario.Docx2Md);
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
        /// Tests writing document with bullet lists combined with quotes.
        /// </summary>
        [Test]
        public void TestBulletListI()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListI", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[1], "A list item with a blockquote:\r", 0, "*", "Normal");
            CheckListItem(paras[2], "This is a blockquote\r", 0, " ", "Quote", "Normal");
            CheckListItem(paras[3], "inside a list item.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests reading of a markdown document with bullet lists and indented code inside.
        /// </summary>
        [Test]
        public void TestBulletListJ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListJ", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckParagraph(paras[0], "To put a code block within a list item\r", "Normal");
            CheckListItem(paras[1], "A list item with a code block:\r", 0, "*", "Normal");
            CheckListItem(paras[2], "[code goes here]\f", 0, " ", "IndentedCode");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests reading of a markdown document with bullet lists and horizontal rule.
        /// </summary>
        [Test]
        public void TestBulletListK()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListK", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckHorizontalRule(paras[0], "Quote", "Normal");
            CheckListItem(paras[0], "\r", 0, "+", "Quote", "Normal");
            CheckListItem(paras[1], "aliqua.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests reading of a markdown document with bullet lists, horizontal rule and indented code.
        /// </summary>
        [Test]
        public void TestBulletListL()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestBulletListL", UnifiedScenario.Docx2Md);
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
        /// Tests writing document with simple ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListA", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "2.", "Normal");
            CheckListItem(paras[3], "d\r", 0, "2.", "Normal");
            CheckListItem(paras[4], "e\r", 1, "1.", "Normal");
            CheckListItem(paras[5], "f\r", 2, "1.", "Normal");
            CheckListItem(paras[6], "g\r", 3, "1.", "Normal");
            CheckListItem(paras[7], "h\r", 2, "2.", "Normal");
            CheckListItem(paras[8], "i\r", 1, "2.", "Normal");
            CheckListItem(paras[9], "j\f", 0, "3.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests writing document with simple ordered lists with ')' as marker delimiter.
        /// </summary>
        [Test]
        public void TestOrderedListB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListB", UnifiedScenario.Docx2Md);
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
        /// Tests 'StartAt' is set properly upon writing ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListC()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListC", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1)", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "1)", "Normal");
            CheckListItem(paras[3], "d\f", 0, "1.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests 'StartAt' is set properly upon writing ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListD()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListD", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1)", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "5)", "Normal");
            CheckListItem(paras[3], "d\f", 0, "8.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests 'StartAt' is set properly upon writing ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListE()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListE", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "5.", "Normal");
            CheckListItem(paras[1], "b\r", 1, "3.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "100)", "Normal");
            CheckListItem(paras[3], "d\f", 0, "6.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests export document with ordered lists combined with bullet lists.
        /// </summary>
        [Test]
        public void TestOrderedListF()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListF", UnifiedScenario.Docx2Md);
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
        /// Tests writing document with nested in a single line ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListG()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListG", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 1, "6.", "List", "Normal");
            CheckListItem(paras[1], "b\r", 0, "10.", "Normal");
            CheckListItem(paras[2], "c\r", 2, "5.", "List1", "Normal");
            CheckListItem(paras[3], "d\f", 2, "3.", "List3", "List2", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests writing document with nested in a single line ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListH()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListH", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "3.", "Quote", "Normal");
            CheckListItem(paras[1], "b\r", 0, " ", "List", "Quote", "Normal");
            CheckListItem(paras[2], "c\r", 2, "7.", "Quote2", "List2", "Quote1", "List1", "Quote", "Normal");
            CheckListItem(paras[3], "d\r", 7, "44.", "Quote7", "List8", "Quote6", "List7", "Quote5", "List6", "Quote4", "List5", "List4", "Quote3", "List3", "Normal");
            CheckListItem(paras[4], "e\f", 5, " ", "List14", "List13", "Quote9", "List12", "List11", "Quote8", "List10", "List9", "Quote", "Normal");

            CheckStyleList(doc.Styles["List"], 0, 2, '.');
            CheckStyleList(doc.Styles["List5"], 4, 45, '.');
            CheckStyleList(doc.Styles["List14"], 0, 1, '.');

            Assert.That(paras.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests writing document with ordered lists combined with headings.
        /// </summary>
        [Test]
        public void TestOrderedListI()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListI", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "7.", "Heading 1", "Normal");
            CheckListItem(paras[1], "b\r", 1, "2.", "Normal");
            CheckListItem(paras[2], "c\r", 1, "3.", "Heading 2", "Normal");
            CheckListItem(paras[3], "d\f", 2, "2.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests writing document with ordered lists nested in Quotes.
        /// </summary>
        [Test]
        public void TestOrderedListJ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListJ", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "a\r", 0, "1.", "Normal");
            CheckListItem(paras[1], "b\r", 1, "1.", "Quote", "Normal");
            CheckListItem(paras[2], "c\f", 1, "1.", "List", "Quote", "Normal");

            CheckStyleList(doc.Styles["List"], 0, 1, '.');

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests writing document with ordered lists nested in Quotes and vise versa.
        /// </summary>
        [Test]
        public void TestOrderedListK()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListK", UnifiedScenario.Docx2Md);
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
        /// Tests writing document with Quote inside ordered list.
        /// </summary>
        [Test]
        public void TestOrderedListL()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListL", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[1], "A list item with a blockquote:\r", 0, "5.", "Normal");
            CheckListItem(paras[2], "This is a blockquote\vinside a list item.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests writing document with ordered lists and indented code inside.
        /// </summary>
        [Test]
        public void TestOrderedListM()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListM", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckParagraph(paras[0], "To put a code block within a list item\r", "Normal");
            CheckListItem(paras[1], "A list item with a code block:\r", 0, "7.", "Normal");
            CheckListItem(paras[2], "<code goes here>\f", 0, " ", "IndentedCode");

            Assert.That(paras.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests writing document with ordered lists and horizontal rule.
        /// </summary>
        [Test]
        public void TestOrderedListN()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListN", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckHorizontalRule(paras[0], "Quote", "Normal");
            CheckListItem(paras[0], "\r", 0, "4.", "Quote", "Normal");
            CheckListItem(paras[1], "aliqua.\f", 0, " ", "Quote", "Normal");

            Assert.That(paras.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests writing a very complex markdown document with ordered lists.
        /// </summary>
        [Test]
        public void TestOrderedListO()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListO", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckListItem(paras[0], "Lorem\r", 0, "2.", "Normal");
            CheckListItem(paras[1], "ipsum\r", 0, "4)", "Normal");
            CheckListItem(paras[2], "dolor\r", 0, "6.", "Normal");
            CheckListItem(paras[3], "sit\r", 1, "1.", "Normal");
            CheckListItem(paras[4], "amet\r", 1, "3)", "Normal");
            CheckListItem(paras[5], "consectetur\r", 2, "", "Normal");
            CheckListItem(paras[6], "adipiscing\velit\r", 2, "25.", "Normal");
            CheckListItem(paras[7], "sed\r", 2, " ", "Heading 1", "Normal");
            CheckListItem(paras[8], "do\r", 0, " ", "SetextHeading2", "Heading 2", "Normal");
            CheckHorizontalRule(paras[9], "Normal");
            CheckListItem(paras[9], "\r", 0, " ", "Normal");
            CheckListItem(paras[10], "tabs indentation\veiusmod\r", 0, " ", "IndentedCode");
            CheckListItem(paras[11], "tempor\r", 1, "1.", "Quote", "Normal");
            CheckListItem(paras[12], "incididunt\r", 1, "1)", "Quote2", "Quote1", "Quote", "Normal");
            CheckListItem(paras[13], "ut\r", 1, "1)", "Quote1", "Quote", "Normal");
            CheckListItem(paras[14], "labore\v1.et\r", 0, "7.", "Normal");
            CheckParagraph(paras[15], "1.dolore\r", "Normal");
            CheckHorizontalRule(paras[16], "Quote", "Normal");
            CheckListItem(paras[16], "\r", 0, "1.", "Quote");
            CheckListItem(paras[17], "aliqua.\r", 0, " ", "Quote");
            CheckListItem(paras[18], "Ut\v1. enim\r", 1, "1.", "Quote");
            CheckParagraph(paras[19], "1\\.\tad\r", "IndentedCode");
            CheckListItem(paras[20], "minim\r", 0, "1.", "Normal");
            CheckListItem(paras[21], "veniam\r", 2, "1.", "Quote4", "List1", "Quote3", "List", "Quote", "Normal");
            CheckListItem(paras[22], "quis\r", 0, "1.", "Normal");
            CheckListItem(paras[23], "nostrud\r", 0, " ", "Normal");
            CheckListItem(paras[24], "exercitation\v1.         foo\f", 1, "1.", "Normal");

            Assert.That(paras.Count, Is.EqualTo(25));
        }

        /// <summary>
        /// Tests that a new list is started when it was broken with a regular paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListP()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListP", UnifiedScenario.Docx2Md);
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
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestOrderedListQ", UnifiedScenario.Docx2Md);
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
        /// Tests writing a table.
        /// </summary>
        [Test]
        public void TestTableA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestTableA", UnifiedScenario.Docx2Md);
            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(1));

            RowCollection rows = tables[0].Rows;
            Assert.That(rows.Count, Is.EqualTo(6));

            CellCollection cells = rows[1].Cells;
            Assert.That(cells.Count, Is.EqualTo(3));

            Cell cell = cells[0];
            Paragraph paragraph = cell.FirstParagraph;
            Assert.That(paragraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Run run = GetRunWithText(paragraph, "First");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            run = GetRunWithText(paragraph, "cell");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            cell = cells[1];
            paragraph = cell.FirstParagraph;
            Assert.That(paragraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Left));
            run = GetRunWithText(paragraph, "Second");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            run = GetRunWithText(paragraph, "cell");
            Assert.That(run.RunPr.StrikeThrough, Is.EqualTo(AttrBoolEx.False));

            cell = cells[2];
            paragraph = cell.FirstParagraph;
            Assert.That(paragraph.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Right));
            run = GetRunWithText(paragraph, "Third");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo(MarkdownUtil.InlineCodeStyleName));
            run = GetRunWithText(paragraph, "cell");
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));

            paragraph = (Paragraph)tables[0].NextSibling;
            CheckParagraph(paragraph, "paragraph after table\f", "Normal");
        }

        /// <summary>
        /// Tests writing a table with multiline cells.
        /// </summary>
        /// <remarks>
        /// Until inline html blocks are implemented, multiline cells are loaded as a single paragraph.
        /// </remarks>
        [Test]
        public void TestTableB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestTableB", UnifiedScenario.Docx2Md);

            Table table = doc.FirstSection.Body.Tables[0];

            Paragraph paragraph = table.FirstRow.FirstCell.FirstParagraph;
            CheckParagraph(paragraph, "Cell1", "Normal");
        }

        /// <summary>
        /// Tests writing a table with merged cells.
        /// </summary>
        /// <remarks>
        /// Note, that neither standard, nor extended markdown https://github.github.com/gfm/ support merged cells.
        /// So, for a moment it just exports all cells 'as is' not considering merged cells.
        /// Also, the delimiter row is always taken from the very first (header) row.
        /// So, despite all cells are exported, they can be non-displaying in markdown editors.
        /// </remarks>
        [Test]
        public void TestTableC()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestTableC", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests simple Autolink.
        /// </summary>
        [Test]
        public void TestAutolinkA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkA", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests simple Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkB", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(6));
        }

        /// <summary>
        /// Tests simple Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkC()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkC", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests Autolinks with Whitespace characters.
        /// </summary>
        [Test]
        public void TestAutolinkD()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkD", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }


        /// <summary>
        /// Tests Autolink without colon and commercial "At".
        /// </summary>
        [Test]
        public void TestAutolinkF()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkF", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests Autolink with colon and backslash not in the schema name.
        /// </summary>
        [Test]
        public void TestAutolinkG()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkG", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests Autolink with commercial "At" and backslash.
        /// </summary>
        [Test]
        public void TestAutolinkH()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkH", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with incorrect length.
        /// </summary>
        [Test]
        public void TestAutolinkI()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkI", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with wrong first symbol (must be [a-zA-Z]).
        /// </summary>
        [Test]
        public void TestAutolinkJ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkJ", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests Autolink with colon and schema name with wrong not-first symbol (must be [a-zA-Z0-9.+-]).
        /// </summary>
        [Test]
        public void TestAutolinkK()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkK", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests Autolink with commercial "At" and incorrect symbols.
        /// </summary>
        [Test]
        public void TestAutolinkL()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkL", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests Autolink with colon and any unexpected symbol after the schema name.
        /// </summary>
        [Test]
        public void TestAutolinkM()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkM", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests nested Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkN()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkN", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(6));
        }

        /// <summary>
        /// Tests the precedence of Autolinks over emphasis delimiters.
        /// </summary>
        [Test]
        public void TestAutolinkO()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkO", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(7));
        }


        /// <summary>
        /// Tests hyperlinks that are formatted or not Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkQ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkQ", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(7));
        }

        /// <summary>
        /// Tests non-Autolinks.
        /// </summary>
        [Test]
        public void TestAutolinkR()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkR", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests Autolink with bookmark.
        /// </summary>
        [Test]
        public void TestAutolinkS()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkS", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests Autolink represented with a sequence of runs.
        /// </summary>
        [Test]
        public void TestAutolinkT()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkT", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that code spans and Autolinks have the same precedence.
        /// </summary>
        [Test]
        public void TestAutolinkU()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkU", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests Autolinks with closer angle brackets.
        /// </summary>
        [Test]
        public void TestAutolinkV()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkV", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that backslash-escaped left angle bracket is not valid for Autolink.
        /// </summary>
        [Test]
        public void TestAutolinkW()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestAutolinkW", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests simple Link.
        /// </summary>
        [Test]
        public void TestLinkA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkA", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests simple Link with a title.
        /// </summary>
        [Test]
        public void TestLinkB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkB", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
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
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkC", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
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
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkD", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests Link titles with different quotes.
        /// </summary>
        [Test]
        public void TestLinkE()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestLinkE", LoadFormat.Docx);
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Inline;
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\TestLinkE.docx.md", so);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests Link with angle brackets for link URI.
        /// </summary>
        [Test]
        public void TestLinkF()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkF", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests Link titles with backslash-escaped quotes.
        /// </summary>
        /// <remarks>
        /// NOTE: For now the 'gold' file is incorrect.
        /// FieldCodeHyperlink.Parse() cannot understand double quotes and backslashes
        /// in the screen tip, if they are not escaped. Backslashes are thrown out.
        /// The screen tip is cut at the first unescaped double quote.
        /// </remarks>
        [Test]
        public void TestLinkG()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestLinkG", LoadFormat.Docx);
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Inline;
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\TestLinkG.docx.md", so);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(13));
        }

        /// <summary>
        /// Tests that the Link destination cannot contain spaces or line breaks, even if enclosed in pointy brackets.
        /// </summary>
        [Test]
        public void TestLinkH()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkH", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests that parentheses inside the Link destination may be escaped.
        /// </summary>
        [Test]
        public void TestLinkI()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkI", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that any number of parentheses in the Link target are allowed without escaping,
        /// as long as they are balanced.
        /// </summary>
        [Test]
        public void TestLinkJ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkJ", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that if Link have unbalanced parentheses,
        /// they need to be escaped or used with the angle brackets.
        /// </summary>
        [Test]
        public void TestLinkK()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkK", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that parentheses and other symbols in the Link can also be escaped, as usual in Markdown.
        /// </summary>
        [Test]
        public void TestLinkL()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkL", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that Links can contain fragment identifiers and queries.
        /// </summary>
        [Test]
        public void TestLinkM()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkM", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that a backslash before a non-escapable character in the Link is just a backslash.
        /// </summary>
        /// <remarks>
        /// NOTE: For now the 'gold' file is incorrect.
        /// FieldCodeHyperlink.Parse() cannot understand double quotes and backslashes
        /// in the screen tip, if they are not escaped. Backslashes are thrown out.
        /// The screen tip is cut at the first unescaped double quote.
        /// </remarks>
        [Test]
        public void TestLinkN()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkN", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that URL-escaping should be left alone inside the Link destination,
        /// as all URL-escaped characters are also valid URL characters.
        /// </summary>
        [Test]
        public void TestLinkO()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkO", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that, because titles can often be parsed as Link destinations,
        /// if you try to omit the destination and keep the title, you’ll get unexpected results.
        /// </summary>
        [Test]
        public void TestLinkP()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkP", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that backslash escapes and entity and numeric character references may be used in Link titles.
        /// </summary>
        [Test]
        public void TestLinkQ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkQ", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that whitespace is allowed around the Link destination and title.
        /// </summary>
        [Test]
        public void TestLinkR()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkR", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }


        /// <summary>
        /// Tests that the Link text may contain balanced brackets, but not unbalanced ones, unless they are escaped.
        /// </summary>
        [Test]
        public void TestLinkT()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkT", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests that the Link text may contain inline content.
        /// </summary>
        [Test]
        public void TestLinkU()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkU", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that Links may not contain other Links, at any level of nesting.
        /// </summary>
        [Test]
        public void TestLinkV()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkV", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests the precedence of Link text grouping over emphasis grouping.
        /// </summary>
        [Test]
        public void TestLinkW()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkW", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that brackets that aren’t part of Links do not take precedence.
        /// </summary>
        [Test]
        public void TestLinkX()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkX", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests the precedence of HTML tags, code spans, and autolinks over Link grouping.
        /// </summary>
        /// <remarks>
        /// TODO: HTML tags are still not implemented.
        /// </remarks>
        [Test]
        public void TestLinkY()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkY", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that a Link destination consists of a sequence of characters between angle brackets that contains
        /// no unescaped angle brackets.
        /// </summary>
        [Test]
        public void TestLinkZ01()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkZ01", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(8));
        }

        /// <summary>
        /// Tests that a Link titles may span multiple lines.
        /// </summary>
        [Test]
        public void TestLinkZ02()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkZ02", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests an empty Link title.
        /// </summary>
        [Test]
        public void TestLinkZ03()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkZ03", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests edge conditions during the Link parsing.
        /// </summary>
        [Test]
        public void TestLinkZ04()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkZ04", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(6));
        }

        /// <summary>
        /// Tests that backslash-escaped left square bracket is not valid for Link.
        /// </summary>
        [Test]
        public void TestLinkZ05()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkZ05", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that Link URI cannot start with the left angle bracket
        /// without the right angle bracket at the very end.
        /// </summary>
        [Test]
        public void TestLinkZ06()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestLinkZ06", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests simple Image.
        /// </summary>
        [Test]
        public void TestImageA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestImageA", UnifiedScenario.Docx2Md);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));
            CheckShape(paras[0].GetChild(NodeType.Any, 0, false) as Shape, "alt text", "/uri");
        }

        /// <summary>
        /// Tests Image with a title.
        /// </summary>
        [Test]
        public void TestImageB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestImageB", UnifiedScenario.Docx2Md);
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
            Document doc = TestUtil.Open(@"ExportMarkdown\TestImageC", LoadFormat.Docx);
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Inline;
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\TestImageC.docx.md", so);

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
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestImageE", UnifiedScenario.Docx2Md);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));
            CheckShape(paras[0].GetChild(NodeType.Any, 0, false) as Shape, string.Empty, "/url");
        }

        // FOSS TestImageF removed: verifies a written PNG hash and image source name, but the
        // FOSS build does not serialize images to Markdown.

        /// <summary>
        /// Tests a simple Footnote Reference.
        /// </summary>
        [Test]
        public void TestFootnoteA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteA", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            CheckFootnotes(doc, "\u0002 Some footnote text.\r");
        }

        /// <summary>
        /// Tests a Footnote with multiline definition.
        /// </summary>
        [Test]
        public void TestFootnoteB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteB", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            CheckFootnotes(doc, "\u0002 Line1.\rLine2.\r");
        }

        /// <summary>
        /// Tests Footnote with custom reference.
        /// </summary>
        [Test]
        public void TestFootnoteC()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteC", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            CheckFootnotes(doc, "\u0002 Custom reference definition.\r");
        }

        /// <summary>
        /// Tests Footnote with an empty definition.
        /// </summary>
        [Test]
        public void TestFootnoteD()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteD", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            CheckFootnotes(doc, "\u0002\r");
        }

        /// <summary>
        /// Tests Footnote with multiple paragraphs.
        /// </summary>
        [Test]
        public void TestFootnoteE()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteE", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            CheckFootnotes(doc, "\u0002 Paragraph1\rParagraph2.\r");
        }

        /// <summary>
        /// Tests various emphases in Footnotes.
        /// </summary>
        [Test]
        public void TestFootnoteF()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteF", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            NodeCollection footnotes = CheckFootnotes(doc, "\u0002 Some bold footnote text.\r", "\u0002 Some italic footnote\r");

            Assert.That(((Footnote)footnotes[0]).Font.Bold, Is.True);
            Assert.That(((Footnote)footnotes[1]).Font.Italic, Is.False);

            Paragraph paragraph = ((Footnote)footnotes[0]).FirstParagraph;
            CheckRun(paragraph.Runs[1], "Some ", false, false);
            CheckRun(paragraph.Runs[2], "bold footnote", true, false);
            CheckRun(paragraph.Runs[3], " text.", false, false);

            paragraph = ((Footnote)footnotes[1]).FirstParagraph;
            CheckRun(paragraph.Runs[1], "Some ", false, false);
            CheckRun(paragraph.Runs[2], "italic footnote", false, true);
        }

        /// <summary>
        /// Tests IndentedCode in footnote definition.
        /// </summary>
        [Test]
        public void TestFootnoteG()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestFootnoteG", UnifiedScenario.Docx2Md);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            NodeCollection footnotes =
                CheckFootnotes(doc, "\u0002 Some footnote text (para1).\r    Some indented footnote text (para2).\r");

            CheckParagraph(((Footnote)footnotes[0]).Paragraphs[0], "\u0002 Some footnote text (para1).\r", "Footnote Text");
            CheckParagraph(((Footnote)footnotes[0]).Paragraphs[1], "    Some indented footnote text (para2).",
                "Footnote Text1", "IndentedCode");
        }

        /// <summary>
        /// Tests various markdown features in a large file.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Docx2Md)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestLargeDocument(UnifiedScenario scenario)
        {
            string fileName = string.Format(@"ImportMarkdown\FormatDetector\LargeFile{0}",
                (TestUtil.GetLoadFormat(scenario) == LoadFormat.Markdown) ? ".mrdwn" : ".docx");

            Document doc = TestUtil.Open(fileName);
            doc.UpdateListLabels();

            CheckBulletListItems(doc);
            CheckOrderedListItems(doc);
            CheckHeadings(doc);
            CheckQuotes(doc);
            CheckHorizontalRules(doc);
            CheckEmphases(doc);
            CheckAutolinks(doc);
            CheckLinks(doc);
            CheckImages(doc);

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Auto;
            // Check gold.
            TestUtil.SaveOpen(doc, @"ImportMarkdown\FormatDetector\LargeFile", scenario, so);
        }

        /// <summary>
        /// WORDSNET-19836 Fails insert list during saving to existing .md document.
        /// Fixed through the implementation of resilient writing.
        /// </summary>
        [Test]
        public void Test19836()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            builder.Writeln("Paragraph before the list.");
            builder.ListFormat.ApplyNumberDefault();
            builder.ListFormat.List.ListLevels[0].NumberFormat = "-";
            builder.Writeln("Text_1");
            builder.Writeln("Text_2");
            builder.ListFormat.ListIndent();
            builder.Writeln("Text_2.1");
            builder.Writeln("Text_2.2");

            Document doc = TestUtil.SaveOpen(builder.Document, @"ExportMarkdown\Test19836",
                UnifiedScenario.Md2Md | UnifiedScenario.NoGold);

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// WORDSNET-19836 DOCX to Markdown conversion issue with numbered lists.
        /// Supported markdown ordered lists.
        /// </summary>
        [Test]
        public void Test19812()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test19812", UnifiedScenario.Docx2Md);
            doc.UpdateListLabels();

            Paragraph paragraph = GetParagraphWithText(doc, "Item1");
            CheckListItem(paragraph, 0, "1.", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Item2", 0, "2.", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Item3", 0, "3.", "Normal");
        }

        /// <summary>
        /// WORDSNET-20353 ArgumentOutOfRangeException is thrown while exporting document
        /// with multiple numbered paragraphs inside a cell into Markdown.
        ///
        /// The wrong previous paragraph was used while processing multiple paragraphs inside a cell.
        /// </summary>
        [Test]
        public void Test20353()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20353", UnifiedScenario.Docx2Md);

            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.FirstRow.FirstCell.FirstParagraph.GetText(), Is.EqualTo("- Line1\r"));
        }

        /// <summary>
        /// WORDSNET-20165 Bold and italic text is not properly converted to Markdown.
        /// The emphases should be moved out of adjacent whitespace characters to make a correct
        /// flanking type in output result. Also leading and trailing whitespace characters should
        /// be wrapped into InlineCode to be preserved in markdown.
        /// </summary>
        [Test]
        public void Test20165A()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20165A", UnifiedScenario.Docx2Md);
            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.That(paragraphs[0].GetText(), Is.EqualTo("   Foo\r"));
            Run run = GetRunWithText(paragraphs[0], "Foo");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            Assert.That(paragraphs[1].GetText(), Is.EqualTo("Bar\r"));
            run = GetRunWithText(paragraphs[1], "Bar");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            Assert.That(paragraphs[2].GetText(), Is.EqualTo("   Baz\r"));
            run = GetRunWithText(paragraphs[2], "Baz");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            Assert.That(paragraphs[3].GetText(), Is.EqualTo("Bop\f"));
            run = GetRunWithText(paragraphs[3], "Bop");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Relates to WORDSNET-20165
        /// Tests that leading whitespace characters does not wrap into InlineCode delimiters
        /// when they are already formatted as InlineCode.
        /// </summary>
        [Test]
        public void Test20165B()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20165B", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("   InlineCode text + bold + italic\r"));
            CheckRun(paragraph.FirstRun, "   InlineCode text + bold + italic", "InlineCode", true, true);

            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(paragraph.GetText(), Is.EqualTo("   InlineCode text\r"));
            CheckRun(paragraph.FirstRun, "   InlineCode text", "InlineCode", false, false);

            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(paragraph.GetText(), Is.EqualTo("   Regular text\f"));
            CheckRun(paragraph.Runs[0], "   ", "InlineCode", false, false);
            CheckRun(paragraph.Runs[1], "Regular text", "Default Paragraph Font", false, false);
        }

        /// <summary>
        /// Relates to WORDSNET-20165
        /// Tests that leading whitespace characters formatted as InlineCode
        /// with more than one delimiter is exported correctly.
        /// </summary>
        [Test]
        public void Test20165C()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20165C", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("   InlineCode.3 + bold + italic\r"));
            CheckRun(paragraph.FirstRun, "   InlineCode.3 + bold + italic", "InlineCode.3", true, true);
        }

        /// <summary>
        /// Relates to WORDSNET-20165
        /// Tests that emphases in the middle of the text are moved correctly out of whitespace characters
        /// to make a correct flanking type.
        /// </summary>
        [Test]
        public void Test20165D()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20165D", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("Foo: bar\f"));
            CheckRun(paragraph.Runs[0], "Foo:", true, true);
            CheckRun(paragraph.Runs[1], " bar", false, false);
        }


        /// <summary>
        /// Relates to WORDSNET-20165
        /// Tests that if character of text is InlineCode delimiter and it is adjacent to InlineCode delimiters,
        /// then this character should be escaped with space.
        /// </summary>
        [Test]
        public void Test20165E()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20165E", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("To indicate a span of code, wrap it with backtick quotes (`).\f"));
            CheckRun(paragraph.Runs[1], "`", "InlineCode.3", true, false);
        }

        /// <summary>
        /// WORDSNET-20393 Incorrect export of nested tables into markdown.
        /// A parent table should be split for inserting nested table in markdown.
        /// </summary>
        [Test]
        public void Test20393A()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20393A", UnifiedScenario.Docx2Md);
            TableCollection tables = doc.FirstSection.Body.Tables;

            // The original table and its nested table is split onto 3 separate tables:
            Assert.That(tables[0].GetText(), Is.EqualTo("Parent cell para1\a\a"));
            Assert.That(tables[1].GetText(), Is.EqualTo("Nested cell1A\aNested cell1B\aNested cell1C\aNested cell1D\a\a" +
                            "Nested cell2A\aNested cell2B\aNested cell2C\aNested cell2D\a\a"));
            Assert.That(tables[2].GetText(), Is.EqualTo("Parent cell para2\a\a"));
            Assert.That(tables.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Relates to WORDSNET-20393
        /// Tests complex tables nesting.
        /// </summary>
        [Test]
        public void Test20393B()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20393B", UnifiedScenario.Docx2Md);
            Assert.That(doc.GetChildNodes(NodeType.Table, true).Count, Is.EqualTo(17));
        }

        /// <summary>
        /// WORDSNET-20394 Content lines with only whitespace characters are incorrectly escaped
        /// while exporting into markdown.
        /// Content lines with only whitespace characters should not be escaped while writing markdown paragraphs.
        /// </summary>
        [Test]
        public void Test20394()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20394",
                UnifiedScenario.Docx2Md | UnifiedScenario.NoGold);

            Table table = (Table)doc.FirstSection.Body.FirstChild;
            CellCollection cells = table.Rows[1].Cells;

            Assert.That(cells[0].GetText(), Is.EqualTo("\a"));
            Assert.That(cells[1].GetText(), Is.EqualTo("B2\a"));
            Assert.That(cells[2].GetText(), Is.EqualTo("\a"));
            Assert.That(cells[3].GetText(), Is.EqualTo("D2\a"));
        }

        /// <summary>
        /// WORDSNET-20407 Incorrect export of Headings inside table into Markdown.
        /// The Headings should be exported as HTML 'h1'-'h6' tags.
        /// </summary>
        [Test]
        public void Test20407()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20407", UnifiedScenario.Docx2Md);
            Table table = (Table)doc.FirstSection.Body.FirstChild;

            Row row = table.Rows[0];
            Assert.That(row.Cells[0].GetText(), Is.EqualTo("Foo\rBar\rB***o***z\rBop\r\r1. ListItem\r\rBem\a"));

            row = table.Rows[1];
            Assert.That(row.Cells[0].GetText(), Is.EqualTo("Lorem\a"));
            Assert.That(row.Cells[1].GetText(), Is.EqualTo("Dolor\a"));

            row = table.Rows[2];
            Assert.That(row.Cells[0].GetText(), Is.EqualTo("1. ListItem\a"));
            Assert.That(row.Cells[1].GetText(), Is.EqualTo("- Bullet1\a"));

            row = table.Rows[3];
            Assert.That(row.Cells[0].GetText(), Is.EqualTo("# # Not a heading\a"));
        }

        /// <summary>
        /// WORDSNET-20408 Wrong output of paragraphs started with ordered list markers inside table
        /// while exporting into Markdown.
        /// We should not escape anything inside table as there are no any other features allowed inside it.
        /// </summary>
        [Test]
        public void Test20408()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20408", UnifiedScenario.Docx2Md);

            Table table = (Table)doc.FirstSection.Body.FirstChild;
            Row row = table.Rows[0];

            Assert.That(row.Cells[0].GetText(), Is.EqualTo("1. Not a ListItem1\r2. Not a ListItem2\a"));
            Assert.That(row.Cells[1].GetText(), Is.EqualTo("1. Not a ListItem3\a"));
        }

        /// <summary>
        /// WORDSNET-20409 List labels are not preserved while exporting numbered paragraphs into Markdown.
        /// Lists are not allowed inside tables, so numbered paragraphs should be saved with their updated list labels.
        /// </summary>
        [Test]
        public void Test20409()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test20409", UnifiedScenario.Docx2Md);

            Table table = (Table)doc.FirstSection.Body.FirstChild;
            Row row = table.Rows[0];
            Assert.That(row.Cells[0].GetText(), Is.EqualTo("1. OrderedItem1\r\u2003a. OrderedItem2\r\u2003b. OrderedItem3\r2. OrderedItem4\a"));
            Assert.That(row.Cells[1].GetText(), Is.EqualTo("- UnorderedItem1\r\u2003- UnorderedItem2\r\u2003\u2003- UnorderedItem3\r- UnorderedItem4\a"));
        }

        /// <summary>
        /// WORDSNET-20297 Add ability to create MarkdownSaveOptions using
        /// SaveOptions.CreateSaveOptions(SaveFormat saveFormat).
        /// Made MarkdownSaveOptions class public.
        /// </summary>
        [Test]
        public void Test20297()
        {
            MarkdownSaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Markdown) as MarkdownSaveOptions;
            Assert.That(so, IsNot.Null());

#if !JAVA && !CPLUSPLUS
            Assert.That(typeof(MarkdownSaveOptions).IsPublic, Is.True);
#endif
        }

        /// <summary>
        /// WORDSNET-20425 Add the ability to control the alignment of table contents
        /// when exporting to the Markdown.
        /// Implemented new save option <see cref="TableContentAlignment"/>.
        /// </summary>
        [TestCase(TableContentAlignment.Auto,
            new ParagraphAlignment[] {ParagraphAlignment.Left, ParagraphAlignment.Center, ParagraphAlignment.Right})]
        [TestCase(TableContentAlignment.Left,
            new ParagraphAlignment[] {ParagraphAlignment.Left, ParagraphAlignment.Left, ParagraphAlignment.Left})]
        [TestCase(TableContentAlignment.Center,
            new ParagraphAlignment[] {ParagraphAlignment.Center, ParagraphAlignment.Center, ParagraphAlignment.Center})]
        [TestCase(TableContentAlignment.Right,
            new ParagraphAlignment[] {ParagraphAlignment.Right, ParagraphAlignment.Right, ParagraphAlignment.Right})]
        public void Test20425(TableContentAlignment tableContentAlignment, ParagraphAlignment[] expectedAlignments)
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Customers\Test20425.docx");

            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions();
            saveOptions.TableContentAlignment = tableContentAlignment;
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\Customers\Test20425",
                UnifiedScenario.Docx2Md | UnifiedScenario.NoGold, saveOptions);

            CellCollection cells = ((Table)doc.FirstSection.Body.FirstChild).FirstRow.Cells;

            Assert.That(cells[0].FirstParagraph.ParaPr.Alignment, Is.EqualTo(expectedAlignments[0]));
            Assert.That(cells[1].FirstParagraph.ParaPr.Alignment, Is.EqualTo(expectedAlignments[1]));
            Assert.That(cells[2].FirstParagraph.ParaPr.Alignment, Is.EqualTo(expectedAlignments[2]));
        }

        /// <summary>
        /// WORDSNET-20879 System.InvalidOperationException when save document to markdown format.
        /// Fixed by adding the ImagesFolder property to the MarkdownSaveOptions class.
        /// </summary>
        [Test]
        [Ignore("FOSS: pending MarkdownImageWriter image-saving restoration. Image files are no longer written to ImagesFolder because ImageShapeWriter was removed with the rendering layer in commit e515d42c2a9, so Directory.GetFiles returns an empty array.")]
        public void Test20879()
        {
            DocumentBuilder builder = new DocumentBuilder();

            // Inserting the image.
            using (Stream srcImageStream = ResourceUtil.FetchResourceStream("Aspose.Words.Resources.3dBubble.jpg"))
                builder.InsertImage(srcImageStream);

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.ImagesFolder = TestUtil.GetInTestOutPath(@"ExportMarkdown\Customers\Images20879");

            // If the image exists before saving, delete it.
            if (Directory.Exists(so.ImagesFolder))
                Directory.Delete(so.ImagesFolder, true);

            // Creating the image directory.
            Directory.CreateDirectory(so.ImagesFolder);

            // Saving the document.
            using (MemoryStream stream = new MemoryStream())
                builder.Document.Save(stream, so);

            // Checking the image after saving.
            string imageFileName = Directory.GetFiles(so.ImagesFolder)[0];
#if JAVA
            TestUtil.VerifyHash("3307863FCBEA17ECAF59A9EC2F2A6625", StreamUtil.CopyFileToByteArray(imageFileName));
#elif NETSTANDARD
            TestUtil.VerifyHash("0464E80A08EF910EBDD64919129073A5", StreamUtil.CopyFileToByteArray(imageFileName));
#else
            TestUtil.VerifyHash("A9C1A23ABF020790B2702162D91ADECE", StreamUtil.CopyFileToByteArray(imageFileName));
#endif
        }

        /// <summary>
        /// Related to WORDSNET-20879
        /// Checks for an exception when saving a stream without the specified ImagesFolder.
        /// </summary>
        [Test]
        // FOSS we don't write images in the FOSS version hence no exception is thrown.
        public void Test20879_2()
        {
            DocumentBuilder builder = new DocumentBuilder();
            using (Stream srcImageStream = ResourceUtil.FetchResourceStream("Aspose.Words.Resources.3dBubble.jpg"))
                builder.InsertImage(srcImageStream);
            using (MemoryStream stream = new MemoryStream())
                builder.Document.Save(stream, SaveFormat.Markdown);
        }

        /// <summary>
        /// WORDSNET-21681 Part of text is lost during DOCX-to-Markdown conversion.
        /// The document contains footnotes that means there are nested paragraphs.
        /// We need a stack of started paragraphs to write them properly in case of nesting.
        /// </summary>
        [Test]
        public void Test21681()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test21681",
                UnifiedScenario.Docx2Md | UnifiedScenario.NoGold);

            // Check the problematic text is not lost.
            Assert.That(GetParagraphWithText(doc, "Text is normal, then bold,"), IsNot.Null());
            Assert.That(GetParagraphWithText(doc, "This text is usual, but"), IsNot.Null());

            CheckFootnotes(doc,
                "\u0002 Standard footnote on page #1.\r",
                "\u0002 See \u0013 HYPERLINK \"https://www.techrepublic.com/blog/microsoft-office/highlight-text-with-the" +
                "-gradient-fill-effect-in-word/\" \u0014https://www.techrepublic.com/blog/microsoft-office/highlight-text" +
                "-with-the-gradient-fill-effect-in-word/\u0015\r");
        }

        /// <summary>
        /// WORDSNET-21680 Missed footnotes during conversion between DOCX and Markdown.
        /// Implemented the Markdown footnotes.
        /// </summary>
        [Test]
        public void Test21680()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Customers\Test21680", UnifiedScenario.Docx2Md);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(25));

            CheckFootnotes(doc,
                "\u0002 1st pagenote in 4th paragraph on the 1st page.\r",
                "\u0002 1st endnote just after 1st pagenote on the 1st page.\r",
                "\u0002 2nd pagenote on the empty paragraph on the 1st page.\r",
                "\u0002 3rd pagenote in 5th paragraph, before the first word of a first letter on the 1st page.\r",
                "\u0002 2nd endnote on the 1st page.\r",
                "\u0002 4th pagenote on the 1st page.\r",
                "\u0002 3rd endnote just before 5th pagenote on the 2nd page.\r",
                "\u0002 5th pagenote on the 2nd page.\r");
        }

        /// <summary>
        /// WORDSNET-22787 Introduce ExportImagesAsBase64 option for MD format.
        /// ExportImagesAsBase64 markdown save option has been implemented.
        /// The case when SourceFileName is empty and ImageBytes is not empty.
        /// </summary>
        [TestCase(true, "Test22787.md")]
        [TestCase(false, "Test22787_Linked.md")]
        public void Test22787(bool exportImagesAsBase64, string filename)
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Test22787.docx");

            MarkdownSaveOptions mso = new MarkdownSaveOptions();
            mso.ExportImagesAsBase64 = exportImagesAsBase64;

            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\" + filename, mso);
        }








        /// <summary>
        /// Tests the roundtrip of the list and that of the escape list.
        /// </summary>
        [TestCase(UnifiedScenario.Md2Docx)]
        [TestCase(UnifiedScenario.Md2Md)]
        public void TestListEscaping(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\TestListEscaping",
                scenario | UnifiedScenario.NoGold);
            doc.UpdateListLabels();
            Paragraph paraListEscaping = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paraListEscaping.IsListItem, Is.False);
            // Checks for "1986." substring and absence of escaping "\.".
            Assert.That(paraListEscaping.FirstRun.Text, Is.EqualTo("1986. What a great season."));

            Paragraph paraList = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(paraList.IsListItem, Is.True);
            // The text of the first run is "What a great season.".
            Assert.That(paraList.FirstRun.Text, Is.EqualTo("What a great season."));
            // Text "1986." gets into ListLabel.
            Assert.That(paraList.ListLabel.LabelString, Is.EqualTo("1986."));
        }

        /// <summary>
        /// Checks resaving markdown reference links with different ExportLinksMode.
        /// </summary>
        [TestCase(MarkdownLinkExportMode.Inline, "_inline")]
        [TestCase(MarkdownLinkExportMode.Reference, "_reference")]
        [TestCase(MarkdownLinkExportMode.Auto, "_auto")]
        public void TestExportLinksMode(MarkdownLinkExportMode linkExportMode, string saveSuffix)
        {
            string[] urls = new string[] { "https://www.w3schools.com/", "https://www.example.com/",
                "https://www.example.com/", "https://www.google.com/" };
            string[] titles = new string[] { "title", "title1", "title1", "title2" };

            Document doc = TestUtil.Open(@"ExportMarkdown\TestExportLinksMode.md");

            FieldCollection fields = doc.FirstSection.Body.Range.Fields;
            Assert.That(fields.Count, Is.EqualTo(4));
            for (int i = 0; i < 4; i++)
            {
                FieldHyperlink field = fields[i] as FieldHyperlink;
                Assert.That(field.HRef.ToString(false), Is.EqualTo(urls[i]));
                Assert.That(field.ScreenTip, Is.EqualTo(titles[i]));
            }

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = linkExportMode;
            TestUtil.SaveCheckGold(doc, string.Format(@"ExportMarkdown\TestExportLinksMode_{0}.md", saveSuffix), so);
        }

        /// <summary>
        /// Exporting a reference link moves definitions to the end of a document.
        /// </summary>
        [Test]
        public void TestExportLinkPosition()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestLinkPosition.md");
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestLinkPosition.md", so);
        }

        /// <summary>
        /// Exported collapsed reference links become shortcuts.
        /// </summary>
        [Test]
        public void TestExportCollapsedLink()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestExportCollapsedLink.md");
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestExportCollapsedLink.md", so);
        }

        /// <summary>
        /// The reference label is the same as the hyperlink display text.
        /// Checks for absence of [text][text] tautology.
        /// </summary>
        [Test]
        public void TestExportDoubleLinkText()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestExportDoubleLinkText.docx");

            FieldHyperlink field = doc.Range.Fields[0] as FieldHyperlink;
            Assert.That(field.DocLocation, Is.EqualTo("[text]"));
            Assert.That(field.Result, Is.EqualTo("text"));

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestExportDoubleLinkText.md", so);
        }

        /// <summary>
        /// DocLocation contains non-roundtrip information.
        /// </summary>
        [Test]
        public void TestExportRealDocLocation()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestExportRealDocLocation.docx");
            // DocLocation contains a non-reference link label.
            Assert.That((doc.Range.Fields[0] as FieldHyperlink).DocLocation, Is.EqualTo("Footer"));

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            // Checking the gold. Markdown does not store regular DocLocation.
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestExportRealDocLocation.md", so);
        }




        /// <summary>
        /// Relates to WORDSNET-24968
        /// Tests the export of superscript and subscript emphasis.
        /// </summary>
        [Test]
        public void TestVerticalAlignedEmphases()
        {
            TestUtil.OpenSaveOpen(@"ExportMarkdown\TestVerticalAlignedEmphases", UnifiedScenario.Docx2Md);
        }



        /// <summary>
        /// Checks resaving markdown reference images with different ExportLinksMode and ExportImagesAsBase64 options.
        /// </summary>
        [TestCase(MarkdownLinkExportMode.Inline, "_inline")]
        [TestCase(MarkdownLinkExportMode.Reference, "_reference")]
        [TestCase(MarkdownLinkExportMode.Auto, "_auto")]
        public void TestExportImageReferenceLinksMode(MarkdownLinkExportMode linkExportMode, string saveSuffix)
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestImageReferenceLinksMode.md");

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = linkExportMode;
            so.ExportImagesAsBase64 = false;
            string outputName = string.Format(@"ExportMarkdown\TestImageReferenceLinksMode{0}.md", saveSuffix);
            TestUtil.SaveCheckGold(doc, outputName, so);

            so.ExportImagesAsBase64 = true;
            string outputNameBase64 = string.Format(@"ExportMarkdown\TestImageReferenceLinksMode{0}_base64.md", saveSuffix);
            TestUtil.SaveCheckGold(doc, outputNameBase64, so);
        }

        /// <summary>
        /// Exporting a reference images moves definitions to the end of a document.
        /// </summary>
        [Test]
        public void TestExportImageLinkPosition()
        {
            Document doc = TestUtil.Open(@"ImportMarkdown\ReferenceImages\TestRefImageA.md");
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestImageRefPosition.md", so);
        }

        /// <summary>
        /// Exported collapsed reference images become shortcuts.
        /// </summary>
        [Test]
        public void TestExportCollapsedImage()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestCollapsedImageReference.md");
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestCollapsedImageReference.md", so);
        }

        /// <summary>
        /// The reference label is the same as the image alternative text.
        /// Checks for absence of [text][text] tautology.
        /// </summary>
        [Test]
        public void TestExportDoubleImageAltText()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestDoubleImageReference.docx");

            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.Name, Is.EqualTo("[foo]"));
            Assert.That(shape.AlternativeText, Is.EqualTo("foo"));

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestDoubleImageReference.md", so);
        }

        /// <summary>
        /// Tests the case when the shape name contains non-roundtrip information.
        /// </summary>
        [Test]
        public void TestExportRealImageName()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\TestRealImageName.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];
            // Shape.Name contains a non-reference link label.
            Assert.That(shape.Name, Is.EqualTo("Image name"));

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LinkExportMode = MarkdownLinkExportMode.Reference;
            // Checking the gold. Markdown does not store regular Shape.Name.
            TestUtil.SaveCheckGold(doc, @"ExportMarkdown\TestExportRealShapeName.md", so);
        }

        // FOSS TestRenderReferenceImage removed: asserts that identical shapes are rendered into the
        // same Markdown image file, but the FOSS build does not render/serialize images to Markdown.


        /// <summary>
        /// WORDSNET-21376 Support MarkdownSaveOptions.ImagesFolderAlias.
        /// Implemented new save option <see cref="MarkdownSaveOptions.ImagesFolderAlias"/>.
        /// </summary>
        [TestCase("", "empty")]
        [TestCase("aaa", "simple")]
        [TestCase("aaa/", "simple")]
        [TestCase(".\\aaa", "simple")]
        [TestCase(".", "dot")]
        [TestCase("./", "dot")]
        [TestCase(".\\", "dot")]
        [Ignore("FOSS: pending MarkdownImageWriter image-saving restoration. ImageShapeWriter was removed with the rendering layer in commit e515d42c2a9, so the gold compare fails because image lines are missing from the .md output.")]
        public void Test21376(string imagesFolderAlias, string suffix)
        {
            string outFolder = TestUtil.GetInTestOutPath("ExportMarkdown");
            Document doc = TestUtil.Open(@"ExportMarkdown\Test21376.docx");
            MarkdownSaveOptions mdSaveOptions = new MarkdownSaveOptions();
            mdSaveOptions.ImagesFolder = TestUtil.GetInTestOutPath(@"ExportMarkdown\Images21376");
            mdSaveOptions.ImagesFolderAlias = imagesFolderAlias;
            mdSaveOptions.LinkExportMode = MarkdownLinkExportMode.Inline;
            TestUtil.SaveCheckGold(doc, string.Format(@"{0}\Test21376_{1}_inline.md", outFolder, suffix), mdSaveOptions);

            mdSaveOptions.LinkExportMode = MarkdownLinkExportMode.Reference;
            TestUtil.SaveCheckGold(doc, string.Format(@"{0}\Test21376{1}_reference.md", outFolder, suffix), mdSaveOptions);
        }


        /// <summary>
        /// WORDSNET-25997 Consider adding an ability to specify custom formatting tags for basic formatting in Markdown.
        /// Implemented option to control over exporting of underline formatting
        /// <see cref="MarkdownSaveOptions.ExportUnderlineFormatting"/>.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test25997(bool isPreserveUnderline)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Underline = Underline.Single;

            builder.Write("text");

            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions() { ExportUnderlineFormatting = isPreserveUnderline };
            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { ImportUnderlineFormatting = isPreserveUnderline };
            string outName = string.Format(@"ExportMarkdown\Test25997_{0}", isPreserveUnderline);
            Document doc = TestUtil.SaveOpen(builder.Document, outName, UnifiedScenario.Md2Md, saveOptions, loadOptions);

            Underline expectedUnderline = isPreserveUnderline ? Underline.Single : Underline.None;
            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.RunPr.Underline, Is.EqualTo(expectedUnderline));
        }

        /// <summary>
        /// WORDSNET-26148 Consider adding an option to preserve empty paragraphs upon exporting to MD.
        /// The issue is not reproducible, just added the test.
        /// </summary>
        [Test]
        public void Test26148()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Customers\Test26148.docx");

            MarkdownLoadOptions loadOptions = new MarkdownLoadOptions() { PreserveEmptyLines = true };
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\Customers\Test26148", UnifiedScenario.Md2Md, loadOptions);

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(TestImportMarkdown.GetEmptyParagraphsCount(para, false), Is.EqualTo(2));
        }


        /// <summary>
        /// WORDSNET-27382 Consider providing an option to export tables as HTML when saving document to Markdown.
        /// Implemented public <see cref="MarkdownSaveOptions.ExportAsHtml"/> option.
        /// </summary>
        // FOSS Tables-as-HTML export needs the removed HTML writer; keep only the pure-Markdown case.
        [TestCase(MarkdownExportAsHtml.None)]
        public void Test27382(MarkdownExportAsHtml exportAsHtml)
        {
            const string testFile = @"ExportMarkdown\RawHtml\Test27382";
            Document doc = TestUtil.Open(testFile, LoadFormat.Docx);

            MarkdownSaveOptions so = new MarkdownSaveOptions { ExportAsHtml = exportAsHtml };
            doc = TestUtil.SaveOpen(doc, string.Format("{0}_{1}.md", testFile, exportAsHtml.ToString()), so, true);

            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Relates to WORDSNET-27382.
        /// Tests nested tables.
        /// </summary>
        // FOSS Tables-as-HTML export needs the removed HTML writer; keep only the pure-Markdown case.
        [TestCase(MarkdownExportAsHtml.None, 3)]
        public void Test27382Nested(MarkdownExportAsHtml exportAsHtml, int expectedCount)
        {
            const string testFile = @"ExportMarkdown\RawHtml\Test27382Nested";
            Document doc = TestUtil.Open(testFile, LoadFormat.Docx);

            MarkdownSaveOptions so = new MarkdownSaveOptions { ExportAsHtml = exportAsHtml };
            doc = TestUtil.SaveOpen(doc, string.Format("{0}_{1}.md", testFile, exportAsHtml.ToString()), so, true);

            Assert.That(doc.GetChildNodes(NodeType.Table, true).Count, Is.EqualTo(expectedCount));
        }


        /// <summary>
        /// Relates to WORDSNET-27646.
        /// Tests with default value.
        /// </summary>
        [Test]
        public void Test27646_Default()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Test27646_Default",
                UnifiedScenario.Docx2Md | UnifiedScenario.NoGold);

            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.Text, Is.EqualTo("fx=a0+n=1∞ancosnπxL+bnsinnπxL"));
        }

        /// <summary>
        /// WORDSNET-27675 Consider adding property to set image resolution in MarkdownSaveOptions.
        /// Implemented public <see cref="MarkdownSaveOptions.ImageResolution"/> option.
        /// </summary>
        [TestCase(1)]
        [TestCase(100)]
        [TestCase(300)]
        [TestCase(1000)]
        public void Test27675(int resolution)
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Test27675.docx");

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            // Test default value.
            Assert.That(so.ImageResolution, Is.EqualTo(96));

            so.ImageResolution = resolution;

            // FOSS: applying MarkdownSaveOptions.ImageResolution requires image re-encoding, which is
            // unavailable without the image-processing subsystem. Verify the save/open round-trip completes
            // without crashing instead of asserting the resulting image DPI.
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\Test27675.md", so, false);
            Assert.That(doc, Is.Not.Null);
        }

        /// <summary>
        /// Relates to WORDSNET-27675.
        /// Tests values must be positive.
        /// </summary>
        [TestCase(0), ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestCase(-1)]
        public void Test27675A(int resolution)
        {
            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.ImageResolution = resolution;
        }

        /// <summary>
        /// Relates to WORDSNET-27675.
        /// Tests default value.
        /// </summary>
        [Test]
        public void Test27675_Default()
        {
            // The image in test file is actually 300 dpi.
            Document doc = TestUtil.Open(@"ExportMarkdown\Test27675_Default.docx");

            // FOSS: the default image DPI can only be verified by decoding/re-encoding the image, which is
            // unavailable without the image-processing subsystem. Verify the round-trip completes.
            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\Test27675_Default", UnifiedScenario.Md2Md | UnifiedScenario.NoGold);
            Assert.That(doc, Is.Not.Null);
        }

        /// <summary>
        /// WORDSNET-18341 Implement Markdown ‘Line Breaks’ feature.
        /// Implemented <see cref="MarkdownSaveOptions.LineBreakExportMode"/>.
        /// </summary>
        [TestCase(MarkdownLineBreakExportMode.Backslash)]
        [TestCase(MarkdownLineBreakExportMode.Spaces)]
        public void TestExportLineBreak(int mode)
        {
            const string testFile = @"ExportMarkdown\TestExportLineBreak";

            Document doc = TestUtil.Open(testFile + ".docx");

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.LineBreakExportMode = (MarkdownLineBreakExportMode)mode;

            doc = TestUtil.SaveOpen(doc, testFile + so.LineBreakExportMode.ToString() + ".md", so, true);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            Assert.That(paragraphs.Count, Is.EqualTo(1));
        }


        /// <summary>
        /// Relates to WORDSNET-28043.
        /// Tests single empty paragraph between two others non-empty.
        /// </summary>
        [TestCase(MarkdownEmptyParagraphExportMode.EmptyLine, 2)]
        [TestCase(MarkdownEmptyParagraphExportMode.MarkdownHardLineBreak, 1)]
        [TestCase(MarkdownEmptyParagraphExportMode.None, 2)]
        public void Test28043A(MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Test28043Core(@"ExportMarkdown\Customers\Test28043A", mode, expectedCount);
        }

        /// <summary>
        /// Relates to WORDSNET-28043.
        /// Tests two empty paragraphs between two others non-empty.
        /// </summary>
        [TestCase(MarkdownEmptyParagraphExportMode.EmptyLine, 2)]
        [TestCase(MarkdownEmptyParagraphExportMode.MarkdownHardLineBreak, 1)]
        [TestCase(MarkdownEmptyParagraphExportMode.None, 2)]
        public void Test28043B(MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Test28043Core(@"ExportMarkdown\Customers\Test28043B", mode, expectedCount);
        }

        /// <summary>
        /// Relates to WORDSNET-28043.
        /// Tests empty paragraphs followed by a table.
        /// </summary>
        [TestCase(MarkdownEmptyParagraphExportMode.EmptyLine, 2)]
        [TestCase(MarkdownEmptyParagraphExportMode.MarkdownHardLineBreak, 4)]
        [TestCase(MarkdownEmptyParagraphExportMode.None, 2)]
        public void Test28043C(MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Test28043Core(@"ExportMarkdown\Customers\Test28043C", mode, expectedCount);
        }

        /// <summary>
        /// Relates to WORDSNET-28043.
        /// Tests empty paragraphs followed by a InlineCode.
        /// </summary>
        [TestCase(MarkdownEmptyParagraphExportMode.EmptyLine, 2)]
        [TestCase(MarkdownEmptyParagraphExportMode.MarkdownHardLineBreak, 1)]
        [TestCase(MarkdownEmptyParagraphExportMode.None, 2)]
        public void Test28043D(MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Test28043Core(@"ExportMarkdown\Customers\Test28043D", mode, expectedCount);
        }

        /// <summary>
        /// Relates to WORDSNET-28043.
        /// Tests empty paragraphs followed by a list item.
        /// </summary>
        [TestCase(MarkdownEmptyParagraphExportMode.EmptyLine, 2)]
        [TestCase(MarkdownEmptyParagraphExportMode.MarkdownHardLineBreak, 3)]
        [TestCase(MarkdownEmptyParagraphExportMode.None, 2)]
        public void Test28043E(MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Test28043Core(@"ExportMarkdown\Customers\Test28043E", mode, expectedCount);
        }


        /// <summary>
        /// Relates to WORDSNET-28043.
        /// Tests empty paragraphs inside Table.
        /// </summary>
        // FOSS EmptyLine/MarkdownHardLineBreak emit inline HTML (<p>/<br>) in table cells, so the
        // saved .md is detected as HTML on reopen and the removed HTML loader throws; keep None.
        [TestCase(MarkdownEmptyParagraphExportMode.None, 1)]
        public void Test28043G(MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Test28043Core(@"ExportMarkdown\Customers\Test28043G", mode, expectedCount);
        }


        /// <summary>
        /// WORDSNET-28289 Regression XML to MD: HTML Tables tags with Markdown Content.
        /// We should adopt <see cref="MarkdownSaveOptions.ExportAsHtml"/> option to exporting tables in Markdown.
        /// </summary>
        // FOSS Tables/NonCompatibleTables emit HTML that FOSS can neither write nor read back;
        // keep only the pure-Markdown case.
        [TestCase(MarkdownExportAsHtml.None, CellMerge.None)]
        public void Test28289(MarkdownExportAsHtml exportAsHtml, CellMerge expectedCellMerge)
        {
            const string fileName = @"ExportMarkdown\Customers\Test28289";
            Document doc = TestUtil.Open(string.Format("{0}.xml", fileName));

            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions();
            saveOptions.ExportAsHtml = exportAsHtml;

            string outFileName = string.Format("{0}_{1}.md",fileName, exportAsHtml.ToString());
            doc = TestUtil.SaveOpen(doc, outFileName, saveOptions);

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(2));

            Table table = tables[0];
            Assert.That(table.FirstRow.FirstCell.CellFormat.VerticalMerge, Is.EqualTo(expectedCellMerge));

            table = tables[1];
            Assert.That(table.FirstRow.FirstCell.CellFormat.VerticalMerge, Is.EqualTo(expectedCellMerge));
        }

        /// <summary>
        /// Relates to WORDSNET-28289.
        /// Tests when there are tables in document that can be properly represented by pure Markdown
        /// and tables that can not be properly represented by Markdown without using raw HTML.
        /// </summary>
        // FOSS Tables/NonCompatibleTables emit HTML that FOSS can neither write nor read back;
        // keep only the pure-Markdown case.
        [TestCase(MarkdownExportAsHtml.None, CellMerge.None, 2)]
        public void Test28289Mixed(MarkdownExportAsHtml exportAsHtml, CellMerge expectedCellMerge, int expectedTablesCount)
        {
            const string fileName = @"ExportMarkdown\Customers\Test28289Mixed";
            Document doc = TestUtil.Open(string.Format("{0}.docx", fileName));

            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions();
            saveOptions.ExportAsHtml = exportAsHtml;

            string outFileName = string.Format("{0}_{1}.md", fileName, exportAsHtml.ToString());
            doc = TestUtil.SaveOpen(doc, outFileName, saveOptions);

            // Note, Markdown for the moment cannot read HTML-tables.
            // Also, when both tables are written as HTML with `MarkdownExportAsHtml.Tables` flag,
            // it then detected and loaded as HTML format, and not as Markdown. That's why there
            // are 2 tables with this flag, despite HTML tables cannot be read in Markdown yet.
            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables.Count, Is.EqualTo(expectedTablesCount));
        }

        // FOSS Test28289BothOptions removed: exercises the ExportAsHtml table-precedence feature,
        // which emits HTML that FOSS can neither write nor read back.


        // FOSS Test28748 removed: verifies image alt-text survives a docx->md->docx roundtrip, but the
        // FOSS build does not serialize images to Markdown, so no shape survives the roundtrip.


        /// <summary>
        /// The core method for Test28043.
        /// </summary>
        private static void Test28043Core(string testFile, MarkdownEmptyParagraphExportMode mode, int expectedCount)
        {
            Document doc = TestUtil.Open(testFile + ".docx");

            MarkdownSaveOptions so = new MarkdownSaveOptions();
            so.EmptyParagraphExportMode = mode;

            doc = TestUtil.SaveOpen(doc, string.Format("{0}_{1}.md", testFile, mode.ToString()), so, true);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            Assert.That(paragraphs.Count, Is.EqualTo(expectedCount));
        }

        /// <summary>
        /// The core method for Test24219.
        /// </summary>
        private static string Test24219Core(bool isExportBase64)
        {
            const string testName = @"ExportMarkdown\Customers\Test24219";
            Document doc = TestUtil.Open(testName, LoadFormat.Docx);

            // The customer's scenario is to write into FileStream.
            MarkdownSaveOptions options = new MarkdownSaveOptions { ExportImagesAsBase64 = isExportBase64 };
            string outFileName = TestUtil.GetInTestOutPath(string.Format("{0} Out_{1}.md", testName, isExportBase64));

            string imageFile = outFileName.Replace(".md", ".001.png");
            if (File.Exists(imageFile))
                File.Delete(imageFile);

            using (FileStream fs = new FileStream(outFileName, FileMode.Create))
            {
                doc.Save(fs, options);
            }

            return outFileName;
        }

        /// <summary>
        /// Checks bullet list items of the TestLargeDocument() test.
        /// </summary>
        private static void CheckBulletListItems(Document doc)
        {
            Paragraph paragraph = GetParagraphWithText(doc, "Overview");
            CheckListItem(paragraph, 0, "*", "Normal");
            paragraph = GetParagraphWithText(doc, "Philosophy");
            CheckListItem(paragraph, 1, "o", "Normal");

            paragraph = GetParagraphWithText(doc, "Miscellaneous");
            CheckListItem(paragraph, 0, "*", "Normal");
            paragraph = GetParagraphWithText(doc, "Backslash Escapes");
            CheckListItem(paragraph, 1, "o", "Normal");
        }

        /// <summary>
        /// Checks ordered list items of the TestLargeDocument() test.
        /// </summary>
        private static void CheckOrderedListItems(Document doc)
        {
            Paragraph paragraph = GetParagraphWithText(doc, "Ordered lists use numbers followed by periods:");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Bird", 0, "1.", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "McHale", 0, "2.", "Normal");

            paragraph = GetParagraphWithText(doc, "If you instead wrote the list in Markdown like this:");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Bird", 0, "1.", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "McHale", 0, "2.", "Normal");

            paragraph = GetParagraphWithText(doc, "or even:");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Bird", 0, "3.", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "McHale", 0, "4.", "Normal");

            paragraph = GetParagraphWithText(doc, "This is a list item with two paragraphs.");
            CheckListItem(paragraph, 0, "1.", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Vestibulum enim wisi", 0, " ", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, "Suspendisse", 0, "2.", "Normal");

            paragraph = GetParagraphWithText(doc, "What a great season.");
            CheckListItem(paragraph, 0, "1986.", "Normal");

            paragraph = GetParagraphWithText(doc, "1986. What a great season.");
            Assert.That(paragraph.IsListItem, Is.False);
        }

        /// <summary>
        /// Checks headings of the TestLargeDocument() test.
        /// </summary>
        private static void CheckHeadings(Document doc)
        {
            Paragraph paragraph = GetParagraphWithText(doc, "Markdown: Syntax");
            CheckStyles(paragraph, "SetextHeading1", "Heading 1");

            paragraph = GetParagraphWithText(doc, "This is an H1");
            CheckStyles(paragraph, "SetextHeading1", "Heading 1");

            paragraph = GetParagraphWithText(doc, "This is an H2");
            CheckStyles(paragraph, "SetextHeading2", "Heading 2");

            paragraph = GetParagraphWithText(doc, "This is an H3");
            CheckStyles(paragraph, "Heading 3");

            // Check Heading inside Quote.
            paragraph = GetParagraphWithText(doc, "This is a header.");
            CheckStyles(paragraph, "Quote2", "Heading 2");
        }

        /// <summary>
        /// Checks quotes of the TestLargeDocument() test.
        /// </summary>
        private static void CheckQuotes(Document doc)
        {
            Paragraph paragraph = GetParagraphWithText(doc, "This is a blockquote with two paragraphs.");
            CheckStyles(paragraph, "Quote");

            paragraph = GetParagraphWithText(doc, "This is the first level of quoting.");
            CheckStyles(paragraph, "Quote");
            paragraph = GetParagraphWithText(doc, "This is nested blockquote.");
            CheckStyles(paragraph, "Quote1");
            paragraph = GetParagraphWithText(doc, "Back to the first level.");
            CheckStyles(paragraph, "Quote");

            // Check Quote inside BulletList item.
            paragraph = GetParagraphWithText(doc, "A list item with a blockquote:");
            CheckListItem(paragraph, 0, "*", "Normal");
            paragraph = (Paragraph)paragraph.NextSibling;
            CheckListItem(paragraph, 0, " ", "Quote", "Normal");
        }

        /// <summary>
        /// Checks horizontal rules of the TestLargeDocument() test.
        /// </summary>
        private static void CheckHorizontalRules(Document doc)
        {
            Paragraph paragraph = GetParagraphWithText(doc, "You can produce a horizontal rule tag");
            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(((Shape)paragraph.FirstChild).HorizontalRule.On, Is.True);
            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(((Shape)paragraph.FirstChild).HorizontalRule.On, Is.True);
            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(((Shape)paragraph.FirstChild).HorizontalRule.On, Is.True);
            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(((Shape)paragraph.FirstChild).HorizontalRule.On, Is.True);
            paragraph = (Paragraph)paragraph.NextSibling;
            Assert.That(((Shape)paragraph.FirstChild).HorizontalRule.On, Is.True);
        }

        /// <summary>
        /// Checks emphases of the TestLargeDocument() test.
        /// </summary>
        private static void CheckEmphases(Document doc)
        {
            Paragraph paragraph = (Paragraph)GetParagraphWithText(doc, "Markdown treats asterisks").NextSibling;
            Run run = GetRunWithText(paragraph, "single asterisks");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            paragraph = (Paragraph)paragraph.NextSibling;
            run = GetRunWithText(paragraph, "single underscores");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));

            paragraph = (Paragraph)paragraph.NextSibling;
            run = GetRunWithText(paragraph, "double asterisks");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            paragraph = (Paragraph)paragraph.NextSibling;
            run = GetRunWithText(paragraph, "double underscores");
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            // Check emphasis inside word.
            paragraph = (Paragraph)GetParagraphWithText(doc, "Emphasis can be used in the middle of a word:").NextSibling;
            run = GetRunWithText(paragraph, "un");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
            run = GetRunWithText(paragraph, "frigging");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            run = GetRunWithText(paragraph, "believable");
            Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
        }

        /// <summary>
        /// Checks autolinks of the TestLargeDocument() test.
        /// </summary>
        private static void CheckAutolinks(Document doc)
        {
            // Checks Reference link.
            Paragraph paragraph = (Paragraph)GetParagraphWithText(doc,
                "Just use an empty set of square brackets").NextSibling;
            CheckParagraph(paragraph,
                "\u0013 HYPERLINK \"http://google.com/\" \\s \"[Google]\" \u0014Google\u0015", "Normal");

            paragraph = (Paragraph)GetParagraphWithText(doc, "Markdown will turn this into:").PreviousSibling;
            CheckParagraph(paragraph,
                "\u0013 HYPERLINK \"http://example.com/\" \u0014http://example.com/\u0015\r", "Normal");

            paragraph = (Paragraph)GetParagraphWithText(doc, "into something like this:").PreviousSibling;
            CheckParagraph(paragraph,
                "\u0013 HYPERLINK \"mailto:address@example.com\" \u0014address@example.com\u0015\r", "Normal");
        }

        /// <summary>
        /// Checks links of the TestLargeDocument() test.
        /// </summary>
        private static void CheckLinks(Document doc)
        {
            Paragraph paragraph = (Paragraph)GetParagraphWithText(doc, "Overview");
            CheckParagraph(paragraph,
                "\u0013 HYPERLINK \\l \"overview\" \u0014Overview\u0015\r", "Normal");

            paragraph = (Paragraph)GetParagraphWithText(doc, "Similarly, because Markdown supports");
            CheckParagraph(paragraph,
                "Similarly, because Markdown supports \u0013 HYPERLINK \\l \"html\" \u0014inline HTML\u0015, " +
                "if you use\vangle brackets as delimiters for HTML tags, Markdown will treat them as\vsuch. But " +
                "if you write:\r", "Normal");

            paragraph = (Paragraph)GetParagraphWithText(doc, "To create an inline link, use a set").NextSibling;
            CheckParagraph(paragraph,
                "This is \u0013 HYPERLINK \"http://example.com/\" \\o \"Title\" \u0014an example\u0015 inline link.\r",
                "Normal");

            paragraph = (Paragraph)paragraph.NextSibling;
            CheckParagraph(paragraph,
                "\u0013 HYPERLINK \"http://example.net/\" \u0014This link\u0015 has no title attribute.\r", "Normal");

            paragraph = (Paragraph)GetParagraphWithText(doc, "If you're referring to a local resource").NextSibling;
            CheckParagraph(paragraph,
                "See my \u0013 HYPERLINK \"/about/\" \u0014About\u0015 page for details.\r", "Normal");

            paragraph = (Paragraph)GetParagraphWithText(doc, "For comparison, here is the same paragraph").NextSibling;
            CheckParagraph(paragraph,
                "I get 10 times more traffic from \u0013 HYPERLINK \"http://google.com/\" \\o \"Google\" \\s \"[Google]\" " +
                "\u0014Google\u0015\vthan from \u0013 HYPERLINK \"http://search.yahoo.com/\" \\o \"Yahoo Search\" \\s \"[Yahoo]\" " +
                "\u0014Yahoo\u0015 or\v\u0013 HYPERLINK \"http://search.msn.com/\" \\o \"MSN Search\" \\s \"[MSN]\" " +
                "\u0014MSN\u0015.\r", "Normal");
        }

        /// <summary>
        /// Checks images of the TestLargeDocument() test.
        /// </summary>
        private static void CheckImages(Document doc)
        {
            Paragraph paragraph =
                (Paragraph)GetParagraphWithText(doc, "Inline image syntax looks like this:").NextSibling;
            CheckShape(paragraph.GetChildNodes(NodeType.Any, false)[0] as Shape, "Alt text", "/path/to/img.jpg");

            paragraph = (Paragraph)paragraph.NextSibling;
            CheckShape(paragraph.GetChildNodes(NodeType.Any, false)[0] as Shape, "Alt text", "/path/to/img.jpg", "Optional title");
        }
    }
}
