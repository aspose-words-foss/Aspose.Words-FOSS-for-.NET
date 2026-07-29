// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2015 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderOnlineVideo
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Test first simple overload -  insert online video from Url.
        /// </summary>
        [Test]
        public void TestInsertOnlineVideoFromUrl()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Pass direct url from youtu.be.
            string url = "https://youtu.be/g1N9ke8Prmk";
            double width = 360;
            double height = 270;
            Shape shape = builder.InsertOnlineVideo(url, width, height);
            CheckOnlineVideoPr(shape, url, YouTubeEmbedCode, width, height);

            // Pass "watch" url from www.youtube.com.
            url = "https://www.youtube.com/watch?v=g1N9ke8Prmk";
            width = 320;
            height = 230;
            shape = builder.InsertOnlineVideo(url, width, height);
            CheckOnlineVideoPr(shape, url, YouTubeEmbedCode, width, height);

            // Pass "watch" url from youtube.com with extra parameters in request.
            url = "https://youtube.com/watch?v=g1N9ke8Prmk&feature=youtu.be";
            width = 280;
            height = 190;
            shape = builder.InsertOnlineVideo(url, width, height);
            CheckOnlineVideoPr(shape, url, YouTubeEmbedCode, width, height);

            // Pass direct url from vimeo.com.
            url = "https://vimeo.com/52477838";
            width = 240;
            height = 150;
            shape = builder.InsertOnlineVideo(url, width, height);
            CheckOnlineVideoPr(shape, url, VimeoEmbedCode, width, height);

            // Pass wrong url - in this case the black image will be inserted to the document.
            url = "https://wrongUrl/Test";
            width = 200;
            height = 110;
            shape = builder.InsertOnlineVideo(url, width, height);
            CheckOnlineVideoPr(shape, url, string.Empty, width, height);
            Assert.That(doc.ComplianceInfo.IsDrawingExtensions, Is.True);

            TestUtil.SaveOpen(doc, @"DocBuilder\OnlineVideo\TestInsertOnlineVideoFromUrl", UnifiedScenario.Docx2Docx);
        }


        private static void CheckOnlineVideoPr(Shape shape, string videoUrl, string embebCode, double width, double height)
        {
            Assert.That(shape.Width, Is.EqualTo(width));
            Assert.That(shape.Height, Is.EqualTo(height));
            Assert.That(shape.HRef, Is.EqualTo(videoUrl));

            Assert.That(shape.HasImage, Is.True);

            DmlBlip dmlBlip = ((DmlPicture)shape.DmlNode).BlipFill.Blip;
            Assert.That(dmlBlip.Extensions.Count, Is.EqualTo(1));
            DmlExtension videoExtension = dmlBlip.Extensions[DmlExtensionUri.VideoPr];

            Assert.That(videoExtension.Uri, Is.EqualTo(DmlExtensionUri.VideoPr));

            DmlWebVideoProperties webVideoProperties = videoExtension.WebVideoPr;

            // The width/height of the video player. The default value for YouTube is 640/390.
            Assert.That(webVideoProperties.FrameWidth, Is.EqualTo(640));
            Assert.That(webVideoProperties.FrameHeight, Is.EqualTo(390));

            // Check EmbedHtml.
            Assert.That(webVideoProperties.EmbedHtml, Is.EqualTo(embebCode));
        }

        private const string VimeoEmbedCode = "<iframe src=\"https://player.vimeo.com/video/52477838?app_id=122963\" width=\"640\" height=\"360\" " +
        "frameborder=\"0\" allow=\"autoplay; fullscreen; picture-in-picture; clipboard-write; encrypted-media; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" title=\"Aspose\"></iframe>";

        private const string YouTubeEmbedCode = "<iframe width=\"640\" height=\"360\" " +
        "src=\"https://www.youtube.com/embed/g1N9ke8Prmk?feature=oembed\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" referrerpolicy=\"strict-origin-when-cross-origin\" allowfullscreen title=\"Aspose.Words - Read DOCX, PDF, MOBI\"></iframe>";
    }
}
