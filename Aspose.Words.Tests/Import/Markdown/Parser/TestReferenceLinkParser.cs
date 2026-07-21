// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2022 by Vadim Saltykov

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Reference Link feature.
    /// </summary>
    public class TestReferenceLinkParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple Full Reference Link.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkA()
        {
            MarkdownDocument doc = Open("TestFRLinkA.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a link ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "text", "http://example.com", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests several Full Reference Links with different labels.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkB()
        {
            MarkdownDocument doc = Open("TestFRLinkB.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a link ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "text", "https://www.w3schools.com/", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], " and ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[3], "here", "http://example.com", "title1");
            CheckInline(((ParagraphBlock)doc[0])[4], " and ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[5], "here1", "http://example.com", "title1");
            CheckInline(((ParagraphBlock)doc[0])[6], " and ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[7], "here2", "http://example.com", "title1");
            CheckInline(((ParagraphBlock)doc[0])[8], ".");
        }

        /// <summary>
        /// Tests Full Reference Link with square brackets in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkC()
        {
            MarkdownDocument doc = Open("TestFRLinkC.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a link ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "link [foo [bar]]", "http://example.com", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests Full Reference Link with inner link reference in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkD()
        {
            MarkdownDocument doc = Open("TestFRLinkD.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a link [link [foo ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "bar", "http://google.com", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], "]]");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[3], "ref", "http://example.com", "title1");
            CheckInline(((ParagraphBlock)doc[0])[4], ".");
        }

        /// <summary>
        /// Tests Full Reference Link with escaped square bracket in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkE()
        {
            MarkdownDocument doc = Open("TestFRLinkE.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link [bar", "/uri");
        }

        /// <summary>
        /// Tests Full Reference Link with formatting of the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkF()
        {
            MarkdownDocument doc = Open("TestFRLinkF.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "link *foo **bar** `#`*", "/uri");
        }

        /// <summary>
        /// Tests Full Reference Link with inline image block in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkG()
        {
            MarkdownDocument doc = Open("TestFRLinkG.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "moonmoon.jpg", "/uri");
            CheckImageBlock(((LinkTextBlock)((ParagraphBlock)doc[0])[0])[0], "moon", "moon.jpg");
        }

        /// <summary>
        /// Tests Full Reference Link with link text block in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkH()
        {
            MarkdownDocument doc = Open("TestFRLinkH.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "bar", "/uri1");
            CheckInline(((ParagraphBlock)doc[0])[3], "]");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[4], "ref", "/uri");
        }

        /// <summary>
        /// Tests Full Reference Link with formatted link text block in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkI()
        {
            MarkdownDocument doc = Open("TestFRLinkI.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo ");
            CheckItalicInline(((ParagraphBlock)doc[0])[1], "*bar baz*");
            CheckInline(((ItalicInlineBlock)((ParagraphBlock)doc[0])[1])[0], "bar ");
            CheckLinkTextBlock(((ItalicInlineBlock)((ParagraphBlock)doc[0])[1])[1], "baz", "/uri");
            CheckInline(((ParagraphBlock)doc[0])[2], "]");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[3], "ref", "/uri");
        }

        /// <summary>
        /// Tests Full Reference Link with start-to-inner formatting of the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkJ()
        {
            MarkdownDocument doc = Open("TestFRLinkJ.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "*");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "foo*", "/uri");
        }

        /// <summary>
        /// Tests Full Reference Link with the mixed case reference.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkK()
        {
            MarkdownDocument doc = Open("TestFRLinkK.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/url", "title");
        }

        /// <summary>
        /// Tests Full Reference Link with the multiline label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkL()
        {
            MarkdownDocument doc = Open("TestFRLinkL.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "Baz", "/url");
        }

        /// <summary>
        /// Tests Full Reference Link with space-separated label and reference.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkM()
        {
            MarkdownDocument doc = Open("TestFRLinkM.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo] ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "bar", "/url", "title");
        }

        /// <summary>
        /// Tests Full Reference Link with a missing reference.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkN()
        {
            MarkdownDocument doc = Open("TestFRLinkN.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This [foo][bar] baz");
        }

        /// <summary>
        /// Tests simple Collapsed Reference Link.
        /// </summary>
        [Test]
        public void TestCollapsedReferenceLinkA()
        {
            MarkdownDocument doc = Open("TestCRLinkA.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a link ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "here", "http://example.com", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests Collapsed Reference Link with formatting of the label.
        /// </summary>
        [Test]
        public void TestCollapsedReferenceLinkB()
        {
            MarkdownDocument doc = Open("TestCRLinkB.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "*foo* bar", "/url", "title");
            CheckItalicInline(((LinkTextBlock)((ParagraphBlock)doc[0])[0])[0], "*foo*");
            CheckInline(((LinkTextBlock)((ParagraphBlock)doc[0])[0])[1], " bar");
        }

        /// <summary>
        /// Tests Collapsed Reference Link with the mixed case reference.
        /// </summary>
        [Test]
        public void TestCollapsedReferenceLinkC()
        {
            MarkdownDocument doc = Open("TestCRLinkC.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "Foo", "/url", "title");
        }

        /// <summary>
        /// Tests Collapsed Reference Link with the multiline label.
        /// </summary>
        [Test]
        public void TestCollapsedReferenceLinkD()
        {
            MarkdownDocument doc = Open("TestCRLinkD.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/url", "title");
            CheckInline(((ParagraphBlock)doc[0])[1], " "+SoftBreak+"[]");
        }

        /// <summary>
        /// Tests simple Shortcut Reference Link.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkA()
        {
            MarkdownDocument doc = Open("TestSRLinkA.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a link ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "here", "http://example.com", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with different definitions.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkB()
        {
            MarkdownDocument doc = Open("TestSRLinkB.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "bar", "/url1");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with escaped character.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkC()
        {
            MarkdownDocument doc = Open("TestSRLinkC.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[bar][foo!]");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with unescaped square bracket in the reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkD()
        {
            MarkdownDocument doc = Open("TestSRLinkD.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo][ref[]");
            CheckInline(((ParagraphBlock)doc[1])[0], "[ref[]: /uri");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with unescaped square bracket in the reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkE()
        {
            MarkdownDocument doc = Open("TestSRLinkE.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo][ref[bar]]");
            CheckInline(((ParagraphBlock)doc[1])[0], "[ref[bar]]: /uri");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with multiply square brackets in the reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkF()
        {
            MarkdownDocument doc = Open("TestSRLinkF.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[[[foo]]]");
            CheckInline(((ParagraphBlock)doc[1])[0], "[[[foo]]]: /url");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with an escaped square bracket in the reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkG()
        {
            MarkdownDocument doc = Open("TestSRLinkG.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/uri");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with an escaped slash bracket in the reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkH()
        {
            MarkdownDocument doc = Open("TestSRLinkH.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "bar\\", "/uri");
        }

        /// <summary>
        /// Tests Shortcut Reference with the empty reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkI()
        {
            MarkdownDocument doc = Open("TestSRLinkI.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[]");
            CheckInline(((ParagraphBlock)doc[1])[0], "[]: /uri");
        }

        /// <summary>
        /// Tests recursive link resolution.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkJ()
        {
            MarkdownDocument doc = Open("TestSRLinkJ.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/url2");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "bar", "/url1");
        }

        /// <summary>
        /// Tests Shortcut Reference Links with escaped square bracket.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkK()
        {
            MarkdownDocument doc = Open("TestSRLinkK.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo]");
        }

        /// <summary>
        /// Tests resolving between Shortcut and Inline link reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkL()
        {
            MarkdownDocument doc = Open("TestSRLinkL.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "");
        }

        /// <summary>
        /// Tests resolving between Shortcut and Inline link reference.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkM()
        {
            MarkdownDocument doc = Open("TestSRLinkM.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/url1");
            CheckInline(((ParagraphBlock)doc[0])[1], "(not a link)");
        }

        /// <summary>
        /// Tests recursive link resolution.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkN()
        {
            MarkdownDocument doc = Open("TestSRLinkN.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "[foo]");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "bar", "/url");
        }

        /// <summary>
        /// Tests recursive link resolution.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkO()
        {
            MarkdownDocument doc = Open("TestSRLinkO.md");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[0], "foo", "/url2");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "baz", "/url1");
        }

        /// <summary>
        /// Tests simple Link Definition.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionA()
        {
            MarkdownDocument doc = Open("TestRDLinkA.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests Link Definition in the middle.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionB()
        {
            MarkdownDocument doc = Open("TestRDLinkB.md");
            CheckLinkDefinition(doc.LinkDefinitions["BAR"], "/baz");
        }

        /// <summary>
        /// Tests Link Definition in the code block.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionC()
        {
            MarkdownDocument doc = Open("TestRDLinkC.md");
            Assert.That(doc.LinkDefinitions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests Link Definition with indent.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionD()
        {
            MarkdownDocument doc = Open("TestRDLinkD.md");
            Assert.That(doc.LinkDefinitions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests Link Definition with the redundant word.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionE()
        {
            MarkdownDocument doc = Open("TestRDLinkE.md");
            Assert.That(doc.LinkDefinitions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests Link Definition with the multiline reference.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionF()
        {
            MarkdownDocument doc = Open("TestRDLinkF.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests Link Definition after Heading definition.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionG()
        {
            MarkdownDocument doc = Open("TestRDLinkG.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests Link Definition with the incorrect title.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionH()
        {
            MarkdownDocument doc = Open("TestRDLinkH.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests Link Definition without line break.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionI()
        {
            MarkdownDocument doc = Open("TestRDLinkI.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests multiline Link Definition.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionJ()
        {
            MarkdownDocument doc = Open("TestRDLinkJ.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url \r\n 'the title'",
                "/url", "the title");
        }


        /// <summary>
        /// Tests Link Definition with the angle bracket.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionL()
        {
            MarkdownDocument doc = Open("TestRDLinkL.md");
            LinkDefinitionBlock linkDefinition = doc.LinkDefinitions["FOO BAR"];
            CheckLinkDefinition(linkDefinition, "<my url>\r\n 'title'", "my%20url",
                "title");
        }

        /// <summary>
        /// Tests Link Definition with the multiline title.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionM()
        {
            MarkdownDocument doc = Open("TestRDLinkM.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url '\r\ntitle\r\nline1\r\nline2\r\n '",
                "/url", "title\r\nline1\r\nline2");
        }

        /// <summary>
        /// Tests Link Definition with the line break in the title.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionN()
        {
            MarkdownDocument doc = Open("TestRDLinkN.md");
            Assert.That(doc.LinkDefinitions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests multiline Link Definition.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionO()
        {
            MarkdownDocument doc = Open("TestRDLinkO.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests for an incomplete link definition.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionP()
        {
            MarkdownDocument doc = Open("TestRDLinkP.md");
            Assert.That(doc.LinkDefinitions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests Link Definition with the empty angle brackets.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionQ()
        {
            MarkdownDocument doc = Open("TestRDLinkQ.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "<>", string.Empty);
        }

        /// <summary>
        /// Tests Link Definition with an incorrect title.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionR()
        {
            MarkdownDocument doc = Open("TestRDLinkR.md");
            Assert.That(doc.LinkDefinitions.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests Link Definition with escaped characters.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionS()
        {
            MarkdownDocument doc = Open("TestRDLinkS.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url\\bar\\*baz \"foo\\\"bar\\baz\"",
                "/url\\bar*baz", "foo\"bar\\baz");
        }

        /// <summary>
        /// Tests Link Definition with a definition after.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionT()
        {
            MarkdownDocument doc = Open("TestRDLinkT.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "url");
        }

        /// <summary>
        /// Tests Link Definition with duplicate references.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionU()
        {
            MarkdownDocument doc = Open("TestRDLinkU.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "first");
        }

        /// <summary>
        /// Tests Link Definition with the mixed case reference.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionV()
        {
            MarkdownDocument doc = Open("TestRDLinkV.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url");
        }

        /// <summary>
        /// Tests Link Definition with the mixed case reference.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionW()
        {
            MarkdownDocument doc = Open("TestRDLinkW.md");
            CheckLinkDefinition(doc.LinkDefinitions["ΑΓΩ"], "/φου");
        }

        /// <summary>
        /// Tests several Link Definitions.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionX()
        {
            MarkdownDocument doc = Open("TestRDLinkX.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/foo-url \"foo\"",
                "/foo-url", "foo");
            CheckLinkDefinition(doc.LinkDefinitions["BAR"], "/bar-url\r\n \"bar\"",
                "/bar-url", "bar");
            CheckLinkDefinition(doc.LinkDefinitions["BAZ"], "/baz-url");
        }

        /// <summary>
        /// Tests Link Definition with a definition before and after.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionY()
        {
            MarkdownDocument doc = Open("TestRDLinkY.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/url \"title\"",
                "/url", "title");
            CheckLinkDefinition(doc.LinkDefinitions["BAR"], "/url1 \"title1\"",
                "/url1", "title1");
        }

        /// <summary>
        /// Tests Link Definition with duplicate references.
        /// </summary>
        [Test]
        public void TestReferenceLinkDefinitionZ()
        {
            MarkdownDocument doc = Open("TestRDLinkZ.md");
            CheckLinkDefinition(doc.LinkDefinitions["BAR"], "/url \"title\"");
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\ReferenceLinks\{0}", fileName));
        }
    }
}
