// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Drawing;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderRun
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }



        /// <summary>
        /// WORDSNET-10565 Issues with hebrew (right-to-left) text formatting.
        /// We should correct wrong bidi attribute set by customer to conform text displaying with MSW.
        /// If text has ltr characters and bidi is set to true
        /// or text has rtl characters and bidi is set to false,
        /// then we should split such text and write it separately
        /// with bidi=true for rtl and with bidi=false for ltr text.
        /// </summary>
        [Test]
        public void TestJira10565()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ParagraphFormat.Bidi = true;
            builder.Font.Bidi = true;

            // Ltr text.
            builder.Write("TODAY IS MONDAY");
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Run run = para.FirstRun;
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.False));

            builder.Write("TODAY-IS-MONDAY");
            run = (Run)run.NextSibling;
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.False));

            builder.Write("TODAY_IS_MONDAY");
            run = (Run)run.NextSibling;
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.False));

            // Rtl text.
            builder.Write("\u05D1\u05D3\u05D9\u05E7\u05EA \u05D1\u05E2\u05D9\u05D9\u05EA ");
            run = (Run)run.NextSibling;
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// WORDSNET-10679 Automatically switch Bidi context of DocumentBuilder for LTR/RTL mixed strings.
        /// See for details TestJira10565().
        /// </summary>
        [Test]
        public void TestJira10679()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Bidi = true;
            builder.Writeln("Hello שלום");

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);

            Assert.That(runs.Count, Is.EqualTo(2));

            Run run = (Run) runs[0];
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.False));

            run = (Run) runs[1];
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// WORDSNET-10667 Automatically switch Bidi context of DocumentBuilder for LTR/RTL mixed strings.
        /// See for details TestJira10565().
        /// </summary>
        [Test]
        public void TestJira10667()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ParagraphFormat.Bidi = true;
            builder.Font.Bidi = true;
            builder.Write("החזרת מכולה 40' ריקה מס' EISU 1775219 לדפו");

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);

            Assert.That(runs.Count, Is.EqualTo(3));

            Run run = (Run)runs[0];
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.Text, Is.EqualTo("החזרת מכולה 40' ריקה מס' "));

            run = (Run)runs[1];
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.False));
            Assert.That(run.Text, Is.EqualTo("EISU 1775219"));

            run = (Run)runs[2];
            Assert.That(run.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.Text, Is.EqualTo(" לדפו"));
        }
    }
}
