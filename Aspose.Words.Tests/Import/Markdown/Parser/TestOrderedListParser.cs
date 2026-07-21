// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Ordered List feature.
    /// </summary>
    public class TestOrderedListParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests simple ordered lists with various markers ('.', ')').
        /// </summary>
        [Test]
        public void TestOrderedListA()
        {
            MarkdownDocument doc = Open(@"TestOrderedListA.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Parenthesis, "Foo");
            CheckOrderedListItem(doc[1], 2, ListMarker.Parenthesis, "bar");
            CheckOrderedListItem(doc[2], 1, ListMarker.Dot, "baz");
            CheckOrderedListItem(doc[3], 2, ListMarker.Dot, "bop");

            Assert.That(doc.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestOrderedListB()
        {
            MarkdownDocument doc = Open(@"TestOrderedListB.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Parenthesis, "one");
            CheckParagraph(doc[1], "two");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestOrderedListC()
        {
            MarkdownDocument doc = Open(@"TestOrderedListC.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "one");
            CheckParagraph(orderedListItemBlock[1], "two");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestOrderedListD()
        {
            MarkdownDocument doc = Open(@"TestOrderedListD.md");

            CheckOrderedListItem(doc[0], 5, ListMarker.Dot, "one");
            CheckIndentedCode(doc[1],  " two");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item.
        /// </summary>
        [Test]
        public void TestOrderedListE()
        {
            MarkdownDocument doc = Open(@"TestOrderedListE.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Parenthesis));
            CheckParagraph(orderedListItemBlock[0], "one");
            CheckParagraph(orderedListItemBlock[1], "two");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item
        /// when it is embedded in other constructions.
        /// </summary>
        [Test]
        public void TestOrderedListF()
        {
            MarkdownDocument doc = Open(@"TestOrderedListF.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)quoteBlock[0];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "one");
            CheckParagraph(orderedListItemBlock[1], "two");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how far content must be indented to be put under the list item
        /// when it is embedded in other constructions.
        /// </summary>
        [Test]
        public void TestOrderedListG()
        {
            MarkdownDocument doc = Open(@"TestOrderedListG.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)quoteBlock[0];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "one");

            CheckParagraph(nestedQuoteBlock[1], "two");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that at least one space is needed between the list marker and any following content.
        /// </summary>
        [Test]
        public void TestOrderedListH()
        {
            MarkdownDocument doc = Open(@"TestOrderedListH.md");

            CheckParagraph(doc[0], "1)one");
            CheckParagraph(doc[1], "2.two");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that a list item may contain blocks that are separated by more than one blank line.
        /// </summary>
        [Test]
        public void TestOrderedListI()
        {
            MarkdownDocument doc = Open(@"TestOrderedListI.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckParagraph(orderedListItemBlock[0], "foo");
            CheckParagraph(orderedListItemBlock[1], "bar");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that a list item may contain any kind of block.
        /// </summary>
        [Test]
        public void TestOrderedListJ()
        {
            MarkdownDocument doc = Open(@"TestOrderedListJ.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckParagraph(orderedListItemBlock[0], "foo");
            CheckFencedCode(orderedListItemBlock[1], "bar");
            CheckParagraph(orderedListItemBlock[2], "baz");

            QuoteBlock quoteBlock = (QuoteBlock)orderedListItemBlock[3];
            CheckParagraph(quoteBlock[0], "bam");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that a list item that contains an indented code block will
        /// preserve empty lines within the code block verbatim.
        /// </summary>
        [Test]
        public void TestOrderedListK()
        {
            MarkdownDocument doc = Open(@"TestOrderedListK.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckParagraph(orderedListItemBlock[0], "Foo");
            CheckIndentedCode(orderedListItemBlock[1], "bar\v\v\vbaz");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that an indented code block will have to be indented four spaces
        /// beyond the edge of the region where text will be included in the list item.
        /// </summary>
        [Test]
        public void TestOrderedListL()
        {
            MarkdownDocument doc = Open(@"TestOrderedListL.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckParagraph(orderedListItemBlock[0], "foo");
            CheckIndentedCode(orderedListItemBlock[1], "bar");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that if the first block in the list item is an indented code block,
        /// then the contents must be indented one space after the list marker.
        /// </summary>
        [Test]
        public void TestOrderedListM()
        {
            MarkdownDocument doc = Open(@"TestOrderedListM.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckIndentedCode(orderedListItemBlock[0], "indented code");
            CheckParagraph(orderedListItemBlock[1], "paragraph");
            CheckIndentedCode(orderedListItemBlock[2], "more code");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that if the first block in the list item is an indented code block,
        /// then the contents must be indented one space after the list marker.
        /// </summary>
        [Test]
        public void TestOrderedListN()
        {
            MarkdownDocument doc = Open(@"TestOrderedListN.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckIndentedCode(orderedListItemBlock[0], " indented code");
            CheckParagraph(orderedListItemBlock[1], "paragraph");
            CheckIndentedCode(orderedListItemBlock[2], "more code");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that if the first block in the list item is an indented code block,
        /// then the contents must be indented one space after the list marker.
        /// </summary>
        [Test]
        public void TestOrderedListO()
        {
            MarkdownDocument doc = Open(@"TestOrderedListO.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckParagraph(orderedListItemBlock[0], "foo");
            CheckParagraph(doc[1], "bar");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that the indentation can always be removed without a change in interpretation.
        /// </summary>
        [Test]
        public void TestOrderedListP()
        {
            MarkdownDocument doc = Open(@"TestOrderedListP.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckParagraph(orderedListItemBlock[0], "foo");
            CheckParagraph(orderedListItemBlock[1], "bar");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests list items that start with a blank line but are not empty.
        /// </summary>
        [Test]
        public void TestOrderedListQ()
        {
            MarkdownDocument doc = Open(@"TestOrderedListQ.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "foo");

            orderedListItemBlock = (OrderedListItemBlock)doc[1];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckFencedCode(orderedListItemBlock[0], "bar");

            orderedListItemBlock = (OrderedListItemBlock)doc[2];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckIndentedCode(orderedListItemBlock[0], "baz");

            Assert.That(doc.Count, Is.EqualTo(3));
        }


        /// <summary>
        /// Tests an empty list item.
        /// </summary>
        [Test]
        public void TestOrderedListS()
        {
            MarkdownDocument doc = Open(@"TestOrderedListS.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "foo");
            CheckOrderedListItem(doc[1], 1, ListMarker.Dot, "");
            CheckOrderedListItem(doc[2], 1, ListMarker.Dot, "bar");

            Assert.That(doc.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that it does not matter whether there are spaces
        /// following the list marker in an empty list item.
        /// </summary>
        [Test]
        public void TestOrderedListT()
        {
            MarkdownDocument doc = Open(@"TestOrderedListT.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "foo");
            CheckOrderedListItem(doc[1], 2, ListMarker.Dot, "");
            CheckOrderedListItem(doc[2], 3, ListMarker.Dot, "bar");

            Assert.That(doc.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that a list may start or end with an empty list item.
        /// </summary>
        [Test]
        public void TestOrderedListU()
        {
            MarkdownDocument doc = Open(@"TestOrderedListU.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Parenthesis, "");
            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that an empty list item cannot interrupt a paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListV()
        {
            MarkdownDocument doc = Open(@"TestOrderedListV.md");

            CheckParagraph(doc[0], "foo"+SoftBreak+"1.");
            CheckParagraph(doc[1], "foo"+SoftBreak+"2.");
            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that four spaces indent gives a code block instead of list item.
        /// </summary>
        [Test]
        public void TestOrderedListW()
        {
            MarkdownDocument doc = Open(@"TestOrderedListW.md");

            CheckIndentedCode(doc[0], "1.  A paragraph\v    with two lines.\v\v        indented code\v\v    > A block quote.");
            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that indentation can be partially deleted.
        /// </summary>
        [Test]
        public void TestOrderedListX()
        {
            MarkdownDocument doc = Open(@"TestOrderedListX.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "A paragraph"+SoftBreak+"with two lines.");
            Assert.That(doc.Count, Is.EqualTo(1));
        }


        /// <summary>
        /// Tests how laziness can work in nested structures.
        /// </summary>
        [Test]
        public void TestOrderedListZ()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)quoteBlock[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            QuoteBlock nestedQuoteBlock = (QuoteBlock)orderedListItemBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "Blockquote"+SoftBreak+"continued here.");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests nested lists.
        /// </summary>
        [Test]
        public void TestOrderedListZ1()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ1.md");

            // Root list.
            OrderedListItemBlock bulletListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(bulletListItemBlock[0], "foo");

            // The first nesting level.
            bulletListItemBlock = (OrderedListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(bulletListItemBlock[0], "bar");

            // The second nesting level.
            bulletListItemBlock = (OrderedListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(bulletListItemBlock[0], "baz");

            // The third nesting level.
            bulletListItemBlock = (OrderedListItemBlock)bulletListItemBlock[1];
            Assert.That(bulletListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(bulletListItemBlock[0], "boo");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that one indentation is not enough to make lists nested.
        /// </summary>
        [Test]
        public void TestOrderedListZ2()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ2.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "foo");
            CheckOrderedListItem(doc[1], 1, ListMarker.Dot, "bar");
            CheckOrderedListItem(doc[2], 1, ListMarker.Dot, "baz");

            Assert.That(doc.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that a list may be the first block in a list item.
        /// </summary>
        [Test]
        public void TestOrderedListZ3()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ3.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));

            CheckOrderedListItem(orderedListItemBlock[0], 7, ListMarker.Parenthesis, "foo");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that a list item can contain a heading.
        /// </summary>
        [Test]
        public void TestOrderedListZ4()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ4.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckAtxHeading(orderedListItemBlock[0], 1, "Foo");

            orderedListItemBlock = (OrderedListItemBlock)doc[1];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckSetextHeading(orderedListItemBlock[0], 2, "Bar");
            CheckParagraph(orderedListItemBlock[1], "baz");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that ordered list can interrupt a paragraph only if it starts at 1.
        /// </summary>
        [Test]
        public void TestOrderedListZ5()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ5.md");

            CheckParagraph(doc[0], "Foo"+SoftBreak+"2. bar");
            CheckOrderedListItem(doc[1], 1, ListMarker.Dot, "baz");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that there can be any number of blank lines between items.
        /// </summary>
        [Test]
        public void TestOrderedListZ6()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ6.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Parenthesis, "foo");
            CheckOrderedListItem(doc[1], 2, ListMarker.Parenthesis, "bar");
            CheckOrderedListItem(doc[2], 3, ListMarker.Parenthesis, "baz");

            Assert.That(doc.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that there can be any number of blank lines between items in nested lists.
        /// </summary>
        [Test]
        public void TestOrderedListZ7()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ7.md");

            // Root list.
            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "foo");

            // The first nesting level.
            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "bar");

            // The second nesting level.
            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "baz");
            CheckParagraph(orderedListItemBlock[1], "bim");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that to separate consecutive lists of the same type, or to separate a list from
        /// an indented code block that would otherwise be parsed as a subparagraph of the final
        /// list item, you can insert a blank HTML comment.
        /// </summary>
        /// <remarks>While HTML feature is not implemented, the HTML snippet is parsed as a paragraph.</remarks>
        [Test]
        public void TestOrderedListZ8()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ8.md");

            Assert.That(doc.Count, Is.EqualTo(5));

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "foo");
            CheckOrderedListItem(doc[1], 2, ListMarker.Dot, "bar");
            CheckParagraph(doc[2], "");
            CheckOrderedListItem(doc[3], 3, ListMarker.Dot, "baz");
            CheckOrderedListItem(doc[4], 4, ListMarker.Dot, "bim");
        }

        /// <summary>
        /// Tests that list items need not be indented to the same level. The following list items will be treated
        /// as items at the same list level, since none is indented enough to belong to the previous list item.
        /// </summary>
        [Test]
        public void TestOrderedListZ9()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ9.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "a");
            CheckOrderedListItem(doc[1], 2, ListMarker.Dot, "b");
            CheckOrderedListItem(doc[2], 3, ListMarker.Dot, "c");

            Assert.That(doc.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that list items may not be indented more than three spaces.
        /// </summary>
        [Test]
        public void TestOrderedListZ10()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ10.md");

            CheckOrderedListItem(doc[0], 1, ListMarker.Dot, "a");
            CheckOrderedListItem(doc[1], 2, ListMarker.Dot, "b");
            // Here, 3. c is treated as in indented code block,
            // because it is indented four spaces and preceded by a blank line.
            CheckIndentedCode(doc[2], "3. c");

            Assert.That(doc.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests list items with tab indentation.
        /// </summary>
        [Test]
        public void TestOrderedListZ11()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ11.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckParagraph(orderedListItemBlock[0], "Foo");
            CheckParagraph(orderedListItemBlock[1], "bar");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that horizontal rule falls under a list item in Quote if it has needed indentation.
        /// </summary>
        [Test]
        public void TestOrderedListZ12()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ12.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)quoteBlock[0];
            Assert.That(orderedListItemBlock.Marker, Is.EqualTo(ListMarker.Dot));
            CheckIndentedCode(orderedListItemBlock[0], "   foo");
            CheckHorizontalRule(orderedListItemBlock[1]);

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests nested Quotes and Lists with empty items.
        /// </summary>
        [Test]
        public void TestOrderedListZ13()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ13.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)quoteBlock[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)orderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedQuoteBlock[0];
            CheckOrderedListItem(nestedQuoteBlock[0], 1, ListMarker.Dot, "foo");

            orderedListItemBlock = (OrderedListItemBlock)quoteBlock[1];
            nestedQuoteBlock = (QuoteBlock)orderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedQuoteBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "bar");
        }

        /// <summary>
        /// Tests nested Quotes and Lists with non-empty items.
        /// </summary>
        [Test]
        public void TestOrderedListZ14()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ14.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckOrderedListItem(quoteBlock[0], 1, ListMarker.Dot, "foo >> 1. bar");
            CheckOrderedListItem(quoteBlock[1], 1, ListMarker.Dot, "bob >>   baz");
        }

        /// <summary>
        /// Tests bullet list many levels.
        /// </summary>
        [Test]
        public void TestOrderedListZ15()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ15.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedListItemBlock[0], "one");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "two");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "three");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "four");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "five");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "six");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "seven");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "eight");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "nine");

            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(orderedListItemBlock[0], "ten");

            Assert.That(doc.Count, Is.EqualTo(1));
        }


        /// <summary>
        /// Tests nested list items with Headings.
        /// </summary>
        [Test]
        public void TestOrderedListZ17()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ17.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedListItemBlock[0], "consectetur");

            OrderedListItemBlock nestedOrderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckParagraph(nestedOrderedListItemBlock[0], "adipiscing"+SoftBreak+"elit");
            CheckAtxHeading(nestedOrderedListItemBlock[1], 1, "sed");

            CheckSetextHeading(orderedListItemBlock[2], 2, "do");
        }

        /// <summary>
        /// Tests that in order to block fall into a list item inside a Quote, this block must be in Quote too.
        /// </summary>
        [Test]
        public void TestOrderedListZ18()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ18.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)quoteBlock[0];
            CheckHorizontalRule(orderedListItemBlock[0]);
            CheckParagraph(orderedListItemBlock[1], "aliqua.");

            OrderedListItemBlock nestedOrderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[2];
            // The line '1. enim' does not fall under the list item, but is lazy continuation of the previous paragraph.
            CheckParagraph(nestedOrderedListItemBlock[0], "Ut"+SoftBreak+"1. enim");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that if paragraph is not in list item, then empty list item cannot interrupt it
        /// and this is a lazy continuation of this paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListZ19()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ19.md");

            CheckParagraph(doc[0], "Foo"+SoftBreak+"1.");
            CheckOrderedListItem(doc[1], 1, ListMarker.Dot, "bar");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests complex nested bullet lists and quotes.
        /// </summary>
        [Test]
        public void TestOrderedListZ20()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ20.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];
            CheckOrderedListItem(quoteBlock[0], 1, ListMarker.Dot, "a");

            CheckOrderedListItem(doc[1], 1, ListMarker.Dot, "b");

            quoteBlock = (QuoteBlock)doc[2];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)quoteBlock[0];
            QuoteBlock nestedQuoteBlock = (QuoteBlock)orderedListItemBlock[0];
            orderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            nestedQuoteBlock = (QuoteBlock)orderedListItemBlock[0];
            CheckOrderedListItem(nestedQuoteBlock[0], 1, ListMarker.Dot, "c");

            orderedListItemBlock = (OrderedListItemBlock)quoteBlock[1];
            OrderedListItemBlock nestedOrderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedOrderedListItemBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedOrderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedOrderedListItemBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            CheckOrderedListItem(nestedOrderedListItemBlock[0], 1, ListMarker.Dot, "d");

            orderedListItemBlock = (OrderedListItemBlock)doc[3];
            nestedOrderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedOrderedListItemBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedOrderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedOrderedListItemBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedQuoteBlock[0];
            nestedOrderedListItemBlock = (OrderedListItemBlock)nestedOrderedListItemBlock[0];
            nestedQuoteBlock = (QuoteBlock)nestedOrderedListItemBlock[0];
            CheckParagraph(nestedQuoteBlock[0], "e");

            Assert.That(doc.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests lists in quote with headings.
        /// </summary>
        [Test]
        public void TestOrderedListZ21()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ21.md");

            QuoteBlock quoteBlock = (QuoteBlock)doc[0];

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)quoteBlock[0];
            CheckParagraph(orderedListItemBlock[0], "a");

            orderedListItemBlock = (OrderedListItemBlock)quoteBlock[1];
            CheckAtxHeading(orderedListItemBlock[0], 1, "b");

            orderedListItemBlock = (OrderedListItemBlock)quoteBlock[2];
            CheckSetextHeading(orderedListItemBlock[0], 2, "c");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how ordered list can break a paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListZ22()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ22.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckSetextHeading(orderedListItemBlock[0], 2, "Foo"+SoftBreak+"   2. Bar");
            CheckParagraph(orderedListItemBlock[1], "baz");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how ordered list can break a paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListZ23()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ23.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedListItemBlock[0], "Foo"+SoftBreak+"   2. ```"+SoftBreak+"   Bar");
            CheckFencedCode(orderedListItemBlock[1], "baz");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how ordered list can break a paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListZ24()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ24.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedListItemBlock[0], "Foo");

            // Nested list item.
            orderedListItemBlock = (OrderedListItemBlock)orderedListItemBlock[1];
            CheckFencedCode(orderedListItemBlock[0], "Bar");
            CheckParagraph(orderedListItemBlock[1], "baz");

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests how ordered list can break a paragraph.
        /// </summary>
        [Test]
        public void TestOrderedListZ25()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ25.md");

            CheckParagraph(doc[0], "Foo"+SoftBreak+"2.  ```Bar```"+SoftBreak+"baz");
            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "Foo"+SoftBreak+"2.  "),
                new ExpectedInline(BlockType.InlineCode, "```Bar```"),
                new ExpectedInline(BlockType.Inline, SoftBreak+"baz")});

            Assert.That(doc.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that ordered list start numbers must be nine digits or less.
        /// </summary>
        [Test]
        public void TestOrderedListZ26()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ26.md");
            CheckOrderedListItem(doc[0], 123456789, ListMarker.Dot, "ok");
        }

        /// <summary>
        /// Tests that ordered list start numbers must be nine digits or less.
        /// </summary>
        [Test]
        public void TestOrderedListZ27()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ27.md");
            CheckParagraph(doc[0], "1234567890. not ok");
        }

        /// <summary>
        /// Tests that a start number may begin with 0s.
        /// </summary>
        [Test]
        public void TestOrderedListZ28()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ28.md");
            CheckOrderedListItem(doc[0], 0, ListMarker.Dot, "ok");
            CheckOrderedListItem(doc[1], 3, ListMarker.Dot, "ok");
        }

        /// <summary>
        /// Tests that a start number may not be negative.
        /// </summary>
        [Test]
        public void TestOrderedListZ29()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ29.md");
            CheckParagraph(doc[0], "-1. not ok");
        }

        /// <summary>
        /// Tests that ordered list can interrupt a paragraph only if it starts at 1 and it is at the very start of the list.
        /// </summary>
        [Test]
        public void TestOrderedListZ30()
        {
            MarkdownDocument doc = Open(@"TestOrderedListZ30.md");

            OrderedListItemBlock orderedListItemBlock = (OrderedListItemBlock)doc[0];
            CheckParagraph(orderedListItemBlock[0], "a");
            CheckOrderedListItem(orderedListItemBlock[1], 1, ListMarker.Dot, "b");
            CheckOrderedListItem(orderedListItemBlock[2], 5, ListMarker.Parenthesis, "c");

            CheckOrderedListItem(doc[1], 8, ListMarker.Dot, "d");

            Assert.That(doc.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Lists\OrderedList\{0}", fileName));
        }
    }
}
