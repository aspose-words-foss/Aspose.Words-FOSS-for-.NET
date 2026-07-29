// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/10/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Bullet List feature.
    /// </summary>
    public class TestBulletListParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple bullet lists with various markers (-, +, *).
        /// </summary>
        [Test]
        public void TestBulletListA()
        {
            MarkdownDocument doc = Open(@"TestBulletListA.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            CheckBulletListItem(doc[0], ListMarker.Minus, "Foo");
            CheckBulletListItem(doc[1], ListMarker.Plus, "bar");
            CheckBulletListItem(doc[2], ListMarker.Asterisk, "baz");
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestBulletListB()
        {
            MarkdownDocument doc = Open(@"TestBulletListB.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            CheckBulletListItem(doc[0], ListMarker.Minus, "one");
            CheckParagraph(doc[1], "two");
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestBulletListC()
        {
            MarkdownDocument doc = Open(@"TestBulletListC.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "one");
            CheckParagraph(bulletListItemBlock[1], "two");
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestBulletListD()
        {
            MarkdownDocument doc = Open(@"TestBulletListD.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            CheckBulletListItem(doc[0], ListMarker.Minus, "one");
            CheckIndentedCode(doc[1],  " two");
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestBulletListE()
        {
            MarkdownDocument doc = Open(@"TestBulletListE.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "one");
            CheckParagraph(bulletListItemBlock[1], "two");
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item
        /// when it is embedded in other constructions.
        /// </summary>
        [Test]
        public void TestBulletListF()
        {
            MarkdownDocument doc = Open(@"TestBulletListF.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)quoteBlock[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Asterisk));
            CheckParagraph(bulletListItemBlock[0], "one");
            CheckParagraph(bulletListItemBlock[1], "two");
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item
        /// when it is embedded in other constructions.
        /// </summary>
        [Test]
        public void TestBulletListG()
        {
            MarkdownDocument doc = Open(@"TestBulletListG.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)quoteBlock[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "one");

            CheckParagraph(nestedQuoteBlock[1], "two");
        }

        /// <summary>
        /// Tests that at least one space is needed between the list marker and any following content.
        /// </summary>
        [Test]
        public void TestBulletListH()
        {
            MarkdownDocument doc = Open(@"TestBulletListH.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            CheckParagraph(doc[0], "-one");
            CheckParagraph(doc[1], "+two");
        }

        /// <summary>
        /// Tests that a list item may contain blocks that are separated by more than one blank line.
        /// </summary>
        [Test]
        public void TestBulletListI()
        {
            MarkdownDocument doc = Open(@"TestBulletListI.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckParagraph(bulletListItemBlock[0], "foo");
            CheckParagraph(bulletListItemBlock[1], "bar");
        }

        /// <summary>
        /// Tests that a list item may contain any kind of block.
        /// </summary>
        [Test]
        public void TestBulletListJ()
        {
            MarkdownDocument doc = Open(@"TestBulletListJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Plus));

            CheckParagraph(bulletListItemBlock[0], "foo");
            CheckFencedCode(bulletListItemBlock[1], "bar");
            CheckParagraph(bulletListItemBlock[2], "baz");

            QuoteBlock quoteBlock = (QuoteBlock)bulletListItemBlock[3];
            CheckParagraph(quoteBlock[0], "bam");
        }

        /// <summary>
        /// Tests that a list item that contains an indented code block will
        /// preserve empty lines within the code block verbatim.
        /// </summary>
        [Test]
        public void TestBulletListK()
        {
            MarkdownDocument doc = Open(@"TestBulletListK.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckParagraph(bulletListItemBlock[0], "Foo");
            CheckIndentedCode(bulletListItemBlock[1], "bar\v\v\vbaz");
        }

        /// <summary>
        /// Tests that an indented code block will have to be indented four spaces
        /// beyond the edge of the region where text will be included in the list item.
        /// </summary>
        [Test]
        public void TestBulletListL()
        {
            MarkdownDocument doc = Open(@"TestBulletListL.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckParagraph(bulletListItemBlock[0], "foo");
            CheckIndentedCode(bulletListItemBlock[1], "bar");
        }

        /// <summary>
        /// Tests that if the first block in the list item is an indented code block,
        /// then the contents must be indented one space after the list marker.
        /// </summary>
        [Test]
        public void TestBulletListM()
        {
            MarkdownDocument doc = Open(@"TestBulletListM.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            CheckIndentedCode(doc[0], "indented code");
            CheckParagraph(doc[1], "paragraph");
            CheckIndentedCode(doc[2], "more code");
        }

        /// <summary>
        /// Tests that if the first block in the list item is an indented code block,
        /// then the contents must be indented one space after the list marker.
        /// </summary>
        [Test]
        public void TestBulletListN()
        {
            MarkdownDocument doc = Open(@"TestBulletListN.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Plus));

            CheckIndentedCode(bulletListItemBlock[0], " indented code");
            CheckParagraph(bulletListItemBlock[1], "paragraph");
            CheckIndentedCode(bulletListItemBlock[2], " more code");
        }

        /// <summary>
        /// Tests that if the first block in the list item is an indented code block,
        /// then the contents must be indented one space after the list marker.
        /// </summary>
        [Test]
        public void TestBulletListO()
        {
            MarkdownDocument doc = Open(@"TestBulletListO.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckParagraph(bulletListItemBlock[0], "foo");
            CheckParagraph(doc[1], "bar");
        }

        /// <summary>
        /// Tests that the indentation can always be removed without a change in interpretation.
        /// </summary>
        [Test]
        public void TestBulletListP()
        {
            MarkdownDocument doc = Open(@"TestBulletListP.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckParagraph(bulletListItemBlock[0], "foo");
            CheckParagraph(bulletListItemBlock[1], "bar");
        }

        /// <summary>
        /// Tests list items that start with a blank line but are not empty.
        /// </summary>
        [Test]
        public void TestBulletListQ()
        {
            MarkdownDocument doc = Open(@"TestBulletListQ.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "foo");

            bulletListItemBlock = (BulletListItemBlock)doc[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckFencedCode(bulletListItemBlock[0], "bar");

            bulletListItemBlock = (BulletListItemBlock)doc[2];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckIndentedCode(bulletListItemBlock[0], "baz");
        }

        /// <summary>
        /// Tests that a list item can begin with at most one blank line.
        /// </summary>
        [Test]
        public void TestBulletListR()
        {
            MarkdownDocument doc = Open(@"TestBulletListR.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            Assert.That(bulletListItemBlock.Count, Is.EqualTo(0));

            CheckParagraph(doc[1], "foo");
        }

        /// <summary>
        /// Tests an empty bullet list item.
        /// </summary>
        [Test]
        public void TestBulletListS()
        {
            MarkdownDocument doc = Open(@"TestBulletListS.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckBulletListItem(doc[1], ListMarker.Minus, "");
            CheckBulletListItem(doc[2], ListMarker.Minus, "bar");
        }

        /// <summary>
        /// Tests that it does not matter whether there are spaces
        /// following the list marker in an empty bullet list item.
        /// </summary>
        [Test]
        public void TestBulletListT()
        {
            MarkdownDocument doc = Open(@"TestBulletListT.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckBulletListItem(doc[1], ListMarker.Minus, "");
            CheckBulletListItem(doc[2], ListMarker.Minus, "bar");
        }

        /// <summary>
        /// Tests that a list may start or end with an empty list item.
        /// </summary>
        [Test]
        public void TestBulletListU()
        {
            MarkdownDocument doc = Open(@"TestBulletListU.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckBulletListItem(doc[0], ListMarker.Asterisk, "");
        }

        /// <summary>
        /// Tests that an empty list item cannot interrupt a paragraph.
        /// </summary>
        [Test]
        public void TestBulletListV()
        {
            MarkdownDocument doc = Open(@"TestBulletListV.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckParagraph(doc[0], "foo"+SoftBreak+"*");
            CheckParagraph(doc[1], "foo"+SoftBreak+"+");
        }

        /// <summary>
        /// Tests that four spaces indent gives a code block instead of list item.
        /// </summary>
        [Test]
        public void TestBulletListW()
        {
            MarkdownDocument doc = Open(@"TestBulletListW.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckIndentedCode(doc[0], "-   A paragraph\v    with two lines.\v\v        indented code\v\v    > A block quote.");
        }

        /// <summary>
        /// Tests that indentation can be partially deleted.
        /// </summary>
        [Test]
        public void TestBulletListX()
        {
            MarkdownDocument doc = Open(@"TestBulletListX.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckBulletListItem(doc[0], ListMarker.Plus, "A paragraph"+SoftBreak+"with two lines.");
        }

        /// <summary>
        /// Tests how laziness can work in nested structures.
        /// </summary>
        [Test]
        public void TestBulletListY()
        {
            MarkdownDocument doc = Open(@"TestBulletListY.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            QuoteBlock nestedQuoteBlock = (QuoteBlock)bulletListItemBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "Blockquote"+SoftBreak+"continued here.");
        }

        /// <summary>
        /// Tests how laziness can work in nested structures.
        /// </summary>
        [Test]
        public void TestBulletListZ()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            QuoteBlock nestedQuoteBlock = (QuoteBlock)bulletListItemBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "Blockquote"+SoftBreak+"continued here.");
        }

        /// <summary>
        /// Tests nested lists.
        /// </summary>
        [Test]
        public void TestBulletListZ1()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ1.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            // Root list.
            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "foo");

            // The first nesting level.
            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "bar");

            // The second nesting level.
            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "baz");

            // The third nesting level.
            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "boo");
        }

        /// <summary>
        /// Tests that one indentation is not enough to make lists nested.
        /// </summary>
        [Test]
        public void TestBulletListZ2()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ2.md");

            Assert.That(doc.Count, Is.EqualTo(4));

            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckBulletListItem(doc[1], ListMarker.Minus, "bar");
            CheckBulletListItem(doc[2], ListMarker.Minus, "baz");
            CheckBulletListItem(doc[3], ListMarker.Minus, "boo");
        }

        /// <summary>
        /// Tests that a list may be the first block in a list item.
        /// </summary>
        [Test]
        public void TestBulletListZ3()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ3.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));

            CheckBulletListItem(bulletListItemBlock[0], ListMarker.Minus, "foo");
        }

        /// <summary>
        /// Tests that a list item can contain a heading.
        /// </summary>
        [Test]
        public void TestBulletListZ4()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ4.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckAtxHeading(bulletListItemBlock[0], 1, "Foo");

            bulletListItemBlock = (BulletListItemBlock)doc[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckSetextHeading(bulletListItemBlock[0], 2, "Bar");
            CheckParagraph(bulletListItemBlock[1], "baz");
        }

        /// <summary>
        /// Tests that a list can interrupt a paragraph. That is, no blank line
        /// is needed to separate a paragraph from a following list.
        /// </summary>
        [Test]
        public void TestBulletListZ5()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ5.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            CheckParagraph(doc[0], "Foo");
            CheckBulletListItem(doc[1], ListMarker.Minus, "bar");
            CheckBulletListItem(doc[2], ListMarker.Minus, "baz");
        }

        /// <summary>
        /// Tests that there can be any number of blank lines between items.
        /// </summary>
        [Test]
        public void TestBulletListZ6()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ6.md");

            Assert.That(doc.Count, Is.EqualTo(3));

            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckBulletListItem(doc[1], ListMarker.Minus, "bar");
            CheckBulletListItem(doc[2], ListMarker.Minus, "baz");
        }

        /// <summary>
        /// Tests that there can be any number of blank lines between items in nested lists.
        /// </summary>
        [Test]
        public void TestBulletListZ7()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ7.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            // Root list.
            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "foo");

            // The first nesting level.
            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "bar");

            // The second nesting level.
            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "baz");
            CheckParagraph(bulletListItemBlock[1], "bim");
        }

        /// <summary>
        /// Tests that to separate consecutive lists of the same type, or to separate a list from
        /// an indented code block that would otherwise be parsed as a subparagraph of the final
        /// list item, you can insert a blank HTML comment.
        /// </summary>
        [Test]
        public void TestBulletListZ8()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ8.md");

            Assert.That(doc.Count, Is.EqualTo(5));

            CheckBulletListItem(doc[0], ListMarker.Minus, "foo");
            CheckBulletListItem(doc[1], ListMarker.Minus, "bar");
            CheckParagraph(doc[2], "");
            CheckBulletListItem(doc[3], ListMarker.Minus, "baz");
            CheckBulletListItem(doc[4], ListMarker.Minus, "bim");
        }

        /// <summary>
        /// Tests that list items need not be indented to the same level. The following list items will be treated
        /// as items at the same list level, since none is indented enough to belong to the previous list item.
        /// </summary>
        [Test]
        public void TestBulletListZ9()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ9.md");

            Assert.That(doc.Count, Is.EqualTo(7));

            CheckBulletListItem(doc[0], ListMarker.Minus, "a");
            CheckBulletListItem(doc[1], ListMarker.Minus, "b");
            CheckBulletListItem(doc[2], ListMarker.Minus, "c");
            CheckBulletListItem(doc[3], ListMarker.Minus, "d");
            CheckBulletListItem(doc[4], ListMarker.Minus, "e");
            CheckBulletListItem(doc[5], ListMarker.Minus, "f");
            CheckBulletListItem(doc[6], ListMarker.Minus, "g");
        }

        /// <summary>
        /// Tests that list items may not be indented more than three spaces.
        /// </summary>
        [Test]
        public void TestBulletListZ10()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ10.md");

            Assert.That(doc.Count, Is.EqualTo(4));

            CheckBulletListItem(doc[0], ListMarker.Minus, "a");
            CheckBulletListItem(doc[1], ListMarker.Minus, "b");
            CheckBulletListItem(doc[2], ListMarker.Minus, "c");
            //  Here - e is treated as a paragraph continuation line, because it is indented more than three spaces.
            CheckBulletListItem(doc[3], ListMarker.Minus, "d"+SoftBreak+"- e");
        }

        /// <summary>
        /// Tests list items with tab indentation.
        /// </summary>
        [Test]
        public void TestBulletListZ11()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ11.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Minus));
            CheckParagraph(bulletListItemBlock[0], "Foo");
            CheckParagraph(bulletListItemBlock[1], "bar");
        }

        /// <summary>
        /// Tests that horizontal rule falls under a list item in Quote if it has needed indentation.
        /// </summary>
        [Test]
        public void TestBulletListZ12()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ12.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Asterisk));
            CheckIndentedCode(bulletListItemBlock[0], "   foo");
            CheckHorizontalRule(bulletListItemBlock[1]);
        }

        /// <summary>
        /// Tests nested Quotes and Lists with empty items.
        /// </summary>
        [Test]
        public void TestBulletListZ13()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ13.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)bulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedQuoteBlock[0];
            CheckBulletListItem(nestedQuoteBlock[0], ListMarker.Minus, "foo");

            bulletListItemBlock = (BulletListItemBlock)quoteBlock[1];
            nestedQuoteBlock = (QuoteBlock)bulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedQuoteBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "bar");
        }

        /// <summary>
        /// Tests nested Quotes and Lists with non-empty items.
        /// </summary>
        [Test]
        public void TestBulletListZ14()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ14.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckBulletListItem(quoteBlock[0], ListMarker.Minus, "foo >> - bar");
            CheckBulletListItem(quoteBlock[1], ListMarker.Minus, "bob >>   baz");
        }

        /// <summary>
        /// Tests bullet list many levels.
        /// </summary>
        [Test]
        public void TestBulletListZ15()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ15.md");

            Assert.That(doc.Count, Is.EqualTo(1));

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            CheckParagraph(bulletListItemBlock[0], "one");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "two");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "three");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "four");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "five");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "six");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "seven");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "eight");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "nine");

            bulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(bulletListItemBlock[0], "ten");
        }

        /// <summary>
        /// Tests nested bullet list items.
        /// </summary>
        [Test]
        public void TestBulletListZ16()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ16.md");

            Assert.That(doc.Count, Is.EqualTo(2));

            BulletListItemBlock listItemLevel1 = (BulletListItemBlock)doc[0];
            CheckParagraph(listItemLevel1[0], "one");

            BulletListItemBlock listItemLevel2 = (BulletListItemBlock)listItemLevel1[1];
            CheckParagraph(listItemLevel2[0], "two");

            listItemLevel2 = (BulletListItemBlock)listItemLevel1[2];
            CheckParagraph(listItemLevel2[0], "three");

            listItemLevel2 = (BulletListItemBlock)listItemLevel1[3];
            CheckParagraph(listItemLevel2[0], "four"+SoftBreak+"- five");

            BulletListItemBlock listItemLevel3 = (BulletListItemBlock)listItemLevel2[1];
            CheckParagraph(listItemLevel3[0], "six");

            listItemLevel2 = (BulletListItemBlock)listItemLevel1[4];
            CheckParagraph(listItemLevel2[0], "seven");

            listItemLevel3 = (BulletListItemBlock)listItemLevel2[1];
            CheckParagraph(listItemLevel3[0], "eight");

            listItemLevel1 = (BulletListItemBlock)doc[1];
            CheckParagraph(listItemLevel1[0], "nine"+SoftBreak+"- ten");
        }

        /// <summary>
        /// Tests nested bullet list items with Headings.
        /// </summary>
        [Test]
        public void TestBulletListZ17()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ17.md");

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)doc[0];
            CheckParagraph(bulletListItemBlock[0], "consectetur");

            BulletListItemBlock nestedBulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[1];
            CheckParagraph(nestedBulletListItemBlock[0], "adipiscing"+SoftBreak+"elit");
            CheckAtxHeading(nestedBulletListItemBlock[1], 1, "sed");

            CheckSetextHeading(bulletListItemBlock[2], 2, "do");
        }

        /// <summary>
        /// Tests that in order to block fall into a list item inside a Quote, this block must be in Quote too.
        /// </summary>
        [Test]
        public void TestBulletListZ18()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ18.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            CheckHorizontalRule(bulletListItemBlock[0]);
            CheckParagraph(bulletListItemBlock[1], "aliqua.");

            BulletListItemBlock nestedBulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[2];
            // The line '+ enim' does not fall under the list item, but is lazy continuation of the previous paragraph.
            CheckParagraph(nestedBulletListItemBlock[0], "Ut"+SoftBreak+"+ enim");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that if paragraph is not in list item, then empty list item cannot interrupt it
        /// and this is a lazy continuation of this paragraph.
        /// </summary>
        [Test]
        public void TestBulletListZ19()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ19.md");

            CheckParagraph(doc[0], "Foo"+SoftBreak+"+");
            CheckBulletListItem(doc[1], ListMarker.Plus, "bar");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests complex nested bullet lists and quotes.
        /// </summary>
        [Test]
        public void TestBulletListZ20()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ20.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckBulletListItem(quoteBlock[0], ListMarker.Minus, "a");

            CheckBulletListItem(doc[1], ListMarker.Minus, "b");

            quoteBlock = (QuoteBlock)doc[2];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)bulletListItemBlock[0];
            bulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            nestedQuoteBlock = (QuoteBlock)bulletListItemBlock[0];
            CheckBulletListItem(nestedQuoteBlock[0], ListMarker.Minus, "c");

            bulletListItemBlock = (BulletListItemBlock)quoteBlock[1];
            BulletListItemBlock nestedBulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedBulletListItemBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedBulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedBulletListItemBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            CheckBulletListItem(nestedBulletListItemBlock[0], ListMarker.Minus, "d");

            bulletListItemBlock = (BulletListItemBlock)doc[3];
            nestedBulletListItemBlock = (BulletListItemBlock)bulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedBulletListItemBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedBulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedBulletListItemBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedQuoteBlock[0];
            nestedBulletListItemBlock = (BulletListItemBlock)nestedBulletListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedBulletListItemBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "e");

            Assert.That(doc.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests bullet lists in quote with headings.
        /// </summary>
        [Test]
        public void TestBulletListZ21()
        {
            MarkdownDocument doc = Open(@"TestBulletListZ21.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            BulletListItemBlock bulletListItemBlock = (BulletListItemBlock)quoteBlock[0];
            CheckParagraph(bulletListItemBlock[0], "a");

            bulletListItemBlock = (BulletListItemBlock)quoteBlock[1];
            CheckAtxHeading(bulletListItemBlock[0], 1, "b");

            bulletListItemBlock = (BulletListItemBlock)quoteBlock[2];
            CheckSetextHeading(bulletListItemBlock[0], 2, "c");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Lists\BulletList\{0}", fileName));
        }
    }
}
