// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for headers and footers.
    /// </summary>
    [TestFixture]
    public class TestHeaders : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderSimple(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderSimple", lf, sf);
            Assert.That(doc.FirstSection.Body.GetText(), Is.EqualTo("Body\x000c"));
            Assert.That(doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("Header\r"));
            Assert.That(doc.SelectNodes("//HeaderFooter").Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Headers and footers same for all pages in one section.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderSame(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderSame", lf, sf);

            Section sect = doc.FirstSection;
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("SimpleHeader\r"));
            Assert.That(sect.HeadersFooters[HeaderFooterType.FooterPrimary].GetText(), Is.EqualTo("SimpleFooter\r"));
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderFirst], Is.EqualTo(null));
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderEven], Is.EqualTo(null));
        }

        /// <summary>
        /// Many sections, some headers are linked to previous, different first/even/odd.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderMany(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderMany", lf, sf);

            //The odd header is not really visible in the document because only two pages in the section.
            Section sect = doc.FirstSection;

            // RK I don't know why page number is 5 in the model when reading from DOC,
            // but in other formats it is clearly 1.
            string firstHeaderText = sect.HeadersFooters[HeaderFooterType.HeaderFirst].GetText();
            if (lf == LoadFormat.Doc)
            {
                Assert.That(firstHeaderText, Is.EqualTo("First Header 1\t\t\x0013 PAGE \x00145\x0015\r"));
            }
            else
            {
                Assert.That(firstHeaderText, Is.EqualTo("First Header 1\t\t\x0013 PAGE \x00141\x0015\r"));
            }

            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderEven].GetText(), Is.EqualTo("Even Header 1\r"));
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("Odd Header 1\r"));

            //There is only one page in this section, but all headers footers are linked to previous.
            sect = (Section)sect.NextSibling;
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderFirst], Is.EqualTo(null));
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderEven], Is.EqualTo(null));
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderPrimary], Is.EqualTo(null));

            sect = (Section)sect.NextSibling;
            //This is linked to previous
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderFirst], Is.EqualTo(null));
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderEven].GetText(), Is.EqualTo("\r"));
            //This is redefined
            Assert.That(sect.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("Odd Header 2\r"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderClear(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderClear", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("SimpleHeader\rSimpleFooter\rPage 1\r\x000cPage 2\x000c"));
            doc.FirstSection.ClearHeadersFooters();
            Assert.That(doc.GetText(), Is.EqualTo("Page 1\r\x000cPage 2\x000c"));
            
            TestUtil.SaveOpen(doc, @"Model\Header\TestHeaderClear Modified", lf, sf);
        }

        [Test]
        public void TestNoHeaders()
        {
            Document doc = new Document();
            Assert.That(doc.SelectNodes("//HeaderFooter").Count, Is.EqualTo(0));
        }

        [Test]
        public void TestHeaderFooterLinkToPrevious()
        {
            Document doc = new Document();

            //Create a header and check it is linked.
            HeaderFooter hf = new HeaderFooter(doc, HeaderFooterType.HeaderPrimary);
            doc.Sections[0].HeadersFooters.Add(hf);
            Assert.That(hf.IsLinkedToPrevious, Is.EqualTo(true));
            Assert.That(hf.IsHeader, Is.EqualTo(true));

            //Add some text to the header and check it is no longer linked.
            Paragraph para = new Paragraph(doc);
            para.AppendChild(new Run(doc, "Hello"));
            hf.AppendChild(para);
            Assert.That(hf.IsLinkedToPrevious, Is.EqualTo(false));

            //Setting linked to false a header that is not linked should not have changed it.
            hf.IsLinkedToPrevious = false;
            Assert.That(hf.GetText(), Is.EqualTo("Hello\r"));

            //Setting a non empty header to be linked clears text.
            hf.IsLinkedToPrevious = true;
            Assert.That(hf.HasChildNodes, Is.EqualTo(false));
        }

        [Test]
        public void TestHeadersFootersLinkToPrevious()
        {
            Document doc = new Document();

            //This actually creates all headers and footers.
            doc.Sections[0].HeadersFooters.LinkToPrevious(false);
            HeaderFooter hf = doc.Sections[0].HeadersFooters[HeaderFooterType.HeaderPrimary];
            Assert.That(hf.IsLinkedToPrevious, Is.EqualTo(false));

            doc.Sections[0].HeadersFooters.LinkToPrevious(true);
            Assert.That(hf.IsLinkedToPrevious, Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-26552 Consider adding an option into Section.ClearHeadersFooters to keep watermarks.
        /// Tests the retainWatermarks parameter for the Section.ClearHeadersFooters() public method.
        /// </summary>
        [Test]
        public void Test26552()
        {
            string[] watermarks = new[]
            {
                "PowerPlusWaterMarkObject254598344", "PowerPlusWaterMarkObject254598345", "PowerPlusWaterMarkObject254598343",
                "PowerPlusWaterMarkObject254598346", "PowerPlusWaterMarkObject254598347", "PowerPlusWaterMarkObject254598348",
                "PowerPlusWaterMarkObject254598346", "PowerPlusWaterMarkObject1292134439", "PowerPlusWaterMarkObject1296749533",
                "PowerPlusWaterMarkObject1296749533", "PowerPlusWaterMarkObject1292134437", "PowerPlusWaterMarkObject1292134438",
                "PowerPlusWaterMarkObject1292134439", "PowerPlusWaterMarkObject1292134437", "WordPictureWatermark1292364843",
                "WordPictureWatermark1292364844", "WordPictureWatermark1292364845", "PowerPlusWaterMarkObject1292134439",
                "WordPictureWatermark1292364843", "PowerPlusWaterNoMarkObject62518", "PowerPlusWaterNoMarkObject62518",
                "WordPictureWatermark19187", "PowerPlusWaterNoMarkObject62517", "PowerPlusWaterMarkObject254598344",
                "PowerPlusWaterMarkObject62517", "WordPictureWatermark100001", "WordPictureWatermark24319",
                "PowerPlusWaterMarkObject254598343", "PowerPlusWaterMarkObject254598346", "PowerPlusWaterMarkObject254598347",
                "PowerPlusWaterMarkObject206392", "WordPictureWatermark165523", "PowerPlusWaterMarkObject254598346",
                "PowerPlusWaterMarkObject254598349", "PowerPlusWaterMarkObject254598350", "PowerPlusWaterMarkObject132896",
                "WordPictureWatermark94552", "PowerPlusWaterMarkObject254598349", "PowerPlusWaterMarkObject62517",
                "PowerPlusWaterMarkObject254598344", "PowerPlusWaterMarkObject62517", "WordPictureWatermark100001",
                "WordPictureWatermark24319", "PowerPlusWaterMarkObject254598343", "PowerPlusWaterMarkObject206392",
                "PowerPlusWaterMarkObject254598347", "PowerPlusWaterMarkObject206392", "WordPictureWatermark165523",
                "PowerPlusWaterMarkObject254598346", "WordPictureWatermark94552", "PowerPlusWaterMarkObject254598350",
                "PowerPlusWaterMarkObject132896", "WordPictureWatermark94552", "PowerPlusWaterMarkObject254598349",
                "WordPictureWatermark197457890", "WordPictureWatermark197457891", "WordPictureWatermark197457892",
                "WordPictureWatermark197457890"
            };

            Document doc = TestUtil.Open(@"Model\Header\Test26552.docx");
            int nodeCount = doc.GetChildNodes(NodeType.Any, true).Count;

            foreach (Section sect in doc.Sections)
                sect.ClearHeadersFooters(true);

            Assert.That(doc.GetChildNodes(NodeType.Any, true).Count, Is.LessThan(nodeCount));
            Assert.That(nodeCount, Is.EqualTo(296));
            Assert.That(doc.GetChildNodes(NodeType.Any, true).Count, Is.EqualTo(249));

            int i = 0;
            foreach (Section sect in doc.Sections)
            {
                NodeCollection shapes = sect.GetChildNodes(NodeType.Shape, true);
                foreach (Shape shape in shapes)
                {
                    Assert.That(shape.Name, Is.EqualTo(watermarks[i]));
                    i++;
                }
                i++;
            }
        }

    }
}
