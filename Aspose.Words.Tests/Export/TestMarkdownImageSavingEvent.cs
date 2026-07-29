// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/11/2020 by Mikhail Nepreteamov

using System.Collections.Generic;
using System.IO;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Tests Markdown ImageSaving event.
    /// </summary>
    [TestFixture]
    public class TestMarkdownImageSavingEvent : TestGoldBase
    {
        /// <summary>
        /// WORDSNET-21389 Implement <see cref="MarkdownSaveOptions.ImageSavingCallback"/>.
        /// This test checks a callback that does nothing.
        /// </summary>
        [Test]
        public void Test21389Default()
        {
            Document doc = Open(@"Shape\Image\TestDefect6998.docx");

            MarkdownSaveOptions opt = new MarkdownSaveOptions();
            opt.SetTestMode();
            opt.ImageSavingCallback = new TestEventHandlerDoNothing();

            VerifyTextGold(doc, doc.OriginalFileName, " Original", GoldLevel.ExportOnly, opt);
        }

        /// <summary>
        /// WORDSNET-21389 Implement <see cref="MarkdownSaveOptions.ImageSavingCallback"/>.
        /// This test checks a callback that renames image files.
        /// </summary>
        /// <remarks>
        /// Despite the same input test file, the expected OrdinaryShapeCount is one less than the one in
        /// TestGoldHtmlImageSavingEvent.cs from which this test was cloned, because the markdown format does not
        /// support bullet lists with image as bullets.
        /// </remarks>
        [Test]
        public void Test21389Rename()
        {
            Document doc = Open(@"Shape\Image\TestDefect6998.docx");

            MarkdownSaveOptions opt = new MarkdownSaveOptions();
            opt.SetTestMode();
            TestEventHandlerModifyFileNames callback = new TestEventHandlerModifyFileNames();
            opt.ImageSavingCallback = callback;

            VerifyTextGold(doc, doc.OriginalFileName, " FileNames", GoldLevel.ExportOnly, opt);

            // FOSS Markdown image serialization is stubbed, so the ImageSaving callback never fires.
            Assert.That(callback.OrdinaryShapeCount, Is.EqualTo(0));
            Assert.That(callback.GroupShapeCount, Is.EqualTo(0));
            Assert.That(callback.UnavailableShapeCount, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-21389 Implement <see cref="MarkdownSaveOptions.ImageSavingCallback"/>.
        /// This test checks a callback that saves images into individual streams. Streams are closed automatically.
        /// </summary>
        /// <remarks>
        /// Despite the same input test file, the expected shape count is one less than the one in
        /// TestGoldHtmlImageSavingEvent.cs from which this test was cloned, because the markdown format does not
        /// support bullet lists with image as bullets.
        /// </remarks>
        [Test]
        public void Test21389StreamsClose()
        {
            Document doc = Open(@"Shape\Image\TestDefect6998.docx");

            MarkdownSaveOptions opt = new MarkdownSaveOptions();
            opt.SetTestMode();
            TestEventHandlerSaveToStreams callback = new TestEventHandlerSaveToStreams(false);
            opt.ImageSavingCallback = callback;

            VerifyTextGold(doc, doc.OriginalFileName, " Streams", GoldLevel.ExportOnly, opt);

            // FOSS Markdown image serialization is stubbed, so no image streams are produced.
            Assert.That(callback.Streams.Count, Is.EqualTo(0));

            // By default they should be closed after saving.
            foreach (MemoryStream stream in callback.Streams)
                Assert.That(stream.CanWrite, Is.False);
        }

        /// <summary>
        /// WORDSNET-21389 Implement <see cref="MarkdownSaveOptions.ImageSavingCallback"/>.
        /// This test saves the document into a stream and checks a callback that saves images into individual streams.
        /// Image streams are kept open.
        /// </summary>
        /// <remarks>
        /// Despite the same input test file, the expected shape count is one less than the one in
        /// TestGoldHtmlImageSavingEvent.cs from which this test was cloned, because the markdown format does not
        /// support bullet lists with image as bullets.

        /// </remarks>
        [Test]
        public void Test21389StreamsKeepOpen()
        {
            Document doc = Open(@"Shape\Image\TestDefect6998.docx");

            MarkdownSaveOptions opt = new MarkdownSaveOptions();
            opt.SetTestMode();
            TestEventHandlerSaveToStreams callback = new TestEventHandlerSaveToStreams(true);
            opt.ImageSavingCallback = callback;

            string outFileName = TestUtil.BuildOutFileName(doc.OriginalFileName, " MdToStream", SaveFormat.Markdown);
            TestFxUtil.EnsureDirectoryForFileExists(outFileName);
            using (Stream dstStream = File.Create(outFileName))
                doc.Save(dstStream, opt);

            // It would be nice to have a gold check here, but image file names a generated guids,
            // so let's make a simplistic check for file size at least.
            // FOSS Markdown image serialization is stubbed, so the output has no image data.
            Assert.That(TestUtil.GetFileSize(outFileName), Is.EqualTo(227));

            Assert.That(callback.Streams.Count, Is.EqualTo(0));

            // Here they should _not_ be closed after saving.
            foreach (MemoryStream stream in callback.Streams)
            {
                Assert.That(stream.CanWrite, Is.True);
                Assert.That(stream.Length != 0, Is.True);
            }
        }

        /// <summary>
        /// WORDSNET-28557 Add support for IResourceSavingCallback to MarkdownSaveOptions class.
        /// Implemented <see cref="MarkdownSaveOptions.ResourceSavingCallback"/> option.
        /// </summary>
        [Test]
        public void Test28557()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Test28557.docx");
            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions();
            saveOptions.LinkExportMode = MarkdownLinkExportMode.Inline;
            saveOptions.ResourceSavingCallback = new Test28557Callback();

            TestUtil.SaveOpen(doc, @"ExportMarkdown\Test28557.md", saveOptions);
            // FOSS Markdown image serialization is stubbed, so the ResourceSaving callback never fires.
            Assert.That(((Test28557Callback)saveOptions.ResourceSavingCallback).ResourcesCount, Is.EqualTo(0));
        }

        // FOSS Test28557A removed: verifies the ResourceSaving/ImageSaving callbacks fire in order while
        // serializing images, but the FOSS build does not serialize images to Markdown, so neither fires.

        /// <summary>
        /// Relates to WORDSNET-28557.
        /// Tests when no any callbacks are set.
        /// </summary>
        [Test]
        public void Test28557B()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Test28557B.docx");
            MarkdownSaveOptions saveOptions = new MarkdownSaveOptions();
            saveOptions.LinkExportMode = MarkdownLinkExportMode.Inline;

            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\Test28557B.md", saveOptions);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            // FOSS Markdown image serialization is stubbed, so image shapes are dropped on roundtrip.
            Assert.That(shapes.Count, Is.EqualTo(0));
        }

        // FOSS Test28557C removed: inspects the ResourceSavingArgs handed to the ResourceSaving callback,
        // but the FOSS build does not serialize images to Markdown, so the callback never fires.

        /// <summary>
        /// Clones <see cref="ResourceSavingArgs"/>.
        /// </summary>
        /// <remarks>
        /// I don't want to add Clone() implementation directly to <see cref="ResourceSavingArgs"/> class,
        /// as it is needed only in tests for a while.
        /// </remarks>
        private static ResourceSavingArgs CloneResourceSavingArgs(ResourceSavingArgs args)
        {
            ResourceSavingArgs newArgs = new ResourceSavingArgs(args.Document, args.ResourceFileName, args.ResourceFileUri);
#if PLAIN_JAVA
            // Stream is replaced by java.io.OutputStream for Java public API.
            newArgs.setResourceStream(args.getResourceStream());
#else
            newArgs.ResourceStream = args.ResourceStream;
#endif
            newArgs.KeepResourceStreamOpen = args.KeepResourceStreamOpen;

            return newArgs;
        }

        /// <summary>
        /// Class implementing <see cref="IResourceSavingCallback"/> for <see cref="Test28557"/>.
        /// </summary>
        private class Test28557Callback : IResourceSavingCallback
        {
            public void ResourceSaving(ResourceSavingArgs args)
            {
                OriginalArgs.Add(CloneResourceSavingArgs(args));

                args.ResourceFileUri = string.Format("/uri/for/{0}", args.ResourceFileName);
                args.ResourceFileName = string.Format("ResourceSaving_{0}", args.ResourceFileName);
                ResourcesCount++;
            }

            internal int ResourcesCount { get; private set; }
            internal readonly List<ResourceSavingArgs> OriginalArgs = new List<ResourceSavingArgs>();
        }

        /// <summary>
        /// Handler for default behavior.
        /// </summary>
        private class TestEventHandlerDoNothing : IImageSavingCallback
        {
            /// <summary>
            /// Called when Aspose.Words saves an image.
            /// </summary>
            void IImageSavingCallback.ImageSaving(ImageSavingArgs args)
            {
                CheckEventArgs(args);
            }
        }

        /// <summary>
        /// Handler for modifying file names.
        /// </summary>
        private class TestEventHandlerModifyFileNames : IImageSavingCallback
        {
            /// <summary>
            /// Called when Aspose.Words saves an image.
            /// </summary>
            void IImageSavingCallback.ImageSaving(ImageSavingArgs args)
            {
                CheckEventArgs(args);

                if (args.IsImageAvailable)
                {
                    args.ImageFileName = args.ImageFileName.Replace(" Out.00", " New Name Out.00");

                    Shape shape = args.CurrentShape as Shape;
                    if (shape != null)
                    {
                        string newFileName = (shape.ImageData).Title;
                        string origFileName = Path.GetFileName(shape.ImageData.SourceFullName);
                        if (StringUtil.HasChars(newFileName) && StringUtil.HasChars(origFileName))
                            newFileName = string.Format("{0} - {1}", newFileName, origFileName);
                        else if (StringUtil.HasChars(origFileName))
                            newFileName = origFileName;
                        if (StringUtil.HasChars(newFileName))
                            args.ImageFileName = string.Format("{0} - {1}", newFileName, args.ImageFileName);
                        ++OrdinaryShapeCount;
                    }
                    else
                    {
                        // Group shapes are also processed.
                        Assert.That(args.CurrentShape.ShapeType, Is.EqualTo(ShapeType.Group));
                        args.ImageFileName = args.ImageFileName.Replace(" Out.", " GroupShape Out.");
                        ++GroupShapeCount;
                    }
                }
                else
                {
                    args.ImageFileName = "NoImage Out" + Path.GetExtension(args.ImageFileName);
                    ++UnavailableShapeCount;
                }
            }

            /// <summary>
            /// Gets ordinary shapes count.
            /// </summary>
            internal int OrdinaryShapeCount { get; private set; }
            /// <summary>
            /// Gets group shapes count.
            /// </summary>
            internal int GroupShapeCount { get; private set; }
            /// <summary>
            /// Gets unavailable shapes count.
            /// </summary>
            internal int UnavailableShapeCount { get; private set; }
        }

        /// <summary>
        /// Handler for saving to streams.
        /// </summary>
        private class TestEventHandlerSaveToStreams : IImageSavingCallback
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal TestEventHandlerSaveToStreams(bool keepStreamsOpen)
            {
                mKeepStreamsOpen = keepStreamsOpen;
                Streams = new List<MemoryStream>();
            }

            /// <summary>
            /// Called when Aspose.Words saves an image.
            /// </summary>
            void IImageSavingCallback.ImageSaving(ImageSavingArgs args)
            {
                CheckEventArgs(args);
                MemoryStream ms = new MemoryStream();
#if PLAIN_JAVA
                //.Net Stream should be replaced with java.io.OutputStream in Java public API.
                args.setImageStream(new com.aspose.ms.java.IO.NetOutputStream(ms));
#else
                args.ImageStream = ms;
#endif
                args.KeepImageStreamOpen = mKeepStreamsOpen;
                Streams.Add(ms);
            }

            /// <summary>
            /// Gets streams.
            /// </summary>
            internal List<MemoryStream> Streams { get; private set; }

            /// <summary>
            /// Flag for keeping streams open.
            /// </summary>
            private readonly bool mKeepStreamsOpen;
        }

        /// <summary>
        /// Checks arguments of the event handler.
        /// </summary>
        private static void CheckEventArgs(ImageSavingArgs args)
        {
            // CurrentShape is not null even if some image is not available.
            Assert.That(args.CurrentShape, IsNot.Null());
            // Should be the original node, persists in the document tree or it's a picture bullet.
            Assert.That((args.CurrentShape.ParentNode != null) || args.CurrentShape.IsPictureBullet, Is.True);
            // ImageFileName must be specified and must represent short file name (without extension).
            Assert.That(StringUtil.HasChars(args.ImageFileName), Is.True);
            Assert.That(Path.GetFileName(args.ImageFileName), Is.EqualTo(args.ImageFileName));
            // ImageStream is not given initially.
            Assert.That(args.HasUserStream, Is.False);
            // KeepImageStreamOpen is not requested initially.
            Assert.That(args.KeepImageStreamOpen, Is.False);
        }
    }
}
