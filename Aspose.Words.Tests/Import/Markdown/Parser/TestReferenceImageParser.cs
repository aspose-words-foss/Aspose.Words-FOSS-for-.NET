// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2022 by Vadim Saltykov

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Reference Image feature.
    /// </summary>
    public class TestReferenceImageParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple Full Reference Image.
        /// </summary>
        [Test]
        public void TestFullReferenceImageA()
        {
            MarkdownDocument doc = Open("TestFRImageA.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a image ");
            CheckImageBlock(((ParagraphBlock)doc[0])[1], "Alt text", "/uri", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests Full Reference Image with square brackets in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkB()
        {
            MarkdownDocument doc = Open("TestFRImageB.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "Image ");
            CheckImageBlock(((ParagraphBlock)doc[0])[1], "link [foo [bar]]", "/uri", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests Full Reference Image with escaped square bracket in the label.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkC()
        {
            MarkdownDocument doc = Open("TestFRImageC.md");
            CheckImageBlock(((ParagraphBlock)doc[0])[0], "link [bar", "/uri");
        }

        /// <summary>
        /// Tests Full Reference Image with space-separated label and reference.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkD()
        {
            MarkdownDocument doc = Open("TestFRImageD.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "![foo] ");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "bar", "/url", "title");
        }


        /// <summary>
        /// Tests resolving between Full Reference Image and Shortcut LinkText.
        /// </summary>
        [Test]
        public void TestFullReferenceLinkF()
        {
            MarkdownDocument doc = Open("TestFRImageF.md");
            CheckImageBlock(((ParagraphBlock)doc[0])[0], "foo", "/url2");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "baz", "/url1");
        }



        /// <summary>
        /// Tests simple Collapsed Reference Image.
        /// </summary>
        [Test]
        public void TestCollapsedReferenceImageA()
        {
            MarkdownDocument doc = Open("TestCRImageA.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a image ");
            CheckImageBlock(((ParagraphBlock)doc[0])[1], "foo", "/uri", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests Collapsed Reference Image with the mixed case reference.
        /// </summary>
        [Test]
        public void TestCollapsedReferenceLinkB()
        {
            MarkdownDocument doc = Open("TestCRImageB.md");
            CheckImageBlock(((ParagraphBlock)doc[0])[0], "Foo", "/url", "title");
        }

        /// <summary>
        /// Tests simple Shortcut Reference Image.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkA()
        {
            MarkdownDocument doc = Open("TestSRImageA.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "This is a image ");
            CheckImageBlock(((ParagraphBlock)doc[0])[1], "foo", "/uri", "title");
            CheckInline(((ParagraphBlock)doc[0])[2], ".");
        }

        /// <summary>
        /// Tests mixing reference image and reference link labels.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkB()
        {
            MarkdownDocument doc = Open("TestSRImageB.md");
            CheckImageBlock(((ParagraphBlock)doc[0])[0], "foo", "/url1");
            CheckLinkTextBlock(((ParagraphBlock)doc[1])[0], "foo", "/url1");
            CheckImageBlock(((ParagraphBlock)doc[1])[1], "bar", "/url2");
        }

        /// <summary>
        /// Tests the priority of inline definition and reference definition.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkС()
        {
            MarkdownDocument doc = Open("TestSRImageC.md");
            CheckImageBlock(((ParagraphBlock)doc[0])[0], "foo", "/uri1");
        }

        /// <summary>
        /// Tests an empty reference image.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkD()
        {
            MarkdownDocument doc = Open("TestSRImageD.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "![]");
            CheckInline(((ParagraphBlock)doc[1])[0], "[]: /uri");
        }

        /// <summary>
        /// Tests the escaped description of the reference image.
        /// </summary>
        [Test]
        public void TestLinkShortcutReferenceLinkE()
        {
            MarkdownDocument doc = Open("TestSRImageE.md");
            CheckInline(((ParagraphBlock)doc[0])[0], "!");
            CheckLinkTextBlock(((ParagraphBlock)doc[0])[1], "foo", "/url", "title");
        }


        /// <summary>
        /// Tests simple Image Definition.
        /// </summary>
        [Test]
        public void TestImageReferenceDefinitionA()
        {
            MarkdownDocument doc = Open("TestRefImageA.md");
            CheckLinkDefinition(doc.LinkDefinitions["REF1"], "/uri1 \"title1\"");
            CheckLinkDefinition(doc.LinkDefinitions["REF2"], "/uri2 \"title2\"");
            CheckLinkDefinition(doc.LinkDefinitions["REF3"], "/uri3 \"title3\"");
            CheckLinkDefinition(doc.LinkDefinitions["REF4"], "/uri4 \"title4\"");
        }

        /// <summary>
        /// Tests Image Definition with duplicate references.
        /// </summary>
        [Test]
        public void TestImageReferenceDefinitionB()
        {
            MarkdownDocument doc = Open("TestRefImageB.md");
            CheckLinkDefinition(doc.LinkDefinitions["FOO"], "/first");
        }


        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\ReferenceImages\{0}", fileName));
        }
    }
}
