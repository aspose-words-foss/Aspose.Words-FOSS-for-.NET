// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderSection
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }




        /// <summary>
        /// Test all headers are the same in one section.
        /// </summary>
        [Test]
        public void TestDocBuilderHeaderSame()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Write("Header");

            builder.MoveToSection(0);
            builder.Writeln("Page1");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page2");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page3");

            TestUtil.Save(doc, @"DocBuilder\TestDocBuilderHeaderSame.docx", null, false);
        }

        /// <summary>
        /// Test headers different for first, even, odd pages in one section.
        /// </summary>
        [Test]
        public void TestDocBuilderHeaderDifferent()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.PageSetup.DifferentFirstPageHeaderFooter = true;
            builder.PageSetup.OddAndEvenPagesHeaderFooter = true;

            builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
            builder.Write("Header First");
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderEven);
            builder.Write("Header Even");
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Write("Header Odd");

            builder.MoveToSection(0);
            builder.Writeln("Page1");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page2");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page3");

            TestUtil.Save(doc, @"DocBuilder\TestDocBuilderHeaderDifferent.docx", null, false);
        }

        /// <summary>
        /// Headers in several sections, some linked some redefined.
        /// </summary>
        [Test]
        public void TestDocBuilderHeaderSections()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            //Create document with three sections
            builder.Writeln("Page1");
            builder.InsertBreak(BreakType.SectionBreakNewPage);
            builder.Writeln("Page2");
            builder.InsertBreak(BreakType.SectionBreakNewPage);
            builder.Writeln("Page3");

            //This header will apply to both first and second section because
            //the second section does not define any header.
            builder.MoveToSection(0);
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Write("Header1");

            //This header will apply to the third section.
            builder.MoveToSection(2);
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Write("Header2");

            TestUtil.Save(doc, @"DocBuilder\TestDocBuilderHeaderSections.docx", null, false);
        }



        /// <summary>
        /// WORDSNET-2159 “NullReferenceException” occurs when move to Header/Footer after linking header to previous.
        ///
        /// After set IsLinkedToPrevious to true in header/footer DocumentBuilder moves cursor to first paragraph of current section.
        /// </summary>
        [Test]
        public void TestJira2159()
        {
            Document doc = new Document();
            DocumentBuilder docBuild = new DocumentBuilder(doc);

            TestJira2159_Check(docBuild);

            docBuild.MoveToDocumentEnd();
            docBuild.InsertBreak(BreakType.SectionBreakNewPage);

            Assert.That(docBuild.CurrentSection, IsNot.SameAs(doc.FirstSection));
            TestJira2159_Check(docBuild);
        }

        private void TestJira2159_Check(DocumentBuilder docBuild)
        {
            docBuild.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            docBuild.Writeln("test");
            Assert.That(docBuild.CurrentSection, IsNot.Null());
            Assert.That(docBuild.CurrentStory, IsNot.Null());
            Assert.That(docBuild.CurrentParagraph, IsNot.Null());

            Section section = docBuild.CurrentSection;
            HeaderFooter header = (HeaderFooter)docBuild.CurrentStory;
            Assert.That(header.IsLinkedToPrevious, Is.False);

            header.IsLinkedToPrevious = true;
            Assert.That(docBuild.CurrentSection, Is.SameAs(section));
            Assert.That(docBuild.CurrentStory, Is.SameAs(section.Body));
            Assert.That(docBuild.CurrentParagraph, Is.SameAs(section.Body.FirstParagraph));

            // chack that we don't have exception as in WORDSNET-2159
            docBuild.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
        }
    }
}
