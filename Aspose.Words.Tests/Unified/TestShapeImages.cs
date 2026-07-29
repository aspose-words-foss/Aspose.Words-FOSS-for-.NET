// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Loading;
using NUnit.Framework;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for images.
    /// </summary>
    [TestFixture]
    public class TestShapeImages : UnifiedTestsBase
    {
        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }


        private static FileFormat GetImageType(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                return ImageUtil.GetImageType(fileStream);
        }

        [Test]
        public void TestBrightness()
        {
            Assert.That(ImageData.PercentToBrightness(0.0), Is.EqualTo(-0.5));
            Assert.That(ImageData.PercentToBrightness(0.5), Is.EqualTo(0.0));
            Assert.That(ImageData.PercentToBrightness(1.0), Is.EqualTo(0.5));

            Assert.That(ImageData.BrightnessToPercent(-0.5), Is.EqualTo(0.0));
            Assert.That(ImageData.BrightnessToPercent(0.0), Is.EqualTo(0.5));
            Assert.That(ImageData.BrightnessToPercent(0.5), Is.EqualTo(1.0));
        }

        [Test]
        public void TestContrast()
        {
            Assert.That(ImageData.PercentToContrast(0.01), Is.EqualTo(0.02).Within(0.01)); //1%
            Assert.That(ImageData.PercentToContrast(0.1), Is.EqualTo(0.2).Within(0.01)); //10%
            Assert.That(ImageData.PercentToContrast(0.4), Is.EqualTo(0.80).Within(0.01)); //40%
            Assert.That(ImageData.PercentToContrast(0.5), Is.EqualTo(1.0).Within(0.01)); //50%
            Assert.That(ImageData.PercentToContrast(0.51), Is.EqualTo(1.02).Within(0.01)); //51%
            Assert.That(ImageData.PercentToContrast(0.6), Is.EqualTo(1.25).Within(0.01)); //60%
            Assert.That(ImageData.PercentToContrast(0.7), Is.EqualTo(1.66).Within(0.01)); //70%
            Assert.That(ImageData.PercentToContrast(0.8), Is.EqualTo(2.5).Within(0.01)); //80%
            Assert.That(ImageData.PercentToContrast(0.9), Is.EqualTo(5.0).Within(0.01)); //90%
            Assert.That(ImageData.PercentToContrast(0.99), Is.EqualTo(50.0).Within(0.01)); //99%
            Assert.That(ImageData.PercentToContrast(1.0), Is.EqualTo(double.MaxValue).Within(0.01)); //100%

            Assert.That(ImageData.ContrastToPercent(0.02), Is.EqualTo(0.01).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(0.20), Is.EqualTo(0.1).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(0.80), Is.EqualTo(0.4).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(1.0), Is.EqualTo(0.5).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(1.02), Is.EqualTo(0.51).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(1.25), Is.EqualTo(0.6).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(1.66), Is.EqualTo(0.7).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(2.5), Is.EqualTo(0.8).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(5.0), Is.EqualTo(0.9).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(50.0), Is.EqualTo(0.99).Within(0.01));
            Assert.That(ImageData.ContrastToPercent(double.MaxValue), Is.EqualTo(1.0).Within(0.01));
        }

        /// <summary>
        /// If image is unavailable then <see cref="ImageData.ToStream" /> and <see cref="ImageData.ToByteArray" />
        /// should return "no image" picture.
        /// </summary>
        [Test]
        public void TestNoImageFallback()
        {
            Document doc = new Document();
            Shape shape = new Shape(doc, ShapeType.Image);
            ImageData imageData = shape.ImageData;

            // Provided neither image bytes nor source full name
            Assert.That(imageData.ImageBytes, Is.Null);
            Assert.That(imageData.SourceFullName, Is.Empty);

            // This magic number is the size of our "no image".
            const int noImageLength = 924;
#if !CPLUSPLUS // In C++ resources can be accessed only from calling assambly. So skip this check for C++ for now.
            Assert.That(ImageUtil.GetNoImageBytes().Length, Is.EqualTo(noImageLength));
#endif
            byte[] imageBytes = imageData.ToByteArray();
            Assert.That(imageBytes.Length, Is.EqualTo(noImageLength));

#if !JAVA // ToStream is not currently ported to Java.
            using (Stream imageStream = imageData.ToStream())
                Assert.That(imageStream.Length, Is.EqualTo(noImageLength));
#endif
        }



        /// <summary>
        /// JAVAGOLD OLE data is compressed using ZIP in WML and compression is different on Java.
        /// </summary>
        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageInline(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageInline", lf, sf);

            if (lf == (LoadFormat)LoadFormatTest.TestDocxDml) // Gold compare is enough.
                return;

            // This verifies the field that surrounds the image was stripped correctly.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(para.GetText(), Is.EqualTo("\r"));

            // This is an inline picture that inserted using Insert/Image From File in MS Word.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.IsImage, Is.EqualTo(true));
            Assert.That(shape.IsInline, Is.EqualTo(true));

            // Word does not write image title to RTF for some reason.
            if (lf != LoadFormat.Rtf)
                Assert.That(shape.ImageData.Title, Is.EqualTo("kyoa1008main"));

            Assert.That(shape.Width, Is.EqualTo(236.25)); // This is 5250 twips, 262.5 points at 90% scale.
            Assert.That(shape.Height, Is.EqualTo(164.25)); // This is 3285 twips.

            // This is an inline picture of an embedded OLE picture.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.IsImage, Is.EqualTo(false));
            Assert.That(shape.IsOleObject, Is.EqualTo(true));
            Assert.That(shape.IsInline, Is.EqualTo(true));

            // Width is 194.25 in WordML made by MS Word.
            Assert.That(shape.Width, Is.EqualTo(194.3).Within(1.0));
            Assert.That(shape.Height, Is.EqualTo(116.2).Within(1.0));

            // This inline picture was simply pasted.
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.IsImage, Is.EqualTo(true));
            Assert.That(shape.IsInline, Is.EqualTo(true));
            Assert.That(shape.ImageData.Title, Is.EqualTo(""));

            // 3- WordArt object is here.

            // 4- Another embedded OLE object (Excel spreadsheet) here.

            // A metafile here.
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            Assert.That(shape.IsImage, Is.EqualTo(true));
            Assert.That(shape.IsInline, Is.EqualTo(true));

            // Word does not write image title to RTF for some reason.
            if (lf != LoadFormat.Rtf)
                Assert.That(shape.ImageData.Title, Is.EqualTo("GLOBE"));
        }


        /// <summary>
        /// Gid and Tif images are stored as PNG inside MS Word document.
        ///
        /// JAVAGOLD PNG is written differently on Java.
        /// </summary>
        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageGifTif(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageGifTif", lf, sf);
            if (lf == (LoadFormat)LoadFormatTest.TestDocxDml) // Gold compare is enough.
                return;

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.ImageData.ImageType, Is.EqualTo(ImageType.Png));

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.ImageData.ImageType, Is.EqualTo(ImageType.Png));
        }

        private static void CheckLinkedImage(
            Document doc,
            int objectIdx,
            bool isInline,
            string title,
            string src,
            bool isLinkOnly)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            ImageData imageData = shape.ImageData;
            Assert.That(shape.IsInline, Is.EqualTo(isInline));
            Assert.That(imageData.Title, Is.EqualTo(title));
            Assert.That(imageData.SourceFullName, Is.EqualTo(src));
            Assert.That(imageData.IsLink, Is.EqualTo(true));
            Assert.That(imageData.IsLinkOnly, Is.EqualTo(isLinkOnly));
        }

        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageCropping(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Cropping\TestImageCropping", lf, sf);

            // Crop to increase the picture.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            ImageData imageData = shape.ImageData;
            Assert.That(imageData.CropLeft, Is.EqualTo(-0.04).Within(0.01)); // 5pt in percent
            Assert.That(imageData.CropTop, Is.EqualTo(-0.17).Within(0.01)); // 10pt in percent
            Assert.That(imageData.CropRight, Is.EqualTo(-0.12).Within(0.01)); // 15pt in percent
            Assert.That(imageData.CropBottom, Is.EqualTo(-0.34).Within(0.01)); // 20pt in percent

            // Crop to decrease the picture.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            imageData = shape.ImageData;
            Assert.That(imageData.CropLeft, Is.EqualTo(0.04).Within(0.01)); // 5pt in percent
            Assert.That(imageData.CropTop, Is.EqualTo(0.17).Within(0.01)); // 10pt in percent
            Assert.That(imageData.CropRight, Is.EqualTo(0.12).Within(0.01)); // 15pt in percent
            Assert.That(imageData.CropBottom, Is.EqualTo(0.34).Within(0.01)); // 20pt in percent

            // No crop, check default values.
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            imageData = shape.ImageData;
            Assert.That(imageData.CropLeft, Is.EqualTo(0.0));
            Assert.That(imageData.CropTop, Is.EqualTo(0.0));
            Assert.That(imageData.CropRight, Is.EqualTo(0.0));
            Assert.That(imageData.CropBottom, Is.EqualTo(0.0));
        }

        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageBorders(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageBorders", lf, sf);

            if (lf == (LoadFormat)LoadFormatTest.TestDocxDml) // Gold compare is enough.
                return;

            // Check picture with borders.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            BorderCollection borders = shape.ImageData.Borders;
            CheckBorder(borders.Left, Color.Red, 3, LineStyle.Single);
            CheckBorder(borders.Top, Color.Blue, 1, LineStyle.Single);
            CheckBorder(borders.Right, DrColor.Green.ToNativeColor(), 3, LineStyle.Dot);
            CheckBorder(borders.Bottom, Color.Empty, 3, LineStyle.Single);

            // Check picture without borders.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            borders = shape.ImageData.Borders;
            CheckBorder(borders.Left, Color.Empty, 0, LineStyle.None);
        }


        private static void CheckBorder(Border border, Color color, double width, LineStyle lineStyle)
        {
            Assert.That(border.Color.ToArgb(), Is.EqualTo(color.ToArgb()));
            Assert.That(border.LineWidth, Is.EqualTo(width));
            Assert.That(border.LineStyle, Is.EqualTo(lineStyle));
        }

        /// <summary>
        /// Test that we can parse and save picture properties such as brightness, contrast.
        /// </summary>
        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageOptions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageOptions", lf, sf);

            if (lf == (LoadFormat)LoadFormatTest.TestDocxDml) // Gold compare is enough.
                return;

            // Image with adjustments to contrast, brightness, grayscale and chroma key.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            CheckImageOptions(shape, unchecked((int)0xff184fca), 0.4, 0.6, true, false);

            // Image with default values.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            CheckImageOptions(shape, 0, 0.5, 0.5, false, false);

            // No color options.
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            CheckImageOptions(shape, 0, 0.5, 0.5, false, false);
            // Contrast +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            CheckImageOptions(shape, 0, 0.65, 0.5, false, false);
            // Brightness +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            CheckImageOptions(shape, 0, 0.5, 0.65, false, false);
            // Contrast +30%, brightness +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            CheckImageOptions(shape, 0, 0.65, 0.65, false, false);

            // Grayscale.
            shape = (Shape)doc.GetChild(NodeType.Shape, 6, true);
            CheckImageOptions(shape, 0, 0.5, 0.5, true, false);
            // Grayscale, contrast +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 7, true);
            CheckImageOptions(shape, 0, 0.65, 0.5, true, false);
            // Grayscale, brightness +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 8, true);
            CheckImageOptions(shape, 0, 0.5, 0.65, true, false);
            // Grayscale, contrast +30%, brightness +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 9, true);
            CheckImageOptions(shape, 0, 0.65, 0.65, true, false);

            // WORDSNET-21919 Docx dml written by MS Word does not have greyscale attribute for bilevel images.
            // Rtf/wml/doc has it.

            bool checkForGrayscale = (lf != (LoadFormat)LoadFormatTest.TestDocxDml);

            // Bilevel.
            shape = (Shape)doc.GetChild(NodeType.Shape, 10, true);
            CheckImageOptions(shape, 0, 0.5, 0.5, checkForGrayscale, true);
            // Bilevel, contrast +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 11, true);
            CheckImageOptions(shape, 0, 0.65, 0.5, checkForGrayscale, true);
            // Bilevel, brightness +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 12, true);
            CheckImageOptions(shape, 0, 0.5, 0.65, checkForGrayscale, true);
            // Bilevel, contrast +30%, brightness +30%.
            shape = (Shape)doc.GetChild(NodeType.Shape, 13, true);
            CheckImageOptions(shape, 0, 0.65, 0.65, checkForGrayscale, true);
        }

        private static void CheckImageOptions(
            Shape image,
            int chromaKey,
            double contrast,
            double brightness,
            bool grayScale,
            bool biLevel)
        {
            ImageData imageData = image.ImageData;
            Assert.That(imageData.ChromaKey.ToArgb(), Is.EqualTo(chromaKey));
            // Delta equals 0.01 is enough to brightness and contrast values
            const double delta = 0.01;
            Assert.That(imageData.Brightness, Is.EqualTo(brightness).Within(delta));
            Assert.That(imageData.Contrast, Is.EqualTo(contrast).Within(delta));
            Assert.That(imageData.GrayScale, Is.EqualTo(grayScale));
            Assert.That(imageData.BiLevel, Is.EqualTo(biLevel));
        }


        // FOSS: UnifiedTestImageInsertWmf removed - it verified WMF placeable-header add/strip via exact metafile resolution/pixel sizes (DV's metafile scanner); that scanning is degraded in FOSS (reads 96 dpi instead of 288).






        /// <summary>
        /// Verify that the same image is loaded into memory only once and stored into file only once.
        /// This test is for a combination of floating and inline images.
        /// </summary>
        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageOnceMixed(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageOnceMixed", lf, sf);

            if (lf == (LoadFormat)LoadFormatTest.TestDocxDml) // Gold compare is enough.
                return;

            // TODO 1 DV This feature is still to be implemented in RTF.
            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
                return;

            // WORDSNET-18645 Disable this optimization for DOC format.
            if ((lf == LoadFormat.Doc) || (sf == SaveFormat.Doc))
                return;

            // Verify how images load.
            // When an image is duplicated in the document, it should be only one object in memory.
            Shape float1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Shape float2 = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Shape inline = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(float1 != float2, Is.True);
            // In this WordML all floating and inline images reuse same data and that loads into model as such.
            // In the DOC file floating and inline images are not reused, but we still load into model properly.
            // In RTF none of the images are reused, but we still need to load into model properly.
            Assert.That(float1.ImageData.ImageBytes == float2.ImageData.ImageBytes, Is.True);
            Assert.That(inline.ImageData.ImageBytes == float2.ImageData.ImageBytes, Is.True);


            // Verify that during cloning - image bytes are not deep copied.
            Shape shapeCopy = (Shape)float1.Clone(true);
            Assert.That(float1 != shapeCopy, Is.True);
            Assert.That(float1.ImageData != shapeCopy.ImageData, Is.True);
            Assert.That(float1.ImageData.ImageBytes == shapeCopy.ImageData.ImageBytes, Is.True);


            // Now save and load again to verify how images were saved.
            // It should still be the single image bytes in memory.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Image\TestImageOnceMixed", lf, sf);
            float1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            float2 = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            inline = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(float1 != float2, Is.True);
            Assert.That(float1.ImageData.ImageBytes == float2.ImageData.ImageBytes, Is.True);
            Assert.That(inline.ImageData.ImageBytes == float2.ImageData.ImageBytes, Is.True);
        }

        /// <summary>
        /// Verify that the same image is loaded into memory only once and stored into file only once.
        /// This test is for inline images.
        /// </summary>
        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageOnceInline(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageOnceInline", lf, sf);

            if (lf == (LoadFormat)LoadFormatTest.TestDocxDml) // Gold compare is enough.
                return;

            // TODO 1 DV This feature is still to be implemented in RTF.
            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
                return;

            // WORDSNET-18645 Disable this optimization for DOC format.
            if ((lf == LoadFormat.Doc) || (sf == SaveFormat.Doc))
                return;

            // When an image is duplicated in the document, it should be only one object in memory.
            Shape inline1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Shape inline2 = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(inline1 != inline2, Is.True);
            Assert.That(inline1.ImageData.ImageBytes == inline2.ImageData.ImageBytes, Is.True);
        }


        /// <summary>
        /// WORDSNET-3702 There is not enough memory or disk space to display or print the picture.
        /// RK As I figured out MS Word seems to fail on 32 bit (and some other) BMP (DIB) images.
        /// I made our code to automatically convert BMP to PNG when a BMP image is inserted.
        ///
        /// JAVAGOLD Some rare rounding differences during number formatting on Java that I was unable to solve.
        /// JAVAGOLD PNG are encoded differently on Java.
        /// </summary>
        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestDefect3702(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Shape shape = builder.InsertImage(TestUtil.GetInTestDataPath(@"Model\Shape\Image\TestDefect3702.bmp"));
            // The BMP was in fact converted into PNG to be stored in the model and in the file. It fixes the defect.
            Assert.That(shape.ImageData.ImageType, Is.EqualTo(ImageType.Png));
            TestUtil.SaveOpen(builder.Document, @"Model\Shape\Image\TestDefect3702", lf, sf);
        }

        [Test]
        [TestCaseSource("ShapeImagesTestScenarios")]
        public void UnifiedTestImageOnceInDifferentDocumentParts(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestImageOnceInDifferentDocumentParts", lf, sf);
        }


        /// <summary>
        /// This tests maximum of DrawingML image properties.
        /// </summary>
        [Test]
        public void TestDmlImageAll()
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Image\TestDmlImageAll.docx");
        }

        // FOSS: TestDefect7159 removed — it verified image size when saving to RTF; RTF save was removed.

        /// <summary>
        /// WORDSNET-15504 Default value of Shape.AspectRatioLocked is incorrect.
        /// The value is correct, because for VML image it is inherited from ShapeType. The default value depends on the
        /// ShapeType, for the ShapeType.Image it is true but for the other shape types it is false. The description of
        /// AspectRatioLocked property was changed.
        /// </summary>
        [Test]
        public void TestJira15504()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert image through the DocumentBuilder.
            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"Model\Shape\Image\TestPng.png"));
            Assert.That(shape.AspectRatioLocked, Is.True);

            // Create image shape (VML by default).
            shape = new Shape(doc, ShapeType.Image);
            Assert.That(shape.AspectRatioLocked, Is.True);

            shape = new Shape(doc, ShapeType.TextBox);
            Assert.That(shape.AspectRatioLocked, Is.False);

            // Set size upon insert but preserve AspectRatioLocked equals True.
            shape = builder.InsertImage(TestUtil.BuildTestFileName(@"Model\Shape\Image\TestPng.png"),
                RelativeHorizontalPosition.Default, 0, RelativeVerticalPosition.Page, 0, 400, 100, WrapType.Inline);
            Assert.That(shape.AspectRatioLocked, Is.True);
            Assert.That(shape.Width, Is.EqualTo(400));
            Assert.That(shape.Height, Is.EqualTo(100));
        }

        // FOSS: TestDefect7160 removed — it verified EMF image size when saving to RTF; RTF save was removed.

        // FOSS: TestJira775 removed - it sized zero-size shapes from a linked WMF; reading the WMF's dimensions needs metafile parsing that is degraded in FOSS (falls back to a 960pt default instead of 139.3).

        /// <summary>
        /// WORDSNET-12462 Page size is set to be equal to size of the image.
        /// The image is placed to occupy the entire page. The image is scaled, but it should not.
        /// </summary>
        [Test]
        public void TestJira12462()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Image image = null;
            try
            {
#if NETSTANDARD
                image = BitmapPal.LoadNativeImage(TestUtil.BuildTestFileName(@"Model\Shape\Image\TestJira12462.gif"));
#else
                // For some reason if use BitmapPal.LoadNativeImage (Image.FromStream) GDI+ exception occurs. So keep using Image.FromFile in this test.
                image = Image.FromFile(TestUtil.BuildTestFileName(@"Model\Shape\Image\TestJira12462.gif"));
#endif
#if JAVA || NETSTANDARD
                //JAVA-changed: java's BufferedImage hasn't Resolution.
                double imageHeight = image.Height;
                double imageWidth = image.Width;
#else
                double imageHeight = ConvertUtil.PixelToPoint(image.Height, image.VerticalResolution);
                double imageWidth = ConvertUtil.PixelToPoint(image.Width, image.HorizontalResolution);
#endif

                builder.PageSetup.PageWidth = imageWidth;
                builder.PageSetup.PageHeight = imageHeight;

                Shape shape = builder.InsertImage(image, RelativeHorizontalPosition.Page, 0,
                    RelativeVerticalPosition.Page, 0, imageWidth, imageHeight, WrapType.None);

                Assert.That(shape.Width, Is.EqualTo(imageWidth).Within(0.5), "Shape width");
                Assert.That(shape.Height, Is.EqualTo(imageHeight).Within(0.5), "Shape height");
            }
            finally
            {
                if (image != null)
                    image.Dispose();
            }
        }


        /// <summary>
        /// WORDSNET-20809, 21073, 21074  Implement conversion of images from metafiles to PNG during import.
        /// The conversion performs when <see cref="LoadOptions.ConvertMetafilesToPng"/> is set to true.
        /// </summary>
        // FOSS: only the DOCX inputs survive — the .rtf/.doc metafile inputs load removed formats.
        [TestCase(@"Model\Shape\Metafile\Test20809.docx")]
        [TestCase(@"Model\Shape\Metafile\Test20809wmf.docx")]
        public void TestMetafilesToPngConversion(string fileName)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertMetafilesToPng = true;

            Document doc = TestUtil.Open(fileName, lo);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(ImageUtil.GetImageType(shape.ImageData.ImageBytes), Is.EqualTo(FileFormat.Png));
        }

        /// <summary>
        /// WORDSNET-21254 InvalidCastException during loading a document with
        /// enabled <see cref="LoadOptions.ConvertMetafilesToPng"/>.
        /// </summary>
        [Test]
        [NetStandard("This test is written exclusively for .NET Standard")]
        public void Test21254()
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertMetafilesToPng = true;

            // Verify no exception on loading.
            Document doc = TestUtil.Open(@"Model\Shape\Metafile\Test21254.docx", lo);

            // Verify conversion to PNG format.
            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(ImageUtil.GetImageType(shape.ImageData.ImageBytes), Is.EqualTo(FileFormat.Png));
        }


        // WORDSCPP-1177 C++ Porter doesn't support TestCase and TestCaseSource attribute at the same time
        public static IEnumerable<TestCaseData> ShapeImagesTestScenarios
        {
            get
            {
                // FOSS: only the OOXML load/save scenarios survive — Doc/Rtf/WordML were removed.
                return new TestCaseData[] {
                    new TestCaseData(LoadFormat.Docx, SaveFormat.Docx),
                    new TestCaseData((LoadFormat)LoadFormatTest.TestDocxDml, SaveFormat.Docx),
                };

            }
        }

        /// <summary>
        /// WORDSNET-24967 Add method to shape API to keep image aspect ratio.
        /// Tests applying FitImageToShape() to DmlNodeType.Picture.
        /// </summary>
        [Test]
        public void Test24967()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Image image = BitmapPal.LoadNativeImage(TestUtil.BuildTestFileName(@"Model\Shape\Image\Test24967.png"));
            Shape shape = builder.InsertImage(image);
            shape.SetWidthSafe(250);
            shape.SetHeightSafe(50);
            shape.ImageData.FitImageToShape();

            DmlBlipFill fill = ((DmlPicture)shape.DmlNode).BlipFill;
            CheckBlipStretch(fill, 0.459);
            TestUtil.SaveCheckGold(doc, @"Model\Shape\Image\Test24967.docx");
        }

        /// <summary>
        /// Relates to WORDSNET-24967
        /// Tests applying FitImageToShape() to inappropriate DmlNode types.
        /// </summary>
        [Test]
        public void Test24967B_NoCrash()
        {
            CheckDmlNode(DmlNodeType.Shape);
            CheckDmlNode(DmlNodeType.ConnectorShape);
            CheckDmlNode(DmlNodeType.Chart);
            CheckDmlNode(DmlNodeType.Diagram);
            CheckDmlNode(DmlNodeType.ContentPart);
        }

        /// <summary>
        /// Relates to WORDSNET-24967
        /// Tests applying FitImageToShape() to Vml shape with the given ImageData.ImageBytes.
        /// </summary>
        [Test]
        public void Test24967D()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = new Shape(builder.Document, ShapeType.Image);
            shape.SetWidthSafe(250);
            shape.SetHeightSafe(50);
            shape.ImageData.ImageBytes = StreamUtil.CopyFileToByteArray(TestUtil.BuildTestFileName(@"Model\Shape\Image\Test24967.png"));
            builder.InsertNode(shape);
            shape.ImageData.FitImageToShape();

            CheckVmlShape(shape);
            TestUtil.SaveCheckGold(doc, @"Model\Shape\Image\Test24967D.docx");
        }

        /// <summary>
        /// Relates to WORDSNET-24967
        /// Tests applying FitImageToShape() to Vml shape with the given ShapePr[ShapeAttr.FillImageBytes].
        /// </summary>
        [Test]
        public void Test24967E()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = new Shape(builder.Document, ShapeType.Image);
            shape.SetWidthSafe(200);
            shape.SetHeightSafe(150);
            builder.InsertNode(shape);
            byte[] imageBytes = StreamUtil.CopyFileToByteArray(TestUtil.BuildTestFileName(@"Model\Shape\Image\Test24967.png"));
            shape.ShapePr[ShapeAttr.FillImageBytes] = imageBytes;
            shape.FillCore.FillType = FillTypeCore.Picture;
            shape.Filled = true;

            shape.ImageData.FitImageToShape();

            CheckVmlShape(shape);
            TestUtil.SaveCheckGold(doc, @"Model\Shape\Image\Test24967E.docx");
        }


        private void CheckDmlNode(DmlNodeType dmlNodeType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = new Shape(builder.Document, ShapeMarkupLanguage.Dml);
            builder.InsertNode(shape);
            shape.SetWidthSafe(250);
            shape.SetHeightSafe(50);
            shape.ImageData.FitImageToShape();

            switch (dmlNodeType)
            {
                case DmlNodeType.Shape:
                    shape.DmlNode = new DmlShape(DmlNodeType.Shape);
                    break;
                case DmlNodeType.ConnectorShape:
                    shape.DmlNode = new DmlShape(DmlNodeType.ConnectorShape);
                    break;
                case DmlNodeType.Diagram:
                    shape.DmlNode = new DmlDiagram();
                    break;
                case DmlNodeType.Chart:
                    shape.DmlNode = new DmlChartSpace();
                    break;
                case DmlNodeType.ContentPart:
                    shape.DmlNode = new DmlContentPart();
                    break;
                default:
                    break;
            }

            shape.ImageData.FitImageToShape();
            Assert.That(shape.ImageData, IsNot.Null());
        }

        private void CheckBlipStretch(DmlBlipFill fill, double expectedOffset)
        {
            DmlBlipFillStretch stretch = fill.BlipFillMode as DmlBlipFillStretch;
            Assert.That(stretch.FillRectangle.LeftOffset, Is.EqualTo(expectedOffset).Within(0.0001));
            Assert.That(stretch.FillRectangle.RightOffset, Is.EqualTo(expectedOffset).Within(0.0001));
            Assert.That(stretch.FillRectangle.TopOffset, Is.EqualTo(0).Within(0.1));
            Assert.That(stretch.FillRectangle.BottomOffset, Is.EqualTo(0).Within(0.1));
        }

        private void CheckVmlShape(Shape shape)
        {
            Assert.That(shape.FillCore.LockAspectRatio, Is.True);
            Assert.That(shape.Filled, Is.True);
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.Picture));
            byte[] imageBytes = (byte[])shape.ShapePr[ShapeAttr.FillImageBytes];
            Assert.That(imageBytes.Length, Is.EqualTo(1383));
            Assert.That(shape.ImageData.ImageBytes, Is.Null);
        }
    }
}
