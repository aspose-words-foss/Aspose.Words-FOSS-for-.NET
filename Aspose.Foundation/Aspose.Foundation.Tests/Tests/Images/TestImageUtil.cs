// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Images
{
    [TestFixture]
    public class TestImageUtil
    {
        [TestCase("CMYK\\TestCMYKImages_8.jpeg", true)]
        [TestCase("TestWrongHeight.jpg", true)]
        [TestCase("TestWrongHeightGrayscale.jpg", false)]
#if !JAVA
        // not work in Java
        [TestCase("CMYK\\TestCMYKImages_10.jpeg", true)]
#endif
        [TestCase("CMYK\\TestCMYKImages_10_gray.png", false)]
        [TestCase("CMYK\\TestCMYKImages_9.jpeg", true)]
        [TestCase("CMYK\\TestCMYKImages_9_gray.jpeg", false)]
        [TestCase("TestDefect11932.jpg", true)]
        [TestCase("TestDefect11932_gray.jpg", false)]
        [TestCase("TestJpegSize.jpg", true)]
        [TestCase("TestJpegSize_gray.jpg", false)]
        [TestCase("TestJiraJ1847.gif", true)]
        [TestCase("TestJiraJ1847_gray.jpg", false)]
        [TestCase("CMYK\\TestJira6368_1.jpeg", true)]
        [TestCase("CMYK\\TestJira6368_1_gray.jpg", false)]
        public void Test24480(string testName, bool isColored)
        {
            string testFileName = TestFxUtil.BuildTestFileName(string.Format(@"Images\{0}", testName));
            using (Stream stream = File.OpenRead(testFileName))
            {
                byte[] imageBytes = StreamUtil.CopyStreamToByteArray(stream);
                Assert.That(ImageUtil.IsImageColored(imageBytes), Is.EqualTo(isColored));
            }
        }

        /// <summary>
        /// Reading resolution in Photoshop metadata.
        /// </summary>
        [Test]
        public void Test23954()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\Test23594.jpeg"));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(300));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(300));
        }

        /// <summary>
        /// WORDSNET-1463, fixed - InsertImage scales image size to 60%.
        /// </summary>
        [Test]
        public void TestDefect1463()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestDefect1463.jpg"));
            Assert.That(imageSize.Width, Is.EqualTo(595));
            Assert.That(imageSize.Height, Is.EqualTo(149));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(72.0));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(72.0));

            imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestDefect1463 2.jpg"));
            Assert.That(imageSize.Width, Is.EqualTo(80));
            Assert.That(imageSize.Height, Is.EqualTo(34));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(96.0));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(96.0));
        }

        [Test]
        public void TestWrongHeight()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestWrongHeight.jpg"));

            Assert.That(imageSize.Width, Is.EqualTo(3072));
            Assert.That(imageSize.Height, Is.EqualTo(2304));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(72.0));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(72.0));
        }

        [Test]
        public void TestGif()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\AWReference.gif"));

            Assert.That(imageSize.Width, Is.EqualTo(467));
            Assert.That(imageSize.Height, Is.EqualTo(382));
        }

        /// <summary>
        /// WORDSNET-4239 Cannot insert PNG image, error Unable to read beyond the end of the stream.
        /// </summary>
        [Test]
        public void TestDefect4239()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestDefect4239.png"));
            Assert.That(imageSize.Width, Is.EqualTo(1200));
            Assert.That(imageSize.Height, Is.EqualTo(500));
        }

        [Test]
        public void TestTiffMacByteOrder()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestTiffMacByteOrder.tif"));

            Assert.That(imageSize.Width, Is.EqualTo(123));
            Assert.That(imageSize.Height, Is.EqualTo(321));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(72.0));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(72.0));
        }

        [Test]
        public void TestTiffIBMByteOrder()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestTiffIBMByteOrder.tif"));

            Assert.That(imageSize.Width, Is.EqualTo(123));
            Assert.That(imageSize.Height, Is.EqualTo(321));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(72.0));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(72.0));
        }

        /// <summary>
        /// Fixed. GetImageSize returns negative value.
        /// </summary>
        [Test]
        public void TestDefect9161()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Rendering\Aps\TestImages\TestImage FormatBmp24bitR8G8B8FlipRows.bmp"));

            Assert.That(imageSize.Width, Is.EqualTo(100));
            Assert.That(imageSize.Height, Is.EqualTo(100));
        }

        [Test]
        public void TestJpegSize()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestJpegSize.jpg"));

            Assert.That(imageSize.Width, Is.EqualTo(196));
            Assert.That(imageSize.Height, Is.EqualTo(266));
            Assert.That(imageSize.HorizontalResolution, Is.EqualTo(96.0));
            Assert.That(imageSize.VerticalResolution, Is.EqualTo(96.0));
        }

        /// <summary>
        /// WORDSNET-11932 "DivideByZeroException" exception occurs, when try to insert image into a document.
        /// The problem occurred because denominator in the image is zero, so DivideByZeroException exception occurs.
        /// Fixed by adding a condition to avoid dividing by zero.
        /// </summary>
        [Test]
        public void TestDefect11932()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestDefect11932.jpg"));
            Assert.That((float)imageSize.HorizontalResolution, Is.EqualTo(ImageConstants.StandardResolution));
            Assert.That((float)imageSize.VerticalResolution, Is.EqualTo(ImageConstants.StandardResolution));
            Assert.That(imageSize.Width, Is.EqualTo(408));
            Assert.That(imageSize.Height, Is.EqualTo(306));
        }

        /// <summary>
        /// WORDSNET-5142 Image size is incorrect during converting to XPS.
        /// The problem occurred because resolution read from Exif is 0 dpi, there is no actualy value of resolution.
        /// So default resolution was used. To resolve the problem check whether values read from Exif are not zeros.
        /// </summary>
        [Test]
        public void TestDefectJira5142()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestDefectJira5142.jpg"));
            Assert.That((float)imageSize.HorizontalResolution, Is.EqualTo(300f));
            Assert.That((float)imageSize.VerticalResolution, Is.EqualTo(300f));
            Assert.That(imageSize.Width, Is.EqualTo(604));
            Assert.That(imageSize.Height, Is.EqualTo(660));
        }

        [TestCase("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"200\" height=\"200\"></svg>")]
        [TestCase(" \t<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\" width=\"200\" height=\"200\"></svg>")]
        [TestCase("<?xml version='1.0' encoding='UTF-8' standalone='no'?><svg xmlns=\"http://www.w3.org/2000/svg\"></svg>")]
        [TestCase("<?xml version='1.0' encoding='UTF-8' standalone='no'?><!DOCTYPE svg><svg xmlns=\"http://www.w3.org/2000/svg\"></svg>")]
        public void TestIsSvgTrue(string svgString)
        {
            Assert.That(CheckSvgString(svgString), Is.True);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t\t")]
        [TestCase("this is simple text")]
        [TestCase("http://www.w3.org/2000/svg")]
        // HTML document with an embedded SVG node should not be considered an SVG image itself.
        [TestCase(@"<html><div><svg xmlns='http://www.w3.org/2000/svg'></svg></div></html>")]
        public void TestIsSvgFalse(string svgContent)
        {
            Assert.That(CheckSvgString(svgContent), Is.False);
        }

        /// <summary>
        /// WORDSNET-10356 DocumentBuilder.InsertImage lose original size of image.
        /// Image resolution was read from Exif in centimeters, but we always store it in the model in inches.
        /// </summary>
        [Test]
        public void TestJira10356()
        {
            ImageSizeCore imageSize = ImageUtil.GetImageSize(TestFxUtil.BuildTestFileName(@"Images\TestJira10356.jpg"));

            // The resolution of this image should be 93.98 inches (37 centimeters).
            const double resolutionInch = 93.98;
            Assert.That(MathUtil.AreEqual(resolutionInch, imageSize.HorizontalResolution, 0.01), Is.True);
            Assert.That(MathUtil.AreEqual(resolutionInch, imageSize.VerticalResolution, 0.01), Is.True);
        }

        /// <summary>
        /// WORDSJAVA-1650 Converting png with alpha to bmp throws IndexOutOfBoundsException.
        /// </summary>
        [Test]
        public void TestJira1650()
        {
            // Input
            byte[] imageBytes = StreamUtil.CopyFileToByteArray(TestFxUtil.BuildTestFileName(@"Images\TestJira1650.png"));
            BitmapPal bitmap = new BitmapPal(imageBytes);

            // Output
            MemoryStream outputSgtream = new MemoryStream();
            bitmap.SaveBmp(outputSgtream);
        }

        /// <summary>
        /// WORDSNET-20272 Some format detection ("IsXxx)" methods raised exceptions if there were not enough input data.
        /// All these methods are expected to return <c>false</c> in this case.
        /// </summary>
        [Test]
        public void Test20272IsXxx()
        {
            byte[] data = { 0x00 };
            using (MemoryStream stream = new MemoryStream(data))
            {
                Assert.That(ImageUtil.IsTiff(stream), Is.False);
                stream.Position = 0;

                Assert.That(ImageUtil.IsJpeg(stream), Is.False);
                stream.Position = 0;

                Assert.That(ImageUtil.IsPng(stream), Is.False);
                stream.Position = 0;

                Assert.That(ImageUtil.IsBmp(stream), Is.False);
                stream.Position = 0;

                Assert.That(ImageUtil.IsPict(stream), Is.False);
                stream.Position = 0;

                Assert.That(ImageUtil.IsIco(stream), Is.False);
                stream.Position = 0;
            }
            Assert.That(ImageUtil.IsDib(data), Is.False);
        }

        /// <summary>
        /// Returns true if the specified string is SVG.
        /// </summary>
        private static bool CheckSvgString(string data)
        {
            return ImageUtil.IsSvg(Encoding.UTF8.GetBytes(data));
        }

        [TestCase(@"Images\CMYK\Test18764_0.jpeg", true)]
        [TestCase(@"Images\CMYK\Test18764_1.jpeg", true)]
        [TestCase(@"Images\CMYK\Test420CMYKImage_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYK517_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCmykImage.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCmykImageDefectJ21_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCmykImageDefectJ422_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCmykImagePhotoshop.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_1.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_10.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_11.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_12.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_2.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_3.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_4.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_5.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_6.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_7.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_8.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKImages_9.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKJ595_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestCMYKJ595_1.jpeg", true)]
        [TestCase(@"Images\CMYK\TestDefect23947_1.jpeg", true)]
        [TestCase(@"Images\CMYK\TestDefect5841.jpeg", true)]
        [TestCase(@"Images\CMYK\TestDefect6728.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira13620.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira5366_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira6368_1.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira6368_2.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira6409_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira7626_1.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJira8459_0.jpeg", true)]
        [TestCase(@"Images\CMYK\TestJiraJ883.jpg", true)]
        [TestCase(@"Images\CMYK\TestShapeColorChanged489_0.jpeg", true)]
        // This image causes problems while reading.
        [TestCase(@"Images\CMYK\TestJira6573_0.jpeg", false)]

        [TestCase(@"Rendering\Aps\TestImages\TestImage Format24bppRgb.jpg", false)]
        [TestCase(@"Rendering\Aps\TestImages\TestImage FormatJpegBaseline.jpg", false)]
        [TestCase(@"Rendering\Aps\TestImages\TestImage FormatJpegOptimizedHuffman.jpg", false)]
        [TestCase(@"Rendering\Aps\TestImages\TestImage FormatJpegProgressive3Scans.jpg", false)]

        [TestCase(@"Images\TestDefect1463 2.jpg", false)]
        [TestCase(@"Images\TestDefect1463.jpg", false)]
        [TestCase(@"Images\TestDefect11932.jpg", false)]
        [TestCase(@"Images\TestDefectJira5142.jpg", false)]
        [TestCase(@"Images\TestJira10356.jpg", false)]
        [TestCase(@"Images\TestJpegSize.jpg", false)]
        [TestCase(@"Images\TestWrongHeight.jpg", false)]

        // WORDSNET-24845 Some images detected as PixelFormat.PixelFormat32bppCMYK in GDI and caused an issue in SkiaSharp.
        [TestCase(@"Images\CMYK\TestPixelFormat32bppCMYK.jpg", true)]
        [TestCase(@"Images\CMYK\Test24845.jpg", true)]
        public void TestIsCmyk(string imageFileName, bool expectedValue)
        {
            string testImageFileName = TestFxUtil.BuildTestFileName(imageFileName);

#if !NETSTANDARD && !JAVA && !CPLUSPLUS // Test using GDI+ to make sure the expected values are correct.
            using (Bitmap bmp = new Bitmap(testImageFileName))
            {
                Assert.That(BitmapPal.IsCmykPixelFormat(bmp), Is.EqualTo(expectedValue));
            }
#endif
            Assert.That(ImageUtil.IsCmykOrYCCK(File.ReadAllBytes(testImageFileName)), Is.EqualTo(expectedValue));
        }
    }
}
