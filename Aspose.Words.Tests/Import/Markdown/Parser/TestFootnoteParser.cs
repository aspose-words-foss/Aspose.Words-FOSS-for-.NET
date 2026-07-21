// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2021 by Mikhail Nepreteamov

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests the parsing of the markdown Footnote.
    /// </summary>
    public class TestFootnoteParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests a simple Footnote Reference.
        /// </summary>
        [Test]
        public void TestFootnote_01()
        {
            MarkdownDocument doc = Open("TestFootnote_01.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote [^1] and definition after two LineBreaks.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote "),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, " and definition after two LineBreaks.") });

            FootnoteReferenceBlock footnoteReferenceBlock = (FootnoteReferenceBlock)((ParagraphBlock)doc[0])[1];
            CheckInlines(footnoteReferenceBlock, new[] { new ExpectedInline(BlockType.Inline, "1") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests a simple Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_02()
        {
            MarkdownDocument doc = Open("TestFootnote_02.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckFootnoteDefinition(doc[0], "Definition without reference (not shown).");
        }

        /// <summary>
        /// Tests a simple Footnote Reference without Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_03()
        {
            MarkdownDocument doc = Open("TestFootnote_03.md");
            Assert.That(doc.Count, Is.EqualTo(1));

            CheckParagraph(doc[0], "Here's a footnote[^10] without definition (shown as simple text).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here's a footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^10]"),
                new ExpectedInline(BlockType.Inline, " without definition (shown as simple text).") });
        }

        /// <summary>
        /// Tests a Footnote Reference with a Footnote Definition placed after the one LineBreak.
        /// </summary>
        [Test]
        public void TestFootnote_04()
        {
            MarkdownDocument doc = Open("TestFootnote_04.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote [^1] and definition after one LineBreak (accepted).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote "),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, " and definition after one LineBreak (accepted).") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests two Footnote References (same labels) with one Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_05()
        {
            MarkdownDocument doc = Open("TestFootnote_05.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote[^3], footnote[^3] and definition (accepted).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^3]"),
                new ExpectedInline(BlockType.Inline, ", footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^3]"),
                new ExpectedInline(BlockType.Inline, " and definition (accepted).") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests one Footnote Reference with two Footnote Definitions with the same label.
        /// </summary>
        [Test]
        public void TestFootnote_06()
        {
            MarkdownDocument doc = Open("TestFootnote_06.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0],
                "Here are footnote[^4] and two definitions with same label (error, accepted line1).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^4]"),
                new ExpectedInline(
                    BlockType.Inline, " and two definitions with same label (error, accepted line1).") });

            CheckFootnoteDefinition(doc[1], "Some footnote text (line1).");
            CheckFootnoteDefinition(doc[2], "Some footnote text (line2).");
        }

        /// <summary>
        /// Tests a Footnote Reference with a multiline Footnote Definition with one LineBreak.
        /// </summary>
        [Test]
        public void TestFootnote_07()
        {
            MarkdownDocument doc = Open("TestFootnote_07.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0],
                "Here are footnote [^d] and multiline definition with one LineBreak (accepted as one line).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote "),
                new ExpectedInline(BlockType.FootnoteReference, "[^d]"),
                new ExpectedInline(
                    BlockType.Inline, " and multiline definition with one LineBreak (accepted as one line).") });

            CheckFootnoteDefinition(doc[1], "Some footnote text (line1)."+SoftBreak+
                                            "Some footnote text (line2)."+SoftBreak+
                                            " Some footnote text (line3)."+SoftBreak+
                                            " Some footnote text (line4).");
        }

        /// <summary>
        /// Tests a Footnote Reference with a multiline Footnote Definition with two LineBreaks and indent.
        /// </summary>
        [Test]
        public void TestFootnote_08()
        {
            MarkdownDocument doc = Open("TestFootnote_08.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0],
                "Here are footnote [^e] and multiline definition with two LineBreaks and indent (accepted).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote "),
                new ExpectedInline(BlockType.FootnoteReference, "[^e]"),
                new ExpectedInline(
                    BlockType.Inline, " and multiline definition with two LineBreaks and indent (accepted).") });

            FootnoteDefinitionBlock footnoteDefinition = (FootnoteDefinitionBlock)doc[1];
            CheckParagraph(footnoteDefinition[0], "Some footnote text (line1).");
            CheckParagraph(footnoteDefinition[1], "Some footnote text (line2).");
            CheckParagraph(footnoteDefinition[2], "Some footnote text (line3).");
        }

        /// <summary>
        /// Tests a Footnote Reference with a multiline Footnote Definition with two LineBreaks and without indent.
        /// </summary>
        [Test]
        public void TestFootnote_09()
        {
            MarkdownDocument doc = Open("TestFootnote_09.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0],
                "Here are footnote [^f] and multiline definition with two LineBreaks and without indent " +
                "(accepted line1).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote "),
                new ExpectedInline(BlockType.FootnoteReference, "[^f]"),
                new ExpectedInline(BlockType.Inline,
                    " and multiline definition with two LineBreaks and without indent (accepted line1).") });

            CheckFootnoteDefinition(doc[1], "Some footnote text (line1).");
            CheckParagraph(doc[2], "Some footnote text (line2).");
        }

        /// <summary>
        /// Tests a Footnote Reference with a Footnote Definition with a space in the label.
        /// </summary>
        [Test]
        public void TestFootnote_10()
        {
            MarkdownDocument doc = Open("TestFootnote_10.md");
            Assert.That(doc.Count, Is.EqualTo(6));

            CheckParagraph(doc[0], "Here are wrong footnote[^ e] and wrong definition (simple text).");
            CheckParagraph(doc[1], "[^ e]: Some footnote text (simple text).");
            CheckParagraph(doc[2], "Here are wrong footnote[^e f] and wrong definition (simple text).");
            CheckParagraph(doc[3], "[^e f]: Some footnote text (simple text).");
            CheckParagraph(doc[4], "Here are wrong footnote[^e ] and wrong definition (simple text).");
            CheckParagraph(doc[5], "[^e ]: Some footnote text (simple text).");
        }

        /// <summary>
        /// Tests a Footnote Reference with a Footnote Definition with a valid symblols in the label.
        /// </summary>
        [Test]
        public void TestFootnote_11()
        {
            MarkdownDocument doc = Open("TestFootnote_11.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote[^t@^%$#()-=+|/>.?;,\"'e] and definition (accepted).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^t@^%$#()-=+|/>.?;,\"'e]"),
                new ExpectedInline(BlockType.Inline, " and definition (accepted).") });
        }

        /// <summary>
        /// Tests that InlineCode delimiters have a precedence over a FootnoteReference delimiter.
        /// </summary>
        [Test]
        public void TestFootnote_12()
        {
            MarkdownDocument doc = Open("TestFootnote_12.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are wrong footnote[^t`]` and wrong definition (simple text and code).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are wrong footnote[^t"),
                new ExpectedInline(BlockType.InlineCode, "`]`"),
                new ExpectedInline(BlockType.Inline, " and wrong definition (simple text and code).") });

            CheckParagraph(doc[1], "[^t`]`: Some footnote text (simple text and code).");

            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "[^t"),
                new ExpectedInline(BlockType.InlineCode, "`]`"),
                new ExpectedInline(BlockType.Inline, ": Some footnote text (simple text and code).") });
        }


        /// <summary>
        /// Tests that InlineLink title has a precedence over a FootnoteReference delimiter.
        /// </summary>
        [Test]
        public void TestFootnote_14()
        {
            MarkdownDocument doc = Open("TestFootnote_14.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0],
                "Here are link with title/uri \"titl[^pe\"], footnote[^pe\")] and definition (accepted).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are "),
                new ExpectedInline(BlockType.LinkText, "link with title"),
                new ExpectedInline(BlockType.LinkDestination, "/uri \"titl[^pe\""),
                new ExpectedInline(BlockType.Inline, "], footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^pe\")]"),
                new ExpectedInline(BlockType.Inline, " and definition (accepted).") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests how the simple text is parsed after the multiline definition.
        /// </summary>
        [Test]
        public void TestFootnote_15()
        {
            MarkdownDocument doc = Open("TestFootnote_15.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "Here are footnote[^d], multiline definition and simple text.");
            CheckParagraph(doc[2], "Simple text.");

            FootnoteDefinitionBlock footnoteDefinition = (FootnoteDefinitionBlock)doc[1];
            CheckParagraph(footnoteDefinition[0], "Some footnote text (para1).");
            CheckParagraph(footnoteDefinition[1], "Some footnote text (para2).");
        }

        /// <summary>
        /// Tests two Footnote Definitions in a row.
        /// </summary>
        [Test]
        public void TestFootnote_16()
        {
            MarkdownDocument doc = Open("TestFootnote_16.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "Here are footnote[^1], footnote[^2] and two definitions in a row (accepted).");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, ", footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^2]"),
                new ExpectedInline(BlockType.Inline, " and two definitions in a row (accepted).") });

            CheckFootnoteDefinition(doc[1], "Footnote def1.");
            CheckFootnoteDefinition(doc[2], "Footnote def2.");
        }


        /// <summary>
        /// Tests IndentedCode in Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_18()
        {
            MarkdownDocument doc = Open("TestFootnote_18.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here is footnote[^indented] with IndentedCode in definition.");
            CheckInlines(doc[0],
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "Here is footnote"),
                    new ExpectedInline(BlockType.FootnoteReference, "[^indented]"),
                    new ExpectedInline(BlockType.Inline, " with IndentedCode in definition.")
                });

            FootnoteDefinitionBlock footnote = (FootnoteDefinitionBlock)doc[1];
            CheckParagraph(footnote[0], "Some footnote text (para1).");
            CheckIndentedCode(footnote[1], "    Some indented footnote text (para2).");

            Assert.That(footnote.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests an empty Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_19()
        {
            MarkdownDocument doc = Open("TestFootnote_19.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote[^1] and empty definition.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, " and empty definition.") });

            FootnoteDefinitionBlock footnoteDefinition = (FootnoteDefinitionBlock)doc[1];
            Assert.That(footnoteDefinition.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests an empty Footnote Definition and followed simple text.
        /// </summary>
        [Test]
        public void TestFootnote_20()
        {
            MarkdownDocument doc = Open("TestFootnote_20.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "Here are footnote[^1], empty definition and simple text.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, ", empty definition and simple text.") });

            FootnoteDefinitionBlock footnoteDefinition = (FootnoteDefinitionBlock)doc[1];
            Assert.That(footnoteDefinition.Count, Is.EqualTo(0));

            CheckParagraph(doc[2], "Simple text.");
        }

        /// <summary>
        /// Tests a Footnote Reference in italics.
        /// </summary>
        [Test]
        public void TestFootnote_21()
        {
            MarkdownDocument doc = Open("TestFootnote_21.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote in italics *[^1]* and definition.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote in italics "),
                new ExpectedInline(BlockType.ItalicInline, "*[^1]*"),
                new ExpectedInline(BlockType.Inline, " and definition.") });

            ItalicInlineBlock italicBlock = (ItalicInlineBlock)((ParagraphBlock)doc[0])[1];
            CheckInlines(italicBlock, new[] { new ExpectedInline(BlockType.FootnoteReference, "[^1]") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests a Footnote Reference and a simple text in italics.
        /// </summary>
        [Test]
        public void TestFootnote_22()
        {
            MarkdownDocument doc = Open("TestFootnote_22.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are *footnote[^1] and* simple text in italics.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are "),
                new ExpectedInline(BlockType.ItalicInline, "*footnote[^1] and*"),
                new ExpectedInline(BlockType.Inline, " simple text in italics.") });

            ItalicInlineBlock italicBlock = (ItalicInlineBlock)((ParagraphBlock)doc[0])[1];
            CheckInlines(italicBlock, new[] {
                new ExpectedInline(BlockType.Inline, "footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, " and") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests a Footnote Reference and a simple text in italics (special cases).
        /// </summary>
        [Test]
        public void TestFootnote_23()
        {
            MarkdownDocument doc = Open("TestFootnote_23.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote *[^1],* *footnote[^1]* and simple text in italics.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote "),
                new ExpectedInline(BlockType.ItalicInline, "*[^1],*"),
                new ExpectedInline(BlockType.Inline, " "),
                new ExpectedInline(BlockType.ItalicInline, "*footnote[^1]*"),
                new ExpectedInline(BlockType.Inline, " and simple text in italics.") });

            ItalicInlineBlock italicBlock = (ItalicInlineBlock)((ParagraphBlock)doc[0])[1];
            CheckInlines(italicBlock, new[] {
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, ",") });

            italicBlock = (ItalicInlineBlock)((ParagraphBlock)doc[0])[3];
            CheckInlines(italicBlock, new[] {
                new ExpectedInline(BlockType.Inline, "footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
        }

        /// <summary>
        /// Tests a Footnote Reference with an indented Footnote Definition.
        /// </summary>
        [Test]
        public void TestFootnote_24()
        {
            MarkdownDocument doc = Open("TestFootnote_24.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote[^1] and indented definition.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, " and indented definition.") });

            CheckFootnoteDefinition(doc[1], "Some footnote text.");
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
            MarkdownDocument doc = Open("TestFootnote_26.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here are footnote[^1] and definition containing list.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here are footnote"),
                new ExpectedInline(BlockType.FootnoteReference, "[^1]"),
                new ExpectedInline(BlockType.Inline, " and definition containing list.") });

            CheckFootnoteDefinition(doc[1], "Some footnote text."+SoftBreak+"+ list item 1."+SoftBreak+"+ list item 2.");
        }

        /// <summary>
        /// Tests that InlineLink text has a precedence over a FootnoteReference delimiter.
        /// </summary>
        [Test]
        public void TestFootnote_27()
        {
            MarkdownDocument doc = Open("TestFootnote_27.md");
            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "Here is link ^1link_dest.");

            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Here is link "),
                new ExpectedInline(BlockType.LinkText, "^1"),
                new ExpectedInline(BlockType.LinkDestination, "link_dest"),
                new ExpectedInline(BlockType.Inline, ".") });

            CheckFootnoteDefinition(doc[1], "Footnote definition without reference.");
        }

        /// <summary>
        /// Tests emphases in footnote.
        /// </summary>
        [Test]
        public void TestFootnote_28()
        {
            MarkdownDocument doc = Open("TestFootnote_28.md");
            Assert.That(doc.Count, Is.EqualTo(3));

            // Check paragraph with footnote references.
            CheckParagraph(doc[0], "Here is footnote in **bold[^1]** and footnote in *italics*[^2].");
            CheckInlines(doc[0],
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "Here is footnote in "),
                    new ExpectedInline(BlockType.BoldInline, "**bold[^1]**"),
                    new ExpectedInline(BlockType.Inline, " and footnote in "),
                    new ExpectedInline(BlockType.ItalicInline, "*italics*"),
                    new ExpectedInline(BlockType.FootnoteReference, "[^2]"),
                    new ExpectedInline(BlockType.Inline, ".")
                });

            // Check first footnote reference is inside bold.
            BoldInlineBlock boldContainer = (BoldInlineBlock)((ParagraphBlock)doc[0])[1];
            CheckInlines(boldContainer,
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "bold"),
                    new ExpectedInline(BlockType.FootnoteReference, "[^1]")
                });

            // Check first footnote.
            FootnoteDefinitionBlock footnote = (FootnoteDefinitionBlock)doc[1];
            CheckParagraph(footnote[0], "Some **bold footnote** text.");
            CheckInlines(footnote[0],
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "Some "),
                    new ExpectedInline(BlockType.BoldInline, "**bold footnote**"),
                    new ExpectedInline(BlockType.Inline, " text.")
                });

            // Check second footnote.
            footnote = (FootnoteDefinitionBlock)doc[2];
            CheckParagraph(footnote[0], "Some *italic footnote*");
            CheckInlines(footnote[0],
                new[]
                {
                    new ExpectedInline(BlockType.Inline, "Some "),
                    new ExpectedInline(BlockType.ItalicInline, "*italic footnote*")
                });
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Footnotes\{0}", fileName));
        }

        /// <summary>
        /// Verifies the FootnoteDefinition block.
        /// </summary>
        private static void CheckFootnoteDefinition(Block footnoteDefinitionBlock, string expectedText)
        {
            FootnoteDefinitionBlock footnoteDefinition = (FootnoteDefinitionBlock)footnoteDefinitionBlock;
            CheckParagraph(footnoteDefinition[0], expectedText);
        }
    }
}
